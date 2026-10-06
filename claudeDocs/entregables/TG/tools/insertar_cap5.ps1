<#
.SYNOPSIS
  Inserta en el trabajo de grado el apartado «Fases del trabajo de grado» del capítulo 5
  (claudeDocs/entregables/TG/cap5-fases.md) con Word por COM.

.DESCRIPTION
  Hace cuatro cosas en un solo guardado y, si algo no cuadra, no guarda nada:
    1. Inserta el apartado (un título de nivel 2, seis de nivel 3 y su prosa) justo antes del título
       que dice la primera línea del .md («<!-- inserta-antes: ... -->»), con los estilos de Word
       del capítulo 5: Título 2 como «5.2 Población y muestra», Título 3 como el primero del
       capítulo 6 y Normal con el espaciado del cuerpo del 5.2. Los estilos se piden por su
       constante integrada (WdBuiltinStyle), no por nombre, así que no dependen del idioma de Word.
    2. Renumera a mano los dos títulos que se corren (la numeración de este documento está
       escrita en el texto del título, no es una lista multinivel).
    3. Añade la entrada [53] de la bibliografía después de la [52], con su misma sangría.
       Las citas del documento son texto plano «[n]» y la bibliografía una lista manual (no hay
       campos CITATION ni BIBLIOGRAPHY ni fuentes en el administrador), así que el «[53]» del
       texto insertado ya es la cita y no hace falta crear ninguna fuente.
    4. Actualiza la tabla de contenido y las listas de tablas y figuras (el apartado corre
       las páginas de todo lo que sigue).

  Idempotente: si ya existe un título «Fases del trabajo de grado» (o una entrada [53]) termina con
  código 3 sin tocar el archivo. -DryRun abre el documento de solo lectura, comprueba todos los
  anclajes y cuenta lo que haría; no escribe nada. Word se cierra siempre (bloque finally).

.PARAMETER Docx
  El trabajo de grado (docs\Trabajo_de_Grado_Entrega_Plantilla_28jul.docx). Se modifica en el
  sitio salvo que se dé -Salida. Antes de guardar se deja una copia en %TEMP%\Algoritmia-TG
  (salvo -SinCopia).

.PARAMETER Md
  El apartado en Markdown (UTF-8). Formato en la cabecera de cap5-fases.md.

.PARAMETER Salida
  Opcional. Guarda el resultado en otro .docx y deja -Docx intacto.

.PARAMETER DryRun
  Solo informa qué haría.

.PARAMETER NoActualizarIndices
  No actualiza la tabla de contenido ni las listas de tablas y figuras.

.PARAMETER SinCopia
  No deja copia de seguridad en %TEMP%.

.NOTES
  Códigos de salida: 0 bien (o DryRun sin problemas) · 1 error · 2 uso (archivo inexistente) ·
  3 ya estaba insertado, no se tocó nada.

.EXAMPLE
  pwsh -NoProfile -File "claudeDocs/entregables/TG/tools/insertar_cap5.ps1" `
       -Docx "docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx" `
       -Md "claudeDocs/entregables/TG/cap5-fases.md" -DryRun
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)][string]$Docx,
  [Parameter(Mandatory = $true)][string]$Md,
  [string]$Salida,
  [switch]$DryRun,
  [switch]$NoActualizarIndices,
  [switch]$SinCopia
)

$ErrorActionPreference = 'Stop'

# Constantes de Word (no se usan nombres de estilo: dependen del idioma de la instalación).
$script:wdStyleNormal = -1      # WdBuiltinStyle.wdStyleNormal
$script:wdStyleHeading2 = -3    # WdBuiltinStyle.wdStyleHeading2
$script:wdStyleHeading3 = -4    # WdBuiltinStyle.wdStyleHeading3
$script:wdOutlineBody = 10      # WdOutlineLevel.wdOutlineLevelBodyText
$script:wdNoNumbering = 0       # WdListType.wdListNoNumbering
$script:wdDoNotSave = 0         # WdSaveOptions.wdDoNotSaveChanges
$script:wdFormatDocx = 12       # WdSaveFormat.wdFormatXMLDocument

# Propiedades de párrafo que se copian del párrafo modelo (en este orden: la sangría francesa de los
# títulos de nivel 2 se fija con LeftIndent y luego FirstLineIndent).
$script:ParaFormatProps = @('LeftIndent', 'RightIndent', 'FirstLineIndent', 'SpaceBefore', 'SpaceAfter',
  'LineSpacingRule', 'Alignment', 'KeepWithNext', 'KeepTogether', 'PageBreakBefore', 'WidowControl')

# Caracteres que el apartado no puede traer (raya, comillas rectas e inglesas): por escrito en
# código para que el script sea ASCII puro en lo que compara.
$script:Forbidden = @{
  ([string][char]0x2014) = 'raya (U+2014)'
  ([string][char]0x22)   = 'comilla recta doble'
  ([string][char]0x27)   = 'comilla recta simple'
  ([string][char]0x201C) = 'comilla inglesa de apertura'
  ([string][char]0x201D) = 'comilla inglesa de cierre'
  ([string][char]0x2018) = 'comilla simple inglesa de apertura'
  ([string][char]0x2019) = 'comilla simple inglesa de cierre'
}

function Write-Info([string]$Message) { Write-Host $Message }
function Write-Warn([string]$Message) { Write-Host ("AVISO: " + $Message) -ForegroundColor Yellow }

# --- Lectura del .md -------------------------------------------------------------------------

function Read-Cap5Md([string]$Path) {
  $enc = New-Object System.Text.UTF8Encoding($false)
  $raw = [System.IO.File]::ReadAllText($Path, $enc)     # quita el BOM si lo trae
  $lines = $raw -split "\r?\n"
  $anchor = $null
  $renumber = New-Object System.Collections.Generic.List[object]
  $items = New-Object System.Collections.Generic.List[object]
  $refNumber = $null
  $refText = $null
  $afterMarker = $false          # desde «renumerar» / «referencia» el texto ya no es del capítulo
  $wantRef = $false
  foreach ($line in $lines) {
    $t = $line.Trim()
    if ($t -eq '') { continue }
    if ($t -match '^<!--\s*inserta-antes:\s*(.+?)\s*-->$') { $anchor = $Matches[1]; continue }
    if ($t -match '^<!--\s*renumerar:\s*(.+?)\s*-->$') {
      $afterMarker = $true
      foreach ($part in ($Matches[1] -split ';')) {
        if ($part -notmatch '^\s*(.+?)\s*->\s*(\d+(?:\.\d+)*)\s*$') { throw "Directiva 'renumerar' ilegible: [$part]" }
        $renumber.Add([pscustomobject]@{ OldPrefix = $Matches[1]; NewNumber = $Matches[2] })
      }
      continue
    }
    if ($t -match '^<!--\s*referencia-(\d+)\s*-->$') { $afterMarker = $true; $refNumber = [int]$Matches[1]; $wantRef = $true; continue }
    if ($t -like '<!--*') { continue }                      # otro comentario
    if ($wantRef) {
      if ($t -notmatch ('^\[' + $refNumber + '\]\s+\S')) { throw "Tras 'referencia-$refNumber' se esperaba una línea que empiece por [$refNumber]." }
      $refText = $t; $wantRef = $false; continue
    }
    if ($afterMarker) { throw "Texto inesperado después de las directivas: [$t]" }
    if ($t -match '^###\s+(.+)$') { $items.Add([pscustomobject]@{ Kind = 'h3'; Text = $Matches[1].Trim() }); continue }
    if ($t -match '^##\s+(.+)$') { $items.Add([pscustomobject]@{ Kind = 'h2'; Text = $Matches[1].Trim() }); continue }
    if ($t -match '^#') { throw "Nivel de título no previsto: [$t]" }
    if ($t -match '^(\*|-|\+|\|)\s' -or $t -match '\*\*' -or $t -match '`') { throw "El .md trae lista, tabla, negrita o código, que este script no inserta: [$($t.Substring(0, [Math]::Min(60, $t.Length)))]" }
    $items.Add([pscustomobject]@{ Kind = 'p'; Text = $t })
  }
  if (-not $anchor) { throw "Falta la primera línea '<!-- inserta-antes: ... -->'." }
  if ($items.Count -eq 0) { throw "El .md no trae contenido para insertar." }
  if ($items[0].Kind -ne 'h2') { throw "El primer elemento debe ser el título de nivel 2." }
  if ($refNumber -and -not $refText) { throw "La referencia [$refNumber] no trae texto." }
  # Lo insertado no puede traer rayas ni comillas rectas o inglesas.
  $toCheck = @($items | ForEach-Object { $_.Text })
  if ($refText) { $toCheck += $refText }
  foreach ($text in $toCheck) {
    foreach ($ch in $script:Forbidden.Keys) {
      if ($text.Contains($ch)) { throw ("El texto trae " + $script:Forbidden[$ch] + ": [" + $text.Substring(0, [Math]::Min(60, $text.Length)) + "...]") }
    }
  }
  [pscustomobject]@{
    Anchor     = $anchor
    Renumber   = $renumber.ToArray()
    Items      = $items.ToArray()
    RefNumber  = $refNumber
    RefText    = $refText
  }
}

# --- Utilidades sobre el documento -----------------------------------------------------------

# Texto de un párrafo sin marcas de párrafo, de celda, saltos ni espacios duros; espacios colapsados.
function Normalize-Text([string]$Text) {
  if ($null -eq $Text) { return '' }
  $t = $Text -replace '[\r\a\v\f]', ' '
  $t = $t.Replace([string][char]0x00A0, ' ')
  ($t -replace '\s+', ' ').Trim()
}

# Igualdad de textos sin distinguir mayúsculas. Los títulos del documento usan estilos con «todo
# mayúsculas» y Word, por COM, devuelve Range.Text ya en mayúsculas («5.1 INSTRUMENTOS O ...») aunque el
# XML guarde «5.1 Instrumentos o ...»; comparar con -ceq no encontraba el ancla (06/10/2026).
function Test-SameText([string]$A, [string]$B) {
  [string]::Equals($A, $B, [System.StringComparison]::OrdinalIgnoreCase)
}

# Una pasada por los párrafos del documento: posición, texto y nivel de esquema. Las posiciones
# solo valen hasta la siguiente edición; después se vuelve a pedir la foto.
function Get-Snapshot($Doc) {
  $list = New-Object System.Collections.Generic.List[object]
  $i = 0
  foreach ($p in $Doc.Paragraphs) {
    $i++
    $r = $p.Range
    $list.Add([pscustomobject]@{
      Index   = $i
      Start   = [int]$r.Start
      End     = [int]$r.End
      Text    = (Normalize-Text $r.Text)
      Outline = [int]$p.OutlineLevel
    })
  }
  , $list.ToArray()
}

function Get-ParaAt($Doc, [int]$Start, [int]$End) { $Doc.Range($Start, $End).Paragraphs.Item(1) }

function Get-ParaFormat($Para) {
  $pf = $Para.Range.ParagraphFormat
  $h = [ordered]@{}
  foreach ($n in $script:ParaFormatProps) { $h[$n] = $pf.$n }
  if ([int]$h['LineSpacingRule'] -ge 3) { $h['LineSpacing'] = $pf.LineSpacing }   # solo con regla mínima, exacta o múltiple
  $h
}

function Set-ParaFormat($Para, $Format) {
  $pf = $Para.Range.ParagraphFormat
  foreach ($k in @($Format.Keys)) { $pf.$k = $Format[$k] }
}

function Compare-ParaFormat($A, $B) {
  $diff = @()
  foreach ($k in $A.Keys) {
    if (-not $B.Contains($k)) { continue }
    if ([Math]::Abs([double]$A[$k] - [double]$B[$k]) -gt 0.05) { $diff += ("{0}: {1} contra {2}" -f $k, $A[$k], $B[$k]) }
  }
  $diff
}

function Get-FontInfo($Para) {
  $f = $Para.Range.Font
  [pscustomobject]@{ Name = [string]$f.Name; Size = [double]$f.Size; Bold = [int]$f.Bold; Italic = [int]$f.Italic; Underline = [int]$f.Underline }
}

function Set-ParaStyle($Doc, $Para, [int]$BuiltinId) {
  $rng = $Para.Range
  try { $rng.Style = $Doc.Styles.Item($BuiltinId) }
  catch { $rng.Style = $BuiltinId }
}

function New-WordApplication { New-Object -ComObject Word.Application }

function Remove-ComObject($Obj) {
  if ($null -ne $Obj) { try { [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($Obj) } catch { } }
}

# --- Búsqueda de anclajes (la comparten -DryRun y la inserción) ------------------------------

function Find-Targets($Doc, $Plan) {
  $snap = Get-Snapshot $Doc
  $title = ($Plan.Items[0].Text -replace '^\d+(?:\.\d+)*\.?\s+', '')
  $titleRegex = '^(?:\d+(?:\.\d+)*\.?\s+)?' + [regex]::Escape($title) + '(?:\s+\d+)?$'

  # Idempotencia: el título (o su línea en la tabla de contenido) o la entrada [53] ya están.
  $already = @($snap | Where-Object { $_.Text -imatch $titleRegex })
  $refExists = @()
  if ($Plan.RefNumber) { $refExists = @($snap | Where-Object { $_.Text -match ('^\[' + $Plan.RefNumber + '\]\s') }) }

  # Ancla: el título de nivel 2 que el .md dice («5.1 Instrumentos o herramientas utilizadas»).
  # Se exige nivel de esquema 2 para no confundirlo con su línea de la tabla de contenido.
  $anchors = @($snap | Where-Object { $_.Outline -eq 2 -and (Test-SameText $_.Text (Normalize-Text $Plan.Anchor)) })

  $ctx = [ordered]@{
    Snapshot = $snap; AlreadyThere = $already; RefExists = $refExists; Anchors = $anchors
    Anchor = $null; Prev = $null; PrevEmpty = $false
    H2Model = $null; H3Model = $null; BodyModel = $null
    Renumber = @(); BibEntry = $null; BibCount = 0; Problems = @()
  }
  if ($anchors.Count -ne 1) { $ctx.Problems += "Se esperaba 1 título de nivel 2 igual a [$($Plan.Anchor)] y hay $($anchors.Count)."; return [pscustomobject]$ctx }
  $a = $anchors[0]
  $ctx.Anchor = $a
  if ($a.Index -lt 2) { $ctx.Problems += "El ancla es el primer párrafo del documento."; return [pscustomobject]$ctx }
  $prev = $snap[$a.Index - 2]
  $ctx.Prev = $prev
  $ctx.PrevEmpty = (($prev.End - $prev.Start) -eq 1)      # solo la marca de párrafo: sin texto, salto ni imagen

  # Modelos de formato: el propio ancla para el nivel 2 (capítulo 5), el primer nivel 3 que
  # sigue (capítulo 6; el 5 no tiene) y el primer párrafo de prosa tras el siguiente nivel 2
  # (el cuerpo del 5.2 «Población y muestra», con 12 pt antes y después).
  $ctx.H2Model = $a
  $ctx.H3Model = @($snap | Where-Object { $_.Index -gt $a.Index -and $_.Outline -eq 3 } | Select-Object -First 1)[0]
  $next2 = @($snap | Where-Object { $_.Index -gt $a.Index -and $_.Outline -eq 2 } | Select-Object -First 1)[0]
  $next1 = @($snap | Where-Object { $_.Index -gt $a.Index -and $_.Outline -eq 1 } | Select-Object -First 1)[0]
  if ($next2 -and $next1 -and $next2.Index -lt $next1.Index) {
    foreach ($c in ($snap | Where-Object { $_.Index -gt $next2.Index -and $_.Index -lt $next1.Index -and $_.Outline -eq $script:wdOutlineBody -and $_.Text.Length -gt 40 })) {
      $para = Get-ParaAt $Doc $c.Start $c.End
      if ([int]$para.Range.ListFormat.ListType -eq $script:wdNoNumbering) { $ctx.BodyModel = $c; break }
    }
  }
  if (-not $ctx.BodyModel) { $ctx.Problems += "No se halló un párrafo de cuerpo del capítulo 5 como modelo." }
  if (-not $ctx.H3Model) { $ctx.Problems += "No se halló ningún título de nivel 3 posterior como modelo." }

  # Renumeraciones: cada una debe dar exactamente un título de nivel 2.
  $ren = @()
  foreach ($r in $Plan.Renumber) {
    $m = @($snap | Where-Object { $_.Index -ge $a.Index -and $_.Outline -eq 2 -and $_.Text.StartsWith($r.OldPrefix, [System.StringComparison]::OrdinalIgnoreCase) })
    if ($m.Count -ne 1) { $ctx.Problems += "La renumeración [$($r.OldPrefix)] halló $($m.Count) títulos de nivel 2."; continue }
    $ren += [pscustomobject]@{ Target = $m[0]; OldPrefix = $r.OldPrefix; NewNumber = $r.NewNumber }
  }
  $ctx.Renumber = $ren

  # Bibliografía: última entrada numerada bajo el título BIBLIOGRAFÍA, antes de «OTRAS FUENTES» o «ANEXOS».
  if ($Plan.RefNumber) {
    $bib = @($snap | Where-Object { $_.Outline -eq 1 -and $_.Text -match '^BIBLIOGRAF.A$' } | Select-Object -First 1)[0]
    if (-not $bib) { $ctx.Problems += "No se halló el título BIBLIOGRAFÍA." }
    else {
      $entries = @()
      foreach ($s in ($snap | Where-Object { $_.Index -gt $bib.Index })) {
        if ($s.Outline -eq 1 -or $s.Text -match '^(OTRAS FUENTES|ANEXOS?\b)') { break }
        if ($s.Text -match '^\[(\d+)\]\s') { $entries += [pscustomobject]@{ Number = [int]$Matches[1]; Para = $s } }
      }
      $ctx.BibCount = $entries.Count
      $last = @($entries | Sort-Object Number | Select-Object -Last 1)[0]
      if (-not $last) { $ctx.Problems += "La bibliografía no trae entradas numeradas." }
      elseif ($last.Number -ne ($Plan.RefNumber - 1)) { $ctx.Problems += "La última entrada es [$($last.Number)] y se esperaba [$($Plan.RefNumber - 1)]." }
      else { $ctx.BibEntry = $last.Para }
    }
  }
  [pscustomobject]$ctx
}

# --- Inserción -------------------------------------------------------------------------------

function Add-Cap5Block($Doc, $Plan, $Ctx) {
  $items = $Plan.Items
  $texts = @($items | ForEach-Object { $_.Text })
  $cr = [string][char]13

  # Formatos de los modelos, leídos antes de tocar nada (las posiciones cambian al insertar).
  $h2Para = Get-ParaAt $Doc $Ctx.H2Model.Start $Ctx.H2Model.End
  $h3Para = Get-ParaAt $Doc $Ctx.H3Model.Start $Ctx.H3Model.End
  $bodyPara = Get-ParaAt $Doc $Ctx.BodyModel.Start $Ctx.BodyModel.End
  $fmt = @{ h2 = (Get-ParaFormat $h2Para); h3 = (Get-ParaFormat $h3Para); p = (Get-ParaFormat $bodyPara) }
  $styleName = @{ h2 = [string]$h2Para.Style.NameLocal; h3 = [string]$h3Para.Style.NameLocal; p = [string]$bodyPara.Style.NameLocal }
  $font = @{ h2 = (Get-FontInfo $h2Para); h3 = (Get-FontInfo $h3Para); p = (Get-FontInfo $bodyPara) }
  $builtin = @{ h2 = $script:wdStyleHeading2; h3 = $script:wdStyleHeading3; p = $script:wdStyleNormal }
  $outline = @{ h2 = 2; h3 = 3; p = $script:wdOutlineBody }

  # El bloque entra al final del párrafo vacío que precede al ancla (el separador que ya tiene el
  # documento antes de cada título de nivel 2): nada se inserta sobre el marcador del ancla, que
  # lleva sus marcadores de tabla de contenido. Cada «\r» abre un párrafo:
  #   [separador vacío] [título] [prosa ...] [separador vacío = la marca original] [ancla]
  # Si el párrafo previo no estuviera vacío, se añade un separador más.
  $pos = $Ctx.Prev.End - 1
  $lead = $cr
  if (-not $Ctx.PrevEmpty) { $lead = $cr + $cr }
  $block = $lead + ($texts -join $cr) + $cr
  $Doc.Range($pos, $pos).InsertAfter($block)

  # Recorrido de lo creado, con comprobación de cada párrafo antes de darle formato.
  $cur = $Doc.Range($pos, $pos).Paragraphs.Item(1)
  if ($Ctx.PrevEmpty) { $expectFirst = '' } else { $expectFirst = $Ctx.Prev.Text }
  if ((Normalize-Text $cur.Range.Text) -ne $expectFirst) { throw "Tras insertar, el primer párrafo no es el esperado." }
  if (-not $Ctx.PrevEmpty) {
    $cur = $cur.Next(1)
    if ((Normalize-Text $cur.Range.Text) -ne '') { throw "Tras insertar, falta el separador vacío tras el párrafo previo." }
  }
  $created = New-Object System.Collections.Generic.List[object]
  for ($k = 0; $k -lt $items.Count; $k++) {
    $cur = $cur.Next(1)
    $got = ([string]$cur.Range.Text).TrimEnd([char]13)
    if (-not (Test-SameText $got $items[$k].Text)) { throw ("Párrafo insertado inesperado en la posición {0}: [{1}]" -f ($k + 1), $got.Substring(0, [Math]::Min(60, $got.Length))) }
    $created.Add([pscustomobject]@{ Para = $cur; Kind = $items[$k].Kind })
  }
  $tail = $cur.Next(1)
  if ((Normalize-Text $tail.Range.Text) -ne '') { throw "Tras el bloque no quedó el separador vacío." }
  $after = $tail.Next(1)
  if (-not (Test-SameText (Normalize-Text $after.Range.Text) (Normalize-Text $Plan.Anchor)) -or [int]$after.OutlineLevel -ne 2) { throw "Tras el separador no está el ancla." }

  # Formato: estilo integrado, se quita lo manual que arrastró el párrafo vacío y se copian del
  # modelo las propiedades de párrafo (sangrías y espaciado, lo que el modelo lleva directo).
  foreach ($c in $created) {
    $kind = $c.Kind
    Set-ParaStyle $Doc $c.Para $builtin[$kind]
    $c.Para.Range.ParagraphFormat.Reset()
    $c.Para.Range.Font.Reset()
    Set-ParaFormat $c.Para $fmt[$kind]
    if ([string]$c.Para.Style.NameLocal -ne $styleName[$kind]) { throw ("El estilo de un párrafo {0} no coincide con el del modelo ({1})." -f $kind, $styleName[$kind]) }
    if ([int]$c.Para.OutlineLevel -ne $outline[$kind]) { throw ("El nivel de esquema de un párrafo {0} no es {1}." -f $kind, $outline[$kind]) }
  }
  $warn = @()
  foreach ($c in $created) {
    $d = Compare-ParaFormat (Get-ParaFormat $c.Para) $fmt[$c.Kind]
    if ($d.Count -gt 0) { $warn += ("formato de un párrafo {0}: {1}" -f $c.Kind, ($d -join '; ')) }
    $fi = Get-FontInfo $c.Para; $fm = $font[$c.Kind]
    if ($fi.Name -ne $fm.Name -or $fi.Size -ne $fm.Size -or $fi.Bold -ne $fm.Bold -or $fi.Italic -ne $fm.Italic) {
      $warn += ("fuente de un párrafo {0}: {1} {2} contra {3} {4}" -f $c.Kind, $fi.Name, $fi.Size, $fm.Name, $fm.Size)
    }
  }
  [pscustomobject]@{ Created = $created.Count; Warnings = $warn; BlockEnd = $pos + $block.Length }
}

function Update-Numbers($Doc, $Plan, $Ctx) {
  # Foto nueva: las posiciones cambiaron al insertar. Se vuelven a localizar los títulos por su
  # texto, solo los que están después del bloque nuevo.
  $snap = Get-Snapshot $Doc
  $edits = @()
  foreach ($r in $Plan.Renumber) {
    $m = @($snap | Where-Object { $_.Outline -eq 2 -and $_.Text.StartsWith($r.OldPrefix, [System.StringComparison]::OrdinalIgnoreCase) })
    if ($m.Count -ne 1) { throw "La renumeración [$($r.OldPrefix)] halló $($m.Count) títulos tras insertar." }
    $oldNum = ($r.OldPrefix -split ' ')[0]
    $newNum = $r.NewNumber
    # Solo se reescribe lo que cambia («1» por «2»): el resto del título conserva su formato y
    # sus marcadores.
    $cp = 0
    while ($cp -lt $oldNum.Length -and $cp -lt $newNum.Length -and $oldNum[$cp] -ceq $newNum[$cp]) { $cp++ }
    $cs = 0
    while ($cs -lt ($oldNum.Length - $cp) -and $cs -lt ($newNum.Length - $cp) -and $oldNum[$oldNum.Length - 1 - $cs] -ceq $newNum[$newNum.Length - 1 - $cs]) { $cs++ }
    $edits += [pscustomobject]@{
      Start = $m[0].Start + $cp
      End = $m[0].Start + $oldNum.Length - $cs
      Old = $oldNum.Substring($cp, $oldNum.Length - $cp - $cs)
      New = $newNum.Substring($cp, $newNum.Length - $cp - $cs)
      From = $m[0].Text
    }
  }
  # De atrás hacia adelante, por si una sustitución cambiara de longitud.
  foreach ($e in ($edits | Sort-Object Start -Descending)) {
    $rng = $Doc.Range($e.Start, $e.End)
    if ($rng.Text -cne $e.Old) { throw ("La renumeración esperaba [{0}] y halló [{1}] en: {2}" -f $e.Old, $rng.Text, $e.From) }
    $rng.Text = $e.New
  }
  @($edits | ForEach-Object { $_.From })
}

function Add-Reference($Doc, $Plan) {
  $snap = Get-Snapshot $Doc
  $bib = @($snap | Where-Object { $_.Outline -eq 1 -and $_.Text -match '^BIBLIOGRAF.A$' } | Select-Object -First 1)[0]
  $last = $null
  foreach ($s in ($snap | Where-Object { $_.Index -gt $bib.Index })) {
    if ($s.Outline -eq 1 -or $s.Text -match '^(OTRAS FUENTES|ANEXOS?\b)') { break }
    if ($s.Text -match '^\[(\d+)\]\s' -and [int]$Matches[1] -eq ($Plan.RefNumber - 1)) { $last = $s }
  }
  if (-not $last) { throw "No se halló la entrada [$($Plan.RefNumber - 1)] de la bibliografía." }
  $prevPara = Get-ParaAt $Doc $last.Start $last.End
  $fmtPrev = Get-ParaFormat $prevPara
  $fontPrev = Get-FontInfo $prevPara
  $stylePrev = [string]$prevPara.Style.NameLocal
  # Al final del texto de la [52], antes de su marca de párrafo: el párrafo nuevo nace con la
  # sangría francesa y el espaciado de las demás entradas.
  $pos = $last.End - 1
  $Doc.Range($pos, $pos).InsertAfter(([string][char]13) + $Plan.RefText)
  $new = $Doc.Range($pos, $pos).Paragraphs.Item(1).Next(1)
  $got = ([string]$new.Range.Text).TrimEnd([char]13)
  if ($got -cne $Plan.RefText) { throw "La entrada [$($Plan.RefNumber)] no quedó como se esperaba." }
  $notes = @()
  if ([string]$new.Style.NameLocal -ne $stylePrev) { throw "La entrada [$($Plan.RefNumber)] no tiene el estilo de la anterior." }
  $d = Compare-ParaFormat (Get-ParaFormat $new) $fmtPrev
  if ($d.Count -gt 0) { Set-ParaFormat $new $fmtPrev; $notes += "se copió el formato de la entrada anterior" }
  $f = Get-FontInfo $new
  if ($f.Name -ne $fontPrev.Name -or $f.Size -ne $fontPrev.Size -or $f.Bold -ne $fontPrev.Bold -or $f.Italic -ne $fontPrev.Italic -or $f.Underline -ne $fontPrev.Underline) {
    $new.Range.Font.Reset(); $notes += "se quitó formato de carácter heredado"
  }
  $notes
}

# --- Programa principal ----------------------------------------------------------------------

function Invoke-Main {
  if (-not (Test-Path -LiteralPath $Docx)) { Write-Host "No existe -Docx: $Docx"; return 2 }
  if (-not (Test-Path -LiteralPath $Md)) { Write-Host "No existe -Md: $Md"; return 2 }
  $docPath = (Resolve-Path -LiteralPath $Docx).Path
  $mdPath = (Resolve-Path -LiteralPath $Md).Path

  try { $plan = Read-Cap5Md $mdPath }
  catch { Write-Host ("ERROR en el .md: " + $_.Exception.Message) -ForegroundColor Red; return 1 }
  $nH2 = @($plan.Items | Where-Object { $_.Kind -eq 'h2' }).Count
  $nH3 = @($plan.Items | Where-Object { $_.Kind -eq 'h3' }).Count
  $nP = @($plan.Items | Where-Object { $_.Kind -eq 'p' }).Count
  Write-Info ("Md: {0} titulos de nivel 2, {1} de nivel 3, {2} parrafos; ancla [{3}]; referencia [{4}]; renumerar {5}." -f $nH2, $nH3, $nP, $plan.Anchor, $plan.RefNumber, @($plan.Renumber).Count)

  $outPath = $null
  if ($Salida) { $outPath = [System.IO.Path]::GetFullPath($Salida) }

  # Copia de seguridad antes de abrir Word (con Word abierto el archivo queda tomado).
  $backup = $null
  if (-not $DryRun -and -not $SinCopia) {
    $dir = Join-Path ([System.IO.Path]::GetTempPath()) 'Algoritmia-TG'
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $backup = Join-Path $dir ([System.IO.Path]::GetFileNameWithoutExtension($docPath) + '.' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.antes-cap5.docx')
    Copy-Item -LiteralPath $docPath -Destination $backup
    Write-Info "Copia de seguridad: $backup"
  }

  $word = $null; $doc = $null; $saved = $false; $code = 0; $trackBefore = $false
  try {
    $word = New-WordApplication
    $word.Visible = $false
    $word.DisplayAlerts = 0                    # wdAlertsNone
    try { $word.Options.BackgroundSave = $false } catch { }
    $doc = $word.Documents.Open($docPath, $false, [bool]$DryRun, $false)    # sin conversion, solo lectura si -DryRun, fuera de recientes
    if (-not $DryRun -and [bool]$doc.ReadOnly) { throw "Word abrio el documento de solo lectura (esta abierto en otro Word o el archivo es de solo lectura). Cierrelo y repita." }
    $ctx = Find-Targets $doc $plan

    # Idempotencia: si ya esta, no se toca nada.
    if (@($ctx.AlreadyThere).Count -gt 0 -or @($ctx.RefExists).Count -gt 0) {
      Write-Host "Ya esta insertado: existe un titulo 'Fases del trabajo de grado' o la entrada [$($plan.RefNumber)]. No se toco nada."
      foreach ($h in @($ctx.AlreadyThere)) { Write-Host ("  parrafo {0}: {1}" -f $h.Index, $h.Text) }
      foreach ($h in @($ctx.RefExists)) { Write-Host ("  parrafo {0}: {1}" -f $h.Index, $h.Text.Substring(0, [Math]::Min(70, $h.Text.Length))) }
      $code = 3
    }
    elseif (@($ctx.Problems).Count -gt 0) {
      foreach ($p in $ctx.Problems) { Write-Host ("No se puede seguir: " + $p) }
      $code = 1
    }
    elseif ($DryRun) {
      Write-Info "-DryRun: no se escribe nada. Se haria esto:"
      Write-Info ("  documento abierto de solo lectura; {0} parrafos; {1} tabla(s) de contenido; {2} lista(s) de tablas/figuras; control de cambios {3}" -f @($ctx.Snapshot).Count, $doc.TablesOfContents.Count, $doc.TablesOfFigures.Count, $doc.TrackRevisions)
      Write-Info ("  ancla: parrafo {0} (nivel 2) '{1}'; parrafo previo {2}" -f $ctx.Anchor.Index, $ctx.Anchor.Text, $(if ($ctx.PrevEmpty) { 'vacio (el bloque entra ahi)' } else { 'NO vacio (se anadiria un separador)' }))
      Write-Info ("  modelos de formato: titulo 2 = el ancla; titulo 3 = parrafo {0} '{1}'; cuerpo = parrafo {2}" -f $ctx.H3Model.Index, $ctx.H3Model.Text, $ctx.BodyModel.Index)
      $n = 0
      foreach ($it in $plan.Items) {
        $n++
        $label = switch ($it.Kind) { 'h2' { 'TITULO 2' } 'h3' { 'TITULO 3' } default { 'parrafo  ' } }
        Write-Info ("  {0,2}. {1} {2}" -f $n, $label, $it.Text.Substring(0, [Math]::Min(78, $it.Text.Length)))
      }
      foreach ($r in $ctx.Renumber) { Write-Info ("  renumerar: '{0}' -> {1}" -f $r.Target.Text, $r.NewNumber) }
      if ($plan.RefNumber) { Write-Info ("  bibliografia: {0} entradas; la [{1}] iria tras la de [{2}] (parrafo {3}), con su misma sangria" -f $ctx.BibCount, $plan.RefNumber, ($plan.RefNumber - 1), $ctx.BibEntry.Index) }
      if ($NoActualizarIndices) { Write-Info "  no se actualizarian los indices (-NoActualizarIndices)" } else { Write-Info "  se actualizarian la tabla de contenido y las listas de tablas y figuras" }
      if ($outPath) { Write-Info "  se guardaria en: $outPath" } else { Write-Info "  se guardaria en el sitio: $docPath (con copia en %TEMP%\Algoritmia-TG)" }
    }
    else {
      $trackBefore = [bool]$doc.TrackRevisions
      if ($trackBefore) { Write-Warn "El control de cambios estaba activo: se apaga durante la insercion y se restaura."; $doc.TrackRevisions = $false }

      $res = Add-Cap5Block $doc $plan $ctx
      Write-Info ("Insertados {0} parrafos del apartado." -f $res.Created)
      foreach ($w in $res.Warnings) { Write-Warn $w }
      $changed = Update-Numbers $doc $plan $ctx
      foreach ($c in $changed) { Write-Info ("Renumerado: {0}" -f $c) }
      if ($plan.RefNumber) {
        $notes = Add-Reference $doc $plan
        Write-Info ("Entrada [{0}] anadida tras la [{1}]." -f $plan.RefNumber, ($plan.RefNumber - 1))
        foreach ($n in $notes) { Write-Info ("  " + $n) }
      }
      if (-not $NoActualizarIndices) {
        $doc.Repaginate()
        for ($i = 1; $i -le $doc.TablesOfContents.Count; $i++) { $doc.TablesOfContents.Item($i).Update() }
        for ($i = 1; $i -le $doc.TablesOfFigures.Count; $i++) { $doc.TablesOfFigures.Item($i).Update() }
        Write-Info "Indices actualizados."
      }
      if ($trackBefore) { $doc.TrackRevisions = $true }
      if ($outPath) { $doc.SaveAs2($outPath, $script:wdFormatDocx); Write-Info "Guardado en: $outPath" }
      else { $doc.Save(); Write-Info "Guardado en el sitio: $docPath" }
      $saved = $true
      Write-Info "Siguiente paso: python claudeDocs/entregables/TG/tools/verificar_cap5.py --docx <resultado> --md <cap5-fases.md>"
    }
  }
  catch {
    Write-Host ("ERROR: " + $_.Exception.Message) -ForegroundColor Red
    if ($_.InvocationInfo) { Write-Host ("  en " + $_.InvocationInfo.PositionMessage) }
    Write-Host "No se guardo nada (el documento se cierra sin guardar)."
    $code = 1
  }
  finally {
    # Word se cierra siempre. Si no se guardo, se marca el documento como limpio para que Word no
    # ofrezca guardar ni guarde por su cuenta lo que quedo a medias.
    if ($null -ne $doc) {
      if (-not $saved) { try { $doc.Saved = $true } catch { } }
      try { $doc.Close($false) } catch { }
      Remove-ComObject $doc
    }
    if ($null -ne $word) {
      try { $word.Quit($false) } catch { }
      Remove-ComObject $word
    }
    [GC]::Collect(); [GC]::WaitForPendingFinalizers(); [GC]::Collect()
  }
  return $code
}

# Si se carga con punto (. .\insertar_cap5.ps1) no ejecuta nada: asi se prueban las funciones.
if ($MyInvocation.InvocationName -ne '.') {
  $exit = Invoke-Main
  exit ([int]($exit | Select-Object -Last 1))
}
