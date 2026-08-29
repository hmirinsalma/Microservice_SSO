# Script de démarrage rapide pour tester SSO + Gestion Personnel
# ONEE SSO - Test Application RH uniquement

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TEST RAPIDE SSO + GESTION PERSONNEL" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$rootPath = "c:\Users\XPS\source\repos\ONEE.SSO"
Set-Location $rootPath

# 1. Démarrer le SSO
Write-Host "🚀 Démarrage du SSO (Port 5205)..." -ForegroundColor Yellow
$ssoPath = Join-Path $rootPath "src\ONEE.SSO.API"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$ssoPath'; Write-Host '🔐 SSO Backend - Port 5205' -ForegroundColor Cyan; Write-Host ''; dotnet run"
Start-Sleep -Seconds 3

# 2. Démarrer Backend Gestion Personnel
Write-Host "🚀 Démarrage Backend Gestion Personnel (Port 5291)..." -ForegroundColor Yellow
$rhBackendPath = Join-Path $rootPath "clients\gestion-personnel\backend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$rhBackendPath'; Write-Host '👥 Backend RH - Port 5291' -ForegroundColor Green; Write-Host ''; dotnet run"
Start-Sleep -Seconds 3

# 3. Démarrer Frontend Gestion Personnel
Write-Host "🚀 Démarrage Frontend Gestion Personnel (Port 5173)..." -ForegroundColor Yellow
$rhFrontendPath = Join-Path $rootPath "clients\gestion-personnel\frontend"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$rhFrontendPath'; Write-Host '🌐 Frontend RH - Port 5173' -ForegroundColor Blue; Write-Host ''; npm run dev"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✅ SERVICES DÉMARRÉS" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "📋 URLS:" -ForegroundColor Cyan
Write-Host "  • SSO Admin:         http://localhost:5205/Dashboard" -ForegroundColor White
Write-Host "  • Gestion Personnel: http://localhost:5173" -ForegroundColor White
Write-Host ""

Write-Host "🔐 IDENTIFIANTS:" -ForegroundColor Cyan
Write-Host "  Email:    admin@onee.ma" -ForegroundColor White
Write-Host "  Password: Admin@123" -ForegroundColor White
Write-Host ""

Write-Host "📝 TEST RAPIDE:" -ForegroundColor Cyan
Write-Host "  1. Attendre 15 secondes" -ForegroundColor Yellow
Write-Host "  2. Ouvrir http://localhost:5173" -ForegroundColor Yellow
Write-Host "  3. Se connecter avec SSO" -ForegroundColor Yellow
Write-Host "  4. Vérifier que le dashboard reste affiché" -ForegroundColor Yellow
Write-Host ""

Write-Host "⏱️  Attendre 15 secondes..." -ForegroundColor Magenta
Start-Sleep -Seconds 15

Write-Host "🌐 Ouverture du navigateur..." -ForegroundColor Green
Start-Process "http://localhost:5173"

Write-Host ""
Write-Host "✅ Prêt!" -ForegroundColor Green
Write-Host ""
Write-Host "Appuyer sur une touche pour fermer..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
