# Script pour ARRETER tous les serveurs SSO
# Usage: .\STOP_ALL.ps1

Write-Host "================================================" -ForegroundColor Red
Write-Host "  ARRET DE TOUS LES SERVEURS SSO" -ForegroundColor Yellow
Write-Host "================================================" -ForegroundColor Red
Write-Host ""

# Arreter tous les processus dotnet (backends)
Write-Host "Arret des backends (.NET)..." -ForegroundColor Yellow
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($dotnetProcesses) {
    $dotnetProcesses | Stop-Process -Force
    Write-Host "  OK $($dotnetProcesses.Count) processus dotnet arretes" -ForegroundColor Green
} else {
    Write-Host "  INFO Aucun processus dotnet en cours" -ForegroundColor Gray
}

# Arreter tous les processus node (frontends)
Write-Host ""
Write-Host "Arret des frontends (Node.js)..." -ForegroundColor Yellow
$nodeProcesses = Get-Process -Name "node" -ErrorAction SilentlyContinue
if ($nodeProcesses) {
    $nodeProcesses | Stop-Process -Force
    Write-Host "  OK $($nodeProcesses.Count) processus node arretes" -ForegroundColor Green
} else {
    Write-Host "  INFO Aucun processus node en cours" -ForegroundColor Gray
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  TOUS LES SERVEURS ONT ETE ARRETES" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""

Start-Sleep -Seconds 2
