# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install Node.js for frontend build (MSBuild triggers npm install/build in Release mode)
RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*

# Copy dependency files first for cache optimization
COPY *.csproj ./
COPY web/package*.json ./web/

# Restore .NET dependencies
RUN dotnet restore

# Copy remaining source files
COPY . ./

# Build and publish (triggers NpmInstall and NpmBuild MSBuild targets automatically)
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Configure port (non-root user requires port >= 1024)
ENV ASPNETCORE_HTTP_PORTS=8080

# Required at runtime (pass via docker run -e or docker-compose environment):
# - SPOTIFY_CLIENT_ID: Spotify API client ID
# - SPOTIFY_CLIENT_SECRET: Spotify API client secret

# Switch to non-root user (built-in 'app' user, UID 1654)
USER $APP_UID

# Expose port for container networking
EXPOSE 8080

# Health check using wget (curl not available in aspnet image)
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/api/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "HitsterCardGenerator.dll"]
