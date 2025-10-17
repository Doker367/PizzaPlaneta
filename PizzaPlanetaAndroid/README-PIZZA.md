# Pizza Planeta Android

Pantalla Home/Login replicada en Jetpack Compose consumiendo Pizza.Backend.

## Endpoints usados

- POST /api/auth/login — devuelve { token }
- GET /api/sucursales — protegido con Bearer

## Base URL

- Emulador: https://10.0.2.2:5001/
- Dispositivo real: https://<IP_PC>:5001/

Si usas HTTP, cambia a 5000 y adapta el código en `getPizzaBaseUrl()`.

## Notas de seguridad

- Cliente OkHttp confía en certificado de desarrollo solo para DEV. Elimina/ajusta para producción.

## Próximos pasos

- Persistir sesión y Authorization interceptor
- Mostrar sucursales reales en selector
- Integrar carrito/órdenes
