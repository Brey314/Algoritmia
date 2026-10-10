<#
.SYNOPSIS
  Agrega una fila al final de una tabla de un .docx, con Word por COM, conservando el formato de la
  fila anterior (pensado para las tablas de control de cambios).

.DESCRIPTION
  Herramienta generica (no sabe de ningun documento). Hermana de reemplazar_parrafos.ps1 e
  insertar_parrafos.ps1: mismo patron (-DryRun, copia en %TEMP%, indices, relectura, Word cerrado
  siempre). Compatible con Windows PowerShell 5.1 y con pwsh. Recibe una lista de filas en un .json y,
  en un solo guardado:
    1. Para cada entrada busca la tabla (del cuerpo, tambien anidada) cuya ULTIMA fila tiene en su
       PRIMERA celda el texto "tabla_ultima_fila". La comparacion usa el texto normalizado (sin marcas
       de parrafo ni de celda, espacios duros como espacios, espacios colapsados) y no distingue
       mayusculas.
    2. Exactamente UNA tabla debe coincidir; si coinciden 0 o varias, aborta SIN guardar y dice
       cuantas. La ultima fila de esa tabla debe tener tantas celdas como textos trae "celdas".
    3. Todos los destinos se buscan ANTES de tocar nada, sobre el estado original del documento: una
       entrada no puede anclarse en la fila que otra entrada va a agregar. Dos entradas con la misma
       ancla caen en la misma tabla y se agregan en el orden del .json (la segunda, debajo de la
       primera).
    4. Agrega la fila con Rows.Add() (sin argumento: al final, con el formato de la ultima fila, como
       al pulsar Tab en su ultima celda) y escribe el texto de cada celda con Range.Text. El texto toma
       el formato de caracter de la marca de celda de la fila nueva; si difiere del primer caracter de
       la celda homologa de la fila anterior, avisa (no lo cambia).
    5. Apaga el control de cambios durante la edicion y lo restaura antes de guardar.
    6. Actualiza la tabla de contenido y las listas de tablas y figuras (salvo -NoActualizarIndices).
    7. Antes de guardar vuelve a leer las tablas: cada tabla tocada tiene las filas que debia, la fila
       nueva tiene el numero de celdas pedido y cada celda se lee igual a lo pedido (texto
       normalizado); las demas tablas no cambiaron. Si algo no cuadra, no guarda nada. Word se cierra
       siempre (bloque finally).

  Formato del .json (UTF-8): una lista de objetos.
    [ {"tabla_ultima_fila": "01/10/2026", "celdas": ["09/10/2026", "Descripcion del cambio", "Autor"]} ]
  "tabla_ultima_fila" es el texto de la primera celda de la ultima fila de la tabla destino, antes de
  agregar nada. "celdas" son los textos de la fila nueva, de izquierda a derecha, al menos 2. Todos son
  texto: una fecha o un numero van entre comillas. Una celda admite un solo parrafo: un salto de linea
  es un error. Se admite ademas la clave "nota" (se ignora); cualquier otra clave es un error (atrapa
  "celdas" mal escrito).

  Idempotente: una entrada ya esta aplicada si la ultima fila de alguna tabla tiene la primera celda
  igual a celdas[0] y la segunda igual a celdas[1]. Si no, y solo entonces, se barren las filas de la
  tabla destino (o, si ninguna tabla tiene ya la ancla, las de todas) buscando esas dos celdas: asi
  tambien cuenta como aplicada una fila que otra entrada de la misma tabla dejo debajo de ella. Las
  aplicadas se saltan; si lo estan todas, termina con codigo 3 sin tocar el archivo. Un estado mixto
  (unas aplicadas y otras pendientes) se aplica: se agregan solo las pendientes. Por eso la ancla de una
  entrada ya aplicada puede no encontrarse: no es un error.

  Alcance: tablas del cuerpo del documento y las anidadas en ellas. No toca encabezados, pies ni cuadros
  de texto. Una tabla cuya ultima fila no se puede leer celda a celda (celdas combinadas en vertical, por
  ejemplo) no puede ser destino; se cuenta aparte en el informe.

.PARAMETER Docx
  El .docx a modificar. Se cambia en el sitio; antes de abrirlo en Word se deja una copia en
  %TEMP%\Algoritmia-Docx.

.PARAMETER Filas
  El .json de filas.

.PARAMETER DryRun
  Abre el documento de solo lectura, comprueba todas las entradas y dice que haria (tabla, filas antes y
  despues, textos, formato de la fila modelo) sin escribir nada.

.PARAMETER NoActualizarIndices
  No actualiza la tabla de contenido ni las listas de tablas y figuras.

.NOTES
  Codigos de salida: 0 bien (o DryRun sin problemas); 1 error (no se guardo nada); 2 uso (archivo
  inexistente); 3 ya estaba aplicado, no se toco nada.
  Si se carga con punto (. .\agregar_fila_tabla.ps1 -Docx x -Filas y) no ejecuta nada: asi se prueban las
  funciones.

.EXAMPLE
  pwsh -NoProfile -File "claudeDocs/entregables/tools/agregar_fila_tabla.ps1" `
       -Docx "docs/Solucion_OE2_Diseno_final.docx" `
       -Filas "claudeDocs/entregables/tools/pares/control_cambios.json" -DryRun
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)][string]$Docx,
  [Parameter(Mandatory = $true)][string]$Filas,
  [switch]$DryRun,
  [switch]$NoActualizarIndices
)

$ErrorActionPreference = 'Stop'

# Constantes de Word.
$script:wdCharacter = 1         # WdUnits.wdCharacter
$script:MaxCols = 64            # Word admite 63 columnas: tope del sondeo de celdas de una fila

# Propiedades de Font que cuentan como "formato de caracter" al comparar la fila nueva con la anterior.
$script:FontProps = @('Bold', 'Italic', 'Underline', 'StrikeThrough', 'Superscript', 'Subscript',
  'AllCaps', 'SmallCaps', 'Hidden', 'Color', 'Size')

function Write-Info([string]$Message) { Write-Host $Message }
function Write-Warn([string]$Message) { Write-Host ("AVISO: " + $Message) -ForegroundColor Yellow }

# --- Texto -----------------------------------------------------------------------------------

# Texto de una celda o parrafo sin marcas de parrafo ni de celda (el Range.Text de una celda termina en
# CR + BEL), saltos ni espacios duros; espacios colapsados.
function Normalize-Text([string]$Text) {
  if ($null -eq $Text) { return '' }
  $t = $Text -replace '[\r\a\v\f]', ' '
  $t = $t.Replace([string][char]0x00A0, ' ')
  ($t -replace '\s+', ' ').Trim()
}

# Igualdad de textos sin distinguir mayusculas (Word devuelve en mayusculas, por COM, el texto de los
# estilos "todo mayusculas").
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

# Lo que una celda no admite: un salto de linea o de parrafo (o cualquier otro caracter de control salvo
# el tabulador) partiria el contenido en mas de un parrafo.
function Test-HasBreak([string]$Text) {
  $Text -match '[\x00-\x08\x0A-\x1F]'
}

# "[a] | [b] | [c]" para los mensajes.
function Format-Row([string[]]$Cells) {
  (@($Cells | ForEach-Object { '[' + (Get-Excerpt $_ 40) + ']' })) -join ' | '
}

# --- Lectura de las filas --------------------------------------------------------------------

# Convierte un JsonElement de System.Text.Json en objetos .NET planos: OrderedDictionary, List[object],
# string, double, bool o $null. Devuelve siempre con la coma unaria para que PowerShell no desenrolle listas.
function ConvertFrom-JsonElement($El) {
  switch ($El.ValueKind.ToString()) {
    'Object' {
      $d = New-Object System.Collections.Specialized.OrderedDictionary
      foreach ($p in $El.EnumerateObject()) { $d.Add($p.Name, (ConvertFrom-JsonElement $p.Value)) }
      return , $d
    }
    'Array' {
      $l = New-Object 'System.Collections.Generic.List[object]'
      foreach ($x in $El.EnumerateArray()) { $l.Add((ConvertFrom-JsonElement $x)) }
      return , $l
    }
    'String' { return , $El.GetString() }
    'Number' { return , $El.GetDouble() }
    'True' { return , $true }
    'False' { return , $false }
    default { return , $null }
  }
}

# Lee un JSON sin interpretar nada: ni ConvertFrom-Json, que en pwsh convierte en fechas los textos que
# parecen ISO ("2026-09-02T00:00:00") y en 5.1 devuelve objetos propios, ni nada que cambie un texto.
# En pwsh usa System.Text.Json; en Windows PowerShell 5.1 (.NET Framework, sin ese tipo) usa
# JavaScriptSerializer, que da Dictionary[string,object] y object[]: ambos sirven igual a Read-Filas.
function Read-JsonPlain([string]$Raw) {
  $jsonDoc = 'System.Text.Json.JsonDocument' -as [type]
  if ($null -ne $jsonDoc) {
    $json = $jsonDoc::Parse($Raw)
    try { return , (ConvertFrom-JsonElement $json.RootElement) }
    finally { $json.Dispose() }
  }
  Add-Type -AssemblyName System.Web.Extensions
  $ser = New-Object System.Web.Script.Serialization.JavaScriptSerializer
  $ser.MaxJsonLength = [int]::MaxValue
  , ($ser.DeserializeObject($Raw))
}

# Lee y valida el .json. Devuelve la lista de entradas {Number, Anchor, AnchorN, Cells, CellsN}; Cells son
# los textos tal como se escribiran y CellsN los mismos normalizados (con lo que se compara).
function Read-Filas([string]$Path) {
  $enc = New-Object System.Text.UTF8Encoding($false)
  $raw = [System.IO.File]::ReadAllText($Path, $enc)       # quita el BOM si lo trae
  try { $root = Read-JsonPlain $raw }
  catch { throw "no es un JSON valido: $($_.Exception.Message)" }
  if ($root -isnot [System.Collections.IList]) { throw 'el .json debe ser una lista de objetos {tabla_ultima_fila, celdas}.' }
  $out = New-Object System.Collections.Generic.List[object]
  $n = 0
  foreach ($el in $root) {
    $n++
    if ($el -isnot [System.Collections.IDictionary]) { throw "La entrada $n no es un objeto {tabla_ultima_fila, celdas}." }
    $anchor = $null; $cells = $null
    foreach ($k in @($el.Keys)) {
      $v = $el[$k]
      switch -CaseSensitive ([string]$k) {
        'tabla_ultima_fila' {
          if ($v -isnot [string]) { throw "La entrada ${n}: 'tabla_ultima_fila' debe ser texto." }
          $anchor = $v
        }
        'celdas' {
          if ($v -isnot [System.Collections.IList]) { throw "La entrada ${n}: 'celdas' debe ser una lista de textos." }
          $cells = New-Object System.Collections.Generic.List[string]
          $j = 0
          foreach ($x in $v) {
            $j++
            if ($x -isnot [string]) { throw "La entrada ${n}: la celda $j no es texto (las fechas y los numeros van entre comillas)." }
            $cells.Add($x)
          }
        }
        'nota' { }
        default { throw "La entrada $n trae la clave desconocida '$k' (validas: tabla_ultima_fila, celdas, nota)." }
      }
    }
    if ($null -eq $anchor) { throw "La entrada $n no trae 'tabla_ultima_fila'." }
    if ($null -eq $cells) { throw "La entrada $n no trae 'celdas'." }
    $anchorN = Normalize-Text $anchor
    if ($anchorN -eq '') { throw "La entrada $n trae un 'tabla_ultima_fila' vacio." }
    if ($cells.Count -lt 2) { throw "La entrada ${n}: 'celdas' necesita al menos 2 textos (las dos primeras identifican la fila al comprobar si ya esta aplicada)." }
    $cellsN = New-Object System.Collections.Generic.List[string]
    $j = 0
    foreach ($x in $cells) {
      $j++
      if (Test-HasBreak $x) { throw "La entrada ${n}: la celda $j trae un salto de linea o de parrafo (una celda admite un solo parrafo)." }
      $cellsN.Add((Normalize-Text $x))
    }
    if ($cellsN[0] -eq '' -or $cellsN[1] -eq '') { throw "La entrada ${n}: las celdas 1 y 2 no pueden estar vacias (identifican la fila)." }
    $out.Add([pscustomobject]@{
      Number  = $n
      Anchor  = $anchor
      AnchorN = $anchorN
      Cells   = $cells.ToArray()
      CellsN  = $cellsN.ToArray()
    })
  }
  if ($out.Count -eq 0) { throw 'el .json no trae ninguna entrada.' }
  # Dos entradas con las mismas celdas 1 y 2 serian la misma fila dos veces.
  for ($a = 0; $a -lt $out.Count; $a++) {
    for ($b = $a + 1; $b -lt $out.Count; $b++) {
      if ((Test-SameText $out[$a].CellsN[0] $out[$b].CellsN[0]) -and (Test-SameText $out[$a].CellsN[1] $out[$b].CellsN[1])) {
        throw "Las entradas $($out[$a].Number) y $($out[$b].Number) traen la misma fila (celdas 1 y 2 iguales): [$(Get-Excerpt $out[$a].CellsN[0] 30)] | [$(Get-Excerpt $out[$a].CellsN[1] 30)]."
      }
    }
  }
  , $out.ToArray()
}

# --- Utilidades sobre el documento -----------------------------------------------------------

function New-WordApplication { New-Object -ComObject Word.Application }

function Remove-ComObject($Obj) {
  if ($null -ne $Obj) { try { [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($Obj) } catch { } }
}

# Cuantas celdas tiene la fila $RowIndex, sondeando Table.Cell(fila, columna) hasta que Word dice que no
# existe. No usa Rows.Item(i).Cells, que Word rechaza si la tabla tiene celdas combinadas en vertical, y
# cuenta una celda combinada en horizontal como una sola.
function Get-RowCellCount($Table, [int]$RowIndex) {
  $c = 0
  while ($c -lt $script:MaxCols) {
    try { $null = $Table.Cell($RowIndex, $c + 1) } catch { break }
    $c++
  }
  $c
}

# Las tablas del cuerpo en orden de documento, cada una seguida de las anidadas en ella. Document.Tables
# trae solo las de primer nivel (Table.Tables, las anidadas); por si trajera tambien las anidadas, la
# posicion Start:End de cada tabla evita repetirlas.
function Add-TablesRec($Tables, $Acc, $Seen) {
  foreach ($t in $Tables) {
    $key = '{0}:{1}' -f [int]$t.Range.Start, [int]$t.Range.End
    if ($Seen.ContainsKey($key)) { continue }
    $Seen[$key] = $true
    $Acc.Add($t)
    Add-TablesRec $t.Tables $Acc $Seen
  }
}

# Una pasada por las tablas: indice (1 = primera en orden de documento), objeto Word, filas, celdas de la
# ultima fila y el texto normalizado de su primera y segunda celda. Una tabla cuya ultima fila no se puede
# leer queda con Readable = $false y el motivo en Why (no es un error: puede no ser la que se busca).
function Get-TableSnapshot($Doc) {
  $tabs = New-Object System.Collections.Generic.List[object]
  Add-TablesRec $Doc.Tables $tabs @{}
  $out = New-Object System.Collections.Generic.List[object]
  $i = 0
  foreach ($t in $tabs) {
    $i++
    $item = [ordered]@{ Index = $i; Table = $t; Readable = $false; Rows = 0; Cells = 0; First = ''; Second = ''; Why = '' }
    try {
      $n = [int]$t.Rows.Count
      if ($n -lt 1) { throw 'la tabla no tiene filas' }
      $c = Get-RowCellCount $t $n
      if ($c -lt 1) { throw 'las celdas de la ultima fila no se pueden leer una a una' }
      $item['First'] = Normalize-Text ([string]$t.Cell($n, 1).Range.Text)
      if ($c -ge 2) { $item['Second'] = Normalize-Text ([string]$t.Cell($n, 2).Range.Text) }
      $item['Rows'] = $n; $item['Cells'] = $c; $item['Readable'] = $true
    }
    catch { $item['Why'] = $_.Exception.Message }
    $out.Add([pscustomobject]$item)
  }
  , $out.ToArray()
}

# Ficha de la fila modelo para el informe: estilo del primer parrafo de su primera celda y fuente.
function Get-CellFicha($Table, [int]$RowIndex) {
  try {
    $rg = $Table.Cell($RowIndex, 1).Range
    $style = [string]$rg.Paragraphs.Item(1).Style.NameLocal
    "estilo [{0}], fuente {1} {2}, negrita {3}" -f $style, [string]$rg.Font.Name, [string]$rg.Font.Size, [string]$rg.Font.Bold
  }
  catch { 'formato no legible' }
}

# --- Formato de caracter ---------------------------------------------------------------------

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

# --- Busqueda de las entradas (la comparten -DryRun y la edicion) ----------------------------

# Fila (1..Rows) de la tabla cuyas dos primeras celdas son $First y $Second (texto normalizado, sin
# distinguir mayusculas), o 0 si no hay ninguna. Lee celda a celda; una fila que no se puede leer cuenta
# como que no.
function Find-RowIndex($Item, [string]$First, [string]$Second) {
  for ($r = 1; $r -le $Item.Rows; $r++) {
    try {
      if ((Test-SameText (Normalize-Text ([string]$Item.Table.Cell($r, 1).Range.Text)) $First) -and
          (Test-SameText (Normalize-Text ([string]$Item.Table.Cell($r, 2).Range.Text)) $Second)) { return $r }
    }
    catch { }
  }
  0
}

# Estado de cada entrada sobre la foto de las tablas:
#   'aplicada'  la ultima fila de alguna tabla ya empieza por celdas[0] y celdas[1]; o, si no, esa fila ya
#               esta en otra posicion de la tabla destino (otra entrada cayo debajo de ella, o se agrego algo
#               despues) o, si la ancla ya no aparece, en cualquier tabla (Row dice en que fila);
#   'pendiente' exactamente una tabla tiene en su ultima fila, primera celda, la ancla, y esa fila tiene
#               tantas celdas como se dan;
#   'problema'  cualquier otra cosa (Problem dice cual).
function Find-Entries($Snap, $EntryList) {
  $unread = @($Snap | Where-Object { -not $_.Readable }).Count
  $res = New-Object System.Collections.Generic.List[object]
  foreach ($e in $EntryList) {
    $applied = @($Snap | Where-Object { $_.Readable -and (Test-SameText $_.First $e.CellsN[0]) -and (Test-SameText $_.Second $e.CellsN[1]) })
    $targets = @($Snap | Where-Object { $_.Readable -and (Test-SameText $_.First $e.AnchorN) })
    $state = 'problema'; $target = $null; $problem = ''; $row = 0
    if ($applied.Count -ge 1) { $state = 'aplicada'; $target = $applied[0]; $row = $target.Rows }
    else {
      # Solo se barren las filas cuando la ultima no dice nada: con una ancla ambigua no se busca.
      $scan = @()
      if ($targets.Count -eq 1) { $scan = $targets }
      elseif ($targets.Count -eq 0) { $scan = @($Snap | Where-Object { $_.Readable }) }
      foreach ($s in $scan) {
        $row = Find-RowIndex $s $e.CellsN[0] $e.CellsN[1]
        if ($row -gt 0) { $state = 'aplicada'; $target = $s; break }
      }
    }
    if ($state -ne 'aplicada') {
      if ($targets.Count -eq 1) {
        $target = $targets[0]
        if ($target.Cells -ne $e.Cells.Count) {
          $problem = "la ultima fila de la tabla {0} tiene {1} celdas y la entrada trae {2}" -f $target.Index, $target.Cells, $e.Cells.Count
        }
        else { $state = 'pendiente' }
      }
      elseif ($targets.Count -eq 0) {
        $problem = "ninguna tabla tiene [{0}] en la primera celda de su ultima fila (0 coincidencias; se esperaba 1) ni trae ya la fila nueva" -f (Get-Excerpt $e.Anchor 50)
        if ($unread -gt 0) { $problem += "; {0} tabla(s) no se pudieron leer" -f $unread }
        $problem += ". Las anclas se buscan en el estado ORIGINAL del documento, no en filas que otra entrada va a agregar."
      }
      else {
        $problem = "{0} tablas tienen [{1}] en la primera celda de su ultima fila (tablas {2}; se esperaba 1)" -f $targets.Count, (Get-Excerpt $e.Anchor 50), ((@($targets | ForEach-Object { $_.Index })) -join ', ')
      }
    }
    $res.Add([pscustomobject]@{ Entry = $e; State = $state; Target = $target; Row = $row; Problem = $problem })
  }
  , $res.ToArray()
}

# --- Edicion ---------------------------------------------------------------------------------

# Agrega la fila de UNA entrada al final de su tabla. Devuelve {Entry, TableIndex, RowsBefore, NewRow, Notes}.
# Cualquier desvio lanza una excepcion (no se guarda nada).
function Invoke-AddRow($Found) {
  $e = $Found.Entry
  $t = $Found.Target.Table
  $count = $e.Cells.Count
  $notes = @()
  $rowsBefore = [int]$t.Rows.Count
  $modelRow = $rowsBefore

  # Formato de caracter del primer caracter de cada celda de la fila modelo (la ultima actual), para
  # comparar despues. Es informativo: si no se puede leer, solo se pierde el aviso.
  $wanted = @()
  try {
    for ($c = 1; $c -le $count; $c++) { $wanted += , (Get-FontInfo $t.Cell($modelRow, $c).Range.Characters.Item(1)) }
  }
  catch { $wanted = @(); $notes += "no se pudo leer el formato de la fila modelo, no se compara: $($_.Exception.Message)" }

  [void]$t.Rows.Add()
  $newRow = [int]$t.Rows.Count
  if ($newRow -ne $rowsBefore + 1) { throw ("Entrada {0}: la tabla {1} paso de {2} a {3} filas al agregar (se esperaba una mas)." -f $e.Number, $Found.Target.Index, $rowsBefore, $newRow) }
  $got = Get-RowCellCount $t $newRow
  if ($got -ne $count) { throw ("Entrada {0}: la fila nueva de la tabla {1} tiene {2} celdas y se esperaban {3}." -f $e.Number, $Found.Target.Index, $got, $count) }
  for ($c = 1; $c -le $count; $c++) { $t.Cell($newRow, $c).Range.Text = [string]$e.Cells[$c - 1] }

  # El texto debe tener el formato de la fila anterior (Rows.Add copia el de la marca de celda).
  if ($wanted.Count -eq $count) {
    try {
      for ($c = 1; $c -le $count; $c++) {
        $txt = $t.Cell($newRow, $c).Range
        [void]$txt.MoveEnd($script:wdCharacter, -1)          # sin la marca de celda
        if ([int]$txt.End -le [int]$txt.Start) { continue }  # celda vacia: no hay texto que comparar
        $diff = @(Compare-FontInfo (Get-FontInfo $txt.Characters.Item(1)) $wanted[$c - 1])
        if ($diff.Count -gt 0) { $notes += ("celda {0} de la fila nueva: el formato del texto difiere del de la fila anterior ({1}): revisarlo" -f $c, ($diff -join '; ')) }
      }
    }
    catch { $notes += "no se pudo comparar el formato de la fila nueva: $($_.Exception.Message)" }
  }
  [pscustomobject]@{ Entry = $e; TableIndex = $Found.Target.Index; RowsBefore = $rowsBefore; NewRow = $newRow; Notes = $notes }
}

# Relectura completa antes de guardar, con una foto nueva de las tablas: mismo numero de tablas; cada tabla
# tocada tiene las filas de antes mas las agregadas; las demas no cambiaron; y cada fila agregada tiene el
# numero de celdas pedido y se lee igual a lo pedido (texto normalizado; sin distinguir mayusculas solo si
# la celda es "todo mayusculas", porque entonces Word devuelve el texto en mayusculas).
function Confirm-Rows($Doc, $Before, $Done) {
  $after = Get-TableSnapshot $Doc
  if (@($after).Count -ne @($Before).Count) { throw ("El numero de tablas cambio ({0} a {1})." -f @($Before).Count, @($after).Count) }
  $added = @{}
  foreach ($d in $Done) { $k = [string]$d.TableIndex; $added[$k] = 1 + [int]$added[$k] }
  foreach ($b in $Before) {
    if (-not $b.Readable) { continue }
    $a = $after[$b.Index - 1]
    $k = [string]$b.Index
    $expected = $b.Rows + [int]$added[$k]
    if (-not $a.Readable) { throw ("La tabla {0} dejo de poder leerse tras editar: {1}" -f $b.Index, $a.Why) }
    if ($a.Rows -ne $expected) { throw ("La tabla {0} tiene {1} filas y se esperaban {2}." -f $b.Index, $a.Rows, $expected) }
    if ($null -eq $added[$k] -and -not (Test-SameText $a.First $b.First)) { throw ("La tabla {0}, que no se debia tocar, cambio su ultima fila." -f $b.Index) }
  }
  foreach ($d in $Done) {
    $t = $after[$d.TableIndex - 1].Table
    $count = $d.Entry.Cells.Count
    if ((Get-RowCellCount $t $d.NewRow) -ne $count) { throw ("Entrada {0}: la fila {1} de la tabla {2} no tiene {3} celdas." -f $d.Entry.Number, $d.NewRow, $d.TableIndex, $count) }
    for ($c = 1; $c -le $count; $c++) {
      $rg = $t.Cell($d.NewRow, $c).Range
      $caps = ([int]$rg.Font.AllCaps -ne 0) -or ([int]$rg.Font.SmallCaps -ne 0)
      $cs = [System.StringComparison]::Ordinal
      if ($caps) { $cs = [System.StringComparison]::OrdinalIgnoreCase }
      $got = Normalize-Text ([string]$rg.Text)
      if (-not [string]::Equals($got, $d.Entry.CellsN[$c - 1], $cs)) {
        throw ("Entrada {0}: la celda {1} de la fila {2} (tabla {3}) quedo como [{4}] y se esperaba [{5}]." -f $d.Entry.Number, $c, $d.NewRow, $d.TableIndex, (Get-Excerpt $got 60), (Get-Excerpt $d.Entry.CellsN[$c - 1] 60))
      }
    }
  }
}

# --- Programa principal ----------------------------------------------------------------------

function Invoke-Main {
  if (-not (Test-Path -LiteralPath $Docx)) { Write-Host "No existe -Docx: $Docx"; return 2 }
  if (-not (Test-Path -LiteralPath $Filas)) { Write-Host "No existe -Filas: $Filas"; return 2 }
  $docPath = (Resolve-Path -LiteralPath $Docx).Path
  $filasPath = (Resolve-Path -LiteralPath $Filas).Path

  try { $entries = Read-Filas $filasPath }
  catch { Write-Host ("ERROR en el .json: " + $_.Exception.Message) -ForegroundColor Red; return 1 }
  Write-Info ("Entradas: {0}." -f @($entries).Count)

  # Copia de seguridad antes de abrir Word (con Word abierto el archivo queda tomado).
  $backup = $null
  if (-not $DryRun) {
    $dir = Join-Path ([System.IO.Path]::GetTempPath()) 'Algoritmia-Docx'
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $backup = Join-Path $dir ([System.IO.Path]::GetFileNameWithoutExtension($docPath) + '.' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.antes-fila.docx')
    Copy-Item -LiteralPath $docPath -Destination $backup
    Write-Info "Copia de seguridad: $backup"
  }

  $word = $null; $doc = $null; $saved = $false; $code = 0
  try {
    $word = New-WordApplication
    $word.Visible = $false
    $word.DisplayAlerts = 0                    # wdAlertsNone
    try { $word.Options.BackgroundSave = $false } catch { }
    $doc = $word.Documents.Open($docPath, $false, [bool]$DryRun, $false)    # sin conversion, solo lectura si -DryRun, fuera de recientes
    if (-not $DryRun -and [bool]$doc.ReadOnly) { throw "Word abrio el documento de solo lectura (esta abierto en otro Word o el archivo es de solo lectura). Cierrelo y repita." }

    $snap = Get-TableSnapshot $doc
    $unread = @($snap | Where-Object { -not $_.Readable })
    Write-Info ("Tablas en el documento: {0} ({1} sin poder leer su ultima fila)." -f @($snap).Count, $unread.Count)
    if ($DryRun) { foreach ($u in $unread) { Write-Info ("  tabla {0}: no se puede leer ({1})" -f $u.Index, (Get-Excerpt $u.Why 80)) } }
    $found = Find-Entries $snap $entries

    # Informe por entrada. $seq cuenta cuantas filas lleva ya agregadas cada tabla, para decir en que
    # numero de fila caera cada una cuando dos entradas comparten tabla.
    $problems = New-Object System.Collections.Generic.List[string]
    $seq = @{}
    $nPend = 0
    foreach ($f in $found) {
      $e = $f.Entry
      switch ($f.State) {
        'aplicada' {
          Write-Info ("  entrada {0}: ya aplicada (tabla {1}, fila {2} de {3}: empieza por {4})" -f $e.Number, $f.Target.Index, $f.Row, $f.Target.Rows, (Format-Row @($e.Cells[0], $e.Cells[1])))
        }
        'pendiente' {
          $nPend++
          $k = [string]$f.Target.Index
          $seq[$k] = 1 + [int]$seq[$k]
          $before = $f.Target.Rows + $seq[$k] - 1
          $verb = 'se agrega'; if ($DryRun) { $verb = 'se agregaria' }
          Write-Info ("  entrada {0}: tabla {1} ({2} celdas): {3} la fila {4} (filas {5} -> {6}): {7}" -f $e.Number, $f.Target.Index, $e.Cells.Count, $verb, ($before + 1), $before, ($before + 1), (Format-Row $e.Cells))
          if ($DryRun) { Write-Info ("    fila modelo (la {0}): {1}" -f $before, (Get-CellFicha $f.Target.Table $before)) }
        }
        default { $problems.Add(("entrada {0}: {1}" -f $e.Number, $f.Problem)) }
      }
    }

    if ($problems.Count -gt 0) {
      foreach ($m in $problems) { Write-Host ("No se puede seguir: " + $m) }
      Write-Host "No se guardo nada."
      $code = 1
    }
    elseif ($nPend -eq 0) {
      Write-Host "Ya esta aplicado: todas las filas estan ya en su tabla. No se toco nada."
      $code = 3
    }
    elseif ($DryRun) {
      Write-Info "-DryRun: no se escribe nada."
      Write-Info ("  documento abierto de solo lectura; {0} tabla(s); {1} tabla(s) de contenido; {2} lista(s) de tablas/figuras; control de cambios {3}" -f @($snap).Count, $doc.TablesOfContents.Count, $doc.TablesOfFigures.Count, $doc.TrackRevisions)
      if ($NoActualizarIndices) { Write-Info "  no se actualizarian los indices (-NoActualizarIndices)" } else { Write-Info "  se actualizarian la tabla de contenido y las listas de tablas y figuras" }
      Write-Info "  se guardaria en el sitio: $docPath (con copia en %TEMP%\Algoritmia-Docx)"
    }
    else {
      $trackBefore = [bool]$doc.TrackRevisions
      if ($trackBefore) { Write-Warn "El control de cambios estaba activo: se apaga durante la edicion y se restaura."; $doc.TrackRevisions = $false }

      # En el orden del .json: dos entradas de la misma tabla quedan en ese orden, de arriba abajo.
      $done = New-Object System.Collections.Generic.List[object]
      foreach ($f in $found) {
        if ($f.State -ne 'pendiente') { continue }
        $r = @(Invoke-AddRow $f) | Select-Object -Last 1
        $done.Add($r)
        foreach ($nt in $r.Notes) { Write-Warn ("entrada {0}: {1}" -f $r.Entry.Number, $nt) }
        Write-Info ("  entrada {0}: tabla {1}: filas {2} -> {3}: {4}" -f $r.Entry.Number, $r.TableIndex, $r.RowsBefore, $r.NewRow, (Format-Row $r.Entry.Cells))
      }
      Write-Info ("Agregadas {0} fila(s)." -f $done.Count)

      Confirm-Rows $doc $snap $done
      Write-Info "Relectura: cada fila quedo como se pidio y las demas tablas no cambiaron."

      if (-not $NoActualizarIndices) {
        $doc.Repaginate()
        for ($i = 1; $i -le $doc.TablesOfContents.Count; $i++) { $doc.TablesOfContents.Item($i).Update() }
        for ($i = 1; $i -le $doc.TablesOfFigures.Count; $i++) { $doc.TablesOfFigures.Item($i).Update() }
        Write-Info "Indices actualizados."
      }
      if ($trackBefore) { $doc.TrackRevisions = $true }
      $doc.Save(); $saved = $true
      Write-Info "Guardado en el sitio: $docPath"
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
  # Sin cambios por ser idempotente: la copia es identica al original y no sirve de nada.
  if ($code -eq 3 -and $backup) { try { Remove-Item -LiteralPath $backup -Force; Write-Info "Sin cambios: se descarto la copia de seguridad." } catch { } }
  return $code
}

# Si se carga con punto (. .\agregar_fila_tabla.ps1 ...) no ejecuta nada: asi se prueban las funciones.
if ($MyInvocation.InvocationName -ne '.') {
  $exit = Invoke-Main
  exit ([int]($exit | Select-Object -Last 1))
}
