# ============================================================
# Metin2Bausia — TMP4 game server image
# Předpoklady:
#   - TMP4 source uložit do: server/src/
#   - Build kompiluje db, auth, game binaries z source
# ============================================================

FROM ubuntu:22.04 AS builder

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    cmake \
    g++ \
    libmysqlclient-dev \
    libssl-dev \
    zlib1g-dev \
    libbz2-dev \
    libboost-all-dev \
    git \
    python3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# TMP4 source musí být zkopírován do server/src/ před buildem
COPY src/ ./src/

# Build game server binaries (TMP4 cmake build)
RUN cd src && \
    mkdir -p build && \
    cd build && \
    cmake .. -DCMAKE_BUILD_TYPE=Release && \
    make -j$(nproc)

# ============================================================
# Runtime image
# ============================================================
FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y --no-install-recommends \
    libmysqlclient21 \
    libssl3 \
    zlib1g \
    libbz2-1.0 \
    libboost-system1.74.0 \
    libboost-filesystem1.74.0 \
    libboost-thread1.74.0 \
    python3 \
    python3-pip \
    && rm -rf /var/lib/apt/lists/*

# Struktura herního serveru
RUN mkdir -p \
    /usr/local/metin2/db \
    /usr/local/metin2/auth \
    /usr/local/metin2/game \
    /usr/local/metin2/share/locale \
    /usr/local/metin2/share/maps \
    /usr/local/metin2/share/item_proto \
    /usr/local/metin2/share/mob_proto \
    /usr/local/metin2/log

# Zkopírovat binaries z builder
COPY --from=builder /build/src/build/db/db /usr/local/metin2/bin/db
COPY --from=builder /build/src/build/auth/auth /usr/local/metin2/bin/auth
COPY --from=builder /build/src/build/game/game /usr/local/metin2/bin/game

# Python skripty (questy apod.)
COPY --from=builder /build/src/game/scripts/ /usr/local/metin2/share/scripts/

# Entrypoint
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

WORKDIR /usr/local/metin2

ENTRYPOINT ["/entrypoint.sh"]
