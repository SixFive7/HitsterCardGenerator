# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install Node.js 24.x LTS for frontend build
RUN apt-get update && apt-get install -y ca-certificates curl gnupg && \
    mkdir -p /etc/apt/keyrings && \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg && \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_24.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list && \
    apt-get update && apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

# Copy dependency files first for cache optimization
COPY *.csproj ./
COPY web/package*.json ./web/

# Restore .NET dependencies
RUN dotnet restore

# Copy remaining source files
COPY . ./

# Build and publish (triggers NpmInstall and NpmBuild MSBuild targets automatically)
RUN dotnet publish HitsterCardGenerator.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install gosu for dropping privileges and bash for entrypoint
RUN apt-get update && apt-get install -y --no-install-recommends gosu \
    && rm -rf /var/lib/apt/lists/*

# Copy published output from build stage
COPY --from=build /app/publish .

# Copy entrypoint script
COPY docker-entrypoint.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Configure port
ENV ASPNETCORE_HTTP_PORTS=8080

# Default environment variables for container customization
ENV TZ=Europe/Amsterdam \
    PUID=1000 \
    PGID=1000

# Required at runtime (pass via docker run -e or docker-compose environment):
# - SPOTIFY_CLIENT_ID: Spotify API client ID
# - SPOTIFY_CLIENT_SECRET: Spotify API client secret

# Expose port for container networking
EXPOSE 8080

# Health check using wget (curl not available in aspnet image)
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/api/health || exit 1

# Start via entrypoint (handles PUID/PGID user creation)
ENTRYPOINT ["docker-entrypoint.sh"]
