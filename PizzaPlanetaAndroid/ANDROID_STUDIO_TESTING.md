# Guía Rápida - Probar en Android Studio

## 1. Configurar AVD (Android Virtual Device)

1. En Android Studio, ve a: **Tools > AVD Manager**
2. Haz clic en **"Create Virtual Device"**
3. Selecciona un dispositivo (recomendado: **Pixel 4** o **Pixel 6**)
4. Selecciona una imagen del sistema:
   - **API Level 30** (Android 11) o superior
   - Arquitectura: **x86_64** (más rápido en PC)
5. Nombra tu AVD: **"ManyBox_Test"**
6. Haz clic en **Finish**

## 2. Ejecutar la aplicación

### Opción A: Usar el botón Run
1. Asegúrate de que el AVD esté seleccionado en la barra superior
2. Haz clic en el botón **Run** (▶️) verde
3. O usa el atajo: **Shift + F10**

### Opción B: Usar el menú
1. Ve a: **Run > Run 'app'**
2. Selecciona tu dispositivo virtual

## 3. Credenciales de prueba

Una vez que la app se ejecute, usa estas credenciales:

```
Usuario: chofer@test.com
Contraseña: 123456
```

## 4. Funcionalidades a probar

### ✅ Login
- Ingresa credenciales
- Verifica que se guarde el token
- Confirma navegación al dashboard

### ✅ Dashboard
- Revisa las tarjetas de estadísticas
- Verifica que carguen las guías recientes
- Prueba el botón de escáner QR

### ✅ Guías Asignadas
- Lista de guías expandible
- Botones de cambio de estado
- Estados visuales correctos

### ✅ Bitácora
- Historial de entregas
- Estadísticas del día
- Botón de generar reporte

### ✅ Notificaciones
- Lista de notificaciones
- Marcar como leídas
- Eliminar notificaciones

## 5. Debugging

### Si hay errores de compilación:
1. Ve a: **Build > Clean Project**
2. Luego: **Build > Rebuild Project**

### Si hay problemas de conexión:
1. Verifica que tu API esté funcionando
2. Revisa los logs en: **View > Tool Windows > Logcat**

### Para ver logs detallados:
```kotlin
Log.d("ManyBox", "Mensaje de debug")
Log.e("ManyBox", "Error", exception)
```

## 6. Dispositivo físico (Opcional)

Si tienes un dispositivo Android real:

1. Habilita **"Opciones de desarrollador"**:
   - Ve a Configuración > Acerca del teléfono
   - Toca 7 veces en "Número de compilación"

2. Habilita **"Depuración USB"**:
   - Ve a Configuración > Opciones de desarrollador
   - Activa "Depuración USB"

3. Conecta tu dispositivo con USB
4. Autoriza la conexión cuando aparezca el diálogo
5. Selecciona tu dispositivo en Android Studio y ejecuta

## Notas importantes:
- La primera ejecución puede tardar más tiempo
- Asegúrate de tener conexión a internet para la API
- Los logs aparecen en la ventana Logcat de Android Studio