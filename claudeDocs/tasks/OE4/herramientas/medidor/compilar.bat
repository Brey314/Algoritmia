@echo off
rem Compila MedirAlgoritmia.exe con el csc de .NET Framework 4 que trae Windows (no instala nada).
rem La salida va a Build\MedidorAlgoritmia\ (ignorada por git).
set OUT=%~dp0..\..\..\..\..\Build\MedidorAlgoritmia
if not exist "%OUT%" mkdir "%OUT%"
"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /codepage:65001 /optimize+ /target:exe /r:System.Management.dll /out:"%OUT%\MedirAlgoritmia.exe" "%~dp0MedirAlgoritmia.cs"
