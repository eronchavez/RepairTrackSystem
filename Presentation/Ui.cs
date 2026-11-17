namespace RepairTrack.Presentation;

public static class Ui
{
    public static readonly Color WindowBackground = ColorTranslator.FromHtml("#f0f2f5");
    public static readonly Color PanelBackground = ColorTranslator.FromHtml("#ffffff");
    public static readonly Color TableBackground = ColorTranslator.FromHtml("#ffffff");
    public static readonly Color TableRowAlternate = ColorTranslator.FromHtml("#f8fafc");
    public static readonly Color TableHeader = ColorTranslator.FromHtml("#f0f2f5");
    public static readonly Color TableRowHover = ColorTranslator.FromHtml("#eff6ff");
    public static readonly Color DisabledBackground = ColorTranslator.FromHtml("#f3f5f7");
    public static readonly Color FooterBackground = ColorTranslator.FromHtml("#fafbfc");

    public static readonly Color TextPrimary = ColorTranslator.FromHtml("#111827");
    public static readonly Color TextDefault = ColorTranslator.FromHtml("#374151");
    public static readonly Color TextSecondary = ColorTranslator.FromHtml("#6b7280");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#818b9b");
    public static readonly Color TextPlaceholder = ColorTranslator.FromHtml("#929aa7");
    public static readonly Color TextOnPrimary = ColorTranslator.FromHtml("#ffffff");

    public static readonly Color Border = ColorTranslator.FromHtml("#d1d5db");
    public static readonly Color DividerLight = ColorTranslator.FromHtml("#e3e7ed");

    public static readonly Color Primary = ColorTranslator.FromHtml("#2563eb");
    public static readonly Color PrimaryHover = ColorTranslator.FromHtml("#1d55cc");
    public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#1e40af");
    public static readonly Color Link = ColorTranslator.FromHtml("#1f5fd3");
    public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#dbeafe");
    public static readonly Color PrimarySubtle = ColorTranslator.FromHtml("#eff6ff");
    public static readonly Color PrimaryBorder = ColorTranslator.FromHtml("#93c5fd");

    public static readonly Color StatusReceivedBack = ColorTranslator.FromHtml("#e5e7eb");
    public static readonly Color StatusReceivedText = ColorTranslator.FromHtml("#374155");
    public static readonly Color StatusRepairBack = ColorTranslator.FromHtml("#dbeafe");
    public static readonly Color StatusRepairText = ColorTranslator.FromHtml("#1e40af");
    public static readonly Color StatusReadyBack = ColorTranslator.FromHtml("#d1fae5");
    public static readonly Color StatusReadyText = ColorTranslator.FromHtml("#065f46");
    public static readonly Color StatusCompletedBack = ColorTranslator.FromHtml("#e0e7ff");
    public static readonly Color StatusCompletedText = ColorTranslator.FromHtml("#3730a3");

    public static readonly Color Positive = ColorTranslator.FromHtml("#065f46");
    public static readonly Color BalanceDue = ColorTranslator.FromHtml("#b54708");
    public static readonly Color Required = ColorTranslator.FromHtml("#d92d20");

    public static readonly Color SuccessText = ColorTranslator.FromHtml("#087764");
    public static readonly Color SuccessBackground = ColorTranslator.FromHtml("#e7f8f2");
    public static readonly Color SuccessBorder = ColorTranslator.FromHtml("#b8eadb");

    public static readonly Color IconDefault = ColorTranslator.FromHtml("#667286");
    public static readonly Color IconPrimary = ColorTranslator.FromHtml("#2563eb");
    public static readonly Color IconPrimaryBackground = ColorTranslator.FromHtml("#dbeafe");
    public static readonly Color IconNeutralBackground = ColorTranslator.FromHtml("#edf0f3");

    public static readonly Color Offline = ColorTranslator.FromHtml("#22a06b");
    public static readonly Color OfflineRing = ColorTranslator.FromHtml("#dcfce7");

    public static readonly Color FocusRing = ColorTranslator.FromHtml("#2563eb");
    public static readonly Color ShadowFocus = Color.FromArgb(20, 37, 99, 235);
    public static readonly Color ShadowSearchResults = Color.FromArgb(31, 17, 24, 39);
    public static readonly Color InsetHighlight = Color.FromArgb(36, 255, 255, 255);

    public static readonly Font TitleFont = new("Segoe UI", 18, FontStyle.Bold);
    public static readonly Font HeadingFont = new("Segoe UI", 14, FontStyle.Bold);
    public static readonly Font SectionFont = new("Segoe UI", 12, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 10);
    public static readonly Font SubtitleFont = new("Segoe UI", 9);
    public static readonly Font SmallFont = new("Segoe UI", 8);
    public static readonly Font CardTitleFont = new("Segoe UI", 9, FontStyle.Bold);
    public static readonly Font CardNumberFont = new("Segoe UI", 20, FontStyle.Bold);
    public static readonly Font GridHeaderFont = new("Segoe UI", 8, FontStyle.Bold);
    public static readonly Font StatusFont = new("Segoe UI", 9, FontStyle.Bold);

    public const string Home = "\uE80F";
    public const string People = "\uE716";
    public const string Document = "\uE8A5";
    public const string Search = "\uE721";
    public const string Add = "\uE710";
    public const string Edit = "\uE70F";
    public const string Eye = "\uE7B3";
    public const string EyeOff = "\uE18B";
    public const string Check = "\uE73E";
    public const string Repair = "\uE90F";
    public const string Package = "\uE7B8";
    public const string Back = "\uE72B";
    public const string Lock = "\uE72E";
    public const string Wallet = "\uE8C7";

    private static FontFamily? _iconFamily;

    public static Font IconFont(float size)
    {
        return new Font(GetIconFamily(), size);
    }

    private static FontFamily GetIconFamily()
    {
        if (_iconFamily != null)
            return _iconFamily;
        foreach (var family in FontFamily.Families)
        {
            if (family.Name == "Segoe Fluent Icons")
            {
                _iconFamily = family;
                return _iconFamily;
            }
        }
        foreach (var family in FontFamily.Families)
        {
            if (family.Name == "Segoe MDL2 Assets")
            {
                _iconFamily = family;
                return _iconFamily;
            }
        }
        _iconFamily = FontFamily.GenericSansSerif;
        return _iconFamily;
    }

    public static Bitmap IconBitmap(string glyph, Color color, int size)
    {
        var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        using var font = IconFont(Math.Max(8, size - 2));
        using var brush = new SolidBrush(color);
        var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), format);
        return bmp;
    }

    public static Label IconLabel(string glyph, float size, Color color)
    {
        return new Label
        {
            Text = glyph,
            Font = IconFont(size),
            ForeColor = color,
            AutoSize = true,
            BackColor = Color.Transparent
        };
    }

    public static Panel IconBadge(string glyph, Color iconColor, Color backColor, int size = 32)
    {
        var panel = new Panel { Width = size, Height = size, BackColor = backColor };
        panel.Controls.Add(new Label
        {
            Text = glyph,
            Font = IconFont(size * 0.45f),
            ForeColor = iconColor,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });
        return panel;
    }

    public static void Open(Control from, UserControl screen)
    {
        if (from.FindForm() is MainForm main)
            main.ShowScreen(screen);
    }

    public static bool TryParseMoney(string text, out decimal amount)
    {
        string raw = text.Replace("₱", "").Replace(",", "").Trim();
        return decimal.TryParse(raw, System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture, out amount);
    }

    public static int? SelectedId(DataGridView grid)
    {
        if (grid.CurrentRow?.Tag is int id)
            return id;
        return null;
    }

    public static Label FieldLabel(string caption, bool required = false)
    {
        var label = new Label
        {
            AutoSize = true,
            Font = SubtitleFont,
            ForeColor = TextSecondary
        };
        if (required)
        {
            label.Text = caption + " ";
        }
        else
        {
            label.Text = caption;
        }
        return label;
    }

    public static Label RequiredMark()
    {
        return new Label
        {
            Text = "*",
            AutoSize = true,
            Font = SubtitleFont,
            ForeColor = Required
        };
    }

    public static void AddCaption(Control parent, string caption, bool required, Point location)
    {
        var label = FieldLabel(caption, required);
        label.Location = location;
        parent.Controls.Add(label);
        if (required)
        {
            var star = RequiredMark();
            star.Location = new Point(location.X + label.PreferredWidth, location.Y);
            parent.Controls.Add(star);
        }
    }

    public static void ShowPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("No password is stored for this device.", "Device password",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        MessageBox.Show(password, "Device password", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static TextBox Input(int width = 360)
    {
        return new TextBox
        {
            Width = width,
            Height = 28,
            Font = BodyFont,
            ForeColor = TextDefault,
            BackColor = PanelBackground,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    public static string DeviceTitle(string deviceType, string? brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            return deviceType;
        return brand + " " + deviceType;
    }

    public static string Money(decimal amount)
    {
        return "₱" + amount.ToString("#,##0.00");
    }

    public static Color StatusBack(string status)
    {
        return status switch
        {
            "Received" => StatusReceivedBack,
            "In Repair" => StatusRepairBack,
            "Ready for Pickup" => StatusReadyBack,
            "Completed" => StatusCompletedBack,
            _ => TableBackground
        };
    }

    public static Color StatusFore(string status)
    {
        return status switch
        {
            "Received" => StatusReceivedText,
            "In Repair" => StatusRepairText,
            "Ready for Pickup" => StatusReadyText,
            "Completed" => StatusCompletedText,
            _ => TextDefault
        };
    }

    public static Color BalanceColor(decimal balance)
    {
        return balance > 0 ? BalanceDue : Positive;
    }

    public static Button PrimaryButton(string text, int width = 150, string? icon = null)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = 38,
            BackColor = Primary,
            ForeColor = TextOnPrimary,
            Font = BodyFont,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = PrimaryHover;
        button.GotFocus += (s, e) => button.FlatAppearance.BorderColor = FocusRing;
        if (icon != null)
            button.Image = IconBitmap(icon, TextOnPrimary, 16);
        return button;
    }

    public static Button SecondaryButton(string text, int width = 130, string? icon = null)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = 38,
            BackColor = PanelBackground,
            ForeColor = TextDefault,
            Font = BodyFont,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft
        };
        button.FlatAppearance.BorderColor = Border;
        button.GotFocus += (s, e) => button.FlatAppearance.BorderColor = FocusRing;
        button.LostFocus += (s, e) => button.FlatAppearance.BorderColor = Border;
        if (icon != null)
            button.Image = IconBitmap(icon, IconDefault, 16);
        return button;
    }

    public static Panel Header(string title, string subtitle, params Button[] rightButtons)
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 64 };
        header.Controls.Add(new Label
        {
            Text = title,
            Font = TitleFont,
            ForeColor = TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        header.Controls.Add(new Label
        {
            Text = subtitle,
            Font = SubtitleFont,
            ForeColor = TextSecondary,
            AutoSize = true,
            Location = new Point(2, 36)
        });
        foreach (var b in rightButtons)
            header.Controls.Add(b);

        header.Resize += (s, e) =>
        {
            int x = header.Width;
            for (int i = rightButtons.Length - 1; i >= 0; i--)
            {
                x -= rightButtons[i].Width;
                rightButtons[i].Location = new Point(x, 8);
                x -= 8;
            }
        };
        return header;
    }

    public static DataGridView Grid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            BorderStyle = BorderStyle.None,
            BackgroundColor = TableBackground,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeight = 36,
            RowTemplate = { Height = 38 },
            GridColor = DividerLight
        };
        grid.ColumnHeadersDefaultCellStyle.BackColor = TableHeader;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
        grid.ColumnHeadersDefaultCellStyle.Font = GridHeaderFont;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = TableHeader;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextMuted;
        grid.DefaultCellStyle.BackColor = TableBackground;
        grid.DefaultCellStyle.ForeColor = TextDefault;
        grid.DefaultCellStyle.Font = BodyFont;
        grid.DefaultCellStyle.SelectionBackColor = TableRowHover;
        grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
        grid.AlternatingRowsDefaultCellStyle.BackColor = TableRowAlternate;
        grid.CellFormatting += FormatGridCell;
        grid.CellMouseEnter += OnGridRowHover;
        grid.MouseLeave += OnGridRowLeave;
        return grid;
    }

    private static void OnGridRowHover(object? sender, DataGridViewCellEventArgs e)
    {
        if (sender is not DataGridView grid || e.RowIndex < 0)
            return;
        ClearHover(grid);
        grid.Tag = e.RowIndex;
        if (!grid.Rows[e.RowIndex].Selected)
            grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = TableRowHover;
    }

    private static void OnGridRowLeave(object? sender, EventArgs e)
    {
        if (sender is DataGridView grid)
            ClearHover(grid);
    }

    private static void ClearHover(DataGridView grid)
    {
        if (grid.Tag is int prev && prev >= 0 && prev < grid.Rows.Count)
            grid.Rows[prev].DefaultCellStyle.BackColor = Color.Empty;
        grid.Tag = -1;
    }

    private static void FormatGridCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView grid)
            return;
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        string name = grid.Columns[e.ColumnIndex].Name;
        string text = e.Value?.ToString() ?? "";

        if (name == "status")
        {
            Color back = StatusBack(text);
            Color fore = StatusFore(text);
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;
            e.CellStyle.SelectionBackColor = back;
            e.CellStyle.SelectionForeColor = fore;
            e.CellStyle.Font = StatusFont;
        }
        else if (name == "balance")
        {
            string raw = text.Replace("₱", "").Replace(",", "").Trim();
            if (decimal.TryParse(raw, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal value))
            {
                Color color = BalanceColor(value);
                e.CellStyle.ForeColor = color;
                e.CellStyle.SelectionForeColor = color;
            }
        }
    }
}
