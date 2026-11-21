#!/usr/bin/env bash
set -e
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$DIR"

if command -v python3 >/dev/null 2>&1; then
    exec python3 "$DIR/DeltaruneModInstaller-Linux.py" "$@"
elif command -v python >/dev/null 2>&1; then
    exec python "$DIR/DeltaruneModInstaller-Linux.py" "$@"
else
    echo "HATA: Python 3 bulunamadi. Lutfen python3 paketini kurun."
    exit 1
fi
