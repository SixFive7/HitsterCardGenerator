# Launcher (`playwright/launch.ps1`)

Parametric PowerShell launcher for the four Playwright MCP servers. Reference doc for [launch.ps1](launch.ps1).

## Files

- [launch.ps1](launch.ps1) — the launcher. Invoked from [../.mcp.json](../.mcp.json) with `-Mode {headless|interactive|tracing|persistent}`. Selects the matching `playwright/<Mode>/config.json` and applies mode-specific pre-launch behaviour, then execs `npx @playwright/mcp@latest`.

## Why one parametric script

Each `.mcp.json` entry needs slightly different launch logic:
- `headless` needs a timestamped per-session `outputDir` so parallel Claude Code sessions don't collide.
- `interactive` and `tracing` need a Windows named mutex acquired before launch to enforce single-instance.
- `persistent` needs nothing extra (Chrome's `SingletonLock` does it).

Rather than four near-duplicate scripts, one parametric script keeps the logic in one place. Adding a new mode or changing common behaviour means editing one file.

## Mode behaviour matrix

| Mode | Pre-launch | npx CLI additions | Mutex name | Notes |
|---|---|---|---|---|
| headless | create `playwright/headless/output/{ts}-{4chr}/` | `--output-dir <session-dir>` | none | parallel-safe |
| interactive | acquire mutex | none | `Global\HitsterCardGenerator-PlaywrightInteractive` | exclusive across all CC sessions |
| tracing | acquire mutex | none | `Global\HitsterCardGenerator-PlaywrightTracing` | independent of interactive |
| persistent | nothing | none | none (Chrome `SingletonLock`) | profile dir is the lock |

## Mutex semantics

`Global\` prefix scopes the mutex system-wide (across user sessions, including services).

`HitsterCardGenerator-` prefix scopes it to this project, so a second Claude Code window in a *different* project that happens to also use these names won't collide.

Windows kernel handles abandoned mutexes automatically: if the holding process crashes, the OS marks the mutex abandoned, and the next acquirer's `WaitOne` throws `AbandonedMutexException` — which the launcher catches and treats as a successful acquire. **No stale-lock cleanup is needed.**

## Debugging

**The MCP server failed to start.** Run the script directly in a terminal to see its actual error:
```powershell
powershell.exe -ExecutionPolicy Bypass -NoProfile -File playwright/launch.ps1 -Mode headless
```
The MCP server is a long-running stdio process — it will sit waiting for protocol messages. Ctrl+C to exit. The actual error (config not found, npx failed, etc.) appears on stderr before that.

**"playwright-interactive is already running"** — another Claude Code session holds the mutex. Either close that session or wait for its server to stop.

**Stale `SingletonLock` on persistent profile** — see [persistent/README.md](persistent/README.md).

**Tracing HAR file empty** — if the browser context didn't close cleanly, the HAR may not have flushed. Reload `network.har` after explicitly closing the browser, or wait a few seconds.

## Adding a new mode

1. Create `playwright/<newmode>/` with `config.json` and `README.md` matching the existing pattern.
2. Add a new case to the `switch` in [launch.ps1](launch.ps1) with whatever pre-launch behaviour the mode needs.
3. Add a new `mcpServers.playwright-<newmode>` entry in [../.mcp.json](../.mcp.json) invoking the launcher with `-Mode <newmode>`.
4. Add a new row to the settings table and the "When to use which" decision rule in [README.md](README.md).

## Why PowerShell instead of Node

Development on this project happens on Windows. Windows named mutexes via `System.Threading.Mutex` are kernel-managed and self-cleaning on process death — no `package.json`, no `node_modules/`, no `proper-lockfile`. The launcher is ~70 lines. (The app itself runs cross-platform via Docker; only this dev tooling is Windows-only.)
