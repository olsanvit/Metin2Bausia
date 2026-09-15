# server/share — nepoužívá se

Dřív se sem měla kopírovat herní data. Upstream projekt (The Old Metin2 Project) je
ale nese přímo ve zdrojácích:

| Co | Kde |
|---|---|
| proto, názvy v 15 jazycích | `server/src/gamefiles/conf/` |
| mapy, spawny, dropy, skupiny, questy | `server/src/gamefiles/data/` |

Upstream `Dockerfile` je do image kopíruje sám, compose v `server/docker/` jen přes
bind mount přesměruje názvy na zvolený jazyk. Postup je v `server/README.md`.
