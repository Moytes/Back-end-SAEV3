# ====================================================================
# STAGE 1: Build
# ====================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivo del proyecto y restaurar dependencias
COPY ["SIAE-V2.csproj", "./"]
RUN dotnet restore "SIAE-V2.csproj"

# Copiar el resto del código fuente
COPY . .

# Compilar la aplicación en modo Release
RUN dotnet build "SIAE-V2.csproj" -c Release -o /app/build

# ====================================================================
# STAGE 2: Publish
# ====================================================================
FROM build AS publish
RUN dotnet publish "SIAE-V2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ====================================================================
# STAGE 3: Runtime
# ====================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# 1. Quita el EXPOSE 8080 o déjalo solo como comentario, 
# Railway ignorará este número y usará el que necesite.
# EXPOSE 8080 

# 2. Copia los archivos
COPY --from=publish /app/publish .

# 3. Elimina la variable fija ASPNETCORE_URLS del Dockerfile
# para que no choque con la de Railway.
ENV ASPNETCORE_ENVIRONMENT=Production

# 4. CAMBIO CRUCIAL: Usa el "Shell form" para que reconozca la variable $PORT
# Esto sobreescribe cualquier configuración previa y escucha en el puerto correcto.
ENTRYPOINT ["sh", "-c", "dotnet SIAE-V2.dll --urls http://0.0.0.0:${PORT}"]
