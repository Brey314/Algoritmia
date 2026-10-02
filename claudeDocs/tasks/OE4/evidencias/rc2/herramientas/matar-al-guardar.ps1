#requires -Version 7.0
# Herramienta de la reverificación rc2 (no es código del juego). Vigila Datos\ y, en cuanto el juego empieza a
# escribir (evento del sistema de archivos que casa con -Filtro), lo mata con TerminateProcess: un cierre forzado
# en el instante del guardado (DEF-SPER-02, guardado atómico). -Filtro '*.tmp' cae a mitad de la escritura del
# temporal; '*.json' cae al reemplazar el perfil.
param([string]$Datos = 'C:\Dev\Algoritmia\Build\Algoritmia\Datos', [string]$Filtro = '*.tmp', [int]$MaxSeg = 300)
$ErrorActionPreference = 'Stop'
$w = [IO.FileSystemWatcher]::new($Datos, $Filtro)
$w.NotifyFilter = [IO.NotifyFilters]'FileName, LastWrite, Size, CreationTime'
$w.IncludeSubdirectories = $false
$procs = @(Get-Process -Name Algoritmia -ErrorAction SilentlyContinue)
if ($procs.Count -ne 1) { "ERROR: hay $($procs.Count) procesos Algoritmia"; exit 2 }
$p = $procs[0]
"VIGILANDO $Datos ($Filtro) pid $($p.Id) desde $([DateTime]::UtcNow.ToString('HH:mm:ss.fff'))Z"
$r = $w.WaitForChanged([IO.WatcherChangeTypes]::All, $MaxSeg * 1000)
if ($r.TimedOut) { "TIMEOUT sin escritura en $MaxSeg s"; exit 1 }
$p.Kill()
$t = [DateTime]::UtcNow.ToString('HH:mm:ss.fff')
$null = $p.WaitForExit(5000)
"MATADO ${t}Z tras $($r.ChangeType) de $($r.Name)"
Get-ChildItem -LiteralPath $Datos -Force | ForEach-Object { "  {0}  {1} B  {2:HH:mm:ss.fff}" -f $_.Name, $_.Length, $_.LastWriteTime }
