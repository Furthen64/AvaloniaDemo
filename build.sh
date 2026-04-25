#!/usr/bin/env bash
# build.sh — Builds the AvaloniaDemo application (.NET 8)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Building AvaloniaDemo..."
cd "$SCRIPT_DIR"
dotnet build -f net8.0
echo "Build complete."
