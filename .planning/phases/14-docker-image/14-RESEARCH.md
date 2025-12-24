# Phase 14: Docker Image - Research

**Researched:** 2025-12-23
**Domain:** .NET 10 Docker containerization with Node.js build tooling
**Confidence:** HIGH

<research_summary>
## Summary

Researched Docker containerization patterns for .NET 10 ASP.NET Core applications with integrated Node.js frontend builds. This is a commodity domain with well-established patterns.

Key finding: .NET 10 introduces a significant change - **Debian images are no longer shipped**. Ubuntu (Noble) is the new default base. The standard approach uses multi-stage builds: SDK image with Node.js for building, aspnet runtime image for production.

The built-in `app` user (UID 1654) from .NET 8+ provides non-root execution by default. Use `USER $APP_UID` in the Dockerfile. Port 8080 is the non-root default (not 80).

**Primary recommendation:** Use multi-stage build with `sdk:10.0` (includes Node.js via apt) for build stage, `aspnet:10.0` for runtime, run as `$APP_UID` user, expose port 8080.
</research_summary>

<standard_stack>
## Standard Stack

### Core Images
| Image | Tag | Purpose | Why Standard |
|-------|-----|---------|--------------|
| mcr.microsoft.com/dotnet/sdk | 10.0 | Build stage | Official .NET 10 SDK for compilation |
| mcr.microsoft.com/dotnet/aspnet | 10.0 | Runtime stage | Minimal ASP.NET Core runtime |

### Alpine Variants (Smaller)
| Image | Tag | Purpose | When to Use |
|-------|-----|---------|-------------|
| mcr.microsoft.com/dotnet/sdk | 10.0-alpine | Build stage | When build time matters less than image size |
| mcr.microsoft.com/dotnet/aspnet | 10.0-alpine | Runtime stage | Smallest runtime (~100MB vs ~220MB) |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| Ubuntu (default) | Alpine | Alpine is smaller but may have glibc compatibility issues |
| aspnet image | runtime-deps + self-contained | Smaller but requires self-contained publish |
| Multi-stage | Single stage | Simpler but bloated image with SDK included |

**Note on Node.js:** The SDK image doesn't include Node.js. Install via apt in the build stage:
```dockerfile
RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*
```
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended Dockerfile Structure
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install Node.js for frontend build
RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*

# Copy and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

### Pattern 1: Layer Ordering for Cache Optimization
**What:** Copy dependency files first, restore, then copy source code
**When to use:** Always - dramatically improves rebuild times
**Example:**
```dockerfile
# Dependencies rarely change - cached
COPY *.csproj ./
COPY web/package*.json ./web/
RUN dotnet restore

# Source changes frequently - rebuilt
COPY . ./
RUN dotnet publish -c Release -o /app/publish
```

### Pattern 2: Non-Root User with $APP_UID
**What:** Use the built-in app user (UID 1654) for security
**When to use:** Always for production
**Example:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
# ... copy files ...
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "App.dll"]
```

### Anti-Patterns to Avoid
- **Using SDK image for runtime:** Bloats image by 1.5GB+, exposes build tools
- **Running as root:** Security vulnerability, not needed for web apps
- **Port 80:** Requires root privileges; use 8080 with non-root user
- **Not using .dockerignore:** Copies bin/, obj/, node_modules/ unnecessarily
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| User creation | Manual useradd | `USER $APP_UID` | Built-in app user already exists in aspnet image |
| Health checks | Custom HTTP scripts | ASP.NET health endpoints | Built-in /healthz pattern with proper status codes |
| Signal handling | Custom trap handlers | Default ENTRYPOINT | .NET handles SIGTERM gracefully by default |
| Port configuration | Manual PORT env var | ASPNETCORE_HTTP_PORTS | Standard .NET 8+ environment variable |

**Key insight:** .NET 8+ Docker images have security and configuration baked in. The $APP_UID environment variable, port 8080 default, and graceful shutdown all work out of the box.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Using Port 80 with Non-Root User
**What goes wrong:** Container fails to start with permission denied
**Why it happens:** Ports below 1024 require root privileges
**How to avoid:** Use port 8080 (default for .NET 8+ non-root), set `EXPOSE 8080`
**Warning signs:** "bind: permission denied" in container logs

### Pitfall 2: Missing .dockerignore
**What goes wrong:** Build context is huge, includes bin/obj/node_modules
**Why it happens:** No .dockerignore means COPY copies everything
**How to avoid:** Create .dockerignore with bin/, obj/, node_modules/, .git/
**Warning signs:** Slow docker build, large context transfer

### Pitfall 3: Secrets in Image Layers
**What goes wrong:** API keys exposed in image history
**Why it happens:** ENV or ARG statements with secrets
**How to avoid:** Pass secrets via environment variables at runtime
**Warning signs:** Secrets visible in `docker history`

### Pitfall 4: Cache Invalidation on Every Build
**What goes wrong:** Full restore every time, slow builds
**Why it happens:** Copying all source before restore
**How to avoid:** Copy *.csproj first, restore, then copy source
**Warning signs:** "Restoring packages..." on every build even when dependencies unchanged
</common_pitfalls>

<code_examples>
## Code Examples

### .dockerignore
```dockerfile
# Source: Docker best practices for .NET
**/bin/
**/obj/
**/node_modules/
**/.git/
**/.vs/
**/.vscode/
**/Dockerfile*
**/.dockerignore
**/*.user
**/*.md
.planning/
.devcontainer/
```

### Environment Variables for Spotify
```dockerfile
# In Dockerfile - document required env vars
ENV ASPNETCORE_HTTP_PORTS=8080

# At runtime (docker run or docker-compose.yml)
# -e SPOTIFY_CLIENT_ID=xxx
# -e SPOTIFY_CLIENT_SECRET=xxx
```

### Health Check Endpoint
```csharp
// Source: ASP.NET Core health checks documentation
// In Program.cs
builder.Services.AddHealthChecks();

// In app configuration
app.MapHealthChecks("/healthz");
```

### Dockerfile HEALTHCHECK
```dockerfile
# Source: Docker documentation
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/healthz || exit 1
```
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| Debian base images | Ubuntu (Noble) | .NET 10 (Nov 2025) | Must use Ubuntu or Alpine, no Debian |
| Port 80 default | Port 8080 default | .NET 8 (Nov 2023) | Non-root works out of box |
| Manual user creation | Built-in $APP_UID | .NET 8 (Nov 2023) | Just use `USER $APP_UID` |
| ASPNETCORE_URLS | ASPNETCORE_HTTP_PORTS | .NET 8 (Nov 2023) | Simpler port configuration |

**New tools/patterns to consider:**
- **Chiseled images:** Ultra-minimal distroless variants (`10.0-noble-chiseled`) - even smaller than Alpine
- **Native AOT:** SDK variants with AOT support (`10.0-noble-aot`) for self-contained apps
- **SHA pinning:** Lock to specific image digest for reproducibility

**Deprecated/outdated:**
- **Debian images:** Not shipped for .NET 10
- **ASPNETCORE_URLS:** Use ASPNETCORE_HTTP_PORTS instead
- **Running as root:** Security anti-pattern, no longer needed
</sota_updates>

<open_questions>
## Open Questions

1. **Health check mechanism**
   - What we know: ASP.NET Core has built-in health check middleware
   - What's unclear: Does this project already have health endpoints?
   - Recommendation: Check during planning; add if missing

2. **curl in runtime image**
   - What we know: HEALTHCHECK can use curl or wget
   - What's unclear: Whether aspnet:10.0 includes curl by default
   - Recommendation: Test during implementation; may need to add package or use different approach
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [Microsoft Learn: Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container) - Multi-stage build pattern, SHA pinning
- [GitHub dotnet-docker: .NET 10 Images Available](https://github.com/dotnet/dotnet-docker/discussions/6801) - Official .NET 10 image tags, Debian discontinuation
- [Microsoft .NET Blog: Secure containers with rootless](https://devblogs.microsoft.com/dotnet/securing-containers-with-rootless/) - Non-root user best practices

### Secondary (MEDIUM confidence)
- [Docker Blog: 9 Tips for .NET Containers](https://www.docker.com/blog/9-tips-for-containerizing-your-net-application/) - General best practices, verified against MS docs
- [Depot: Optimal .NET Dockerfile](https://depot.dev/docs/container-builds/optimal-dockerfiles/dotnet-aspnetcore-dockerfile) - Cache optimization, verified patterns

### Tertiary (LOW confidence - needs validation)
- None - all findings verified against official sources
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: .NET 10 Docker images
- Ecosystem: Multi-stage builds, non-root users, health checks
- Patterns: Layer caching, .dockerignore, environment variables
- Pitfalls: Port 80, missing .dockerignore, secrets in layers

**Confidence breakdown:**
- Standard stack: HIGH - official Microsoft images, verified
- Architecture: HIGH - documented patterns from Microsoft
- Pitfalls: HIGH - common issues documented in official guides
- Code examples: HIGH - from official documentation

**Research date:** 2025-12-23
**Valid until:** 2026-01-23 (30 days - Docker patterns are stable)
</metadata>

---

*Phase: 14-docker-image*
*Research completed: 2025-12-23*
*Ready for planning: yes*
