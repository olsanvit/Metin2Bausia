using System.Net.Http;
using System.Security.Cryptography;
using Metin2Bausia.Patcher.Models;
using Newtonsoft.Json;

namespace Metin2Bausia.Patcher.Services;

public class PatcherService
{
    private readonly HttpClient _http;
    private readonly string _manifestUrl;
    private readonly string _clientDir;

    public event Action<string>? StatusChanged;
    public event Action<int>? ProgressChanged;   // 0–100
    public event Action<long, long>? BytesChanged; // downloaded, total

    public PatcherService(string manifestUrl, string clientDir)
    {
        _manifestUrl = manifestUrl;
        _clientDir   = clientDir;
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _http.DefaultRequestHeaders.Add("User-Agent", "Metin2BausiaPatcher/1.0");
    }

    public async Task<PatchManifest?> FetchManifestAsync(CancellationToken ct = default)
    {
        StatusChanged?.Invoke("Stahuji informace o aktualizacích...");
        var json = await _http.GetStringAsync(_manifestUrl, ct);
        return JsonConvert.DeserializeObject<PatchManifest>(json);
    }

    /// <summary>Vrátí seznam souborů které je třeba stáhnout (chybí nebo jiný sha256)</summary>
    public List<PatchFile> GetFilesToUpdate(PatchManifest manifest)
    {
        var toUpdate = new List<PatchFile>();

        foreach (var file in manifest.Files)
        {
            var localPath = System.IO.Path.Combine(_clientDir, file.Path.Replace('/', '\\'));

            if (!File.Exists(localPath))
            {
                toUpdate.Add(file);
                continue;
            }

            // Ověřit sha256
            var localHash = ComputeSha256(localPath);
            if (!string.Equals(localHash, file.Sha256, StringComparison.OrdinalIgnoreCase))
                toUpdate.Add(file);
        }

        return toUpdate;
    }

    /// <summary>Stáhne a ověří všechny soubory ze seznamu</summary>
    public async Task DownloadFilesAsync(
        PatchManifest manifest,
        List<PatchFile> files,
        CancellationToken ct = default)
    {
        long totalBytes = files.Sum(f => f.Size);
        long downloadedBytes = 0;
        int fileIndex = 0;

        foreach (var file in files)
        {
            if (ct.IsCancellationRequested) break;

            fileIndex++;
            var url = file.Url ?? manifest.BaseUrl.TrimEnd('/') + "/" + file.Path.Replace('\\', '/');
            var localPath = System.IO.Path.Combine(_clientDir, file.Path.Replace('/', '\\'));

            StatusChanged?.Invoke($"[{fileIndex}/{files.Count}] {file.Path}");

            // Zajistit adresář
            var dir = System.IO.Path.GetDirectoryName(localPath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Dočasný soubor (přepsat až po úspěšném stažení)
            var tempPath = localPath + ".tmp";

            await DownloadWithProgressAsync(url, tempPath, file.Size, totalBytes,
                bytes =>
                {
                    downloadedBytes += bytes;
                    BytesChanged?.Invoke(downloadedBytes, totalBytes);
                    int pct = totalBytes > 0 ? (int)(downloadedBytes * 100 / totalBytes) : 0;
                    ProgressChanged?.Invoke(Math.Min(pct, 100));
                }, ct);

            // Ověřit sha256 staženého souboru
            var downloadedHash = ComputeSha256(tempPath);
            if (!string.Equals(downloadedHash, file.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(tempPath);
                throw new InvalidDataException($"Checksum nesedí pro soubor: {file.Path}");
            }

            // Přepsat existující soubor
            if (File.Exists(localPath)) File.Delete(localPath);
            File.Move(tempPath, localPath);
        }

        ProgressChanged?.Invoke(100);
        StatusChanged?.Invoke("Aktualizace dokončena!");
    }

    private async Task DownloadWithProgressAsync(
        string url, string destPath, long expectedSize, long totalBytes,
        Action<long> onProgress, CancellationToken ct)
    {
        using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var stream   = await response.Content.ReadAsStreamAsync(ct);
        await using var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);

        var buffer = new byte[81920];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
            onProgress(bytesRead);
        }
    }

    public static string ComputeSha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        return Convert.ToHexString(sha.ComputeHash(stream)).ToLower();
    }
}
