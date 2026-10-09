<#
.SYNOPSIS
  Mide las paginas de un .docx por Word COM (solo lectura): total y pagina inicial de cada Titulo 1 y Titulo 2.
  Uso: pwsh -NoProfile -File medir_paginas.ps1 -Docx docs/Trabajo_de_Grado_Entrega_Plantilla_28jul.docx
#>
param([Parameter(Mandatory=$true)][string]$Docx)
$ErrorActionPreference = 'Stop'
$path = (Resolve-Path -LiteralPath $Docx).Path
$word = New-Object -ComObject Word.Application
$word.Visible = $false; $word.DisplayAlerts = 0
try {
  $doc = $word.Documents.Open($path, $false, $true, $false)
  $doc.Repaginate()
  $total = $doc.ComputeStatistics(2)
  Write-Host "TOTAL paginas: $total"
  $rows = @()
  foreach ($p in $doc.Paragraphs) {
    $s = [string]$p.Style.NameLocal
    if ($s -eq 'Título 1' -or $s -eq 'Título 2') {
      $t = ([string]$p.Range.Text).Trim()
      if ($t.Length -gt 0) { $rows += [pscustomobject]@{ Nivel = $s; Pagina = [int]$p.Range.Information(3); Texto = $t.Substring(0, [Math]::Min(70, $t.Length)) } }
    }
  }
  $rows | ForEach-Object { "{0,-9} p.{1,3}  {2}" -f $_.Nivel, $_.Pagina, $_.Texto }
  $doc.Close($false)
} finally { $word.Quit($false); [GC]::Collect() }
