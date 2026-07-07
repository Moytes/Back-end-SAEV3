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

# 1. Exponer puerto (Render inyecta $PORT)
EXPOSE 8080

# 2. Copia los archivos
COPY --from=publish /app/publish .

# 3. Entorno — Render inyecta PORT como variable de entorno
ENV ASPNETCORE_ENVIRONMENT=Production

# 4. Entrypoint con shell para expandir $PORT
CMD ["sh", "-c", "dotnet SIAE-V2.dll --urls http://+:${WEBSITES_PORT:-${PORT:-8080}}"]
