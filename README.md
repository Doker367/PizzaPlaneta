# Pizza Planeta — Monorepo

Este repositorio contiene 3 piezas principales del proyecto Pizza Planeta:

- Pizza.Backend: API REST en .NET 8 (ASP.NET Core) con MySQL y JWT.
- PizzaPlanetaAndroid: App Android (Jetpack Compose) que replica la UI de Home/Login y consume la API local.
- Pizza (MAUI/Blazor): App .NET MAUI/Blazor usada como referencia visual y futura app multiplataforma.


## Requisitos

- .NET 8 SDK
- MySQL 8.x (o compatible con la cadena de conexión que uses)
- Android Studio o JDK+Android SDK (para compilar la app Android)
- Linux/Mac/Windows


## Estructura

```
Pizza.Backend/           # API ASP.NET Core (JWT, MySQL, Swagger, CORS)
PizzaPlanetaAndroid/     # App Android (Kotlin + Jetpack Compose + Retrofit)
Pizza/                   # App MAUI/Blazor (referencia visual)
```


## 1) Backend (Pizza.Backend)

API REST con autenticación JWT. Carga configuración desde variables de entorno (archivo .env soportado via DotNetEnv).

Variables requeridas (crea un archivo `.env` en `Pizza.Backend/` basado en `.env.sample`):

- DB_CONNECTION_STRING: Cadena de conexión de MySQL.
- JWT_SECRET: Clave secreta para firmar tokens.
- JWT_ISSUER: Emisor aceptado.
- JWT_AUDIENCE: Audiencia aceptada.

Ejemplo de `.env`:

```
DB_CONNECTION_STRING=server=localhost;port=3306;database=pizzadb;user=root;password=tu_password;
JWT_SECRET=super_secreto_largo_y_aleatorio
JWT_ISSUER=pizzaplaneta
JWT_AUDIENCE=pizzaplaneta.clients
```

Cómo ejecutar en desarrollo:

- Abre una terminal en `Pizza.Backend/` y ejecuta:
  - dotnet restore
  - dotnet run

Por defecto ASP.NET Core expone:

- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

Swagger: https://localhost:5001/swagger

Notas de DB:

- Si no tienes esquema, puedes partir de `Pizza.Backend/PizzaPlaneta.sql` y/o `PizaPlaneta.sql` para crear tablas iniciales, o configurar EF Migrations a tu gusto.


## 2) Android (PizzaPlanetaAndroid)

App Android con Jetpack Compose. Implementa:

- Pantalla Home con modal Login/Registro/Recuperar contraseña
- Login contra `POST /api/auth/login`
- Llamada protegida de prueba a `GET /api/sucursales` con Bearer token

Ajuste de URL base:

- La app usa `10.0.2.2` para acceder al host desde el emulador Android.
- Base URL por defecto: `https://10.0.2.2:5001/`.
- Si ejecutas el backend sin HTTPS, cambia a `http://10.0.2.2:5000/`.
- El helper de red confía en el certificado de desarrollo (solo para DEV).

Archivo relevante:

- `PizzaPlanetaAndroid/app/src/main/java/com/manybox/chofer/ui/PizzaHomeScreen.kt` — UI y flujo de login.
- `PizzaPlanetaAndroid/app/src/main/java/com/manybox/chofer/api/PizzaApi.kt` — Interfaces Retrofit (`login`, `getSucursales`).

Cómo compilar/ejecutar:

- Desde Android Studio: Open > `PizzaPlanetaAndroid/` > Sync > Run sobre un emulador.
- O por terminal:
  - ./gradlew :app:assembleDebug
  - ./gradlew :app:installDebug

Inicio de sesión de prueba:

- Crea un usuario en tu backend o ajusta las credenciales de acuerdo a tu base de datos.
- Si el login es exitoso, se intentará cargar sucursales usando el token recibido.


## 3) App MAUI/Blazor (Pizza)

Proyecto .NET MAUI con componentes Blazor. Actualmente sirve de referencia de UI (por ejemplo, Home) para portar pantallas a Android.


## Solución de problemas

- Certificado HTTPS local: la app Android usa un cliente que confía en el certificado de desarrollo solo en entorno DEV. En producción, debes usar certificados válidos y eliminar ese código.
- CORS: el backend define una política `AllowAll` para facilitar el desarrollo local.
- Puertos: asegura que los puertos 5000/5001 no estén ocupados. Puedes cambiar los puertos en ASP.NET Core si lo necesitas.
- Emulador físico: si pruebas en dispositivo real, reemplaza `10.0.2.2` por la IP de tu PC (por ejemplo, `https://192.168.1.100:5001/`) y ajusta firewall/red.


## Roadmap corto

- Persistir token en Android (DataStore) y usar interceptor para Authorization.
- Consumir Carrito/Ordenes/Tarjetas.
- Lista real de sucursales en el selector de tiendas.
- Limpieza de código legado ManyBox que no se use.


## Licencia

Este proyecto es de uso interno/privado del autor. Ajusta la licencia según tus necesidades.
