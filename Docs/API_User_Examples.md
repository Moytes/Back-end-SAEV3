# USER API - Ejemplos de Uso

## Endpoint: Crear Usuario

**URL:** `POST /api/user`

**Headers:**
```
Content-Type: application/json
```

### Ejemplo 1: Crear Usuario Administrador

```json
{
  "email": "admin@siae.edu.mx",
  "password": "Admin123!@#",
  "name": "Juan",
  "fatherLastName": "Pérez",
  "motherLastName": "García",
  "role": 1,
  "phoneNumber": "+52 1234567890",
  "avatarUrl": "https://example.com/avatars/admin.jpg"
}
```

### Ejemplo 2: Crear Usuario Especialista en Comunicación con Zona Escolar

```json
{
  "email": "comunicacion@siae.edu.mx",
  "password": "SecurePass2024!",
  "name": "María",
  "fatherLastName": "López",
  "motherLastName": "Martínez",
  "role": 4,
  "schoolZoneId": "10000000-0000-0000-0000-000000000001",
  "phoneNumber": "+52 9876543210"
}
```

### Ejemplo 3: Crear Docente

```json
{
  "email": "docente@escuela.edu.mx",
  "password": "Teacher2024#",
  "name": "Carlos",
  "fatherLastName": "Ramírez",
  "role": 8,
  "phoneNumber": "+52 5551234567"
}
```

## Roles Disponibles

| Código | Nombre                         |
|--------|-------------------------------|
| 1      | ADMIN - Administrador          |
| 2      | SUPERVISOR - Supervisor        |
| 3      | DIRECTOR_USAER - Director      |
| 4      | ESPECIALISTA_COM - Comunicación|
| 5      | ESPECIALISTA_PSI - Psicología  |
| 6      | ESPECIALISTA_APR - Aprendizaje |
| 7      | TRABAJO_SOCIAL - Trabajo Social|
| 8      | DOCENTE - Docente              |

## Respuestas

### 201 Created - Usuario creado exitosamente
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "email": "admin@siae.edu.mx",
  "name": "Juan",
  "fatherLastName": "Pérez",
  "motherLastName": "García",
  "role": 1,
  "schoolZoneId": null,
  "phoneNumber": "+52 1234567890",
  "avatarUrl": "https://example.com/avatars/admin.jpg",
  "status": 1,
  "createdAt": "2026-04-07T18:30:00.000Z"
}
```

### 400 Bad Request - Datos inválidos
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "Invalid email format"
    ],
    "Password": [
      "Password must be at least 8 characters"
    ]
  }
}
```

### 409 Conflict - Email ya existe
```json
{
  "error": "A user with email 'admin@siae.edu.mx' already exists."
}
```

### 500 Internal Server Error - Error del servidor
```json
{
  "error": "An error occurred while creating the user. Please try again later."
}
```

## Validaciones

### Email
- Requerido
- Formato de email válido
- Máximo 200 caracteres
- Único en el sistema

### Password
- Requerido
- Mínimo 8 caracteres
- Máximo 100 caracteres
- Se encripta con PBKDF2 (100,000 iteraciones, SHA256)

### Name
- Requerido
- Máximo 100 caracteres

### FatherLastName
- Requerido
- Máximo 100 caracteres

### MotherLastName
- Opcional
- Máximo 100 caracteres

### Role
- Requerido
- Valor numérico entre 1-8

### SchoolZoneId
- Opcional
- Debe existir en la base de datos si se proporciona

### PhoneNumber
- Opcional
- Formato de teléfono válido
- Máximo 20 caracteres

### AvatarUrl
- Opcional
- URL válida

## Ejemplo con cURL

```bash
curl -X POST "http://localhost:5000/api/user" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@siae.edu.mx",
    "password": "Admin123!@#",
    "name": "Juan",
    "fatherLastName": "Pérez",
    "motherLastName": "García",
    "role": 1,
    "phoneNumber": "+52 1234567890"
  }'
```

## Notas de Seguridad

- Las contraseñas se almacenan encriptadas usando PBKDF2 con 100,000 iteraciones
- Se genera un salt único para cada usuario
- Las contraseñas nunca se devuelven en las respuestas
- El hash y salt se almacenan en campos separados en la base de datos
