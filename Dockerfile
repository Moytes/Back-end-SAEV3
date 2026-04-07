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

# Exponer el puerto (Railway asigna dinámicamente el puerto mediante la variable PORT)
EXPOSE 8080

# Copiar los archivos publicados desde el stage de publish
COPY --from=publish /app/publish .

# Configurar variables de entorno para producción
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Railway proporciona la variable PORT, pero ASP.NET usa ASPNETCORE_URLS
# Este comando permite que la app escuche en el puerto que Railway asigne
ENTRYPOINT ["dotnet", "SIAE-V2.dll"]
