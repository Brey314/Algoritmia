# Termina los .docx que genera pandoc: tablas con cuadrícula y encabezado sombreado que se repite,
# numeración de páginas al pie, tabla de contenido actualizada y un PDF de revisión por documento.
# -List: archivo con una línea «ruta.docx<TAB>ruta.pdf» por documento; un solo Word para todos.
param([string]$List)
# Un error de COM no debe dejar un documento a medio terminar con salida 0: build.py lo publicaría.
$ErrorActionPreference = 'Stop'
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
# Con el guardado en segundo plano, reabrir el archivo para exportar puede llegar antes de que el
# guardado termine, y el PDF no se escribe (sin error).
$word.Options.BackgroundSave = $false

function Complete-Document([string]$In, [string]$Pdf) {
  $doc = $word.Documents.Open($In, $false, $false)
  foreach ($table in $doc.Tables) {
    $table.Borders.Enable = 1
    $table.Range.Font.Size = 9
    $table.Range.ParagraphFormat.SpaceAfter = 0
    # Primero al contenido (una columna de una letra no ocupa lo que una de texto) y luego al
    # ancho de la página; pandoc da a todas las columnas el mismo ancho.
    $table.AutoFitBehavior(1)            # wdAutoFitContent
    $table.AutoFitBehavior(2)            # wdAutoFitWindow
    # La matriz del Anexo A del OE4 (nueve columnas): al ajustar a la ventana Word escala cada columna en
    # proporción y parte el caso y la fecha letra por letra, así que sus anchos se fijan aquí.
    if ($table.Columns.Count -eq 9 -and $table.Cell(1, 1).Range.Text -like 'Caso*') {
      $table.AllowAutoFit = $false
      $percent = 9, 11, 3.5, 3.5, 8, 12, 15, 25, 13
      for ($i = 1; $i -le 9; $i++) {
        $table.Columns.Item($i).PreferredWidthType = 2   # wdPreferredWidthPercent
        $table.Columns.Item($i).PreferredWidth = [single]$percent[$i - 1]
      }
    }
    $header = $table.Rows.Item(1)
    $header.HeadingFormat = -1           # se repite en cada página
    $header.Range.Font.Bold = 1
    $header.Shading.BackgroundPatternColor = 14277081   # gris 15 %
    # Los estilos de párrafo los añade build.py al .docx antes de abrirlo aquí. El rótulo
    # «Tabla N.K.» va justo antes de su tabla: que no queden en páginas distintas.
    $before = $table.Range.Paragraphs.Item(1).Previous(1)
    if ($before -ne $null -and $before.Range.Text -like 'Tabla *') { $before.KeepWithNext = -1 }
  }
  $footer = $doc.Sections.Item(1).Footers.Item(1)
  $footer.PageNumbers.Add(1, $true) | Out-Null        # centrada, también en la primera página
  foreach ($toc in $doc.TablesOfContents) { $toc.Update() }
  $doc.Fields.Update() | Out-Null
  foreach ($toc in $doc.TablesOfContents) { $toc.Update() }
  $pages = $doc.ComputeStatistics(2)
  $doc.Save()
  $doc.Close($false)
  # Exportar el mismo documento recién modificado no siempre escribe el PDF (sin error), y tampoco
  # sobrescribe uno anterior: se borra antes, se exporta desde una copia de solo lectura y se comprueba.
  if (Test-Path $Pdf) { Remove-Item $Pdf -ErrorAction Stop }
  $copy = $word.Documents.Open($In, $false, $true)
  for ($attempt = 1; $attempt -le 3 -and -not (Test-Path $Pdf); $attempt++) {
    if ($attempt -gt 1) { Start-Sleep -Seconds 3 }
    $copy.ExportAsFixedFormat($Pdf, 17)
  }
  $copy.Close($false)
  if (-not (Test-Path $Pdf)) { throw "No se genero el PDF tras tres intentos: $Pdf" }
  "{0}: {1} paginas" -f [IO.Path]::GetFileName($In), $pages
}

try {
  foreach ($line in Get-Content -Encoding UTF8 $List) {
    if (-not $line.Trim()) { continue }
    $in, $pdf = $line -split "`t"
    Complete-Document $in $pdf
  }
} finally { $word.Quit() }
