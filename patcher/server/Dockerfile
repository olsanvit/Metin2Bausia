# ============================================================
# Metin2Bausia — Patch server (nginx file hosting)
# Slouží ke stahování klientských souborů a manifestu
# ============================================================

FROM nginx:alpine

# Nginx konfigurace
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Adresář pro soubory klienta (mountovaný volume)
RUN mkdir -p /usr/share/nginx/html/client

# Výchozí manifest (přepíše se při prvním nasazení)
COPY manifest.json /usr/share/nginx/html/client/manifest.json

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s \
  CMD wget -qO- http://localhost/client/manifest.json || exit 1
