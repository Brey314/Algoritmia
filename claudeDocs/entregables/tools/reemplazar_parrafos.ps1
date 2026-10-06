<#
.SYNOPSIS
  Sustituye parrafos concretos de un .docx por coincidencia exacta de texto, con Word por COM,
  conservando el estilo y el formato del parrafo.

.DESCRIPTION
  Herramienta generica (no sabe de ningun documento). Recibe una lista de pares
  {"antes": "...", "despues": "..."} en un .json y, en un solo guardado:
    1. Busca cada «antes» como PARRAFO COMPLETO, en el cuerpo del documento y dentro de las celdas de
       tabla. La comparacion usa el texto normalizado (sin marcas de parrafo ni de celda, espacios
       duros como espacios, espacios colapsados) y no distingue mayusculas, porque Word devuelve en
       mayusculas, por COM, el texto de los estilos «todo mayusculas».
    2. Cada «antes» debe aparecer exactamente una vez (o exactamente «veces» veces, ver abajo). Si
       alguno no aparece o aparece otro numero de veces, aborta SIN guardar y lo dice.
    3. Cambia solo el texto del parrafo (nunca la marca de fin de parrafo ni de celda), asi que el
       estilo de parrafo, la sangria, el espaciado y los marcadores de tabla no se tocan. Para no
       perder el formato de caracter, solo se reescribe el TRAMO que cambia: se deja intacto el prefijo
       y el sufijo que «antes» y «despues» comparten, y lo demas conserva su formato (por ejemplo, en
       «**Serie:** texto» solo se reescribe «texto» y «Serie:» sigue en negrita). Si el tramo que cambia
       tiene formato de caracter mezclado (negritas internas, etc.), avisa y el texto nuevo toma el
       formato de su primer caracter; con -Estricto aborta en cambio.
    4. Apaga el control de cambios durante la edicion y lo restaura antes de guardar.
    5. Actualiza la tabla de contenido y las listas de tablas y figuras (salvo -NoActualizarIndices).
    6. Antes de guardar vuelve a leer el documento y comprueba cada sustitucion; si algo no cuadra, no
       guarda nada y Word se cierra siempre (bloque finally).

  Formato del .json (UTF-8): una lista de objetos.
    [ {"antes": "texto actual", "despues": "texto nuevo"},
      {"antes": "O03-1", "despues": "D01-1", "veces": 2, "nota": "opcional, se ignora"} ]
  «veces» (opcional, 1 por omision) es el numero EXACTO de parrafos que deben coincidir con «antes»:
  sirve para un texto que se repite a proposito (el mismo codigo en dos tablas). Todos se sustituyen
  por «despues». Cualquier otra clave es un error (atrapa «despues» mal escrito).

  Idempotente: si ningun «antes» aparece y todos los «despues» ya estan, termina con codigo 3 sin
  tocar el archivo. Un estado mixto (unos pares aplicados y otros pendientes) aborta con codigo 1.

  Alcance: parrafos del cuerpo y de las celdas de tabla (tambien anidadas). No toca encabezados,
  pies, cuadros de texto ni notas. Un parrafo con campos (hipervinculos, referencias cruzadas, la
  propia tabla de contenido) no se edita: aborta. Un parrafo con revisiones pendientes tampoco.

.PARAMETER Docx
  El .docx a modificar. Se cambia en el sitio salvo que se de -Salida; antes de guardar se deja una
  copia en %TEMP%\Algoritmia-Docx (salvo -SinCopia o -Salida).

.PARAMETER Pares
  El .json de pares.

.PARAMETER Salida
  Opcional. Guarda el resultado en otro .docx y deja -Docx intacto.

.PARAMETER DryRun
  Abre el documento de solo lectura, comprueba todos los pares, el estado del formato y lo que haria
  (el tramo exacto que cambia en cada parrafo) y no escribe nada.

.PARAMETER Estricto
  Aborta si el tramo que cambia en algun parrafo tiene formato de caracter mezclado, en lugar de
  aplicarle el formato de su primer caracter.

.PARAMETER NoActualizarIndices
  No actualiza la tabla de contenido ni las listas de tablas y figuras.

.PARAMETER SinCopia
  No deja copia de seguridad en %TEMP%.

.NOTES
  Codigos de salida: 0 bien (o DryRun sin problemas) · 1 error (no se guardo nada) · 2 uso (archivo
  inexistente) · 3 ya estaba aplicado, no se toco nada.
  Despues de guardar, comprobar con: python claudeDocs/entregables/tools/verificar_reemplazos.py
  --docx <resultado> --original <el .docx de antes> --pares <el .json>

.EXAMPLE
  pwsh -NoProfile -File "claudeDocs/entregables/tools/reemplazar_parrafos.ps1" `
       -Docx "docs/actas/OE3/Acta_D01_2026-09-02.docx" `
       -Pares "claudeDocs/entregables/tools/pares/acta_D01.json" -DryRun
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)][string]$Docx,
  [Parameter(Mandatory = $true)][string]$Pares,
  [string]$Salida,
  [switch]$DryRun,
  [switch]$Estricto,
  [switch]$NoActualizarIndices,
  [switch]$SinCopia
)

$ErrorActionPreference = 'Stop'

# Constantes de Word.
$script:wdWithInTable = 12      # WdInformation.wdWithInTable
$script:wdUndefined = 9999999   # WdConstants.wdUndefined: lo que devuelven Font.* cuando el rango mezcla valores
$script:wdFormatDocx = 12       # WdSaveFormat.wdFormatXMLDocument

# Propiedades de Font que cuentan como «formato de caracter» al decidir si un tramo es uniforme.
$script:FontProps = @('Bold', 'Italic', 'Underline', 'StrikeThrough', 'Superscript', 'Subscript',
  'AllCaps', 'SmallCaps', 'Hidden', 'Color', 'Size')

function Write-Info([string]$Message) { Write-Host $Message }
function Write-Warn([string]$Message) { Write-Host ("AVISO: " + $Message) -ForegroundColor Yellow }

# --- Texto -----------------------------------------------------------------------------------

# Texto de un parrafo sin marcas de parrafo, de celda, saltos ni espacios duros; espacios colapsados.
function Normalize-Text([string]$Text) {
  if ($null -eq $Text) { return '' }
  $t = $Text -replace '[\r\a\v\f]', ' '
  $t = $t.Replace([string][char]0x00A0, ' ')
  ($t -replace '\s+', ' ').Trim()
}

# Igualdad de textos sin distinguir mayusculas (ver .DESCRIPTION: estilos «todo mayusculas»).
function Test-SameText([string]$A, [string]$B) {
  [string]::Equals($A, $B, [System.StringComparison]::OrdinalIgnoreCase)
}

# Un fragmento de texto para los mensajes: una linea, recortado.
function Get-Excerpt([string]$Text, [int]$Max = 70) {
  if ($null -eq $Text) { return '' }
  $t = ($Text -replace '[\r\n\a\v\f]', ' ')
  if ($t.Length -le $Max) { return $t }
  $t.Substring(0, $Max) + '...'
}

# Lo que una sustitucion no admite en «despues»: un salto partiria el parrafo.
function Test-HasBreak([string]$Text) {
  $Text -match '[\r\n\a\v\f]'
}

# --- Lectura de los pares --------------------------------------------------------------------

function Read-Pairs([string]$Path) {
  $enc = New-Object System.Text.UTF8Encoding($false)
  $raw = [System.IO.File]::ReadAllText($Path, $enc)       # quita el BOM si lo trae
  # System.Text.Json y no ConvertFrom-Json: este convierte en fechas los textos que parecen ISO
  # («2026-09-02») y el «antes» ya no llegaria como texto.
  try { $json = [System.Text.Json.JsonDocument]::Parse($raw) }
  catch { throw "no es un JSON valido: $($_.Exception.Message)" }
  $out = New-Object System.Collections.Generic.List[object]
  try {
    $root = $json.RootElement
    if ($root.ValueKind -eq [System.Text.Json.JsonValueKind]::Object) {
      foreach ($prop in $root.EnumerateObject()) { if ($prop.Name -ceq 'pares') { $root = $prop.Value; break } }
    }
    if ($root.ValueKind -ne [System.Text.Json.JsonValueKind]::Array) { throw "el .json debe ser una lista de pares {antes, despues}." }
    $n = 0
    foreach ($el in $root.EnumerateArray()) {
      $n++
      if ($el.ValueKind -ne [System.Text.Json.JsonValueKind]::Object) { throw "El par $n no es un objeto {antes, despues}." }
      $antes = $null; $despues = $null; $veces = 1
      foreach ($prop in $el.EnumerateObject()) {
        switch -CaseSensitive ($prop.Name) {
          'antes' {
            if ($prop.Value.ValueKind -ne [System.Text.Json.JsonValueKind]::String) { throw "El par ${n}: 'antes' debe ser texto." }
            $antes = $prop.Value.GetString()
          }
          'despues' {
            if ($prop.Value.ValueKind -ne [System.Text.Json.JsonValueKind]::String) { throw "El par ${n}: 'despues' debe ser texto." }
            $despues = $prop.Value.GetString()
          }
          'veces' {
            $v = 0
            if ($prop.Value.ValueKind -ne [System.Text.Json.JsonValueKind]::Number -or -not $prop.Value.TryGetInt32([ref]$v) -or $v -lt 1) { throw "El par ${n}: 'veces' debe ser un entero de 1 o mas." }
            $veces = $v
          }
          'nota' { }
          default { throw "El par $n trae la clave desconocida '$($prop.Name)' (validas: antes, despues, veces, nota)." }
        }
      }
      if ($null -eq $antes) { throw "El par $n no trae 'antes'." }
      if ($null -eq $despues) { throw "El par $n no trae 'despues'." }
      $antesN = Normalize-Text $antes
      $despuesN = Normalize-Text $despues
      if ($antesN -eq '') { throw "El par $n trae un 'antes' vacio." }
      if ($despues.Trim() -eq '') { throw "El par $n trae un 'despues' vacio (esta herramienta no borra parrafos)." }
      if (Test-HasBreak $despues) { throw "El par ${n}: 'despues' trae un salto de linea o de parrafo, que partiria el parrafo." }
      if ($antesN -ceq $despuesN) { throw "El par $n no cambia nada: 'antes' y 'despues' son iguales una vez normalizados (solo cambian espacios duros o repetidos, que no se pueden distinguir)." }
      $out.Add([pscustomobject]@{
        Number        = $n
        Antes         = $antes
        Despues       = $despues
        AntesN        = $antesN
        DespuesN      = $despuesN
        Veces         = $veces
        # Un cambio solo de mayusculas («Octubre» a «octubre») no se puede reconocer sin distinguirlas.
        CaseSensitive = (Test-SameText $antesN $despuesN)
      })
    }
  }
  finally { $json.Dispose() }
  if ($out.Count -eq 0) { throw "el .json no trae ningun par." }
  # Un «antes» repetido, o un «despues» que es el «antes» de otro par, volveria ambiguo el estado.
  foreach ($a in $out) {
    foreach ($b in $out) {
      if ([object]::ReferenceEquals($a, $b)) { continue }
      if (Test-SameText $a.AntesN $b.AntesN) { throw "Los pares $($a.Number) y $($b.Number) tienen el mismo 'antes': [$(Get-Excerpt $a.AntesN)]. Use un solo par con 'veces'." }
      if (Test-SameText $a.DespuesN $b.AntesN) { throw "El 'despues' del par $($a.Number) es el 'antes' del par $($b.Number): [$(Get-Excerpt $a.DespuesN)]. No se admiten cadenas de sustituciones." }
    }
  }
  , $out.ToArray()
}

# --- Utilidades sobre el documento -----------------------------------------------------------

function New-WordApplication { New-Object -ComObject Word.Application }

function Remove-ComObject($Obj) {
  if ($null -ne $Obj) { try { [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($Obj) } catch { } }
}

# Una pasada por los parrafos del documento (cuerpo y celdas): posicion, texto normalizado. Las
# posiciones solo valen hasta la siguiente edicion; despues se vuelve a pedir la foto.
function Get-Snapshot($Doc) {
  $list = New-Object System.Collections.Generic.List[object]
  $i = 0
  foreach ($p in $Doc.Paragraphs) {
    $i++
    $r = $p.Range
    $list.Add([pscustomobject]@{
      Index = $i
      Start = [int]$r.Start
      End   = [int]$r.End
      Text  = (Normalize-Text $r.Text)
    })
  }
  , $list.ToArray()
}

function Get-Where($Doc, $Entry) {
  try { if ($Doc.Range($Entry.Start, $Entry.End).Information($script:wdWithInTable)) { return 'tabla' } } catch { }
  'cuerpo'
}

# --- Busqueda de los pares (la comparten -DryRun y la edicion) --------------------------------

# Estado de cada par: 'pendiente' (el «antes» aparece las veces pedidas), 'aplicado' (ya no esta el
# «antes» y el «despues» esta), 'falta' (no aparece ni uno ni otro) o 'distinto' (el «antes» aparece
# un numero de veces que no es el pedido).
function Find-Pairs($Snapshot, $PairList) {
  $res = New-Object System.Collections.Generic.List[object]
  foreach ($pair in $PairList) {
    $cs = [System.StringComparison]::OrdinalIgnoreCase
    if ($pair.CaseSensitive) { $cs = [System.StringComparison]::Ordinal }
    $pending = @($Snapshot | Where-Object { [string]::Equals($_.Text, $pair.AntesN, $cs) })
    $done = @($Snapshot | Where-Object { [string]::Equals($_.Text, $pair.DespuesN, $cs) })
    if ($pending.Count -eq $pair.Veces) { $state = 'pendiente' }
    elseif ($pending.Count -eq 0 -and $done.Count -ge $pair.Veces) { $state = 'aplicado' }
    elseif ($pending.Count -eq 0) { $state = 'falta' }
    else { $state = 'distinto' }
    $res.Add([pscustomobject]@{ Pair = $pair; State = $state; Pending = $pending; Done = $done })
  }
  , $res.ToArray()
}

# --- Tramo que cambia ------------------------------------------------------------------------

# Cuantos caracteres comparten por el principio y por el final «old» y «new» (distinguiendo
# mayusculas), sin partir un par sustituto. Solo lo que queda en medio se reescribe.
function Get-EditSpan([string]$Old, [string]$New) {
  $max = [Math]::Min($Old.Length, $New.Length)
  $p = 0
  while ($p -lt $max -and $Old[$p] -ceq $New[$p]) { $p++ }
  if ($p -gt 0 -and $p -lt $max -and [char]::IsHighSurrogate($Old[$p - 1])) { $p-- }
  $s = 0
  while ($s -lt ($max - $p) -and $Old[$Old.Length - 1 - $s] -ceq $New[$New.Length - 1 - $s]) { $s++ }
  if ($s -gt 0 -and [char]::IsLowSurrogate($Old[$Old.Length - $s])) { $s-- }
  [pscustomobject]@{ Prefix = $p; Suffix = $s }
}

# ¿Tiene el rango un solo formato de caracter? Word devuelve wdUndefined (o un nombre de fuente
# vacio) en Font.* cuando el rango mezcla valores.
function Get-FormatMix($Rng) {
  $f = $Rng.Font
  $mixed = @()
  foreach ($n in $script:FontProps) {
    if ([int]$f.$n -eq $script:wdUndefined) { $mixed += $n }
  }
  if ([string]$f.Name -eq '') { $mixed += 'Name' }
  $mixed
}

function Get-FontInfo($Rng) {
  $f = $Rng.Font
  $h = [ordered]@{ Name = [string]$f.Name }
  foreach ($n in $script:FontProps) {
    if ($n -eq 'Size') { $h[$n] = [double]$f.$n } else { $h[$n] = [int]$f.$n }
  }
  $h
}

function Compare-FontInfo($A, $B) {
  $diff = @()
  foreach ($k in $A.Keys) {
    if ($A[$k] -ne $B[$k]) { $diff += ("{0}: {1} contra {2}" -f $k, $A[$k], $B[$k]) }
  }
  $diff
}

function Set-FontInfo($Rng, $Info) {
  $f = $Rng.Font
  foreach ($k in @($Info.Keys)) {
    if ($k -eq 'Name') { if ([string]$Info[$k] -ne '') { $f.Name = [string]$Info[$k] }; continue }
    if ([double]$Info[$k] -eq $script:wdUndefined) { continue }      # el primer caracter no puede dar «mezclado», pero por si acaso
    $f.$k = $Info[$k]
  }
}

# Texto del parrafo sin su marca final: «\r» en un parrafo corriente, «\r\a» en el ultimo de una celda.
# Comparacion ORDINAL: EndsWith(string) a secas usa la cultura (ICU, desde .NET 5), que trata «\r» y «\a»
# como caracteres ignorables y daria «true» para cualquier texto.
function Remove-EndMark([string]$Raw) {
  $ord = [System.StringComparison]::Ordinal
  if ($Raw.EndsWith("`r`a", $ord)) { return $Raw.Substring(0, $Raw.Length - 2) }
  if ($Raw.EndsWith("`r", $ord) -or $Raw.EndsWith("`a", $ord)) { return $Raw.Substring(0, $Raw.Length - 1) }
  $Raw
}

# Planea la edicion de UN parrafo sin tocar nada: que tramo cambia, si su formato es uniforme y que
# problemas impiden editarlo. El plan se hace con las posiciones de la foto, antes de editar.
function Get-EditPlan($Doc, $Entry, $Pair) {
  $plan = [ordered]@{
    Entry = $Entry; Pair = $Pair; Content = ''; Prefix = 0; Suffix = 0; OldMid = ''; NewMid = ''
    MidStart = 0; MidEnd = 0; Caps = $false; Mixed = @(); Problems = @(); Warnings = @()
  }
  $rng = $Doc.Range($Entry.Start, $Entry.End)
  $content = Remove-EndMark ([string]$rng.Text)
  $plan.Content = $content
  # Cada caracter del texto debe ocupar una posicion: si no, hay campos, objetos o texto oculto y
  # el rango del tramo no se puede calcular con seguridad.
  if (($Entry.End - $Entry.Start - 1) -ne $content.Length) {
    $plan.Problems += "las posiciones no cuadran con el texto (campos, objetos incrustados o texto oculto): no se edita"
    return [pscustomobject]$plan
  }
  if ([int]$rng.Fields.Count -gt 0) { $plan.Problems += "el parrafo tiene campos (hipervinculos, referencias cruzadas...): no se edita"; return [pscustomobject]$plan }
  if ([int]$rng.Revisions.Count -gt 0) { $plan.Problems += "el parrafo tiene revisiones pendientes: acepte o rechace los cambios antes"; return [pscustomobject]$plan }
  if (-not (Test-SameText (Normalize-Text $content) $Pair.AntesN)) { $plan.Problems += "el texto leido ya no es el esperado"; return [pscustomobject]$plan }

  # Con «todo mayusculas» Range.Text no es el texto guardado: no se pueden compartir prefijo ni sufijo.
  $caps = ([int]$rng.Font.AllCaps -ne 0) -or ([int]$rng.Font.SmallCaps -ne 0)
  $plan.Caps = $caps
  if ($caps) { $span = [pscustomobject]@{ Prefix = 0; Suffix = 0 } } else { $span = Get-EditSpan $content $Pair.Despues }
  if ($caps -and ($content -ceq $Pair.Despues)) { $plan.Problems += "el parrafo ya tiene ese texto"; return [pscustomobject]$plan }
  $plan.Prefix = $span.Prefix; $plan.Suffix = $span.Suffix
  $plan.OldMid = $content.Substring($span.Prefix, $content.Length - $span.Prefix - $span.Suffix)
  $plan.NewMid = $Pair.Despues.Substring($span.Prefix, $Pair.Despues.Length - $span.Prefix - $span.Suffix)
  $plan.MidStart = $Entry.Start + $span.Prefix
  $plan.MidEnd = $plan.MidStart + $plan.OldMid.Length
  if ($plan.OldMid -ceq $plan.NewMid) { $plan.Problems += "no hay nada que cambiar en el parrafo"; return [pscustomobject]$plan }

  # Lo que se reescribe no puede llevar saltos manuales ni objetos: se perderian.
  if ($plan.OldMid -match '[\x00-\x08\x0B\x0C\x0E-\x1F]') { $plan.Problems += "el tramo que cambia trae saltos de linea manuales u objetos incrustados, que se perderian"; return [pscustomobject]$plan }

  if ($plan.OldMid.Length -gt 0) {
    $mid = $Doc.Range($plan.MidStart, $plan.MidEnd)
    if (([string]$mid.Text) -cne $plan.OldMid) { $plan.Problems += "el tramo [$(Get-Excerpt $plan.OldMid 30)] no se leyo igual al pedirlo por posicion"; return [pscustomobject]$plan }
    $plan.Mixed = @(Get-FormatMix $mid)
    if ($plan.Mixed.Count -gt 0) {
      $msg = "el tramo [{0}] tiene formato de caracter mezclado ({1})" -f (Get-Excerpt $plan.OldMid 40), ($plan.Mixed -join ', ')
      if ($Estricto) { $plan.Problems += ($msg + "; -Estricto no lo permite") }
      else { $plan.Warnings += ($msg + ": el texto nuevo toma el formato de su primer caracter") }
    }
  }
  [pscustomobject]$plan
}

# Aplica un plan. Devuelve los avisos. Cualquier desvio lanza una excepcion (no se guarda nada).
function Invoke-EditPlan($Doc, $Plan) {
  $notes = @()
  $e = $Plan.Entry
  $paraBefore = $Doc.Range($e.Start, $e.Start).Paragraphs.Item(1)
  $styleBefore = [string]$paraBefore.Style.NameLocal
  $countBefore = [int]$Doc.Paragraphs.Count
  $delta = $Plan.NewMid.Length - $Plan.OldMid.Length

  # Formato con el que debe quedar el texto nuevo: el del tramo (uniforme) o el de su primer
  # caracter (mezclado); en una insercion pura, el del caracter anterior.
  $fontWanted = $null
  if ($Plan.OldMid.Length -gt 0) { $fontWanted = Get-FontInfo $Doc.Range($Plan.MidStart, $Plan.MidStart + 1) }
  elseif ($Plan.MidStart -gt $e.Start) { $fontWanted = Get-FontInfo $Doc.Range($Plan.MidStart - 1, $Plan.MidStart) }

  # Marcadores que quedan enteros dentro del tramo: Word los borra al reescribir el texto y se
  # vuelven a crear sobre el texto nuevo (una referencia cruzada a un titulo apunta a uno).
  $bookmarks = @()
  if ($Plan.OldMid.Length -gt 0) {
    try {
      $bms = $Doc.Range($Plan.MidStart, $Plan.MidEnd).Bookmarks
      try { $bms.ShowHidden = $true } catch { }
      foreach ($b in $bms) {
        if ([int]$b.Start -ge $Plan.MidStart -and [int]$b.End -le $Plan.MidEnd) {
          $bookmarks += [pscustomobject]@{ Name = [string]$b.Name; Start = [int]$b.Start; End = [int]$b.End }
        }
      }
    } catch { $notes += "no se pudieron leer los marcadores del tramo: $($_.Exception.Message)" }
  }

  $mid = $Doc.Range($Plan.MidStart, $Plan.MidEnd)
  $mid.Text = $Plan.NewMid

  if ($bookmarks.Count -gt 0) {
    try {
      try { $Doc.Bookmarks.ShowHidden = $true } catch { }
      foreach ($b in $bookmarks) {
        if ($Doc.Bookmarks.Exists($b.Name)) { continue }
        if ($b.Start -eq $Plan.MidStart -and $b.End -eq $Plan.MidEnd) {
          [void]$Doc.Bookmarks.Add($b.Name, $Doc.Range($Plan.MidStart, $Plan.MidStart + $Plan.NewMid.Length))
        }
        else { $notes += "se perdio el marcador '$($b.Name)', que cubria solo una parte del texto sustituido" }
      }
    } catch { $notes += "no se pudieron restaurar los marcadores: $($_.Exception.Message)" }
  }

  # Comprobaciones del resultado: texto del parrafo, estilo, numero de parrafos y formato.
  $newEnd = $e.End + $delta
  $after = $Doc.Range($e.Start, $newEnd)
  $got = Remove-EndMark ([string]$after.Text)
  if (-not (Test-SameText $got $Plan.Pair.Despues)) { throw ("El parrafo {0} quedo como [{1}] y se esperaba [{2}]." -f $e.Index, (Get-Excerpt $got), (Get-Excerpt $Plan.Pair.Despues)) }
  if (-not $Plan.Caps -and $got -cne $Plan.Pair.Despues) { throw ("El parrafo {0} quedo con otras mayusculas o espacios: [{1}] y se esperaba [{2}]." -f $e.Index, (Get-Excerpt $got), (Get-Excerpt $Plan.Pair.Despues)) }
  if (([int]$after.Paragraphs.Count) -ne 1) { throw ("El parrafo {0} se partio en {1}." -f $e.Index, $after.Paragraphs.Count) }
  if ([int]$Doc.Paragraphs.Count -ne $countBefore) { throw ("El numero de parrafos del documento cambio al editar el {0}." -f $e.Index) }
  $styleAfter = [string]$after.Paragraphs.Item(1).Style.NameLocal
  if ($styleAfter -ne $styleBefore) { throw ("El estilo del parrafo {0} cambio de [{1}] a [{2}]." -f $e.Index, $styleBefore, $styleAfter) }
  if ($null -ne $fontWanted -and $Plan.NewMid.Length -gt 0) {
    $newRng = $Doc.Range($Plan.MidStart, $Plan.MidStart + $Plan.NewMid.Length)
    $diff = @(Compare-FontInfo (Get-FontInfo $newRng) $fontWanted)
    if ($diff.Count -gt 0) {
      try { Set-FontInfo $newRng $fontWanted } catch { }
      $diff = @(Compare-FontInfo (Get-FontInfo $newRng) $fontWanted)
      if ($diff.Count -gt 0) { $notes += ("el formato del texto nuevo del parrafo {0} no coincide con el del tramo ({1}): revisarlo" -f $e.Index, ($diff -join '; ')) }
      else { $notes += ("se restauro el formato de caracter del texto nuevo del parrafo {0}" -f $e.Index) }
    }
  }
  $notes
}

# --- Programa principal ----------------------------------------------------------------------

function Show-Plan($Plan) {
  $e = $Plan.Entry
  $before = $Plan.Content.Substring(0, $Plan.Prefix)
  $beforeShown = $before; if ($before.Length -gt 14) { $beforeShown = '...' + $before.Substring($before.Length - 14) }
  $tail = $Plan.Content.Substring($Plan.Content.Length - $Plan.Suffix)
  $tailShown = $tail; if ($tail.Length -gt 14) { $tailShown = $tail.Substring(0, 14) + '...' }
  Write-Info ("    parrafo {0}: {1}[{2}] -> [{3}]{4}" -f $e.Index, $beforeShown, (Get-Excerpt $Plan.OldMid 60), (Get-Excerpt $Plan.NewMid 60), $tailShown)
}

function Invoke-Main {
  if (-not (Test-Path -LiteralPath $Docx)) { Write-Host "No existe -Docx: $Docx"; return 2 }
  if (-not (Test-Path -LiteralPath $Pares)) { Write-Host "No existe -Pares: $Pares"; return 2 }
  $docPath = (Resolve-Path -LiteralPath $Docx).Path
  $paresPath = (Resolve-Path -LiteralPath $Pares).Path

  try { $pairList = Read-Pairs $paresPath }
  catch { Write-Host ("ERROR en el .json: " + $_.Exception.Message) -ForegroundColor Red; return 1 }
  $total = ($pairList | Measure-Object -Property Veces -Sum).Sum
  Write-Info ("Pares: {0} ({1} parrafos por sustituir)." -f @($pairList).Count, $total)

  $outPath = $null
  if ($Salida) {
    # Relativa a la ubicacion de PowerShell, no al directorio del proceso (que Set-Location no cambia).
    $outPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Salida)
    if ($outPath -eq $docPath) { $outPath = $null }        # -Salida igual a -Docx: es en el sitio
    elseif (-not $DryRun) { New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outPath) | Out-Null }
  }

  # Copia de seguridad antes de abrir Word (con Word abierto el archivo queda tomado). Con -Salida el
  # original no se toca y no hace falta.
  $backup = $null
  if (-not $DryRun -and -not $SinCopia -and -not $outPath) {
    $dir = Join-Path ([System.IO.Path]::GetTempPath()) 'Algoritmia-Docx'
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $backup = Join-Path $dir ([System.IO.Path]::GetFileNameWithoutExtension($docPath) + '.' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.antes.docx')
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

    $snap = Get-Snapshot $doc
    $found = Find-Pairs $snap $pairList

    # Informe por par.
    $problems = New-Object System.Collections.Generic.List[string]
    foreach ($f in $found) {
      $p = $f.Pair
      switch ($f.State) {
        'pendiente' { Write-Info ("  par {0}: [{1}] -> [{2}]  {3} parrafo(s)" -f $p.Number, (Get-Excerpt $p.AntesN 45), (Get-Excerpt $p.DespuesN 45), $f.Pending.Count) }
        'aplicado' { Write-Info ("  par {0}: ya aplicado ([{1}] esta y [{2}] no)" -f $p.Number, (Get-Excerpt $p.DespuesN 45), (Get-Excerpt $p.AntesN 45)) }
        'falta' { $problems.Add(("par {0}: no aparece [{1}] ni su reemplazo" -f $p.Number, (Get-Excerpt $p.AntesN 60))) }
        'distinto' {
          $where = (@($f.Pending | ForEach-Object { "{0} ({1})" -f $_.Index, (Get-Where $doc $_) }) -join ', ')
          $problems.Add(("par {0}: [{1}] aparece {2} vez/veces (parrafos {3}) y se esperaba {4}" -f $p.Number, (Get-Excerpt $p.AntesN 60), $f.Pending.Count, $where, $p.Veces))
        }
      }
    }
    $nPend = @($found | Where-Object { $_.State -eq 'pendiente' }).Count
    $nDone = @($found | Where-Object { $_.State -eq 'aplicado' }).Count

    if ($problems.Count -eq 0 -and $nDone -eq @($found).Count) {
      Write-Host "Ya esta aplicado: ningun 'antes' aparece y todos los 'despues' estan. No se toco nada."
      $code = 3
    }
    elseif ($problems.Count -gt 0 -or ($nDone -gt 0 -and $nPend -gt 0)) {
      if ($problems.Count -eq 0) { $problems.Add("estado mixto: $nDone par(es) ya aplicados y $nPend pendientes; se aborta en vez de aplicar a medias") }
      foreach ($m in $problems) { Write-Host ("No se puede seguir: " + $m) }
      Write-Host "No se guardo nada."
      $code = 1
    }
    else {
      # Planes de todos los parrafos, antes de editar ninguno.
      $plans = New-Object System.Collections.Generic.List[object]
      foreach ($f in $found) {
        foreach ($entry in $f.Pending) { $plans.Add((Get-EditPlan $doc $entry $f.Pair)) }
      }
      $bad = @($plans | Where-Object { $_.Problems.Count -gt 0 })
      foreach ($pl in $plans) {
        foreach ($w in $pl.Warnings) { Write-Warn ("parrafo {0}: {1}" -f $pl.Entry.Index, $w) }
      }
      if ($bad.Count -gt 0) {
        foreach ($pl in $bad) { foreach ($pr in $pl.Problems) { Write-Host ("No se puede seguir: parrafo {0} ({1}): {2}" -f $pl.Entry.Index, (Get-Excerpt $pl.Entry.Text 40), $pr) } }
        Write-Host "No se guardo nada."
        $code = 1
      }
      elseif ($DryRun) {
        Write-Info "-DryRun: no se escribe nada. Se haria esto (solo se reescribe el tramo entre corchetes):"
        foreach ($pl in ($plans | Sort-Object { $_.Entry.Start })) { Show-Plan $pl }
        Write-Info ("  documento abierto de solo lectura; {0} parrafos; {1} tabla(s) de contenido; {2} lista(s) de tablas/figuras; control de cambios {3}" -f @($snap).Count, $doc.TablesOfContents.Count, $doc.TablesOfFigures.Count, $doc.TrackRevisions)
        if ($NoActualizarIndices) { Write-Info "  no se actualizarian los indices (-NoActualizarIndices)" } else { Write-Info "  se actualizarian la tabla de contenido y las listas de tablas y figuras" }
        if ($outPath) { Write-Info "  se guardaria en: $outPath" } else { Write-Info "  se guardaria en el sitio: $docPath (con copia en %TEMP%\Algoritmia-Docx)" }
      }
      else {
        $trackBefore = [bool]$doc.TrackRevisions
        if ($trackBefore) { Write-Warn "El control de cambios estaba activo: se apaga durante la edicion y se restaura."; $doc.TrackRevisions = $false }
        $countBefore = [int]$doc.Paragraphs.Count

        # De atras hacia adelante: una edicion no corre las posiciones de los parrafos anteriores.
        foreach ($pl in ($plans | Sort-Object { $_.Entry.Start } -Descending)) {
          $notes = Invoke-EditPlan $doc $pl
          foreach ($n in $notes) { Write-Warn ("parrafo {0}: {1}" -f $pl.Entry.Index, $n) }
        }
        Write-Info ("Sustituidos {0} parrafo(s)." -f $plans.Count)

        # Relectura completa: cada «antes» desaparecio, cada «despues» esta las veces pedidas y no
        # cambio el numero de parrafos.
        if ([int]$doc.Paragraphs.Count -ne $countBefore) { throw "El numero de parrafos cambio ($countBefore a $($doc.Paragraphs.Count))." }
        $recheck = Find-Pairs (Get-Snapshot $doc) $pairList
        foreach ($r in $recheck) {
          if ($r.State -ne 'aplicado') { throw ("Tras editar, el par {0} esta en estado '{1}': [{2}]" -f $r.Pair.Number, $r.State, (Get-Excerpt $r.Pair.AntesN 60)) }
        }
        Write-Info "Relectura: todos los pares quedaron aplicados."

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
        $resultado = $docPath; if ($outPath) { $resultado = $outPath }
        $original = $docPath; if ($backup) { $original = $backup }
        Write-Info ("Siguiente paso: python claudeDocs/entregables/tools/verificar_reemplazos.py --docx `"{0}`" --original `"{1}`" --pares `"{2}`"" -f $resultado, $original, $paresPath)
      }
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

# Si se carga con punto (. .\reemplazar_parrafos.ps1 ...) no ejecuta nada: asi se prueban las funciones.
if ($MyInvocation.InvocationName -ne '.') {
  $exit = Invoke-Main
  exit ([int]($exit | Select-Object -Last 1))
}
