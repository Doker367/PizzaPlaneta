# ManyBox Chofer - Aplicación Android

Aplicación Android para choferes del sistema ManyBox. Permite a los choferes iniciar sesión y ver las guías de envío asignadas con su estado de seguimiento.

## Características

- **Login de usuario**: Autenticación segura usando JWT
- **Visualización de guías**: Lista de guías asignadas al chofer
- **Estados de seguimiento**: Visualización detallada del progreso de cada envío
- **Interfaz expandible**: Toque una guía para ver detalles del seguimiento
- **Actualización manual**: Pull-to-refresh para actualizar datos
- **Modo offline**: Manejo de errores de conexión

## Estructura del Proyecto

```
app/src/main/java/com/manybox/chofer/
├── activities/
│   ├── LoginActivity.kt          # Pantalla de inicio de sesión
│   └── MainActivity.kt           # Pantalla principal con guías
├── adapters/
│   ├── GuiasAdapter.kt          # Adaptador para lista de guías
│   └── EstadosAdapter.kt        # Adaptador para estados de seguimiento
├── api/
│   ├── ApiClient.kt             # Cliente HTTP con Retrofit
│   └── ApiService.kt            # Definición de endpoints
├── models/
│   ├── LoginRequest.kt          # Modelo para request de login
│   ├── LoginResponse.kt         # Modelo para response de login
│   ├── GuiaChofer.kt           # Modelo para guías de chofer
│   └── ApiError.kt             # Modelo para errores de API
└── utils/
    └── SessionManager.kt        # Manejo de sesión de usuario
```

## Configuración

### 1. Configurar URL de API

Edita el archivo `ApiClient.kt` y cambia la URL base:

```kotlin
private const val BASE_URL = "https://tu-api-url.com/"
```

### 2. Configurar empleadoId

En `LoginActivity.kt`, ajusta cómo se obtiene el empleadoId después del login. Actualmente usa el userId como temporal.

### 3. Iconos de aplicación

Reemplaza los iconos en las carpetas `mipmap-*` con los iconos de tu aplicación.

## API Endpoints Utilizados

### Autenticación
- `POST /api/auth/login`
  - Request: `{ "username": "string", "password": "string" }`
  - Response: `{ "token": "string", "user": { ... } }`

### Guías de Chofer
- `GET /api/envios/asignados/empleado/{empleadoId}`
  - Header: `Authorization: Bearer {token}`
  - Response: Array de guías con estados

## Estados de Guía

La aplicación maneja 4 estados principales:

1. **En preparación** (0) - Paquete en sucursal
2. **En camino** (1) - El chofer tiene el paquete
3. **Último tramo** (2) - El chofer está por entregar
4. **Entregado** (3) - Paquete entregado

## Dependencias Principales

- **Retrofit 2.9.0**: Cliente HTTP para API REST
- **Material Design Components**: UI/UX consistente
- **Kotlin Coroutines**: Programación asíncrona
- **ViewBinding**: Acceso type-safe a vistas
- **SwipeRefreshLayout**: Pull-to-refresh functionality

## Instalación

1. Clona el repositorio
2. Abre el proyecto en Android Studio
3. Configura la URL de API en `ApiClient.kt`
4. Ejecuta `./gradlew build` para compilar
5. Instala en dispositivo con `./gradlew installDebug`

## Personalización

### Colores
Edita `colors.xml` para cambiar el esquema de colores:
- `primary`: Color principal de la app
- `estado_*`: Colores de estados de guía

### Strings
Todos los textos están en `strings.xml` para fácil traducción.

### Estilos
Los estilos están definidos en `themes.xml` siguiendo Material Design.

## Seguridad

- Los tokens JWT se almacenan de forma segura en SharedPreferences
- Se excluyen del backup automático de Android
- Se limpian al cerrar sesión

## Compatibilidad

- **MinSDK**: 24 (Android 7.0)
- **TargetSDK**: 34 (Android 14)
- **Lenguaje**: Kotlin

## Próximas Funcionalidades

- [ ] Actualización automática de estados
- [ ] Notificaciones push
- [ ] Mapas de ubicación
- [ ] Cámara para evidencias de entrega
- [ ] Firma digital del destinatario