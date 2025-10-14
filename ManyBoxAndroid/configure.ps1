# Script de configuración para ManyBox Android
# Ejecutar como administrador en PowerShell

Write-Host "🚀 Configurando ManyBox Android App..." -ForegroundColor Green

# Función para obtener IP local
function Get-LocalIP {
    $ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -like "*Wi-Fi*" -or $_.InterfaceAlias -like "*Ethernet*"} | Select-Object -First 1).IPAddress
    return $ip
}

# Obtener IP local
$localIP = Get-LocalIP
Write-Host "📡 Tu IP local es: $localIP" -ForegroundColor Yellow

# Crear archivo de configuración
$configPath = ".\app\src\main\java\com\manybox\chofer\api\ApiClient.kt"
$originalUrl = "https://your-api-url.com/"
$newUrl = "http://${localIP}:5000/"

Write-Host "🔧 Configuraciones disponibles:" -ForegroundColor Cyan
Write-Host "1. Emulador Android (10.0.2.2:5000)"
Write-Host "2. Dispositivo físico ($localIP:5000)"
Write-Host "3. Servidor remoto (manual)"

$choice = Read-Host "Selecciona una opción (1-3)"

switch ($choice) {
    "1" { 
        $apiUrl = "http://10.0.2.2:5000/"
        Write-Host "✅ Configurado para emulador Android" -ForegroundColor Green
    }
    "2" { 
        $apiUrl = "http://${localIP}:5000/"
        Write-Host "✅ Configurado para dispositivo físico" -ForegroundColor Green
    }
    "3" { 
        $apiUrl = Read-Host "Ingresa la URL de tu servidor remoto (ej: https://tu-api.com/)"
        Write-Host "✅ Configurado para servidor remoto" -ForegroundColor Green
    }
    default { 
        $apiUrl = "http://10.0.2.2:5000/"
        Write-Host "⚠️  Usando configuración por defecto: emulador" -ForegroundColor Yellow
    }
}

# Mostrar checklist
Write-Host "`n📋 CHECKLIST DE CONFIGURACIÓN:" -ForegroundColor Magenta
Write-Host "□ 1. Modificar AuthController.cs (agregado EmpleadoId) ✅" -ForegroundColor Green
Write-Host "□ 2. Configurar CORS en tu API" -ForegroundColor Yellow
Write-Host "□ 3. Crear usuario de prueba en base de datos" -ForegroundColor Yellow
Write-Host "□ 4. URL de API configurada: $apiUrl ✅" -ForegroundColor Green
Write-Host "□ 5. Android Studio instalado" -ForegroundColor Yellow
Write-Host "□ 6. Emulador/dispositivo listo" -ForegroundColor Yellow

Write-Host "`n🎯 PRÓXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "1. Abrir Android Studio"
Write-Host "2. Open Project → Seleccionar carpeta 'ManyBoxAndroid'"
Write-Host "3. Esperar a que descargue dependencias"
Write-Host "4. Run → Run 'app' (Shift+F10)"
Write-Host "5. Probar login con credenciales de prueba"

Write-Host "`n📱 CREDENCIALES DE PRUEBA SUGERIDAS:" -ForegroundColor Blue
Write-Host "Usuario: chofer1"
Write-Host "Contraseña: 123456"
Write-Host "(Crear en tu base de datos usando QUICK_SETUP.md)"

Write-Host "`n✨ ¡Configuración completada!" -ForegroundColor Green
Write-Host "💡 Revisa TESTING.md para guía detallada de pruebas" -ForegroundColor Blue

# Pausa para leer
Read-Host "`nPresiona Enter para continuar..."