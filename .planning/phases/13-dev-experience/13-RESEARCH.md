# Phase 13: Dev Experience - Research

**Researched:** 2025-12-23
**Domain:** VS Code debugging configuration for .NET + Vite concurrent development
**Confidence:** HIGH

<research_summary>
## Summary

Researched VS Code launch.json and tasks.json configuration patterns for enabling a smooth development workflow with .NET backend and Vite frontend running concurrently. The standard approach uses compound launch configurations to start both servers with F5, background tasks with problem matchers for the Vite dev server, and MSBuild conditions to skip frontend builds in Debug mode.

Key finding: The current `NpmBuild` target runs unconditionally on every build, including Debug. This should be gated with `Condition="'$(Configuration)' == 'Release'"` to enable fast debug cycles where Vite serves the frontend with HMR while .NET handles the API.

**Primary recommendation:** Add compound launch config with background Vite task using `activeOnStart` problem matcher. Gate NpmBuild to Release-only. Document all configuration options for future maintainers.
</research_summary>

<standard_stack>
## Standard Stack

### Core (VS Code Configuration)
| Feature | Purpose | Why Standard |
|---------|---------|--------------|
| Compound launch | Start multiple debug sessions with F5 | Built-in VS Code feature, well-documented |
| Background tasks | Keep Vite running alongside .NET | Required for long-running dev servers |
| Problem matcher | Signal when background task is "ready" | Required for preLaunchTask with background tasks |
| serverReadyAction | Auto-open browser when server starts | Built-in, no extensions needed |

### Supporting (MSBuild)
| Feature | Purpose | When to Use |
|---------|---------|-------------|
| `$(Configuration)` condition | Skip npm build in Debug mode | When frontend is served by Vite in development |
| BeforeTargets="Build" | Hook custom targets into build pipeline | Standard pattern for pre-build steps |

### Already Configured (Project Context)
| Component | Port | Purpose |
|-----------|------|---------|
| .NET API | 5657 | Backend serving API endpoints |
| Vite dev server | 5173 | Frontend with HMR, proxies /api to 5657 |
| wwwroot | - | Static files output for Release builds |
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Pattern 1: Compound Launch Configuration
**What:** Single F5 keypress starts both .NET debugger and Vite dev server
**When to use:** Local development where you want debugging + HMR
**Structure:**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET API",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build-debug",
      "program": "${workspaceFolder}/bin/Debug/net10.0/HitsterCardGenerator.dll",
      "cwd": "${workspaceFolder}",
      "env": { "ASPNETCORE_ENVIRONMENT": "Development" }
    },
    {
      "name": "Vite Dev Server",
      "type": "node",
      "request": "launch",
      "runtimeExecutable": "npm",
      "runtimeArgs": ["run", "dev"],
      "cwd": "${workspaceFolder}/web",
      "console": "integratedTerminal"
    }
  ],
  "compounds": [
    {
      "name": "Full Stack (Debug)",
      "configurations": [".NET API", "Vite Dev Server"],
      "stopAll": true
    }
  ]
}
```

### Pattern 2: Background Task with Problem Matcher
**What:** Vite task runs as background process, VS Code knows when it's ready
**When to use:** When using Vite as preLaunchTask or in compound
**Critical:** Background tasks need a problem matcher so VS Code knows the task "finished"
```json
{
  "label": "vite-dev",
  "type": "npm",
  "script": "dev",
  "path": "web",
  "isBackground": true,
  "problemMatcher": {
    "owner": "vite",
    "pattern": { "regexp": "." },
    "background": {
      "activeOnStart": true,
      "beginsPattern": "^.*VITE.*$",
      "endsPattern": "^.*ready in.*$"
    }
  },
  "presentation": {
    "reveal": "silent",
    "panel": "dedicated"
  }
}
```

### Pattern 3: Release-Only npm Build in MSBuild
**What:** NpmBuild only runs when Configuration is Release
**When to use:** Debug builds should be fast; Vite serves frontend in dev
```xml
<Target Name="NpmBuild" BeforeTargets="Build"
        Condition="'$(Configuration)' == 'Release'">
  <Message Importance="high" Text="Building frontend..." />
  <Exec Command="$(NpmCommand) run build" WorkingDirectory="web" />
</Target>
```

### Anti-Patterns to Avoid
- **Running npm build on every Debug build:** Slow feedback loop, defeats purpose of HMR
- **Using `dependsOrder: sequence` without problem matchers:** Task chain may proceed before server ready
- **Hardcoding paths in launch.json:** Use `${workspaceFolder}` for portability
- **Launching Chrome debugger for API work:** Only needed when debugging frontend JS
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Dev server orchestration | Shell scripts to start/stop servers | Compound launch configs | Built-in, debuggable, one-click |
| Build condition logic | Runtime checks in code | MSBuild Condition attribute | Evaluated at build time, zero overhead |
| Browser auto-open | Manual URL navigation | serverReadyAction | Reliable, pattern-matched, configurable |
| Task sequencing | dependsOn without matchers | Problem matchers with patterns | VS Code needs to know when task is "ready" |

**Key insight:** VS Code's debugging infrastructure is highly capable. Compound configs, problem matchers, and serverReadyAction replace custom scripts and manual coordination.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Background Task Never "Finishes"
**What goes wrong:** Debug session doesn't start because preLaunchTask waits forever
**Why it happens:** Background task (dev server) has no exit point
**How to avoid:** Add `isBackground: true` and a problem matcher with `beginsPattern`/`endsPattern`
**Warning signs:** "Waiting for preLaunchTask..." message that never clears

### Pitfall 2: Wrong Configuration Property
**What goes wrong:** Using `"type": "npm"` task with wrong property names
**Why it happens:** Different task types have different property schemas
**How to avoid:** Check official docs; `npm` type uses `script` not `command`
**Warning signs:** Task does nothing or throws cryptic error

### Pitfall 3: HMR Not Working Through Proxy
**What goes wrong:** Vite's hot reload fails when accessed through .NET
**Why it happens:** WebSocket connections not proxied correctly
**How to avoid:** Access Vite directly at port 5173 during development
**Warning signs:** "WebSocket connection failed" in browser console

### Pitfall 4: Debug Launch Opens Production URL
**What goes wrong:** serverReadyAction opens .NET port (5657) instead of Vite (5173)
**Why it happens:** Pattern matches .NET's "Now listening" message
**How to avoid:** Either: (a) remove serverReadyAction for API-only debugging, or (b) configure it to open Vite port
**Warning signs:** Browser shows "no static files" or API JSON instead of UI

### Pitfall 5: NpmInstall Runs Every Build
**What goes wrong:** Slow builds due to npm install checking
**Why it happens:** Condition on NpmInstall not specific enough
**How to avoid:** Use `Condition="!Exists('web/node_modules')"` - only run if missing
**Warning signs:** "npm install" message on every build (already fixed in project)
</common_pitfalls>

<code_examples>
## Code Examples

### Complete tasks.json (Recommended)
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build-debug",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/HitsterCardGenerator.sln",
        "-c", "Debug",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary;ForceNoAlign"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "build-release",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/HitsterCardGenerator.sln",
        "-c", "Release",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary;ForceNoAlign"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "vite-dev",
      "type": "npm",
      "script": "dev",
      "path": "web",
      "isBackground": true,
      "problemMatcher": {
        "owner": "vite",
        "pattern": [{ "regexp": ".", "file": 1, "line": 2, "message": 3 }],
        "background": {
          "activeOnStart": true,
          "beginsPattern": "VITE",
          "endsPattern": "ready in"
        }
      },
      "presentation": {
        "reveal": "silent",
        "panel": "dedicated",
        "close": false
      }
    }
  ]
}
```

### Complete launch.json (Recommended)
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET API (Debug)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build-debug",
      "program": "${workspaceFolder}/bin/Debug/net10.0/HitsterCardGenerator.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "logging": {
        "moduleLoad": false
      }
    },
    {
      "name": "Vite Dev Server",
      "type": "node",
      "request": "launch",
      "runtimeExecutable": "npm",
      "runtimeArgs": ["run", "dev"],
      "cwd": "${workspaceFolder}/web",
      "console": "integratedTerminal",
      "skipFiles": ["<node_internals>/**"]
    },
    {
      "name": ".NET Core Attach",
      "type": "coreclr",
      "request": "attach"
    }
  ],
  "compounds": [
    {
      "name": "Full Stack (F5)",
      "configurations": [".NET API (Debug)", "Vite Dev Server"],
      "stopAll": true,
      "presentation": {
        "hidden": false,
        "group": "debug",
        "order": 1
      }
    }
  ]
}
```

### MSBuild Condition for Release-Only Frontend Build
```xml
<!-- In HitsterCardGenerator.csproj -->
<Target Name="NpmBuild" BeforeTargets="Build"
        Condition="'$(Configuration)' == 'Release'">
  <Message Importance="high" Text="Building frontend (Release only)..." />
  <Exec Command="$(NpmCommand) run build" WorkingDirectory="web" />
</Target>
```
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| npm task type deprecated | npm type still works but `shell` with npm command also valid | N/A | Either approach works |
| Manual browser opening | serverReadyAction with patterns | VS Code 1.30+ | Automatic, configurable |
| launch.json compounds | Unchanged, stable feature | N/A | Reliable pattern |

**New tools/patterns to consider:**
- **VS Code Profiles:** Create a "Development" profile with specific extensions/settings
- **Dev Containers:** Already using; can pre-configure launch.json in devcontainer.json

**Deprecated/outdated:**
- **SpaServices.Extensions UseProxyToSpaDevelopmentServer:** Not needed; Vite's built-in proxy is simpler
- **gulp/grunt task runners:** Replaced by npm scripts in modern projects
</sota_updates>

<open_questions>
## Open Questions

1. **Vite problem matcher pattern reliability**
   - What we know: `beginsPattern: "VITE"` and `endsPattern: "ready in"` match Vite's output
   - What's unclear: May need adjustment for error states or Vite version changes
   - Recommendation: Test with current Vite version; adjust patterns if needed

2. **Should serverReadyAction open Vite port (5173) or be disabled?**
   - What we know: Current config opens .NET port (5657) which shows API, not UI
   - What's unclear: User preference - some want auto-open, some prefer manual
   - Recommendation: Document both options, let user choose in implementation
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [VS Code Debugging Configuration](https://code.visualstudio.com/docs/debugtest/debugging-configuration) - compound configs, serverReadyAction, preLaunchTask
- [VS Code Tasks Documentation](https://code.visualstudio.com/docs/debugtest/tasks) - dependsOn, isBackground, problemMatcher
- [VS Code C# Debugger Settings](https://code.visualstudio.com/docs/csharp/debugger-settings) - .NET-specific launch.json properties
- [MSBuild Conditions](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-conditions) - Configuration condition syntax

### Secondary (MEDIUM confidence)
- [Vite Server Options](https://vite.dev/config/server-options) - proxy configuration
- [VS Code Practical Guide (Aug 2025)](https://www.mykolaaleksandrov.dev/posts/2025/08/vscode-launch-and-tasks-ultimate/) - compound config examples for .NET + Vite

### Tertiary (LOW confidence - needs validation)
- Vite problemMatcher patterns from GitHub discussions - may need adjustment for specific Vite versions
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: VS Code launch.json and tasks.json
- Ecosystem: .NET debugging, Vite dev server, MSBuild conditions
- Patterns: Compound launches, background tasks, problem matchers
- Pitfalls: Task sequencing, HMR proxy issues, build conditions

**Confidence breakdown:**
- Standard stack: HIGH - official VS Code documentation
- Architecture: HIGH - documented compound config patterns
- Pitfalls: HIGH - common issues documented in VS Code GitHub issues
- Code examples: HIGH - synthesized from official docs and verified patterns

**Research date:** 2025-12-23
**Valid until:** 2026-01-23 (30 days - VS Code config is stable)
</metadata>

---

*Phase: 13-dev-experience*
*Research completed: 2025-12-23*
*Ready for planning: yes*
