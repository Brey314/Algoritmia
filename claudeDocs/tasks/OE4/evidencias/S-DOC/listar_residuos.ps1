param([string]$Salida)
$ll = Join-Path $env:USERPROFILE 'AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia'
$out = [Collections.Generic.List[string]]::new()
$out.Add("== Listado de residuos · $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') ==")
$out.Add("-- LocalLow: $ll")
if (Test-Path -LiteralPath $ll) {
  Get-ChildItem -LiteralPath $ll -Force -Recurse | Sort-Object FullName | ForEach-Object {
    $rel = $_.FullName.Substring($ll.Length + 1)
    if ($_.PSIsContainer) { $out.Add("  [d] $rel") } else { $out.Add(("  [f] {0}  ({1} B, {2:yyyy-MM-dd HH:mm:ss})" -f $rel, $_.Length, $_.LastWriteTime)) }
  }
} else { $out.Add('  (no existe)') }
foreach ($k in 'HKCU:\Software\Universidad Catolica de Colombia', 'HKCU:\Software\Universidad Catolica de Colombia\Algoritmia') {
  $out.Add("-- Registro: $k")
  if (Test-Path $k) {
    $i = Get-Item $k
    $out.Add("  subclaves: $((@($i.GetSubKeyNames()) -join ', '))")
    foreach ($n in $i.GetValueNames()) { $out.Add("  valor: $n ($($i.GetValueKind($n)))") }
  } else { $out.Add('  (no existe)') }
}
$exeDir = 'C:\Dev\Algoritmia\Build\Algoritmia'
$out.Add("-- Datos junto al exe: $exeDir\Datos")
if (Test-Path -LiteralPath "$exeDir\Datos" -PathType Container) { Get-ChildItem -LiteralPath "$exeDir\Datos" -Force | ForEach-Object { $out.Add(("  {0}  ({1} B, {2:yyyy-MM-dd HH:mm:ss.fff})" -f $_.Name, $_.Length, $_.LastWriteTime)) } } elseif (Test-Path -LiteralPath "$exeDir\Datos") { $out.Add('  (es un ARCHIVO, no una carpeta)') } else { $out.Add('  (no existe)') }
[IO.File]::WriteAllLines($Salida, $out, [Text.UTF8Encoding]::new($false))
$out | Select-Object -First 60
