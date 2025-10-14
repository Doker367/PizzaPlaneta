# 🚀 GUÍA RÁPIDA - PROBAR EN ANDROID STUDIO

## ✅ PASO A PASO

### 1. Abrir el Proyecto
1. **Abrir Android Studio**
2. Seleccionar **"Open an existing Android Studio project"**
3. Navegar a: `C:\Users\Wiliamm\source\repos\ManyBox\ManyBoxAndroid`
4. Esperar sincronización de Gradle (2-5 minutos)

### 2. Configurar Dispositivo Virtual
1. **Tools → AVD Manager → Create Virtual Device**
2. Dispositivo recomendado: **Pixel 4** o **Pixel 6**
3. Sistema: **API Level 30+** (Android 11+)
4. Arquitectura: **x86_64** (más rápido en PC)

### 3. Ejecutar la App
1. Presionar botón **Run** (▶️) verde
2. O usar atajo: **Shift + F10**
3. Seleccionar tu dispositivo virtual

## 🔐 CREDENCIALES DE PRUEBA
```
Usuario: chofer@test.com
Contraseña: 123456
```

## 📱 FUNCIONALIDADES A PROBAR

### ✅ **Login**
- Interfaz de autenticación
- Validación de campos
- Conexión con API
- Navegación al dashboard

### ✅ **Dashboard**
- Saludo personalizado
- Tarjetas de estadísticas
- Guías recientes
- Botones de acción rápida

### ✅ **Guías Asignadas**
- Lista expandible de guías
- Estados visuales (pendiente, activo, completado)
- Botones de acción contextual:
  - **RECOGER** (estado 0 → 1)
  - **INICIAR ENTREGA** (estado 1 → 2)
  - **ENTREGAR** (estado 2 → 3)
- Filtros por estado

### ✅ **Bitácora**
- Historial de entregas
- Estadísticas del día
- Efectividad y tiempo promedio
- Botón generar reporte

### ✅ **Notificaciones**
- Lista de notificaciones del sistema
- Marcar como leídas (individual/masivo)
- Eliminar notificaciones
- Contador de no leídas

## 🔧 DEBUGGING

### Si hay errores de compilación:
```
Build → Clean Project
Build → Rebuild Project
```

### Para ver logs:
```
View → Tool Windows → Logcat
Filtrar por: "ManyBox"
```

### Si hay problemas de conexión:
- Verificar que la API esté funcionando
- Revisar conexión a internet del emulador

## 📋 CHECKLIST DE PRUEBAS

- [ ] Login exitoso
- [ ] Dashboard carga estadísticas
- [ ] Lista de guías se muestra correctamente
- [ ] Botones de cambio de estado funcionan
- [ ] Bitácora muestra historial
- [ ] Notificaciones se cargan
- [ ] Navegación entre pestañas fluida
- [ ] SwipeRefresh funciona en todas las pantallas

## 🎯 PRÓXIMOS PASOS

Una vez que la app funcione correctamente:
1. **Probar en dispositivo físico** (opcional)
2. **Ajustar la URL de la API** si es necesario
3. **Personalizar colores y estilos**
4. **Agregar funcionalidades adicionales** (QR, GPS, etc.)

¡Tu aplicación Android está lista para probar! 🎉