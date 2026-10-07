#requires -Version 7.0
<#
.SYNOPSIS
    Suite completa (EditMode + PlayMode) con DOS Editores de Unity en paralelo.
    Norma: claudeDocs/tasks/OE4/NORMA-PRUEBAS.md. Se lanza en segundo plano y se espera el aviso.

.DESCRIPTION
    Carril A, el proyecto: EditMode completa y después la PlayMode de Game.UI, la mitad larga (~20 min).
    Carril B, una copia del proyecto (por defecto C:\Dev\Algoritmia-B): el resto de la PlayMode, un
    assembly por corrida. El Editor de B se abre de nuevo en cada suite porque RiverLevel_RNF05 mide la
    memoria del Editor y solo pasa con uno recién abierto que no haya corrido EditMode.

    Pasos:
    1. Cierra el Editor de la copia y la sincroniza con robocopy /MIR (Assets, Packages, ProjectSettings).
       La primera vez copia también Library, sin Library\Pipeline, para no reimportar todo; esa primera
       copia exige el Editor A cerrado o sin escenas sucias, porque lo cierra.
    2. Abre los dos Editores y espera que estén listos y sin errores de compilación.
    3. Corre los dos carriles a la vez con editor.ps1 ($env:EDITOR_PROYECTO elige el Editor).
    4. Cruza lo ejecutado con list_tests: cada prueba listada debe salir exactamente una vez.

.NOTES
    Salida: 0 todo verde · 1 alguna prueba falla · 2 infraestructura o cobertura incompleta.
    Resultado: <Out>\resumen.md y resumen.json. El detalle por corrida, con el formato de editor.ps1, va en
    <Out>\A y <Out>\B, y el registro en <Out>\registro.txt.
    Deja los dos Editores abiertos. B es desechable: se puede cerrar o borrar la copia cuando se quiera.
#>
param(
    [string]$Copia = 'C:\Dev\Algoritmia-B',
    [string]$Out = (Join-Path $env:TEMP ('Algoritmia-suite\' + (Get-Date -Format 'yyyy-MM-dd_HHmmss'))),
    [int]$TimeoutSec = 3600,
    [switch]$SoloPreparar    # sincroniza la copia, deja los dos Editores listos y sale (corridas filtradas a mano)
)
$ErrorActionPreference = 'Stop'
# editor.ps1 escribe en UTF-8: sin esto, los nombres de prueba con tildes llegan rotos y la cobertura no cuadra.
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$Ed = Join-Path $PSScriptRoot 'editor.ps1'
$null = New-Item -ItemType Directory -Force -Path (Join-Path $Out 'A'), (Join-Path $Out 'B')
$t0 = Get-Date

function Log([string]$m) { $l = '{0:HH:mm:ss} {1}' -f (Get-Date), $m; Write-Host $l; Add-Content -LiteralPath (Join-Path $Out 'registro.txt') $l }
function Fail([string]$m) { Log "ERROR: $m"; exit 2 }

# editor.ps1 contra un proyecto. Devuelve la salida como texto y deja el código de salida en $script:EdCode.
function Ed([string]$Proj, [string[]]$A) {
    $env:EDITOR_PROYECTO = $Proj
    try { $o = & pwsh -NoProfile -File $Ed @A 2>&1 | Out-String } finally { Remove-Item Env:EDITOR_PROYECTO -ErrorAction SilentlyContinue }
    $script:EdCode = $LASTEXITCODE
    $o
}

function Get-EditorProcess([string]$Proj) {
    $p = [regex]::Escape($Proj)
    Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" |
        Where-Object { $_.CommandLine -match "-projectpath\s+`"?$p`"?(\s|$)" -and $_.CommandLine -notmatch 'AssetImportWorker' }
}

function Start-Editor([string]$Proj, [string]$Exe) {
    # Un descriptor viejo (o copiado de A) haría que editor.ps1 hablara con otro Editor.
    Remove-Item -LiteralPath (Join-Path $Proj 'Library\Pipeline\.unity-pipeline-port') -Force -ErrorAction SilentlyContinue
    Start-Process -FilePath $Exe -ArgumentList @('-projectPath', "`"$Proj`"") | Out-Null
}

# -Forzar solo para la copia, que es desechable: el Editor del proyecto nunca se mata.
function Stop-Editor([string]$Proj, [switch]$Forzar) {
    $p = Get-EditorProcess $Proj | Select-Object -First 1
    if (-not $p) { return }
    $null = Ed $Proj @('eval', 'UnityEditor.EditorApplication.Exit(0); return 0;')
    Wait-Process -Id $p.ProcessId -Timeout 180 -ErrorAction SilentlyContinue
    if (Get-Process -Id $p.ProcessId -ErrorAction SilentlyContinue) {
        if (-not $Forzar) { Fail "el Editor de $Proj (pid $($p.ProcessId)) no se cerró en 3 min" }
        Stop-Process -Id $p.ProcessId -Force
        Log "Editor de $Proj cerrado a la fuerza (es la copia desechable)"
    }
}

# Listo = «ready», sin errores de compilación y con el pid del Editor de ESE proyecto.
function Wait-Editor([string]$Proj, [int]$Sec) {
    $deadline = (Get-Date).AddSeconds($Sec)
    while ((Get-Date) -lt $deadline) {
        $s = Ed $Proj @('status')
        $p = Get-EditorProcess $Proj
        if ($script:EdCode -eq 0 -and $p -and $s -match 'Editor: ready' -and $s -match "pid $($p.ProcessId)\b") {
            if ($s -match 'errores=(\d+)' -and [int]$Matches[1] -gt 0) { Fail "el Editor de $Proj tiene $($Matches[1]) errores de compilación" }
            return
        }
        Start-Sleep -Seconds 10
    }
    Fail "el Editor de $Proj no quedó listo en $Sec s"
}

# Con la Library copiada, el Editor de la copia puede quedarse con MonoScripts sin clase: el 02/10/2026 les pasó a
# GameFlowRunner y SceneLoader, Boot perdió esos componentes y nada navegaba (35 fallos en el carril B). Se detectan
# los MonoBehaviour y ScriptableObject sin clase, se reimportan y se vuelve a comprobar.
$ScriptsSinClase = @'
var sb = new System.Text.StringBuilder();
foreach (var g in UnityEditor.AssetDatabase.FindAssets("t:MonoScript", new string[] { "Assets/Game", "Assets/Tests" })) {
  var p = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
  var ms = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(p);
  if (ms != null && ms.GetClass() == null && (ms.text.Contains(": MonoBehaviour") || ms.text.Contains(": ScriptableObject"))) { sb.Append(p).Append('|'); }
}
return sb.ToString();
'@ -replace "`r?`n", ' '

function Get-ScriptsSinClase([string]$Proj) {
    $o = Ed $Proj @('eval', $ScriptsSinClase)
    if ($script:EdCode -ne 0) { Fail "no pude revisar los scripts de ${Proj}: $o" }
    @((($o.Trim() -split "`r?`n")[-1]).Split('|', [StringSplitOptions]::RemoveEmptyEntries) | Where-Object { $_ -like 'Assets/*' })
}

function Repair-Scripts([string]$Proj) {
    $rotos = Get-ScriptsSinClase $Proj
    if (-not $rotos.Count) { return }
    Log ("{0}: {1} script(s) sin clase, se reimportan: {2}" -f $Proj, $rotos.Count, ($rotos -join ', '))
    $imp = ($rotos | ForEach-Object { "UnityEditor.AssetDatabase.ImportAsset(`"$_`", UnityEditor.ImportAssetOptions.ForceUpdate);" }) -join ' '
    $null = Ed $Proj @('eval', "$imp return `"ok`";")
    Start-Sleep -Seconds 5
    $null = Ed $Proj @('wait-compile')
    Wait-Editor $Proj 600
    $quedan = Get-ScriptsSinClase $Proj
    if ($quedan.Count) { Fail "$Proj sigue con scripts sin clase: $($quedan -join ', ')" }
}

function Sync-Copy {
    foreach ($d in 'Assets', 'Packages', 'ProjectSettings') {
        robocopy (Join-Path $Root $d) (Join-Path $Copia $d) /MIR /MT:16 /R:2 /W:2 /NFL /NDL /NJH /NJS /NP | Out-Null
        if ($LASTEXITCODE -ge 8) { Fail "robocopy de $d falló (código $LASTEXITCODE)" }
    }
}

# --- 0. Procedencia y Editor de Unity -----------------------------------------------------------------
$head = (git -C $Root rev-parse --short HEAD).Trim()
$huella = ((git -C $Root status --porcelain=v1 | Out-String) | ForEach-Object { [BitConverter]::ToString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($_))).Replace('-', '').Substring(0, 12).ToLower() })
$ver = ((Get-Content (Join-Path $Root 'ProjectSettings\ProjectVersion.txt')) -match '^m_EditorVersion:')[0].Split(':')[1].Trim()
$exe = (Get-EditorProcess $Root | Select-Object -First 1).ExecutablePath
if (-not $exe) {
    $exe = Get-ChildItem 'C:\Program Files\Unity\Hub\Editor' -Directory | Where-Object Name -like "$ver*" | Sort-Object Name -Descending |
        ForEach-Object { Join-Path $_.FullName 'Editor\Unity.exe' } | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
}
if (-not $exe) { Fail "no encuentro Unity.exe $ver" }
Log "Suite en 2 Editores | HEAD $head + árbol $huella | Unity $ver | A=$Root | B=$Copia | salida $Out"

# --- 1. Copia ---------------------------------------------------------------------------------------
Stop-Editor $Copia -Forzar
if (-not (Test-Path -LiteralPath (Join-Path $Copia 'Library'))) {
    if (Get-EditorProcess $Root) {
        $sc = Ed $Root @('scenes')
        if ($sc -notmatch '\b0 sucia') { Fail 'la primera copia necesita cerrar el Editor A y tiene escenas sin guardar' }
        Log 'Primera copia: cierro el Editor A para copiar Library sin que la esté escribiendo'
        Stop-Editor $Root
    }
    Log 'Primera copia de Library (una sola vez; evita reimportar todo)'
    robocopy (Join-Path $Root 'Library') (Join-Path $Copia 'Library') /MIR /MT:16 /R:2 /W:2 /XD Pipeline /NFL /NDL /NJH /NJS /NP | Out-Null
    if ($LASTEXITCODE -ge 8) { Fail "robocopy de Library falló (código $LASTEXITCODE)" }
}
Sync-Copy
Log 'Copia sincronizada'

# --- 2. Editores ------------------------------------------------------------------------------------
if (-not (Get-EditorProcess $Root)) { Log 'Abro el Editor A'; Start-Editor $Root $exe }
Log 'Abro el Editor B'; Start-Editor $Copia $exe
Wait-Editor $Root 900; Log 'Editor A listo'
Wait-Editor $Copia 1800; Log 'Editor B listo'
# El Editor A sin foco no ve lo editado desde fuera: se refresca para que compile antes de probar.
$null = Ed $Root @('eval', 'UnityEditor.AssetDatabase.Refresh(); return "ok";')
Start-Sleep -Seconds 5
$null = Ed $Root @('wait-compile')
Wait-Editor $Root 600
Repair-Scripts $Root
Repair-Scripts $Copia
Log 'Scripts revisados en los dos Editores'
if ($SoloPreparar) { Log "Preparado: el Editor B se usa con `$env:EDITOR_PROYECTO='$Copia'"; exit 0 }

$lista = (Ed $Root @('exec', 'list_tests', 'mode=all', '-Raw')) | Out-String
if ($script:EdCode -ne 0) { Fail "list_tests falló: $lista" }
$tests = @((($lista.Substring($lista.IndexOf('{'))) | ConvertFrom-Json -Depth 50).result.Tests | Where-Object { -not $_.Explicit })
$esperadas = @{}; foreach ($t in $tests) { $esperadas[$t.FullName] = $t.Mode }
Log ("list_tests: {0} pruebas ({1} EditMode, {2} PlayMode)" -f $tests.Count, @($tests | Where-Object Mode -eq 'EditMode').Count, @($tests | Where-Object Mode -eq 'PlayMode').Count)

# --- 3. Carriles en paralelo ------------------------------------------------------------------------
$asmB = Get-ChildItem (Join-Path $Root 'Assets\Tests\PlayMode') -Recurse -Filter *.asmdef |
    ForEach-Object { (Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json).name } |
    Where-Object { $_ -ne 'Game.UI.PlayMode.Tests' } | Sort-Object
$carril = {
    param($Ed, $Proj, $Dir, $Corridas)
    $env:EDITOR_PROYECTO = $Proj
    foreach ($c in $Corridas) {
        $argv = [string[]]@($c)   # Start-Job deserializa los arreglos anidados como ArrayList
        "$(Get-Date -Format HH:mm:ss) >>> $($argv -join ' ')" | Out-File -Append -LiteralPath (Join-Path $Dir 'salida.txt')
        & pwsh -NoProfile -File $Ed @argv -Out $Dir *>&1 | Out-File -Append -LiteralPath (Join-Path $Dir 'salida.txt')
        [pscustomobject]@{ corrida = ($argv -join ' '); codigo = $LASTEXITCODE }
    }
}
$corridasA = @(, @('tests-edit', '-', 'testName', '900')) + @(, @('tests-play', 'Game.UI.PlayMode.Tests', 'assembly', "$TimeoutSec"))
$corridasB = @($asmB | ForEach-Object { , @('tests-play', $_, 'assembly', "$TimeoutSec") })
Log ("Carril A: EditMode + Game.UI.PlayMode.Tests | Carril B: {0}" -f ($asmB -join ', '))
$jA = Start-Job -Name 'carril-A' -ScriptBlock $carril -ArgumentList $Ed, $Root, (Join-Path $Out 'A'), $corridasA
$jB = Start-Job -Name 'carril-B' -ScriptBlock $carril -ArgumentList $Ed, $Copia, (Join-Path $Out 'B'), $corridasB
$null = Wait-Job $jA, $jB -Timeout ($TimeoutSec + 1800)
$codigos = @(Receive-Job $jA) + @(Receive-Job $jB)
$finA = $jA.PSEndTime; $finB = $jB.PSEndTime
Remove-Job $jA, $jB -Force

# --- 4. Resumen y cobertura -------------------------------------------------------------------------
$runs = foreach ($l in 'A', 'B') {
    Get-ChildItem (Join-Path $Out $l) -Filter '*.json' | Sort-Object Name | ForEach-Object {
        $j = Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json -Depth 50
        [pscustomobject]@{ carril = $l; archivo = $_.Name; modo = $j.mode; filtro = $j.filter; seg = $j.elapsedSec; s = $j.summary; results = @($j.results) }
    }
}
$vistas = @{}; $dup = @()
foreach ($r in $runs) { foreach ($t in $r.results) { if ($vistas.ContainsKey($t.FullName)) { $dup += $t.FullName } else { $vistas[$t.FullName] = $t.Status } } }
$faltan = @($esperadas.Keys | Where-Object { -not $vistas.ContainsKey($_) } | Sort-Object)
$fallos = @($runs | ForEach-Object { $_.results } | Where-Object Status -eq 'Failed')
$infra = @($codigos | Where-Object { $_.codigo -notin 0, 1 })
$tot = { param($m) $x = @($runs | Where-Object modo -eq $m); [pscustomobject]@{ Total = ($x.s.Total | Measure-Object -Sum).Sum; Passed = ($x.s.Passed | Measure-Object -Sum).Sum; Failed = ($x.s.Failed | Measure-Object -Sum).Sum; Skipped = ($x.s.Skipped | Measure-Object -Sum).Sum } }
$em = & $tot 'editmode'; $pm = & $tot 'playmode'
$pared = [math]::Round(((Get-Date) - $t0).TotalMinutes, 1)

$md = [Collections.Generic.List[string]]::new()
$md.Add("# Suite en dos Editores — $(Get-Date -Format 'dd/MM/yyyy HH:mm')"); $md.Add('')
$md.Add("HEAD ``$head`` + árbol ``$huella`` · Unity $ver · pared **$pared min** (carril A hasta $('{0:HH:mm}' -f $finA), carril B hasta $('{0:HH:mm}' -f $finB))"); $md.Add('')
$md.Add('| Carril | Corrida | Total | Pasan | Fallan | Omitidas | s |'); $md.Add('|---|---|---|---|---|---|---|')
foreach ($r in $runs) { $md.Add("| $($r.carril) | $($r.modo) $($r.filtro) | $($r.s.Total) | $($r.s.Passed) | $($r.s.Failed) | $($r.s.Skipped) | $($r.seg) |") }
$md.Add(''); $md.Add("**EditMode** $($em.Total) = $($em.Passed) + $($em.Skipped) omitidas, $($em.Failed) fallos · **PlayMode** $($pm.Total) = $($pm.Passed) + $($pm.Skipped) omitidas, $($pm.Failed) fallos")
$md.Add("**Cobertura** (list_tests, sin [Explicit]): $($esperadas.Count) listadas · $($vistas.Count) ejecutadas · faltan $($faltan.Count) · repetidas $($dup.Count)")
foreach ($f in $faltan | Select-Object -First 30) { $md.Add("- falta: ``$f``") }
foreach ($f in $dup | Select-Object -First 30) { $md.Add("- repetida: ``$f``") }
foreach ($c in $infra) { $md.Add("- corrida con código $($c.codigo) (infraestructura o guarda): ``$($c.corrida)``") }
if ($fallos.Count) { $md.Add(''); $md.Add('**Fallos**'); foreach ($f in $fallos) { $md.Add("- ``$($f.FullName)``: $((([string]$f.Message) -split "`n" | Select-Object -First 1).Trim())") } }
[IO.File]::WriteAllLines((Join-Path $Out 'resumen.md'), $md, [Text.UTF8Encoding]::new($false))
[ordered]@{ head = $head; arbol = $huella; unity = $ver; paredMin = $pared; editmode = $em; playmode = $pm; listadas = $esperadas.Count; ejecutadas = $vistas.Count
    faltan = $faltan; repetidas = $dup; corridas = @($runs | Select-Object carril, archivo, modo, filtro, seg, s); fallos = @($fallos | Select-Object FullName, Message); codigos = $codigos } |
    ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $Out 'resumen.json') -Encoding utf8NoBOM

Log ("FIN en $pared min | EditMode {0}/{1} | PlayMode {2}/{3} | fallos {4} | faltan {5} | repetidas {6} | resumen: {7}" -f `
        $em.Passed, $em.Total, $pm.Passed, $pm.Total, $fallos.Count, $faltan.Count, $dup.Count, (Join-Path $Out 'resumen.md'))
if ($infra.Count -or $faltan.Count -or $dup.Count) { exit 2 }
if ($fallos.Count) { exit 1 }
exit 0
