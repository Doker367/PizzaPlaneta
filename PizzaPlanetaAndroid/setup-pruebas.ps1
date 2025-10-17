# Script de Configuración Automática para Pruebas
# Ejecutar en PowerShell como Administrador

Write-Host "🚀 CONFIGURANDO MANYBOX ANDROID PARA PRUEBAS..." -ForegroundColor Green

# Función para obtener IP local
function Get-LocalIP {
    $adapters = Get-NetIPAddress -AddressFamily IPv4 | Where-Object {
        $_.InterfaceAlias -notlike "*Loopback*" -and 
        $_.IPAddress -notlike "169.254.*" -and
        $_.IPAddress -ne "127.0.0.1"
    }
    return $adapters[0].IPAddress
}

$localIP = Get-LocalIP
Write-Host "📡 Tu IP local detectada: $localIP" -ForegroundColor Yellow

# Verificar si ManyBoxApi está ejecutándose
Write-Host "`n🔍 Verificando si ManyBoxApi está corriendo..." -ForegroundColor Cyan
$apiRunning = $false
try {
    $null = Invoke-WebRequest -Uri "http://localhost:5000" -Method GET -TimeoutSec 3 -ErrorAction SilentlyContinue
    $apiRunning = $true
    Write-Host "✅ ManyBoxApi está corriendo en puerto 5000" -ForegroundColor Green
} catch {
    Write-Host "❌ ManyBoxApi NO está corriendo" -ForegroundColor Red
    Write-Host "   💡 Ejecuta tu API desde Visual Studio primero" -ForegroundColor Yellow
}

# Mostrar opciones de configuración
Write-Host "`n🎯 OPCIONES DE CONFIGURACIÓN:" -ForegroundColor Magenta
Write-Host "1. 📱 Emulador Android (recomendado para principiantes)"
Write-Host "2. 📲 Dispositivo físico (requiere cable USB)"
Write-Host "3. 🌐 Servidor remoto (URL personalizada)"

$choice = Read-Host "`nSelecciona una opción (1-3)"

switch ($choice) {
    "1" { 
        $apiUrl = "http://10.0.2.2:5000/"
        $setupType = "EMULADOR"
        Write-Host "✅ Configurando para EMULADOR Android" -ForegroundColor Green
    }
    "2" { 
        $apiUrl = "http://${localIP}:5000/"
        $setupType = "DISPOSITIVO FÍSICO"
        Write-Host "✅ Configurando para DISPOSITIVO FÍSICO" -ForegroundColor Green
    }
    "3" { 
        $apiUrl = Read-Host "Ingresa la URL completa de tu servidor (ej: https://mi-api.com/)"
        $setupType = "SERVIDOR REMOTO"
        Write-Host "✅ Configurando para SERVIDOR REMOTO" -ForegroundColor Green
    }
    default { 
        $apiUrl = "http://10.0.2.2:5000/"
        $setupType = "EMULADOR (por defecto)"
        Write-Host "⚠️ Usando configuración por defecto: EMULADOR" -ForegroundColor Yellow
    }
}

# Crear archivo de configuración para Android
$apiClientPath = ".\ManyBoxAndroid\app\src\main\java\com\manybox\chofer\api\ApiClient.kt"
if (Test-Path $apiClientPath) {
    Write-Host "`n🔧 Configurando URL de API en la app..." -ForegroundColor Cyan
    
    # Leer contenido actual
    $content = Get-Content $apiClientPath -Raw
    
    # Reemplazar URL
    $newContent = $content -replace 'private const val BASE_URL = ".*"', "private const val BASE_URL = `"$apiUrl`""
    
    # Escribir de vuelta
    Set-Content $apiClientPath $newContent
    
    Write-Host "✅ URL configurada: $apiUrl" -ForegroundColor Green
} else {
    Write-Host "❌ No se encontró el archivo ApiClient.kt" -ForegroundColor Red
}

# Mostrar resumen de configuración
Write-Host "`n📋 RESUMEN DE CONFIGURACIÓN:" -ForegroundColor Blue
Write-Host "================================" -ForegroundColor Blue
Write-Host "🎯 Tipo: $setupType" -ForegroundColor White
Write-Host "🌐 URL API: $apiUrl" -ForegroundColor White
Write-Host "💻 API Local: $(if($apiRunning){'✅ Corriendo'}else{'❌ Detenida'})" -ForegroundColor White
Write-Host "📱 App: ✅ Configurada" -ForegroundColor White

# Mostrar próximos pasos
Write-Host "`n🚀 PRÓXIMOS PASOS:" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green

if (-not $apiRunning) {
    Write-Host "1. ⚠️  EJECUTAR ManyBoxApi desde Visual Studio" -ForegroundColor Red
}

Write-Host "2. 📱 Instalar Android Studio:" -ForegroundColor Yellow
Write-Host "   https://developer.android.com/studio" -ForegroundColor Cyan

Write-Host "3. 🔗 Abrir proyecto Android:" -ForegroundColor Yellow
Write-Host "   Android Studio → Open → Seleccionar: ManyBoxAndroid" -ForegroundColor Cyan

Write-Host "4. ⚙️  Primera ejecución (esperar 5-10 min para descargas)" -ForegroundColor Yellow

Write-Host "5. 🎮 Crear emulador:" -ForegroundColor Yellow
Write-Host "   Tools → AVD Manager → Create Virtual Device" -ForegroundColor Cyan

Write-Host "6. ▶️  Ejecutar app (botón play verde)" -ForegroundColor Yellow

Write-Host "7. 🧪 Probar login:" -ForegroundColor Yellow
Write-Host "   Usuario: chofer1 | Contraseña: 123456" -ForegroundColor Cyan

# Crear datos de prueba
Write-Host "`n💾 ¿CREAR DATOS DE PRUEBA EN BASE DE DATOS?" -ForegroundColor Magenta
$createData = Read-Host "¿Ejecutar script SQL de datos de prueba? (y/n)"

if ($createData -eq 'y' -or $createData -eq 'Y') {
    Write-Host "📄 Revisa el archivo: database_setup.sql" -ForegroundColor Yellow
    Write-Host "💡 Ejecuta ese SQL en tu base de datos ManyBox" -ForegroundColor Yellow
}

# Mostrar checklist final
Write-Host "`n✅ CHECKLIST DE VERIFICACIÓN:" -ForegroundColor Cyan
Write-Host "□ ManyBoxApi corriendo (Visual Studio)" -ForegroundColor Gray
Write-Host "□ Android Studio instalado" -ForegroundColor Gray
Write-Host "□ Proyecto ManyBoxAndroid abierto" -ForegroundColor Gray
Write-Host "□ URL de API configurada ✅" -ForegroundColor Green
Write-Host "□ Emulador creado" -ForegroundColor Gray
Write-Host "□ Datos de prueba en BD" -ForegroundColor Gray
Write-Host "□ App ejecutándose" -ForegroundColor Gray

Write-Host "`n🎉 ¡CONFIGURACIÓN COMPLETADA!" -ForegroundColor Green
Write-Host "💡 Si tienes problemas, revisa: COMO_PROBAR.md" -ForegroundColor Blue

# Pausa para que el usuario pueda leer
Read-Host "`nPresiona Enter para continuar..."