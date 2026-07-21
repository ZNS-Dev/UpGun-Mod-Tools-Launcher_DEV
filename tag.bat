@echo off
chcp 65001 > nul
set /p VERSION="Entrez le numéro de version (ex: 1.0.1) : "

if "%VERSION%"=="" (
    echo [ERREUR] Aucune version renseignée.
    pause
    exit /b 1
)

:: Supprime le 'v' initial si tu l'as déjà tapé (ex: "v1.0.1" -> "1.0.1")
set VERSION=%VERSION:v=%

echo.
echo [1/2] Création du tag v%VERSION%...
git tag v%VERSION%

if %ERRORLEVEL% NEQ 0 (
    echo [ERREUR] Échec de la création du tag local.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Envoi du tag vers origin...
git push origin v%VERSION%

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [SUCCÈS] Le tag v%VERSION% a été poussé ! Le CI/CD va démarrer.
) else (
    echo [ERREUR] Échec du push vers origin.
)

pause