# Despliegue en Azure con bajo consumo

## Opcion recomendada: Azure Container Apps

Usa Azure Container Apps cuando quieras que el backend pueda bajar a cero replicas si no hay trafico.

Configuracion sugerida:

- `min replicas`: `0`
- `max replicas`: `1`
- `target port`: `8080`
- `ingress`: habilitado si sera publico
- `health path`: `/health`

Variables de entorno necesarias:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__SupabaseConnection=Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true
Jwt__Issuer=Issuer
Jwt__Audience=Audience
Jwt__SecretKey=...
Jwt__ExpirationInMinutes=1440
Database__ApplyMigrationsOnStartup=false
Cors__AllowedOrigins__0=https://tu-frontend.azurestaticapps.net
```

No actives migraciones automaticas en produccion. Ejecutalas manualmente cuando despliegues cambios de esquema.

## Azure App Service

App Service no escala a cero en todos los planes. Para reducir consumo:

- Usa plan Free/Shared para pruebas.
- Desactiva `Always On`.
- Configura `WEBSITES_PORT=8080` si usas contenedor.
- Usa `/health` como ruta de health check.
- No uses `/health/database` para monitoreo frecuente, porque abre conexion a PostgreSQL.

## Imagen Docker

El contenedor escucha en `WEBSITES_PORT`, `PORT` o `8080` por defecto:

```text
http://+:${WEBSITES_PORT:-${PORT:-8080}}
```

Esto permite usar la misma imagen en Azure App Service, Azure Container Apps y otros hosts.
