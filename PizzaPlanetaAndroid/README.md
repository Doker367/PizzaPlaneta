# Pizza Planeta Android (Jetpack Compose)

Documentación de la app Android que replica la pantalla Home/Login de Pizza Planeta y consume la API local del backend.

## Qué hace (hoy)

- Pantalla Home con header, logo y acciones “Cuenta” y “Ordenar”.
- Modal de autenticación: Login, Crear cuenta (UI), Olvidé mi contraseña (3 pasos de UI).
- Login contra el backend: POST /api/auth/login.
- Llamada protegida de prueba (con Bearer): GET /api/sucursales.
- Menú lateral con opciones de cuenta y pedido.

## Requisitos

- Android Studio Flamingo o superior (AGP 8+), Java 17.
- Emulador Android o dispositivo físico.
- Backend ejecutándose local: Pizza.Backend (HTTP 5000 / HTTPS 5001 por defecto).

## Cómo correr

- Abre la carpeta `PizzaPlanetaAndroid/` en Android Studio y haz Sync.
- Selecciona un emulador y ejecuta Run (la app abre `PizzaActivity`).
- Por terminal, desde `PizzaPlanetaAndroid/`:
  - Compilar: `./gradlew :app:assembleDebug`
  - Instalar: `./gradlew :app:installDebug`

## Configuración de API (dev)

La base de URL se define en `getPizzaBaseUrl()` dentro de `ui/PizzaHomeScreen.kt`:

- Emulador: `https://10.0.2.2:5001/` (o `http://10.0.2.2:5000/` si usas HTTP).
- Dispositivo físico: reemplaza por la IP de tu PC, ej. `https://192.168.1.10:5001/`.

Para facilitar HTTPS de desarrollo, se usa un `OkHttpClient` que confía en el certificado local (solo DEV). Para producción, elimina esa configuración e instala un certificado válido.

Archivos clave de red:

- `app/src/main/java/com/manybox/chofer/api/PizzaApi.kt`
  - Interface `PizzaApiService` (Retrofit): `login`, `getSucursales`.
  - DTOs: `PizzaLoginRequest`, `PizzaLoginResponse`, `SucursalDto`.
- `app/src/main/java/com/manybox/chofer/ui/PizzaHomeScreen.kt`
  - `buildDevHttpsClient()` y `getPizzaBaseUrl()`.

## Estructura de carpetas (Android)

```
app/src/main/java/com/manybox/chofer/
├── PizzaActivity.kt                # Activity de entrada para Pizza Planeta (launcher)
├── MainActivity.kt                 # Activity legado (ManyBox), no es launcher
├── api/
│   └── PizzaApi.kt                 # Retrofit service + DTOs (login, sucursales)
├── model/                          # Modelos previos (ManyBox); no usados por PizzaHomeScreen
└── ui/
    └── PizzaHomeScreen.kt          # Pantalla Home + modal Login/Registro/Olvido y menú lateral
```

Recursos:

```
app/src/main/res/
├── values/colors.xml               # Paleta de colores
├── values/strings.xml              # Textos
└── values/themes.xml               # Temas Material 3
```

## Dónde hacer cambios

- Base URL/backend:
  - Edita `getPizzaBaseUrl()` en `ui/PizzaHomeScreen.kt`.
  - Si usas HTTP, cambia a `http://...:5000/` y considera permitir cleartext (ver “HTTP sin SSL”).
- Autenticación:
  - `ui/PizzaHomeScreen.kt` -> bloque de `onLogin`: se hace `login` y luego `getSucursales` con el token.
  - Para persistir sesión, crea un `DataStore` (p. ej. `TokenStore.kt`) y agrega un interceptor de `Authorization` a `OkHttpClient`.
- UI Home/Modal:
  - Componentes `LoginForm`, `RegisterForm`, `ForgotStep*` dentro de `PizzaHomeScreen.kt`.
  - Menú lateral (opciones): función `SideOption` y lógica `sideType`.
- Selector de sucursal:
  - Hoy es mock. Reemplaza el `repeat(3)` por la lista real de `SucursalDto` y agrega acciones de selección.
- Temas/colores/textos:
  - `res/values/colors.xml`, `res/values/themes.xml`, `res/values/strings.xml`.
- Launcher Activity:
  - `app/src/main/AndroidManifest.xml` define `PizzaActivity` como launcher. Cambia aquí si necesitas otra entrada.

## Añadir nuevos endpoints

1) Declara el endpoint en `PizzaApiService` (Retrofit) dentro de `api/PizzaApi.kt`.
2) Crea los `data class` necesarios para request/response.
3) Consume el endpoint desde tus Composables o a través de una capa Repository/ViewModel si escalas la arquitectura.
4) Maneja errores y estado de carga (ej. `LinearProgressIndicator`).

Ejemplo mínimo (GET productos):

```kotlin
interface PizzaApiService {
    @GET("api/productos")
    fun getProductos(@Header("Authorization") bearer: String): Call<List<ProductoDto>>
}

data class ProductoDto(val id: Int, val nombre: String, val precio: Double)
```

## HTTP sin SSL (opcional, solo dev)

Si usas `http://10.0.2.2:5000/` y Android 9+ bloquea cleartext:

- Crea `res/xml/network_security_config.xml` con `cleartextTrafficPermitted="true"` para tu dominio/IP.
- En `AndroidManifest.xml` agrega `android:networkSecurityConfig="@xml/network_security_config"` en `<application>`.

Para producción, usa siempre HTTPS real con certificado válido.

## Problemas comunes

- 10.0.2.2 no responde: asegúrate de que el backend corre en tu PC y el puerto es accesible.
- 401 Unauthorized en `getSucursales`: valida que `Bearer <token>` se pasa correctamente y que el backend comparte `JWT_ISSUER/AUDIENCE/SECRET` válidos.
- CORS/HTTPS: backend define CORS AllowAll y HTTPS por defecto. Importa el certificado en el SO del host si necesitas.
- Dispositivo físico: usa la IP de tu PC, desactiva firewall local o permite el puerto.

## Roadmap

- Persistencia de token (DataStore) + interceptor Authorization.
- Lista real de sucursales y flujo de selección.
- Integración de Carrito/Órdenes/Tarjetas.
- Limpieza de clases legadas que no se usen.

## Licencia

Uso interno/privado. Ajusta la licencia según necesites.