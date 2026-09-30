#requires -Version 7.0
<#
.SYNOPSIS
    Envoltorio del Editor de Unity ABIERTO para el cierre OE4: pruebas, compilación, escenas y eval
    por la API HTTP de com.unity.pipeline, sin cerrar el Editor.

.DESCRIPTION
    `unity cmd` no sirve con este Editor (com.unity.pipeline 0.5.0-exp.1; el CLI pide 0.6.0), pero el
    servidor HTTP del paquete sí: POST http://127.0.0.1:<puerto>/api/exec con la cabecera
    `Authorization: Bearer <evalToken>` y el cuerpo {"command":"…","parameters":{…}}. Puerto y token
    salen de Library/Pipeline/.unity-pipeline-port y se RELEEN en cada llamada (una recarga de dominio
    puede cambiar el puerto). El token no se imprime ni se guarda en ninguna parte.

    ANTES DE CADA LLAMADA toca Temp/claude-active: así ClaudeSceneAutosave (Game.EditorTools) guarda
    durante 2 min las escenas que el propio Claude ensucie y no aparece el aviso modal «Scene(s) Have
    Been Modified», que bloquea el hilo principal y con él al puente. Por eso cada corrida de pruebas
    exige antes que no haya escenas sucias (espera unos segundos a que el autoguardado las limpie y, si
    siguen sucias, SE DETIENE sin tocar nada: nunca pulsa el diálogo).

    EditMode va como trabajo desacoplado ("job": true, se sondea GET /api/job?id=). PlayMode va siempre
    asíncrono (async_tests) y se sondea el ARCHIVO Temp/pipeline_test_status.json, porque durante la
    recarga de dominio el HTTP no responde. Las corridas largas conviene lanzarlas en segundo plano.

    Un Editor único: NUNCA dos llamadas a la vez (el servidor serializa los exec, pero dos agentes sobre
    el mismo Editor se pisan la escena activa).

.NOTES
    Códigos de salida: 0 bien · 1 pruebas con fallos, errores de compilación o eval con error ·
    2 fallo de infraestructura (Editor inaccesible, timeout, filtro sin coincidencias, comando
    rechazado) · 3 guarda (escena sucia, Editor no listo o en Play; no se tocó nada).
    Para pruebas, `filter` `-` significa «sin filtro» (un `*` lo expandiría el shell).
    Resultados: <-Out | $env:EDITOR_RUNS_DIR | carpeta por defecto>\<fecha-hora>_<modo>.json (+ .xml).
#>
param(
    [Parameter(Position = 0)][string]$Sub = 'help',
    [Parameter(Position = 1, ValueFromRemainingArguments = $true)][string[]]$Rest = @(),
    [string]$Out,                # carpeta de resultados de las corridas
    [switch]$IncludeExplicit,    # tests-*: incluye las pruebas [Explicit]
    [switch]$FileMode,           # tests-edit: por archivo (async) en vez de trabajo HTTP
    [switch]$NoGameView,         # tests-play: no fijar la Game View a 1920x1080
    [switch]$Raw,                # exec/console/commands: JSON crudo
    [switch]$Force               # open: abrir aunque haya escenas sucias (puede abrir el aviso modal)
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)

# --- Rutas -------------------------------------------------------------------------------------
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
if (-not (Test-Path -LiteralPath (Join-Path $Root 'Assets'))) { $Root = 'C:\Dev\Algoritmia' }
$PortFile = Join-Path $Root 'Library\Pipeline\.unity-pipeline-port'
$Marker = Join-Path $Root 'Temp\claude-active'
$TestStatusFile = Join-Path $Root 'Temp\pipeline_test_status.json'
$TestRequestFile = Join-Path $Root 'Temp\pipeline_test_request.json'
$RecompileFile = Join-Path $Root 'Temp\pipeline_recompile_status.json'
$XmlSource = Join-Path $env:USERPROFILE 'AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia\TestResults.xml'
$BootScene = 'Assets/Game/Scenes/Boot.unity'
# Sesión del cierre (30/09/2026); se sobrescribe con -Out o $env:EDITOR_RUNS_DIR.
$DefaultRunsDir = 'C:\Users\benab\AppData\Local\Temp\claude\c--Dev-Algoritmia\a5dca15e-0f11-4624-ba8a-5eb31dd2d2f0\scratchpad\cierre\runs'
$GameViewCode = 'UnityEditor.PlayModeWindow.SetCustomRenderingResolution(1920, 1080, "Pruebas 1080p"); ' +
                'uint w, h; UnityEditor.PlayModeWindow.GetRenderingResolution(out w, out h); return w + "x" + h;'

function Stop-With([int]$Code, [string]$Message) {
    if ($Message) { [Console]::Error.WriteLine($Message) }
    exit $Code
}

# --- Acceso al servidor ------------------------------------------------------------------------
function Touch-Marker {
    if (-not (Test-Path -LiteralPath (Split-Path $Marker))) { return }   # sin Temp/ el Editor está cerrado
    if (Test-Path -LiteralPath $Marker) { [IO.File]::SetLastWriteTimeUtc($Marker, [DateTime]::UtcNow) }
    else { [IO.File]::Create($Marker).Dispose() }
}

function Get-Descriptor {
    if (-not (Test-Path -LiteralPath $PortFile)) { throw "No existe ${PortFile}: el Editor está cerrado o su servidor no arrancó." }
    $d = $null
    for ($i = 0; $i -lt 5 -and -not $d; $i++) {      # el Editor reescribe el archivo en cada latido
        try { $d = Get-Content -LiteralPath $PortFile -Raw | ConvertFrom-Json } catch { Start-Sleep -Milliseconds 300 }
    }
    if (-not $d -or -not $d.port -or -not $d.evalToken) { throw "No se pudo leer el descriptor $PortFile." }
    if (-not (Get-Process -Id $d.pid -ErrorAction SilentlyContinue)) { throw "El descriptor apunta al pid $($d.pid), que ya no existe: el Editor está cerrado." }
    $d
}

# Una llamada HTTP. Reintenta (cada 4 s, hasta RetrySec) si el servidor no responde —recarga de
# dominio— o contesta 503 «ocupado». Solo para GET o llamadas idempotentes: nunca para run_tests.
function Invoke-Api {
    param([string]$Path, [string]$Method = 'GET', $Body = $null, [int]$TimeoutSec = 60, [int]$RetrySec = 0)
    $deadline = [DateTime]::UtcNow.AddSeconds($RetrySec)
    while ($true) {
        Touch-Marker
        try {
            $d = Get-Descriptor
            $req = @{
                Uri = "http://127.0.0.1:$($d.port)$Path"; Method = $Method; TimeoutSec = $TimeoutSec
                Headers = @{ Authorization = "Bearer $($d.evalToken)" }; SkipHttpErrorCheck = $true
            }
            if ($null -ne $Body) { $req.Body = ($Body | ConvertTo-Json -Depth 20 -Compress); $req.ContentType = 'application/json' }
            $r = Invoke-WebRequest @req
            $text = [Text.Encoding]::UTF8.GetString($r.RawContentStream.ToArray())
            if ([int]$r.StatusCode -eq 503 -and [DateTime]::UtcNow -lt $deadline) { Start-Sleep -Seconds 4; continue }
            $json = $null
            try { $json = $text | ConvertFrom-Json -Depth 100 } catch { }
            return [pscustomobject]@{ Code = [int]$r.StatusCode; Json = $json; Text = $text }
        }
        catch {
            if ([DateTime]::UtcNow -ge $deadline) { throw }
            Start-Sleep -Seconds 4
        }
    }
}

# POST /api/exec. Devuelve el sobre {success, command, result, …}; falla si el sobre no es success.
function Invoke-Cmd {
    param([string]$Name, $Params = @{}, [int]$TimeoutSec = 60, [switch]$Job, [int]$RetrySec = 0)
    $body = [ordered]@{ command = $Name; parameters = $Params; timeout = $TimeoutSec * 1000 }
    if ($Job) { $body.job = $true }
    $r = Invoke-Api -Path '/api/exec' -Method POST -Body $body -TimeoutSec ($TimeoutSec + 20) -RetrySec $RetrySec
    if ($r.Code -ne 200 -or -not $r.Json -or -not $r.Json.success) {
        $why = if ($r.Json) { "$($r.Json.error): $($r.Json.errorDetails)" } else { $r.Text }
        throw "HTTP $($r.Code) en '$Name': $why"
    }
    $r.Json
}

function Get-EditorState {
    $r = Invoke-Api -Path '/api/editor_status' -TimeoutSec 20
    if ($r.Code -ne 200 -or -not $r.Json) { throw "HTTP $($r.Code) en editor_status: $($r.Text)" }
    $r.Json
}

# Dos lecturas «ready» seguidas: entre el fin de una compilación y la recarga de dominio hay un
# instante falso de calma.
function Wait-Ready([int]$TimeoutSec = 180) {
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSec); $ok = 0; $s = $null
    while ([DateTime]::UtcNow -lt $deadline) {
        try { $s = Get-EditorState; $ok = if ($s.status -eq 'ready') { $ok + 1 } else { 0 } } catch { $ok = 0 }
        if ($ok -ge 2) { return $s }
        Start-Sleep -Seconds 2
    }
    throw "El Editor no quedó 'ready' en $TimeoutSec s (último estado: $($s.status))."
}

# --- Escenas -----------------------------------------------------------------------------------
function Get-Scenes { (Invoke-Cmd 'list_open_scenes' -TimeoutSec 30).result }

function Format-Scene($s) {
    '{0} | {1} | activa={2} sucia={3}' -f $s.name, $(if ($s.path) { $s.path } else { '(sin ruta)' }), $s.isActive, $s.isDirty
}

# Ninguna escena sucia: con el marcador fresco ClaudeSceneAutosave limpia en <1 s lo que Claude ensució.
# Una escena sin ruta (Untitled) no la guarda nadie: si está sucia, se detiene.
function Assert-ScenesClean {
    $deadline = [DateTime]::UtcNow.AddSeconds(8)
    while ($true) {
        $sc = Get-Scenes
        $dirty = @($sc.scenes | Where-Object { $_.isDirty })
        if ($dirty.Count -eq 0) { return $sc }
        if ([DateTime]::UtcNow -ge $deadline) {
            Stop-With 3 ("ESCENA SUCIA: " + (($dirty | ForEach-Object { Format-Scene $_ }) -join '; ') +
                ". Seguir abriría el aviso modal «Scene(s) Have Been Modified» y bloquearía el Editor y el puente. " +
                "No se tocó nada: guárdela (saveall) o decida qué hacer con ella.")
        }
        Start-Sleep -Seconds 1
    }
}

function Set-GameView1080 {
    $r = Invoke-Cmd 'eval' @{ code = $GameViewCode; timeout = 30000 } -TimeoutSec 40
    if (-not $r.result.success) { throw "eval de la Game View falló: $($r.result.error) $($r.result.errorDetails)" }
    [string]$r.result.result
}

# --- Pruebas -----------------------------------------------------------------------------------
function Get-Summary($o) {
    [pscustomobject]@{ Total = [int]$o.Total; Passed = [int]$o.Passed; Failed = [int]$o.Failed; Skipped = [int]$o.Skipped; Inconclusive = [int]$o.Inconclusive }
}

# EditMode como trabajo desacoplado: devuelve {status, message, summary, results}.
function Wait-EditModeJob([string]$JobId, [int]$TimeoutSec) {
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSec + 120); $beat = [DateTime]::UtcNow
    while ($true) {
        Start-Sleep -Seconds 2
        $r = Invoke-Api -Path "/api/job?id=$JobId" -TimeoutSec 20 -RetrySec 90
        if ($r.Code -eq 404) { throw "El trabajo $JobId ya no existe: una recarga de dominio lo descartó (los trabajos no sobreviven). Reintente o use -FileMode." }
        $job = $r.Json
        if ($job.state -in 'completed', 'failed', 'canceled') {
            if ($job.state -ne 'completed') { return [pscustomobject]@{ status = 'error'; message = "trabajo $($job.state): $($job.error) $($job.errorDetails)"; summary = (Get-Summary $null); results = @() } }
            $res = $job.result
            if (-not $res.Success) { return [pscustomobject]@{ status = 'error'; message = [string]$res.Error; summary = (Get-Summary $null); results = @() } }
            return [pscustomobject]@{ status = 'completed'; message = $null; summary = (Get-Summary $res.Summary); results = @($res.Results | Where-Object { $null -ne $_ }) }
        }
        if ([DateTime]::UtcNow -ge $deadline) {
            try { $null = Invoke-Cmd 'cancel_tests' -TimeoutSec 30 } catch { }
            throw "Timeout de $TimeoutSec s esperando el trabajo $JobId (se pidió cancel_tests)."
        }
        if (([DateTime]::UtcNow - $beat).TotalSeconds -ge 30) { Write-Host "  … EditMode en curso ($($job.state))"; $beat = [DateTime]::UtcNow }
    }
}

# PlayMode (y EditMode con -FileMode): run_tests asíncrono y sondeo del archivo de estado.
function Wait-TestStatusFile([datetime]$StartUtc, [int]$TimeoutSec) {
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSec + 120); $beat = [DateTime]::UtcNow
    while ($true) {
        Touch-Marker
        if ((Test-Path -LiteralPath $TestStatusFile) -and (Get-Item -LiteralPath $TestStatusFile).LastWriteTimeUtc -ge $StartUtc.AddSeconds(-2)) {
            $st = $null
            try { $st = Get-Content -LiteralPath $TestStatusFile -Raw | ConvertFrom-Json -Depth 100 } catch { }   # escritura a medias
            if ($st -and $st.status -in 'completed', 'error', 'cancelled') {
                return [pscustomobject]@{ status = [string]$st.status; message = [string]$st.message; summary = (Get-Summary $st.summary); results = @($st.results | Where-Object { $null -ne $_ }) }
            }
        }
        if ([DateTime]::UtcNow -ge $deadline) {
            try { $null = Invoke-Cmd 'cancel_tests' -TimeoutSec 30 } catch { }
            throw "Timeout de $TimeoutSec s esperando $TestStatusFile (se pidió cancel_tests). Si el Editor no responde, puede haber un diálogo modal abierto."
        }
        if (([DateTime]::UtcNow - $beat).TotalSeconds -ge 30) { Write-Host '  … corrida en curso'; $beat = [DateTime]::UtcNow }
        Start-Sleep -Seconds 5
    }
}

function Finish-Run($Run) {
    $dir = if ($Out) { $Out } elseif ($env:EDITOR_RUNS_DIR) { $env:EDITOR_RUNS_DIR } else { $DefaultRunsDir }
    $null = New-Item -ItemType Directory -Force -Path $dir
    $base = Join-Path $dir ('{0:yyyy-MM-dd_HHmmss}_{1}' -f $Run.startedUtc.ToLocalTime(), $Run.mode)
    $xml = $null
    if ((Test-Path -LiteralPath $XmlSource) -and (Get-Item -LiteralPath $XmlSource).LastWriteTimeUtc -gt $Run.startedUtc) {
        Copy-Item -LiteralPath $XmlSource -Destination "$base.xml" -Force
        $xml = "$base.xml"
    }
    $finished = [DateTime]::UtcNow
    $s = $Run.summary
    $doc = [ordered]@{
        mode = $Run.mode; filter = $Run.filter; filterType = $Run.filterType
        startedUtc = $Run.startedUtc.ToString('o'); finishedUtc = $finished.ToString('o')
        elapsedSec = [math]::Round(($finished - $Run.startedUtc).TotalSeconds, 1)
        status = $Run.status; error = $Run.message; summary = $s; xml = $xml; results = $Run.results
    }
    [IO.File]::WriteAllText("$base.json", ($doc | ConvertTo-Json -Depth 20), [Text.UTF8Encoding]::new($false))

    Write-Host ("`n{0} | filtro='{1}' ({2}) | {3} s | estado={4}" -f $Run.mode, $Run.filter, $Run.filterType, $doc.elapsedSec, $Run.status)
    Write-Host ("Total {0} | Passed {1} | Failed {2} | Skipped {3} | Inconclusive {4}" -f $s.Total, $s.Passed, $s.Failed, $s.Skipped, $s.Inconclusive)
    if ($Run.results.Count -ne $s.Total) { Write-Host "AVISO: el resumen dice $($s.Total) pruebas y el detalle trae $($Run.results.Count) (fallos de fixture/SetUp no salen en el detalle)." }
    $failed = @($Run.results | Where-Object { $_.Status -eq 'Failed' })
    foreach ($f in ($failed | Select-Object -First 60)) {
        Write-Host ("  FALLA {0}`n        {1}" -f $f.FullName, (([string]$f.Message -split "`n" | Select-Object -First 1).Trim()))
    }
    if ($failed.Count -gt 60) { Write-Host "  … y $($failed.Count - 60) fallos más (en el JSON)." }
    Write-Host "Resultados: $base.json"
    Write-Host $(if ($xml) { "TestResults.xml: $xml" } else { 'TestResults.xml: no se actualizó durante la corrida (no se copia).' })

    if ($Run.status -ne 'completed') { Stop-With 2 "La corrida no terminó bien: $($Run.status) $($Run.message)" }
    if ($s.Total -eq 0) { Stop-With 2 'Se ejecutaron 0 pruebas: el filtro no coincide con nada (compruebe filter_type y el nombre).' }
    if ($s.Failed -gt 0) { exit 1 }
    exit 0
}

function Do-Tests([string]$Mode) {
    $filter = if ($Rest.Count -gt 0 -and $Rest[0] -ne '-') { $Rest[0] } else { '' }
    $ft = if ($Rest.Count -gt 1 -and $Rest[1]) { $Rest[1] } else { 'testName' }
    $to = if ($Rest.Count -gt 2 -and $Rest[2]) { [int]$Rest[2] } elseif ($Mode -eq 'editmode') { 900 } else { 1800 }
    if ($ft -notin 'testName', 'assembly', 'category') { Stop-With 2 "filter_type '$ft' inválido (testName | assembly | category)." }

    $s = Get-EditorState
    if ($s.status -ne 'ready') { Stop-With 3 "El Editor no está 'ready' (estado: $($s.status), playMode: $($s.playMode)). Espere (wait-compile) o deténgalo." }
    if ($Mode -eq 'playmode' -and -not $NoGameView) { Write-Host "Game View: $(Set-GameView1080)" }   # eval puede ensuciar la escena
    $sc = Assert-ScenesClean
    if ($Mode -eq 'playmode' -and -not (@($sc.scenes | Where-Object { $_.isActive -and $_.path -eq $BootScene }))) {
        Write-Host 'La escena activa no es Boot: se abre (está limpia).'
        $null = Invoke-Cmd 'open_scene' @{ path = $BootScene }
        $sc = Assert-ScenesClean
    }
    Write-Host ("Corrida {0} | filtro='{1}' ({2}) | timeout {3} s | escenas: {4}" -f $Mode, $filter, $ft, $to, (($sc.scenes | ForEach-Object { Format-Scene $_ }) -join '; '))

    $p = [ordered]@{ mode = $(if ($Mode -eq 'editmode') { 'editor' } else { 'playmode' }); filter = $filter; filter_type = $ft
                     include_explicit = $IncludeExplicit.IsPresent; async_tests = ($Mode -eq 'playmode' -or $FileMode.IsPresent); timeout = $to }
    $start = [DateTime]::UtcNow
    if ($Mode -eq 'editmode' -and -not $FileMode) {
        $job = Invoke-Cmd 'run_tests' $p -Job
        $res = Wait-EditModeJob $job.result.jobId $to
    }
    else {
        try {
            $r = Invoke-Cmd 'run_tests' $p
            if (-not $r.result.success) { throw "run_tests rechazado: $($r.result.error)" }
        }
        catch {
            # El dominio puede recargarse antes de que llegue la respuesta: manda el archivo de petición.
            if ($_.Exception.Message -like 'run_tests rechazado*' -or -not ((Test-Path -LiteralPath $TestRequestFile) -and (Get-Item -LiteralPath $TestRequestFile).LastWriteTimeUtc -ge $start.AddSeconds(-2))) { throw }
        }
        $res = Wait-TestStatusFile $start $to
    }
    Write-Host 'Esperando a que el Editor vuelva a estar listo…'
    try { $null = Wait-Ready 240 } catch { Write-Host "AVISO: $($_.Exception.Message)" }
    Finish-Run ([pscustomobject]@{ mode = $Mode; filter = $filter; filterType = $ft; startedUtc = $start; status = $res.status; message = $res.message; summary = $res.summary; results = @($res.results) })
}

# --- Compilación -------------------------------------------------------------------------------
function Read-RecompileStatus {
    try { Get-Content -LiteralPath $RecompileFile -Raw | ConvertFrom-Json } catch { [pscustomobject]@{ status = 'idle'; failed = $false; errors = @() } }
}

function Wait-Compile([int]$TimeoutSec = 300) {
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSec); $ok = 0
    Start-Sleep -Seconds 1
    while ([DateTime]::UtcNow -lt $deadline) {
        $f = Read-RecompileStatus
        $idle = $false
        try { $idle = ((Get-EditorState).status -eq 'ready') } catch { }
        $ok = if ($idle -and $f.status -notin 'compiling', 'triggered') { $ok + 1 } else { 0 }
        if ($ok -ge 2) { break }
        Start-Sleep -Seconds 2
    }
    if ($ok -lt 2) { Stop-With 2 "El Editor no terminó de compilar en $TimeoutSec s." }
    $f = Read-RecompileStatus
    if ($f.failed) {
        Write-Host "COMPILACIÓN CON ERRORES ($(@($f.errors).Count)):"
        @($f.errors) | ForEach-Object { Write-Host "  $_" }
        exit 1
    }
    Write-Host "Compilación OK (status=$($f.status), errores=0)."
}

# --- Subcomandos -------------------------------------------------------------------------------
function ConvertTo-Params([string[]]$Tokens) {
    if (-not $Tokens -or $Tokens.Count -eq 0) { return @{} }
    $first = $Tokens[0].Trim()
    if ($first.StartsWith('{')) { return ($Tokens -join ' ' | ConvertFrom-Json -AsHashtable -Depth 50) }
    if ($first.StartsWith('@')) { return (Get-Content -LiteralPath $first.Substring(1) -Raw | ConvertFrom-Json -AsHashtable -Depth 50) }
    $h = [ordered]@{}
    foreach ($t in $Tokens) {
        $i = $t.IndexOf('=')
        if ($i -lt 1) { throw "Parámetro '$t': use clave=valor, un JSON '{…}' o @archivo.json." }
        $v = $t.Substring($i + 1)
        $h[$t.Substring(0, $i)] = if ($v -in 'true', 'false') { [bool]::Parse($v) } elseif ($v -match '^-?\d+$') { [int64]$v } else { $v }
    }
    $h
}

function Show-Help {
    Write-Host @'
editor.ps1 — envoltorio del Editor de Unity abierto (API HTTP de com.unity.pipeline)
Uso: pwsh -NoProfile -File editor.ps1 <subcomando> [argumentos] [-Out <carpeta>] [-IncludeExplicit] [-FileMode] [-NoGameView] [-Raw] [-Force]

  status                           estado del Editor, escenas y última compilación
  scenes                           escenas abiertas (activa / sucia)
  open <ruta.unity>                abre una escena (se niega si hay escenas sucias, salvo -Force)
  saveall                          guarda las escenas sucias
  autotick [on|off]                set_autotick (persist): el Editor procesa sin foco
  gameview1080                     Game View a 1920x1080 (eval) y confirma la resolución
  eval <código|@archivo.cs> [ms]   cuerpo de método C# con `return …;` (usings: System, Linq, UnityEngine, UnityEditor)
  exec <cmd> [{json}|@archivo.json|clave=valor …]   cualquier comando del servidor (ver `commands`)
  commands [texto]                 lista los comandos disponibles
  recompile                        AssetDatabase.Refresh + espera a que compile (errores → salida 1)
  wait-compile [timeoutSec=300]    espera a que el Editor termine de compilar
  tests-edit <filter|-> [filter_type=testName] [timeoutSec=900]    EditMode (trabajo HTTP)
  tests-play <filter|-> [filter_type=testName] [timeoutSec=1800]   PlayMode (asíncrono, sondea el archivo)
  test-status                      lee Temp/pipeline_test_status.json
  console [n=30] [level=log]       últimas líneas de la consola (level: log|warn|error)

filter_type: testName (subcadena de FullName) | assembly (subcadena del assembly) | category.
Salida: 0 bien · 1 fallos/errores · 2 infraestructura · 3 guarda (escena sucia, Editor no listo).
'@
}

try {
    switch ($Sub) {
        'help' { Show-Help }

        'status' {
            $d = Get-Descriptor
            $s = Get-EditorState
            Write-Host ("Editor: {0} | compiling={1} reloading={2} playMode={3} | Unity {4} | pid {5} puerto {6}" -f $s.status, $s.compiling, $s.domainReloadInProgress, $s.playMode, $s.unityVersion, $d.pid, $d.port)
            foreach ($x in (Get-Scenes).scenes) { Write-Host ("Escena: " + (Format-Scene $x)) }
            $f = Read-RecompileStatus
            Write-Host "Compilación: $($f.status) failed=$($f.failed) errores=$(@($f.errors).Count)"
            $age = if (Test-Path -LiteralPath $Marker) { '{0:N0} s' -f ([DateTime]::UtcNow - (Get-Item -LiteralPath $Marker).LastWriteTimeUtc).TotalSeconds } else { 'no existe' }
            Write-Host "Marcador claude-active tocado hace: $age"
        }

        'scenes' {
            $sc = Get-Scenes
            foreach ($x in $sc.scenes) { Write-Host (Format-Scene $x) }
            Write-Host ("{0} abierta(s), {1} sucia(s)" -f $sc.count, @($sc.scenes | Where-Object { $_.isDirty }).Count)
        }

        'open' {
            if ($Rest.Count -lt 1) { Stop-With 2 'Uso: open <ruta.unity>' }
            if (-not $Force) { $null = Assert-ScenesClean }
            $null = Invoke-Cmd 'open_scene' @{ path = $Rest[0] }
            foreach ($x in (Get-Scenes).scenes) { Write-Host (Format-Scene $x) }
        }

        'saveall' { (Invoke-Cmd 'save_all').result | ConvertTo-Json -Depth 10 -Compress | Write-Host }

        'autotick' {
            $on = -not ($Rest.Count -gt 0 -and $Rest[0] -in 'off', 'false', '0')
            Write-Host ((Invoke-Cmd 'set_autotick' @{ enable = $on; interval_ms = 16; persist = $true }).result)
        }

        'gameview1080' {
            $res = Set-GameView1080
            Write-Host "Game View: $res"
            if ($res -ne '1920x1080') { Stop-With 2 "Se esperaba 1920x1080 y la Game View quedó en $res." }
        }

        'eval' {
            if ($Rest.Count -lt 1) { Stop-With 2 'Uso: eval <código|@archivo.cs> [timeoutMs]' }
            $code = if ($Rest[0].StartsWith('@')) { Get-Content -LiteralPath $Rest[0].Substring(1) -Raw } else { $Rest[0] }
            $ms = if ($Rest.Count -gt 1) { [int]$Rest[1] } else { 30000 }
            $e = (Invoke-Cmd 'eval' @{ code = $code; timeout = $ms } -TimeoutSec ([int]($ms / 1000) + 10)).result
            if (-not $e.success) {
                Write-Host "EVAL FALLÓ: $($e.error) $($e.errorDetails)"
                @($e.diagnostics) | ForEach-Object { Write-Host "  $($_.severity) $($_.id) (línea $($_.line)): $($_.message)" }
                exit 1
            }
            if ($e.output) { Write-Host $e.output }
            Write-Host $(if ($e.result -is [string]) { $e.result } else { $e.result | ConvertTo-Json -Depth 10 -Compress })
        }

        'exec' {
            if ($Rest.Count -lt 1) { Stop-With 2 'Uso: exec <cmd> [{json}|@archivo.json|clave=valor …]' }
            $r = Invoke-Cmd $Rest[0] (ConvertTo-Params @($Rest | Select-Object -Skip 1)) -TimeoutSec 120
            if ($Raw) { Write-Host ($r | ConvertTo-Json -Depth 30) }
            else { Write-Host $(if ($r.result -is [string]) { $r.result } else { $r.result | ConvertTo-Json -Depth 30 }) }
        }

        'commands' {
            $q = if ($Rest.Count -gt 0) { '&query=' + [uri]::EscapeDataString($Rest[0]) } else { '' }
            $r = Invoke-Api -Path "/api/commands?detail=compact$q" -TimeoutSec 30
            if ($Raw) { Write-Host $r.Text }
            else { foreach ($c in $r.Json.commands) { Write-Host ("{0,-28} {1}" -f $c.name, $c.description) } }
        }

        'recompile' {
            $r = (Invoke-Cmd 'recompile' @{ focus = $false }).result
            Write-Host "recompile: $($r.status)"
            Wait-Compile
        }

        'wait-compile' { Wait-Compile $(if ($Rest.Count -gt 0) { [int]$Rest[0] } else { 300 }) }

        'tests-edit' { Do-Tests 'editmode' }
        'tests-play' { Do-Tests 'playmode' }

        'test-status' {
            if (-not (Test-Path -LiteralPath $TestStatusFile)) {
                Write-Host $(if (Test-Path -LiteralPath $TestRequestFile) { 'running (hay petición y aún no hay resultados)' } else { 'no_tests' })
                break
            }
            $st = Get-Content -LiteralPath $TestStatusFile -Raw | ConvertFrom-Json -Depth 100
            $s = Get-Summary $st.summary
            Write-Host ("status={0} | Total {1} Passed {2} Failed {3} Skipped {4} | {5}" -f $st.status, $s.Total, $s.Passed, $s.Failed, $s.Skipped, $st.message)
        }

        'console' {
            $n = if ($Rest.Count -gt 0) { [int]$Rest[0] } else { 30 }
            $lv = if ($Rest.Count -gt 1) { $Rest[1] } else { 'log' }
            $r = (Invoke-Cmd 'console' @{ tail = $n; level = $lv }).result
            if ($Raw) { Write-Host ($r | ConvertTo-Json -Depth 10) }
            else {
                foreach ($e in $r.entries) {
                    Write-Host ("[{0:HH:mm:ss}] {1,-7} {2}" -f ([datetime]$e.timestampUtc).ToLocalTime(), $e.level, (([string]$e.message -split "`n")[0]))
                }
            }
        }

        default { Stop-With 2 "Subcomando desconocido '$Sub'. Use: help" }
    }
}
catch { Stop-With 2 "ERROR: $($_.Exception.Message)" }
