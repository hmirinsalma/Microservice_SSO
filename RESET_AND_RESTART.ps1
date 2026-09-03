# ========================================
# SCRIPT COMPLET: RESET + UPDATE + RESTART
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESET COMPLET ET REDEMARRAGE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Etape 1: Arreter tous les services
Write-Host "Etape 1/5: Arret des services..." -ForegroundColor Yellow
& .\STOP_ALL.ps1
Start-Sleep -Seconds 2

# Etape 2: Supprimer la base SSO
Write-Host ""
Write-Host "Etape 2/5: Suppression de la base SSO..." -ForegroundColor Yellow
$ssoDbPath = ".\src\ONEE.SSO.API\onee_sso.db"
if (Test-Path $ssoDbPath) {
    Remove-Item $ssoDbPath -Force
    Write-Host "  Base SSO supprimee" -ForegroundColor Green
} else {
    Write-Host "  Base SSO inexistante (sera creee)" -ForegroundColor Gray
}

# Etape 3: Mettre a jour le SsoId de Mohamed
Write-Host ""
Write-Host "Etape 3/5: Mise a jour du SsoId..." -ForegroundColor Yellow
& .\UpdateMohamedSsoId.ps1

# Etape 4: Demarrer tous les services
Write-Host ""
Write-Host "Etape 4/5: Demarrage des services..." -ForegroundColor Yellow
Write-Host "  (Cela va prendre environ 30 secondes)" -ForegroundColor Gray
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; .\START_ALL.ps1"

# Etape 5: Attendre que les services soient prets
Write-Host ""
Write-Host "Etape 5/5: Attente du demarrage..." -ForegroundColor Yellow
$seconds = 30
for ($i = $seconds; $i -gt 0; $i--) {
    Write-Host "  Demarrage en cours... $i secondes restantes" -ForegroundColor Gray
    Start-Sleep -Seconds 1
}

# Afficher le guide
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SERVICES PRETS!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs d'acces:" -ForegroundColor Yellow
Write-Host "  RH:   http://localhost:5174" -ForegroundColor White
Write-Host "  TIMS: http://localhost:5175" -ForegroundColor White
Write-Host "  EAMS: http://localhost:5173" -ForegroundColor White
Write-Host ""
Write-Host "Identifiants de test:" -ForegroundColor Yellow
Write-Host "  Email:    mohamed.hassan@onee.ma" -ForegroundColor White
Write-Host "  Password: Test@123" -ForegroundColor White
Write-Host ""
Write-Host "Procedure de test:" -ForegroundColor Yellow
Write-Host "  1. Ouvrez http://localhost:5174" -ForegroundColor White
Write-Host "  2. Cliquez sur 'Se connecter avec SSO'" -ForegroundColor White
Write-Host "  3. Entrez les identifiants ci-dessus" -ForegroundColor White
Write-Host "  4. Cliquez sur 'Autoriser'" -ForegroundColor White
Write-Host "  5. Le dashboard RH devrait s'afficher" -ForegroundColor White
Write-Host ""
Write-Host "Pour voir le guide complet, ouvrez:" -ForegroundColor Yellow
Write-Host "  GUIDE_TEST_COMPLET.md" -ForegroundColor White
Write-Host ""
