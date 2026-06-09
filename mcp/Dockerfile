FROM node:22-alpine
WORKDIR /app
COPY package.json .
RUN npm install
COPY server.js .
RUN mkdir -p /app/uploads /app/mt2files
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD wget -qO- http://localhost:${PORT:-3000}/ping || exit 1
CMD ["node", "server.js"]
