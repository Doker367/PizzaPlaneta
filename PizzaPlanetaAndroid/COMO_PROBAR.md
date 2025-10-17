# Guía Rápida: Probar App Android

## 🚀 MÉTODO 1: Android Studio (FÁCIL)

### 1. Descargar Android Studio
- Ir a: https://developer.android.com/studio
- Descargar e instalar (incluye SDK automáticamente)

### 2. Abrir Proyecto
```
Android Studio → Open → Seleccionar:
C:\Users\Wiliamm\source\repos\ManyBox\ManyBoxAndroid
```

### 3. Primera vez (esperar 5-10 minutos):
- Descarga dependencias automáticamente
- Configura SDK
- Instala herramientas necesarias

### 4. Crear Emulador
```
Tools → AVD Manager → Create Virtual Device
- Seleccionar: Pixel 6 Pro
- API Level: 34 (Android 14)
- Hacer clic en Download y Next
```

### 5. Configurar API
Editar archivo: `app/src/main/java/com/manybox/chofer/api/ApiClient.kt`
```kotlin
// Línea 9, cambiar por:
private const val BASE_URL = "http://10.0.2.2:5000/" // Para emulador
```

### 6. Ejecutar
- Presionar botón ▶️ verde
- Esperar que abra el emulador
- ¡Probar la app!

---

## 📱 MÉTODO 2: Tu Teléfono Android (RÁPIDO)

### 1. Habilitar Modo Desarrollador
```
Configuración → Acerca del teléfono → 
Tocar "Número de compilación" 7 veces
```

### 2. Activar USB Debugging
```
Configuración → Opciones de desarrollador → 
Activar "Depuración USB"
```

### 3. Conectar y Configurar
- Conectar teléfono vía USB
- Autorizar la conexión
- En ApiClient.kt cambiar a tu IP local:
```kotlin
private const val BASE_URL = "http://192.168.1.XXX:5000/"
```

### 4. Desde Android Studio
- Seleccionar tu dispositivo en lugar del emulador
- Presionar ▶️ Run

---

## ⚡ MÉTODO 3: Compilar APK Directo

### En Android Studio:
```
Build → Build Bundle(s) / APK(s) → Build APK(s)
```
- Se genera: `app/build/outputs/apk/debug/app-debug.apk`
- Transferir al teléfono e instalar

---

## 🔧 ANTES DE PROBAR: Configurar tu API

### 1. Ejecutar tu ManyBoxApi
```powershell
# En Visual Studio, ejecutar ManyBoxApi
# O desde terminal:
cd "API\ManyBoxApi"
dotnet run
```

### 2. Obtener tu IP local
```powershell
ipconfig
# Buscar IPv4 Address de tu adaptador de red
```

### 3. Configurar base de datos (usar el SQL que creé)
Ver archivo: `ManyBoxAndroid\database_setup.sql`

---

## 🧪 FLUJO DE PRUEBA COMPLETO

### Credenciales de prueba:
- **Usuario**: `chofer1`
- **Contraseña**: `123456`

### Pasos:
1. ✅ **Ejecutar ManyBoxApi** (localhost:5000)
2. ✅ **Ejecutar app Android** (emulador o teléfono)
3. ✅ **Login** con credenciales
4. ✅ **Ver guías asignadas**
5. ✅ **Expandir detalles** (tocar una guía)
6. ✅ **Pull-to-refresh** (deslizar hacia abajo)
7. ✅ **Logout** (menú → cerrar sesión)

---

## 🆘 Si algo no funciona:

### Error "Network Error":
- Verificar que ManyBoxApi esté corriendo
- Verificar IP en ApiClient.kt
- Verificar firewall de Windows

### No carga guías:
- Verificar datos en base de datos
- Verificar que empleadoId exista
- Ver logs en Android Studio (Logcat)

### App no instala:
- Habilitar "Fuentes desconocidas" en Android
- Verificar que el emulador tenga suficiente memoria