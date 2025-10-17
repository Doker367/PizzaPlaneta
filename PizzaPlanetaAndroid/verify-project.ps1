# Script de verificación para Android Studio - PowerShell
Write-Host "=== Verificación de Proyecto ManyBox Android ===" -ForegroundColor Cyan

# Verificar estructura del proyecto
Write-Host "1. Verificando estructura del proyecto..." -ForegroundColor Yellow
if ((Test-Path "build.gradle") -and (Test-Path "settings.gradle") -and (Test-Path "app")) {
    Write-Host "✅ Estructura del proyecto correcta" -ForegroundColor Green
} else {
    Write-Host "❌ Estructura del proyecto incompleta" -ForegroundColor Red
    exit 1
}

# Verificar archivos principales
Write-Host "2. Verificando archivos principales..." -ForegroundColor Yellow
$files = @(
    "app\build.gradle",
    "app\src\main\AndroidManifest.xml",
    "app\src\main\java\com\manybox\chofer\MainActivity.kt",
    "app\src\main\java\com\manybox\chofer\LoginActivity.kt",
    "app\src\main\java\com\manybox\chofer\DashboardFragment.kt",
    "app\src\main\java\com\manybox\chofer\GuiasFragment.kt",
    "app\src\main\java\com\manybox\chofer\BitacoraFragment.kt",
    "app\src\main\java\com\manybox\chofer\NotificacionesFragment.kt"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
    } else {
        Write-Host "❌ $file - FALTANTE" -ForegroundColor Red
    }
}

# Verificar dependencias principales
Write-Host "3. Verificando dependencias en build.gradle..." -ForegroundColor Yellow
$buildGradle = Get-Content "app\build.gradle" -Raw

if ($buildGradle -match "androidx.navigation") {
    Write-Host "✅ Navigation Component" -ForegroundColor Green
} else {
    Write-Host "❌ Navigation Component no encontrado" -ForegroundColor Red
}

if ($buildGradle -match "retrofit2") {
    Write-Host "✅ Retrofit" -ForegroundColor Green
} else {
    Write-Host "❌ Retrofit no encontrado" -ForegroundColor Red
}

if ($buildGradle -match "material") {
    Write-Host "✅ Material Design" -ForegroundColor Green
} else {
    Write-Host "❌ Material Design no encontrado" -ForegroundColor Red
}

# Verificar servicios y modelos
Write-Host "4. Verificando servicios y modelos..." -ForegroundColor Yellow
$serviceFiles = @(
    "app\src\main\java\com\manybox\chofer\services\ApiService.kt",
    "app\src\main\java\com\manybox\chofer\utils\SessionManager.kt",
    "app\src\main\java\com\manybox\chofer\models\GuiaChofer.kt",
    "app\src\main\java\com\manybox\chofer\models\DashboardModels.kt"
)

foreach ($file in $serviceFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
    } else {
        Write-Host "❌ $file - FALTANTE" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== INSTRUCCIONES PARA ANDROID STUDIO ===" -ForegroundColor Cyan
Write-Host "1. Abre Android Studio" -ForegroundColor White
Write-Host "2. Selecciona 'Open an existing Android Studio project'" -ForegroundColor White
Write-Host "3. Navega a esta carpeta: $PWD" -ForegroundColor White
Write-Host "4. Espera la sincronización de Gradle (2-5 minutos)" -ForegroundColor White
Write-Host "5. Configura un AVD (Android Virtual Device):" -ForegroundColor White
Write-Host "   - Tools > AVD Manager > Create Virtual Device" -ForegroundColor Gray
Write-Host "   - Selecciona Pixel 4 o Pixel 6" -ForegroundColor Gray
Write-Host "   - API Level 30+ (Android 11+)" -ForegroundColor Gray
Write-Host "6. Presiona el botón Run (▶️) o Shift+F10" -ForegroundColor White
Write-Host ""
Write-Host "=== CREDENCIALES DE PRUEBA ===" -ForegroundColor Green
Write-Host "Usuario: chofer@test.com" -ForegroundColor White
Write-Host "Contraseña: 123456" -ForegroundColor White
Write-Host ""
Write-Host "=== FUNCIONALIDADES A PROBAR ===" -ForegroundColor Magenta
Write-Host "✅ Login y autenticación" -ForegroundColor White
Write-Host "✅ Dashboard con estadísticas" -ForegroundColor White
Write-Host "✅ Lista de guías asignadas" -ForegroundColor White
Write-Host "✅ Cambio de estados de guías" -ForegroundColor White
Write-Host "✅ Bitácora de entregas" -ForegroundColor White
Write-Host "✅ Sistema de notificaciones" -ForegroundColor White
Write-Host ""
Write-Host "¡Proyecto listo para probar! 🚀" -ForegroundColor Green