# Espera a que el equipo lleve -Min minutos sin entrada humana (GetLastInputInfo). Herramienta, no juego.
param([double]$Min = 4, [double]$MaxMin = 9)
Add-Type @'
using System; using System.Runtime.InteropServices;
public static class IdleW { [StructLayout(LayoutKind.Sequential)] public struct LII { public uint cbSize; public uint dwTime; }
 [DllImport("user32.dll")] static extern bool GetLastInputInfo(ref LII p);
 public static uint Ms() { var l = new LII(); l.cbSize = 8; GetLastInputInfo(ref l); return (uint)Environment.TickCount - l.dwTime; } }
'@
$t0 = Get-Date; $max = 0
while (((Get-Date) - $t0).TotalMinutes -lt $MaxMin) {
    $ms = [IdleW]::Ms(); if ($ms -gt $max) { $max = $ms }
    if ($ms -ge $Min * 60000) { "IDLE {0:HH:mm:ss} {1:N0} s sin uso" -f (Get-Date), ($ms / 1000); exit 0 }
    Start-Sleep -Seconds 2
}
"TIMEOUT {0:HH:mm:ss}: sin {1} min seguidos de inactividad en {2} min (máximo observado {3:N0} s; hoi4 {4})" -f (Get-Date), $Min, $MaxMin, ($max / 1000), $(if (Get-Process hoi4 -ErrorAction SilentlyContinue) { 'abierto' } else { 'cerrado' })
exit 1
