#requires -Version 7.0
<#
.SYNOPSIS
    Arnés de Algoritmia.exe para el cierre OE4: maneja el ejecutable portable como CAJA NEGRA, igual que
    un jugador (lanzar, mirar, clic, arrastrar, escribir) y mide cargas, memoria, red y residuos.
    No es código del juego.

.DESCRIPTION
    Uso:   pwsh -NoProfile -File oe4.ps1 <Subcomando> [argumentos] [-Opciones]
    Guía completa: oe4-USO.md (junto a este archivo). `pwsh -NoProfile -File oe4.ps1 help` la resume.

    Un solo script y SIN estado en memoria: cada llamada es un proceso nuevo. El estado vive en
    estado.json dentro de $env:OE4_TRABAJO (por defecto %TEMP%\Algoritmia-OE4); también ahí quedan
    fotos\, rafagas\, muestras\ (mem/red/registro por sesión), logs\, residuos\, cargas.csv y acciones.tsv.
    Variables opcionales: OE4_TRABAJO · OE4_EXE · OE4_SHA256 · OE4_LOGDIR (carpeta del Player.log) ·
    OE4_REGKEY (clave de HKCU del reproductor) · OE4_STRICT=1 (Set-StrictMode -Version 1, para depurar el arnés).

    COORDENADAS: fracciones 0–1 del ÁREA CLIENTE (origen arriba a la izquierda; admite «0.5» y «0,5»). La
    misma idea que IllustrationFraming: no dependen de la resolución. El proceso se declara DPI-aware
    por monitor (SetProcessDpiAwarenessContext -4) y el ratón se mueve con SendInput absoluto sobre el
    escritorio virtual, con comprobación de que el cursor cayó donde se pidió.

    FOCO: el Input System del juego ignora la entrada sin foco. Antes de cada acción de entrada (y de
    cada Foto) se asegura el primer plano —AllowSetForegroundWindow, un toque de Alt por SendInput,
    SetForegroundWindow, comprobación con GetForegroundWindow, reintentos y AttachThreadInput—; si no
    se consigue, falla con salida 3 y NO envía nada. Tampoco pulsa si otra ventana cubre el punto o si el
    cursor se movió (alguien tocó el ratón) entre mover y pulsar.

    SUBCOMANDOS
      Start-Juego [sesión] [-Exe r] [-Ancho 1920 -Alto 1080 | -Completa] [-SinMarco] [-Sha256 hash|archivo]
          Comprueba el SHA-256 del exe (si se pasa referencia y no coincide: salida 3, no lanza), aparta los
          Player.log anteriores, toma la FOTO PREVIA de residuos (LocalLow\<empresa>\<producto>, primer nivel
          de %TEMP% y la clave HKCU\Software\<empresa>\<producto>) si la sesión no la tiene, lanza con
          -screen-fullscreen 0 -screen-width W -screen-height H -monitor 1 (o pantalla completa), espera la
          ventana y arranca el muestreador oculto.
      Foto <nombre> [-Gris] [-Espera ms]       PNG del área cliente en fotos\ (con -Gris, además <nombre>_gris.png)
      Clic <x> <y> [-Veces n] [-Intervalo ms]  Intervalo = ms entre el inicio de un clic y el del siguiente
      Sostener <x> <y> <ms>                    clic sostenido sin moverse (flechas del N3)
      Arrastrar <x1> <y1> <x2> <y2> [ms=700]   pulsa, ~20 movimientos, suelta (supera el umbral de arrastre de uGUI)
      Escribir <texto>                         SendInput Unicode (solo el nombre del perfil usa texto)
      Teclas <t1 t2 …> [-Pausa ms]             Esc Enter Espacio Arriba Abajo Izquierda Derecha W A S D F1…F12
                                               Rueda, combinaciones (Ctrl+A, Alt+Enter) y repetición (Retroceso*24)
      Clic, Sostener y Arrastrar aceptan -ConFoto <nombre> [-FotoMs 500]: foto N ms después de soltar,
      EN LA MISMA LLAMADA (dos llamadas separadas pierden ~2 s en arrancar PowerShell).
      Esperar-Carga <escena|*> [segundos=30]   (alias Medir-Carga) lee Player.log desde el offset guardado,
                                               busca «RNF-04: «<escena>» cargó en N s» y devuelve N
      Rafaga <prefijo> <segundos> <fps>        cuadros en rafagas\ + <prefijo>.csv (luminancia por cuadro)
      Muestrear                                interno: lo lanza Start-Juego (mem.csv, red.csv, registro.tsv)
      Preparar-Datos <perfil|ruta.json …> [-SinCarpeta]   con el juego CERRADO: vacía <exe>\Datos y copia semillas
      Matar [-Todos]  ·  Cerrar-Juego          RNF-14 / cierre normal (WM_CLOSE; Matar si no responde en 10 s)
      Revisar-Log [sesión]                     copia Player.log y Player-prev.log como .txt y lista Exception/Error/RNF-04
      Residuos [sesión] [-Previo] [-Buscar t1,t2]   diferencia contra la foto previa; con -Buscar, rastrea los términos
      Restaurar-Registro                       deja la clave del reproductor como estaba en la foto previa (la borra si no existía)
      Tamano [-Carpeta r]                      tamaño de la carpeta sin *_DoNotShip ni Datos (RNF-06)
      Medidas [sesión|-]                       resume mem/red/cargas de lo muestreado (RNF-04, RNF-05, RNF-10)
      Estado                                   qué hay lanzado, ventana, monitores, DPI
      help

    SALIDAS (como editor.ps1): 0 bien · 1 hallazgo (Exception en el log, residuos hallados, carga ≥ 10 s,
    paquete ≥ 500 MB) · 2 fallo de infraestructura (no hay juego, argumento inválido, timeout) ·
    3 guarda (sin foco, otra ventana encima, SHA-256 distinto, juego ya en marcha, otra invocación en curso;
    no se envió nada).
#>
param(
    [Parameter(Position = 0)][string]$Sub = 'help',
    [Parameter(Position = 1, ValueFromRemainingArguments = $true)][string[]]$Rest = @(),
    [string]$Exe,                 # ruta de Algoritmia.exe (por defecto $env:OE4_EXE o Build\Algoritmia)
    [string]$Sesion,              # nombre de la sesión (S-N1, S-INI…); por defecto la del estado
    [int]$Ancho = 1920,           # Start-Juego: cliente en ventana
    [int]$Alto = 1080,
    [switch]$Completa,            # Start-Juego: pantalla completa en el monitor principal
    [switch]$SinMarco,            # Start-Juego: ventana sin marco (-popupwindow), útil si el monitor no da para marco
    [string]$Sha256,              # Start-Juego: hash (64 hex) o archivo con la referencia
    [switch]$Gris,                # Foto: además la versión en escala de grises (RNF-19)
    [int]$Veces = 1,              # Clic
    [int]$Intervalo = 120,        # Clic: ms entre inicios de clic
    [string]$ConFoto,             # Clic/Sostener/Arrastrar: nombre de la foto posterior
    [int]$FotoMs = 500,           # …y cuántos ms después de soltar
    [int]$Espera = 0,             # Foto: ms antes de capturar
    [int]$Pausa = 120,            # Teclas: ms entre teclas
    [string[]]$Buscar = @(),      # Residuos: términos a rastrear (coma como separador)
    [switch]$Previo,              # Residuos: tomar la foto previa a mano
    [string]$Carpeta,             # Tamano: carpeta a medir
    [switch]$SinCarpeta,          # Preparar-Datos: borrar también la carpeta Datos (PF-RNF07-01)
    [switch]$Todos                # Matar: todos los procesos con el nombre del exe
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)
# Fechas, decimales de los CSV y -f: siempre invariantes (Windows en es-CO usaría coma decimal).
[Threading.Thread]::CurrentThread.CurrentCulture = [Globalization.CultureInfo]::InvariantCulture
[Threading.Thread]::CurrentThread.CurrentUICulture = [Globalization.CultureInfo]::InvariantCulture
if ($env:OE4_STRICT) { Set-StrictMode -Version 1 }   # solo variables sin inicializar: las propiedades que faltan en el estado (hashtable) son normales

# --- Rutas y constantes --------------------------------------------------------------------------
$Aqui = $PSScriptRoot
$Trabajo = if ($env:OE4_TRABAJO) { $env:OE4_TRABAJO } else { Join-Path $env:TEMP 'Algoritmia-OE4' }
$EstadoFile = Join-Path $Trabajo 'estado.json'
$ExeDefecto = 'C:\Dev\Algoritmia\Build\Algoritmia\Algoritmia.exe'
$Empresa = 'Universidad Catolica de Colombia'
$Producto = 'Algoritmia'
$LogDir = if ($env:OE4_LOGDIR) { $env:OE4_LOGDIR } else { Join-Path $env:USERPROFILE "AppData\LocalLow\$Empresa\$Producto" }
$RegSub = if ($env:OE4_REGKEY) { $env:OE4_REGKEY } else { "Software\$Empresa\$Producto" }
$PresupuestoCargaS = 10.0      # RNF-04
$PresupuestoPaqueteMB = 500.0  # RNF-06
$Inv = [Globalization.CultureInfo]::InvariantCulture
$Utf8 = [Text.UTF8Encoding]::new($false)
# La línea que añade SceneLoader (RNF-04). Los decimales pueden salir con punto o con coma según el equipo.
$LineaCarga = [regex]'RNF-04:\s*«(?<e>[^»]+)»\s*cargó en\s*(?<n>[0-9]+(?:[.,][0-9]+)?)\s*s'

function Stop-With([int]$Code, [string]$Message) {
    if ($Message) { [Console]::Error.WriteLine($Message) }
    exit $Code
}
function Say([string]$Text) { Write-Host $Text }
# UTC con espacio y «Z»: ConvertFrom-Json convierte en [datetime] cualquier cadena ISO con «T», y eso rompería las comparaciones.
function Iso([datetime]$D) { $D.ToUniversalTime().ToString('yyyy-MM-dd HH:mm:ss.fff', $Inv) + 'Z' }
function ParseIso([string]$S) { [datetime]::ParseExact($S, 'yyyy-MM-dd HH:mm:ss.fffZ', $Inv, [Globalization.DateTimeStyles]::AssumeUniversal -bor [Globalization.DateTimeStyles]::AdjustToUniversal) }
function Safe-Name([string]$S) { $S -replace '[\\/:*?"<>|\s]', '_' }
function Quote([string]$S) { '"' + $S + '"' }
function New-Dir([string]$P) { $null = New-Item -ItemType Directory -Force -Path $P; $P }
function Write-Text([string]$Path, [string]$Text) { [IO.File]::WriteAllText($Path, $Text, $Utf8) }
function Add-Text([string]$Path, [string]$Text) { [IO.File]::AppendAllText($Path, $Text, $Utf8) }

# --- Código nativo (P/Invoke + captura), compilado UNA vez por llamada -----------------------------
$Fuente = @'
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public static class Oe4
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    [StructLayout(LayoutKind.Sequential)] struct RECT { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] struct POINT { public int X, Y; }
    [StructLayout(LayoutKind.Sequential)] struct MOUSEINPUT { public int dx, dy; public uint mouseData, dwFlags, time; public IntPtr dwExtraInfo; }
    [StructLayout(LayoutKind.Sequential)] struct KEYBDINPUT { public ushort wVk, wScan; public uint dwFlags, time; public IntPtr dwExtraInfo; }
    [StructLayout(LayoutKind.Explicit)] struct INPUTUNION { [FieldOffset(0)] public MOUSEINPUT mi; [FieldOffset(0)] public KEYBDINPUT ki; }
    [StructLayout(LayoutKind.Sequential)] struct INPUT { public uint type; public INPUTUNION u; }
    [StructLayout(LayoutKind.Sequential)] struct MONITORINFO { public int cbSize; public RECT rcMonitor, rcWork; public uint dwFlags; }

    delegate bool EnumWindowsProc(IntPtr h, IntPtr l);
    delegate bool EnumMonitorsProc(IntPtr m, IntPtr dc, ref RECT r, IntPtr l);

    [DllImport("user32.dll", SetLastError = true)] static extern bool SetProcessDpiAwarenessContext(IntPtr v);
    [DllImport("user32.dll")] static extern IntPtr GetThreadDpiAwarenessContext();
    [DllImport("user32.dll")] static extern int GetAwarenessFromDpiAwarenessContext(IntPtr v);
    [DllImport("user32.dll")] static extern uint GetDpiForWindow(IntPtr h);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumWindowsProc cb, IntPtr l);
    [DllImport("user32.dll")] static extern bool EnumDisplayMonitors(IntPtr dc, IntPtr clip, EnumMonitorsProc cb, IntPtr l);
    [DllImport("user32.dll")] static extern bool GetMonitorInfo(IntPtr m, ref MONITORINFO mi);
    [DllImport("user32.dll")] static extern IntPtr MonitorFromWindow(IntPtr h, uint flags);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder sb, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder sb, int n);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] static extern bool IsIconic(IntPtr h);
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr h, int cmd);
    [DllImport("user32.dll")] static extern bool GetClientRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] static extern bool ClientToScreen(IntPtr h, ref POINT p);
    [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] static extern bool BringWindowToTop(IntPtr h);
    [DllImport("user32.dll")] static extern bool AllowSetForegroundWindow(uint pid);
    [DllImport("user32.dll")] static extern bool AttachThreadInput(uint a, uint b, bool attach);
    [DllImport("user32.dll")] static extern IntPtr WindowFromPoint(POINT p);
    [DllImport("user32.dll")] static extern IntPtr GetAncestor(IntPtr h, uint flags);
    [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT p);
    [DllImport("user32.dll")] static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll", SetLastError = true)] static extern uint SendInput(uint n, INPUT[] inputs, int size);
    [DllImport("user32.dll")] static extern uint MapVirtualKey(uint code, uint type);
    [DllImport("user32.dll")] static extern int GetSystemMetrics(int i);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint msg, IntPtr w, IntPtr l);
    [DllImport("kernel32.dll")] static extern uint SetThreadExecutionState(uint f);
    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
    [DllImport("winmm.dll")] static extern uint timeBeginPeriod(uint ms);
    [DllImport("winmm.dll")] static extern uint timeEndPeriod(uint ms);

    const uint M_MOVE = 0x0001, M_LEFTDOWN = 0x0002, M_LEFTUP = 0x0004, M_WHEEL = 0x0800, M_VIRTUALDESK = 0x4000, M_ABSOLUTE = 0x8000;
    const uint K_EXT = 0x0001, K_KEYUP = 0x0002, K_UNICODE = 0x0004;

    static readonly double[] Lin = MakeLin();
    static double[] MakeLin()
    {
        var t = new double[256];
        for (int i = 0; i < 256; i++) { double c = i / 255.0; t[i] = c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4); }
        return t;
    }

    // ---------- DPI y pantalla ----------
    public static bool SetDpiAware() { try { return SetProcessDpiAwarenessContext(new IntPtr(-4)); } catch { return false; } }
    public static string DpiMode()
    {
        try { int a = GetAwarenessFromDpiAwarenessContext(GetThreadDpiAwarenessContext()); return a == 2 ? "por-monitor" : a == 1 ? "sistema" : a == 0 ? "ninguna" : "?"; }
        catch { return "?"; }
    }
    public static int[] Virtual() { return new[] { GetSystemMetrics(76), GetSystemMetrics(77), GetSystemMetrics(78), GetSystemMetrics(79) }; }
    public static int[] Primary() { return new[] { GetSystemMetrics(0), GetSystemMetrics(1) }; }
    public static string Monitors()
    {
        var sb = new StringBuilder();
        EnumMonitorsProc cb = delegate (IntPtr m, IntPtr dc, ref RECT r, IntPtr l)
        {
            var mi = new MONITORINFO(); mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO)); GetMonitorInfo(m, ref mi);
            sb.AppendFormat(Inv, "{0}x{1} en ({2},{3}){4}; ", r.Right - r.Left, r.Bottom - r.Top, r.Left, r.Top, (mi.dwFlags & 1) != 0 ? " [principal]" : "");
            return true;
        };
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, cb, IntPtr.Zero);
        return sb.ToString().TrimEnd(' ', ';');
    }
    public static void KeepAwake(bool on) { SetThreadExecutionState(on ? 0x80000003u : 0x80000000u); }

    // ---------- ventanas ----------
    static string Cls(IntPtr h) { var sb = new StringBuilder(256); GetClassName(h, sb, 256); return sb.ToString(); }
    static string Title(IntPtr h) { var sb = new StringBuilder(256); GetWindowText(h, sb, 256); return sb.ToString(); }
    public static string Describe(IntPtr h)
    {
        if (h == IntPtr.Zero) return "(ninguna)";
        uint pid; GetWindowThreadProcessId(h, out pid);
        string name = "?"; try { name = Process.GetProcessById((int)pid).ProcessName; } catch { }
        return Cls(h) + " «" + Title(h) + "» (" + name + " pid " + pid + ")";
    }
    public static IntPtr Foreground() { return GetForegroundWindow(); }
    public static IntPtr FindWindow(int pid)
    {
        IntPtr best = IntPtr.Zero; long bestScore = -1;
        EnumWindowsProc cb = delegate (IntPtr h, IntPtr l)
        {
            uint p; GetWindowThreadProcessId(h, out p);
            if (p != (uint)pid || !IsWindowVisible(h)) return true;
            RECT r; GetClientRect(h, out r);
            long score = (Cls(h) == "UnityWndClass" ? (1L << 40) : 0L) + (long)r.Right * r.Bottom;
            if (score > bestScore) { bestScore = score; best = h; }
            return true;
        };
        EnumWindows(cb, IntPtr.Zero);
        return best;
    }
    // {x, y, ancho, alto} del área cliente en coordenadas de pantalla (píxeles físicos).
    public static int[] Client(IntPtr h)
    {
        RECT r; GetClientRect(h, out r);
        POINT p = new POINT(); ClientToScreen(h, ref p);
        return new[] { p.X, p.Y, r.Right, r.Bottom };
    }
    public static string WhereIs(IntPtr h)
    {
        IntPtr mon = MonitorFromWindow(h, 2);
        var mi = new MONITORINFO(); mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO)); GetMonitorInfo(mon, ref mi);
        uint dpi = 0; try { dpi = GetDpiForWindow(h); } catch { }
        return string.Format(Inv, "monitor {0}x{1} en ({2},{3}){4}, DPI de la ventana {5} ({6} %)",
            mi.rcMonitor.Right - mi.rcMonitor.Left, mi.rcMonitor.Bottom - mi.rcMonitor.Top, mi.rcMonitor.Left, mi.rcMonitor.Top,
            (mi.dwFlags & 1) != 0 ? " [principal]" : " [NO es el principal]", dpi, dpi * 100 / 96);
    }
    // null si el punto es del juego; si lo cubre otra ventana, su descripción.
    public static string Covered(IntPtr hwnd, int x, int y)
    {
        POINT p = new POINT(); p.X = x; p.Y = y;
        IntPtr w = WindowFromPoint(p);
        IntPtr root = w == IntPtr.Zero ? IntPtr.Zero : GetAncestor(w, 2);
        return (w == hwnd || root == hwnd) ? null : Describe(root == IntPtr.Zero ? w : root);
    }
    public static void Close(IntPtr h) { PostMessage(h, 0x0010, IntPtr.Zero, IntPtr.Zero); }

    // Primer plano: AllowSetForegroundWindow + toque de Alt + SetForegroundWindow, comprobando y reintentando.
    public static bool EnsureForeground(IntPtr h, int tries)
    {
        if (IsIconic(h)) { ShowWindow(h, 9); Thread.Sleep(250); }
        for (int i = 0; i < tries; i++)
        {
            if (GetForegroundWindow() == h) return true;
            AllowSetForegroundWindow(0xFFFFFFFF);
            Key(0x12, false); Key(0x12, true);   // Alt: concede el derecho a cambiar el primer plano
            SetForegroundWindow(h); BringWindowToTop(h);
            Thread.Sleep(150);
            if (GetForegroundWindow() == h) return true;
            IntPtr fg = GetForegroundWindow(); uint dummy;
            uint fgThread = fg == IntPtr.Zero ? 0u : GetWindowThreadProcessId(fg, out dummy);
            uint me = GetCurrentThreadId();
            if (fgThread != 0 && fgThread != me && AttachThreadInput(me, fgThread, true))
            {
                try { SetForegroundWindow(h); BringWindowToTop(h); } finally { AttachThreadInput(me, fgThread, false); }
            }
            Thread.Sleep(150);
        }
        return GetForegroundWindow() == h;
    }

    // ---------- entrada ----------
    static void Send(INPUT i)
    {
        var a = new[] { i };
        if (SendInput(1, a, Marshal.SizeOf(typeof(INPUT))) != 1)
            throw new InvalidOperationException("SendInput falló (código " + Marshal.GetLastWin32Error() + "): ¿escritorio bloqueado o ventana elevada?");
    }
    static INPUT Mouse(uint flags, int dx, int dy, uint data)
    {
        var i = new INPUT(); i.type = 0;
        i.u.mi = new MOUSEINPUT { dx = dx, dy = dy, mouseData = data, dwFlags = flags };
        return i;
    }
    static bool IsExt(ushort vk) { return (vk >= 0x21 && vk <= 0x28) || vk == 0x2D || vk == 0x2E; }
    public static void Key(ushort vk, bool up)
    {
        var i = new INPUT(); i.type = 1;
        i.u.ki = new KEYBDINPUT { wVk = vk, wScan = (ushort)MapVirtualKey(vk, 0), dwFlags = (IsExt(vk) ? K_EXT : 0u) | (up ? K_KEYUP : 0u) };
        Send(i);
    }
    static void Unicode(char c)
    {
        var d = new INPUT(); d.type = 1;
        d.u.ki = new KEYBDINPUT { wVk = 0, wScan = c, dwFlags = K_UNICODE };
        var u = d; u.u.ki.dwFlags = K_UNICODE | K_KEYUP;
        Send(d); Thread.Sleep(15); Send(u);
    }

    // SendInput absoluto sobre el escritorio virtual. Fórmula de AutoHotkey (65536·px/ancho + 1) y, como
    // el redondeo exacto de Windows no está documentado, se COMPRUEBA el cursor y se corrige con SetCursorPos.
    public static string MoveTo(int px, int py)
    {
        int vx = GetSystemMetrics(76), vy = GetSystemMetrics(77), vw = GetSystemMetrics(78), vh = GetSystemMetrics(79);
        if (px < vx || py < vy || px >= vx + vw || py >= vy + vh)
            return "FUERA: el punto (" + px + "," + py + ") cae fuera del escritorio virtual (" + vx + "," + vy + " " + vw + "x" + vh + ")";
        long ax = Math.Min(65535L, (65536L * (px - vx)) / vw + 1);
        long ay = Math.Min(65535L, (65536L * (py - vy)) / vh + 1);
        Send(Mouse(M_MOVE | M_ABSOLUTE | M_VIRTUALDESK, (int)ax, (int)ay, 0));
        Thread.Sleep(8);
        POINT c; GetCursorPos(out c);
        if (Math.Abs(c.X - px) > 1 || Math.Abs(c.Y - py) > 1)
        {
            SetCursorPos(px, py);
            Thread.Sleep(8);
            GetCursorPos(out c);
            if (Math.Abs(c.X - px) > 1 || Math.Abs(c.Y - py) > 1)
                return "CURSOR: se pidió (" + px + "," + py + ") y el cursor quedó en (" + c.X + "," + c.Y + ")";
        }
        return null;
    }
    static string Guard(IntPtr hwnd, int x, int y)
    {
        if (GetForegroundWindow() != hwnd) return "FOCO: la ventana del juego ya no está en primer plano (ahora: " + Describe(GetForegroundWindow()) + ")";
        POINT c; GetCursorPos(out c);
        if (Math.Abs(c.X - x) > 2 || Math.Abs(c.Y - y) > 2)
            return "CURSOR: el cursor se movió a (" + c.X + "," + c.Y + ") en vez de (" + x + "," + y + "): ¿alguien tocó el ratón?";
        return null;
    }
    // Un píxel al lado y de vuelta: SIEMPRE hay un movimiento real antes de pulsar. Tras cargar una escena, su
    // EventSystem no sabe dónde está el puntero hasta que el ratón se mueve, y un clic sin movimiento previo se
    // pierde (spike 01/10/2026: 11 clics seguidos en el mismo punto, ninguno llegó al botón de la narrativa nueva).
    static string Nudge(int x, int y)
    {
        var e = MoveTo(x > GetSystemMetrics(76) ? x - 1 : x + 1, y); if (e != null) return e;
        Thread.Sleep(20);
        return MoveTo(x, y);
    }
    static string Settle(IntPtr hwnd, int x, int y)
    {
        var e = Nudge(x, y); if (e != null) return e;
        Thread.Sleep(90);                       // el juego registra el «encima» antes de la pulsación
        return Guard(hwnd, x, y);
    }

    public static string Click(IntPtr hwnd, int x, int y, int n, int periodMs, int holdMs)
    {
        timeBeginPeriod(1); bool down = false;
        try
        {
            var e = Settle(hwnd, x, y); if (e != null) return e + " (no se pulsó)";
            for (int i = 0; i < n; i++)
            {
                var sw = Stopwatch.StartNew();
                if (i > 0) { e = Nudge(x, y); if (e != null) return e + " (clic " + (i + 1) + " de " + n + "; no se pulsó)"; Thread.Sleep(30); }   // un clic anterior pudo cambiar de escena
                e = Guard(hwnd, x, y); if (e != null) return e + " (clic " + (i + 1) + " de " + n + "; no se pulsó)";
                Send(Mouse(M_LEFTDOWN, 0, 0, 0)); down = true;
                Thread.Sleep(holdMs);
                Send(Mouse(M_LEFTUP, 0, 0, 0)); down = false;
                int rest = periodMs - (int)sw.ElapsedMilliseconds;
                if (i < n - 1 && rest > 0) Thread.Sleep(rest);
            }
            Thread.Sleep(30);
            return "ok";
        }
        finally { if (down) Send(Mouse(M_LEFTUP, 0, 0, 0)); timeEndPeriod(1); }
    }

    public static string Hold(IntPtr hwnd, int x, int y, int ms)
    {
        timeBeginPeriod(1); bool down = false; string lost = null;
        try
        {
            var e = Settle(hwnd, x, y); if (e != null) return e + " (no se pulsó)";
            Send(Mouse(M_LEFTDOWN, 0, 0, 0)); down = true;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < ms)
            {
                Thread.Sleep((int)Math.Min(40, Math.Max(1, ms - sw.ElapsedMilliseconds)));
                if (lost == null && GetForegroundWindow() != hwnd) lost = "FOCO: se perdió el primer plano durante el clic sostenido (se soltó el botón)";
            }
            Send(Mouse(M_LEFTUP, 0, 0, 0)); down = false;
            Thread.Sleep(30);
            return lost ?? "ok";
        }
        finally { if (down) Send(Mouse(M_LEFTUP, 0, 0, 0)); timeEndPeriod(1); }
    }

    public static string Drag(IntPtr hwnd, int x1, int y1, int x2, int y2, int totalMs, int steps)
    {
        timeBeginPeriod(1); bool down = false;
        try
        {
            var e = Settle(hwnd, x1, y1); if (e != null) return e + " (arrastre; no se pulsó)";
            Send(Mouse(M_LEFTDOWN, 0, 0, 0)); down = true;
            Thread.Sleep(100);
            for (int s = 1; s <= steps; s++)
            {
                int px = x1 + (int)Math.Round((x2 - x1) * (double)s / steps);
                int py = y1 + (int)Math.Round((y2 - y1) * (double)s / steps);
                e = MoveTo(px, py); if (e != null) return e + " (arrastre; se soltó el botón)";
                if (GetForegroundWindow() != hwnd) return "FOCO: se perdió el primer plano durante el arrastre (se soltó el botón)";
                Thread.Sleep(Math.Max(1, totalMs / steps));
            }
            Thread.Sleep(150);                  // el destino registra el «encima» antes de soltar
            e = Guard(hwnd, x2, y2); if (e != null) return e + " (arrastre; se soltó el botón)";
            Send(Mouse(M_LEFTUP, 0, 0, 0)); down = false;
            Thread.Sleep(40);
            return "ok";
        }
        finally { if (down) Send(Mouse(M_LEFTUP, 0, 0, 0)); timeEndPeriod(1); }
    }

    public static string TypeText(IntPtr hwnd, string text, int delayMs)
    {
        timeBeginPeriod(1);
        try
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (GetForegroundWindow() != hwnd) return "FOCO: se perdió el primer plano tras " + i + " de " + text.Length + " caracteres";
                Unicode(text[i]); Thread.Sleep(delayMs);
            }
            return "ok";
        }
        finally { timeEndPeriod(1); }
    }

    public static string Press(IntPtr hwnd, ushort[] mods, ushort vk, int holdMs)
    {
        if (GetForegroundWindow() != hwnd) return "FOCO: la ventana del juego no está en primer plano (ahora: " + Describe(GetForegroundWindow()) + ")";
        int pressed = 0; bool keyDown = false;
        try
        {
            foreach (var m in mods) { Key(m, false); pressed++; }
            Key(vk, false); keyDown = true;
            Thread.Sleep(holdMs);
            return "ok";
        }
        finally
        {
            if (keyDown) Key(vk, true);
            for (int i = pressed - 1; i >= 0; i--) Key(mods[i], true);
        }
    }

    public static string Wheel(IntPtr hwnd, int x, int y, int notches)
    {
        var e = Settle(hwnd, x, y); if (e != null) return e;
        Send(Mouse(M_WHEEL, 0, 0, unchecked((uint)(notches * 120))));
        return "ok";
    }

    // ---------- imagen ----------
    public static Bitmap Capture(int x, int y, int w, int h)
    {
        var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
            // Solo SourceCopy: System.Drawing de .NET valida el enum y rechaza «SourceCopy | CaptureBlt» (spike 01/10/2026).
            g.CopyFromScreen(x, y, 0, 0, new Size(w, h), CopyPixelOperation.SourceCopy);
        return bmp;
    }
    // {luminancia relativa WCAG 0–1, luma Rec.709 sobre valores sRGB 0–255}, muestreando cada `step` píxeles.
    public static double[] Luma(Bitmap b, int step)
    {
        var bd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            int stride = bd.Stride; var buf = new byte[stride * b.Height];
            Marshal.Copy(bd.Scan0, buf, 0, buf.Length);
            double sy = 0, s8 = 0; long n = 0;
            for (int y = 0; y < b.Height; y += step)
                for (int x = 0; x < b.Width; x += step)
                {
                    int i = y * stride + x * 4; byte B = buf[i], G = buf[i + 1], R = buf[i + 2];
                    sy += 0.2126 * Lin[R] + 0.7152 * Lin[G] + 0.0722 * Lin[B];
                    s8 += 0.2126 * R + 0.7152 * G + 0.0722 * B;
                    n++;
                }
            return new[] { sy / n, s8 / n };
        }
        finally { b.UnlockBits(bd); }
    }
    public static Bitmap Gray(Bitmap src)
    {
        int w = src.Width, h = src.Height;
        var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        var rc = new Rectangle(0, 0, w, h);
        var bs = src.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var bd = dst.LockBits(rc, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            int ss = bs.Stride, ds = bd.Stride;
            var a = new byte[ss * h]; var o = new byte[ds * h];
            Marshal.Copy(bs.Scan0, a, 0, a.Length);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int i = y * ss + x * 4, j = y * ds + x * 4;
                    byte g8 = (byte)(0.2126 * a[i + 2] + 0.7152 * a[i + 1] + 0.0722 * a[i] + 0.5);
                    o[j] = g8; o[j + 1] = g8; o[j + 2] = g8; o[j + 3] = 255;
                }
            Marshal.Copy(o, 0, bd.Scan0, o.Length);
        }
        finally { src.UnlockBits(bs); dst.UnlockBits(bd); }
        return dst;
    }

    // Ráfaga: captura en memoria al ritmo pedido y SOLO después calcula luminancias y guarda los PNG (así el
    // ritmo no depende del coste de codificar). Devuelve el resumen.
    public static string Burst(IntPtr hwnd, int x, int y, int w, int h, double seconds, double fps, string prefix)
    {
        int frames = Math.Max(1, (int)Math.Round(seconds * fps));
        long bytes = (long)frames * w * h * 4;
        if (bytes > 3L * 1024 * 1024 * 1024)
            throw new InvalidOperationException("La ráfaga ocuparía " + (bytes >> 20) + " MB en memoria (límite 3072): baje los segundos o los fps.");
        var bmps = new List<Bitmap>(frames); var ts = new List<double>(frames);
        int sinFoco = 0;
        timeBeginPeriod(1);
        var sw = Stopwatch.StartNew();
        try
        {
            for (int i = 0; i < frames; i++)
            {
                double due = i * 1000.0 / fps;
                while (true) { double rest = due - sw.Elapsed.TotalMilliseconds; if (rest <= 0) break; Thread.Sleep(rest > 2 ? 1 : 0); }
                ts.Add(sw.Elapsed.TotalMilliseconds);
                if (GetForegroundWindow() != hwnd) sinFoco++;
                bmps.Add(Capture(x, y, w, h));
            }
        }
        finally { timeEndPeriod(1); }
        double total = sw.Elapsed.TotalSeconds;
        var csv = new StringBuilder("frame,t_ms,y_rel,luma8\n");
        double minY = 2, maxY = -1;
        for (int i = 0; i < bmps.Count; i++)
        {
            var l = Luma(bmps[i], 2);
            minY = Math.Min(minY, l[0]); maxY = Math.Max(maxY, l[0]);
            csv.Append(i + 1).Append(',').Append(ts[i].ToString("0.0", Inv)).Append(',').Append(l[0].ToString("0.0000", Inv)).Append(',').Append(l[1].ToString("0.00", Inv)).Append('\n');
            bmps[i].Save(prefix + "_" + (i + 1).ToString("0000", Inv) + ".png", ImageFormat.Png);
            bmps[i].Dispose();
        }
        File.WriteAllText(prefix + ".csv", csv.ToString(), new UTF8Encoding(false));
        return string.Format(Inv, "{0} cuadros de {1}x{2} en {3:0.00} s ({4:0.0} fps reales de {5:0.#} pedidos) · y_rel {6:0.0000}–{7:0.0000} · cuadros sin foco: {8}",
            bmps.Count, w, h, total, bmps.Count / Math.Max(0.001, total), fps, minY, maxY, sinFoco);
    }

    // ---------- búsqueda de términos en nombres y contenido (UTF-8 y UTF-16) ----------
    public static List<string> Grep(string root, string[] terms)
    {
        var hits = new List<string>();
        var labels = new List<string>(); var needles = new List<byte[]>();
        foreach (var t in terms)
        {
            if (string.IsNullOrEmpty(t)) continue;
            labels.Add("«" + t + "» (UTF-8)"); needles.Add(Encoding.UTF8.GetBytes(t));
            labels.Add("«" + t + "» (UTF-16)"); needles.Add(Encoding.Unicode.GetBytes(t));
        }
        if (needles.Count == 0 || !(Directory.Exists(root) || File.Exists(root))) return hits;
        int maxLen = 0; foreach (var n in needles) maxLen = Math.Max(maxLen, n.Length);
        var opt = new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.ReparsePoint };
        const int Chunk = 1 << 20;
        var buf = new byte[Chunk + maxLen];
        var paths = new List<string>();
        if (File.Exists(root)) paths.Add(root); else foreach (var p in Directory.EnumerateFileSystemEntries(root, "*", opt)) paths.Add(p);
        foreach (var path in paths)
        {
            string name = Path.GetFileName(path);
            foreach (var t in terms)
                if (!string.IsNullOrEmpty(t) && name.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0) hits.Add(path + "  <- el nombre contiene «" + t + "»");
            if (Directory.Exists(path)) continue;
            try
            {
                var found = new bool[needles.Count];
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 1 << 16))
                {
                    int carry = 0;
                    while (true)
                    {
                        int n = fs.Read(buf, carry, Chunk);
                        if (n <= 0) break;
                        int len = carry + n;
                        var span = new ReadOnlySpan<byte>(buf, 0, len);
                        for (int i = 0; i < needles.Count; i++) if (!found[i] && span.IndexOf(needles[i]) >= 0) found[i] = true;
                        carry = Math.Min(maxLen - 1, len);
                        Buffer.BlockCopy(buf, len - carry, buf, 0, carry);
                    }
                }
                for (int i = 0; i < needles.Count; i++) if (found[i]) hits.Add(path + "  <- el contenido incluye " + labels[i]);
            }
            catch (IOException) { hits.Add(path + "  <- NO SE PUDO LEER (en uso)"); }
            catch (UnauthorizedAccessException) { hits.Add(path + "  <- NO SE PUDO LEER (acceso denegado)"); }
        }
        return hits;
    }
}
'@

function Initialize-Native {
    if (-not ('Oe4' -as [type])) {
        $refs = @('System.Drawing.Common', 'System.Drawing.Primitives', 'System.Diagnostics.Process', 'System.ComponentModel.Primitives',
            'System.Collections', 'System.Threading.Thread', 'System.Memory', 'System.Runtime.InteropServices', 'System.Text.Encoding.Extensions')
        # Desde .NET 10, System.Drawing.Common reenvía a estos dos; en runtimes anteriores no existen y se omiten.
        foreach ($n in 'System.Private.Windows.GdiPlus', 'System.Private.Windows.Core') {
            try { [void][Reflection.Assembly]::Load($n); $refs += $n } catch { }
        }
        Add-Type -TypeDefinition $Fuente -Language CSharp -ErrorAction Stop -ReferencedAssemblies $refs
    }
    [void][Oe4]::SetDpiAware()
}

# --- Estado entre llamadas -----------------------------------------------------------------------
function Get-Estado {
    if (Test-Path -LiteralPath $EstadoFile) {
        for ($i = 0; $i -lt 5; $i++) {          # puede estar reescribiéndose
            try { return (Get-Content -LiteralPath $EstadoFile -Raw -Encoding utf8 | ConvertFrom-Json -AsHashtable -Depth 30) } catch { Start-Sleep -Milliseconds 150 }
        }
    }
    @{}
}
function Save-Estado($E) {
    $null = New-Dir $Trabajo
    $tmp = "$EstadoFile.$PID.tmp"
    Write-Text $tmp ($E | ConvertTo-Json -Depth 30)
    Move-Item -LiteralPath $tmp -Destination $EstadoFile -Force
}
function Set-Estado([hashtable]$Cambios) {
    $e = Get-Estado
    foreach ($k in $Cambios.Keys) { $e[$k] = $Cambios[$k] }
    Save-Estado $e
}
function Resolve-Exe {
    $x = if ($Exe) { $Exe } elseif ($env:OE4_EXE) { $env:OE4_EXE } else { $ExeDefecto }
    [IO.Path]::GetFullPath($x)
}
function Get-SesionArg([switch]$Obligatoria) {
    $s = if ($Sesion) { $Sesion } elseif ($Rest.Count -gt 0) { $Rest[0] } else { [string](Get-Estado).sesion }
    if (-not $s -and $Obligatoria) { Stop-With 2 'Falta el nombre de la sesión (argumento, -Sesion o una sesión ya iniciada con Start-Juego).' }
    if ($s) { Safe-Name $s } else { 'sesion' }
}

# El proceso del juego según el estado; $null si no hay (o si el pid lo reutilizó otro proceso).
function Get-Juego([switch]$Opcional) {
    $e = Get-Estado
    $p = $null
    if ($e.pid) { $p = Get-Process -Id ([int]$e.pid) -ErrorAction SilentlyContinue }
    if ($p -and $e.inicioTicks) {
        try { if ($p.StartTime.ToUniversalTime().Ticks -ne [long]$e.inicioTicks) { $p = $null } } catch { $p = $null }
    }
    if (-not $p -and -not $Opcional) { Stop-With 2 'No hay un juego lanzado por el arnés (o ya terminó). Use Start-Juego.' }
    $p
}
function Get-Ventana($P) {
    $h = [Oe4]::FindWindow($P.Id)
    if ($h -eq [IntPtr]::Zero) { $P.Refresh(); $h = $P.MainWindowHandle }
    if ($h -eq [IntPtr]::Zero) { Stop-With 2 'El juego no tiene ventana visible todavía.' }
    $h
}
function Assert-Foco([IntPtr]$H) {
    if (-not [Oe4]::EnsureForeground($H, 4)) {
        Stop-With 3 ('FOCO: no se pudo poner la ventana del juego en primer plano; ahora lo está: ' + [Oe4]::Describe([Oe4]::Foreground()) +
            '. No se envió ninguna entrada. Cierre o minimice lo que esté encima y reintente.')
    }
}
function Check-Result([string]$R) {
    if ($R -ne 'ok') { Stop-With ($(if ($R.StartsWith('FOCO') -or $R.StartsWith('CURSOR') -or $R.StartsWith('CUBIERTO')) { 3 } else { 2 })) $R }
}
function Prepare-Input {
    Initialize-Native
    $p = Get-Juego
    $h = Get-Ventana $p
    Assert-Foco $h
    $h
}
function ConvertTo-Norm([string]$S) {
    $v = 0.0
    if (-not [double]::TryParse($S.Trim().Replace(',', '.'), [Globalization.NumberStyles]::Float, $Inv, [ref]$v)) { Stop-With 2 "'$S' no es un número." }
    if ($v -lt 0 -or $v -gt 1) { Stop-With 2 "Coordenada fuera de 0–1 ($S): se usan fracciones del área cliente." }
    $v
}
# Fracción → píxel de pantalla (origen arriba a la izquierda; x = 1 cae en el último píxel).
function Get-Punto([IntPtr]$H, [double]$X, [double]$Y) {
    $c = [Oe4]::Client($H)
    if ($c[2] -lt 2 -or $c[3] -lt 2) { Stop-With 2 'La ventana no tiene área cliente (¿minimizada?).' }
    [pscustomobject]@{
        X = [int]($c[0] + [math]::Min($c[2] - 1, [math]::Floor($X * $c[2])))
        Y = [int]($c[1] + [math]::Min($c[3] - 1, [math]::Floor($Y * $c[3])))
    }
}
function Assert-Libre([IntPtr]$H, $Pt) {
    $otra = [Oe4]::Covered($H, $Pt.X, $Pt.Y)
    if ($otra) { Stop-With 3 "CUBIERTO: en el punto ($($Pt.X),$($Pt.Y)) hay otra ventana encima del juego: $otra. No se envió nada." }
}

# --- Registro del Player.log ----------------------------------------------------------------------
function Get-LogPath { Join-Path $LogDir 'Player.log' }
function Open-Shared([string]$Path) {
    [IO.File]::Open($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite -bor [IO.FileShare]::Delete)
}
# Tamaño por manejador (el del directorio NTFS se queda atrás mientras Unity escribe).
function Get-LogLength {
    $p = Get-LogPath
    if (-not (Test-Path -LiteralPath $p)) { return [long]0 }
    $fs = Open-Shared $p
    try { $fs.Length } finally { $fs.Dispose() }
}
# Lee las líneas COMPLETAS desde $Offset. Si el archivo es más corto que el offset, se rotó: vuelve a 0.
function Read-LogChunk([long]$Offset) {
    $p = Get-LogPath
    $vacio = @{ Texto = ''; Offset = $Offset }
    if (-not (Test-Path -LiteralPath $p)) { return $vacio }
    $fs = Open-Shared $p
    try {
        if ($fs.Length -lt $Offset) { $Offset = 0 }
        $n = [int]($fs.Length - $Offset)
        if ($n -le 0) { return @{ Texto = ''; Offset = $Offset } }
        $buf = [byte[]]::new($n)
        $null = $fs.Seek($Offset, [IO.SeekOrigin]::Begin)
        $leidos = 0
        while ($leidos -lt $n) { $k = $fs.Read($buf, $leidos, $n - $leidos); if ($k -le 0) { break }; $leidos += $k }
        $ultimo = [Array]::LastIndexOf($buf, [byte]10, $leidos - 1)
        if ($ultimo -lt 0) { return @{ Texto = ''; Offset = $Offset } }
        @{ Texto = [Text.Encoding]::UTF8.GetString($buf, 0, $ultimo + 1); Offset = $Offset + $ultimo + 1 }
    }
    finally { $fs.Dispose() }
}
function Mark-Offset { Set-Estado @{ logOffset = (Get-LogLength); marcaUtc = (Iso ([DateTime]::UtcNow)) } }
function Log-Accion([string]$Accion, [string]$Args2, [datetime]$Inicio) {
    $null = New-Dir $Trabajo
    Add-Text (Join-Path $Trabajo 'acciones.tsv') ("{0}`t{1}`t{2}`t{3}`n" -f (Iso $Inicio), (Iso ([DateTime]::UtcNow)), $Accion, $Args2)
}

# --- Perfiles -------------------------------------------------------------------------------------
# Valida un perfil contra el formato real (PlayerProfile.cs): tres claves, seis por fase, fases que existen
# (PhaseId.PhasesPerLevel = 1, 3, 3), sin duplicados. JsonUtility no valida nada y una fase inexistente deja
# el juego sin salida (ConfirmedPhases lanza), por eso las semillas se comprueban aquí.
function Test-Entero($V) { $V -is [int] -or $V -is [long] }     # ConvertFrom-Json devuelve Int64
function Test-Perfil([string]$Json, [string]$NombreArchivo) {
    $p = [Collections.Generic.List[string]]::new()
    try { $o = $Json | ConvertFrom-Json -AsHashtable -Depth 10 } catch { return @("JSON inválido: $($_.Exception.Message)") }
    if ($o -isnot [hashtable]) { return @('La raíz no es un objeto.') }
    $claves = @('name', 'reachedLevel', 'phases')
    foreach ($k in $claves) { if (-not $o.ContainsKey($k)) { $p.Add("Falta la clave '$k'.") } }
    foreach ($k in $o.Keys) { if ($k -notin $claves) { $p.Add("Clave de más '$k' (RNF-09: lista cerrada).") } }
    if ($p.Count -gt 0) { return $p }
    $porNivel = @{ 1 = 1; 2 = 3; 3 = 3 }
    if ($o['name'] -isnot [string] -or -not $o['name']) { $p.Add('name no es un texto.') }
    else {
        $n = [string]$o['name']
        if ($n -ne $n.Trim()) { $p.Add('name tiene espacios al borde (Create los recorta).') }
        if ($n.IndexOfAny([IO.Path]::GetInvalidFileNameChars()) -ge 0) { $p.Add('name tiene caracteres no válidos en un nombre de archivo.') }
        if ($n.Length -gt 24) { $p.Add('name pasa de 24 caracteres (el campo del juego los limita).') }
        if ($NombreArchivo -and $n -cne $NombreArchivo) { $p.Add("name '$n' no coincide con el nombre del archivo '$NombreArchivo'.") }
    }
    $alcanzado = 0
    if (-not (Test-Entero $o['reachedLevel']) -or $o['reachedLevel'] -lt 1 -or $o['reachedLevel'] -gt 3) { $p.Add('reachedLevel debe ser un entero de 1 a 3.') } else { $alcanzado = [int]$o['reachedLevel'] }
    if ($o['phases'] -isnot [object[]]) { $p.Add('phases no es una lista.'); return $p }
    $vistas = @{}; $maxNivel = 0
    $kf = @('level', 'phase', 'attempts', 'correctedErrors', 'stepsUsed', 'resolutionSeconds')
    foreach ($f in $o['phases']) {
        if ($f -isnot [hashtable]) { $p.Add('Una fase no es un objeto.'); continue }
        $faltan = @($kf | Where-Object { -not $f.ContainsKey($_) }); $sobran = @($f.Keys | Where-Object { $_ -notin $kf })
        if ($faltan -or $sobran) { $p.Add("Fase con claves incorrectas (faltan: $($faltan -join ',') · sobran: $($sobran -join ','))."); continue }
        foreach ($k in 'level', 'phase', 'attempts', 'correctedErrors', 'stepsUsed') { if (-not (Test-Entero $f[$k]) -or $f[$k] -lt 0) { $p.Add("fase.$k no es un entero ≥ 0.") } }
        $rs = $f['resolutionSeconds']
        if ($rs -isnot [double] -and -not (Test-Entero $rs)) { $p.Add('fase.resolutionSeconds no es un número.') }
        elseif ($rs -lt 0) { $p.Add('fase.resolutionSeconds es negativo.') }
        if ((Test-Entero $f['level']) -and (Test-Entero $f['phase'])) {
            $nv = [int]$f['level']; $fa = [int]$f['phase']
            if (-not $porNivel.ContainsKey($nv) -or $fa -lt 1 -or $fa -gt $porNivel[$nv]) { $p.Add("La fase $nv/$fa no existe (fases por nivel: 1, 3, 3).") }
            elseif ($vistas.ContainsKey("$nv/$fa")) { $p.Add("Fase $nv/$fa repetida.") }
            else { $vistas["$nv/$fa"] = $true; $maxNivel = [math]::Max($maxNivel, $nv) }
        }
    }
    if ($alcanzado -gt 0 -and $maxNivel -gt $alcanzado) { $p.Add("Hay fases del nivel $maxNivel pero reachedLevel es $alcanzado.") }
    $p
}

# Una semilla por nombre (herramientas\perfiles\<nombre>.json) o una ruta (cwd o relativa a la carpeta OE4).
function Resolve-Perfil([string]$A) {
    $esRuta = ($A -match '[\\/]') -or ($A -like '*.json')
    $cands = if ($esRuta) { @($A, (Join-Path (Join-Path $Aqui '..') $A)) } else { @((Join-Path $Aqui "perfiles\$A.json")) }
    $f = $cands | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
    if (-not $f) {
        $hay = (Get-ChildItem -LiteralPath (Join-Path $Aqui 'perfiles') -Filter *.json -ErrorAction SilentlyContinue | ForEach-Object { $_.BaseName }) -join ', '
        Stop-With 2 "Perfil '$A' no encontrado (buscado: $($cands -join ' | ')). Semillas disponibles: $hay"
    }
    $f = (Resolve-Path -LiteralPath $f).Path
    $bytes = [IO.File]::ReadAllBytes($f)
    $texto = $Utf8.GetString($bytes).TrimStart([char]0xFEFF)
    $base = [IO.Path]::GetFileNameWithoutExtension($f)
    $corrupto = $base -match 'Corrupto'
    $nombre = $base; $parsea = $true
    try { $o = $texto | ConvertFrom-Json -AsHashtable -Depth 10; if ($o -is [hashtable] -and $o.name -is [string] -and $o.name) { $nombre = $o.name } } catch { $parsea = $false }
    if ($corrupto) {
        if ($parsea) { Say "AVISO: '$base' se llama «Corrupto» pero parsea como JSON: no servirá para PF-RF02-04." }
    }
    else {
        $problemas = Test-Perfil $texto $nombre
        if ($problemas.Count -gt 0) { Stop-With 2 ("El perfil '$A' ($f) no es válido:`n  - " + ($problemas -join "`n  - ")) }
    }
    @{ Nombre = $nombre; Bytes = $bytes; Origen = $f }
}

# --- Residuos: foto previa y diferencia ------------------------------------------------------------
function Get-Listado([string]$Dir, [switch]$Recursivo) {
    $r = @{}
    if (-not (Test-Path -LiteralPath $Dir)) { return $r }
    $base = $Dir.TrimEnd('\')
    $items = if ($Recursivo) { Get-ChildItem -LiteralPath $Dir -Force -Recurse -ErrorAction SilentlyContinue } else { Get-ChildItem -LiteralPath $Dir -Force -ErrorAction SilentlyContinue }
    foreach ($i in $items) {
        $rel = $i.FullName.Substring($base.Length + 1)
        $r[$rel] = @{ dir = [bool]$i.PSIsContainer; len = $(if ($i.PSIsContainer) { [long]0 } else { [long]$i.Length }); mt = [long]$i.LastWriteTimeUtc.Ticks }
    }
    $r
}
function Get-RegistroSnap {
    $hk = [Microsoft.Win32.Registry]::CurrentUser
    $padre = $RegSub.Substring(0, $RegSub.LastIndexOf('\'))
    $s = @{ clave = $RegSub; existe = $false; empresaExiste = $false; valores = @(); subclaves = @() }
    $k0 = $hk.OpenSubKey($padre); if ($k0) { $s.empresaExiste = $true; $k0.Dispose() }
    $k = $hk.OpenSubKey($RegSub)
    if ($k) {
        $s.existe = $true
        $s.subclaves = @($k.GetSubKeyNames())
        $vals = @()
        foreach ($n in $k.GetValueNames()) {
            $tipo = $k.GetValueKind($n)
            $v = $k.GetValue($n, $null, [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames)
            $dato = switch ($tipo) { 'Binary' { [Convert]::ToBase64String([byte[]]$v) } 'MultiString' { @($v) } default { $v } }
            $vals += @{ nombre = $n; tipo = [string]$tipo; valor = $dato }
        }
        $s.valores = $vals
        $k.Dispose()
    }
    $s
}
function Get-FotoResiduos {
    @{
        locallowDir = $LogDir
        locallow    = (Get-Listado $LogDir -Recursivo)
        tempDir     = $env:TEMP
        temp        = (Get-Listado $env:TEMP)       # solo el primer nivel: %TEMP% es enorme y ajeno
        registro    = (Get-RegistroSnap)
    }
}
function Save-Previo([string]$Ses, [switch]$Forzar) {
    $f = Join-Path (New-Dir (Join-Path $Trabajo 'residuos')) "previo-$Ses.json"
    if ((Test-Path -LiteralPath $f) -and -not $Forzar) { Say "Foto previa de residuos de '$Ses': ya existía, se conserva ($f)."; return $f }
    $foto = Get-FotoResiduos
    $foto.sesion = $Ses; $foto.utc = Iso ([DateTime]::UtcNow)
    Write-Text $f ($foto | ConvertTo-Json -Depth 30)
    Say "Foto previa de residuos de '$Ses': $($foto.locallow.Count) entradas en LocalLow, $($foto.temp.Count) en %TEMP%, clave del registro $(if ($foto.registro.existe) { 'EXISTE' } else { 'no existe' }) ($f)."
    $f
}
function Compare-Listado($A, $B) {
    $n = @(); $m = @(); $q = @()
    foreach ($k in $B.Keys) {
        if (-not $A.ContainsKey($k)) { $n += $k }
        elseif ($A[$k].len -ne $B[$k].len -or $A[$k].mt -ne $B[$k].mt) { $m += $k }
    }
    foreach ($k in $A.Keys) { if (-not $B.ContainsKey($k)) { $q += $k } }
    @{ Nuevos = @($n | Sort-Object); Cambiados = @($m | Sort-Object); Quitados = @($q | Sort-Object) }
}
function Format-Tam($L) { if ($L -ge 1MB) { '{0:N1} MB' -f ($L / 1MB) } elseif ($L -ge 1KB) { '{0:N1} KB' -f ($L / 1KB) } else { "$L B" } }

# --- Red y memoria (muestreador) --------------------------------------------------------------------
function Test-LoopbackIp([string]$Ip) {
    $a = $null
    if (-not [Net.IPAddress]::TryParse(($Ip -replace '%.*$', ''), [ref]$a)) { return $false }
    if ($a.IsIPv4MappedToIPv6) { $a = $a.MapToIPv4() }
    [Net.IPAddress]::IsLoopback($a)
}
function Test-SinDireccion([string]$Ip) { (-not $Ip) -or $Ip -in '0.0.0.0', '::', '*' }
# clase: loopback · escucha-local (socket abierto sin interlocutor) · EXTERNA (interlocutor que no es loopback)
function Get-Conexiones([int]$ProcId) {
    $filas = @()
    $tcp = @(Get-NetTCPConnection -OwningProcess $ProcId -ErrorAction SilentlyContinue)
    foreach ($c in $tcp) {
        $clase = if (Test-SinDireccion $c.RemoteAddress) { if (Test-LoopbackIp $c.LocalAddress) { 'loopback' } else { 'escucha-local' } }
        elseif (Test-LoopbackIp $c.RemoteAddress) { 'loopback' } else { 'EXTERNA' }
        $filas += [pscustomobject]@{ proto = 'TCP'; local = "$($c.LocalAddress):$($c.LocalPort)"; remoto = "$($c.RemoteAddress):$($c.RemotePort)"; estado = "$($c.State)"; clase = $clase }
    }
    $udp = @(Get-NetUDPEndpoint -OwningProcess $ProcId -ErrorAction SilentlyContinue)
    foreach ($c in $udp) {
        $clase = if (Test-LoopbackIp $c.LocalAddress) { 'loopback' } else { 'escucha-local' }
        $filas += [pscustomobject]@{ proto = 'UDP'; local = "$($c.LocalAddress):$($c.LocalPort)"; remoto = ''; estado = 'bound'; clase = $clase }
    }
    $filas
}

# --- Subcomandos -----------------------------------------------------------------------------------
function Show-Help {
    Write-Host @'
oe4.ps1: arnés de Algoritmia.exe (cierre OE4). Uso: pwsh -NoProfile -File oe4.ps1 <Subcomando> [argumentos] [-Opciones]
Coordenadas: fracciones 0-1 del área cliente, origen arriba a la izquierda (admite 0.5 y 0,5).

  Start-Juego [sesión] [-Exe r] [-Ancho 1920 -Alto 1080 | -Completa] [-SinMarco] [-Sha256 hash|archivo]
  Foto <nombre> [-Gris] [-Espera ms]
  Clic <x> <y> [-Veces n] [-Intervalo ms]            (también -ConFoto <nombre> [-FotoMs 500])
  Sostener <x> <y> <ms>                              (también -ConFoto)
  Arrastrar <x1> <y1> <x2> <y2> [ms=700]             (también -ConFoto)
  Escribir <texto>
  Teclas <Esc Enter Espacio Arriba Abajo Izquierda Derecha W A S D F1..F12 Rueda Ctrl+A Retroceso*24 ...> [-Pausa ms]
  Esperar-Carga <escena|*> [segundos=30]             (alias Medir-Carga; devuelve N de «RNF-04: «escena» cargó en N s»)
  Rafaga <prefijo> <segundos> <fps>
  Preparar-Datos <perfil|ruta.json ...> [-SinCarpeta]   (juego cerrado: vacía <exe>\Datos y copia semillas validadas)
  Matar [-Todos]    Cerrar-Juego
  Revisar-Log [sesión]    Residuos [sesión] [-Previo] [-Buscar t1,t2]    Restaurar-Registro
  Tamano [-Carpeta r]     Medidas [sesión|-]     Estado

Salidas: 0 bien · 1 hallazgo · 2 fallo de infraestructura · 3 guarda (sin foco, ventana encima, SHA distinto, juego en marcha, otra invocación).
Detalle completo: oe4-USO.md.
'@
}

function Do-Estado {
    Initialize-Native
    $e = Get-Estado
    Say "Trabajo: $Trabajo $(if (Test-Path -LiteralPath $EstadoFile) { '(estado.json presente)' } else { '(sin estado.json)' })"
    Say "Arnés: DPI $([Oe4]::DpiMode()) · escritorio virtual $(([Oe4]::Virtual()) -join ' ') · monitores: $([Oe4]::Monitors())"
    Say "Primer plano ahora: $([Oe4]::Describe([Oe4]::Foreground()))"
    Say "Registro del juego: $(Get-LogPath) $(if (Test-Path -LiteralPath (Get-LogPath)) { '(existe, ' + (Format-Tam (Get-LogLength)) + ')' } else { '(no existe)' })"
    if (-not $e.pid) { Say 'Juego: ninguno lanzado por el arnés.'; return }
    Say "Sesión '$($e.sesion)' · lanzamiento $($e.lanzamiento) · exe $($e.exe)"
    Say "SHA-256 del exe: $($e.exeSha256)"
    $p = Get-Juego -Opcional
    if (-not $p) { Say "Juego: pid $($e.pid) YA NO ESTÁ ($($e.fin) a las $($e.finUtc))."; return }
    $h = [Oe4]::FindWindow($p.Id)
    Say "Juego: pid $($p.Id) vivo desde $($e.inicioUtc) · memoria $(Format-Tam $p.WorkingSet64) · modo $($e.modo) $($e.ancho)x$($e.alto)"
    if ($h -ne [IntPtr]::Zero) {
        $c = [Oe4]::Client($h)
        Say "Ventana: $([Oe4]::Describe($h)) · cliente $($c[2])x$($c[3]) en ($($c[0]),$($c[1])) · $([Oe4]::WhereIs($h)) · primer plano: $($h -eq [Oe4]::Foreground())"
    }
    else { Say 'Ventana: no se encuentra.' }
    $m = if ($e.muestreadorPid) { Get-Process -Id ([int]$e.muestreadorPid) -ErrorAction SilentlyContinue } else { $null }
    Say "Muestreador: $(if ($m) { "vivo (pid $($m.Id))" } else { 'NO está corriendo' })"
    if ($e.ultimaCarga) { Say "Última carga: $($e.ultimaCarga.escena) en $($e.ultimaCarga.segundos) s" }
}

function Get-ShaRef {
    $v = if ($Sha256) { $Sha256 } elseif ($env:OE4_SHA256) { $env:OE4_SHA256 } else { return $null }
    if ($v -match '^[0-9a-fA-F]{64}$') { return $v.ToUpperInvariant() }
    if (Test-Path -LiteralPath $v -PathType Leaf) {
        $t = Get-Content -LiteralPath $v -Raw
        foreach ($l in ($t -split "`r?`n")) { if ($l -match 'Algoritmia\.exe' -and $l -match '\b([0-9a-fA-F]{64})\b') { return $Matches[1].ToUpperInvariant() } }
        if ($t -match '\b([0-9a-fA-F]{64})\b') { return $Matches[1].ToUpperInvariant() }
    }
    Stop-With 2 "-Sha256: ni 64 hexadecimales ni un archivo que contenga un SHA-256 ($v)."
}

# Aparta los Player.log de la corrida anterior: así el que exista tras lanzar es SIEMPRE el nuevo (el
# «tunneling» de NTFS conserva la fecha de creación del archivo renombrado, y la de escritura se queda atrás).
function Move-LogsPrevios([string]$Ses, [int]$Lanz) {
    $dest = New-Dir (Join-Path $Trabajo 'logs')
    $sello = [DateTime]::Now.ToString('yyyyMMdd-HHmmss')
    foreach ($n in 'Player.log', 'Player-prev.log') {
        $src = Join-Path $LogDir $n
        if (-not (Test-Path -LiteralPath $src)) { continue }
        $e = Get-Estado
        $destino = if ($e.sesion -eq $Ses -and $Lanz -gt 1 -and $n -eq 'Player.log') { "${Ses}_lanz$($Lanz - 1)_Player.txt" } else { "previo-$sello-$([IO.Path]::GetFileNameWithoutExtension($n)).txt" }
        Move-Item -LiteralPath $src -Destination (Join-Path $dest $destino) -Force
        Say "Registro anterior apartado: $n -> logs\$destino"
    }
}

function Do-StartJuego {
    Initialize-Native
    $exe = Resolve-Exe
    if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) { Stop-With 2 "No existe el ejecutable: $exe" }
    $dir = Split-Path $exe -Parent
    $nombreProc = [IO.Path]::GetFileNameWithoutExtension($exe)
    $vivos = @(Get-Process -Name $nombreProc -ErrorAction SilentlyContinue)
    if ($vivos.Count -gt 0) { Stop-With 3 "Ya hay un proceso '$nombreProc' en ejecución (pid $(($vivos | ForEach-Object { $_.Id }) -join ', ')). Use Matar o Matar -Todos. No se lanzó nada." }
    if ($Completa -and ($Ancho -ne 1920 -or $Alto -ne 1080) -and $PSBoundParameters.ContainsKey('Ancho')) { Say 'AVISO: -Completa ignora -Ancho y -Alto.' }

    $sha = (Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash
    $ref = Get-ShaRef
    if ($ref -and $ref -ne $sha) {
        Stop-With 3 "SHA-256 DISTINTO: el exe es $sha y la referencia $ref. Es otra versión que la congelada: la sesión no contaría. No se lanzó nada."
    }
    $contam = @(Get-Process -ErrorAction SilentlyContinue | Where-Object { $_.ProcessName -in 'Unity', 'WINWORD' } | ForEach-Object { "$($_.ProcessName)#$($_.Id)" })
    if ($contam.Count -gt 0) { Say "AVISO DE CONTAMINACIÓN: hay abiertos $($contam -join ', '). Las cargas (RNF-04) y la memoria (RNF-05) medidas así no valen: cierre el Editor y Word antes de medir." }

    $e = Get-Estado
    $ses = if ($Sesion) { Safe-Name $Sesion } elseif ($Rest.Count -gt 0) { Safe-Name $Rest[0] } elseif ($e.sesion) { [string]$e.sesion } else { 'sesion-' + [DateTime]::Now.ToString('yyyyMMdd-HHmmss') }
    $lanz = if ($e.sesion -eq $ses -and $e.lanzamiento) { [int]$e.lanzamiento + 1 } else { 1 }

    Move-LogsPrevios $ses $lanz
    $previo = Save-Previo $ses

    $argv = if ($Completa) {
        $pr = [Oe4]::Primary()
        @('-screen-fullscreen', '1', '-screen-width', "$($pr[0])", '-screen-height', "$($pr[1])", '-monitor', '1')
    }
    else {
        $a = @('-screen-fullscreen', '0', '-screen-width', "$Ancho", '-screen-height', "$Alto", '-monitor', '1')
        # Un cliente del tamaño del monitor no cabe con marco: Unity lo deja bajo la barra de título y el borde inferior
        # cae fuera de la pantalla (spike 01/10/2026, monitor de 1920x1080: 38 px perdidos). Sin marco cabe entero.
        $pr = [Oe4]::Primary()
        if (-not $SinMarco -and ($Ancho -ge $pr[0] -or $Alto -ge $pr[1])) {
            Say "AVISO: el cliente pedido (${Ancho}x${Alto}) no cabe con marco en el monitor principal ($($pr[0])x$($pr[1])): se lanza sin marco (-popupwindow)."
            $SinMarco = [switch]$true
        }
        if ($SinMarco) { $a += '-popupwindow' }
        $a
    }
    $p = Start-Process -FilePath $exe -ArgumentList $argv -WorkingDirectory $dir -PassThru
    $ticks = $p.StartTime.ToUniversalTime().Ticks
    $prov = $null
    $provF = Join-Path $dir 'Algoritmia.provenance.json'
    if (Test-Path -LiteralPath $provF) { try { $prov = Get-Content -LiteralPath $provF -Raw | ConvertFrom-Json } catch { } }
    Save-Estado @{
        sesion = $ses; lanzamiento = $lanz; exe = $exe; exeSha256 = $sha
        pid = $p.Id; inicioTicks = $ticks; inicioUtc = (Iso $p.StartTime)
        modo = $(if ($Completa) { 'completa' } else { 'ventana' }); ancho = $(if ($Completa) { [Oe4]::Primary()[0] } else { $Ancho }); alto = $(if ($Completa) { [Oe4]::Primary()[1] } else { $Alto })
        logOffset = [long]0; marcaUtc = (Iso $p.StartTime); previo = $previo; contaminacion = $contam
        fin = $null; finUtc = $null; ultimaCarga = $null
    }

    # La ventana: espera a que exista y a que su cliente deje de cambiar (Unity la crea y la redimensiona).
    $t0 = Get-Date; $h = [IntPtr]::Zero; $estable = 0; $ultimo = ''
    while (((Get-Date) - $t0).TotalSeconds -lt 60) {
        if ($p.HasExited) { Stop-With 2 "El juego terminó (código $($p.ExitCode)) antes de abrir ventana. Revise el Player.log con Revisar-Log." }
        $h = [Oe4]::FindWindow($p.Id)
        if ($h -ne [IntPtr]::Zero) {
            $c = [Oe4]::Client($h); $clave = "$($c[2])x$($c[3])"
            if ($c[2] -gt 100 -and $clave -eq $ultimo) { $estable++ } else { $estable = 0 }
            $ultimo = $clave
            if ($estable -ge 3) { break }
        }
        Start-Sleep -Milliseconds 300
    }
    if ($estable -lt 3) { Stop-With 2 "En 60 s no apareció una ventana estable del juego (pid $($p.Id) sigue vivo: use Matar o Estado)." }

    $m = Start-Process -FilePath (Get-Process -Id $PID).Path -WindowStyle Hidden -PassThru -ArgumentList @(
        '-NoProfile', '-File', (Quote $PSCommandPath), 'Muestrear', "$($p.Id)", "$ticks", (Quote $ses), "$lanz")
    Set-Estado @{ muestreadorPid = $m.Id; cliente = @{ w = $c[2]; h = $c[3]; x = $c[0]; y = $c[1] } }

    $foco = [Oe4]::EnsureForeground($h, 4)
    Say "Juego en marcha: pid $($p.Id) · sesión '$ses' · lanzamiento $lanz · ventana $([Oe4]::Describe($h))"
    Say "Cliente $($c[2])x$($c[3]) en ($($c[0]),$($c[1])) · $([Oe4]::WhereIs($h)) · primer plano: $foco"
    if (-not $Completa -and ($c[2] -ne $Ancho -or $c[3] -ne $Alto)) { Say "AVISO: se pidió un cliente de ${Ancho}x${Alto} y Unity dejó $($c[2])x$($c[3]) (la pantalla no da para el marco: pruebe -SinMarco o -Completa). Las capturas saldrán a ese tamaño." }
    $v = [Oe4]::Virtual()
    if ($c[0] -lt $v[0] -or $c[1] -lt $v[1] -or ($c[0] + $c[2]) -gt ($v[0] + $v[2]) -or ($c[1] + $c[3]) -gt ($v[1] + $v[3])) { Say 'AVISO: parte del área cliente cae fuera del escritorio virtual: las fotos saldrán incompletas.' }
    Say "SHA-256 del exe: $sha $(if ($ref) { '(coincide con la referencia)' } else { '(sin referencia: anótelo en la cabecera de resultados)' })"
    if ($prov) { Say "Procedencia: revisión $($prov.source.revision) $(if ($prov.source.dirty) { '(árbol SUCIO al compilar)' } else { '(árbol limpio)' }) · compilado $($prov.endedAt)" }
    Say "Muestreador oculto: pid $($m.Id) · muestras en $(Join-Path $Trabajo 'muestras') · foto previa: $previo"
}

function Do-Foto([string]$Nombre, [int]$EsperaMs = 0) {
    if (-not $Nombre) { Stop-With 2 'Uso: Foto <nombre> [-Gris] [-Espera ms]' }
    $h = Prepare-Input
    if ($EsperaMs -gt 0) { Start-Sleep -Milliseconds $EsperaMs }
    $c = [Oe4]::Client($h)
    if ($c[2] -lt 2 -or $c[3] -lt 2) { Stop-With 2 'La ventana no tiene área cliente.' }
    $v = [Oe4]::Virtual()
    $fuera = ($c[0] -lt $v[0] -or $c[1] -lt $v[1] -or ($c[0] + $c[2]) -gt ($v[0] + $v[2]) -or ($c[1] + $c[3]) -gt ($v[1] + $v[3]))
    $dir = New-Dir (Join-Path $Trabajo 'fotos')
    $base = (Safe-Name $Nombre) -replace '\.png$', ''
    $f = Join-Path $dir "$base.png"
    $bmp = $null; $bmpGris = $null  # no «$gris»: PowerShell no distingue mayúsculas y taparía el switch -Gris
    try {
        $t = Get-Date
        try { $bmp = [Oe4]::Capture($c[0], $c[1], $c[2], $c[3]) }
        catch { Stop-With 2 "No se pudo capturar la pantalla: $($_.Exception.Message). ¿Pantalla bloqueada o sesión de escritorio remoto desconectada?" }
        $l = [Oe4]::Luma($bmp, 2)
        $bmp.Save($f, [Drawing.Imaging.ImageFormat]::Png)
        $extra = ''
        if ($Gris) {
            $bmpGris = [Oe4]::Gray($bmp)
            $fg = Join-Path $dir "${base}_gris.png"
            $bmpGris.Save($fg, [Drawing.Imaging.ImageFormat]::Png)
            $extra = "`nFOTO $fg"
        }
    }
    finally { if ($bmp) { $bmp.Dispose() }; if ($bmpGris) { $bmpGris.Dispose() } }
    Log-Accion 'Foto' "$base $($c[2])x$($c[3])" $t
    Say ("Foto {0}x{1} · luminancia relativa media {2:0.000} (0 = negro) · {3}" -f $c[2], $c[3], $l[0], $(if ($l[0] -lt 0.002) { 'AVISO: cuadro casi NEGRO (¿fundido, escena oscura o captura bloqueada?)' } else { 'con contenido' }))
    if ($fuera) { Say 'AVISO: parte del área cliente cae fuera del escritorio virtual: la foto está incompleta.' }
    Say "FOTO $f$extra"
}

function Do-Clic {
    if ($Rest.Count -lt 2) { Stop-With 2 'Uso: Clic <x> <y> [-Veces n] [-Intervalo ms] [-ConFoto nombre] [-FotoMs 500]' }
    $x = ConvertTo-Norm $Rest[0]; $y = ConvertTo-Norm $Rest[1]
    $h = Prepare-Input
    $pt = Get-Punto $h $x $y
    Assert-Libre $h $pt
    Mark-Offset
    $t = Get-Date
    $n = [math]::Max(1, $Veces)
    Check-Result ([Oe4]::Click($h, $pt.X, $pt.Y, $n, [math]::Max(70, $Intervalo), 60))
    Log-Accion 'Clic' "$x $y x$n/${Intervalo}ms -> ($($pt.X),$($pt.Y))" $t
    Say "Clic en ($x; $y) = píxel ($($pt.X),$($pt.Y))$(if ($n -gt 1) { " ×$n cada ${Intervalo} ms" })."
    if ($ConFoto) { Do-Foto $ConFoto $FotoMs }
}

function Do-Sostener {
    if ($Rest.Count -lt 3) { Stop-With 2 'Uso: Sostener <x> <y> <ms> [-ConFoto nombre]' }
    $x = ConvertTo-Norm $Rest[0]; $y = ConvertTo-Norm $Rest[1]
    $ms = 0; if (-not [int]::TryParse($Rest[2], [ref]$ms) -or $ms -lt 50 -or $ms -gt 60000) { Stop-With 2 "Duración inválida '$($Rest[2])': de 50 a 60000 ms." }
    $h = Prepare-Input
    $pt = Get-Punto $h $x $y
    Assert-Libre $h $pt
    Mark-Offset
    $t = Get-Date
    Check-Result ([Oe4]::Hold($h, $pt.X, $pt.Y, $ms))
    Log-Accion 'Sostener' "$x $y ${ms}ms -> ($($pt.X),$($pt.Y))" $t
    Say "Clic sostenido ${ms} ms en ($x; $y) = píxel ($($pt.X),$($pt.Y))."
    if ($ConFoto) { Do-Foto $ConFoto $FotoMs }
}

function Do-Arrastrar {
    if ($Rest.Count -lt 4) { Stop-With 2 'Uso: Arrastrar <x1> <y1> <x2> <y2> [ms=700] [-ConFoto nombre]' }
    $x1 = ConvertTo-Norm $Rest[0]; $y1 = ConvertTo-Norm $Rest[1]; $x2 = ConvertTo-Norm $Rest[2]; $y2 = ConvertTo-Norm $Rest[3]
    $ms = 700; if ($Rest.Count -gt 4 -and (-not [int]::TryParse($Rest[4], [ref]$ms) -or $ms -lt 100 -or $ms -gt 30000)) { Stop-With 2 "Duración inválida '$($Rest[4])': de 100 a 30000 ms." }
    $h = Prepare-Input
    $a = Get-Punto $h $x1 $y1; $b = Get-Punto $h $x2 $y2
    Assert-Libre $h $a; Assert-Libre $h $b
    Mark-Offset
    $t = Get-Date
    Check-Result ([Oe4]::Drag($h, $a.X, $a.Y, $b.X, $b.Y, $ms, 20))
    Log-Accion 'Arrastrar' "$x1 $y1 -> $x2 $y2 ${ms}ms" $t
    Say "Arrastre ($x1; $y1) -> ($x2; $y2) en ${ms} ms = píxeles ($($a.X),$($a.Y)) -> ($($b.X),$($b.Y))."
    if ($ConFoto) { Do-Foto $ConFoto $FotoMs }
}

function Do-Escribir {
    $texto = $Rest -join ' '
    if (-not $texto) { Stop-With 2 'Uso: Escribir <texto>' }
    $h = Prepare-Input
    Mark-Offset
    $t = Get-Date
    Check-Result ([Oe4]::TypeText($h, $texto, 40))
    Log-Accion 'Escribir' "$($texto.Length) caracteres" $t
    Say "Escritos $($texto.Length) caracteres."
}

$Teclas = @{
    'esc' = 0x1B; 'escape' = 0x1B; 'enter' = 0x0D; 'intro' = 0x0D; 'return' = 0x0D; 'espacio' = 0x20; 'space' = 0x20; 'tab' = 0x09
    'retroceso' = 0x08; 'backspace' = 0x08; 'supr' = 0x2E; 'delete' = 0x2E; 'arriba' = 0x26; 'up' = 0x26; 'abajo' = 0x28; 'down' = 0x28
    'izquierda' = 0x25; 'left' = 0x25; 'derecha' = 0x27; 'right' = 0x27; 'inicio' = 0x24; 'home' = 0x24; 'fin' = 0x23; 'end' = 0x23
    'shift' = 0x10; 'mayus' = 0x10; 'ctrl' = 0x11; 'control' = 0x11; 'alt' = 0x12
}
function Get-VkTecla([string]$Nombre) {
    $n = $Nombre.ToLowerInvariant()
    if ($Teclas.ContainsKey($n)) { return [uint16]$Teclas[$n] }
    if ($n -match '^[a-z]$') { return [uint16][int][char]$n.ToUpperInvariant() }
    if ($n -match '^[0-9]$') { return [uint16][int][char]$n }
    if ($n -match '^f([1-9]|1[0-2])$') { return [uint16](0x6F + [int]$Matches[1]) }
    Stop-With 2 "Tecla desconocida '$Nombre'. Use: $((($Teclas.Keys | Sort-Object) -join ' ')) a-z 0-9 f1-f12 Rueda."
}
function Do-Teclas {
    if ($Rest.Count -lt 1) { Stop-With 2 'Uso: Teclas <Esc Enter Espacio Arriba … Rueda Ctrl+A Retroceso*24> [-Pausa ms]' }
    $h = Prepare-Input
    Mark-Offset
    $t = Get-Date
    $c = [Oe4]::Client($h); $centro = Get-Punto $h 0.5 0.5
    Assert-Libre $h $centro
    foreach ($tok in $Rest) {
        $veces = 1; $spec = $tok
        if ($tok -match '^(.+)\*(\d+)$') { $spec = $Matches[1]; $veces = [int]$Matches[2] }
        for ($i = 0; $i -lt $veces; $i++) {
            if ($spec -in 'Rueda', 'RuedaArriba', 'RuedaAbajo') {
                if ($spec -ne 'RuedaAbajo') { Check-Result ([Oe4]::Wheel($h, $centro.X, $centro.Y, 3)); Start-Sleep -Milliseconds $Pausa }
                if ($spec -ne 'RuedaArriba') { Check-Result ([Oe4]::Wheel($h, $centro.X, $centro.Y, -3)) }
            }
            else {
                $partes = @($spec -split '\+')
                $mods = [uint16[]]@($partes[0..([math]::Max(0, $partes.Count - 2))] | Where-Object { $partes.Count -gt 1 } | ForEach-Object { Get-VkTecla $_ })
                $vk = Get-VkTecla $partes[-1]
                Check-Result ([Oe4]::Press($h, $mods, $vk, 60))
            }
            Start-Sleep -Milliseconds $Pausa
        }
    }
    Log-Accion 'Teclas' ($Rest -join ' ') $t
    Say "Teclas enviadas: $($Rest -join ' ')."
}

function Do-EsperarCarga {
    if ($Rest.Count -lt 1) { Stop-With 2 'Uso: Esperar-Carga <escena|*> [segundos=30]' }
    $escena = $Rest[0]
    $limite = 30.0
    if ($Rest.Count -gt 1 -and -not [double]::TryParse($Rest[1].Replace(',', '.'), [Globalization.NumberStyles]::Float, $Inv, [ref]$limite)) { Stop-With 2 "Segundos inválidos: '$($Rest[1])'." }
    $e = Get-Estado
    $off = if ($e.logOffset) { [long]$e.logOffset } else { [long]0 }
    $fin = [DateTime]::UtcNow.AddSeconds($limite)
    $vistas = [Collections.Generic.List[string]]::new()
    while ($true) {
        $r = Read-LogChunk $off
        $off2 = [long]$r.Offset
        if ($r.Texto) {
            $acum = 0
            foreach ($linea in ($r.Texto -split "`n")) {
                $acum += $Utf8.GetByteCount($linea) + 1
                $m = $LineaCarga.Match($linea)
                if (-not $m.Success) { continue }
                $vistas.Add($m.Groups['e'].Value)
                if ($escena -in '*', '-' -or $m.Groups['e'].Value -ieq $escena) {
                    $seg = [double]::Parse($m.Groups['n'].Value.Replace(',', '.'), $Inv)
                    # El offset persistido queda justo después de la línea encontrada.
                    $nuevo = [long]$off + $acum
                    $juego = Get-Juego -Opcional
                    $ses = if ($e.sesion) { [string]$e.sesion } else { 'sesion' }
                    $null = New-Dir $Trabajo
                    $cab = Join-Path $Trabajo 'cargas.csv'
                    if (-not (Test-Path -LiteralPath $cab)) { Add-Text $cab "utc,sesion,lanzamiento,escena,segundos`n" }
                    Add-Text $cab ("{0},{1},{2},{3},{4}`n" -f (Iso ([DateTime]::UtcNow)), $ses, $e.lanzamiento, $m.Groups['e'].Value, $seg.ToString('0.000', $Inv))
                    # La hora de escritura de la línea, con la precisión del muestreador (250 ms); el log no lleva hora.
                    # El registro acumula todos los lanzamientos y las líneas se repiten («Narrative» cargó en 0.035 s):
                    # vale la PRIMERA igual de este lanzamiento escrita después de la marca (spike 01/10/2026: tomaba la
                    # última de cualquier lanzamiento y daba una hora de minutos antes).
                    $reg = Join-Path $Trabajo "muestras\${ses}_registro.tsv"
                    $hora = ''; $hit = $null; $marca = $e.marcaUtc
                    if (Test-Path -LiteralPath $reg) {
                        $desde = if ($e.marcaUtc) { (ParseIso $e.marcaUtc).AddSeconds(-0.5) } else { [datetime]::MinValue }
                        $lz = "$($e.lanzamiento)"; $fin = $linea.TrimEnd("`r")
                        for ($k = 0; $k -lt 5 -and -not $hit; $k++) {
                            if ($k) { Start-Sleep -Milliseconds 150 }      # el muestreador copia el log cada 250 ms
                            $hit = Get-Content -LiteralPath $reg -Encoding utf8 | Where-Object {
                                $c = $_ -split "`t", 3; $c.Count -eq 3 -and $c[1] -eq $lz -and $c[2] -eq $fin -and (ParseIso $c[0]) -ge $desde } | Select-Object -First 1
                        }
                        if ($hit) {
                            $t = ParseIso (($hit -split "`t")[0])
                            $hora = ' · escrita hacia las ' + $t.ToLocalTime().ToString('HH:mm:ss.fff')
                            $marca = Iso ($t.AddMilliseconds(501))   # la próxima espera no vuelve a tomar esta línea
                        }
                    }
                    Set-Estado @{ logOffset = $nuevo; marcaUtc = $marca; ultimaCarga = @{ escena = $m.Groups['e'].Value; segundos = $seg; utc = (Iso ([DateTime]::UtcNow)) } }
                    $ok = $seg -lt $PresupuestoCargaS
                    Say ("Carga de «{0}»: {1:0.000} s · {2}{3}" -f $m.Groups['e'].Value, $seg, $(if ($ok) { "dentro de los $PresupuestoCargaS s de RNF-04" } else { "FUERA del presupuesto de $PresupuestoCargaS s (RNF-04)" }), $hora)
                    Say ($seg.ToString('0.000', $Inv))
                    if (-not $ok) { exit 1 }
                    return
                }
            }
        }
        $off = $off2
        if (-not (Get-Juego -Opcional) -and -not $r.Texto) {
            Stop-With 2 ("El juego terminó sin dejar la línea de carga de «$escena». Vistas: " + $(if ($vistas.Count) { $vistas -join ', ' } else { 'ninguna' }))
        }
        if ([DateTime]::UtcNow -ge $fin) {
            Stop-With 2 ("Pasaron $limite s sin la línea «RNF-04: «$escena» cargó en N s» en $(Get-LogPath). Cargas vistas desde el offset: " + $(if ($vistas.Count) { $vistas -join ', ' } else { 'ninguna' }) +
                '. ¿El ejecutable lleva el registro de SceneLoader? ¿La escena se llama así?')
        }
        Start-Sleep -Milliseconds 100
    }
}

function Do-Rafaga {
    if ($Rest.Count -lt 3) { Stop-With 2 'Uso: Rafaga <prefijo> <segundos> <fps>' }
    $pref = Safe-Name $Rest[0]
    $seg = 0.0; $fps = 0.0
    if (-not [double]::TryParse($Rest[1].Replace(',', '.'), [Globalization.NumberStyles]::Float, $Inv, [ref]$seg) -or $seg -lt 0.2 -or $seg -gt 120) { Stop-With 2 "Segundos inválidos '$($Rest[1])' (0,2 a 120)." }
    if (-not [double]::TryParse($Rest[2].Replace(',', '.'), [Globalization.NumberStyles]::Float, $Inv, [ref]$fps) -or $fps -lt 1 -or $fps -gt 60) { Stop-With 2 "fps inválidos '$($Rest[2])' (1 a 60)." }
    $h = Prepare-Input
    $c = [Oe4]::Client($h)
    $dir = New-Dir (Join-Path $Trabajo 'rafagas')
    $t = Get-Date
    $resumen = [Oe4]::Burst($h, $c[0], $c[1], $c[2], $c[3], $seg, $fps, (Join-Path $dir $pref))
    Log-Accion 'Rafaga' "$pref ${seg}s ${fps}fps" $t
    Say $resumen
    Say "RAFAGA $(Join-Path $dir "$pref.csv") (cuadros: $(Join-Path $dir "${pref}_NNNN.png"))"
}

# --- Datos del juego -------------------------------------------------------------------------------
function Assert-JuegoCerrado {
    $nombre = [IO.Path]::GetFileNameWithoutExtension((Resolve-Exe))
    $vivos = @(Get-Process -Name $nombre -ErrorAction SilentlyContinue)
    if ($vivos.Count -gt 0) { Stop-With 3 "El juego está en marcha (pid $(($vivos | ForEach-Object { $_.Id }) -join ', ')): con él abierto reescribiría Datos al salir. Use Cerrar-Juego o Matar. No se tocó nada." }
}
function Do-PrepararDatos {
    $exe = Resolve-Exe
    if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) { Stop-With 2 "No existe el ejecutable: $exe (se usa su carpeta)." }
    Assert-JuegoCerrado
    $datos = Join-Path (Split-Path $exe -Parent) 'Datos'
    # Primero resolver y validar TODO: si algo falla no se ha tocado nada.
    $plan = @(foreach ($a in $Rest) { Resolve-Perfil $a })
    $nombres = @($plan | ForEach-Object { $_.Nombre })
    if (($nombres | Select-Object -Unique).Count -ne $nombres.Count) { Stop-With 2 'Hay dos perfiles con el mismo nombre en la lista.' }

    if (Test-Path -LiteralPath $datos) {
        if ($SinCarpeta) { Remove-Item -LiteralPath $datos -Recurse -Force; Say "Carpeta Datos eliminada: $datos" }
        else { $q = @(Get-ChildItem -LiteralPath $datos -Force); $q | Remove-Item -Recurse -Force; Say "Datos vaciada ($($q.Count) entradas): $datos" }
    }
    if (-not $SinCarpeta) { $null = New-Dir $datos }
    # Si la ruta de respaldo (INC-34) conserva perfiles de otra prueba, el juego los listaría también.
    foreach ($j in @(Get-ChildItem -LiteralPath $LogDir -Filter *.json -File -ErrorAction SilentlyContinue)) {
        Remove-Item -LiteralPath $j.FullName -Force; Say "Perfil residual de la ruta de respaldo eliminado: $($j.FullName)"
    }
    if ($SinCarpeta -and $plan.Count -gt 0) { Stop-With 2 '-SinCarpeta no admite perfiles: no hay dónde copiarlos.' }
    foreach ($p in $plan) {
        $destino = Join-Path $datos "$($p.Nombre).json"
        [IO.File]::WriteAllBytes($destino, $p.Bytes)     # archivo nuevo: LastWriteTime = ahora
        $i = Get-Item -LiteralPath $destino
        Say ("Perfil «{0}»  <- {1}  ({2} B, LastWriteTime {3})" -f $p.Nombre, $p.Origen, $i.Length, $i.LastWriteTime.ToString('yyyy-MM-dd HH:mm:ss.fff'))
    }
    if ($plan.Count -eq 0 -and -not $SinCarpeta) { Say 'Datos queda vacía (el juego arrancará sin perfiles).' }
}

function Do-Matar {
    $e = Get-Estado
    $p = Get-Juego -Opcional
    $muertos = @()
    if ($p) { $id = $p.Id; & taskkill.exe /F /T /PID $id 2>&1 | Out-Null; $null = $p.WaitForExit(5000); $muertos += $id }
    if ($Todos) {
        $nombre = [IO.Path]::GetFileNameWithoutExtension((Resolve-Exe))
        foreach ($x in @(Get-Process -Name $nombre -ErrorAction SilentlyContinue)) { & taskkill.exe /F /T /PID $x.Id 2>&1 | Out-Null; $muertos += $x.Id }
    }
    if ($muertos.Count -eq 0) { Say 'No hay juego que matar.'; return }
    Start-Sleep -Milliseconds 400
    Set-Estado @{ fin = 'matado'; finUtc = (Iso ([DateTime]::UtcNow)) }
    Say "Juego terminado a la fuerza (taskkill /F /T): pid $($muertos -join ', '). Equivale a un cierre forzado (RNF-14)."
}

function Do-CerrarJuego {
    $p = Get-Juego -Opcional
    if (-not $p) { Say 'No hay juego en marcha.'; return }
    Initialize-Native
    $h = [Oe4]::FindWindow($p.Id)
    if ($h -ne [IntPtr]::Zero) { [Oe4]::Close($h) } else { $null = $p.CloseMainWindow() }
    if ($p.WaitForExit(10000)) { $como = 'cierre normal (WM_CLOSE)' }
    else { & taskkill.exe /F /T /PID $p.Id 2>&1 | Out-Null; $null = $p.WaitForExit(5000); $como = 'NO respondió en 10 s: se mató' }
    $e = Get-Estado
    if ($e.muestreadorPid) {
        $m = Get-Process -Id ([int]$e.muestreadorPid) -ErrorAction SilentlyContinue
        if ($m) {
            Write-Text (Join-Path $Trabajo "muestreador-$($m.Id).stop") 'stop'
            if (-not $m.WaitForExit(8000)) { Stop-Process -Id $m.Id -Force -ErrorAction SilentlyContinue }
        }
    }
    Set-Estado @{ fin = $(if ($como -like 'NO*') { 'matado' } else { 'cerrado' }); finUtc = (Iso ([DateTime]::UtcNow)) }
    Say "Juego cerrado: $como."
}

# --- Registro del juego, residuos, registro de Windows -----------------------------------------------
function Do-RevisarLog {
    $ses = Get-SesionArg -Obligatoria
    $dest = New-Dir (Join-Path $Trabajo 'logs')
    $copias = @()
    foreach ($n in 'Player.log', 'Player-prev.log') {
        $src = Join-Path $LogDir $n
        if (-not (Test-Path -LiteralPath $src)) { Say "$n : no existe en $LogDir"; continue }
        $d = Join-Path $dest "${ses}_$([IO.Path]::GetFileNameWithoutExtension($n)).txt"
        $in = Open-Shared $src
        try { $out = [IO.File]::Create($d); try { $in.CopyTo($out) } finally { $out.Dispose() } } finally { $in.Dispose() }
        $copias += $d
        Say "$n -> $d ($(Format-Tam (Get-Item -LiteralPath $d).Length))"
    }
    $apartados = @(Get-ChildItem -LiteralPath $dest -Filter "${ses}_lanz*_Player.txt" -ErrorAction SilentlyContinue | Sort-Object Name)
    foreach ($a in $apartados) { $copias += $a.FullName; Say "Corrida anterior de la sesión: $($a.FullName)" }
    if ($copias.Count -eq 0) { Stop-With 2 'No hay ningún Player.log que revisar.' }
    $graves = 0
    foreach ($f in $copias) {
        $hall = @(Select-String -LiteralPath $f -Pattern 'Exception|Error|Assert|RNF-04:' -Encoding utf8)
        Say "--- $([IO.Path]::GetFileName($f)): $($hall.Count) línea(s) con Exception/Error/Assert/RNF-04"
        foreach ($h in $hall) {
            $txt = $h.Line.Trim(); if ($txt.Length -gt 220) { $txt = $txt.Substring(0, 220) + ' …' }
            Say ("  {0,5}: {1}" -f $h.LineNumber, $txt)
            if ($h.Line -notmatch 'RNF-04:') { $graves++ }
        }
    }
    if ($graves -gt 0) { Say "HALLAZGO: $graves línea(s) con Exception/Error/Assert: cada una es un DEF hasta que se demuestre lo contrario."; exit 1 }
    Say 'Sin Exception, Error ni Assert en el registro.'
}

function Do-Residuos {
    $ses = Get-SesionArg -Obligatoria
    if ($Previo) { $null = Save-Previo $ses -Forzar; return }
    $f = Join-Path $Trabajo "residuos\previo-$ses.json"
    if (-not (Test-Path -LiteralPath $f)) { Stop-With 2 "No hay foto previa de '$ses' ($f). La toma Start-Juego, o hágala a mano con: Residuos $ses -Previo" }
    $a = Get-Content -LiteralPath $f -Raw -Encoding utf8 | ConvertFrom-Json -AsHashtable -Depth 30
    $b = Get-FotoResiduos
    $informe = [Collections.Generic.List[string]]::new()
    # En pantalla, un tope por sección (un %TEMP% ajeno puede traer cientos de líneas); el informe lleva todas.
    function Add-Linea([string]$T) { $informe.Add($T); Say $T }
    function Add-Lista([string[]]$Items, [int]$Max = 25) {
        $i = 0
        foreach ($t in $Items) { $informe.Add($t); if ($i -lt $Max) { Say $t }; $i++ }
        if ($i -gt $Max) { Say "  … y $($i - $Max) más (todas en el informe)" }
    }

    Add-Linea "== Residuos de la sesión '$ses' (foto previa: $($a.utc) UTC) =="
    $dl = Compare-Listado $a.locallow $b.locallow
    Add-Linea "-- Fuera de la carpeta portable: $($b.locallowDir)"
    Add-Lista @(
        @($dl.Nuevos | ForEach-Object { "  + nuevo     $_  ($(if ($b.locallow[$_].dir) { 'carpeta' } else { Format-Tam $b.locallow[$_].len }))" })
        @($dl.Cambiados | ForEach-Object { "  ~ cambió    $_" })
        @($dl.Quitados | ForEach-Object { "  - quitado   $_" }))
    if (-not ($dl.Nuevos + $dl.Cambiados + $dl.Quitados)) { Add-Linea '  (sin diferencias)' }

    $dt = Compare-Listado $a.temp $b.temp
    $relev = '*Universidad*', '*Algoritmia*', '*Unity*', '*Crash*'
    $esRelev = { param($n) ($relev | Where-Object { $n -like $_ }).Count -gt 0 }
    $tNuevos = @($dt.Nuevos | Where-Object { & $esRelev $_ }); $tCamb = @($dt.Cambiados | Where-Object { & $esRelev $_ })
    $otros = @($dt.Nuevos + $dt.Cambiados | Where-Object { -not (& $esRelev $_) })
    Add-Linea "-- %TEMP% ($($b.tempDir)), primer nivel"
    Add-Lista @(@($tNuevos | ForEach-Object { "  + nuevo     $_" }); @($tCamb | ForEach-Object { "  ~ cambió    $_" }))
    if (-not ($tNuevos + $tCamb)) { Add-Linea '  (nada con el nombre de la empresa, el producto, Unity ni «Crash»)' }
    Add-Linea "  otros cambios en %TEMP% (casi seguro ajenos al juego): $($otros.Count)"

    $ra = $a.registro; $rb = $b.registro
    Add-Linea "-- Registro HKCU\$($rb.clave)"
    if (-not $ra.existe -and -not $rb.existe) { Add-Linea '  no existía y no existe' }
    elseif (-not $ra.existe) { Add-Linea "  NO existía y ahora existe, con $($rb.valores.Count) valor(es): $((@($rb.valores | ForEach-Object { $_.nombre }) -join ', '))  -> Restaurar-Registro la borra" }
    else {
        $antes = @{}; foreach ($v in $ra.valores) { $antes[$v.nombre] = $v }
        $cambios = 0
        foreach ($v in $rb.valores) {
            if (-not $antes.ContainsKey($v.nombre)) { Add-Linea "  + valor nuevo $($v.nombre)"; $cambios++ }
            elseif (($antes[$v.nombre].valor | ConvertTo-Json -Compress) -ne ($v.valor | ConvertTo-Json -Compress)) { Add-Linea "  ~ valor cambiado $($v.nombre)"; $cambios++ }
        }
        if ($cambios -eq 0) { Add-Linea '  existía y no cambió' }
    }

    $exeDir = Split-Path (Resolve-Exe) -Parent
    $hits = @()
    $terminos = @($Buscar | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
    if ($terminos.Count -gt 0) {
        Initialize-Native
        Add-Linea "-- Rastreo de: $($terminos -join ', ') (UTF-8 y UTF-16, en nombres y contenido; un término corto da falsos positivos en los binarios)"
        $lugares = [ordered]@{ 'carpeta portable' = $exeDir; 'LocalLow' = $LogDir }
        foreach ($l in $lugares.GetEnumerator()) { foreach ($x in [Oe4]::Grep($l.Value, [string[]]$terminos)) { $hits += "[$($l.Key)] $x" } }
        foreach ($k in ($dt.Nuevos + $dt.Cambiados)) {      # %TEMP%: solo lo que cambió, y nunca el scratchpad de Claude
            if ($k -ieq 'claude') { continue }
            foreach ($x in [Oe4]::Grep((Join-Path $env:TEMP $k), [string[]]$terminos)) { $hits += "[%TEMP%] $x" }
        }
        $kr = [Microsoft.Win32.Registry]::CurrentUser.OpenSubKey($RegSub)
        if ($kr) {
            foreach ($n in $kr.GetValueNames()) {
                $v = $kr.GetValue($n, $null, [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames)
                $txt = if ($v -is [byte[]]) { [Text.Encoding]::UTF8.GetString($v) + [Text.Encoding]::Unicode.GetString($v) } else { "$v" }
                foreach ($t in $terminos) { if ($n -like "*$t*" -or $txt.Contains($t)) { $hits += "[registro] $n  <- contiene «$t»" } }
            }
            $kr.Dispose()
        }
        if ($hits.Count -eq 0) { Add-Linea '  cero apariciones en la carpeta portable, LocalLow, lo nuevo de %TEMP% ni el registro.' }
        Add-Lista @($hits | ForEach-Object { "  APARECE: $_" }) 40
    }
    $null = New-Dir (Join-Path $Trabajo 'residuos')
    $informeF = Join-Path $Trabajo "residuos\$ses.txt"
    Write-Text $informeF ($informe -join "`n")
    Say "Informe completo: $informeF"
    if ($hits.Count -gt 0) { exit 1 }
}

function Do-RestaurarRegistro {
    Assert-JuegoCerrado
    $ses = Get-SesionArg -Obligatoria
    $f = Join-Path $Trabajo "residuos\previo-$ses.json"
    if (-not (Test-Path -LiteralPath $f)) { Stop-With 2 "No hay foto previa de '$ses' ($f): no sé cómo estaba la clave." }
    $a = (Get-Content -LiteralPath $f -Raw -Encoding utf8 | ConvertFrom-Json -AsHashtable -Depth 30).registro
    $hk = [Microsoft.Win32.Registry]::CurrentUser
    $padre = $RegSub.Substring(0, $RegSub.LastIndexOf('\'))
    if (-not $a.existe) {
        if ($hk.OpenSubKey($RegSub)) { $hk.DeleteSubKeyTree($RegSub, $false); Say "Clave borrada (no existía antes): HKCU\$RegSub" } else { Say "La clave HKCU\$RegSub no existe: nada que borrar." }
        $kp = $hk.OpenSubKey($padre)
        if ($kp) {
            $vacia = ($kp.SubKeyCount -eq 0 -and $kp.ValueCount -eq 0); $kp.Dispose()
            if (-not $a.empresaExiste -and $vacia) { $hk.DeleteSubKey($padre, $false); Say "Clave de la empresa borrada (no existía y quedó vacía): HKCU\$padre" }
        }
        return
    }
    $k = $hk.OpenSubKey($RegSub, $true)
    if (-not $k) { $k = $hk.CreateSubKey($RegSub) }
    $antes = @{}
    foreach ($v in $a.valores) { $antes[$v.nombre] = $v }
    foreach ($n in $k.GetValueNames()) { if (-not $antes.ContainsKey($n)) { $k.DeleteValue($n); Say "Valor nuevo borrado: $n" } }
    foreach ($v in $antes.Values) {
        $dato = switch ($v.tipo) { 'Binary' { [Convert]::FromBase64String($v.valor) } 'MultiString' { [string[]]@($v.valor) } 'DWord' { [int]$v.valor } 'QWord' { [long]$v.valor } default { [string]$v.valor } }
        $actual = $k.GetValue($v.nombre, $null, [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames)
        $igual = ($null -ne $actual) -and ((($actual | ConvertTo-Json -Compress) -eq ($dato | ConvertTo-Json -Compress)))
        if (-not $igual) { $k.SetValue($v.nombre, $dato, [Microsoft.Win32.RegistryValueKind]::$($v.tipo)); Say "Valor restaurado: $($v.nombre)" }
    }
    $k.Dispose()
    Say "La clave HKCU\$RegSub ya existía antes: se dejó con sus valores de la foto previa."
}

# --- Medidas ---------------------------------------------------------------------------------------
function Do-Tamano {
    $dir = if ($Carpeta) { $Carpeta } else { Split-Path (Resolve-Exe) -Parent }
    if (-not (Test-Path -LiteralPath $dir -PathType Container)) { Stop-With 2 "No existe la carpeta: $dir" }
    $dir = (Resolve-Path -LiteralPath $dir).Path.TrimEnd('\')
    $opt = [IO.EnumerationOptions]::new(); $opt.RecurseSubdirectories = $true; $opt.IgnoreInaccessible = $true; $opt.AttributesToSkip = [IO.FileAttributes]::ReparsePoint
    $porEntrada = @{}; $total = [long]0; $noship = [long]0; $datos = [long]0
    foreach ($fi in ([IO.DirectoryInfo]::new($dir)).EnumerateFiles('*', $opt)) {
        $rel = $fi.FullName.Substring($dir.Length + 1)
        $top = if ($rel.Contains('\')) { $rel.Substring(0, $rel.IndexOf('\')) } else { '(archivos de la raíz)' }
        if ($top -like '*_DoNotShip') { $noship += $fi.Length; continue }
        if ($top -ieq 'Datos') { $datos += $fi.Length; continue }
        $total += $fi.Length
        $porEntrada[$top] = [long]($porEntrada[$top]) + $fi.Length
    }
    Say "Carpeta medida: $dir"
    foreach ($k in ($porEntrada.Keys | Sort-Object { $porEntrada[$_] } -Descending)) { Say ("  {0,-46} {1,10:N1} MiB" -f $k, ($porEntrada[$k] / 1MB)) }
    Say ("TOTAL (sin *_DoNotShip ni Datos): {0:N1} MiB = {1:N1} MB decimales = {2:N0} bytes" -f ($total / 1MB), ($total / 1e6), $total)
    Say ("Aparte: *_DoNotShip {0:N1} MiB · Datos {1:N3} MiB · total con *_DoNotShip {2:N1} MiB" -f ($noship / 1MB), ($datos / 1MB), (($total + $noship) / 1MB))
    # Se juzga con megabytes decimales (500 000 000 B), que es la lectura más exigente de «500 MB».
    $ok = ($total / 1e6) -lt $PresupuestoPaqueteMB
    Say "RNF-06: $(if ($ok) { "paquete < $PresupuestoPaqueteMB MB: CUMPLE" } else { "paquete >= $PresupuestoPaqueteMB MB: NO CUMPLE" })"
    if (-not $ok) { exit 1 }
}

function Do-Medidas {
    $sel = if ($Rest.Count -gt 0 -and $Rest[0] -ne '-') { Safe-Name $Rest[0] } else { $null }
    $dir = Join-Path $Trabajo 'muestras'
    $mems = @(Get-ChildItem -LiteralPath $dir -Filter '*_mem.csv' -ErrorAction SilentlyContinue | Where-Object { -not $sel -or $_.Name -eq "${sel}_mem.csv" })
    if ($mems.Count -eq 0) { Stop-With 2 "No hay muestras de memoria en $dir$(if ($sel) { " para '$sel'" })." }
    $maxWs = $null; $maxPr = $null; $externas = 0
    foreach ($f in $mems) {
        $filas = @(Import-Csv -LiteralPath $f.FullName)
        if ($filas.Count -eq 0) { Say "$($f.Name): sin filas"; continue }
        $ws = $filas | Sort-Object { [long]$_.working_set_b } -Descending | Select-Object -First 1
        $pr = $filas | Sort-Object { [long]$_.private_b } -Descending | Select-Object -First 1
        $pw = ($filas | Measure-Object -Property peak_working_set_b -Maximum).Maximum
        $pp = ($filas | Measure-Object -Property peak_paged_b -Maximum).Maximum
        Say ("{0}: {1} muestras · WorkingSet máx {2:N0} MiB (escena «{3}») · Private máx {4:N0} MiB (escena «{5}») · picos del SO: WS {6:N0} MiB, commit {7:N0} MiB" -f
            $f.BaseName.Replace('_mem', ''), $filas.Count, ([long]$ws.working_set_b / 1MB), $ws.escena, ([long]$pr.private_b / 1MB), $pr.escena, ([long]$pw / 1MB), ([long]$pp / 1MB))
        if (-not $maxWs -or [long]$ws.working_set_b -gt [long]$maxWs.working_set_b) { $maxWs = $ws }
        if (-not $maxPr -or [long]$pr.private_b -gt [long]$maxPr.private_b) { $maxPr = $pr }
    }
    if ($maxWs) {
        $mejor = [math]::Max([long]$maxWs.working_set_b, [long]$maxPr.private_b)
        Say ("RNF-05: máximo {0:N0} MiB ({1:N2} GiB) frente a 2048 MiB: {2}" -f ($mejor / 1MB), ($mejor / 1GB), $(if ($mejor / 1MB -lt 2048) { 'CUMPLE' } else { 'NO CUMPLE' }))
    }
    foreach ($f in @(Get-ChildItem -LiteralPath $dir -Filter '*_red.csv' -ErrorAction SilentlyContinue | Where-Object { -not $sel -or $_.Name -eq "${sel}_red.csv" })) {
        $filas = @(Import-Csv -LiteralPath $f.FullName)
        $porClase = $filas | Group-Object clase | ForEach-Object { "$($_.Name)=$($_.Count)" }
        $ext = @($filas | Where-Object { $_.clase -eq 'EXTERNA' })
        $externas += $ext.Count
        Say "$($f.BaseName.Replace('_red', '')): red · $($filas.Count) filas ($($porClase -join ', '))"
        foreach ($x in ($ext | Select-Object -First 10)) { Say "  EXTERNA: $($x.proto) $($x.local) -> $($x.remoto) $($x.estado)" }
    }
    Say "RNF-10: $(if ($externas -eq 0) { 'cero conexiones a direcciones que no son de loopback (las UDP/TCP «escucha-local» no cuentan, pero se anotan)' } else { "$externas fila(s) EXTERNA(s): revisar" })"
    $cc = Join-Path $Trabajo 'cargas.csv'
    if (Test-Path -LiteralPath $cc) {
        $c = @(Import-Csv -LiteralPath $cc | Where-Object { -not $sel -or $_.sesion -eq $sel })
        Say "RNF-04: $($c.Count) cargas medidas (peor valor por escena; límite $PresupuestoCargaS s)"
        foreach ($g in ($c | Group-Object escena | Sort-Object Name)) {
            $peor = ($g.Group | ForEach-Object { [double]::Parse($_.segundos, $Inv) } | Measure-Object -Maximum).Maximum
            Say ("  {0,-18} n={1}  peor {2:0.000} s  {3}" -f $g.Name, $g.Count, $peor, $(if ($peor -lt $PresupuestoCargaS) { 'ok' } else { 'FUERA' }))
        }
    }
    if ($externas -gt 0) { exit 1 }
}

# --- El muestreador (proceso oculto; muere con el juego) --------------------------------------------------
function Do-Muestrear {
    if ($Rest.Count -lt 4) { Stop-With 2 'Muestrear es interno: lo lanza Start-Juego.' }
    $gp = [int]$Rest[0]; $ticks = [long]$Rest[1]; $ses = Safe-Name $Rest[2]; $lanz = [int]$Rest[3]
    Initialize-Native
    [Oe4]::KeepAwake($true)        # pantalla y sistema encendidos mientras dure el juego
    $dir = New-Dir (Join-Path $Trabajo 'muestras')
    $memF = Join-Path $dir "${ses}_mem.csv"; $redF = Join-Path $dir "${ses}_red.csv"; $regF = Join-Path $dir "${ses}_registro.tsv"
    $errF = Join-Path $dir "${ses}_muestreador.log"
    if (-not (Test-Path -LiteralPath $memF)) { Add-Text $memF "utc,t_s,lanz,pid,escena,working_set_b,private_b,peak_working_set_b,peak_paged_b`n" }
    if (-not (Test-Path -LiteralPath $redF)) { Add-Text $redF "utc,lanz,pid,proto,local,remoto,estado,clase`n" }
    $stop = Join-Path $Trabajo "muestreador-$PID.stop"
    Remove-Item -LiteralPath $stop -ErrorAction SilentlyContinue
    $reloj = [Diagnostics.Stopwatch]::StartNew(); $proximo = 0.0; $off = [long]0; $escena = ''
    try {
        while ($true) {
            $p = Get-Process -Id $gp -ErrorAction SilentlyContinue
            $vivo = $false
            if ($p) { try { $vivo = ($p.StartTime.ToUniversalTime().Ticks -eq $ticks) } catch { $vivo = $false } }
            # Cola del Player.log con hora: el log no la lleva y T0/T1 de las sesiones la necesitan (±0,25 s).
            try {
                $r = Read-LogChunk $off
                if ($r.Texto) {
                    $ahora = Iso ([DateTime]::UtcNow)
                    $buf = [Text.StringBuilder]::new()
                    foreach ($l in ($r.Texto -split "`n")) {
                        $l = $l.TrimEnd("`r"); if (-not $l) { continue }
                        $null = $buf.Append("$ahora`t$lanz`t$l`n")
                        $m = $LineaCarga.Match($l); if ($m.Success) { $escena = $m.Groups['e'].Value }
                    }
                    Add-Text $regF $buf.ToString()
                }
                $off = [long]$r.Offset
            }
            catch { Add-Text $errF "$(Iso ([DateTime]::UtcNow)) cola del log: $($_.Exception.Message)`n" }
            if (-not $vivo -or (Test-Path -LiteralPath $stop)) { break }
            if ($reloj.Elapsed.TotalSeconds -ge $proximo) {
                try {
                    $ahora = Iso ([DateTime]::UtcNow)
                    Add-Text $memF ("{0},{1},{2},{3},{4},{5},{6},{7},{8}`n" -f $ahora, $reloj.Elapsed.TotalSeconds.ToString('0.0', $Inv), $lanz, $gp, $escena, $p.WorkingSet64, $p.PrivateMemorySize64, $p.PeakWorkingSet64, $p.PeakPagedMemorySize64)
                    $filas = @(Get-Conexiones $gp)
                    if ($filas.Count -eq 0) { Add-Text $redF "$ahora,$lanz,$gp,NINGUNA,,,,`n" }
                    foreach ($c in $filas) { Add-Text $redF "$ahora,$lanz,$gp,$($c.proto),$($c.local),$($c.remoto),$($c.estado),$($c.clase)`n" }
                }
                catch { Add-Text $errF "$(Iso ([DateTime]::UtcNow)) muestra: $($_.Exception.Message)`n" }
                $proximo += 2
            }
            Start-Sleep -Milliseconds 250
        }
    }
    finally { [Oe4]::KeepAwake($false); Remove-Item -LiteralPath $stop -ErrorAction SilentlyContinue }
}

# --- Despacho ---------------------------------------------------------------------------------------
# Dos invocaciones que MANEJAN el juego a la vez se pisarían la entrada: la segunda se rechaza con salida 3.
$Exclusivos = 'start-juego', 'foto', 'clic', 'sostener', 'arrastrar', 'escribir', 'teclas', 'rafaga', 'preparar-datos', 'matar', 'cerrar-juego', 'restaurar-registro'
if ($Sub.ToLowerInvariant() -in $Exclusivos) {
    $script:Candado = [Threading.Mutex]::new($false, 'Local\Algoritmia.oe4.ps1')
    $got = $false
    try { $got = $script:Candado.WaitOne(0) } catch [Threading.AbandonedMutexException] { $got = $true }
    if (-not $got) { Stop-With 3 'Otra invocación de oe4.ps1 que maneja el juego sigue en curso: el juego es único y se maneja en serie. No se hizo nada.' }
}

try {
    switch ($Sub.ToLowerInvariant()) {
        { $_ -in 'help', '-h', '--help', '/?' } { Show-Help }
        'estado' { Do-Estado }
        'start-juego' { Do-StartJuego }
        'foto' { Do-Foto $(if ($Rest.Count -gt 0) { $Rest[0] } else { '' }) $Espera }
        'clic' { Do-Clic }
        'sostener' { Do-Sostener }
        'arrastrar' { Do-Arrastrar }
        'escribir' { Do-Escribir }
        'teclas' { Do-Teclas }
        { $_ -in 'esperar-carga', 'medir-carga' } { Do-EsperarCarga }
        'rafaga' { Do-Rafaga }
        'muestrear' { Do-Muestrear }
        'preparar-datos' { Do-PrepararDatos }
        'matar' { Do-Matar }
        'cerrar-juego' { Do-CerrarJuego }
        'revisar-log' { Do-RevisarLog }
        'residuos' { Do-Residuos }
        'restaurar-registro' { Do-RestaurarRegistro }
        'tamano' { Do-Tamano }
        'medidas' { Do-Medidas }
        default { Stop-With 2 "Subcomando desconocido '$Sub'. Use: help" }
    }
}
catch {
    $m = $_.Exception.Message
    if ($m -match 'SendInput') { $m += ' — la entrada se bloqueó: ¿pantalla bloqueada, otro escritorio activo o ventana del juego elevada?' }
    Stop-With 2 "ERROR: $m`n$($_.ScriptStackTrace)"
}
