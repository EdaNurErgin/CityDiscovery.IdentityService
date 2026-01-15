# ============================================================================
# CityDiscovery Identity Service - Multi-stage Dockerfile
# ============================================================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["IdentityService.csproj", "./"]
RUN dotnet restore "IdentityService.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "IdentityService.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "IdentityService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Copy published files
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "IdentityService.dll"]
