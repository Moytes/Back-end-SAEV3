# Documentación de API y Matriz de Roles (SIAE-V2)

Esta guía detalla los endpoints clave y los permisos asociados para cada rol dentro de la plataforma SIAE-V2.

## 1. Matriz de Responsabilidades por Rol

| Acción | ADMIN | DIRECTOR_USAER | ESPECIALISTA* | TRABAJO_SOCIAL | STUDENT |
| :--- | :---: | :---: | :---: | :---: | :---: |
| Crear Usuarios (Personal) | ✅ | ✅ | ❌ | ❌ | ❌ |
| Crear Cuentas de Alumnos | ✅ | ✅ | ❌ | ❌ | ❌ |
| Crear Escuelas / Grupos | ✅ | ✅ | ❌ | ❌ | ❌ |
| Alta de Alumnos (Padrón) | ✅ | ✅ | ❌ | ✅ | ❌ |
| Inscribir Alumnos a Grupos| ✅ | ✅ | ✅ | ✅ | ❌ |
| Asignar Tareas | ✅ | ✅ | ✅ | ✅ | ❌ |
| Resolver Tareas (Autónoma)| ❌ | ❌ | ❌ | ❌ | ✅ |
| Supervisar / Evaluar | ✅ | ✅ | ✅ | ❌ | ❌ |

*\*Especialistas incluyen: Comunicación, Psicología y Aprendizaje.*

---

## 2. Endpoints de Gestión de Infraestructura y Usuarios

### Infraestructura (`AdminCatalogController`)
*   `POST /api/escuelas`: Crea una nueva institución educativa.
    *   **Roles:** `ADMIN`, `DIRECTOR_USAER`
*   `POST /api/grupos`: Crea un grupo (Grado/Sección) para un ciclo escolar.
    *   **Roles:** `ADMIN`, `DIRECTOR_USAER`

### Usuarios y Asignaciones (`UserController`)
*   `POST /api/usuarios`: Crea un nuevo usuario (Personal o Alumno).
    *   **Nota:** Para alumnos, se requiere enviar `StudentId` y `Role = STUDENT`.
*   `POST /api/usuarios/{id}/escuelas`: Asigna a un directivo/especialista a una escuela.
*   `POST /api/usuarios/{id}/grupos`: Asigna a un docente a un grupo específico.

---

## 3. Área del Estudiante (`StudentAreaController`)

Estos endpoints están diseñados para el acceso directo del alumno. El sistema identifica al alumno automáticamente mediante su token.

*   **Ruta Base:** `api/area-estudiante`
*   **Rol Requerido:** `STUDENT`

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/mis-asignaciones` | Obtiene la lista de tareas asignadas al alumno logueado. |
| `POST` | `/mis-asignaciones/{id}/entregar` | Envía la resolución de una tarea. Valida propiedad. |
| `POST` | `/dialogos/{id}/progreso` | Guarda el avance escena por escena de actividades interactivas. |

---

## 4. Gestión de Alumnos (`StudentController`)

*   `POST /api/alumnos`: Alta de un nuevo alumno en el padrón.
    *   **Roles:** `ADMIN`, `DIRECTOR_USAER`, `TRABAJO_SOCIAL`
*   `POST /api/inscripciones`: Inscribe a un alumno en un ciclo, escuela y grupo.
    *   **Roles:** `ADMIN`, `DIRECTOR_USAER`, `ESPECIALISTAS`, `TRABAJO_SOCIAL`

---

## 5. Seguridad y Auditoría

*   **Aislamiento:** Un alumno (`STUDENT`) no puede acceder a ningún endpoint de gestión de otros alumnos ni ver la lista general de usuarios.
*   **Logs:** Todas las acciones de creación y actualización quedan registradas en la tabla `audit_log` con el ID del usuario que realizó la acción.
*   **Integridad:** No se permite crear más de una cuenta de usuario para el mismo alumno.
