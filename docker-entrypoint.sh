#!/bin/bash
set -e

# Default values
PUID=${PUID:-1000}
PGID=${PGID:-1000}

# Determine group to use
# First check if a group with this GID already exists
EXISTING_GROUP=$(getent group "$PGID" | cut -d: -f1)
if [ -n "$EXISTING_GROUP" ]; then
    # Use the existing group
    APP_GROUP="$EXISTING_GROUP"
elif ! getent group appgroup > /dev/null 2>&1; then
    # Create appgroup with the specified GID
    groupadd -g "$PGID" appgroup
    APP_GROUP="appgroup"
else
    APP_GROUP="appgroup"
fi

# Determine user to use
# First check if a user with this UID already exists
EXISTING_USER=$(getent passwd "$PUID" | cut -d: -f1)
if [ -n "$EXISTING_USER" ]; then
    # Use the existing user
    APP_USER="$EXISTING_USER"
elif ! getent passwd appuser > /dev/null 2>&1; then
    # Create appuser with the specified UID
    useradd -u "$PUID" -g "$APP_GROUP" -d /app -s /bin/bash appuser
    APP_USER="appuser"
else
    APP_USER="appuser"
fi

# Ensure app directory ownership
chown -R "$PUID:$PGID" /app

# Run the application as the specified user
exec gosu "$APP_USER" dotnet HitsterCardGenerator.dll
