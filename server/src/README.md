# TMP4 Source

Sem zkopíruj TMP4 zdrojové kódy před buildem:

```bash
# Příklad:
cp -r /cesta/k/tmp4/* server/src/

# Nebo přes scp z jiného stroje:
scp -r user@host:/path/to/tmp4/* server/src/
```

Očekávaná struktura po zkopírování:
```
server/src/
├── CMakeLists.txt
├── db/          ← DB server source
├── auth/        ← Auth server source (někdy součástí game/)
└── game/        ← Game server source
    ├── CMakeLists.txt
    └── ...
```

Tento adresář je v `.gitignore` — zdrojové kódy TMP4 se neverzují.
