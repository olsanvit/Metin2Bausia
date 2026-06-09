#!/bin/bash
# ============================================================
# Metin2Bausia — entrypoint pro game server kontejner
# MT2_SERVICE: db | auth | game
# ============================================================
set -e

SERVICE="${MT2_SERVICE:-game}"
BIN_DIR="/usr/local/metin2/bin"
CONF_BASE="/usr/local/metin2"
LOG_DIR="/usr/local/metin2/log"

mkdir -p "$LOG_DIR"

echo "[entrypoint] Spouštím službu: $SERVICE"

case "$SERVICE" in
  db)
    CONF_DIR="$CONF_BASE/db"
    echo "[entrypoint] DB server: $CONF_DIR/conf.cfg"
    exec "$BIN_DIR/db" "$CONF_DIR/conf.cfg"
    ;;

  auth)
    CONF_DIR="$CONF_BASE/auth"
    echo "[entrypoint] Auth server: $CONF_DIR/conf.cfg"
    exec "$BIN_DIR/auth" "$CONF_DIR/conf.cfg"
    ;;

  game)
    CHANNEL="${MT2_CHANNEL:-1}"
    CONF_DIR="$CONF_BASE/game"
    echo "[entrypoint] Game server channel $CHANNEL: $CONF_DIR/conf.cfg"
    exec "$BIN_DIR/game" "$CONF_DIR/conf.cfg"
    ;;

  *)
    echo "[entrypoint] CHYBA: neznámá MT2_SERVICE='$SERVICE'. Použij: db | auth | game"
    exit 1
    ;;
esac
