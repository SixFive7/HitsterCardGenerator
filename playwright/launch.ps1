<#
.SYNOPSIS
  Parametric launcher for the Playwright MCP server. One script, four modes.

.DESCRIPTION
  Invoked from .mcp.json with -Mode {headless|interactive|tracing|persistent}.
  Each mode loads its own playwright/<Mode>/config.json and applies
  mode-specific behaviour:

    headless    - per-session timestamped outputDir under playwright/headless/output/.
                  Collision-safe for parallel Claude Code sessions.

    interactive - single-instance via Windows named mutex
                  'Global\HitsterCardGenerator-PlaywrightInteractive'. Second concurrent
                  attempt across any Claude Code session fails fast with a clear error.

    tracing     - single-instance via separate named mutex
                  'Global\HitsterCardGenerator-PlaywrightTracing'. Independent of interactive
                  (the two can run side-by-side).

    persistent  - no extra handling. Chrome's SingletonLock on the persistent
                  userDataDir (playwright/persistent/profile/) provides exclusivity.
                  A second concurrent launch surfaces 'Browser is already in use'.

  See playwright/LAUNCHER.md for the full launcher reference, and
  playwright/README.md for the four-server architecture and per-mode settings table.

.PARAMETER Mode
  One of: headless, interactive, tracing, persistent.

.EXAMPLE
  powershell.exe -ExecutionPolicy Bypass -NoProfile -File playwright/launch.ps1 -Mode headless

.NOTES
  Mutexes are Windows kernel objects. If the holding process crashes, the OS marks
  the mutex 'abandoned' and the next acquirer is granted ownership with an
  AbandonedMutexException - which we catch and treat as a successful acquire.
  No stale-lock cleanup is needed.

  Mutex names are scoped to the project ("HitsterCardGenerator-...") so other projects in
  parallel Claude Code windows cannot collide with these locks.
#>

[CmdletBinding()]
param(
  [Parameter(Mandatory = $true, Position = 0)]
  [ValidateSet('headless', 'interactive', 'tracing', 'persistent')]
  [string]$Mode
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Force UTF-8 stdio so multi-byte characters survive the pipe to Claude Code.
$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Resolve repo root from this script's location: launch.ps1 lives in playwright/,
# so go up one level. Defensive: do not assume CWD is the repo root.
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location -Path $RepoRoot

$ConfigPath = "playwright/$Mode/config.json"
if (-not (Test-Path -Path $ConfigPath)) {
  Write-Error "Config not found: $ConfigPath (cwd=$RepoRoot)"
  exit 1
}

# Mode-specific setup
$ExtraArgs = @()
$Mutex = $null

switch ($Mode) {

  'headless' {
    # Per-session outputDir: timestamp + 4-char random suffix.
    # Eliminates collisions when multiple Claude Code sessions run headless in parallel
    # within the same second.
    $Timestamp = Get-Date -Format 'yyyy-MM-dd-HH-mm-ss'
    $Suffix = -join ((48..57) + (97..122) | Get-Random -Count 4 | ForEach-Object { [char]$_ })
    $SessionDir = "playwright/headless/output/$Timestamp-$Suffix"
    New-Item -ItemType Directory -Force -Path $SessionDir | Out-Null
    # CLI flag wins over config (merge order: defaults < configFile < env < CLI).
    $ExtraArgs = @('--output-dir', $SessionDir)
  }

  'interactive' {
    $created = $false
    $Mutex = New-Object System.Threading.Mutex($true, 'Global\HitsterCardGenerator-PlaywrightInteractive', [ref]$created)
    $acquired = $created
    if (-not $acquired) {
      try { $acquired = $Mutex.WaitOne(0) }
      catch [System.Threading.AbandonedMutexException] { $acquired = $true }
    }
    if (-not $acquired) {
      Write-Error 'playwright-interactive is already running in another Claude Code session. Wait for it to finish or close the other session.'
      $Mutex.Dispose()
      exit 1
    }
  }

  'tracing' {
    $created = $false
    $Mutex = New-Object System.Threading.Mutex($true, 'Global\HitsterCardGenerator-PlaywrightTracing', [ref]$created)
    $acquired = $created
    if (-not $acquired) {
      try { $acquired = $Mutex.WaitOne(0) }
      catch [System.Threading.AbandonedMutexException] { $acquired = $true }
    }
    if (-not $acquired) {
      Write-Error 'playwright-tracing is already running in another Claude Code session. Wait for it to finish or close the other session.'
      $Mutex.Dispose()
      exit 1
    }
  }

  'persistent' {
    # No external lock. Chrome's SingletonLock on playwright/persistent/profile/
    # blocks a second concurrent launch and produces a clear error.
  }
}

try {
  & npx -y '@playwright/mcp@latest' --config $ConfigPath --output-mode file @ExtraArgs
}
finally {
  if ($null -ne $Mutex) {
    try { $Mutex.ReleaseMutex() } catch {}
    $Mutex.Dispose()
  }
}
