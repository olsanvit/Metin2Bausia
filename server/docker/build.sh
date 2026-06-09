#!/bin/bash
# ============================================================
# Metin2Bausia — build a deploy herního serveru
# Spouštět z adresáře: server/docker/
# ============================================================
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SERVER_DIR="$(dirname "$SCRIPT_DIR")"

echo "=== Metin2Bausia — Build game server ==="
echo "Server dir: $SERVER_DIR"
echo ""

# Ověřit source
if [ ! -d "$SERVER_DIR/src" ]; then
  echo "✗ CHYBA: Chybí TMP4 source v server/src/"
  echo "  Zkopíruj TMP4 zdrojové kódy do: $SERVER_DIR/src/"
  exit 1
fi

# Ověřit share data
if [ ! -d "$SERVER_DIR/share" ]; then
  echo "⚠️  VAROVÁNÍ: Chybí herní data v server/share/"
  echo "  Zkopíruj maps, locale, scripts do: $SERVER_DIR/share/"
  echo "  Pokračuji bez nich — server nemusí fungovat správně..."
fi

# Ověřit .env
if [ ! -f "$SCRIPT_DIR/.env" ]; then
  echo "→ Vytvářím .env z .env.example..."
  cp "$SCRIPT_DIR/.env.example" "$SCRIPT_DIR/.env"
  echo "  ⚠️  Uprav server/docker/.env (zejm. PUBLIC_IP!)"
fi

# Build image
echo "→ Builduji metin2bausia-server:latest..."
docker compose -f "$SCRIPT_DIR/docker-compose.yml" build --no-cache

echo ""
echo "✓ Build dokončen!"
echo ""
echo "Spuštění serveru:"
echo "  docker compose -f server/docker/docker-compose.yml up -d"
echo ""
echo "Logy:"
echo "  docker compose -f server/docker/docker-compose.yml logs -f"
echo ""
echo "=== Porty ==="
echo "  MySQL (lokální admin): localhost:3366"
echo "  phpMyAdmin:            http://localhost:8099"
echo "  Auth server:           :11002"
echo "  Game CH1:              :13000"
