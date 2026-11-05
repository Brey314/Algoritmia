#requires -Version 7.0
<#
.SYNOPSIS
    Lista de huellas de un candidato del OE4: identifica la carpeta entregable entera, no solo el lanzador.
    No es código del juego.

.DESCRIPTION
    -Generar   Escribe un TSV con ruta, bytes y SHA-256 de cada archivo, sin Datos/, más la huella global.
    -Verificar Compara la carpeta con el TSV. Sale con 0 si coincide y con 3 si algo cambió, falta o sobra.
               Datos/ se ignora: la crea el juego al jugar. También un archivo llamado Datos, que S-DOC usa para
               comprobar la ruta de respaldo cuando esa carpeta no se puede crear.

    La huella global usa el algoritmo de evidencias/build-rc*.md, para que se pueda cotejar con esos registros:
    desde la carpeta, en Git Bash,
      find . -type f ! -path './Datos/*' ! -path './*_DoNotShip/*' | LC_ALL=C sort | tr '\n' '\0' | xargs -0 sha256sum | sha256sum
    Los archivos de *_DoNotShip van en la lista, marcados «no» en la columna en_huella, porque viajan en el paquete
    pero no entran a la huella ni a la medida de RNF-06.

    Con -Generar, -HuellaEsperada y -HuellaDataEsperada detienen la escritura si los valores no coinciden.

.EXAMPLE
    pwsh -NoProfile -File manifiesto.ps1 -Generar -Carpeta Build\Algoritmia_rc5 -Salida evidencias\rc5\manifiesto-rc5.tsv -Candidato rc5 -Commit ca6fd91 -Zip Build\Algoritmia.zip
    pwsh -NoProfile -File manifiesto.ps1 -Verificar -Carpeta Build\Algoritmia_rc5 -Manifiesto evidencias\rc5\manifiesto-rc5.tsv

    Salidas: 0 coincide · 2 argumento o archivo inválido · 3 la carpeta no coincide o una huella esperada falló.
#>
[CmdletBinding(DefaultParameterSetName = 'Verificar')]
param(
    [Parameter(ParameterSetName = 'Generar', Mandatory)][switch]$Generar,
    [Parameter(ParameterSetName = 'Verificar', Mandatory)][switch]$Verificar,
    [Parameter(Mandatory)][string]$Carpeta,
    [Parameter(ParameterSetName = 'Generar', Mandatory)][string]$Salida,
    [Parameter(ParameterSetName = 'Verificar', Mandatory)][string]$Manifiesto,
    [Parameter(ParameterSetName = 'Generar')][string]$Candidato = '',
    [Parameter(ParameterSetName = 'Generar')][string]$Commit = '',
    [Parameter(ParameterSetName = 'Generar')][string]$Zip,
    [Parameter(ParameterSetName = 'Generar')][string]$HuellaEsperada,
    [Parameter(ParameterSetName = 'Generar')][string]$HuellaDataEsperada
)

$ErrorActionPreference = 'Stop'
$Utf8 = [Text.UTF8Encoding]::new($false)

function Get-HashTexto([string]$Texto) {
    [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($Utf8.GetBytes($Texto))).ToLowerInvariant()
}

function Get-Archivos([string]$Raiz) {
    $base = (Resolve-Path -LiteralPath $Raiz).Path.TrimEnd('\', '/')
    foreach ($f in [IO.Directory]::EnumerateFiles($base, '*', [IO.SearchOption]::AllDirectories)) {
        $ruta = $f.Substring($base.Length + 1).Replace('\', '/')
        # «Datos» como archivo es la precondición del paso 11 de S-DOC: fuerza la ruta de respaldo (INC-34).
        if ($ruta -ieq 'Datos' -or $ruta.StartsWith('Datos/', [StringComparison]::OrdinalIgnoreCase)) { continue }
        [pscustomobject]@{ Ruta = $ruta; Completa = $f }
    }
}

function Test-EnHuella([string]$Ruta) { $Ruta -notmatch '^[^/]*_DoNotShip/' }

# Reproduce «sha256sum» sobre el listado ordenado por bytes: «<hash> *./<ruta>», una línea por archivo.
function Get-HuellaGlobal($Filas) {
    $enHuella = @($Filas | Where-Object { Test-EnHuella $_.Ruta })
    $porRuta = @{}
    foreach ($f in $enHuella) { $porRuta["./$($f.Ruta)"] = $f.Sha }
    $ordenadas = [string[]]@($porRuta.Keys)
    [Array]::Sort($ordenadas, [StringComparer]::Ordinal)
    $sb = [Text.StringBuilder]::new()
    foreach ($r in $ordenadas) { [void]$sb.Append("$($porRuta[$r]) *$r`n") }
    Get-HashTexto $sb.ToString()
}

# La de Algoritmia_Data de build-rc2.md y siguientes: líneas «./<ruta><TAB><hash>» ordenadas por bytes.
function Get-HuellaData($Filas) {
    $lineas = [string[]]@($Filas | Where-Object { $_.Ruta -like 'Algoritmia_Data/*' } | ForEach-Object { "./$($_.Ruta.Substring('Algoritmia_Data/'.Length))`t$($_.Sha)" })
    [Array]::Sort($lineas, [StringComparer]::Ordinal)
    $sb = [Text.StringBuilder]::new()
    foreach ($l in $lineas) { [void]$sb.Append("$l`n") }
    Get-HashTexto $sb.ToString()
}

if (-not (Test-Path -LiteralPath $Carpeta -PathType Container)) { Write-Output "No existe la carpeta: $Carpeta"; exit 2 }

if ($Generar) {
    $filas = @(Get-Archivos $Carpeta | ForEach-Object {
            $i = [IO.FileInfo]::new($_.Completa)
            [pscustomobject]@{ Ruta = $_.Ruta; Bytes = $i.Length; Sha = (Get-FileHash -LiteralPath $_.Completa -Algorithm SHA256).Hash.ToLowerInvariant() }
        })
    $huella = Get-HuellaGlobal $filas
    $data = Get-HuellaData $filas
    if ($HuellaEsperada -and $huella -ne $HuellaEsperada.ToLowerInvariant()) { Write-Output "Huella global $huella distinta de la esperada $HuellaEsperada. No se escribió nada."; exit 3 }
    if ($HuellaDataEsperada -and $data -ne $HuellaDataEsperada.ToLowerInvariant()) { Write-Output "Huella de Algoritmia_Data $data distinta de la esperada $HuellaDataEsperada. No se escribió nada."; exit 3 }

    $orden = [string[]]@($filas | ForEach-Object { $_.Ruta })
    [Array]::Sort($orden, [StringComparer]::Ordinal)
    $porRuta = @{}
    foreach ($f in $filas) { $porRuta[$f.Ruta] = $f }
    $nHuella = @($filas | Where-Object { Test-EnHuella $_.Ruta }).Count
    $bytesHuella = ($filas | Where-Object { Test-EnHuella $_.Ruta } | Measure-Object Bytes -Sum).Sum

    $sal = [Collections.Generic.List[string]]::new()
    $sal.Add("# Lista de huellas del candidato $Candidato (commit $Commit). Generada con herramientas/manifiesto.ps1.")
    $sal.Add("# huella_global`t$huella")
    $sal.Add("# huella_algoritmia_data`t$data")
    $sal.Add("# archivos`t$($filas.Count)")
    $sal.Add("# archivos_en_huella`t$nHuella")
    $sal.Add("# bytes_en_huella`t$bytesHuella")
    if ($Zip) { $sal.Add("# sha256_zip`t$((Get-FileHash -LiteralPath $Zip -Algorithm SHA256).Hash.ToLowerInvariant())`t$([IO.Path]::GetFileName($Zip))") }
    $sal.Add("ruta`tbytes`tsha256`ten_huella")
    foreach ($r in $orden) {
        $f = $porRuta[$r]
        $sal.Add("$($f.Ruta)`t$($f.Bytes)`t$($f.Sha)`t$(if (Test-EnHuella $f.Ruta) { 'si' } else { 'no' })")
    }
    [IO.File]::WriteAllText([IO.Path]::GetFullPath($Salida), (($sal -join "`n") + "`n"), $Utf8)
    Write-Output "Manifiesto escrito: $Salida · $($filas.Count) archivos · $nHuella en la huella"
    Write-Output "Huella global: $huella"
    Write-Output "Huella de Algoritmia_Data: $data"
    exit 0
}

if (-not (Test-Path -LiteralPath $Manifiesto -PathType Leaf)) { Write-Output "No existe el manifiesto: $Manifiesto"; exit 2 }
$esperados = @{}
$huellaDeclarada = ''
foreach ($l in [IO.File]::ReadAllLines($Manifiesto, $Utf8)) {
    if ($l.StartsWith('# huella_global')) { $huellaDeclarada = ($l -split "`t")[1]; continue }
    if ($l.StartsWith('#') -or $l.StartsWith("ruta`t") -or -not $l) { continue }
    $c = $l -split "`t"
    $esperados[$c[0]] = [pscustomobject]@{ Bytes = [long]$c[1]; Sha = $c[2] }
}
if ($esperados.Count -eq 0 -or -not $huellaDeclarada) { Write-Output "Manifiesto sin filas o sin huella global: $Manifiesto"; exit 2 }

$problemas = [Collections.Generic.List[string]]::new()
$filas = [Collections.Generic.List[object]]::new()
foreach ($a in (Get-Archivos $Carpeta)) {
    $sha = (Get-FileHash -LiteralPath $a.Completa -Algorithm SHA256).Hash.ToLowerInvariant()
    $filas.Add([pscustomobject]@{ Ruta = $a.Ruta; Sha = $sha })
    if (-not $esperados.ContainsKey($a.Ruta)) { $problemas.Add("SOBRA     $($a.Ruta)"); continue }
    if ($esperados[$a.Ruta].Sha -ne $sha) { $problemas.Add("CAMBIÓ    $($a.Ruta)") }
}
$presentes = @{}
foreach ($f in $filas) { $presentes[$f.Ruta] = $true }
foreach ($r in $esperados.Keys) { if (-not $presentes.ContainsKey($r)) { $problemas.Add("FALTA     $r") } }

if ($problemas.Count -gt 0) {
    $problemas | Sort-Object | ForEach-Object { Write-Output $_ }
    Write-Output "Manifiesto NO coincide: $($problemas.Count) diferencia(s) contra $Manifiesto"
    exit 3
}
$huella = Get-HuellaGlobal $filas
if ($huella -ne $huellaDeclarada) { Write-Output "Huella global $huella distinta de la declarada $huellaDeclarada"; exit 3 }
Write-Output "Manifiesto coincide: $($filas.Count) archivos · huella global $huella"
exit 0
