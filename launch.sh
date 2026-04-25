#!/usr/bin/env bash
# launch.sh — Runs the AvaloniaDemo application (.NET 8)
# Requires a running X display. If none is available, set DISPLAY or use xvfb-run.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

cd "$SCRIPT_DIR"

if [ -z "${DISPLAY:-}" ]; then
    if command -v xvfb-run &>/dev/null; then
        echo "No DISPLAY set — launching with xvfb-run..."
        exec xvfb-run dotnet run --framework net8.0
    else
        echo "Error: DISPLAY is not set and xvfb-run is not available."
        echo "Set DISPLAY or install xvfb: sudo apt-get install -y xvfb"
        exit 1
    fi
else
    echo "Launching AvaloniaDemo (DISPLAY=$DISPLAY)..."
    exec dotnet run --framework net8.0
fi
