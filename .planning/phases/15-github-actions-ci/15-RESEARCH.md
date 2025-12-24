# Phase 15: GitHub Actions CI - Research

**Researched:** 2025-12-24
**Domain:** GitHub Actions CI/CD for Docker image publishing to GHCR
**Confidence:** HIGH

<research_summary>
## Summary

Researched the GitHub Actions ecosystem for building and pushing Docker images to GitHub Container Registry (GHCR) on version tag releases. The standard approach uses the Docker-maintained action suite: `docker/login-action`, `docker/metadata-action`, `docker/setup-buildx-action`, and `docker/build-push-action`.

Key finding: The Docker action suite handles authentication, semantic version tagging, multi-platform builds, and layer caching out of the box. GHCR authentication requires explicit `packages: write` permission and uses `GITHUB_TOKEN` directly - no PAT needed.

**Primary recommendation:** Use docker/build-push-action@v6 with docker/metadata-action@v5 for automatic semver tags. Enable GHA cache with `cache-from: type=gha` and `cache-to: type=gha,mode=max`. Use ffurrer2/extract-release-notes@v2 for CHANGELOG extraction.
</research_summary>

<standard_stack>
## Standard Stack

### Core Actions
| Action | Version | Purpose | Why Standard |
|--------|---------|---------|--------------|
| docker/login-action | v3 | GHCR authentication | Official Docker action, handles credential masking |
| docker/metadata-action | v5 | Generate tags/labels from git refs | Automatic semver + latest + SHA tagging |
| docker/setup-buildx-action | v3 | Create BuildKit builder | Required for cache and multi-arch support |
| docker/build-push-action | v6 | Build and push image | Official, supports BuildKit features |

### Supporting Actions
| Action | Version | Purpose | When to Use |
|--------|---------|---------|-------------|
| actions/checkout | v4 | Check out code | Always - needed for Dockerfile context |
| actions/setup-dotnet | v5 | .NET SDK with NuGet cache | If running dotnet build/test before Docker build |
| ffurrer2/extract-release-notes | v2 | Extract notes from CHANGELOG.md | Auto-populating GitHub Release body |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| docker/metadata-action | Manual tag logic | Metadata handles edge cases (latest, semver parsing) |
| GHA cache | Registry cache | Registry cache has no 10GB limit but slightly slower |
| extract-release-notes | Custom AWK script | Action handles escaping, edge cases |

**Workflow Permissions:**
```yaml
permissions:
  contents: write   # For creating GitHub releases
  packages: write   # For pushing to GHCR
```
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended Workflow Structure
```
.github/
└── workflows/
    └── release.yml   # Single workflow for tag-triggered releases
```

### Pattern 1: Tag-Triggered Release Workflow
**What:** Workflow triggers on semantic version tags, builds Docker image, pushes to GHCR, creates GitHub release
**When to use:** Any project publishing Docker images on release
**Example:**
```yaml
# Source: Docker official documentation + GitHub docs
name: Release

on:
  push:
    tags:
      - 'v*.*.*'

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  release:
    runs-on: ubuntu-latest
    permissions:
      contents: write
      packages: write
    steps:
      - uses: actions/checkout@v4

      - name: Log in to GHCR
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=raw,value=latest

      - name: Set up Buildx
        uses: docker/setup-buildx-action@v3

      - name: Build and push
        uses: docker/build-push-action@v6
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

### Pattern 2: Pre-Build Verification
**What:** Run tests and format checks before building Docker image
**When to use:** When verification must pass before publishing
**Example:**
```yaml
# Source: .NET best practices + GitHub Actions docs
jobs:
  verify:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v5
        with:
          dotnet-version: '10.x'
          cache: true
      - run: dotnet restore --locked-mode
      - run: dotnet build --no-restore
      - run: dotnet test --no-build
      - run: dotnet format --verify-no-changes

  release:
    needs: verify
    # ... docker build steps
```

### Pattern 3: GitHub Release with CHANGELOG Notes
**What:** Create GitHub release with notes extracted from CHANGELOG.md
**When to use:** When maintaining Keep a Changelog format
**Example:**
```yaml
# Source: ffurrer2/extract-release-notes docs
- name: Extract release notes
  id: release-notes
  uses: ffurrer2/extract-release-notes@v2

- name: Create GitHub release
  env:
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
  run: |
    gh release create ${{ github.ref_name }} \
      --title "Release ${{ github.ref_name }}" \
      --notes '${{ steps.release-notes.outputs.release_notes }}'
```

### Anti-Patterns to Avoid
- **Manual docker login:** Use docker/login-action to avoid credential leakage in logs
- **Hardcoded image tags:** Use docker/metadata-action for consistent tagging
- **Skipping verification for speed:** Always run tests before publishing
- **Using `ubuntu-latest` without version awareness:** Pin to ubuntu-24.04 if builds are sensitive to runner changes
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Docker tag generation | Shell script with version parsing | docker/metadata-action | Handles latest, semver edge cases (v0.x), SHA tags |
| GHCR authentication | `echo $TOKEN \| docker login` | docker/login-action | Proper credential masking, no log leakage |
| Docker layer caching | Manual cache save/restore | BuildKit GHA cache | `cache-from/to: type=gha` handles everything |
| CHANGELOG parsing | AWK/sed script | ffurrer2/extract-release-notes | Handles escaping, multi-line, edge cases |
| .NET package caching | actions/cache with manual keys | setup-dotnet cache: true | Built-in, uses lock files automatically |
| Version extraction | grep/cut on tag | `${{ github.ref_name }}` | Native GitHub context, always correct |

**Key insight:** GitHub Actions has a mature ecosystem for Docker CI/CD. Every common operation has a maintained action that handles edge cases (credential masking, multi-line escaping, cache invalidation). Custom scripts accumulate bugs that these actions have already fixed.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Missing packages:write Permission
**What goes wrong:** Push fails with "denied: permission_denied: write_package"
**Why it happens:** GITHUB_TOKEN has read-only packages access by default
**How to avoid:** Add `permissions: packages: write` at workflow or job level
**Warning signs:** Error mentions "permission" or "denied"

### Pitfall 2: Legacy GHA Cache Service (Critical - 2025)
**What goes wrong:** Cache operations fail with "legacy service shutting down"
**Why it happens:** GitHub cache API v1 deprecated April 15, 2025
**How to avoid:** Use docker/setup-buildx-action@v3 (auto-updates BuildKit)
**Warning signs:** Cache errors mentioning "legacy" or "v1"

### Pitfall 3: Git Context Ignores Local Changes
**What goes wrong:** .dockerignore changes or pre-build file mutations not reflected
**Why it happens:** docker/build-push-action uses Git context by default (BuildKit fetches from GitHub)
**How to avoid:** Add `context: .` to use local checkout, or always commit changes
**Warning signs:** Build includes files that should be ignored, or missing generated files

### Pitfall 4: CHANGELOG Notes Escaping
**What goes wrong:** Release notes appear garbled or `gh release create` fails
**Why it happens:** Multi-line strings with quotes break shell commands
**How to avoid:** Use extract-release-notes action which handles escaping
**Warning signs:** Truncated notes, shell syntax errors

### Pitfall 5: Cache Scope Conflicts
**What goes wrong:** Multi-image builds overwrite each other's cache
**Why it happens:** Default GHA cache scope is repository-wide
**How to avoid:** Add unique `scope` parameter if building multiple images
**Warning signs:** Cache misses despite no code changes
</common_pitfalls>

<code_examples>
## Code Examples

Verified patterns from official sources:

### Complete Release Workflow (All-in-One)
```yaml
# Source: Docker docs + GitHub docs + extract-release-notes docs
name: Release

on:
  push:
    tags:
      - 'v*.*.*'

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  verify:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v5
        with:
          dotnet-version: '10.x'
          cache: true

      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release
      - run: dotnet test --no-build --configuration Release
      - run: dotnet format --verify-no-changes

  release:
    needs: verify
    runs-on: ubuntu-latest
    permissions:
      contents: write
      packages: write
    steps:
      - uses: actions/checkout@v4

      - name: Log in to GHCR
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract Docker metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=semver,pattern={{version}}
            type=raw,value=latest

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Build and push Docker image
        uses: docker/build-push-action@v6
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max

      - name: Extract release notes
        id: release-notes
        uses: ffurrer2/extract-release-notes@v2

      - name: Create GitHub release
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          gh release create ${{ github.ref_name }} \
            --title "Release ${{ github.ref_name }}" \
            --notes '${{ steps.release-notes.outputs.release_notes }}'
```

### CHANGELOG.md Format (Keep a Changelog)
```markdown
# Changelog

## [2.3.0] - 2025-12-24

Milestone v2.3 brings Docker containerization for easy deployment.

### Added
- Docker image with multi-stage build
- GitHub Actions CI for automated releases
- Health check endpoint

### Changed
- Port changed from 5000 to 8080 for non-root compatibility

### Fixed
- Build integration for Release mode

## [2.2.0] - 2025-12-22

Previous release notes...
```
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| GHA cache v1 API | GHA cache v2 API | April 2025 | Must use setup-buildx-action v3+ |
| docker/build-push-action v5 | v6 | 2024 | New job summaries, better attestations |
| docker/metadata-action v4 | v5 | 2024 | More tag types, better semver handling |
| Manual NuGet cache | setup-dotnet cache: true | 2024 | Built-in with lock file support |
| PAT for GHCR | GITHUB_TOKEN | Established | Simpler, auto-rotating, secure |

**New tools/patterns to consider:**
- **Build attestations:** `provenance: true` adds SLSA metadata for supply chain security
- **Job summaries:** v6 generates detailed build reports automatically
- **Artifact attestations:** GitHub's new attestation feature for signed artifacts

**Deprecated/outdated:**
- **Legacy cache API:** Must migrate before April 2025 deadline
- **docker/login with echo:** Security risk, always use login-action
- **ubuntu-20.04:** Deprecated, use ubuntu-22.04 or ubuntu-24.04
</sota_updates>

<open_questions>
## Open Questions

1. **NuGet Lock Files**
   - What we know: setup-dotnet caching requires packages.lock.json
   - What's unclear: Project doesn't currently generate lock files
   - Recommendation: Either add lock files or use explicit actions/cache with csproj hash

2. **Multi-Architecture (Phase 16)**
   - What we know: buildx supports multi-platform builds
   - What's unclear: ARM64 emulation performance on GitHub runners
   - Recommendation: Research in Phase 16, may need matrix or separate jobs
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [docker/build-push-action](https://github.com/docker/build-push-action) - v6 features, caching syntax
- [docker/metadata-action](https://github.com/docker/metadata-action) - v5 tag patterns, semver
- [docker/login-action](https://github.com/docker/login-action) - v3 GHCR authentication
- [Docker Build GitHub Actions](https://docs.docker.com/build/ci/github-actions/) - Official Docker CI docs
- [GitHub Actions cache | Docker Docs](https://docs.docker.com/build/cache/backends/gha/) - GHA cache backend
- [actions/setup-dotnet](https://github.com/actions/setup-dotnet) - Built-in NuGet caching
- [ffurrer2/extract-release-notes](https://github.com/ffurrer2/extract-release-notes) - v2/v3 CHANGELOG extraction

### Secondary (MEDIUM confidence)
- [Controlling permissions for GITHUB_TOKEN](https://docs.github.com/en/actions/writing-workflows/choosing-what-your-workflow-does/controlling-permissions-for-github_token) - GitHub docs, verified workflow permissions
- [Cache is King - Blacksmith](https://www.blacksmith.sh/blog/cache-is-king-a-guide-for-docker-layer-caching-in-github-actions) - Layer caching strategies, verified against Docker docs

### Tertiary (LOW confidence - needs validation)
- None - all findings verified against official sources
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: GitHub Actions for Docker CI/CD
- Ecosystem: Docker action suite, CHANGELOG actions, .NET caching
- Patterns: Tag-triggered release, verification-first, auto-release-notes
- Pitfalls: Permissions, cache migration, context issues

**Confidence breakdown:**
- Standard stack: HIGH - all from official Docker/GitHub repositories
- Architecture: HIGH - patterns from official documentation
- Pitfalls: HIGH - verified against GitHub community discussions and changelogs
- Code examples: HIGH - composed from verified official examples

**Research date:** 2025-12-24
**Valid until:** 2025-03-24 (90 days - GitHub Actions ecosystem stable, cache API migration complete)
</metadata>

---

*Phase: 15-github-actions-ci*
*Research completed: 2025-12-24*
*Ready for planning: yes*
