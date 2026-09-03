# ========================================
# SETUP COMPLET - CONFIGURATION ET DEMARRAGE
# ========================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   SETUP COMPLET SSO ONEE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Etape 1: Verification de la configuration
Write-Host "ETAPE 1/5: VERIFICATION DE LA CONFIGURATION" -ForegroundColor Yellow
Write-Host "============================================" -ForegroundColor Yellow
Write-Host ""

$configOk = $true

# Verifier la base RH
Write-Host "Verification de la base RH..." -ForegroundColor Gray
try {
    $connectionString = "Server=(localdb)\MSSQLLocalDB;Database=GestionPersonnelDB;Trusted_Connection=True;TrustServerCertificate=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "  OK - Base RH accessible" -ForegroundColor Green
    $connection.Close()
} catch {
    Write-Host "  ERREUR - Base RH inaccessible" -ForegroundColor Red
    Write-Host "  Verifiez que SQL Server LocalDB est installe" -ForegroundColor Yellow
    $configOk = $false
}

if (-not $configOk) {
    Write-Host ""
    Write-Host "Configuration incomplete! Corrigez les erreurs ci-dessus." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "OK - Configuration valide!" -ForegroundColor Green
Start-Sleep -Seconds 2

# Etape 2: Creation/mise a jour de l'utilisateur
Write-Host ""
Write-Host "ETAPE 2/5: CREATION DE L'UTILISATEUR TEST" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host ""

Write-Host "Creation de l'utilisateur mohamed.hassan@onee.ma..." -ForegroundColor Gray
& .\CreateMohamedUser.ps1

Write-Host ""
Write-Host "Mise a jour du SsoId..." -ForegroundColor Gray
& .\UpdateMohamedSsoId.ps1

Start-Sleep -Seconds 2

# Etape 3: Arret des services existants
Write-Host ""
Write-Host "ETAPE 3/5: ARRET DES SERVICES EXISTANTS" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

& .\STOP_ALL.ps1
Start-Sleep -Seconds 2

# Etape 4: Suppression de la base SSO pour forcer la recreation
Write-Host ""
Write-Host "ETAPE 4/5: REINITIALISATION BASE SSO" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Yellow
Write-Host ""

$ssoDbPath = ".\src\ONEE.SSO.API\onee_sso.db"
if (Test-Path $ssoDbPath) {
    Remove-Item $ssoDbPath -Force
    Write-Host "Base SSO supprimee (sera recreee avec les nouvelles configurations)" -ForegroundColor Green
} else {
    Write-Host "Base SSO inexistante (sera creee au demarrage)" -ForegroundColor Gray
}

Start-Sleep -Seconds 2

# Etape 5: Demarrage des services
Write-Host ""
Write-Host "ETAPE 5/5: DEMARRAGE DES SERVICES" -ForegroundColor Yellow
Write-Host "==================================" -ForegroundColor Yellow
Write-Host ""

Write-Host "Demarrage des 7 services (SSO, RH, TIMS, EAMS)..." -ForegroundColor Gray
Write-Host "Cela va prendre environ 30 secondes..." -ForegroundColor Gray
Write-Host ""

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; .\START_ALL.ps1"

# Attendre le demarrage
Write-Host "Attente du demarrage complet..." -ForegroundColor Yellow
for ($i = 30; $i -gt 0; $i--) {
    Write-Progress -Activity "Demarrage en cours" -Status "$i secondes restantes" -PercentComplete ((30 - $i) / 30 * 100)
    Start-Sleep -Seconds 1
}
Write-Progress -Activity "Demarrage en cours" -Completed

# Afficher le resultat final
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   SETUP TERMINE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Vos applications sont pretes:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  RH:   http://localhost:5174" -ForegroundColor White
Write-Host "  TIMS: http://localhost:5175" -ForegroundColor White
Write-Host "  EAMS: http://localhost:5173" -ForegroundColor White
Write-Host ""
Write-Host "Identifiants de test:" -ForegroundColor Yellow
Write-Host "  Email:    mohamed.hassan@onee.ma" -ForegroundColor White
Write-Host "  Password: Test@123" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   TEST DU SSO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Ouvrez http://localhost:5174 dans votre navigateur" -ForegroundColor White
Write-Host "2. Cliquez sur 'Se connecter avec SSO'" -ForegroundColor White
Write-Host "3. Entrez les identifiants ci-dessus" -ForegroundColor White
Write-Host "4. Cliquez sur 'Autoriser' (si demande)" -ForegroundColor White
Write-Host "5. Le dashboard RH doit s'afficher!" -ForegroundColor White
Write-Host ""
Write-Host "Ensuite, testez TIMS et EAMS (connexion automatique!)" -ForegroundColor Yellow
Write-Host ""
Write-Host "Pour plus d'informations, consultez:" -ForegroundColor Gray
Write-Host "  - DEMARRAGE_RAPIDE.md" -ForegroundColor Gray
Write-Host "  - GUIDE_TEST_COMPLET.md" -ForegroundColor Gray
Write-Host ""
Write-Host "Bon test! " -NoNewline -ForegroundColor Green
Write-Host "🚀🎉" -ForegroundColor Yellow
Write-Host ""
