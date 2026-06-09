# ============================================================
# Metin2Bausia — Generování patch manifestu
# Spouštět na Windows z adresáře s klientem
#
# Použití:
#   .\generate_manifest.ps1 -ClientDir "C:\Metin2Bausia" -Version "1.0.1"
#
# Výstup:
#   manifest.json — nahrát na patch server: https://patch.bausia.cz/client/manifest.json
# ============================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$ClientDir,

    [Parameter(Mandatory=$true)]
    [string]$Version,

    [string]$BaseUrl = "https://patch.bausia.cz/client/",
    [string]$ServerIp = "0.0.0.0",
    [int]$ServerPort = 11002,
    [string]$OutputFile = "manifest.json"
)

Write-Host "=== Metin2Bausia Manifest Generator ===" -ForegroundColor Cyan
Write-Host "Klient: $ClientDir"
Write-Host "Verze:  $Version"
Write-Host ""

if (-not (Test-Path $ClientDir)) {
    Write-Error "Adresář klienta neexistuje: $ClientDir"
    exit 1
}

# Soubory k zahrnutí do manifestu
$includeExtensions = @(".exe", ".dll", ".epk", ".eix", ".bin", ".tbl", ".cfg")
$excludePatterns   = @("patcher.exe", "updater.exe", "*.log", "*.tmp")

$files = @()
$totalSize = 0
$fileCount = 0

Write-Host "Procházím soubory..." -ForegroundColor Yellow

Get-ChildItem -Path $ClientDir -Recurse -File | Where-Object {
    $ext = $_.Extension.ToLower()
    $includeExtensions -contains $ext
} | ForEach-Object {
    $file = $_
    $relativePath = $file.FullName.Substring($ClientDir.Length).TrimStart('\', '/')
    $relativePath = $relativePath.Replace('\', '/')

    # SHA-256 checksum
    $hash = (Get-FileHash -Path $file.FullName -Algorithm SHA256).Hash.ToLower()
    $size = $file.Length

    $files += [PSCustomObject]@{
        path     = $relativePath
        sha256   = $hash
        size     = $size
        required = $true
    }

    $totalSize += $size
    $fileCount++
    Write-Host "  [$fileCount] $relativePath ($([math]::Round($size/1MB, 2)) MB)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Celkem souborů: $fileCount" -ForegroundColor Green
Write-Host "Celková velikost: $([math]::Round($totalSize/1MB, 1)) MB ($([math]::Round($totalSize/1GB, 2)) GB)" -ForegroundColor Green

# Sestavit manifest
$manifest = [PSCustomObject]@{
    version    = $Version
    date       = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")
    serverName = "Metin2 Bausia"
    serverIp   = $ServerIp
    serverPort = $ServerPort
    baseUrl    = $BaseUrl
    news       = "Verze $Version"
    websiteUrl = "https://bausia.cz"
    registerUrl= "https://bausia.cz/register"
    files      = $files
    launcher   = [PSCustomObject]@{
        exe  = "metin2.exe"
        args = ""
    }
}

# Uložit
$json = $manifest | ConvertTo-Json -Depth 10 -Compress:$false
$json | Out-File -FilePath $OutputFile -Encoding UTF8

Write-Host ""
Write-Host "✓ Manifest uložen: $OutputFile" -ForegroundColor Green
Write-Host ""
Write-Host "Další kroky:" -ForegroundColor Cyan
Write-Host "  1. Zkontroluj $OutputFile (serverIp, baseUrl)"
Write-Host "  2. Nahraj soubory klienta na: $BaseUrl"
Write-Host "  3. Nahraj manifest.json na: ${BaseUrl}manifest.json"
Write-Host "  4. Aktualizuj patcher/server/manifest.json v git repozitáři"
