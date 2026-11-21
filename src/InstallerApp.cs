using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: System.Reflection.AssemblyTitle("DELTARUNE Mod Installer")]
[assembly: System.Reflection.AssemblyProduct("DELTARUNE Mod Installer")]
[assembly: System.Reflection.AssemblyVersion("2.1.0.0")]

namespace DeltaruneModInstaller
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class InstallerSettings
    {
        public string Language = "tr";          // "tr" or "en"
        public int AccentIndex = 0;             // 0: Indigo, 1: Cyan, 2: Crimson, 3: Gold, 4: Emerald
        public bool AutoLaunchGame = false;
        public bool VerboseLog = false;
        public bool ConfirmBeforeInstall = false;

        private static string SettingsPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "installer_settings.ini"); }
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return;
                foreach (var line in File.ReadAllLines(SettingsPath))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length != 2) continue;
                    string k = parts[0].Trim().ToUpperInvariant();
                    string v = parts[1].Trim();
                    if (k == "LANG") Language = (v.ToLowerInvariant() == "en") ? "en" : "tr";
                    else if (k == "ACCENT") { int a; if (int.TryParse(v, out a)) AccentIndex = Math.Max(0, Math.Min(4, a)); }
                    else if (k == "AUTOLAUNCH") AutoLaunchGame = (v == "1" || v.ToLowerInvariant() == "true");
                    else if (k == "VERBOSE") VerboseLog = (v == "1" || v.ToLowerInvariant() == "true");
                    else if (k == "CONFIRM") ConfirmBeforeInstall = (v == "1" || v.ToLowerInvariant() == "true");
                }
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                string content =
                    "LANG=" + Language + "\n" +
                    "ACCENT=" + AccentIndex + "\n" +
                    "AUTOLAUNCH=" + (AutoLaunchGame ? "1" : "0") + "\n" +
                    "VERBOSE=" + (VerboseLog ? "1" : "0") + "\n" +
                    "CONFIRM=" + (ConfirmBeforeInstall ? "1" : "0") + "\n";
                File.WriteAllText(SettingsPath, content, new UTF8Encoding(false));
            }
            catch { }
        }
    }

    static class UiTheme
    {
        public static readonly Color BgMain = Color.FromArgb(11, 13, 19);
        public static readonly Color BgHeader = Color.FromArgb(15, 18, 26);
        public static readonly Color BgCard = Color.FromArgb(19, 23, 34);
        public static readonly Color BgCardHover = Color.FromArgb(24, 29, 43);
        public static readonly Color BgCardSelected = Color.FromArgb(23, 28, 44);
        public static readonly Color BgInput = Color.FromArgb(16, 19, 28);

        public static readonly Color BorderSubtle = Color.FromArgb(35, 42, 59);
        public static readonly Color BorderHover = Color.FromArgb(56, 67, 94);

        public static Color Accent = Color.FromArgb(99, 102, 241);
        public static Color AccentHover = Color.FromArgb(79, 70, 229);
        public static Color AccentPressed = Color.FromArgb(67, 56, 202);
        public static Color AccentLight = Color.FromArgb(129, 140, 248);

        public static readonly Color Success = Color.FromArgb(52, 211, 153);
        public static readonly Color Danger = Color.FromArgb(248, 113, 113);

        public static readonly Color TextPrimary = Color.FromArgb(241, 245, 249);
        public static readonly Color TextSecondary = Color.FromArgb(148, 163, 184);
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);

        public static void ApplyAccent(int index)
        {
            switch (index)
            {
                case 1: // Cyber Cyan
                    Accent = Color.FromArgb(14, 165, 233);
                    AccentHover = Color.FromArgb(2, 132, 199);
                    AccentPressed = Color.FromArgb(3, 105, 161);
                    AccentLight = Color.FromArgb(56, 189, 248);
                    break;
                case 2: // Soul Crimson
                    Accent = Color.FromArgb(244, 63, 94);
                    AccentHover = Color.FromArgb(225, 29, 72);
                    AccentPressed = Color.FromArgb(190, 18, 60);
                    AccentLight = Color.FromArgb(251, 113, 133);
                    break;
                case 3: // Royal Gold
                    Accent = Color.FromArgb(234, 179, 8);
                    AccentHover = Color.FromArgb(202, 138, 4);
                    AccentPressed = Color.FromArgb(161, 98, 7);
                    AccentLight = Color.FromArgb(250, 204, 21);
                    break;
                case 4: // Emerald
                    Accent = Color.FromArgb(16, 185, 129);
                    AccentHover = Color.FromArgb(5, 150, 105);
                    AccentPressed = Color.FromArgb(4, 120, 87);
                    AccentLight = Color.FromArgb(52, 211, 153);
                    break;
                default: // Indigo
                    Accent = Color.FromArgb(99, 102, 241);
                    AccentHover = Color.FromArgb(79, 70, 229);
                    AccentPressed = Color.FromArgb(67, 56, 202);
                    AccentLight = Color.FromArgb(129, 140, 248);
                    break;
            }
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0 || bounds.Width < d || bounds.Height < d)
            {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class ModernButton : Control
    {
        public bool IsPrimary { get; set; }
        private bool _hover;
        private bool _pressed;

        public ModernButton(string text, bool isPrimary)
        {
            Text = text;
            IsPrimary = isPrimary;
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

            MouseEnter += (s, e) => { _hover = true; Invalidate(); };
            MouseLeave += (s, e) => { _hover = false; _pressed = false; Invalidate(); };
            MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { _pressed = true; Invalidate(); } };
            MouseUp += (s, e) => { _pressed = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Color bg;
            Color fg;
            Color border;

            if (!Enabled)
            {
                bg = Color.FromArgb(20, 24, 34);
                fg = UiTheme.TextMuted;
                border = UiTheme.BorderSubtle;
            }
            else if (IsPrimary)
            {
                bg = _pressed ? UiTheme.AccentPressed : (_hover ? UiTheme.AccentHover : UiTheme.Accent);
                fg = Color.White;
                border = bg;
            }
            else
            {
                bg = _pressed ? Color.FromArgb(19, 23, 34) : (_hover ? Color.FromArgb(30, 36, 52) : Color.FromArgb(22, 27, 40));
                fg = UiTheme.TextPrimary;
                border = _hover ? UiTheme.BorderHover : UiTheme.BorderSubtle;
            }

            using (GraphicsPath path = UiTheme.RoundedRect(rect, 8))
            using (SolidBrush br = new SolidBrush(bg))
            using (Pen pen = new Pen(border, 1f))
            {
                g.FillPath(br, path);
                g.DrawPath(pen, path);
            }

            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using (SolidBrush textBr = new SolidBrush(fg))
            {
                g.DrawString(Text, Font, textBr, new RectangleF(0, 0, Width, Height), sf);
            }
        }
    }

    public class ModCard : Panel
    {
        public bool IsChecked { get; set; }
        public string Title { get; set; }
        public string TagText { get; set; }
        public string Description { get; set; }
        public event EventHandler CheckedChanged;

        private bool _hover = false;

        public ModCard(string title, string tag, string desc, bool defaultChecked = true)
        {
            Title = title;
            TagText = tag;
            Description = desc;
            IsChecked = defaultChecked;
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            Height = 68;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

            MouseEnter += (s, e) => { _hover = true; Invalidate(); };
            MouseLeave += (s, e) => { _hover = false; Invalidate(); };
            MouseClick += (s, e) =>
            {
                if (!Enabled) return;
                IsChecked = !IsChecked;
                Invalidate();
                if (CheckedChanged != null)
                    CheckedChanged(this, EventArgs.Empty);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Color bg = IsChecked
                ? (_hover ? Color.FromArgb(27, 33, 52) : UiTheme.BgCardSelected)
                : (_hover ? UiTheme.BgCardHover : UiTheme.BgCard);

            Color borderCol = IsChecked
                ? UiTheme.Accent
                : (_hover ? UiTheme.BorderHover : UiTheme.BorderSubtle);

            using (GraphicsPath path = UiTheme.RoundedRect(rect, 10))
            using (SolidBrush bgBrush = new SolidBrush(bg))
            using (Pen borderPen = new Pen(borderCol, IsChecked ? 1.5f : 1f))
            {
                g.FillPath(bgBrush, path);
                g.DrawPath(borderPen, path);
            }

            Rectangle chkRect = new Rectangle(20, (Height - 20) / 2, 20, 20);
            using (GraphicsPath chkPath = UiTheme.RoundedRect(chkRect, 5))
            using (SolidBrush chkBrush = new SolidBrush(IsChecked ? UiTheme.Accent : UiTheme.BgInput))
            using (Pen chkPen = new Pen(IsChecked ? UiTheme.Accent : Color.FromArgb(71, 85, 105), 1.5f))
            {
                g.FillPath(chkBrush, chkPath);
                g.DrawPath(chkPen, chkPath);
                if (IsChecked)
                {
                    using (Pen checkMarkPen = new Pen(Color.White, 2f))
                    {
                        checkMarkPen.StartCap = LineCap.Round;
                        checkMarkPen.EndCap = LineCap.Round;
                        g.DrawLines(checkMarkPen, new[]
                        {
                            new Point(chkRect.X + 5, chkRect.Y + 10),
                            new Point(chkRect.X + 9, chkRect.Y + 14),
                            new Point(chkRect.X + 15, chkRect.Y + 6)
                        });
                    }
                }
            }

            using (Font titleFont = new Font("Segoe UI Semibold", 10f, FontStyle.Bold))
            using (Font tagFont = new Font("Segoe UI", 8f, FontStyle.Regular))
            using (Font descFont = new Font("Segoe UI", 8.75f, FontStyle.Regular))
            using (SolidBrush titleBrush = new SolidBrush(IsChecked ? UiTheme.TextPrimary : Color.FromArgb(203, 213, 225)))
            using (SolidBrush descBrush = new SolidBrush(UiTheme.TextSecondary))
            {
                int textX = 54;
                g.DrawString(Title, titleFont, titleBrush, textX, 12);

                SizeF titleSize = g.MeasureString(Title, titleFont);
                int badgeX = textX + (int)titleSize.Width + 8;
                SizeF badgeSize = g.MeasureString(TagText, tagFont);
                Rectangle badgeRect = new Rectangle(badgeX, 14, (int)badgeSize.Width + 12, 18);

                using (GraphicsPath badgePath = UiTheme.RoundedRect(badgeRect, 4))
                using (SolidBrush badgeBg = new SolidBrush(IsChecked ? Color.FromArgb(45, UiTheme.Accent) : Color.FromArgb(28, 34, 48)))
                using (SolidBrush badgeTextBrush = new SolidBrush(IsChecked ? UiTheme.AccentLight : UiTheme.TextMuted))
                {
                    g.FillPath(badgeBg, badgePath);
                    g.DrawString(TagText, tagFont, badgeTextBrush, badgeX + 6, 15);
                }

                RectangleF descRect = new RectangleF(textX, 36, Width - textX - 20, 24);
                g.DrawString(Description, descFont, descBrush, descRect);
            }
        }
    }

    public class SettingsDialog : Form
    {
        private InstallerSettings _settings;
        private Action _onSettingsChanged;

        private ComboBox _cmbLang;
        private ComboBox _cmbAccent;
        private CheckBox _chkAutoLaunch;
        private CheckBox _chkVerbose;
        private CheckBox _chkConfirm;
        private string _gameDir;

        public SettingsDialog(InstallerSettings settings, string gameDir, Action onSettingsChanged)
        {
            _settings = settings;
            _gameDir = gameDir;
            _onSettingsChanged = onSettingsChanged;

            bool en = _settings.Language == "en";
            Text = en ? "Settings" : "Ayarlar";
            Size = new Size(460, 430);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = UiTheme.BgMain;
            ForeColor = UiTheme.TextPrimary;
            Font = new Font("Segoe UI", 9f);

            int y = 20;

            Label lblHeader = new Label
            {
                Text = en ? "Application Settings" : "Uygulama Ayarları",
                Font = new Font("Segoe UI Semibold", 12f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(24, y),
                AutoSize = true
            };
            Controls.Add(lblHeader);
            y += 38;

            // 1. Language
            Label lblLang = new Label
            {
                Text = en ? "Language / Dil:" : "Uygulama Dili / Language:",
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(24, y + 4),
                AutoSize = true
            };
            Controls.Add(lblLang);

            _cmbLang = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = UiTheme.BgCard,
                ForeColor = UiTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(220, y),
                Width = 200
            };
            _cmbLang.Items.AddRange(new object[] { "Türkçe (TR)", "English (EN)" });
            _cmbLang.SelectedIndex = (_settings.Language == "en") ? 1 : 0;
            Controls.Add(_cmbLang);
            y += 38;

            // 2. Accent Color
            Label lblAccent = new Label
            {
                Text = en ? "Theme Accent Color:" : "Tema Vurgu Rengi:",
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(24, y + 4),
                AutoSize = true
            };
            Controls.Add(lblAccent);

            _cmbAccent = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = UiTheme.BgCard,
                ForeColor = UiTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(220, y),
                Width = 200
            };
            if (en)
                _cmbAccent.Items.AddRange(new object[] { "Indigo (Default)", "Cyber Cyan", "Soul Crimson", "Royal Gold", "Emerald Green" });
            else
                _cmbAccent.Items.AddRange(new object[] { "İndigo (Varsayılan)", "Siber Mavi", "Kızıl Ruh", "Altın Sarısı", "Zümrüt Yeşili" });
            _cmbAccent.SelectedIndex = _settings.AccentIndex;
            Controls.Add(_cmbAccent);
            y += 44;

            // 3. Checkboxes
            _chkAutoLaunch = new CheckBox
            {
                Text = en ? "Launch DELTARUNE automatically after installation" : "Kurulum tamamlandığında oyunu otomatik başlat",
                Checked = _settings.AutoLaunchGame,
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(24, y),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            Controls.Add(_chkAutoLaunch);
            y += 30;

            _chkVerbose = new CheckBox
            {
                Text = en ? "Show detailed UTMT CLI output in log console" : "Konsolda ayrıntılı UTMT CLI çıktılarını göster",
                Checked = _settings.VerboseLog,
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(24, y),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            Controls.Add(_chkVerbose);
            y += 30;

            _chkConfirm = new CheckBox
            {
                Text = en ? "Ask for confirmation before installing mods" : "Kuruluma başlamadan önce onay penceresi göster",
                Checked = _settings.ConfirmBeforeInstall,
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(24, y),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            Controls.Add(_chkConfirm);
            y += 42;

            // 4. Quick Tools
            Label lblTools = new Label
            {
                Text = en ? "Quick Actions" : "Hızlı Araçlar",
                Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(24, y),
                AutoSize = true
            };
            Controls.Add(lblTools);
            y += 26;

            ModernButton btnOpenGame = new ModernButton(en ? "Open Game Folder" : "Oyun Klasörünü Aç", false)
            {
                Bounds = new Rectangle(24, y, 192, 34)
            };
            btnOpenGame.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_gameDir) && Directory.Exists(_gameDir))
                    Process.Start("explorer.exe", _gameDir);
            };
            Controls.Add(btnOpenGame);

            ModernButton btnOpenBackups = new ModernButton(en ? "Open Backups Folder" : "Yedek Klasörünü Aç", false)
            {
                Bounds = new Rectangle(228, y, 192, 34)
            };
            btnOpenBackups.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_gameDir))
                {
                    string bDir = Path.Combine(_gameDir, "mod_backups");
                    if (Directory.Exists(bDir))
                        Process.Start("explorer.exe", bDir);
                    else
                        MessageBox.Show(en ? "No backup folder created yet." : "Henüz yedek klasörü oluşturulmadı.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            Controls.Add(btnOpenBackups);
            y += 54;

            // Save & Close
            ModernButton btnSave = new ModernButton(en ? "Save & Apply" : "Kaydet ve Uygula", true)
            {
                Bounds = new Rectangle(24, y, 396, 38),
                Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold)
            };
            btnSave.Click += (s, e) =>
            {
                _settings.Language = (_cmbLang.SelectedIndex == 1) ? "en" : "tr";
                _settings.AccentIndex = _cmbAccent.SelectedIndex;
                _settings.AutoLaunchGame = _chkAutoLaunch.Checked;
                _settings.VerboseLog = _chkVerbose.Checked;
                _settings.ConfirmBeforeInstall = _chkConfirm.Checked;
                _settings.Save();
                UiTheme.ApplyAccent(_settings.AccentIndex);
                if (_onSettingsChanged != null) _onSettingsChanged();
                Close();
            };
            Controls.Add(btnSave);
        }
    }

    public class MainForm : Form
    {
        private InstallerSettings _settings = new InstallerSettings();

        private Panel _header;
        private PictureBox _picLogo;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private ModernButton _btnLangToggle;
        private ModernButton _btnSettings;
        private ModernButton _btnFullscreen;

        private Panel _container;
        private Panel _pathCard;
        private Label _lblPathCaption;
        private TextBox _txtGamePath;
        private ModernButton _btnBrowse;
        private Label _lblPathStatus;

        private ModCard _cardMenu;
        private ModCard _cardFps;
        private ModCard _cardBorder;

        private Panel _chCard;
        private CheckBox[] _chkChapters;
        private readonly string[] _roleKeys = { "launcher", "ch1", "ch2", "ch3", "ch4", "ch5" };
        private readonly string[] _roleRelPaths = { "data.win", @"chapter1_windows\data.win", @"chapter2_windows\data.win", @"chapter3_windows\data.win", @"chapter4_windows\data.win", @"chapter5_windows\data.win" };
        private readonly string[] _fpsScripts = { "apply_60fps_launcher.csx", "apply_60fps_ch1.csx", "apply_60fps_ch2.csx", "apply_60fps_ch3.csx", "apply_60fps_ch4.csx", "apply_60fps_ch5.csx" };

        private Panel _progressBarBg;
        private Panel _progressBarFill;
        private Label _lblProgressStatus;
        private TextBox _txtLog;

        private ModernButton _btnInstall;
        private ModernButton _btnRestore;

        private bool _isWorking = false;
        private int _progressPercent = 0;

        private bool _isFullscreen = false;
        private FormWindowState _prevWindowState;
        private FormBorderStyle _prevBorderStyle;
        private Rectangle _prevBounds;

        public MainForm()
        {
            _settings.Load();
            UiTheme.ApplyAccent(_settings.AccentIndex);

            Text = "DELTARUNE Mod Installer";
            Size = new Size(860, 700);
            MinimumSize = new Size(740, 620);
            MaximizeBox = true;
            MinimizeBox = true;
            KeyPreview = true;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = UiTheme.BgMain;
            ForeColor = UiTheme.TextPrimary;
            Font = new Font("Segoe UI", 9f);
            DoubleBuffered = true;

            try
            {
                Icon assoc = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (assoc != null) Icon = assoc;
            }
            catch { }

            BuildUI();
            ApplyLocalization();
            AutoDetectGamePath();

            Resize += (s, e) => UpdateResponsiveLayout();
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F11)
                {
                    ToggleFullscreen();
                    e.Handled = true;
                }
            };
            UpdateResponsiveLayout();
        }

        private bool IsEn { get { return _settings.Language == "en"; } }

        private string GetRoleName(int idx)
        {
            if (idx == 0) return IsEn ? "Launcher" : "Başlatıcı";
            return (IsEn ? "Chapter " : "Bölüm ") + idx;
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                _prevWindowState = WindowState;
                _prevBorderStyle = FormBorderStyle;
                _prevBounds = Bounds;

                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                Bounds = Screen.FromControl(this).Bounds;
                _isFullscreen = true;
            }
            else
            {
                FormBorderStyle = _prevBorderStyle;
                WindowState = _prevWindowState;
                if (_prevWindowState == FormWindowState.Normal)
                    Bounds = _prevBounds;
                _isFullscreen = false;
            }
            ApplyLocalization();
            UpdateResponsiveLayout();
        }

        private void BuildUI()
        {
            _header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = UiTheme.BgHeader
            };
            _header.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(UiTheme.BorderSubtle, 1))
                {
                    e.Graphics.DrawLine(pen, 0, _header.Height - 1, _header.Width, _header.Height - 1);
                }
            };

            _picLogo = new PictureBox
            {
                Size = new Size(32, 32),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            string pngIcon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"utmt-cli\app_icon.png");
            if (File.Exists(pngIcon))
            {
                try { _picLogo.Image = Image.FromFile(pngIcon); } catch { }
            }

            _lblTitle = new Label
            {
                Text = "DELTARUNE",
                Font = new Font("Segoe UI Semibold", 13.5f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true
            };

            _lblSubtitle = new Label
            {
                Text = "Mod Installer  •  UTMT CLI",
                Font = new Font("Segoe UI", 9.25f),
                ForeColor = UiTheme.TextSecondary,
                AutoSize = true
            };

            _btnLangToggle = new ModernButton("EN", false)
            {
                Size = new Size(46, 32),
                Font = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold)
            };
            _btnLangToggle.Click += (s, e) =>
            {
                _settings.Language = IsEn ? "tr" : "en";
                _settings.Save();
                ApplyLocalization();
            };

            _btnSettings = new ModernButton("⚙ Ayarlar", false)
            {
                Size = new Size(92, 32),
                Font = new Font("Segoe UI", 8.5f)
            };
            _btnSettings.Click += (s, e) =>
            {
                using (var dlg = new SettingsDialog(_settings, _txtGamePath.Text.Trim(), () =>
                {
                    _progressBarFill.BackColor = UiTheme.Accent;
                    ApplyLocalization();
                    Invalidate(true);
                }))
                {
                    dlg.ShowDialog(this);
                }
            };

            _btnFullscreen = new ModernButton("Tam Ekran (F11)", false)
            {
                Size = new Size(124, 32),
                Font = new Font("Segoe UI", 8.5f)
            };
            _btnFullscreen.Click += (s, e) => ToggleFullscreen();

            _header.Controls.Add(_picLogo);
            _header.Controls.Add(_lblTitle);
            _header.Controls.Add(_lblSubtitle);
            _header.Controls.Add(_btnLangToggle);
            _header.Controls.Add(_btnSettings);
            _header.Controls.Add(_btnFullscreen);
            Controls.Add(_header);

            _container = new Panel
            {
                BackColor = UiTheme.BgMain
            };
            Controls.Add(_container);

            // Path Bar Card
            _pathCard = new Panel
            {
                Height = 64,
                BackColor = UiTheme.BgMain
            };
            _pathCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(0, 0, _pathCard.Width - 1, _pathCard.Height - 1);
                using (GraphicsPath path = UiTheme.RoundedRect(r, 10))
                using (SolidBrush br = new SolidBrush(UiTheme.BgCard))
                using (Pen pen = new Pen(UiTheme.BorderSubtle, 1f))
                {
                    e.Graphics.FillPath(br, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            _lblPathCaption = new Label
            {
                Text = "Oyun Klasörü",
                Font = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold),
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(18, 9),
                AutoSize = true
            };
            _pathCard.Controls.Add(_lblPathCaption);

            _lblPathStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = UiTheme.Success,
                Location = new Point(140, 9),
                Size = new Size(400, 18),
                TextAlign = ContentAlignment.MiddleRight
            };
            _pathCard.Controls.Add(_lblPathStatus);

            _txtGamePath = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = UiTheme.BgCard,
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(20, 32)
            };
            _txtGamePath.TextChanged += (s, e) => ValidateGamePath();
            _pathCard.Controls.Add(_txtGamePath);

            _btnBrowse = new ModernButton("Klasör Seç", false)
            {
                Size = new Size(96, 34),
                Font = new Font("Segoe UI", 8.75f)
            };
            _btnBrowse.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = IsEn ? "Select DELTARUNE game folder:" : "DELTARUNE oyun klasörünü seçin:";
                    if (Directory.Exists(_txtGamePath.Text))
                        fbd.SelectedPath = _txtGamePath.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                        _txtGamePath.Text = fbd.SelectedPath;
                }
            };
            _pathCard.Controls.Add(_btnBrowse);
            _container.Controls.Add(_pathCard);

            // Mod Cards
            _cardMenu = new ModCard("", "", "", true);
            _container.Controls.Add(_cardMenu);

            _cardFps = new ModCard("", "", "", true);
            _container.Controls.Add(_cardFps);

            _cardBorder = new ModCard("", "", "", true);
            _container.Controls.Add(_cardBorder);

            // Chapter Checkboxes Bar
            _chCard = new Panel
            {
                Height = 40,
                BackColor = UiTheme.BgMain
            };
            _chCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(0, 0, _chCard.Width - 1, _chCard.Height - 1);
                using (GraphicsPath path = UiTheme.RoundedRect(r, 8))
                using (SolidBrush br = new SolidBrush(UiTheme.BgCard))
                using (Pen pen = new Pen(UiTheme.BorderSubtle, 1f))
                {
                    e.Graphics.FillPath(br, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            _chkChapters = new CheckBox[6];
            for (int i = 0; i < 6; i++)
            {
                _chkChapters[i] = new CheckBox
                {
                    Text = GetRoleName(i),
                    Checked = true,
                    AutoSize = true,
                    ForeColor = UiTheme.TextPrimary,
                    BackColor = UiTheme.BgCard,
                    Font = new Font("Segoe UI", 8.75f),
                    Cursor = Cursors.Hand
                };
                _chCard.Controls.Add(_chkChapters[i]);
            }
            _container.Controls.Add(_chCard);

            // Progress Status & Bar
            _lblProgressStatus = new Label
            {
                Text = "Hazır",
                Font = new Font("Segoe UI", 8.75f),
                ForeColor = UiTheme.TextSecondary,
                AutoSize = true
            };
            _container.Controls.Add(_lblProgressStatus);

            _progressBarBg = new Panel
            {
                Height = 6,
                BackColor = Color.FromArgb(24, 29, 42)
            };
            _progressBarFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(0, 6),
                BackColor = UiTheme.Accent
            };
            _progressBarBg.Controls.Add(_progressBarFill);
            _container.Controls.Add(_progressBarBg);

            // Clean Log Console
            _txtLog = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = UiTheme.BgInput,
                ForeColor = UiTheme.TextSecondary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8.5f)
            };
            _container.Controls.Add(_txtLog);

            // Bottom Action Buttons
            _btnRestore = new ModernButton("Orijinale Dön", false)
            {
                Height = 42,
                Font = new Font("Segoe UI Semibold", 9.25f, FontStyle.Bold)
            };
            _btnRestore.Click += async (s, e) => await RestoreBackupsAsync();
            _container.Controls.Add(_btnRestore);

            _btnInstall = new ModernButton("Seçili Modları Kur", true)
            {
                Height = 42,
                Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold)
            };
            _btnInstall.Click += async (s, e) => await InstallSelectedModsAsync();
            _container.Controls.Add(_btnInstall);

            AppendLog(IsEn
                ? "Ready. Select your desired mod combination and click Install."
                : "Hazır. İstediğiniz mod kombinasyonunu seçip kurulumu başlatabilirsiniz.");
        }

        private void ApplyLocalization()
        {
            bool en = IsEn;
            _lblSubtitle.Text = en ? "Mod Installer  •  UTMT CLI" : "Mod Kurulum Aracı  •  UTMT CLI";
            _btnLangToggle.Text = en ? "TR" : "EN";
            _btnSettings.Text = en ? "⚙ Settings" : "⚙ Ayarlar";
            _btnFullscreen.Text = _isFullscreen
                ? (en ? "Exit Fullscreen (F11)" : "Küçült (F11)")
                : (en ? "Fullscreen (F11)" : "Tam Ekran (F11)");

            _lblPathCaption.Text = en ? "Game Directory" : "Oyun Klasörü";
            _btnBrowse.Text = en ? "Browse..." : "Klasör Seç";

            _cardMenu.Title = en ? "Redesigned Menu & Turkish Mod" : "Yenilenmiş Türkçe Menü";
            _cardMenu.TagText = en ? "UI & Turkish Font" : "Arayüz & Türkçe";
            _cardMenu.Description = en
                ? "Horizontal card UI for chapter & save selection, animated chapter backgrounds, and Turkish pixel font."
                : "Yatay kart tasarımlı bölüm ve kayıt seçimi, bölüme özel animasyonlu arka planlar ve Türkçe font.";
            _cardMenu.Invalidate();

            _cardFps.Title = en ? "60 FPS Mod" : "60 FPS Modu";
            _cardFps.TagText = en ? "Performance" : "Performans";
            _cardFps.Description = en
                ? "Upgrades all movement, battles, cutscenes, and UI animations to smooth 60 FPS while preserving game speed."
                : "Oyun hızını değiştirmeden tüm hareket, savaş, ara sahne ve animasyonları 60 FPS akıcılığına yükseltir.";
            _cardFps.Invalidate();

            _cardBorder.Title = en ? "Console Border Mod (NXRUNE)" : "Konsol Border Modu";
            _cardBorder.TagText = en ? "16:9 Widescreen" : "16:9 Çerçeve";
            _cardBorder.Description = en
                ? "16:9 widescreen window, dynamic console borders per chapter, and Border option in the CONFIG menu."
                : "Geniş ekran pencere görünümü, konsol sürümündeki dinamik kenarlıklar ve ayarlar menüsü seçeneği.";
            _cardBorder.Invalidate();

            for (int i = 0; i < 6; i++)
            {
                if (_chkChapters != null && _chkChapters[i] != null)
                    _chkChapters[i].Text = GetRoleName(i);
            }

            if (!_isWorking)
                _lblProgressStatus.Text = en ? "Ready to install" : "Kuruluma hazır";

            _btnRestore.Text = en ? "Restore Original" : "Orijinale Dön";
            _btnInstall.Text = en ? "Install Selected Mods" : "Seçili Modları Kur";

            ValidateGamePath();
            UpdateResponsiveLayout();
        }

        private void UpdateResponsiveLayout()
        {
            if (_container == null) return;

            int clientW = ClientSize.Width;
            int clientH = ClientSize.Height;

            int contentW = Math.Max(680, Math.Min(clientW - 56, 920));
            int leftMargin = (clientW - contentW) / 2;

            // Header positioning
            _picLogo.Location = new Point(leftMargin, 16);
            _lblTitle.Location = new Point(leftMargin + 40, 18);
            _lblSubtitle.Location = new Point(leftMargin + 40 + _lblTitle.Width + 10, 23);

            int rightX = leftMargin + contentW;
            _btnFullscreen.Location = new Point(rightX - _btnFullscreen.Width, 16);
            _btnSettings.Location = new Point(_btnFullscreen.Left - _btnSettings.Width - 8, 16);
            _btnLangToggle.Location = new Point(_btnSettings.Left - _btnLangToggle.Width - 8, 16);

            // Container bounds
            int topY = _header.Height + 16;
            int containerH = Math.Max(480, clientH - topY - 20);
            _container.Bounds = new Rectangle(leftMargin, topY, contentW, containerH);

            int y = 0;

            // 1. Path Card
            _pathCard.Bounds = new Rectangle(0, y, contentW, 62);
            _btnBrowse.Location = new Point(contentW - _btnBrowse.Width - 14, 14);
            _txtGamePath.Width = contentW - _btnBrowse.Width - 48;
            _lblPathStatus.Location = new Point(150, 9);
            _lblPathStatus.Width = contentW - _btnBrowse.Width - 174;
            y += 74;

            // 2. Mod Cards
            _cardMenu.Bounds = new Rectangle(0, y, contentW, 68);
            y += 78;
            _cardFps.Bounds = new Rectangle(0, y, contentW, 68);
            y += 78;
            _cardBorder.Bounds = new Rectangle(0, y, contentW, 68);
            y += 80;

            // 3. Chapters Bar
            _chCard.Bounds = new Rectangle(0, y, contentW, 40);
            int stepX = (contentW - 28) / 6;
            for (int i = 0; i < 6; i++)
            {
                _chkChapters[i].Location = new Point(18 + (i * stepX), 10);
            }
            y += 52;

            // 4. Progress
            _lblProgressStatus.Location = new Point(2, y);
            y += 22;
            _progressBarBg.Bounds = new Rectangle(0, y, contentW, 6);
            _progressBarFill.Width = (_progressBarBg.Width * _progressPercent) / 100;
            y += 14;

            // 5. Bottom Buttons
            int btnY = containerH - 42;
            int restoreW = 180;
            _btnRestore.Bounds = new Rectangle(0, btnY, restoreW, 42);
            _btnInstall.Bounds = new Rectangle(restoreW + 12, btnY, contentW - restoreW - 12, 42);

            // 6. Log Box expands vertically
            int logH = Math.Max(70, btnY - y - 14);
            _txtLog.Bounds = new Rectangle(0, y, contentW, logH);
        }

        private void AutoDetectGamePath()
        {
            var candidates = new List<string>();

            string appDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            candidates.Add(appDir);
            DirectoryInfo parent = Directory.GetParent(appDir);
            if (parent != null)
            {
                candidates.Add(parent.FullName);
                candidates.Add(Path.Combine(parent.FullName, "DELTARUNE"));
                if (parent.Parent != null)
                    candidates.Add(Path.Combine(parent.Parent.FullName, "DELTARUNE"));
            }

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam") ??
                                 Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam") ??
                                 Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                {
                    if (key != null)
                    {
                        object val1 = key.GetValue("InstallPath");
                        object val2 = key.GetValue("SteamPath");
                        string steamPath = (val1 != null) ? val1.ToString() : ((val2 != null) ? val2.ToString() : null);
                        if (!string.IsNullOrEmpty(steamPath))
                        {
                            steamPath = steamPath.Replace('/', '\\');
                            candidates.Add(Path.Combine(steamPath, @"steamapps\common\DELTARUNE"));
                        }
                    }
                }
            }
            catch { }

            foreach (var drive in new[] { "C", "D", "E", "F" })
            {
                candidates.Add(drive + @":\Program Files (x86)\Steam\steamapps\common\DELTARUNE");
                candidates.Add(drive + @":\SteamLibrary\steamapps\common\DELTARUNE");
                candidates.Add(drive + @":\Games\DELTARUNE");
            }

            foreach (var c in candidates)
            {
                if (IsValidDeltaruneDir(c))
                {
                    _txtGamePath.Text = c;
                    return;
                }
            }
        }

        private bool IsValidDeltaruneDir(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir)) return false;
            return File.Exists(Path.Combine(dir, "data.win")) ||
                   File.Exists(Path.Combine(dir, @"chapter1_windows\data.win"));
        }

        private void ValidateGamePath()
        {
            if (_txtGamePath == null || _lblPathStatus == null) return;
            string dir = _txtGamePath.Text.Trim();
            if (!IsValidDeltaruneDir(dir))
            {
                _lblPathStatus.Text = IsEn ? "● No valid DELTARUNE folder selected" : "● Geçerli bir DELTARUNE klasörü seçilmedi";
                _lblPathStatus.ForeColor = UiTheme.Danger;
                return;
            }

            int found = 0;
            for (int i = 0; i < _roleRelPaths.Length; i++)
            {
                bool exists = File.Exists(Path.Combine(dir, _roleRelPaths[i]));
                if (exists) found++;
                if (_chkChapters != null && _chkChapters[i] != null)
                {
                    _chkChapters[i].Enabled = exists && !_isWorking;
                    if (!exists) _chkChapters[i].Checked = false;
                }
            }

            string backupDir = Path.Combine(dir, "mod_backups");
            bool hasBackup = Directory.Exists(backupDir);
            if (IsEn)
                _lblPathStatus.Text = string.Format("● {0}/6 chapters ready{1}", found, hasBackup ? "  •  Backup available" : "");
            else
                _lblPathStatus.Text = string.Format("● {0}/6 bölüm hazır{1}", found, hasBackup ? "  •  Yedek mevcut" : "");
            _lblPathStatus.ForeColor = UiTheme.Success;
        }

        private void SetProgress(int percent, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetProgress(percent, status)));
                return;
            }
            _progressPercent = Math.Max(0, Math.Min(100, percent));
            _progressBarFill.Width = (_progressBarBg.Width * _progressPercent) / 100;
            _lblProgressStatus.Text = status;
        }

        private void AppendLog(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendLog(msg)));
                return;
            }
            string line = string.Format("[{0:HH:mm:ss}] {1}{2}", DateTime.Now, msg, Environment.NewLine);
            _txtLog.AppendText(line);
        }

        private void SetWorkingState(bool working)
        {
            _isWorking = working;
            _txtGamePath.Enabled = !working;
            _btnBrowse.Enabled = !working;
            _cardMenu.Enabled = !working;
            _cardFps.Enabled = !working;
            _cardBorder.Enabled = !working;
            _btnInstall.Enabled = !working;
            _btnRestore.Enabled = !working;
            _btnSettings.Enabled = !working;
            for (int i = 0; i < _chkChapters.Length; i++)
            {
                bool exists = IsValidDeltaruneDir(_txtGamePath.Text.Trim()) &&
                              File.Exists(Path.Combine(_txtGamePath.Text.Trim(), _roleRelPaths[i]));
                _chkChapters[i].Enabled = !working && exists;
            }
        }

        private async Task InstallSelectedModsAsync()
        {
            bool en = IsEn;
            string gameDir = _txtGamePath.Text.Trim();
            if (!IsValidDeltaruneDir(gameDir))
            {
                MessageBox.Show(
                    en ? "Please select a valid DELTARUNE game directory." : "Lütfen geçerli bir DELTARUNE oyun klasörü seçin.",
                    en ? "Warning" : "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            bool modMenu = _cardMenu.IsChecked;
            bool modFps = _cardFps.IsChecked;
            bool modBorder = _cardBorder.IsChecked;

            if (!modMenu && !modFps && !modBorder)
            {
                MessageBox.Show(
                    en ? "Please select at least 1 mod to install." : "Kurulum için en az 1 mod seçmelisiniz.",
                    en ? "No Mod Selected" : "Mod Seçilmedi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var selectedRoles = new List<int>();
            for (int i = 0; i < _chkChapters.Length; i++)
            {
                if (_chkChapters[i].Checked && File.Exists(Path.Combine(gameDir, _roleRelPaths[i])))
                    selectedRoles.Add(i);
            }

            if (selectedRoles.Count == 0)
            {
                MessageBox.Show(
                    en ? "Please select at least one chapter to patch." : "Lütfen uygulanacak en az bir bölüm seçin.",
                    en ? "No Chapter Selected" : "Bölüm Seçilmedi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var activeModNames = new List<string>();
            if (modMenu) activeModNames.Add(_cardMenu.Title);
            if (modFps) activeModNames.Add(_cardFps.Title);
            if (modBorder) activeModNames.Add(_cardBorder.Title);

            if (_settings.ConfirmBeforeInstall)
            {
                string confirmMsg = (en ? "The following mods will be installed:\n\n• " : "Aşağıdaki modlar kurulacak:\n\n• ") +
                                    string.Join("\n• ", activeModNames);
                if (MessageBox.Show(confirmMsg, en ? "Confirm Installation" : "Kurulum Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string utmtExe = Path.Combine(appDir, @"utmt-cli\utmt-agent.exe");
            string modsDir = Path.Combine(appDir, "mods");
            string patcherCsx = Path.Combine(modsDir, "patcher.csx");

            if (!File.Exists(utmtExe))
            {
                MessageBox.Show("UTMT CLI not found:\n" + utmtExe, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetWorkingState(true);
            _txtLog.Clear();
            AppendLog((en ? "Selected Mods: " : "Seçili Modlar: ") + string.Join(" + ", activeModNames));

            string backupRoot = Path.Combine(gameDir, "mod_backups");
            string tempWorkDir = Path.Combine(Path.GetTempPath(), "DeltaruneModInstaller_" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(tempWorkDir);

            try
            {
                int totalSteps = selectedRoles.Count * ((modFps && (modBorder || modMenu)) ? 2 : 1);
                int currentStep = 0;

                foreach (int idx in selectedRoles)
                {
                    string role = _roleKeys[idx];
                    string roleTitle = GetRoleName(idx);
                    string targetWin = Path.Combine(gameDir, _roleRelPaths[idx]);
                    string backupWin = Path.Combine(backupRoot, _roleRelPaths[idx]);

                    Directory.CreateDirectory(Path.GetDirectoryName(backupWin));
                    if (!File.Exists(backupWin))
                    {
                        AppendLog("[" + roleTitle + "] " + (en ? "Backing up original data.win -> mod_backups\\" : "Orijinal data.win yedekleniyor -> mod_backups\\") + _roleRelPaths[idx]);
                        await Task.Run(() => File.Copy(targetWin, backupWin, false));
                    }
                    else
                    {
                        AppendLog("[" + roleTitle + "] " + (en ? "Starting clean patch from backup..." : "Temiz yedek üzerinden kuruluma başlanıyor..."));
                    }

                    string currentInputWin = backupWin;

                    if (modFps)
                    {
                        currentStep++;
                        int pct = (currentStep * 100) / (totalSteps + 1);
                        SetProgress(pct, roleTitle + (en ? " • Applying 60 FPS Mod..." : " • 60 FPS Modu uygulanıyor..."));
                        string fpsCsx = Path.Combine(modsDir, "60fps", _fpsScripts[idx]);
                        string stage1Out = (modBorder || modMenu)
                            ? Path.Combine(tempWorkDir, role + "_stage1.win")
                            : targetWin;

                        AppendLog("[" + roleTitle + "] UTMT CLI -> " + _fpsScripts[idx]);
                        bool ok = await RunUtmtScriptWorkerAsync(utmtExe, currentInputWin, stage1Out, fpsCsx, tempWorkDir, modsDir, role, modFps, modBorder, modMenu);
                        if (!ok)
                            throw new Exception(roleTitle + (en ? ": Failed to apply 60 FPS mod." : " üzerinde 60 FPS modu uygulanırken hata oluştu."));

                        currentInputWin = stage1Out;
                    }

                    if (modBorder || modMenu)
                    {
                        currentStep++;
                        int pct = (currentStep * 100) / (totalSteps + 1);
                        SetProgress(pct, roleTitle + (en ? " • Applying mod patches..." : " • Yamalar uygulanıyor..."));

                        AppendLog("[" + roleTitle + "] UTMT CLI -> patcher.csx");
                        bool ok = await RunUtmtScriptWorkerAsync(utmtExe, currentInputWin, targetWin, patcherCsx, tempWorkDir, modsDir, role, modFps, modBorder, modMenu);
                        if (!ok)
                            throw new Exception(roleTitle + (en ? ": Failed to apply mod patch." : " üzerinde mod yaması uygulanırken hata oluştu."));
                    }

                    AppendLog("[" + roleTitle + "] ✓ " + (en ? "Completed" : "Tamamlandı"));
                }

                SetProgress(100, en ? "✓ Installation complete" : "✓ Kurulum tamamlandı");
                AppendLog(en ? "All selected mods installed successfully!" : "Tüm seçili modlar başarıyla kuruldu!");
                ValidateGamePath();

                if (_settings.AutoLaunchGame)
                {
                    string gameExe = Path.Combine(gameDir, "DELTARUNE.exe");
                    if (File.Exists(gameExe))
                    {
                        AppendLog(en ? "Launching DELTARUNE.exe..." : "DELTARUNE.exe başlatılıyor...");
                        Process.Start(new ProcessStartInfo { FileName = gameExe, WorkingDirectory = gameDir });
                    }
                }
                else
                {
                    MessageBox.Show(
                        (en ? "Selected mods have been installed successfully!\n\nActive Mods:\n• " : "Seçtiğiniz modlar başarıyla kuruldu!\n\nAktif Modlar:\n• ") +
                        string.Join("\n• ", activeModNames),
                        en ? "Installation Complete" : "Kurulum Tamamlandı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                SetProgress(0, en ? "✗ Error occurred" : "✗ Hata oluştu");
                AppendLog("ERROR: " + ex.Message);
                MessageBox.Show(ex.Message, en ? "Installation Error" : "Kurulum Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try { if (Directory.Exists(tempWorkDir)) Directory.Delete(tempWorkDir, true); } catch { }
                try { File.Delete(Path.Combine(modsDir, "patch_config.txt")); } catch { }
                SetWorkingState(false);
            }
        }

        private async Task<bool> RunUtmtScriptWorkerAsync(
            string utmtExe,
            string dataPath,
            string outPath,
            string scriptPath,
            string workDir,
            string modsDir,
            string role,
            bool fps60,
            bool border,
            bool menu)
        {
            string cfgPath = Path.Combine(modsDir, "patch_config.txt");
            File.WriteAllText(cfgPath,
                "ROLE=" + role + "\n" +
                "FPS60=" + (fps60 ? "1" : "0") + "\n" +
                "BORDER=" + (border ? "1" : "0") + "\n" +
                "MENU=" + (menu ? "1" : "0") + "\n",
                new UTF8Encoding(false));

            string jobPath = Path.Combine(workDir, string.Format("job_{0}_{1:N}.json", role, Guid.NewGuid()));
            string json = "{\n" +
                "  \"data_path\": \"" + EscapeJson(dataPath) + "\",\n" +
                "  \"out_path\": \"" + EscapeJson(outPath) + "\",\n" +
                "  \"script_path\": \"" + EscapeJson(scriptPath) + "\",\n" +
                "  \"exports_dir\": \"" + EscapeJson(workDir) + "\",\n" +
                "  \"compile_only\": false\n" +
                "}";
            File.WriteAllText(jobPath, json, new UTF8Encoding(false));

            var psi = new ProcessStartInfo
            {
                FileName = utmtExe,
                Arguments = "script-worker \"" + jobPath + "\"",
                WorkingDirectory = Path.GetDirectoryName(utmtExe),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            psi.EnvironmentVariables["DR_ROLE"] = role;
            psi.EnvironmentVariables["DR_MOD_FPS60"] = fps60 ? "1" : "0";
            psi.EnvironmentVariables["DR_MOD_BORDER"] = border ? "1" : "0";
            psi.EnvironmentVariables["DR_MOD_MENU"] = menu ? "1" : "0";

            bool verbose = _settings.VerboseLog;
            bool succeeded = false;
            await Task.Run(() =>
            {
                using (var proc = new Process { StartInfo = psi })
                {
                    proc.OutputDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrWhiteSpace(e.Data)) return;
                        string line = e.Data.Trim();
                        if (line.Contains("\"kind\":\"message\""))
                        {
                            var m = Regex.Match(line, "\"text\":\"([^\"]*)\"");
                            if (m.Success) AppendLog("  " + Regex.Unescape(m.Groups[1].Value));
                        }
                        else if (verbose && line.Contains("\"kind\":\"progress\""))
                        {
                            AppendLog("  [UTMT] " + line);
                        }
                        else if (line.Contains("\"kind\":\"error\""))
                        {
                            AppendLog("  [UTMT ERROR] " + line);
                        }
                        else if (line.Contains("\"kind\":\"result\"") && line.Contains("\"ok\":true"))
                        {
                            succeeded = true;
                        }
                    };
                    proc.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                            AppendLog("  [STDERR] " + e.Data.Trim());
                    };

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit();

                    if (proc.ExitCode == 0 && File.Exists(outPath))
                        succeeded = true;
                }
            });

            return succeeded;
        }

        private async Task RestoreBackupsAsync()
        {
            bool en = IsEn;
            string gameDir = _txtGamePath.Text.Trim();
            string backupRoot = Path.Combine(gameDir, "mod_backups");
            if (!Directory.Exists(backupRoot))
            {
                MessageBox.Show(
                    en ? "No 'mod_backups' folder found in the game directory." : "Oyun klasöründe 'mod_backups' yedek klasörü bulunamadı.",
                    en ? "No Backup" : "Yedek Yok",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                en ? "Original data.win files will be restored from backup and all mods will be removed.\nDo you want to continue?"
                   : "Yedeklenen orijinal data.win dosyaları geri yüklenecek ve tüm modlar kaldırılacak.\nDevam etmek istiyor musunuz?",
                en ? "Restore Original" : "Orijinale Dön",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            SetWorkingState(true);
            _txtLog.Clear();
            AppendLog(en ? "Restoring original backups..." : "Orijinal yedekler geri yükleniyor...");

            try
            {
                int restored = 0;
                for (int i = 0; i < _roleRelPaths.Length; i++)
                {
                    string backupWin = Path.Combine(backupRoot, _roleRelPaths[i]);
                    string targetWin = Path.Combine(gameDir, _roleRelPaths[i]);
                    if (File.Exists(backupWin))
                    {
                        string rName = GetRoleName(i);
                        SetProgress(((i + 1) * 100) / _roleRelPaths.Length, rName + (en ? " restoring..." : " geri yükleniyor..."));
                        await Task.Run(() => File.Copy(backupWin, targetWin, true));
                        AppendLog("[" + rName + "] ✓ " + (en ? "Original data.win restored" : "Orijinal data.win geri yüklendi"));
                        restored++;
                    }
                }

                SetProgress(100, "✓ " + restored + (en ? " chapters restored to original" : " bölüm orijinal haline döndürüldü"));
                MessageBox.Show(
                    restored + (en ? " chapters have been restored to original." : " bölümün orijinal dosyaları başarıyla geri yüklendi."),
                    en ? "Restore Complete" : "İşlem Tamamlandı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorkingState(false);
            }
        }

        private static string EscapeJson(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
