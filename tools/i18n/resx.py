#!/usr/bin/env python3
# ============================================================
# Doplní klíče do Resources/SharedResources.resx (čeština) a .en.resx (angličtina).
#
# Proč skript: stránky mají přes 200 textů a každý musí být v obou souborech se stejným
# klíčem — ruční úpravy dvou XML souborů vedou k chybějícím překladům. Existující klíče
# se zachovají, nové se přidají, hodnoty z parametru přepíšou stávající.
#
# Použití: python3 tools/i18n/resx.py <soubor.json>   (json: {"Klic": ["česky", "english"], ...})
# ============================================================
import json, sys, re, os
from xml.sax.saxutils import escape

ROOT = os.path.join(os.path.dirname(__file__), "..", "..", "src", "Metin2Bausia.Web", "Resources")
FILES = {"cs": "SharedResources.resx", "en": "SharedResources.en.resx"}

HEADER = """<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
"""

def read(path):
    if not os.path.exists(path): return {}
    text = open(path, encoding="utf-8").read()
    out = {}
    for m in re.finditer(r'<data name="([^"]+)"[^>]*>\s*<value>(.*?)</value>', text, re.S):
        out[m.group(1)] = (m.group(2).replace("&lt;", "<").replace("&gt;", ">")
                                     .replace("&quot;", '"').replace("&amp;", "&"))
    return out

def write(path, entries):
    body = "".join(
        f'  <data name="{escape(k)}" xml:space="preserve">\n    <value>{escape(v)}</value>\n  </data>\n'
        for k, v in sorted(entries.items(), key=lambda kv: kv[0].lower()))
    open(path, "w", encoding="utf-8").write(HEADER + body + "</root>\n")

new = json.load(open(sys.argv[1], encoding="utf-8"))
for idx, (lang, name) in enumerate(FILES.items()):
    path = os.path.join(ROOT, name)
    entries = read(path)
    for key, values in new.items():
        entries[key] = values[idx]
    write(path, entries)
    print(f"{name}: {len(entries)} klíčů")
