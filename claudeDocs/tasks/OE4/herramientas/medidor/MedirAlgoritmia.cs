// Medidor de RNF-04 (carga de escena < 10 s) y RNF-05 (memoria < 2 GB) para los equipos del colegio.
// Lanza Algoritmia.exe, muestrea su memoria cada 250 ms mientras se juega y, cuando el juego se cierra,
// lee del Player.log las líneas «RNF-04: «escena» cargó en N s» que escribe SceneLoader.
// No toca la red ni la carpeta Datos del juego: solo escribe su informe junto a este .exe.
// Se compila con el csc de .NET Framework 4 que trae Windows (ver compilar.bat); C# 5, sin dependencias.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

static class MedirAlgoritmia
{
    const double PresupuestoCargaS = 10.0;   // RNF-04
    const double PresupuestoMemoriaMiB = 2048; // RNF-05
    static readonly Regex LineaCarga = new Regex(@"RNF-04:\s*«(?<e>[^»]+)»\s*cargó en\s*(?<n>[0-9]+(?:[.,][0-9]+)?)\s*s");

    static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        string aqui = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine("Medidor de Algoritmia: tiempos de carga (RNF-04) y memoria (RNF-05)");
        Console.WriteLine();

        string exe = BuscarJuego(args, aqui);
        if (exe == null)
        {
            Console.Write("No encontré Algoritmia.exe. Escriba la ruta de la carpeta del juego y pulse Intro: ");
            string carpeta = (Console.ReadLine() ?? "").Trim().Trim('"');
            exe = Path.Combine(carpeta, "Algoritmia.exe");
            if (!File.Exists(exe)) { Console.WriteLine("Tampoco está ahí. Cierre esta ventana."); Esperar(); return 2; }
        }

        string log = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            @"AppData\LocalLow\Universidad Catolica de Colombia\Algoritmia\Player.log");
        Dictionary<string, string> equipo = DatosDelEquipo();

        Console.WriteLine("Juego: " + exe);
        Console.WriteLine("Equipo: " + equipo["cpu"] + " · " + equipo["ram"] + " · " + equipo["gpu"]);
        Console.WriteLine();
        Console.WriteLine("Se abrirá el juego. Juéguelo con normalidad, pasando por varias escenas (idealmente los tres niveles).");
        Console.WriteLine("Cuando termine, cierre el juego («Guardar y salir» o la X). No cierre esta ventana negra.");
        Console.WriteLine();

        DateTime inicio = DateTime.Now;
        Process juego;
        try { juego = Process.Start(new ProcessStartInfo(exe) { WorkingDirectory = Path.GetDirectoryName(exe), UseShellExecute = true }); }
        catch (Exception e) { Console.WriteLine("No se pudo abrir el juego: " + e.Message); Esperar(); return 2; }

        long maxWs = 0, maxPriv = 0, maxPeakWs = 0; int muestras = 0;
        while (!juego.HasExited)
        {
            try
            {
                juego.Refresh();
                maxWs = Math.Max(maxWs, juego.WorkingSet64);
                maxPriv = Math.Max(maxPriv, juego.PrivateMemorySize64);
                maxPeakWs = Math.Max(maxPeakWs, juego.PeakWorkingSet64);
                muestras++;
                if (muestras % 40 == 0)
                    Console.Write("\r  Jugando: {0:0} min · memoria máxima hasta ahora {1:N0} MiB   ", (DateTime.Now - inicio).TotalMinutes, Math.Max(maxWs, maxPriv) / 1048576.0);
            }
            catch (InvalidOperationException) { break; }
            Thread.Sleep(250);
        }
        juego.WaitForExit();
        int codigo = juego.ExitCode;
        TimeSpan duracion = DateTime.Now - inicio;
        Console.WriteLine();
        Thread.Sleep(1000); // el reproductor termina de escribir el log

        // Cargas: peor valor por escena; Exception en el log
        var cargas = new SortedDictionary<string, List<double>>();
        int excepciones = 0;
        bool hayLog = File.Exists(log) && File.GetLastWriteTime(log) >= inicio.AddSeconds(-5);
        if (hayLog)
        {
            foreach (string linea in File.ReadAllLines(log, Encoding.UTF8))
            {
                Match m = LineaCarga.Match(linea);
                if (m.Success)
                {
                    double n = double.Parse(m.Groups["n"].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                    string escena = m.Groups["e"].Value;
                    if (!cargas.ContainsKey(escena)) cargas[escena] = new List<double>();
                    cargas[escena].Add(n);
                }
                if (linea.Contains("Exception")) excepciones++;
            }
        }

        double peorCarga = cargas.Count == 0 ? double.NaN : cargas.Values.SelectMany(v => v).Max();
        double memMiB = Math.Max(maxWs, maxPriv) / 1048576.0;
        bool cumple04 = cargas.Count > 0 && peorCarga < PresupuestoCargaS;
        bool cumple05 = muestras > 0 && memMiB < PresupuestoMemoriaMiB;

        var inf = new StringBuilder();
        inf.AppendLine("MEDICIÓN DE ALGORITMIA · " + inicio.ToString("yyyy-MM-dd HH:mm:ss"));
        inf.AppendLine("Equipo: " + equipo["nombre"]);
        inf.AppendLine("Procesador: " + equipo["cpu"]);
        inf.AppendLine("Memoria RAM: " + equipo["ram"]);
        inf.AppendLine("Gráficos: " + equipo["gpu"]);
        inf.AppendLine("Sistema: " + equipo["so"]);
        inf.AppendLine("Juego: " + exe);
        inf.AppendLine(string.Format("Tiempo jugado: {0:0.0} min · código de salida {1} · {2} muestras de memoria", duracion.TotalMinutes, codigo, muestras));
        inf.AppendLine();
        inf.AppendLine("RNF-04, carga de escena (límite 10 s), peor valor por escena:");
        if (cargas.Count == 0) inf.AppendLine("  No se encontraron cargas en el Player.log (" + (hayLog ? "el log no trae líneas RNF-04" : "no hay log nuevo en " + log) + ").");
        foreach (var c in cargas) inf.AppendLine(string.Format(CultureInfo.InvariantCulture, "  {0,-18} {1,7:0.000} s  ({2} carga(s))", c.Key, c.Value.Max(), c.Value.Count));
        inf.AppendLine(string.Format(CultureInfo.InvariantCulture, "  Peor carga: {0:0.000} s -> {1}", peorCarga, cumple04 ? "CUMPLE" : "NO CUMPLE / SIN DATOS"));
        inf.AppendLine();
        inf.AppendLine("RNF-05, memoria (límite 2 GB = 2048 MiB):");
        inf.AppendLine(string.Format("  Working set máximo {0:N0} MiB · memoria privada máxima {1:N0} MiB · pico del sistema {2:N0} MiB", maxWs / 1048576.0, maxPriv / 1048576.0, maxPeakWs / 1048576.0));
        inf.AppendLine(string.Format("  Máximo considerado: {0:N0} MiB -> {1}", memMiB, cumple05 ? "CUMPLE" : "NO CUMPLE / SIN DATOS"));
        inf.AppendLine();
        inf.AppendLine("Líneas con «Exception» en el Player.log: " + excepciones);

        string sello = inicio.ToString("yyyyMMdd-HHmmss");
        string informe = Path.Combine(aqui, "Resultado_" + Limpio(equipo["nombre"]) + "_" + sello + ".txt");
        File.WriteAllText(informe, inf.ToString(), new UTF8Encoding(true));
        if (hayLog) { try { File.Copy(log, Path.Combine(aqui, "Player_" + Limpio(equipo["nombre"]) + "_" + sello + ".log"), true); } catch (IOException) { } }

        string csv = Path.Combine(aqui, "resultados_medicion.csv");
        bool nuevo = !File.Exists(csv);
        using (var w = new StreamWriter(csv, true, new UTF8Encoding(true)))
        {
            if (nuevo) w.WriteLine("fecha;equipo;procesador;ram;graficos;minutos;peor_carga_s;escenas_medidas;memoria_max_mib;rnf04;rnf05;excepciones");
            w.WriteLine(string.Join(";", new[] {
                inicio.ToString("yyyy-MM-dd HH:mm"), equipo["nombre"], equipo["cpu"], equipo["ram"], equipo["gpu"],
                duracion.TotalMinutes.ToString("0.0", CultureInfo.InvariantCulture),
                double.IsNaN(peorCarga) ? "" : peorCarga.ToString("0.000", CultureInfo.InvariantCulture), cargas.Count.ToString(),
                memMiB.ToString("0", CultureInfo.InvariantCulture), cumple04 ? "CUMPLE" : "NO", cumple05 ? "CUMPLE" : "NO", excepciones.ToString() }));
        }

        Console.WriteLine(inf.ToString());
        Console.WriteLine("Informe guardado en: " + informe);
        Console.WriteLine("Fila añadida a: " + csv);
        Esperar();
        return cumple04 && cumple05 ? 0 : 1;
    }

    static string BuscarJuego(string[] args, string aqui)
    {
        var candidatos = new List<string>();
        if (args.Length > 0) candidatos.Add(args[0].EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? args[0] : Path.Combine(args[0], "Algoritmia.exe"));
        candidatos.Add(Path.Combine(aqui, "Algoritmia.exe"));
        candidatos.Add(Path.Combine(aqui, @"Algoritmia\Algoritmia.exe"));
        candidatos.Add(Path.Combine(aqui, @"..\Algoritmia.exe"));
        candidatos.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"Algoritmia\Algoritmia.exe"));
        foreach (string c in candidatos) if (File.Exists(c)) return Path.GetFullPath(c);
        return null;
    }

    static Dictionary<string, string> DatosDelEquipo()
    {
        var d = new Dictionary<string, string>();
        d["nombre"] = Environment.MachineName;
        d["cpu"] = Wmi("SELECT Name FROM Win32_Processor", o => ((string)o["Name"] ?? "").Trim());
        d["ram"] = Wmi("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem", o => string.Format("{0:0.0} GB", Convert.ToDouble(o["TotalPhysicalMemory"]) / 1073741824.0));
        d["gpu"] = Wmi("SELECT Name, AdapterRAM FROM Win32_VideoController", o => (string)o["Name"] + (o["AdapterRAM"] != null ? string.Format(" ({0:N0} MB)", Convert.ToDouble(o["AdapterRAM"]) / 1048576.0) : ""));
        d["so"] = Wmi("SELECT Caption, Version FROM Win32_OperatingSystem", o => (string)o["Caption"] + " " + (string)o["Version"]);
        return d;
    }

    static string Wmi(string consulta, Func<ManagementBaseObject, string> f)
    {
        try
        {
            using (var s = new ManagementObjectSearcher(consulta))
                return string.Join(" + ", s.Get().Cast<ManagementBaseObject>().Select(f).Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }
        catch (Exception) { return "desconocido"; }
    }

    static string Limpio(string s) { return Regex.Replace(s, @"[^\w-]", "_"); }

    static void Esperar() { Console.WriteLine(); Console.WriteLine("Pulse Intro para cerrar."); Console.ReadLine(); }
}
