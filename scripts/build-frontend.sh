#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
FRONTEND_DIR="$REPO_ROOT/frontend"
RESOURCES_DIR="$REPO_ROOT/native/Skyjo/data/resources"
OUTPUT_DIR="$REPO_ROOT/native/Skyjo/data"
STAGING_DIR="$(mktemp -d)"

cleanup() {
    rm -rf "$STAGING_DIR"
}
trap cleanup EXIT

echo "Building frontend..."
(cd "$FRONTEND_DIR" && npm run build)

echo "Staging files..."
cp -r "$FRONTEND_DIR/dist/." "$STAGING_DIR/"
cp -r "$RESOURCES_DIR" "$STAGING_DIR/resources"

echo "Creating ui.vpk..."
python "$REPO_ROOT/scripts/vpk_pack.py" "$STAGING_DIR" "$OUTPUT_DIR/ui.vpk"

echo "Done — $OUTPUT_DIR/ui.vpk"
