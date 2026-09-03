# Script de demarrage de TOUS les serveurs SSO
# Usage: .\START_ALL.ps1

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  DEMARRAGE DE TOUS LES SERVEURS SSO" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

$BaseDir = "c:\Users\XPS\source\repos\ONEE.SSO"

# Fonction pour demarrer un serveur dans un nouveau terminal
function Start-Server {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Command,
        [string]$Port,
        [string]$Color = "Yellow"
    )
    
    Write-Host "  >> $Name (Port $Port)" -ForegroundColor $Color
    
    $FullPath = Join-Path $BaseDir $Path
    $Title = "$Name - Port $Port"
    
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "cd '$FullPath'; `$Host.UI.RawUI.WindowTitle = '$Title'; Write-Host '=======================================' -ForegroundColor Cyan; Write-Host ' >> $Name' -ForegroundColor Green; Write-Host ' Port: $Port' -ForegroundColor Yellow; Write-Host '=======================================' -ForegroundColor Cyan; Write-Host ''; $Command"
    )
    
    Start-Sleep -Milliseconds 800
}

# ===============================================================
# BACKENDS (Ordre important: SSO en premier)
# ===============================================================

Write-Host "BACKENDS" -ForegroundColor Magenta
Write-Host "---------------------------------------------" -ForegroundColor Gray

Start-Server -Name "SSO Backend" -Path "src\ONEE.SSO.API" -Command "dotnet run" -Port "5115" -Color "Green"
Start-Sleep -Seconds 3

Start-Server -Name "RH Backend" -Path "clients\gestion-personnel\backend" -Command "dotnet run" -Port "5291" -Color "Cyan"
Start-Server -Name "TIMS Backend" -Path "clients\tims\backend\TIMS.API" -Command "dotnet run" -Port "5178" -Color "Cyan"
Start-Server -Name "EAMS Backend" -Path "clients\eams\backend\ONEE.EAMS.API" -Command "dotnet run" -Port "5137" -Color "Cyan"

Write-Host ""
Write-Host "Attente du demarrage des backends (15 secondes)..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# ===============================================================
# FRONTENDS
# ===============================================================

Write-Host ""
Write-Host "FRONTENDS" -ForegroundColor Magenta
Write-Host "---------------------------------------------" -ForegroundColor Gray

Start-Server -Name "RH Frontend" -Path "clients\gestion-personnel\frontend" -Command "npm run dev" -Port "5174" -Color "Blue"
Start-Server -Name "TIMS Frontend" -Path "clients\tims\frontend" -Command "npm run dev" -Port "5175" -Color "Blue"
Start-Server -Name "EAMS Frontend" -Path "clients\eams\frontend" -Command "npm run dev" -Port "5173" -Color "Blue"

# ===============================================================
# RESUME
# ===============================================================

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  7 SERVEURS EN COURS DE DEMARRAGE" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "URLs d'acces:" -ForegroundColor White
Write-Host ""
Write-Host "  BACKENDS:" -ForegroundColor Yellow
Write-Host "    - SSO  : http://localhost:5115" -ForegroundColor Gray
Write-Host "    - RH   : http://localhost:5291" -ForegroundColor Gray
Write-Host "    - TIMS : http://localhost:5178" -ForegroundColor Gray
Write-Host "    - EAMS : http://localhost:5137" -ForegroundColor Gray
Write-Host ""
Write-Host "  FRONTENDS:" -ForegroundColor Yellow
Write-Host "    - RH   : http://localhost:5174  <-- Ouvrir cette URL" -ForegroundColor Green
Write-Host "    - TIMS : http://localhost:5175" -ForegroundColor Gray
Write-Host "    - EAMS : http://localhost:5173" -ForegroundColor Gray
Write-Host ""

Write-Host "Attendre 30-60 secondes que tous les serveurs demarrent..." -ForegroundColor Yellow
Write-Host ""

Write-Host "Identifiants de test:" -ForegroundColor White
Write-Host "    Email: admin@onee.ma" -ForegroundColor Gray
Write-Host "    Mot de passe: Admin@123" -ForegroundColor Gray
Write-Host ""

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Appuyer sur ENTREE pour fermer ce terminal" -ForegroundColor DarkGray
Write-Host "================================================" -ForegroundColor Cyan

Read-Host
