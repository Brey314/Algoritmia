<#
.SYNOPSIS
  Aplica al trabajo de grado lo que reemplazar_parrafos.ps1 no alcanza en la edición de rc5: celdas de tabla con
  texto repetido («4 · 0 · 0»), pies con campo SEQ y fragmentos de párrafos con citas (que aborta, para protegerlas),
  y actualiza índices, listas y bibliografía. Solo usa Buscar y Range.Text de celdas: ningún campo CITATION se reescribe.
  De un solo uso; entradas: tesis-rc5-tablas.json y tesis-rc5-fragmentos.json. Aborta sin guardar si algo no aparece.
#>
param([Parameter(Mandatory = $true)][string]$Docx, [switch]$DryRun)
$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $PSCommandPath
$tables = Get-Content -Raw -Encoding UTF8 (Join-Path $here 'tesis-rc5-tablas.json') | ConvertFrom-Json
$fragments = Get-Content -Raw -Encoding UTF8 (Join-Path $here 'tesis-rc5-fragmentos.json') | ConvertFrom-Json
$path = (Resolve-Path -LiteralPath $Docx).Path

function Replace-Once($Range, [string]$Old, [string]$New) {
  $f = $Range.Find
  $f.ClearFormatting(); $f.Replacement.ClearFormatting()
  $ok = $f.Execute($Old, $true, $false, $false, $false, $false, $true, 0, $false, $New, 1)
  if (-not $ok) { throw "No aparece: $Old" }
}

$word = New-Object -ComObject Word.Application
$word.Visible = $false; $word.DisplayAlerts = 0
$doc = $null; $saved = $false
try {
  $doc = $word.Documents.Open($path, $false, [bool]$DryRun, $false)
  $tracking = $doc.TrackRevisions
  if (-not $DryRun) { $doc.TrackRevisions = $false }
  $before = @($doc.Fields | Where-Object { $_.Type -eq 96 }).Count
  foreach ($entry in $tables) {
    $table = $null
    foreach ($t in $doc.Tables) {
      $previous = $t.Range.Paragraphs.Item(1).Previous(1)
      if ($previous -ne $null -and ([string]$previous.Range.Text).Contains($entry.pie)) { $table = $t; $caption = $previous; break }
    }
    if ($null -eq $table) { throw "No hay tabla con el pie: $($entry.pie)" }
    "tabla: $($entry.pie) ($($table.Rows.Count) x $($table.Columns.Count))"
    if ($DryRun) { continue }
    foreach ($cell in $entry.celdas.PSObject.Properties) {
      $row, $column = $cell.Name -split ','
      $table.Cell([int]$row, [int]$column).Range.Text = [string]$cell.Value
    }
    if ($entry.titulo) { Replace-Once $caption.Range $entry.pie $entry.titulo }
  }
  foreach ($fragment in $fragments) {
    "fragmento: $($fragment.antes)"
    if (-not $DryRun) { Replace-Once $doc.Content $fragment.antes $fragment.despues }
  }
  if ($DryRun) { return }
  for ($i = 1; $i -le $doc.TablesOfContents.Count; $i++) { $doc.TablesOfContents.Item($i).Update() }
  for ($i = 1; $i -le $doc.TablesOfFigures.Count; $i++) { $doc.TablesOfFigures.Item($i).Update() }
  foreach ($field in @($doc.Fields | Where-Object { $_.Type -eq 97 })) { [void]$field.Update() }   # bibliografía
  for ($i = 1; $i -le $doc.TablesOfContents.Count; $i++) { $doc.TablesOfContents.Item($i).UpdatePageNumbers() }
  for ($i = 1; $i -le $doc.TablesOfFigures.Count; $i++) { $doc.TablesOfFigures.Item($i).UpdatePageNumbers() }
  $after = @($doc.Fields | Where-Object { $_.Type -eq 96 }).Count
  if ($after -ne $before) { throw "Campos CITATION: $before antes y $after después; no se guarda" }
  $doc.TrackRevisions = $tracking
  $doc.Save(); $saved = $true
  "guardado; CITATION $after"
} finally {
  if ($null -ne $doc) { if (-not $saved) { try { $doc.Saved = $true } catch {} }; try { $doc.Close(0) } catch {} }
  try { $word.Quit(0) } catch {}
}
