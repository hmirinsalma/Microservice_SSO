#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Lance tous les serveurs du projet ONEE SSO
    
.DESCRIPTION
    Ce script lance automatiquement :
    - Le serveur SSO (port 5205)
    - Les 3 applications clientes (frontend + backend)
    
.EXAMPLE
    .\START_ALL.ps1
#>

param(
    [switch]$SkipSSO,
    [switch]$SkipRH,
    [switch]$SkipTIMS,
    [switch]$SkipEAMS
)

Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║          🚀 ONEE SSO - Démarrage de tous les serveurs        ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

$rootPath = $PSScriptRoot
$jobs = @()

function Start-ServerInNewWindow {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Command,
        [string]$Port,
        [ConsoleColor]$Color = "White"
    )
    
    Write-Host "▶ Démarrage de $Name sur le port $Port..." -ForegroundColor $Color
    
    # Créer un script temporaire pour la nouvelle fenêtre
    $scriptContent = @"
`$Host.UI.RawUI.WindowTitle = "$Name - Port $Port"
Write-Host "═══════════════════════════════════════════════════" -ForegroundColor $Color
Write-Host "  $Name" -ForegroundColor $Color
Write-Host "  Port: $Port" -ForegroundColor $Color
Write-Host "═══════════════════════════════════════════════════" -ForegroundColor $Color
Write-Host ""
cd "$Path"
$Command
"@
    
    $tempScript = Join-Path $env:TEMP "onee_start_$([Guid]::NewGuid()).ps1"
    $scriptContent | Out-File -FilePath $tempScript -Encoding UTF8
    
    # Lancer dans une nouvelle fenêtre PowerShell
    Start-Process pwsh -ArgumentList "-NoExit", "-ExecutionPolicy", "Bypass", "-File", $tempScript
    
    Start-Sleep -Seconds 2
}

# 1. SSO Server
if (-not $SkipSSO) {
    Start-ServerInNewWindow `
        -Name "SSO Server" `
        -Path "$rootPath\src\ONEE.SSO.API" `
        -Command "dotnet run" `
        -Port "5205" `
        -Color "Cyan"
    
    Write-Host "  ✅ SSO Server lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5205" -ForegroundColor Gray
    Write-Host ""
    Start-Sleep -Seconds 5  # Attendre que le SSO démarre
}

# 2. Gestion Personnel (RH)
if (-not $SkipRH) {
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
    Write-Host "  GESTION PERSONNEL (RH)" -ForegroundColor Yellow
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
    
    # Backend RH
    Start-ServerInNewWindow `
        -Name "RH Backend" `
        -Path "$rootPath\clients\gestion-personnel\backend" `
        -Command "dotnet run" `
        -Port "5291" `
        -Color "Yellow"
    
    Write-Host "  ✅ RH Backend lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5291" -ForegroundColor Gray
    Write-Host ""
    
    # Frontend RH
    Start-ServerInNewWindow `
        -Name "RH Frontend" `
        -Path "$rootPath\clients\gestion-personnel\frontend" `
        -Command "npm run dev" `
        -Port "5173" `
        -Color "Yellow"
    
    Write-Host "  ✅ RH Frontend lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5173" -ForegroundColor Gray
    Write-Host ""
}

# 3. TIMS
if (-not $SkipTIMS) {
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Magenta
    Write-Host "  TIMS (Gestion des Interventions)" -ForegroundColor Magenta
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Magenta
    
    # Backend TIMS
    Start-ServerInNewWindow `
        -Name "TIMS Backend" `
        -Path "$rootPath\clients\tims\backend" `
        -Command "dotnet run" `
        -Port "5115" `
        -Color "Magenta"
    
    Write-Host "  ✅ TIMS Backend lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5115" -ForegroundColor Gray
    Write-Host ""
    
    # Frontend TIMS
    Start-ServerInNewWindow `
        -Name "TIMS Frontend" `
        -Path "$rootPath\clients\tims\frontend" `
        -Command "npm run dev" `
        -Port "5175" `
        -Color "Magenta"
    
    Write-Host "  ✅ TIMS Frontend lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5175" -ForegroundColor Gray
    Write-Host ""
}

# 4. EAMS
if (-not $SkipEAMS) {
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
    Write-Host "  EAMS (Gestion des Équipements)" -ForegroundColor Green
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
    
    # Backend EAMS
    Start-ServerInNewWindow `
        -Name "EAMS Backend" `
        -Path "$rootPath\clients\eams\backend" `
        -Command "dotnet run" `
        -Port "5137" `
        -Color "Green"
    
    Write-Host "  ✅ EAMS Backend lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5137" -ForegroundColor Gray
    Write-Host ""
    
    # Frontend EAMS
    Start-ServerInNewWindow `
        -Name "EAMS Frontend" `
        -Path "$rootPath\clients\eams\frontend" `
        -Command "npm run dev" `
        -Port "5173" `
        -Color "Green"
    
    Write-Host "  ✅ EAMS Frontend lancé" -ForegroundColor Green
    Write-Host "  🌐 http://localhost:5173" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                  ✅ TOUS LES SERVEURS SONT LANCÉS             ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 URLS des applications :" -ForegroundColor White
Write-Host "  • SSO:             http://localhost:5205" -ForegroundColor Cyan
Write-Host "  • RH (Frontend):   http://localhost:5173" -ForegroundColor Yellow
Write-Host "  • RH (Backend):    http://localhost:5291" -ForegroundColor Yellow
Write-Host "  • TIMS (Frontend): http://localhost:5175" -ForegroundColor Magenta
Write-Host "  • TIMS (Backend):  http://localhost:5115" -ForegroundColor Magenta
Write-Host "  • EAMS (Frontend): http://localhost:5173" -ForegroundColor Green
Write-Host "  • EAMS (Backend):  http://localhost:5137" -ForegroundColor Green
Write-Host ""
Write-Host "🔐 Identifiants de test :" -ForegroundColor White
Write-Host "  Email:    admin@onee.ma" -ForegroundColor Gray
Write-Host "  Password: Admin@123" -ForegroundColor Gray
Write-Host ""
Write-Host "⚠️  Pour arrêter tous les serveurs, fermez toutes les fenêtres PowerShell ouvertes." -ForegroundColor Yellow
Write-Host ""
Write-Host "Appuyez sur une touche pour fermer cette fenêtre..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
