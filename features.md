# Features - estado y backlog

## Vision de producto

ComentApp conecta usuarios y creadores. Un usuario autenticado elige un creador, escribe un comentario, define un monto y paga via Mercado Pago. El creador recibe el dinero y ve el comentario en tiempo real.

No es:

- plataforma de suscripciones
- gestor de contenido
- mensajeria privada
- procesador de pagos fuera de Mercado Pago
- marketplace multi-creador por transaccion
- panel financiero/analytics avanzado

## Features existentes

### Landing publica

Estado: parcial/done.

Frontend tiene `HomePage` con hero simple y cards. No requiere auth.
Hay deuda de copy: texto actual habla de comentar/conectar, no de donaciones a creadores.
Hay links rotos a `/comentarios` y `/about`.

### Registro

Estado: funcional base.

Frontend:

- formulario con Formik + Yup
- envia `name`, `surname`, `userName`, `email`, `password`, `confirmPassword`

Backend:

- valida email duplicado
- valida username duplicado
- hashea password
- crea usuario
- envia email de confirmacion

### Confirmacion de email

Estado: funcional base.

Frontend:

- ruta `/confirm-email`
- lee `email` y `token` desde query string
- muestra loading/success/error

Backend:

- valida token
- marca `IsEmailConfirmed = true`

### Login/logout/sesion

Estado: funcional base.

Frontend:

- login con Axios
- `me` para hidratar usuario
- logout
- interceptor de refresh ante 401
- evento global de sesion expirada

Backend:

- auth local por email/password
- exige email confirmado
- cookies HTTP-only
- refresh token con rotacion
- logout revoca refresh token

### Modelo de creador

Estado: modelo/migracion existen, feature no expuesta.

Backend tiene entidad `Creator`, migracion e indices.
Falta servicio, repositorio especifico, endpoint y contrato frontend.

### Modelo de comentarios

Estado: modelo/migracion existen, endpoint dummy.

Backend tiene entidad `Comment` y migracion.
`CommentsController.POST` solo responde eco, no guarda en DB.
Frontend tiene `CommentsPage` con formulario inicial, pero no llama backend.

### Pantalla "Be Creator"

Estado: UI inicial.

Frontend tiene ruta protegida `/be-creator`.
Usuario acepta terminos y dispara `POST /be-a-creator`.
Backend no implementa ese endpoint.

### Settings

Estado: placeholder.

Pantalla existe, pero no tiene modo creador ni Mercado Pago.

### Explore

Estado: placeholder.

## Features a implementar

### 1. Alinear contrato de usuario

Prioridad: alta.

Necesario para auth, roles y rutas protegidas.

Tareas:

- Backend `GET /Authentication/me` debe devolver `id`, `name`, `surname`, `userName`, `email`, `isCreator`.
- Frontend `User` debe reflejar ese contrato.
- Actualizar `AuthContext` para guardar usuario completo.
- Definir si `isCreator` viene de tabla `Creators` o campo en `User`.

### 2. Activacion de rol creador

Prioridad: alta.

Objetivo:
usuario autenticado activa/desactiva modo creador de forma explicita.

Backend sugerido:

- `POST /Creators` o `POST /Creator/me`
- `GET /Creators/me`
- `PATCH /Creators/me`
- opcional `DELETE /Creators/me` o `PATCH active=false`

Datos minimos:

- creatorName
- userId desde sesion
- MercadoPagoAccount cuando MP este conectado
- descripcion/links opcionales

Frontend:

- mover flujo real a `/settings`
- mostrar estado activo/inactivo
- toggle o accion explicita
- toast de exito/error
- actualizar usuario local
- si activa por primera vez, pedir conectar Mercado Pago

Nota:
`/be-creator` puede quedar como onboarding, pero debe usar endpoint real.

### 3. Conexion Mercado Pago

Prioridad: alta para producto.

Objetivo:
creador conecta cuenta de Mercado Pago para recibir pagos.

Backend pendiente:

- OAuth/connect de Mercado Pago
- guardar account/user id/token segun integracion elegida
- endpoint de estado de conexion
- endpoint de desconexion

Frontend pendiente:

- seccion "Mercado Pago" en settings
- boton conectar/desconectar
- estado conectado/pendiente/error
- callbacks visuales

Specs existentes:

- `spec/features/feature-mercadopago-connect.md`
- `spec/features/feature-mercadopago-callbacks.md`

### 4. Formulario de donacion con comentario

Prioridad: alta.

Objetivo:
usuario autenticado elige creador, monto y comentario; backend genera preferencia de Mercado Pago; frontend redirige a checkout.

Frontend actual:

- `CommentsPage` ya tiene creador, comentario y precio.
- creadores hardcodeados.
- submit solo loguea.

Cambios frontend:

- reemplazar lista hardcodeada por endpoint de creadores.
- validar monto mayor a 0.
- llamar endpoint backend que crea preferencia.
- redirigir a checkout URL.
- manejar loading/error.

Backend pendiente:

- endpoint para listar/buscar creadores.
- endpoint para crear preferencia Mercado Pago.
- persistir intento de donacion/comentario como pendiente.
- asociar usuario, creador, monto, comentario y payment/preference id.

### 5. Callbacks de Mercado Pago

Prioridad: alta.

Objetivo:
actualizar estado de pago y comentario segun resultado.

Estados requeridos:

- success
- pending
- failure

Backend pendiente:

- webhook o endpoint de notificacion Mercado Pago.
- endpoint de retorno si aplica.
- validacion server-side del pago.
- marcar donacion/comment como pagado, pendiente o fallido.

Frontend pendiente:

- paginas o vista de resultado.
- feedback claro al usuario.
- no confiar solo en query params de retorno.

### 6. Dashboard creador realtime

Prioridad: media, depende de pagos end-to-end.

Objetivo:
creador ve comentarios recibidos y monto donado en tiempo real.

Backend pendiente:

- SignalR hub.
- emitir evento cuando pago confirmado crea comentario visible.
- autorizacion para creator owner.

Frontend pendiente:

- ruta `/dashboard` protegida por `user.isCreator === true`.
- hook `useSignalR` con estados `Connecting`, `Connected`, `Disconnected`, `Reconnecting`.
- cards de comentarios recibidos.
- marcar comentario como leido en UI.

### 7. Persistencia real de comentarios

Prioridad: alta, junto con donacion.

Backend:

- agregar `DbSet<Comment>` y repositorio/servicio.
- validar `CommentText <= 300`.
- validar creator existe.
- usar `UserId` desde sesion, no desde body.
- guardar `CreatedAt`.

Frontend:

- enviar DTO alineado con backend.
- no permitir submit sin auth.

### 8. Busqueda/exploracion de creadores

Prioridad: media.

Objetivo:
usuario encuentra creadores reales.

Backend:

- endpoint `GET /Creators?query=...`
- paginacion simple
- datos publicos: id, creatorName, descripcion, links, avatar futuro, estado MP conectado

Frontend:

- completar `ExplorePage`
- usar combobox/lista en donation form desde datos reales

## Orden recomendado

1. Alinear contrato `me` y `User`.
2. Implementar creator backend + `isCreator`.
3. Completar settings/modo creador.
4. Integrar Mercado Pago connect.
5. Implementar donation form end-to-end.
6. Implementar callbacks/webhooks.
7. Persistir/mostrar comentarios confirmados.
8. Agregar dashboard realtime con SignalR.

## Contratos a definir antes de avanzar

### Usuario autenticado

```json
{
  "id": 1,
  "name": "Ada",
  "surname": "Lovelace",
  "userName": "ada",
  "email": "ada@example.com",
  "isCreator": false
}
```

### Crear preferencia de donacion

```json
{
  "creatorId": 123,
  "comment": "Gran stream",
  "amount": 1500
}
```

Respuesta:

```json
{
  "preferenceId": "mp-pref-id",
  "checkoutUrl": "https://..."
}
```

### Comentario recibido para dashboard

```json
{
  "id": 456,
  "comment": "Gran stream",
  "amount": 1500,
  "fromUserName": "ada",
  "createdAt": "2026-07-04T19:00:00Z",
  "paymentStatus": "approved",
  "isRead": false
}
```

## Deuda tecnica transversal

- Arreglar encoding de textos visibles y specs existentes.
- Agregar tests frontend/backend.
- Definir naming estable: `price`, `amount`, `monto`.
- Evitar rutas duplicadas.
- Alinear casing de rutas backend (`Authentication` vs `authentication`) y frontend.
- No guardar secretos reales en `appsettings.json`; usar user secrets/env vars para ambientes reales.
