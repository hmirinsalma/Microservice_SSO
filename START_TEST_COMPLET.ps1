# Script de démarrage pour tester le flow SSO complet
# ONEE SSO - Test des 3 Applications

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  DÉMARRAGE TEST SSO ONEE COMPLET" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Vérifier que nous sommes dans le bon dossier
$rootPath = "c:\Users\XPS\source\repos\ONEE.SSO"
if (-not (Test-Path $rootPath)) {
    Write-Host "❌ Erreur: Dossier du projet non trouvé: $rootPath" -ForegroundColor Red
    exit 1
}

Set-Location $rootPath
Write-Host "📁 Dossier racine: $rootPath" -ForegroundColor Green
Write-Host ""

# 1. Démarrer le SSO
Write-Host "🚀 Démarrage du SSO (Port 5205)..." -ForegroundColor Yellow
$ssoPath = Join-Path $rootPath "src\ONEE.SSO.API"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$ssoPath'; Write-Host '🔐 SSO Backend - Port 5205' -ForegroundColor Cyan; dotnet run"
Start-Sleep -Seconds 2

# 2. Démarrer Backend Gestion Personnel
Write-Host "🚀 Démarrage Backend Gestion Personnel (Port 5291)..." -ForegroundColor Yellow
$rhBackendPath = Join-Path $rootPath "clients\gestion-personnel\backend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$rhBackendPath'; Write-Host '👥 Backend RH - Port 5291' -ForegroundColor Green; dotnet run"
Start-Sleep -Seconds 2

# 3. Démarrer Frontend Gestion Personnel
Write-Host "🚀 Démarrage Frontend Gestion Personnel (Port 5173)..." -ForegroundColor Yellow
$rhFrontendPath = Join-Path $rootPath "clients\gestion-personnel\frontend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$rhFrontendPath'; Write-Host '🌐 Frontend RH - Port 5173' -ForegroundColor Blue; npm run dev"
Start-Sleep -Seconds 2

# 4. Démarrer Backend TIMS (Optionnel)
Write-Host "🚀 Démarrage Backend TIMS (Port 5115)..." -ForegroundColor Yellow
$timsBackendPath = Join-Path $rootPath "clients\tims\backend"
if (Test-Path $timsBackendPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$timsBackendPath'; Write-Host '⏱️ Backend TIMS - Port 5115' -ForegroundColor Magenta; dotnet run"
    Start-Sleep -Seconds 2
}

# 5. Démarrer Frontend TIMS (Optionnel)
Write-Host "🚀 Démarrage Frontend TIMS (Port 5175)..." -ForegroundColor Yellow
$timsFrontendPath = Join-Path $rootPath "clients\tims\frontend"
if (Test-Path $timsFrontendPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$timsFrontendPath'; Write-Host '🌐 Frontend TIMS - Port 5175' -ForegroundColor DarkBlue; npm run dev"
    Start-Sleep -Seconds 2
}

# 6. Démarrer Backend EAMS (Optionnel)
Write-Host "🚀 Démarrage Backend EAMS (Port 5137)..." -ForegroundColor Yellow
$eamsBackendPath = Join-Path $rootPath "clients\eams\backend\ONEE.EAMS.API"
if (Test-Path $eamsBackendPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$eamsBackendPath'; Write-Host '🔧 Backend EAMS - Port 5137' -ForegroundColor DarkGreen; dotnet run"
    Start-Sleep -Seconds 2
}

# 7. Démarrer Frontend EAMS (Optionnel)
Write-Host "🚀 Démarrage Frontend EAMS (Port 5174)..." -ForegroundColor Yellow
$eamsFrontendPath = Join-Path $rootPath "clients\eams\frontend"
if (Test-Path $eamsFrontendPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$eamsFrontendPath'; Write-Host '🌐 Frontend EAMS - Port 5174' -ForegroundColor DarkYellow; npm run dev"
    Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✅ TOUS LES SERVICES DÉMARRÉS" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "📋 URLS D'ACCÈS:" -ForegroundColor Cyan
Write-Host "  • SSO Admin:             http://localhost:5205/Dashboard" -ForegroundColor White
Write-Host "  • Gestion Personnel:     http://localhost:5173" -ForegroundColor White
Write-Host "  • TIMS:                  http://localhost:5175" -ForegroundColor White
Write-Host "  • EAMS:                  http://localhost:5174" -ForegroundColor White
Write-Host ""

Write-Host "🔐 IDENTIFIANTS DE TEST:" -ForegroundColor Cyan
Write-Host "  Email:    admin@onee.ma" -ForegroundColor White
Write-Host "  Password: Admin@123" -ForegroundColor White
Write-Host ""

Write-Host "📝 ÉTAPES DE TEST:" -ForegroundColor Cyan
Write-Host "  1. Attendre ~20 secondes que tous les services démarrent" -ForegroundColor Yellow
Write-Host "  2. Ouvrir http://localhost:5173" -ForegroundColor Yellow
Write-Host "  3. Cliquer 'Se connecter avec SSO'" -ForegroundColor Yellow
Write-Host "  4. Login avec admin@onee.ma / Admin@123" -ForegroundColor Yellow
Write-Host "  5. Autoriser l'accès" -ForegroundColor Yellow
Write-Host "  6. ✅ Vérifier que le dashboard RH reste affiché" -ForegroundColor Yellow
Write-Host ""

Write-Host "⚠️  ATTENTION:" -ForegroundColor Red
Write-Host "  - Vérifier qu'il n'y a pas d'erreur IDX10517 dans la console backend RH" -ForegroundColor Yellow
Write-Host "  - Le dashboard RH ne doit PAS retourner au login automatiquement" -ForegroundColor Yellow
Write-Host ""

Write-Host "⏱️  Attendre 20 secondes avant de tester..." -ForegroundColor Magenta
Write-Host ""

# Ouvrir automatiquement le navigateur après 20 secondes
Start-Sleep -Seconds 10
Write-Host "⏱️  10 secondes..." -ForegroundColor Magenta
Start-Sleep -Seconds 5
Write-Host "⏱️  5 secondes..." -ForegroundColor Magenta
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "🌐 Ouverture du navigateur..." -ForegroundColor Green
Start-Process "http://localhost:5173"

Write-Host ""
Write-Host "✅ Prêt pour les tests!" -ForegroundColor Green
Write-Host ""
Write-Host "Appuyer sur une touche pour fermer cette fenêtre..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
