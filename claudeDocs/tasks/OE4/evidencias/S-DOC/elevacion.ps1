param([int]$ProcId)
Add-Type @"
using System; using System.Runtime.InteropServices;
public static class Elev {
  [DllImport("kernel32.dll")] static extern IntPtr OpenProcess(uint a, bool i, int p);
  [DllImport("advapi32.dll")] static extern bool OpenProcessToken(IntPtr h, uint a, out IntPtr t);
  [DllImport("advapi32.dll")] static extern bool GetTokenInformation(IntPtr t, int c, out int v, int l, out int r);
  [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr h);
  public static string Query(int pid) {
    IntPtr h = OpenProcess(0x1000, false, pid); if (h == IntPtr.Zero) return "no se pudo abrir el proceso";
    IntPtr t; if (!OpenProcessToken(h, 0x0008, out t)) { CloseHandle(h); return "no se pudo abrir el token"; }
    int elev, tipo, r; GetTokenInformation(t, 20, out elev, 4, out r); GetTokenInformation(t, 18, out tipo, 4, out r);
    CloseHandle(t); CloseHandle(h);
    return "TokenElevation=" + elev + " (0 = no elevado) · TokenElevationType=" + tipo + " (1 default, 2 full/elevado, 3 limited)";
  }
}
"@
"pid $ProcId : " + [Elev]::Query($ProcId)
