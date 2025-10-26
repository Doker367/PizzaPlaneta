# Guía de Pruebas - ManyBox Chofer

## 🚀 Método 1: Android Studio + Emulador

### Prerrequisitos
1. **Descargar Android Studio**: https://developer.android.com/studio
2. **Instalar SDK de Android** (se hace automáticamente)

### Pasos:
1. **Abrir proyecto**:
   ```
   Android Studio → Open → Seleccionar carpeta "ManyBoxAndroid"
   ```

2. **Crear emulador**:
   ```
   Tools → AVD Manager → Create Virtual Device
   Seleccionar: Pixel 6 API 34 (Android 14)
   ```

3. **Configurar API**:
   - Editar `app/src/main/java/com/manybox/chofer/api/ApiClient.kt`
   - Cambiar línea 9: `BASE_URL = "http://10.0.2.2:5000/"` (para emulador)
   
4. **Ejecutar**:
   ```
   Run → Run 'app' (o presionar Shift+F10)
   ```

---

## 📱 Método 2: Dispositivo Físico

### Prerrequisitos
1. **Teléfono Android** con USB Debugging habilitado
2. **Cable USB**

### Habilitar USB Debugging:
1. Ir a **Configuración → Acerca del teléfono**
2. Tocar **Número de compilación** 7 veces
3. Ir a **Configuración → Opciones de desarrollador**
4. Activar **Depuración USB**

### Pasos:
1. **Conectar teléfono** vía USB
2. **Autorizar** la conexión en el teléfono
3. **Configurar API**:
   - Cambiar `BASE_URL = "http://TU_IP_LOCAL:5000/"`
   - Ejemplo: `"http://192.168.1.100:5000/"`
4. **Ejecutar** desde Android Studio

---

## 🌐 Método 3: APK Directo (Más Simple)

### Si ya tienes el APK compilado:
1. **Transferir APK** al teléfono
2. **Habilitar instalación** de fuentes desconocidas
3. **Instalar APK**
4. **Configurar IP** de tu servidor API

---

## ⚙️ Configuración de API Necesaria

### Antes de probar, configura tu API:

1. **En ApiClient.kt** (línea 9):
```kotlin
// Para emulador Android
private const val BASE_URL = "http://10.0.2.2:5000/"

// Para dispositivo físico (reemplaza por tu IP)
private const val BASE_URL = "http://192.168.1.XXX:5000/"

// Para servidor remoto
private const val BASE_URL = "https://tu-dominio.com/"
```

2. **Modificar AuthController.cs** para incluir empleadoId:
```csharp
return Ok(new
{
    token,
    user = new
    {
        usuario.Id,
        usuario.Username,
        usuario.Nombre,
        usuario.Apellido,
        Rol = usuario.Rol?.Nombre,
        usuario.Activo,
        EmpleadoId = usuario.EmpleadoId ?? usuario.Id // Agregar esta línea
    }
});
```

---

## 🧪 Datos de Prueba

### Crear usuario chofer en tu base de datos:
```sql
-- Ejemplo de usuario para pruebas
INSERT INTO usuarios (username, nombre, apellido, password_hash, rol_id, empleado_id, activo)
VALUES ('chofer1', 'Juan', 'Pérez', 'hash_de_contraseña', 3, 1, 1);

-- Crear empleado asociado
INSERT INTO empleados (nombre, apellido, correo, telefono)
VALUES ('Juan', 'Pérez', 'juan@test.com', '555-0123');
```

### Credenciales de prueba:
- **Usuario**: `chofer1`
- **Contraseña**: `123456` (o la que configures)

---

## 🔍 Testing Paso a Paso

### 1. Probar Login:
- Abrir app
- Ingresar credenciales
- Verificar que redirija a pantalla principal

### 2. Probar Guías:
- Verificar que cargue lista de guías
- Tocar una guía para expandir estados
- Usar pull-to-refresh

### 3. Probar Logout:
- Menú → Cerrar Sesión
- Verificar que regrese al login

---

## 🐛 Troubleshooting Común

### Error de conexión:
```
- Verificar IP/URL de API
- Verificar que API esté corriendo
- Verificar firewall/permisos de red
```

### No carga guías:
```
- Verificar que usuario tenga empleadoId
- Verificar endpoint /api/envios/asignados/empleado/{id}
- Revisar logs en Android Studio
```

### Error de compilación:
```
- Build → Clean Project
- Build → Rebuild Project
- Verificar versión de Android SDK
```

---

## 📋 Checklist de Pruebas

- [ ] Login exitoso
- [ ] Login con credenciales incorrectas (error)
- [ ] Carga de guías asignadas
- [ ] Expansión de detalles de guía
- [ ] Pull-to-refresh funciona
- [ ] Logout funciona
- [ ] Manejo de errores de red
- [ ] Estados visuales (loading, empty, error)

---

## 🚀 Siguiente Nivel: Pruebas Avanzadas

### Con datos reales:
1. **Crear envíos** en tu sistema web
2. **Asignar a chofer** de prueba
3. **Actualizar estados** desde web
4. **Verificar** que se reflejen en la app móvil