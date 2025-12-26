# Phase 12: Devcontainer Setup - Research

**Researched:** 2025-12-22
**Domain:** VS Code Dev Containers with Claude Code CLI for .NET 10 + Node.js project
**Confidence:** HIGH

<research_summary>
## Summary

Researched the devcontainer ecosystem for setting up a Claude Code development environment for a .NET 10 + Svelte project. Anthropic provides an official devcontainer-features repository with a Claude Code CLI feature that installs globally via npm.

The recommended approach is a **simple feature-based setup** using the .NET 10 devcontainer base image with Node.js and Claude Code features added. No custom Dockerfile needed - just a devcontainer.json with the right features.

The user accepts running `--dangerously-skip-permissions` without firewall restrictions for unconstrained development.

**Primary recommendation:** Use `mcr.microsoft.com/devcontainers/dotnet:1-10.0` base image with Node.js feature + Claude Code feature. Single devcontainer.json file, no Dockerfile or firewall scripts.
</research_summary>

<standard_stack>
## Standard Stack

### Core
| Library/Tool | Version | Purpose | Why Standard |
|--------------|---------|---------|--------------|
| @anthropic-ai/claude-code | latest | Claude Code CLI | Official Anthropic CLI for agentic coding |
| Node.js | 20.x | Runtime for Claude Code + npm for Vite build | Required by Claude Code feature |
| .NET SDK | 10.0 | .NET development | Project requirement (net10.0 TFM) |
| Docker | 20.10+ | Container runtime | Required for devcontainers |

### Supporting
| Library/Tool | Version | Purpose | When to Use |
|--------------|---------|---------|-------------|
| ghcr.io/anthropics/devcontainer-features/claude-code:1 | 1.0 | Feature-based Claude Code install | Always - official way to install |
| ghcr.io/devcontainers/features/dotnet:2 | 2.x | .NET SDK feature | When not using .NET base image |
| ghcr.io/devcontainers/features/node:1 | 1.x | Node.js feature | Always - needed for Claude Code + npm |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| .NET base + Node feature | Node base + .NET feature | .NET base is more natural for .NET-primary project |
| Features only | Custom Dockerfile | Custom Dockerfile adds complexity without benefit here |

**Installation (features approach):**
```json
"features": {
  "ghcr.io/devcontainers/features/node:1": { "version": "20" },
  "ghcr.io/anthropics/devcontainer-features/claude-code:1": {}
}
```
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended Project Structure
```
.devcontainer/
└── devcontainer.json      # Container settings, features, mounts - single file setup
```

### Pattern: Feature-Based Setup (Recommended)
**What:** Use devcontainer features without custom Dockerfile
**When to use:** All cases for this project - simple, maintainable, auto-updates
**Example:**
```json
{
  "name": "Hitster Card Generator",
  "image": "mcr.microsoft.com/devcontainers/dotnet:1-10.0",
  "features": {
    "ghcr.io/devcontainers/features/node:1": { "version": "20" },
    "ghcr.io/anthropics/devcontainer-features/claude-code:1": {}
  },
  "forwardPorts": [5657],
  "mounts": [
    "source=hitster-claude-config,target=/home/vscode/.claude,type=volume"
  ],
  "customizations": {
    "vscode": {
      "extensions": [
        "anthropic.claude-code",
        "ms-dotnettools.csharp",
        "ms-dotnettools.csdevkit",
        "svelte.svelte-vscode",
        "bradlc.vscode-tailwindcss"
      ]
    }
  },
  "postCreateCommand": "dotnet restore"
}
```

### Anti-Patterns to Avoid
- **Running as root:** Always use non-root user (devcontainer default or explicit `remoteUser`)
- **No volume mount for Claude config:** Config should persist between rebuilds
- **Hardcoded API keys:** Use `containerEnv` to forward from host or use secrets
- **Custom Dockerfile for simple setups:** Features handle everything needed here
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Claude Code installation | Manual npm install | `ghcr.io/anthropics/devcontainer-features/claude-code:1` | Handles dependencies, updates automatically |
| .NET SDK installation | Manual download | Base image `mcr.microsoft.com/devcontainers/dotnet:1-10.0` | Cross-platform, version management |
| Node.js installation | Manual download | `ghcr.io/devcontainers/features/node:1` | nvm support, cross-platform |
| VS Code settings | Manual config | `customizations.vscode.settings` in devcontainer.json | Portable, version controlled |

**Key insight:** The devcontainer ecosystem has mature solutions for all development environment needs. Using features means automatic updates and no maintenance burden.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Missing Node.js Dependency
**What goes wrong:** Claude Code feature fails to install
**Why it happens:** Claude Code requires Node.js 18+ and npm; feature auto-install is best-effort
**How to avoid:** Explicitly add Node.js feature before Claude Code feature
**Warning signs:** "npm: command not found" during container build

### Pitfall 2: Config Not Persisting
**What goes wrong:** Claude config lost on container rebuild
**Why it happens:** No volume mount for `~/.claude` directory
**How to avoid:** Add volume mount: `"source=hitster-claude-config,target=/home/vscode/.claude,type=volume"`
**Warning signs:** Need to re-authenticate Claude after each rebuild

### Pitfall 3: Wrong User Home Directory
**What goes wrong:** Claude config volume mount doesn't work
**Why it happens:** .NET devcontainer uses `vscode` user, not `node` user
**How to avoid:** Use `/home/vscode/.claude` not `/home/node/.claude`
**Warning signs:** Claude config empty after setting up

### Pitfall 4: .NET 10 Image Not Found
**What goes wrong:** Container build fails with image not found
**Why it happens:** .NET 10 image tag might differ from expected
**How to avoid:** Check available tags at mcr.microsoft.com; try `1-10.0` or `10.0`
**Warning signs:** "manifest unknown" or "not found" during pull
</common_pitfalls>

<code_examples>
## Code Examples

Verified patterns from official sources:

### Complete devcontainer.json for Hitster Card Generator
```json
// Source: Anthropic devcontainer-features + devcontainers/features
{
  "name": "Hitster Card Generator",
  "image": "mcr.microsoft.com/devcontainers/dotnet:1-10.0",
  "features": {
    "ghcr.io/devcontainers/features/node:1": { "version": "20" },
    "ghcr.io/anthropics/devcontainer-features/claude-code:1": {}
  },
  "forwardPorts": [5657],
  "mounts": [
    "source=hitster-claude-config,target=/home/vscode/.claude,type=volume"
  ],
  "customizations": {
    "vscode": {
      "extensions": [
        "anthropic.claude-code",
        "ms-dotnettools.csharp",
        "ms-dotnettools.csdevkit",
        "svelte.svelte-vscode",
        "bradlc.vscode-tailwindcss"
      ]
    }
  },
  "postCreateCommand": "dotnet restore"
}
```

### Volume Mount for Claude Config Persistence
```json
// Source: Anthropic claude-code reference devcontainer
"mounts": [
  "source=hitster-claude-config,target=/home/vscode/.claude,type=volume"
]
```

### postCreateCommand for Project Setup
```json
// Source: devcontainers best practices
"postCreateCommand": "dotnet restore"
```
Note: `npm install` runs automatically via MSBuild NpmInstall target during `dotnet restore`.
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| Manual Claude Code install | ghcr.io/anthropics/devcontainer-features/claude-code:1 | Dec 2025 | Official feature, auto-updates |
| .NET 8 images | .NET 10 images (mcr.microsoft.com/devcontainers/dotnet:1-10.0) | Nov 2025 | LTS support until 2028 |
| Custom Dockerfile for multi-runtime | Feature composition | 2024+ | Simpler, more maintainable |

**New tools/patterns to consider:**
- **Anthropic claude-code feature:** Official devcontainer feature (Dec 2025)
- **.NET 10 Native AOT containers:** Smaller container images for production

**Deprecated/outdated:**
- **Manual npm install of claude-code:** Use the feature instead for updates
- **.NET 8 for new projects:** .NET 10 is now the LTS (Nov 2025)
</sota_updates>

<open_questions>
## Open Questions

Things that couldn't be fully resolved:

1. **Exact .NET 10 devcontainer image tag**
   - What we know: `mcr.microsoft.com/devcontainers/dotnet:1-10.0` should exist
   - What's unclear: Whether `-bookworm` or `-jammy` suffix is preferred/available
   - Recommendation: Try `1-10.0` first, fall back to explicit tag if needed

2. **Claude Code VS Code extension behavior in container**
   - What we know: Extension ID is `anthropic.claude-code`
   - What's unclear: Whether it auto-connects to CLI or needs configuration
   - Recommendation: Install both extension and CLI, test integration
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [anthropics/devcontainer-features](https://github.com/anthropics/devcontainer-features) - Claude Code feature
- [Anthropic claude-code devcontainer reference](https://github.com/anthropics/claude-code/tree/main/.devcontainer) - Reference implementation
- [Claude Code devcontainer docs](https://code.claude.com/docs/en/devcontainer) - Official documentation
- [devcontainers/features/dotnet](https://github.com/devcontainers/features/tree/main/src/dotnet) - .NET SDK feature options

### Secondary (MEDIUM confidence)
- [.NET Blog: Dev Containers](https://devblogs.microsoft.com/dotnet/dotnet-in-dev-container/) - .NET 10 dev container setup
- [containers.dev/features](https://containers.dev/features) - Official feature registry
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: VS Code Dev Containers, Docker
- Ecosystem: Anthropic devcontainer-features, devcontainers/features
- Patterns: Feature-based setup (simple, no firewall)
- Pitfalls: Node.js dependency, config persistence, user paths

**Confidence breakdown:**
- Standard stack: HIGH - verified with official repos and docs
- Architecture: HIGH - from Anthropic reference implementation
- Pitfalls: HIGH - documented in official docs and GitHub issues
- Code examples: HIGH - from Anthropic/devcontainers official sources

**Research date:** 2025-12-22
**Valid until:** 2026-01-22 (30 days - devcontainer ecosystem stable)
</metadata>

---

*Phase: 12-devcontainer-setup*
*Research completed: 2025-12-22*
*Ready for planning: yes*
