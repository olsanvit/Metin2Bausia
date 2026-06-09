# Herní data (share)

Sem zkopíruj herní data serveru před spuštěním:

```
server/share/
├── maps/           ← herní mapy (.msa soubory)
├── locale/         ← texty, stringy, překlady
│   └── czech/
│       ├── item_names.txt
│       └── mob_names.txt
└── scripts/
    └── quest/      ← Python quest skripty (.py)
```

Tento adresář je v `.gitignore` — herní data se neverzují (jsou příliš velká a proprietární).
Pouze vygenerované `item_proto` a `mob_proto` z DB se ukládají do `proto/` v rootu projektu.
