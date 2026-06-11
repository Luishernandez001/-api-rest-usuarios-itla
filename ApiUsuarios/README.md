# ApiUsuarios — Práctica 7: Code First + Agregaciones

API REST en ASP.NET Core 8 con autenticación JWT, modelo extendido con Producto, Proveedor y Categoría, y endpoints de agregación.

---

## Tecnologías utilizadas

- ASP.NET Core 8 Web API
- Entity Framework Core 8 + SQL Server
- JWT Bearer Authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Swagger / OpenAPI

---

## Configuración

### Cadena de conexión (`appsettings.json`)

Actualiza el servidor SQL si es necesario:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR\\SQLEXPRESS;Database=ApiUsuariosDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Configuración JWT (`appsettings.json`)

```json
"Jwt": {
  "Key": "C#.NETIntermedio_ClaveSecretaJWT_2026!ApiUsuarios_ITLA",
  "Issuer": "ApiUsuarios",
  "Audience": "ApiUsuariosClientes",
  "ExpirationMinutes": 60,
  "RefreshExpirationMinutes": 1440
}
```

- **ExpirationMinutes**: tiempo de vida del access token (60 minutos por defecto).
- **RefreshExpirationMinutes**: tiempo de vida del refresh token (24 horas por defecto).

---

## Cómo ejecutar

```bash
dotnet run
```

Swagger estará disponible en: `https://localhost:{puerto}/swagger`

---

## Endpoints

### Autenticación (públicos)

#### POST `/api/auth/login`
Autentica al usuario y devuelve un token JWT.

**Body:**
```json
{
  "nombreUsuario": "admin",
  "password": "miPassword123"
}
```

**Respuesta exitosa (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiration": "2026-06-05T14:00:00Z"
}
```

**Error de credenciales (401):**
```json
{ "mensaje": "Credenciales incorrectas." }
```

---

#### POST `/api/auth/refresh`
Renueva el access token usando un refresh token válido.

**Body:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

**Respuesta exitosa (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiration": "2026-06-05T14:00:00Z"
}
```

---

### Usuarios (requieren token JWT) 🔒

Todos los endpoints de `/api/usuarios` están protegidos con `[Authorize]`.  
Debes incluir el header: `Authorization: Bearer {token}`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/usuarios` | Lista todos los usuarios |
| GET | `/api/usuarios/{id}` | Obtiene un usuario por ID |
| POST | `/api/usuarios` | Crea un nuevo usuario |
| PUT | `/api/usuarios/{id}` | Actualiza un usuario |
| DELETE | `/api/usuarios/{id}` | Elimina un usuario |

#### Ejemplo de creación de usuario (POST)

```json
{
  "nombreUsuario": "admin",
  "nombre": "Luis Hernández",
  "correo": "luis@correo.com",
  "fechaDeNacimiento": "2000-01-15T00:00:00",
  "password": "admin123"
}
```

> La contraseña es almacenada encriptada con SHA-256. No se devuelve en ninguna respuesta.

---

## Flujo de prueba en Postman / Swagger

1. **Crear un usuario** — `POST /api/usuarios` con un token de administrador, o crea el primero directamente en la base de datos.
2. **Autenticarse** — `POST /api/auth/login` con `nombreUsuario` y `password`.
3. **Copiar el `token`** de la respuesta.
4. **Usar el token** — En Swagger, haz clic en **Authorize** e ingresa `Bearer {token}`. En Postman, agrega el header `Authorization: Bearer {token}`.
5. **Acceder a endpoints protegidos** — Cualquier endpoint de `/api/usuarios`.
6. **Refrescar el token** — Antes de que expire, usa `POST /api/auth/refresh` con el `refreshToken`.

---

## Validaciones (DataAnnotations)

### Modelo `Usuario`

| Campo | Validaciones |
|-------|-------------|
| `NombreUsuario` | Required, MinLength(3), MaxLength(50), único en BD |
| `Nombre` | Required, MinLength(2), MaxLength(100) |
| `Correo` | Required, EmailAddress, MaxLength(150), único en BD |
| `FechaDeNacimiento` | Required |
| `Password` | Required — se almacena como hash SHA-256 |

### DTO `LoginRequest`

| Campo | Validaciones |
|-------|-------------|
| `NombreUsuario` | Required, MaxLength(50) |
| `Password` | Required, MinLength(6), MaxLength(100) |

---

## Seguridad

- Las contraseñas se encriptan con **SHA-256** antes de almacenarse o compararse.
- Los tokens JWT están firmados con **HMAC-SHA256**.
- El campo `Password` está marcado con `[JsonIgnore]` — nunca se expone en las respuestas de la API.
- El middleware `UseAuthentication()` + `UseAuthorization()` protege automáticamente los endpoints marcados con `[Authorize]`.

---

## Nuevas entidades (Práctica 7)

### Categorías 🔒

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/categorias` | Lista todas las categorías |
| GET | `/api/categorias/{id}` | Obtiene una categoría por ID |
| POST | `/api/categorias` | Crea una nueva categoría |
| PUT | `/api/categorias/{id}` | Actualiza una categoría |
| DELETE | `/api/categorias/{id}` | Elimina una categoría (si no tiene productos) |

**Ejemplo POST:**
```json
{ "id": 0, "nombre": "Electrónica" }
```

---

### Proveedores 🔒

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/proveedores` | Lista todos los proveedores |
| GET | `/api/proveedores/{id}` | Obtiene un proveedor por ID |
| POST | `/api/proveedores` | Crea un nuevo proveedor |
| PUT | `/api/proveedores/{id}` | Actualiza un proveedor |
| DELETE | `/api/proveedores/{id}` | Elimina un proveedor (si no tiene productos) |

**Ejemplo POST:**
```json
{ "id": 0, "nombre": "Tech Supplies SRL", "contacto": "ventas@techsupplies.com" }
```

---

### Productos 🔒

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/productos` | Lista todos los productos (con categoría y proveedor) |
| GET | `/api/productos/{id}` | Obtiene un producto por ID |
| POST | `/api/productos` | Crea un nuevo producto |
| PUT | `/api/productos/{id}` | Actualiza un producto |
| DELETE | `/api/productos/{id}` | Elimina un producto |
| GET | `/api/productos/estadisticas` | Precio más alto, más bajo, suma total y promedio |
| GET | `/api/productos/categoria/{idCategoria}` | Productos de una categoría específica |
| GET | `/api/productos/proveedor/{idProveedor}` | Productos de un proveedor específico |
| GET | `/api/productos/total` | Cantidad total de productos registrados |

**Ejemplo POST:**
```json
{
  "id": 0,
  "nombre": "Laptop HP 15",
  "precio": 850.00,
  "stock": 10,
  "idProveedor": 1,
  "idCategoria": 1
}
```

**Ejemplo respuesta `/api/productos/estadisticas`:**
```json
{
  "productoMasCaro": { "id": 2, "nombre": "MacBook Pro", "precio": 1500.00, ... },
  "productoMasBarato": { "id": 1, "nombre": "Cable USB", "precio": 5.99, ... },
  "sumaTotal": 2355.99,
  "precioPromedio": 785.33
}
```

---

## Validaciones (DataAnnotations) — Nuevas entidades

### Modelo `Categoria`

| Campo | Validaciones |
|-------|-------------|
| `Nombre` | Required, MinLength(2), MaxLength(100) |

### Modelo `Proveedor`

| Campo | Validaciones |
|-------|-------------|
| `Nombre` | Required, MinLength(2), MaxLength(100) |
| `Contacto` | Required, MaxLength(150) |

### Modelo `Producto`

| Campo | Validaciones |
|-------|-------------|
| `Nombre` | Required, MinLength(2), MaxLength(150) |
| `Precio` | Required, Range(0.01, 999999.99) |
| `Stock` | Required, Range(0, MaxValue) |
| `IdProveedor` | Required — debe existir en BD |
| `IdCategoria` | Required — debe existir en BD |
