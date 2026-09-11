#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

MAP="$SCRIPT_DIR/map.ascii"
BOOKINGS="$SCRIPT_DIR/bookings.json"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --map)
      MAP="$2"
      shift 2
      ;;
    --bookings)
      BOOKINGS="$2"
      shift 2
      ;;
    *)
      echo "Unknown option: $1" >&2
      exit 1
      ;;
  esac
done

# Convert to absolute path if relative
[[ "$MAP" != /* && "$MAP" != ?:* ]] && MAP="$SCRIPT_DIR/$MAP"
[[ "$BOOKINGS" != /* && "$BOOKINGS" != ?:* ]] && BOOKINGS="$SCRIPT_DIR/$BOOKINGS"

if [[ ! -f "$MAP" ]]; then
  echo "Error: Map file not found: $MAP" >&2
  exit 1
fi
if [[ ! -f "$BOOKINGS" ]]; then
  echo "Error: Bookings file not found: $BOOKINGS" >&2
  exit 1
fi

echo "Map: $MAP"
echo "Bookings: $BOOKINGS"

echo "Starting backend..."
cd "$SCRIPT_DIR/ResortMap.Server"
dotnet run --urls "http://localhost:5012" -- --map "$MAP" --bookings "$BOOKINGS" &
BACKEND_PID=$!

echo "Starting frontend..."
cd "$SCRIPT_DIR/ResortMap.Client"
npm install --silent
npm run dev &
FRONTEND_PID=$!

cleanup() {
  echo "Shutting down..."
  kill $FRONTEND_PID 2>/dev/null
  kill $BACKEND_PID 2>/dev/null
  wait $FRONTEND_PID 2>/dev/null
  wait $BACKEND_PID 2>/dev/null
}
trap cleanup EXIT INT TERM

wait