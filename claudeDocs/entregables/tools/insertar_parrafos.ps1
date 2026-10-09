<#
.SYNOPSIS
  Inserta parrafos nuevos en un .docx, cada uno justo despues de un parrafo existente, por Word COM.
  Hermano de reemplazar_parrafos.ps1: mismo patron (-DryRun, copia en %TEMP%, indices, relectura).

.DESCRIPTION
  Formato del .json (UTF-8): una lista de objetos
    [ {"despues_de": "texto completo del parrafo ancla", "texto": "parrafo nuevo", "como": "texto completo del parrafo modelo (opcional)"} ]
  El parrafo nuevo toma el estilo y el formato de parrafo y de caracter del modelo ("como"; por omision, el ancla).
  Los anclas se buscan por texto normalizado completo (sin distinguir mayusculas), cuerpo y celdas; cada uno
  debe aparecer exactamente una vez. Las entradas se aplican en orden, asi que una puede anclarse en el
  texto de la anterior. Si el texto nuevo ya esta en el documento, la entrada se salta (idempotente).
  Cualquier problema aborta sin guardar. No toca parrafos con campos como ancla de formato (usa uno sin campos).

.PARAMETER Docx   El .docx (se cambia en el sitio; copia previa en %TEMP%\Algoritmia-Docx).
.PARAMETER Parrafos  El .json.
.PARAMETER DryRun  Solo comprueba y dice que haria.
.PARAMETER NoActualizarIndices  No actualiza TOC ni listas de tablas y figuras.
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)][string]$Docx,
  [Parameter(Mandatory = $true)][string]$Parrafos,
  [switch]$DryRun,
  [switch]$NoActualizarIndices
)
$ErrorActionPreference = 'Stop'

function Normalize-Text([string]$Text) {
  if ($null -eq $Text) { return '' }
  $t = ($Text -replace '[\r\a\v\f]', ' ').Replace([string][char]0x00A0, ' ')
  ($t -replace '\s+', ' ').Trim()
}
function Find-Para($Doc, [string]$Norm) {
  $hits = @()
  foreach ($p in $Doc.Paragraphs) {
    if ([string]::Equals((Normalize-Text $p.Range.Text), $Norm, [System.StringComparison]::OrdinalIgnoreCase)) { $hits += $p }
  }
  , $hits
}

$docPath = (Resolve-Path -LiteralPath $Docx).Path
$raw = [System.IO.File]::ReadAllText((Resolve-Path -LiteralPath $Parrafos).Path, (New-Object System.Text.UTF8Encoding($false)))
$items = $raw | ConvertFrom-Json
if (@($items).Count -eq 0) { throw 'el .json no trae entradas' }
foreach ($it in $items) {
  if (-not $it.despues_de -or -not $it.texto) { throw 'cada entrada necesita despues_de y texto' }
  if ($it.texto -match '[\r\n\a\v\f]') { throw 'texto no admite saltos de linea' }
}

$backup = $null
if (-not $DryRun) {
  $dir = Join-Path ([System.IO.Path]::GetTempPath()) 'Algoritmia-Docx'
  New-Item -ItemType Directory -Force -Path $dir | Out-Null
  $backup = Join-Path $dir ([System.IO.Path]::GetFileNameWithoutExtension($docPath) + '.' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.antes-insertar.docx')
  Copy-Item -LiteralPath $docPath -Destination $backup
  Write-Host "Copia de seguridad: $backup"
}

$word = $null; $doc = $null; $saved = $false; $code = 0
try {
  $word = New-Object -ComObject Word.Application
  $word.Visible = $false; $word.DisplayAlerts = 0
  $doc = $word.Documents.Open($docPath, $false, [bool]$DryRun, $false)
  if (-not $DryRun -and $doc.ReadOnly) { throw 'Word abrio el documento de solo lectura' }
  $track = [bool]$doc.TrackRevisions
  if ($track -and -not $DryRun) { $doc.TrackRevisions = $false }
  $n = 0
  foreach ($it in $items) {
    $n++
    $nuevoN = Normalize-Text $it.texto
    if ((Find-Para $doc $nuevoN).Count -ge 1) { Write-Host "  entrada ${n}: ya esta, se salta"; continue }
    $anc = Find-Para $doc (Normalize-Text $it.despues_de)
    if ($DryRun -and $anc.Count -eq 0 -and ($items | Where-Object { (Normalize-Text $_.texto) -eq (Normalize-Text $it.despues_de) })) { Write-Host "  entrada ${n}: se ancla en una entrada anterior (no se comprueba en -DryRun)"; continue }
    if ($anc.Count -ne 1) { throw "entrada ${n}: el ancla aparece $($anc.Count) veces (se esperaba 1): [$($it.despues_de.Substring(0, [Math]::Min(60, $it.despues_de.Length)))]" }
    $modelo = $anc[0]
    if ($it.como) {
      $m = Find-Para $doc (Normalize-Text $it.como)
      if ($m.Count -ne 1) { throw "entrada ${n}: el modelo aparece $($m.Count) veces" }
      $modelo = $m[0]
    }
    $est = [string]$modelo.Style.NameLocal
    $mr = $modelo.Range
    $ficha = "estilo [$est], alineacion $($modelo.Alignment), sangria 1a linea $($modelo.FirstLineIndent), fuente $($mr.Font.Name) $($mr.Font.Size) negrita $($mr.Font.Bold)"
    Write-Host "  entrada ${n}: tras [$($it.despues_de.Substring(0, [Math]::Min(50, $it.despues_de.Length)))...] con $ficha"
    if ($DryRun) { continue }
    $a = $anc[0].Range
    $a.InsertParagraphAfter()
    $nuevo = $anc[0].Next(1)    # el parrafo vacio recien creado (OJO: $a.End ya lo incluye)
    if (-not [string]::IsNullOrEmpty((Normalize-Text $nuevo.Range.Text))) { throw "entrada ${n}: el parrafo nuevo no esta vacio" }
    $nuevo.Format = $modelo.Format
    $nuevo.Style = $est
    $nuevo.Format = $modelo.Format
    $r = $nuevo.Range
    $r.MoveEnd(1, -1) | Out-Null
    $r.Text = [string]$it.texto
    $r.Font = $mr.Characters.Item(1).Font.Duplicate
    $r.Font.Bold = 0; $r.Font.Italic = 0
    $got = Normalize-Text $nuevo.Range.Text
    if ($got -cne $nuevoN) { throw "entrada ${n}: quedo distinto de lo pedido" }
  }
  if (-not $DryRun) {
    if (-not $NoActualizarIndices) {
      $doc.Repaginate()
      for ($i = 1; $i -le $doc.TablesOfContents.Count; $i++) { $doc.TablesOfContents.Item($i).Update() }
      for ($i = 1; $i -le $doc.TablesOfFigures.Count; $i++) { $doc.TablesOfFigures.Item($i).Update() }
    }
    if ($track) { $doc.TrackRevisions = $true }
    $doc.Save(); $saved = $true
    Write-Host 'Guardado.'
  } else { Write-Host '-DryRun: no se escribio nada.' }
}
catch { Write-Host ("ERROR: " + $_.Exception.Message) -ForegroundColor Red; Write-Host 'No se guardo nada.'; $code = 1 }
finally {
  if ($null -ne $doc) { if (-not $saved) { try { $doc.Saved = $true } catch {} }; try { $doc.Close($false) } catch {} }
  if ($null -ne $word) { try { $word.Quit($false) } catch {} }
  [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}
exit $code
