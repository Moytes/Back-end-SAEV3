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

# 1. Exponer puerto estándar de .NET 8+
EXPOSE 8080 

# 2. Copia los archivos
COPY --from=publish /app/publish .

# 3. Establecer el entorno local y puerto
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# 4. Entrypoint estándar y directo para contenedores .NET
ENTRYPOINT ["dotnet", "SIAE-V2.dll"]
