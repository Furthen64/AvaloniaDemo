#!/usr/bin/env bash
# checkreq.sh — Checks requirements for .NET 8 / Avalonia development on Ubuntu 24.04

set -euo pipefail

PASS=0
FAIL=0

check() {
    local name="$1"
    local cmd="$2"
    local expected="$3"

    if result=$(eval "$cmd" 2>&1); then
        if echo "$result" | grep -q "$expected"; then
            echo "  [OK]  $name ($result)"
            PASS=$((PASS + 1))
        else
            echo "  [WARN] $name found but version may not match (got: $result, expected: $expected)"
            PASS=$((PASS + 1))
        fi
    else
        echo "  [MISSING] $name — $result"
        FAIL=$((FAIL + 1))
    fi
}

echo "========================================"
echo " Requirement Check — .NET 8 / Avalonia"
echo " Ubuntu 24.04"
echo "========================================"

# --- OS ---
echo ""
echo "[ Operating System ]"
if [ -f /etc/os-release ]; then
    . /etc/os-release
    echo "  Detected: $PRETTY_NAME"
    if [[ "$ID" == "ubuntu" && "$VERSION_ID" == "24.04" ]]; then
        echo "  [OK]  Ubuntu 24.04"
        PASS=$((PASS + 1))
    else
        echo "  [WARN] Expected Ubuntu 24.04, got $PRETTY_NAME"
        FAIL=$((FAIL + 1))
    fi
else
    echo "  [WARN] Cannot detect OS (/etc/os-release not found)"
    FAIL=$((FAIL + 1))
fi

# --- .NET SDK ---
echo ""
echo "[ .NET SDK ]"
check ".NET SDK 8" "dotnet --version" "^8\."

# --- Required libraries for Avalonia on Linux ---
echo ""
echo "[ Native Libraries (Avalonia / X11 / Skia) ]"

libs=(
    "libx11-6"
    "libxext6"
    "libxrender1"
    "libfontconfig1"
    "libgl1"
    "libice6"
    "libsm6"
)

for lib in "${libs[@]}"; do
    if dpkg -s "$lib" &>/dev/null; then
        echo "  [OK]  $lib"
        PASS=$((PASS + 1))
    else
        echo "  [MISSING] $lib  (install with: sudo apt-get install -y $lib)"
        FAIL=$((FAIL + 1))
    fi
done

# --- Optional but recommended ---
echo ""
echo "[ Optional — Display / Debug ]"

optional_bins=(
    "git"
    "xvfb-run"
)

for bin in "${optional_bins[@]}"; do
    if command -v "$bin" &>/dev/null; then
        echo "  [OK]  $bin"
    else
        echo "  [INFO] $bin not found (optional)"
    fi
done

# --- Summary ---
echo ""
echo "========================================"
echo " Summary: $PASS passed, $FAIL failed"
echo "========================================"

if [ "$FAIL" -gt 0 ]; then
    echo " Some requirements are missing."
    echo " Run the following to install them:"
    echo "   sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0 \\"
    echo "     libx11-6 libxext6 libxrender1 libfontconfig1 libgl1 libice6 libsm6"
    exit 1
else
    echo " All required dependencies are present."
    exit 0
fi
