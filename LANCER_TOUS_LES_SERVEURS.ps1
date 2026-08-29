# 🚀 SCRIPT DE LANCEMENT COMPLET - SSO + 3 APPLICATIONS
# Ce script lance automatiquement les 7 serveurs nécessaires

Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "🚀 LANCEMENT DU SYSTÈME SSO ONEE COMPLET" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Fonction pour démarrer un processus dans une nouvelle fenêtre PowerShell
function Start-ServerWindow {
    param(
        [string]$Title,
        [string]$Path,
        [string]$Command,
        [string]$Color
    )
    
    Write-Host "🔄 Démarrage : $Title" -ForegroundColor $Color
    
    $encodedCommand = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes(@"
`$Host.UI.RawUI.WindowTitle = '$Title'
Write-Host '═══════════════════════════════════════════════════════════' -ForegroundColor $Color
Write-Host '$Title' -ForegroundColor $Color
Write-Host '═══════════════════════════════════════════════════════════' -ForegroundColor $Color
Write-Host ''
Set-Location '$Path'
$Command
"@))
    
    Start-Process powershell -ArgumentList "-NoExit", "-EncodedCommand", $encodedCommand
    Start-Sleep -Seconds 2
}

# ═══════════════════════════════════════════════════════════
# 1️⃣ SERVEUR SSO (PORT 5205)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "🔐 SERVEUR SSO (Port 5205)" `
    -Path "c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API" `
    -Command "dotnet run" `
    -Color "Green"

Write-Host "⏳ Attente 8 secondes pour le démarrage du SSO..." -ForegroundColor Yellow
Start-Sleep -Seconds 8

# ═══════════════════════════════════════════════════════════
# 2️⃣ BACKEND GESTION PERSONNEL (PORT 5291)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "📊 BACKEND GESTION PERSONNEL (Port 5291)" `
    -Path "c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\GestionPersonnel.API" `
    -Command "dotnet run" `
    -Color "Blue"

Write-Host "⏳ Attente 5 secondes..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# ═══════════════════════════════════════════════════════════
# 3️⃣ BACKEND TIMS (PORT 5115)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "🔧 BACKEND TIMS (Port 5115)" `
    -Path "c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API" `
    -Command "dotnet run" `
    -Color "Magenta"

Write-Host "⏳ Attente 5 secondes..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# ═══════════════════════════════════════════════════════════
# 4️⃣ BACKEND EAMS (PORT 5137)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "⚙️ BACKEND EAMS (Port 5137)" `
    -Path "c:\Users\XPS\Desktop\gestion des equipements\backend\ONEE.EAMS.API" `
    -Command "dotnet run" `
    -Color "Cyan"

Write-Host "⏳ Attente 5 secondes..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# ═══════════════════════════════════════════════════════════
# 5️⃣ FRONTEND GESTION PERSONNEL (PORT 5173)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "🖥️ FRONTEND GESTION PERSONNEL (Port 5173)" `
    -Path "c:\Users\XPS\Desktop\Gestion du Prsonnel\frontend" `
    -Command "npm run dev" `
    -Color "Yellow"

Write-Host "⏳ Attente 5 secondes..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# ═══════════════════════════════════════════════════════════
# 6️⃣ FRONTEND TIMS (PORT 5173 - Vite choisira un autre port automatiquement)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "🖥️ FRONTEND TIMS (Port auto)" `
    -Path "c:\Users\XPS\Desktop\gestion des interventions\frontend" `
    -Command "npm run dev" `
    -Color "Yellow"

Write-Host "⏳ Attente 5 secondes..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# ═══════════════════════════════════════════════════════════
# 7️⃣ FRONTEND EAMS (PORT 5173 - Vite choisira un autre port automatiquement)
# ═══════════════════════════════════════════════════════════
Start-ServerWindow `
    -Title "🖥️ FRONTEND EAMS (Port auto)" `
    -Path "c:\Users\XPS\Desktop\gestion des equipements\frontend" `
    -Command "npm run dev" `
    -Color "Yellow"

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "✅ TOUS LES SERVEURS SONT EN COURS DE DÉMARRAGE" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Host "📋 SERVEURS LANCÉS :" -ForegroundColor Cyan
Write-Host "   🔐 SSO              : http://localhost:5205" -ForegroundColor Green
Write-Host "   📊 Backend RH       : http://localhost:5291" -ForegroundColor Blue
Write-Host "   🔧 Backend TIMS     : http://localhost:5115" -ForegroundColor Magenta
Write-Host "   ⚙️  Backend EAMS     : http://localhost:5137" -ForegroundColor Cyan
Write-Host "   🖥️  Frontend RH      : http://localhost:5173" -ForegroundColor Yellow
Write-Host "   🖥️  Frontend TIMS    : (Port auto - voir fenêtre)" -ForegroundColor Yellow
Write-Host "   🖥️  Frontend EAMS    : (Port auto - voir fenêtre)" -ForegroundColor Yellow
Write-Host ""
Write-Host "⏳ Attendre ~30 secondes que tous les serveurs démarrent..." -ForegroundColor Yellow
Write-Host ""
Write-Host "📖 POUR TESTER :" -ForegroundColor Cyan
Write-Host "   1. Ouvrir le fichier : GUIDE_TESTS_E2E.md" -ForegroundColor White
Write-Host "   2. Suivre les instructions étape par étape" -ForegroundColor White
Write-Host ""
Write-Host "🔑 IDENTIFIANTS DE TEST :" -ForegroundColor Cyan
Write-Host "   Email    : admin@onee.ma" -ForegroundColor White
Write-Host "   Password : Admin@123" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  POUR ARRÊTER TOUS LES SERVEURS :" -ForegroundColor Red
Write-Host "   Fermer toutes les fenêtres PowerShell ouvertes" -ForegroundColor White
Write-Host ""

# Ouvrir le guide de tests dans le navigateur par défaut
Write-Host "📖 Ouverture du guide de tests..." -ForegroundColor Cyan
Start-Sleep -Seconds 2

# Garder cette fenêtre ouverte
Write-Host "Script termine. Les serveurs sont en cours d'execution." -ForegroundColor Green
Write-Host "Appuyez sur une touche pour fermer cette fenetre..." -ForegroundColor Gray
Read-Host
