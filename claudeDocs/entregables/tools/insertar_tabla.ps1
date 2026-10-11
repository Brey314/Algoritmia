<#
.SYNOPSIS
  Inserta tablas nuevas en un .docx, cada una justo despues de un parrafo existente, por Word COM.
  Hermano de insertar_parrafos.ps1: mismo patron (-DryRun, copia en %TEMP%, indices, relectura, aborta sin guardar).

.DESCRIPTION
  Formato del .json (UTF-8): una lista de objetos
    [ {"despues_de": "texto completo del parrafo ancla",
       "titulo": "Metas del cuarto objetivo",            # el pie sera «Tabla N. <titulo>», N con campo SEQ
       "modelo": "Metas del tercer objetivo",             # titulo del pie de una tabla ya existente
       "filas": [["Encabezado 1","Encabezado 2"],["a","b"]],   # la primera fila es el encabezado
       "anchos": [40,60],                                 # opcional: % por columna, suma 100
       "fuente": "Fuente: elaboracion propia ..."} ]      # opcional: linea bajo la tabla
  El bloque (pie, tabla, fuente) copia del modelo el estilo del pie (con su campo SEQ), la tabla entera
  (bordes, sombreado, encabezado que se repite, tamanos) y el formato de la linea «Fuente:». El modelo debe
  tener las mismas columnas que "filas". Se agregan o se quitan filas hasta igualar. Cada ancla y cada modelo
  deben aparecer exactamente una vez. Idempotente: si ya hay una tabla con ese pie, la entrada se salta.
  Cualquier problema aborta sin guardar.

.PARAMETER Docx   El .docx (se cambia en el sitio; copia previa en %TEMP%\Algoritmia-Docx).
.PARAMETER Tablas El .json.
.PARAMETER DryRun Solo comprueba y dice que haria.
.PARAMETER NoActualizarIndices  No actualiza TOC ni listas de tablas y figuras.
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)][string]$Docx,
  [Parameter(Mandatory = $true)][string]$Tablas,
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
# Tablas del cuerpo cuyo pie (el parrafo anterior) es «Tabla N. <titulo>».
function Find-Tabla($Doc, [string]$Titulo) {
  $hits = @()
  foreach ($t in $Doc.Tables) {
    $cap = $t.Range.Paragraphs.Item(1).Previous(1)
    if ($null -eq $cap) { continue }
    $c = Normalize-Text $cap.Range.Text
    if ($c -match '^Tabla \d+\. ' -and $c.EndsWith('. ' + (Normalize-Text $Titulo), [System.StringComparison]::OrdinalIgnoreCase)) {
      $hits += [pscustomobject]@{ Tabla = $t; Pie = $cap }
    }
  }
  , $hits
}

$docPath = (Resolve-Path -LiteralPath $Docx).Path
$raw = [System.IO.File]::ReadAllText((Resolve-Path -LiteralPath $Tablas).Path, (New-Object System.Text.UTF8Encoding($false)))
$items = $raw | ConvertFrom-Json
if (@($items).Count -eq 0) { throw 'el .json no trae entradas' }
foreach ($it in $items) {
  if (-not $it.despues_de -or -not $it.titulo -or -not $it.modelo -or @($it.filas).Count -lt 2) { throw 'cada entrada necesita despues_de, titulo, modelo y al menos 2 filas' }
  $w = @($it.filas[0]).Count
  foreach ($f in $it.filas) {
    if (@($f).Count -ne $w) { throw "[$($it.titulo)] filas de distinto ancho" }
    foreach ($c in $f) { if ([string]$c -match '[\r\n\a\v\f]') { throw "[$($it.titulo)] una celda trae saltos de linea" } }
  }
  if ($it.anchos -and ((@($it.anchos).Count -ne $w) -or (($it.anchos | Measure-Object -Sum).Sum -ne 100))) { throw "[$($it.titulo)] anchos: $w valores que sumen 100" }
}

$backup = $null
if (-not $DryRun) {
  $dir = Join-Path ([System.IO.Path]::GetTempPath()) 'Algoritmia-Docx'
  New-Item -ItemType Directory -Force -Path $dir | Out-Null
  $backup = Join-Path $dir ([System.IO.Path]::GetFileNameWithoutExtension($docPath) + '.' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.antes-tabla.docx')
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
    if ((Find-Tabla $doc $it.titulo).Count -ge 1) { Write-Host "  entrada ${n}: ya esta, se salta"; continue }
    $anc = Find-Para $doc (Normalize-Text $it.despues_de)
    if ($anc.Count -ne 1) {
      if ($DryRun -and $anc.Count -eq 0) { Write-Host "  entrada ${n}: ancla no encontrada (en -DryRun es normal si aun no se insertaron los parrafos): [$($it.despues_de.Substring(0, [Math]::Min(50, $it.despues_de.Length)))]" }
      else { throw "entrada ${n}: el ancla aparece $($anc.Count) veces (se esperaba 1)" }
    }
    $m = Find-Tabla $doc $it.modelo
    if ($m.Count -ne 1) { throw "entrada ${n}: el modelo [$($it.modelo)] aparece $($m.Count) veces" }
    $mt = $m[0].Tabla; $mcap = $m[0].Pie
    $cols = @($it.filas[0]).Count
    if (-not $mt.Uniform -or $mt.Columns.Count -ne $cols) { throw "entrada ${n}: el modelo tiene $($mt.Columns.Count) columnas y no es uniforme, o no coincide con las $cols pedidas" }
    $seq = $null
    foreach ($fl in $mcap.Range.Fields) { if ($fl.Type -eq 12) { $seq = ([string]$fl.Code.Text).Trim(); break } }
    if (-not $seq) { throw "entrada ${n}: el pie del modelo no tiene campo SEQ" }
    $er = $mt.Range; $er.Collapse(0); $mf = $er.Paragraphs.Item(1)
    if ((Normalize-Text $mf.Range.Text) -notlike 'Fuente:*') { throw "entrada ${n}: tras el modelo no hay una linea «Fuente:»" }
    Write-Host ("  entrada {0}: [{1}] {2} filas x {3} columnas; pie [{4}] estilo [{5}] campo [{6}]; modelo {7} filas" -f $n, $it.titulo, @($it.filas).Count, $cols, $mcap.Range.Text.Trim(), $mcap.Style.NameLocal, $seq, $mt.Rows.Count)
    if ($DryRun) { continue }

    $a = $anc[0].Range
    $a.InsertParagraphAfter()
    $capP = $anc[0].Next(1)
    if (-not [string]::IsNullOrEmpty((Normalize-Text $capP.Range.Text))) { throw "entrada ${n}: el parrafo del pie no esta vacio" }
    # 1. pie «Tabla <SEQ>. titulo»
    $capP.Format = $mcap.Format; $capP.Style = [string]$mcap.Style.NameLocal; $capP.Format = $mcap.Format
    $r = $capP.Range; $r.MoveEnd(1, -1) | Out-Null; $r.Text = 'Tabla '
    $r.Collapse(0)
    $doc.Fields.Add($r, -1, $seq, $false) | Out-Null
    $r2 = $capP.Range; $r2.MoveEnd(1, -1) | Out-Null; $r2.InsertAfter('. ' + [string]$it.titulo)
    $mr = $mcap.Range
    $capP.Range.Font = $mr.Characters.Item($mr.Characters.Count - 1).Font.Duplicate
    # 2. tabla copiada del modelo, en un parrafo vacio bajo el pie
    $capP.Range.InsertParagraphAfter()
    $tp = $capP.Next(1)
    $dest = $tp.Range; $dest.Collapse(1)
    $dest.FormattedText = $mt.Range.FormattedText
    $nt = $capP.Next(1).Range.Tables.Item(1)
    $need = @($it.filas).Count
    while ($nt.Rows.Count -gt $need) { $nt.Rows.Item($nt.Rows.Count).Delete() }
    while ($nt.Rows.Count -lt $need) { $nt.Rows.Add() | Out-Null }
    for ($i = 1; $i -le $need; $i++) { for ($j = 1; $j -le $cols; $j++) { $nt.Cell($i, $j).Range.Text = [string]$it.filas[$i - 1][$j - 1] } }
    if ($it.anchos) { for ($j = 1; $j -le $cols; $j++) { $c = $nt.Columns.Item($j); $c.PreferredWidthType = 2; $c.PreferredWidth = [double]$it.anchos[$j - 1] } }
    # 3. linea «Fuente:» en el parrafo vacio que queda tras la tabla
    $er2 = $nt.Range; $er2.Collapse(0); $fp = $er2.Paragraphs.Item(1)
    if (-not [string]::IsNullOrEmpty((Normalize-Text $fp.Range.Text))) { throw "entrada ${n}: el parrafo tras la tabla no esta vacio" }
    if ($it.fuente) {
      $fp.Format = $mf.Format; $fp.Style = [string]$mf.Style.NameLocal; $fp.Format = $mf.Format
      $fr = $fp.Range; $fr.MoveEnd(1, -1) | Out-Null; $fr.Text = [string]$it.fuente
      $fr.Font = $mf.Range.Characters.Item(1).Font.Duplicate; $fr.Font.Bold = 0; $fr.Font.Italic = 0
    } else { $fp.Range.Delete() | Out-Null }
    # 4. relectura
    $capT = Normalize-Text $capP.Range.Text
    if ($capT -notmatch '^Tabla \d+\. ' -or -not $capT.EndsWith('. ' + (Normalize-Text $it.titulo))) { throw "entrada ${n}: el pie quedo distinto de lo pedido: [$capT]" }
    if ($nt.Rows.Count -ne $need -or $nt.Columns.Count -ne $cols) { throw "entrada ${n}: la tabla quedo de $($nt.Rows.Count) x $($nt.Columns.Count)" }
    for ($i = 1; $i -le $need; $i++) { for ($j = 1; $j -le $cols; $j++) {
      if ((Normalize-Text $nt.Cell($i, $j).Range.Text) -cne (Normalize-Text ([string]$it.filas[$i - 1][$j - 1]))) { throw "entrada ${n}: la celda ($i,$j) quedo distinta" } } }
    if ($it.fuente -and (Normalize-Text $fp.Range.Text) -cne (Normalize-Text $it.fuente)) { throw "entrada ${n}: la fuente quedo distinta" }
    Write-Host "    insertada: $capT"
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
