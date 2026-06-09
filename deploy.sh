#!/bin/bash
# deploy.sh — Metin2Bausia QNAP deployment script
# Nasazuje management stack (PostgreSQL + MT2 MCP) na QNAP přes SSH
set -e

QNAP_HOST="192.168.60.221"
QNAP_USER="admin"
QNAP_SSH_KEY="$HOME/.ssh/claude-qnap"
QNAP_DIR="/share/Public/Metin2Bausia"
COMPOSE_FILE="docker-compose.yml"

echo "=== Metin2Bausia Deploy Script ==="
echo "Target: $QNAP_USER@$QNAP_HOST:$QNAP_DIR"
echo ""

# Ověřit SSH přístup
echo "→ Ověřuji SSH přístup..."
ssh -i "$QNAP_SSH_KEY" -o ConnectTimeout=10 "$QNAP_USER@$QNAP_HOST" "echo OK" || {
  echo "✗ SSH přístup selhání"
  exit 1
}
echo "✓ SSH OK"

# Vytvořit adresář na QNAP
echo "→ Vytvářím adresáře na QNAP..."
ssh -i "$QNAP_SSH_KEY" "$QNAP_USER@$QNAP_HOST" "mkdir -p $QNAP_DIR/mcp $QNAP_DIR/database/postgres"

# Nahrát soubory přes scp
echo "→ Nahrávám soubory..."
scp -i "$QNAP_SSH_KEY" "$COMPOSE_FILE" "$QNAP_USER@$QNAP_HOST:$QNAP_DIR/"
scp -i "$QNAP_SSH_KEY" mcp/Dockerfile mcp/server.js mcp/package.json "$QNAP_USER@$QNAP_HOST:$QNAP_DIR/mcp/"
scp -i "$QNAP_SSH_KEY" database/postgres/01_content_tables.sql database/postgres/02_approval_workflow.sql "$QNAP_USER@$QNAP_HOST:$QNAP_DIR/database/postgres/"

# Pokud existuje .env, nahrát také
if [ -f ".env" ]; then
  scp -i "$QNAP_SSH_KEY" .env "$QNAP_USER@$QNAP_HOST:$QNAP_DIR/"
  echo "✓ .env nahrán"
fi

echo "✓ Soubory nahrány"

# Build a spustit kontejnery
echo "→ Spouštím docker-compose build + up na QNAP..."
ssh -i "$QNAP_SSH_KEY" "$QNAP_USER@$QNAP_HOST" "
  cd $QNAP_DIR
  docker compose build --no-cache mt2-mcp
  docker compose up -d
  echo '--- Kontejnery ---'
  docker compose ps
"

echo ""
echo "✓ Deploy dokončen!"
echo ""
echo "=== Přístupy ==="
echo "MT2 MCP health: http://$QNAP_HOST:3030/ping"
echo "MT2 MCP API:    http://$QNAP_HOST:3030/"
echo "PostgreSQL:     $QNAP_HOST:5442 (db: Metin2Bausia)"
echo ""
echo "=== Nginx proxy ==="
echo "Přidej do nginx config: /MT2/ → http://localhost:3030/"
echo "Veřejný endpoint: https://mcp.vo2info.cz/MT2/"
