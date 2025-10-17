# Configuración Adicional - ManyBox Chofer

## Pasos Post-Creación

### 1. Configuración de API
- Editar `ApiClient.kt` línea 9: cambiar `BASE_URL` por la URL real de tu API
- Editar `Constants.kt` línea 6: actualizar la misma URL

### 2. Mapping de EmpleadoId
En tu API actual, necesitas agregar el `empleadoId` al response del login, o crear un endpoint adicional para obtenerlo:

```csharp
// En AuthController.cs, modificar el response del login:
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
        EmpleadoId = usuario.EmpleadoId  // Agregar esta línea
    }
});
```

### 3. Endpoint de empleado por usuario
Alternativamente, crear un endpoint en `UsuariosController.cs`:

```csharp
[HttpGet("empleado")]
[Authorize]
public async Task<ActionResult<object>> GetEmpleadoActual()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!int.TryParse(userId, out int id)) return BadRequest();
    
    var usuario = await _context.Usuarios
        .Include(u => u.Empleado)
        .FirstOrDefaultAsync(u => u.Id == id);
        
    if (usuario?.Empleado == null) return NotFound();
    
    return Ok(new { empleadoId = usuario.Empleado.Id });
}
```

### 4. Permisos Android
El AndroidManifest.xml ya incluye:
- `INTERNET`: Para llamadas a API
- `ACCESS_NETWORK_STATE`: Para verificar conectividad

### 5. Iconos de Aplicación
Reemplazar los iconos en:
- `app/src/main/res/mipmap-*/ic_launcher.png`
- `app/src/main/res/mipmap-*/ic_launcher_round.png`

### 6. Compilación
```bash
# En el directorio ManyBoxAndroid
./gradlew build
./gradlew installDebug  # Para instalar en dispositivo conectado
```

### 7. Testing
Usuarios de prueba recomendados en tu base de datos:
- Username: `chofer1` / Password: `123456`
- Rol: `Chofer`
- Con empleadoId válido asignado

### 8. Personalización de Colores
En `res/values/colors.xml`:
- `primary`: Color principal de ManyBox
- `estado_*`: Colores para estados de entrega

### 9. Mejoras Futuras Sugeridas
- Implementar SignalR para actualizaciones en tiempo real
- Agregar geolocalización para tracking
- Implementar cámara para evidencias
- Agregar modo offline con SQLite local

## Estructura de Datos Esperada

### Login Response
```json
{
  "token": "eyJ...",
  "user": {
    "id": 1,
    "username": "chofer1",
    "nombre": "Juan",
    "apellido": "Pérez",
    "rol": "Chofer",
    "activo": true,
    "empleadoId": 5  // IMPORTANTE: Agregar este campo
  }
}
```

### Guías Response
```json
[
  {
    "envioId": 123,
    "guiaRastreo": "MB123456",
    "destinatario": "Cliente Test",
    "ventaId": 456,
    "estados": [
      {
        "titulo": "En preparación",
        "descripcion": "Paquete en sucursal.",
        "fechaHora": "2024-01-15T08:00:00Z"
      }
    ],
    "estadoActual": 0,
    "expandido": false
  }
]
```

## Troubleshooting

### Error de conexión
- Verificar URL de API en `ApiClient.kt`
- Asegurar que `usesCleartextTraffic="true"` en AndroidManifest.xml para HTTP

### Error de autenticación
- Verificar que el endpoint `/api/auth/login` funcione
- Verificar formato del token JWT

### No cargan guías
- Verificar que el empleadoId sea correcto
- Verificar que el endpoint `/api/envios/asignados/empleado/{id}` funcione