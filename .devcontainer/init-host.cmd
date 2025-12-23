@echo off
REM Cross-platform initializeCommand for devcontainers
REM
REM This .cmd file runs on Windows via: cmd /c .devcontainer\init-host.cmd
REM Unix/Mac/WSL use init-host via: sh .devcontainer/init-host
REM
REM Workaround for: https://github.com/devcontainers/spec/issues/347
REM (OS-specific initializeCommand not yet supported in devcontainer spec)
REM
REM Creates the bind mount source directories/files on the host before container starts.

REM Create .claude directory for settings, history, plugins, etc.
if not exist ".devcontainer\mounts\.claude" md ".devcontainer\mounts\.claude"

REM Create .claude.json file for onboarding state (API acknowledgments, theme, tips)
REM Docker requires the file to exist before bind-mounting
if not exist ".devcontainer\mounts\.claude.json" echo {}> ".devcontainer\mounts\.claude.json"

REM Create VS Code "Remote" settings file (Settings > Remote tab)
REM Persists ALL Remote-scoped settings, not just Claude Code
if not exist ".devcontainer\mounts\.vscode-machine-settings.json" echo {}> ".devcontainer\mounts\.vscode-machine-settings.json"
