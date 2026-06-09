using System.Diagnostics;
using Metin2Bausia.Patcher.Models;
using Metin2Bausia.Patcher.Services;

namespace Metin2Bausia.Patcher;

public partial class MainForm : Form
{
    // ── Konfigurace ──────────────────────────────────────────
    private const string ManifestUrl  = "https://patch.bausia.cz/client/manifest.json";
    private const string WebsiteUrl   = "https://bausia.cz";
    private const string RegisterUrl  = "https://bausia.cz/register";
    // ─────────────────────────────────────────────────────────

    private readonly string _clientDir;
    private readonly PatcherService _patcher;
    private CancellationTokenSource? _cts;
    private PatchManifest? _manifest;

    public MainForm()
    {
        InitializeComponent();
        _clientDir = AppDomain.CurrentDomain.BaseDirectory;
        _patcher   = new PatcherService(ManifestUrl, _clientDir);

        _patcher.StatusChanged  += msg => SafeInvoke(() => lblStatus.Text = msg);
        _patcher.ProgressChanged+= pct => SafeInvoke(() =>
        {
            progressBar.Value = Math.Max(0, Math.Min(100, pct));
            lblProgress.Text  = $"{pct}%";
        });
        _patcher.BytesChanged += (dl, total) => SafeInvoke(() =>
        {
            lblBytes.Text = $"{FormatBytes(dl)} / {FormatBytes(total)}";
        });
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        btnPlay.Enabled  = false;
        btnPatch.Enabled = false;
        SetStatus("Připojuji se k patch serveru...");
        await CheckForUpdatesAsync();
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            _manifest = await _patcher.FetchManifestAsync();
            if (_manifest == null)
            {
                SetStatus("Nepodařilo se načíst manifest.");
                return;
            }

            lblVersion.Text    = $"Verze serveru: {_manifest.Version}";
            lblServerName.Text = _manifest.ServerName;
            if (!string.IsNullOrEmpty(_manifest.News))
                txtNews.Text = _manifest.News;

            var toUpdate = _patcher.GetFilesToUpdate(_manifest);

            if (toUpdate.Count == 0)
            {
                SetStatus("Klient je aktuální.");
                btnPlay.Enabled  = true;
                btnPatch.Enabled = false;
            }
            else
            {
                long totalSize = toUpdate.Sum(f => f.Size);
                SetStatus($"Dostupná aktualizace: {toUpdate.Count} souborů ({FormatBytes(totalSize)})");
                btnPatch.Enabled = true;
                btnPlay.Enabled  = false;
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Chyba: {ex.Message}");
            // Pokud manifest nelze stáhnout, přesto povolit spuštění (offline mode)
            btnPlay.Enabled = File.Exists(Path.Combine(_clientDir, "metin2.exe"));
        }
    }

    private async void btnPatch_Click(object sender, EventArgs e)
    {
        if (_manifest == null) return;

        btnPatch.Enabled = false;
        btnPlay.Enabled  = false;
        _cts = new CancellationTokenSource();

        try
        {
            var toUpdate = _patcher.GetFilesToUpdate(_manifest);
            await _patcher.DownloadFilesAsync(_manifest, toUpdate, _cts.Token);
            SetStatus("Aktualizace dokončena! Můžeš spustit hru.");
            btnPlay.Enabled = true;
        }
        catch (OperationCanceledException)
        {
            SetStatus("Aktualizace zrušena.");
            btnPatch.Enabled = true;
        }
        catch (Exception ex)
        {
            SetStatus($"Chyba při aktualizaci: {ex.Message}");
            btnPatch.Enabled = true;
        }
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
        var exe = _manifest?.Launcher.Exe ?? "metin2.exe";
        var args = _manifest?.Launcher.Args ?? "";
        var exePath = Path.Combine(_clientDir, exe);

        if (!File.Exists(exePath))
        {
            MessageBox.Show($"Soubor nenalezen: {exe}", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(exePath, args) { WorkingDirectory = _clientDir });
            Application.Exit();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Nepodařilo se spustit hru: {ex.Message}", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnWebsite_Click(object sender, EventArgs e)
        => Process.Start(new ProcessStartInfo(_manifest?.WebsiteUrl ?? WebsiteUrl) { UseShellExecute = true });

    private void btnRegister_Click(object sender, EventArgs e)
        => Process.Start(new ProcessStartInfo(_manifest?.RegisterUrl ?? RegisterUrl) { UseShellExecute = true });

    private void btnCancel_Click(object sender, EventArgs e)
        => _cts?.Cancel();

    private void SetStatus(string msg)
        => SafeInvoke(() => lblStatus.Text = msg);

    private void SafeInvoke(Action action)
    {
        if (InvokeRequired) Invoke(action);
        else action();
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }
}
