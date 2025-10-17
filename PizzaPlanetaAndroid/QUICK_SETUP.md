# Configuración Rápida para Pruebas

## 🔧 Cambios Necesarios en tu API

### 1. Modificar AuthController.cs

Busca en tu `AuthController.cs` la parte del return del login y agrégale el empleadoId:

```csharp
// ANTES (línea ~42 aproximadamente):
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
        usuario.Activo
    }
});

// DESPUÉS (agregar EmpleadoId):
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
        EmpleadoId = usuario.EmpleadoId ?? usuario.Id // ← AGREGAR ESTA LÍNEA
    }
});
```

### 2. Permitir CORS para la app móvil

En `Program.cs`, asegúrate de tener CORS configurado:

```csharp
// Agregar antes de app.UseRouting():
app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
```

### 3. Crear usuario de prueba

Ejecuta este SQL en tu base de datos:

```sql
-- Crear rol de chofer si no existe
INSERT INTO roles (nombre) VALUES ('Chofer');

-- Crear empleado
INSERT INTO empleados (nombre, apellido, correo, telefono, sucursal_id) 
VALUES ('Juan', 'Pérez', 'juan@test.com', '555-0123', 1);

-- Obtener IDs (ajustar según tu base de datos)
DECLARE @rolId INT = (SELECT id FROM roles WHERE nombre = 'Chofer');
DECLARE @empleadoId INT = (SELECT id FROM empleados WHERE nombre = 'Juan' AND apellido = 'Pérez');

-- Crear usuario chofer (ajustar el hash según tu sistema)
INSERT INTO usuarios (username, nombre, apellido, password_hash, rol_id, empleado_id, activo, fecha_creacion)
VALUES ('chofer1', 'Juan', 'Pérez', 'TU_HASH_AQUI', @rolId, @empleadoId, 1, GETDATE());
```

### 4. Crear algunos envíos de prueba

```sql
-- Crear envíos asignados al chofer
INSERT INTO envios (guia_rastreo, fecha_entrega, empleado_id, venta_id)
VALUES 
('MB001234', NULL, @empleadoId, 1),
('MB001235', NULL, @empleadoId, 2),
('MB001236', NULL, @empleadoId, 3);

-- Crear algunos seguimientos
INSERT INTO seguimiento_paquete (envio_id, status, fecha_status, descripcion)
VALUES 
(1, 'Asignado', GETDATE(), 'Paquete asignado al chofer'),
(2, 'En camino', GETDATE(), 'Chofer en ruta'),
(3, 'Entregado', GETDATE(), 'Paquete entregado');
```

## 📱 Configuración de la App Android

### Obtener tu IP local:

**Windows:**
```cmd
ipconfig
# Buscar "IPv4 Address" de tu adaptador de red principal
```

**Android Studio:**
1. Abrir proyecto en `ManyBoxAndroid`
2. Editar `app/src/main/java/com/manybox/chofer/api/ApiClient.kt`
3. Cambiar línea 9:

```kotlin
// Para pruebas locales (reemplaza XXX.XXX.XXX.XXX con tu IP)
private const val BASE_URL = "http://XXX.XXX.XXX.XXX:5000/"

// Ejemplo:
private const val BASE_URL = "http://192.168.1.100:5000/"
```

## 🏃‍♂️ Pasos Rápidos para Probar

### Opción A: Emulador (Más fácil)
1. **Instalar Android Studio**
2. **Abrir proyecto** ManyBoxAndroid
3. **Cambiar URL** a `http://10.0.2.2:5000/` (IP especial para emulador)
4. **Crear emulador** (Tools → AVD Manager)
5. **Run** (botón play verde)

### Opción B: Tu teléfono
1. **Habilitar "Opciones de desarrollador"** en tu teléfono
2. **Activar "Depuración USB"**
3. **Conectar** teléfono vía USB
4. **Cambiar URL** a tu IP local
5. **Run** desde Android Studio

## 🧪 Flujo de Prueba Completo

1. **Ejecutar tu API** (IIS Express o similar)
2. **Abrir app** en emulador/teléfono
3. **Login** con: `chofer1` / `tu_contraseña`
4. **Verificar** que carguen las guías
5. **Tocar** una guía para expandir detalles
6. **Pull-to-refresh** para actualizar
7. **Logout** desde el menú

## ❗ Si algo no funciona

### Verificar logs:
- **Android Studio**: Ventana "Logcat" (abajo)
- **API**: Console de Visual Studio o logs de IIS

### Errores comunes:
- **"Network error"**: Verificar IP/URL y que la API esté corriendo
- **"Invalid credentials"**: Verificar usuario en base de datos
- **"No guías"**: Verificar que el empleadoId tenga envíos asignados