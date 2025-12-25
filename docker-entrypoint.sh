#!/bin/bash
set -e

# Default values
PUID=${PUID:-1000}
PGID=${PGID:-1000}

# Create group if it doesn't exist
if ! getent group appgroup > /dev/null 2>&1; then
    groupadd -g "$PGID" appgroup
fi

# Create user if it doesn't exist
if ! getent passwd appuser > /dev/null 2>&1; then
    useradd -u "$PUID" -g "$PGID" -d /app -s /bin/bash appuser
fi

# Ensure app directory ownership
chown -R "$PUID:$PGID" /app

# Run the application as the specified user
exec gosu appuser dotnet HitsterCardGenerator.dll
