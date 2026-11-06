<#
.SYNOPSIS
  De un solo uso: corrige datos de cuatro fuentes bibliográficas del trabajo de grado y mantiene enteras las Tablas 6
  y 13 con «conservar con el siguiente». Word no deja borrar una fuente citada ni tocar sus campos, así que se editan
  los nodos de la parte XML de fuentes; la bibliografía se regenera después, al actualizar su campo.
  Aborta sin guardar si un nodo no aparece o si cambia el número de campos CITATION.
#>
param([Parameter(Mandatory = $true)][string]$Docx)
$ErrorActionPreference = 'Stop'
$path = (Resolve-Path -LiteralPath $Docx).Path
$ns = 'http://schemas.openxmlformats.org/officeDocument/2006/bibliography'
$cambios = @(
  @{ Tag = 'Ref09'; Valores = [ordered]@{ Year = '2021. [En línea]. Disponible en: http://saber.ucv.ve/ojs/index.php/rev_arete/article/view/21327' } },
  @{ Tag = 'Ref21'; Valores = [ordered]@{ JournalName = 'Revista Científica Multidisciplinaria Tsafiki'; Volume = '3' } },
  @{ Tag = 'Ref45'; Nuevos = [ordered]@{ Volume = '61'; Issue = '7'; Pages = '1505-1536' } },
  @{ Tag = 'Ref46'; Nuevos = [ordered]@{ Volume = '63'; Issue = '5'; Pages = '1186-1218' }; Valores = [ordered]@{ Year = '2025. doi: 10.1177/07356331251337628' } }
)
$word = New-Object -ComObject Word.Application
$word.Visible = $false; $word.DisplayAlerts = 0
$doc = $null; $saved = $false
try {
  $doc = $word.Documents.Open($path, $false, $false, $false)
  $tracking = $doc.TrackRevisions; $doc.TrackRevisions = $false
  $citas = @($doc.Fields | Where-Object { $_.Type -eq 96 }).Count
  $partes = $doc.CustomXMLParts.SelectByNamespace($ns)
  if ($partes.Count -ne 1) { throw "Partes de fuentes: $($partes.Count)" }
  $parte = $partes.Item(1)
  $parte.NamespaceManager.AddNamespace('b', $ns)
  foreach ($c in $cambios) {
    $base = "//b:Source[b:Tag='$($c.Tag)']"
    if ($c.Nuevos) {
      $fuente = $parte.SelectSingleNode($base)
      $anio = $parte.SelectSingleNode("$base/b:Year")
      if ($null -eq $fuente -or $null -eq $anio) { throw "$($c.Tag): sin nodo" }
      foreach ($k in $c.Nuevos.Keys) { $fuente.InsertNodeBefore($k, $ns, 1, $c.Nuevos[$k], $anio) }
    }
    if ($c.Valores) {
      foreach ($k in $c.Valores.Keys) {
        $nodo = $parte.SelectSingleNode("$base/b:$k")
        if ($null -eq $nodo) { throw "$($c.Tag): sin $k" }
        $nodo.Text = $c.Valores[$k]
      }
    }
    "fuente $($c.Tag)"
  }
  foreach ($t in $doc.Tables) {
    $previo = $t.Range.Paragraphs.Item(1).Previous(1)
    if ($null -eq $previo -or ([string]$previo.Range.Text) -notmatch '^Tabla (6|13)\.') { continue }
    $previo.KeepWithNext = -1
    for ($i = 1; $i -lt $t.Rows.Count; $i++) { $t.Rows.Item($i).Range.ParagraphFormat.KeepWithNext = -1 }
    "tabla: $([string]$previo.Range.Text)"
  }
  $despues = @($doc.Fields | Where-Object { $_.Type -eq 96 }).Count
  if ($despues -ne $citas) { throw "CITATION: $citas antes y $despues después" }
  $doc.TrackRevisions = $tracking
  $doc.Save(); $saved = $true
  "guardado; CITATION $despues"
} finally {
  if ($null -ne $doc) { if (-not $saved) { try { $doc.Saved = $true } catch {} }; try { $doc.Close(0) } catch {} }
  try { $word.Quit(0) } catch {}
}
