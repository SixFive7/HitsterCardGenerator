# Phase 16: Multi-Architecture & User Docs - Research

**Researched:** 2025-12-24
**Domain:** Docker buildx multi-platform builds in GitHub Actions
**Confidence:** HIGH

<research_summary>
## Summary

Researched the Docker buildx ecosystem for multi-platform image builds in GitHub Actions, specifically targeting AMD64 + ARM64 architectures with GitHub Container Registry (GHCR).

The standard approach uses Docker's official GitHub Actions (`build-push-action@v6`, `setup-buildx-action@v3`, `setup-qemu-action@v3`) with QEMU emulation for cross-platform builds. The existing workflow already has most infrastructure in place - only needs `platforms` parameter and QEMU setup added.

Key finding: Native ARM64 runners (`ubuntu-24.04-arm`, `ubuntu-22.04-arm`) became available for free in public repositories in January 2025. These offer 10-22x faster ARM builds compared to QEMU emulation, but add workflow complexity. For this project's build frequency, QEMU emulation is sufficient and simpler.

**Primary recommendation:** Add QEMU action and platforms parameter to existing workflow. Use single-runner QEMU approach for simplicity - the multi-arch build only runs on releases, so speed is acceptable tradeoff for simplicity.
</research_summary>

<standard_stack>
## Standard Stack

### Core
| Action | Version | Purpose | Why Standard |
|--------|---------|---------|--------------|
| docker/build-push-action | v6 | Build and push Docker images | Official Docker action, full buildx support |
| docker/setup-buildx-action | v3 | Enable buildx with BuildKit | Required for multi-platform builds |
| docker/setup-qemu-action | v3 | Cross-platform emulation | Enables ARM64 builds on AMD64 runners |
| docker/login-action | v3 | Registry authentication | Standard for GHCR login |
| docker/metadata-action | v5 | Generate tags and labels | Already in use, handles semver |

### Supporting
| Action | Version | Purpose | When to Use |
|--------|---------|---------|-------------|
| actions/checkout | v4 | Checkout code | Always needed |
| ffurrer2/extract-release-notes | v2 | Extract CHANGELOG notes | Already in use |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| QEMU emulation | Native ARM runners + matrix | Faster but more complex, overkill for release-only builds |
| Single workflow job | Separate AMD64/ARM64 jobs | Better parallelism but requires manifest merging |
| type=gha cache | type=registry cache | Registry cache persists longer but needs external registry |

**Additions to existing workflow:**
```yaml
# Add after docker/setup-buildx-action step:
- name: Set up QEMU
  uses: docker/setup-qemu-action@v3

# Add to docker/build-push-action:
with:
  platforms: linux/amd64,linux/arm64
```
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended: Simple QEMU Approach

**What:** Single runner builds both architectures via QEMU emulation
**When to use:** Release-only builds, infrequent builds, simplicity preferred
**Why for this project:** Builds only run on version tags, complexity not justified

```yaml
jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - name: Set up QEMU
        uses: docker/setup-qemu-action@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Build and push
        uses: docker/build-push-action@v6
        with:
          platforms: linux/amd64,linux/arm64
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

### Alternative: Native Runners with Matrix

**What:** Separate jobs for each architecture using native runners
**When to use:** Frequent builds, CI time-sensitive, large images
**Example:**

```yaml
jobs:
  build:
    strategy:
      matrix:
        include:
          - platform: linux/amd64
            runner: ubuntu-latest
          - platform: linux/arm64
            runner: ubuntu-24.04-arm
    runs-on: ${{ matrix.runner }}
    steps:
      # Build for single platform, push by digest
      - uses: docker/build-push-action@v6
        with:
          platforms: ${{ matrix.platform }}
          outputs: type=image,name=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }},push-by-digest=true

  merge:
    needs: build
    runs-on: ubuntu-latest
    steps:
      # Merge digests into multi-platform manifest
      - uses: docker/buildx-imagetools-action@v3
```

**Note:** Native ARM runners require `ubuntu-24.04-arm` or `ubuntu-22.04-arm` labels. Only available in **public repositories** (free preview) or Team/Enterprise plans (private repos).

### Anti-Patterns to Avoid
- **Building without QEMU setup:** ARM64 builds will fail on AMD64 runners
- **Using `ubuntu-latest-arm`:** This label doesn't exist
- **Caching per-architecture separately:** Complicates cache management for little benefit in simple builds
- **Push + load:** Cannot load multi-platform images locally without containerd image store
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Multi-arch build scripts | Shell scripts with docker buildx | docker/build-push-action | Handles all edge cases, auth, caching |
| QEMU installation | Manual binfmt setup | docker/setup-qemu-action | Idempotent, cached, version managed |
| Manifest creation | docker manifest create | docker/build-push-action (automatic) | One action handles it all |
| Cache management | DIY cache restore/save | type=gha built-in | Integrated with buildx, handles scopes |
| Tag generation | Custom shell scripts | docker/metadata-action | Already in use, handles semver patterns |

**Key insight:** The Docker Actions ecosystem is mature and handles all edge cases. The entire multi-arch setup is 3 lines added to an existing workflow. Don't create custom scripts.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Missing QEMU for ARM64
**What goes wrong:** ARM64 build fails with "exec format error"
**Why it happens:** AMD64 runner can't execute ARM64 instructions without emulation
**How to avoid:** Always include `docker/setup-qemu-action` before `docker/setup-buildx-action`
**Warning signs:** Build works for AMD64 but fails for ARM64

### Pitfall 2: GitHub Actions Cache API v2 Migration
**What goes wrong:** Error "This legacy service is shutting down"
**Why it happens:** GHA cache API v1 deprecated April 15, 2025
**How to avoid:** Use docker/build-push-action@v6 and recent buildx versions
**Warning signs:** Cache-related errors starting April 2025

### Pitfall 3: Cache Scope Collisions
**What goes wrong:** Multi-image builds overwrite each other's cache
**Why it happens:** Default scope is "buildkit" for all builds
**How to avoid:** Add `scope` parameter if building multiple images
**Warning signs:** Cache hit rate drops, unexpected rebuilds
**Note:** Not an issue for this project (single image)

### Pitfall 4: ARM Native Runner Labels
**What goes wrong:** Workflow queued indefinitely or fails
**Why it happens:** Using wrong labels or in private repos without enterprise plan
**How to avoid:** Use `ubuntu-24.04-arm` or `ubuntu-22.04-arm` only in public repos
**Warning signs:** Jobs stuck in "Queued" state, label errors

### Pitfall 5: Large Image Cache Limits
**What goes wrong:** Cache eviction, slow builds after cache miss
**Why it happens:** GitHub cache limited to 10GB per repo
**How to avoid:** Use `mode=max` for important layers, consider registry cache for huge images
**Warning signs:** Cache size warnings in logs, frequent full rebuilds
</common_pitfalls>

<code_examples>
## Code Examples

Verified patterns from official sources:

### Complete Multi-Platform Workflow Addition
```yaml
# Source: https://docs.docker.com/build/ci/github-actions/multi-platform/
# Add these steps to existing release.yml

- name: Set up QEMU
  uses: docker/setup-qemu-action@v3

- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3

- name: Build and push Docker image
  uses: docker/build-push-action@v6
  with:
    context: .
    push: true
    platforms: linux/amd64,linux/arm64  # <-- Add this line
    tags: ${{ steps.meta.outputs.tags }}
    labels: ${{ steps.meta.outputs.labels }}
    cache-from: type=gha
    cache-to: type=gha,mode=max
```

### Docker Compose - Simple (Direct Ports)
```yaml
# Source: User pattern preference from CONTEXT.md
services:
  hitster:
    image: ghcr.io/sixfive7/histercardgenerator:latest
    container_name: hitster
    restart: unless-stopped
    ports:
      - "8080:8080"
    environment:
      SPOTIFY_CLIENT_ID: your_client_id_here
      SPOTIFY_CLIENT_SECRET: your_client_secret_here
```

### Docker Compose - Traefik Labels (No Port Exposure)
```yaml
# Source: User's example.yaml pattern
services:
  hitster:
    image: ghcr.io/sixfive7/histercardgenerator:latest
    container_name: hitster
    restart: unless-stopped
    environment:
      SPOTIFY_CLIENT_ID: your_client_id_here
      SPOTIFY_CLIENT_SECRET: your_client_secret_here
    labels:
      traefik.enable: true
      traefik.http.services.hitster.loadbalancer.server.port: 8080
```
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| QEMU only for ARM | Native ARM runners available | Jan 2025 | 10-22x faster ARM builds (optional) |
| docker/build-push-action@v5 | v6 | 2024 | Better BuildKit integration |
| Cache API v1 | Cache API v2 required | Apr 2025 | Must use recent action versions |
| docker manifest create | Automatic in build-push-action | 2023+ | No manual manifest handling needed |

**New tools/patterns to consider:**
- **Native ARM runners:** `ubuntu-24.04-arm` and `ubuntu-22.04-arm` for public repos (free preview)
- **Registry cache:** `type=registry` for cross-branch caching (not needed for simple projects)

**Deprecated/outdated:**
- **docker manifest commands:** build-push-action handles manifests automatically
- **Self-hosted ARM runners:** Native runners now available for public repos
- **Cache API v1:** Shut down April 15, 2025
</sota_updates>

<open_questions>
## Open Questions

None - the domain is well-documented and straightforward for this use case.

The main decision (QEMU vs native runners) is clear: QEMU is simpler and sufficient for release-only builds.
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [Docker Multi-Platform GitHub Actions](https://docs.docker.com/build/ci/github-actions/multi-platform/) - Complete workflow structure
- [docker/build-push-action](https://github.com/docker/build-push-action) - v6 inputs, cache options
- [docker/setup-buildx-action](https://github.com/docker/setup-buildx-action) - v3 configuration
- [docker/setup-qemu-action](https://github.com/docker/setup-qemu-action) - v3 platform support
- [GitHub Actions Cache Backend](https://docs.docker.com/build/cache/backends/gha/) - Cache configuration

### Secondary (MEDIUM confidence)
- [GitHub Blog: ARM64 on GitHub Actions](https://github.blog/2024-06-03-arm64-on-github-actions-powering-faster-more-efficient-build-systems/) - Native runner announcement
- [GitHub Changelog: Linux ARM64 Runners](https://github.blog/changelog/2025-01-16-linux-arm64-hosted-runners-now-available-for-free-in-public-repositories-public-preview/) - Public repo availability

### Tertiary (LOW confidence - needs validation)
- None - all findings verified against official documentation
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: Docker buildx with GitHub Actions
- Ecosystem: Docker official actions suite
- Patterns: QEMU emulation vs native runners
- Pitfalls: Cache, QEMU setup, runner labels

**Confidence breakdown:**
- Standard stack: HIGH - official Docker actions, documented patterns
- Architecture: HIGH - verified with Docker documentation
- Pitfalls: HIGH - documented in official sources and community discussions
- Code examples: HIGH - from Docker official documentation

**Research date:** 2025-12-24
**Valid until:** 2026-03-24 (90 days - Docker Actions ecosystem stable, infrequent releases)
</metadata>

---

*Phase: 16-multi-arch-docs*
*Research completed: 2025-12-24*
*Ready for planning: yes*
