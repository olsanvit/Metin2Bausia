namespace Metin2Bausia.Patcher;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Ovládací prvky
    private Label lblServerName = null!;
    private Label lblVersion    = null!;
    private Label lblStatus     = null!;
    private Label lblProgress   = null!;
    private Label lblBytes      = null!;
    private ProgressBar progressBar = null!;
    private RichTextBox txtNews = null!;
    private Button btnPatch     = null!;
    private Button btnPlay      = null!;
    private Button btnWebsite   = null!;
    private Button btnRegister  = null!;
    private Button btnCancel    = null!;
    private Panel panelBottom   = null!;
    private Panel panelTop      = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // ── Form ──────────────────────────────────────────────
        this.Text            = "Metin2 Bausia — Launcher";
        this.Size            = new Size(640, 480);
        this.MinimumSize     = new Size(640, 480);
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.BackColor       = Color.FromArgb(20, 20, 35);
        this.ForeColor       = Color.White;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox     = false;
        this.Load           += MainForm_Load;

        // ── Top panel (logo + info) ────────────────────────────
        panelTop = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 80,
            BackColor = Color.FromArgb(30, 30, 50)
        };

        lblServerName = new Label
        {
            Text      = "Metin2 Bausia",
            Font      = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(220, 180, 60),
            AutoSize  = true,
            Location  = new Point(20, 12)
        };

        lblVersion = new Label
        {
            Text      = "Načítám verzi...",
            Font      = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(150, 150, 170),
            AutoSize  = true,
            Location  = new Point(22, 50)
        };

        panelTop.Controls.Add(lblServerName);
        panelTop.Controls.Add(lblVersion);

        // ── News box ──────────────────────────────────────────
        txtNews = new RichTextBox
        {
            ReadOnly  = true,
            BackColor = Color.FromArgb(15, 15, 28),
            ForeColor = Color.FromArgb(200, 200, 220),
            Font      = new Font("Segoe UI", 9),
            BorderStyle = BorderStyle.None,
            ScrollBars  = RichTextBoxScrollBars.Vertical,
            Dock        = DockStyle.Fill,
            Text        = "Načítám novinky..."
        };

        // ── Bottom panel (progress + buttons) ─────────────────
        panelBottom = new Panel
        {
            Dock      = DockStyle.Bottom,
            Height    = 110,
            BackColor = Color.FromArgb(25, 25, 42)
        };

        lblStatus = new Label
        {
            Text      = "Inicializuji...",
            Font      = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(180, 180, 200),
            AutoSize  = false,
            Width     = 590,
            Location  = new Point(20, 10)
        };

        progressBar = new ProgressBar
        {
            Minimum  = 0,
            Maximum  = 100,
            Value    = 0,
            Width    = 490,
            Height   = 18,
            Location = new Point(20, 35),
            Style    = ProgressBarStyle.Continuous
        };

        lblProgress = new Label
        {
            Text      = "0%",
            Font      = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(160, 160, 180),
            AutoSize  = true,
            Location  = new Point(520, 38)
        };

        lblBytes = new Label
        {
            Text      = "",
            Font      = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(120, 120, 140),
            AutoSize  = true,
            Location  = new Point(20, 60)
        };

        // Tlačítka
        btnWebsite = CreateButton("Web", 20, 78, 80, Color.FromArgb(50, 50, 80));
        btnWebsite.Click += btnWebsite_Click;

        btnRegister = CreateButton("Registrace", 110, 78, 100, Color.FromArgb(50, 80, 50));
        btnRegister.Click += btnRegister_Click;

        btnCancel = CreateButton("Zrušit", 310, 78, 80, Color.FromArgb(80, 40, 40));
        btnCancel.Click += btnCancel_Click;

        btnPatch = CreateButton("Aktualizovat", 400, 78, 100, Color.FromArgb(60, 100, 60));
        btnPatch.Enabled = false;
        btnPatch.Click  += btnPatch_Click;

        btnPlay = CreateButton("▶  HRÁT", 510, 74, 110, Color.FromArgb(40, 100, 40));
        btnPlay.Font    = new Font("Segoe UI", 11, FontStyle.Bold);
        btnPlay.Height  = 32;
        btnPlay.Enabled = false;
        btnPlay.Click  += btnPlay_Click;

        panelBottom.Controls.AddRange(new Control[] {
            lblStatus, progressBar, lblProgress, lblBytes,
            btnWebsite, btnRegister, btnCancel, btnPatch, btnPlay
        });

        // ── Sestavit form ────────────────────────────────────
        this.Controls.Add(txtNews);
        this.Controls.Add(panelTop);
        this.Controls.Add(panelBottom);
    }

    private static Button CreateButton(string text, int x, int y, int width, Color back)
    {
        return new Button
        {
            Text      = text,
            Location  = new Point(x, y),
            Width     = width,
            Height    = 26,
            BackColor = back,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9),
            Cursor    = Cursors.Hand
        };
    }
}
