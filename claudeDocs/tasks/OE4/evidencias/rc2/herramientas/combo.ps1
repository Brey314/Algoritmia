#requires -Version 7.0
# Herramienta de la reverificación rc2 (no es código del juego): clics rápidos + ráfaga EN EL MISMO PROCESO,
# porque oe4.ps1 serializa cada orden en una invocación (~1-2 s de arranque) y así no se puede:
#   - un doble clic a < 150 ms (oe4 Clic -Veces 2 da ~150 ms por el Nudge entre clics);
#   - grabar la ignición del N1 desde su primer cuadro (dura 3,5 s).
# Reutiliza la clase nativa Oe4 de oe4.ps1 (se extrae su fuente C#, sin tocar el arnés).
# Uso: combo.ps1 -SX 0.410 -SY 0.896 -Clicks 2 -GapMs 40 -B1 0.6 -GX 0.589 -GY 0.896 -B2 5 -Fps 10 -Prefijo s-n1_ignicion
param(
    [double]$SX, [double]$SY, [int]$Clicks = 1, [int]$GapMs = 40, [int]$HoldMs = 30,
    [double]$B1 = 0.0, [double]$GX = -1, [double]$GY = -1, [double]$B2 = 0.0, [double]$Fps = 10,
    [string]$Prefijo = 'combo'
)
$ErrorActionPreference = 'Stop'
[Threading.Thread]::CurrentThread.CurrentCulture = [Globalization.CultureInfo]::InvariantCulture
$Trabajo = $env:OE4_TRABAJO
$arnes = 'C:\Dev\Algoritmia\claudeDocs\tasks\OE4\herramientas\oe4.ps1'
$txt = [IO.File]::ReadAllText($arnes)
$i0 = $txt.IndexOf("`$Fuente = @'"); $i0 = $txt.IndexOf("`n", $i0) + 1
$i1 = $txt.IndexOf("`n'@", $i0)
$src = $txt.Substring($i0, $i1 - $i0)
$refs = @('System.Drawing.Common', 'System.Drawing.Primitives', 'System.Diagnostics.Process', 'System.ComponentModel.Primitives',
    'System.Collections', 'System.Threading.Thread', 'System.Memory', 'System.Runtime.InteropServices', 'System.Text.Encoding.Extensions')
foreach ($n in 'System.Private.Windows.GdiPlus', 'System.Private.Windows.Core') { try { [void][Reflection.Assembly]::Load($n); $refs += $n } catch { } }
Add-Type -TypeDefinition $src -Language CSharp -ReferencedAssemblies $refs
Add-Type @'
using System; using System.Runtime.InteropServices; using System.Threading;
public static class Btn {
  [StructLayout(LayoutKind.Sequential)] struct MI { public int dx, dy; public uint data, flags, time; public IntPtr extra; }
  [StructLayout(LayoutKind.Sequential)] struct IN { public uint type; public MI mi; }
  [DllImport("user32.dll", SetLastError = true)] static extern uint SendInput(uint n, IN[] i, int size);
  [DllImport("winmm.dll")] static extern uint timeBeginPeriod(uint p);
  [DllImport("winmm.dll")] static extern uint timeEndPeriod(uint p);
  static void S(uint f) { var a = new IN[1]; a[0].type = 0; a[0].mi.flags = f; if (SendInput(1, a, Marshal.SizeOf(typeof(IN))) != 1) throw new Exception("SendInput falló " + Marshal.GetLastWin32Error()); }
  // Devuelve los ms (Stopwatch) en que se envió cada pulsación.
  public static double[] Clicks(int n, int holdMs, int gapMs) {
    timeBeginPeriod(1); var sw = System.Diagnostics.Stopwatch.StartNew(); var t = new double[n];
    try { for (int i = 0; i < n; i++) { t[i] = sw.Elapsed.TotalMilliseconds; S(2); Thread.Sleep(holdMs); S(4); if (i < n - 1) Thread.Sleep(gapMs); } }
    finally { timeEndPeriod(1); }
    return t;
  }
}
'@
[void][Oe4]::SetDpiAware()
$e = Get-Content -LiteralPath (Join-Path $Trabajo 'estado.json') -Raw | ConvertFrom-Json
$h = [Oe4]::FindWindow([int]$e.pid)
if ($h -eq [IntPtr]::Zero) { throw 'No hay ventana del juego.' }
if (-not [Oe4]::EnsureForeground($h, 4)) { Write-Host 'FOCO: no se pudo poner el juego en primer plano; no se envió nada.'; exit 3 }
$c = [Oe4]::Client($h)
function Px([double]$x, [double]$y) { @([int]($c[0] + [math]::Floor($x * $c[2])), [int]($c[1] + [math]::Floor($y * $c[3]))) }
function Settle([int[]]$p) {
    $r = [Oe4]::MoveTo($p[0] + 1, $p[1]); if ($r) { throw $r }
    Start-Sleep -Milliseconds 20
    $r = [Oe4]::MoveTo($p[0], $p[1]); if ($r) { throw $r }
    Start-Sleep -Milliseconds 90
    if ([Oe4]::Foreground() -ne $h) { throw 'FOCO perdido antes de pulsar.' }
}
$dir = New-Item -ItemType Directory -Force (Join-Path $Trabajo 'rafagas')
$log = Join-Path $Trabajo 'acciones.tsv'
function Iso { [DateTime]::UtcNow.ToString('yyyy-MM-dd HH:mm:ss.fff') + 'Z' }
$p = Px $SX $SY
Settle $p
$t0 = Iso
$ts = [Btn]::Clicks($Clicks, $HoldMs, $GapMs)
$gaps = for ($k = 1; $k -lt $ts.Count; $k++) { '{0:0.0}' -f ($ts[$k] - $ts[$k - 1]) }
Add-Content -LiteralPath $log ("$t0`t$(Iso)`tcombo-Clic`t$SX $SY x$Clicks hold${HoldMs} gap${GapMs} -> ($($p[0]),$($p[1])) entre pulsaciones: $($gaps -join ',') ms") -Encoding utf8
Write-Host "Clic x$Clicks en ($SX; $SY) a las $t0; entre pulsaciones (inicio a inicio): $($gaps -join ', ') ms"
if ($B1 -gt 0) {
    $t = Iso; $r = [Oe4]::Burst($h, $c[0], $c[1], $c[2], $c[3], $B1, $Fps, (Join-Path $dir "${Prefijo}_a"))
    Add-Content -LiteralPath $log "$t`t$(Iso)`tcombo-Rafaga`t${Prefijo}_a ${B1}s ${Fps}fps" -Encoding utf8
    Write-Host "Ráfaga A desde $t : $r"
}
if ($GX -ge 0) {
    $q = Px $GX $GY
    Settle $q
    $t = Iso; $null = [Btn]::Clicks(1, $HoldMs, 0)
    Add-Content -LiteralPath $log "$t`t$(Iso)`tcombo-Clic`t$GX $GY x1 -> ($($q[0]),$($q[1]))" -Encoding utf8
    Write-Host "Clic en ($GX; $GY) a las $t"
}
if ($B2 -gt 0) {
    $t = Iso; $r = [Oe4]::Burst($h, $c[0], $c[1], $c[2], $c[3], $B2, $Fps, (Join-Path $dir "${Prefijo}_b"))
    Add-Content -LiteralPath $log "$t`t$(Iso)`tcombo-Rafaga`t${Prefijo}_b ${B2}s ${Fps}fps" -Encoding utf8
    Write-Host "Ráfaga B desde $t : $r"
}
