# API de Usuarios

Este módulo gestiona la administración y manipulación de usuarios dentro del sistema, incluyendo roles de Administrador, Barbero y Clientes.

## Endpoints y Roles Permitidos

| Método | Ruta                             | Roles Permitidos       | Descripción                                                        |
| ------ | -------------------------------- | ---------------------- | ------------------------------------------------------------------ |
| GET    | `/api/usuarios`                  | Administrador          | Lista todos los usuarios (excepto clientes).                       |
| GET    | `/api/usuarios/usuarios-sistema` | Administrador, Barbero | Lista usuarios activos del sistema (admins y barberos).            |
| GET    | `/api/usuarios/clientes`         | Administrador, Barbero | Lista clientes.                                                    |
| GET    | `/api/usuarios/{id}`             | Administrador, Barbero | Obtiene detalles de un usuario. Barberos solo pueden ver clientes. |
| POST   | `/api/usuarios`                  | Administrador          | Crea un nuevo usuario.                                             |
| PUT    | `/api/usuarios/{id}`             | Administrador, Barbero | Edita un usuario. Barberos solo pueden modificar su propio perfil. |
| DELETE | `/api/usuarios/{id}`             | Administrador          | Desactiva un usuario (eliminación lógica).                         |
| PATCH  | `/api/usuarios/{id}/estado`      | Administrador          | Cambia estado activo/inactivo de un usuario.                       |
| GET    | `/api/usuarios/perfil`           | Administrador, Barbero | Obtiene el perfil del usuario autenticado.                         |
| PUT    | `/api/usuarios/perfil`           | Administrador, Barbero | Actualiza el perfil del usuario autenticado.                       |

## Detalles de Endpoints

### GET `/api/usuarios`

- **Roles:** Administrador
- **Descripción:** Retorna la lista de todos los usuarios del sistema, excepto los clientes.
- **Respuesta:**

```json
{
  "status": 200,
  "usuarios": [
    {
      "Id": 1,
      "Nombre": "Admin",
      "Email": "admin@example.com",
      "RolNombre": "Administrador",
      ...
    },
    ...
  ]
}
GET /api/usuarios/usuarios-sistema
Roles: Administrador, Barbero

Descripción: Lista usuarios activos que acceden al sistema (administradores y barberos).

Respuesta exitosa:

json
Copiar
Editar
{
  "status": 200,
  "usuarios": [
    {
      "Id": 2,
      "Nombre": "Barbero Juan",
      "Email": "barbero@ejemplo.com",
      "RolId": 2,
      "RolNombre": "Barbero",
      "Activo": true,
      ...
    },
    ...
  ]
}
GET /api/usuarios/clientes
Roles: Administrador, Barbero

Descripción: Lista usuarios con rol cliente (RolId == 3).

Respuesta exitosa:

json
Copiar
Editar
{
  "status": 200,
  "clientes": [
    {
      "Id": 10,
      "Nombre": "Cliente X",
      "Email": "cliente@ejemplo.com",
      "RolId": 3,
      "RolNombre": "Cliente",
      "Activo": true,
      ...
    },
    ...
  ]
}
GET /api/usuarios/{id}
Roles: Administrador, Barbero

Descripción: Obtiene detalles del usuario con id especificado.

Los barberos solo pueden acceder a clientes (RolId == 3).

Parámetros:

id (int): ID del usuario a consultar.

Respuestas:

200 OK:

json
Copiar
Editar
{
  "status": 200,
  "usuario": {
    "Id": 10,
    "Nombre": "Cliente X",
    "Email": "cliente@ejemplo.com",
    "RolId": 3,
    "RolNombre": "Cliente",
    "Activo": true,
    ...
  }
}
403 Forbidden: Si un barbero intenta ver un usuario que no sea cliente.

404 Not Found: Usuario no encontrado.

POST /api/usuarios
Roles: Administrador

Descripción: Crea un nuevo usuario.

Body (CrearUsuarioDto):

json
Copiar
Editar
{
  "Nombre": "string",
  "Email": "string",
  "Telefono": "string",
  "Avatar": "string (URL o base64)",
  "RolId": 1|2|3,
  "AccedeAlSistema": true|false,
  "Password": "string"
}
Notas:

Solo administradores pueden crear usuarios con rol distinto a cliente (RolId != 3).

Si el usuario accede al sistema, la contraseña es obligatoria.

Respuestas:

201 Created: Usuario creado correctamente.

400 Bad Request: Datos inválidos o email duplicado.

401 Unauthorized: Intento de crear usuario con rol prohibido para el creador.

PUT /api/usuarios/{id}
Roles: Administrador, Barbero

Descripción: Actualiza usuario con ID especificado.

Restricciones:

Los barberos solo pueden modificar su propio perfil o usuarios clientes (RolId 3).

Un barbero no puede cambiar su propio rol.

Body (EditarUsuarioDto):

json
Copiar
Editar
{
  "Nombre": "string",
  "Email": "string",
  "Telefono": "string",
  "Avatar": "string",
  "RolId": 1|2|3,
  "AccedeAlSistema": true|false,
  "Activo": true|false,
  "Password": "string (opcional)"
}
Respuestas:

200 OK: Usuario actualizado.

400 Bad Request: Datos inválidos, email duplicado o intento inválido de cambiar rol propio.

401 Unauthorized: Intento de modificar usuario sin permisos.

404 Not Found: Usuario no encontrado.

DELETE /api/usuarios/{id}
Roles: Administrador

Descripción: Desactiva (eliminación lógica) un usuario por ID.

Notas:

No se puede eliminar el propio usuario autenticado.

Respuestas:

200 OK: Usuario marcado como inactivo.

400 Bad Request: Intento de eliminar el propio usuario.

401 Unauthorized: Usuario no autenticado.

404 Not Found: Usuario no encontrado.

PATCH /api/usuarios/{id}/estado
Roles: Administrador

Descripción: Cambia el estado activo/inactivo de un usuario.

Body (CambiarEstadoDto):

json
Copiar
Editar
{
  "Activo": true|false
}
Respuestas:

200 OK: Estado actualizado.

404 Not Found: Usuario no encontrado.

GET /api/usuarios/perfil
Roles: Administrador, Barbero

Descripción: Obtiene perfil del usuario autenticado.

Respuesta:

json
Copiar
Editar
{
  "status": 200,
  "usuario": {
    "Id": 2,
    "Nombre": "Barbero Juan",
    "Email": "barbero@ejemplo.com",
    "Telefono": "123456789",
    "RolNombre": "Barbero",
    "AccedeAlSistema": true,
    "Activo": true,
    "FechaRegistro": "2024-01-01T12:00:00Z",
    "Avatar": "/avatars/2/avatar.jpg"
  }
}
PUT /api/usuarios/perfil
Roles: Administrador, Barbero

Descripción: Actualiza el perfil del usuario autenticado.

Datos recibidos: Form-data con campos:

Nombre (string)

Email (string)

Telefono (string)

Password (string, opcional)

EliminarAvatar (bool, opcional)

Archivo avatar (opcional)

Notas:

Valida email único.

Cambia contraseña si se envía.

Elimina avatar si se solicita.

Guarda y redimensiona nuevo avatar si se sube archivo.

Respuesta:

json
Copiar
Editar
{
  "status": 200,
  "message": "Perfil actualizado correctamente.",
  "usuario": {
    "Id": 2,
    "Nombre": "Barbero Juan",
    "Email": "barbero@ejemplo.com",
    "Telefono": "123456789",
    "RolNombre": "Barbero",
    "AccedeAlSistema": true,
    "Activo": true,
    "FechaRegistro": "2024-01-01T12:00:00Z",
    "Avatar": "/avatars/2/avatar_nuevo.jpg"
  }
}
Errores:

400 Bad Request: Email duplicado.

401 Unauthorized: Usuario no autenticado.

500 Internal Server Error: Error al procesar la solicitud.

Notas generales
Se utiliza la extensión ClaimsPrincipalExtensions.GetUserId() para obtener el ID del usuario autenticado de manera consistente.

Las validaciones y permisos están reforzados internamente en los endpoints para asegurar que los barberos no accedan o modifiquen usuarios fuera de su alcance.

Las respuestas están estandarizadas con un objeto que contiene status y message para facilitar el manejo en frontend.

El manejo de avatar incluye guardado físico en disco con redimensionamiento y eliminación segura.

```
