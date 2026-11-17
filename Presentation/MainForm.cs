using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class MainForm : Form
{
    private readonly Panel _sidebar = new() { Dock = DockStyle.Left, Width = 200, BackColor = Ui.PanelBackground };
    private readonly Panel _topBar = new() { Dock = DockStyle.Top, Height = 60, BackColor = Ui.PanelBackground };
    private readonly Panel _content = new() { Dock = DockStyle.Fill, BackColor = Ui.WindowBackground };
    private readonly Panel _searchWrap = new()
    {
        Location = new Point(20, 12),
        Size = new Size(440, 36),
        BackColor = Ui.Border,
        Padding = new Padding(1)
    };
    private readonly TextBox _search = new()
    {
        PlaceholderText = "Search customer name, phone, or serial #...",
        Font = Ui.BodyFont,
        BackColor = Ui.PanelBackground,
        ForeColor = Ui.TextDefault,
        BorderStyle = BorderStyle.None,
        Dock = DockStyle.Fill
    };
    private readonly ListBox _results = new()
    {
        Font = Ui.BodyFont,
        IntegralHeight = false,
        BorderStyle = BorderStyle.None,
        Dock = DockStyle.Fill
    };
    private readonly Panel _resultsPanel = new()
    {
        Visible = false,
        Width = 440,
        Height = 240,
        BackColor = Ui.PanelBackground
    };
    private readonly Panel _resultsBottomLine = new()
    {
        Dock = DockStyle.Bottom,
        Height = 2,
        BackColor = Ui.ShadowSearchResults
    };
    private readonly SearchService _searchService = new();
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 250 };
    private readonly List<Button> _menuButtons = new();

    public MainForm()
    {
        BuildUi();
        ShowScreen(new DashboardScreen());
        SetActive(_menuButtons[0]);
    }

    private void BuildUi()
    {
        Text = "Repair Track";
        Width = 1200;
        Height = 750;
        StartPosition = FormStartPosition.CenterScreen;
        Font = Ui.BodyFont;
        BackColor = Ui.WindowBackground;

        BuildSidebar();
        BuildTopBar();
        BuildSearchResults();

        Controls.Add(_content);
        Controls.Add(_topBar);
        Controls.Add(_sidebar);
        Controls.Add(_resultsPanel);
        _resultsPanel.BringToFront();

        Click += (s, e) => HideResults();
        _content.Click += (s, e) => HideResults();
    }

    private void BuildSidebar()
    {
        var title = new Label
        {
            Text = "Repair Track",
            Font = Ui.HeadingFont,
            ForeColor = Ui.TextPrimary,
            Dock = DockStyle.Top,
            Height = 60,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        };

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = Ui.PanelBackground };
        var ring = new Panel
        {
            Size = new Size(16, 16),
            Location = new Point(16, 18),
            BackColor = Ui.OfflineRing
        };
        var dot = new Panel
        {
            Size = new Size(10, 10),
            Location = new Point(3, 3),
            BackColor = Ui.Offline
        };
        ring.Controls.Add(dot);
        footer.Controls.Add(ring);
        footer.Controls.Add(new Label
        {
            Text = "Offline · this PC",
            Font = Ui.SmallFont,
            ForeColor = Ui.Offline,
            AutoSize = true,
            Location = new Point(38, 19)
        });

        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Ui.PanelBackground
        };
        menu.Controls.Add(MakeMenuButton("Dashboard", Ui.Home, () => new DashboardScreen()));
        menu.Controls.Add(MakeMenuButton("Customers", Ui.People, () => new CustomersScreen()));
        menu.Controls.Add(MakeMenuButton("Job Orders", Ui.Document, () => new JobOrdersScreen()));

        _sidebar.Controls.Add(menu);
        _sidebar.Controls.Add(footer);
        _sidebar.Controls.Add(title);
    }

    private Button MakeMenuButton(string text, string icon, Func<UserControl> createScreen)
    {
        var button = new Button
        {
            Text = "  " + text,
            Width = 190,
            Height = 42,
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat,
            Font = Ui.BodyFont,
            ForeColor = Ui.TextDefault,
            Padding = new Padding(8, 0, 0, 0),
            Margin = new Padding(5, 4, 5, 0),
            Image = Ui.IconBitmap(icon, Ui.IconDefault, 16),
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (s, e) =>
        {
            HideResults();
            ShowScreen(createScreen());
            SetActive(button);
        };
        _menuButtons.Add(button);
        return button;
    }

    private void SetActive(Button active)
    {
        foreach (var b in _menuButtons)
        {
            bool isActive = b == active;
            b.BackColor = isActive ? Ui.PrimaryLight : Ui.PanelBackground;
            b.ForeColor = isActive ? Ui.PrimaryDark : Ui.TextDefault;
        }
    }

    public void ShowScreen(UserControl screen)
    {
        foreach (Control old in _content.Controls.Cast<Control>().ToList())
            old.Dispose();
        screen.Dock = DockStyle.Fill;
        _content.Controls.Add(screen);
        HideResults();
    }

    private void BuildTopBar()
    {
        var inner = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(8, 6, 8, 6) };
        var icon = Ui.IconLabel(Ui.Search, 12, Ui.IconDefault);
        icon.Dock = DockStyle.Left;
        icon.AutoSize = false;
        icon.Width = 22;
        icon.TextAlign = ContentAlignment.MiddleCenter;
        _search.GotFocus += (s, e) => _searchWrap.BackColor = Ui.FocusRing;
        _search.LostFocus += (s, e) => _searchWrap.BackColor = Ui.Border;
        _search.TextChanged += (s, e) =>
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        };
        _search.KeyDown += OnSearchKey;
        inner.Controls.Add(_search);
        inner.Controls.Add(icon);
        _searchWrap.Controls.Add(inner);
        _topBar.Controls.Add(_searchWrap);

        _searchTimer.Tick += (s, e) =>
        {
            _searchTimer.Stop();
            RunSearch();
        };
    }

    private void BuildSearchResults()
    {
        _results.ForeColor = Ui.TextDefault;
        _results.BackColor = Ui.PanelBackground;
        _results.Click += (s, e) => OpenSelectedResult();
        _results.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenSelectedResult();
                e.Handled = true;
            }
        };
        _resultsPanel.Controls.Add(_results);
        _resultsPanel.Controls.Add(_resultsBottomLine);
        _resultsPanel.Paint += (s, e) =>
        {
            using var pen = new Pen(Ui.Border);
            e.Graphics.DrawRectangle(pen, 0, 0, _resultsPanel.Width - 1, _resultsPanel.Height - 1);
        };
    }

    private void RunSearch()
    {
        string text = _search.Text.Trim();
        if (text.Length == 0)
        {
            HideResults();
            return;
        }

        var hits = _searchService.Search(text);
        _results.Items.Clear();
        foreach (var hit in hits)
            _results.Items.Add(hit);

        if (_results.Items.Count == 0)
            _results.Items.Add("No matching customers or devices.");

        _resultsPanel.Location = new Point(_sidebar.Width + _searchWrap.Left, _topBar.Height);
        _resultsPanel.Visible = true;
        _resultsPanel.BringToFront();
    }

    private void OnSearchKey(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            HideResults();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Down && _resultsPanel.Visible && _results.Items.Count > 0)
        {
            _results.Focus();
            _results.SelectedIndex = 0;
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Enter)
        {
            OpenSelectedResult();
            e.Handled = true;
        }
    }

    private void OpenSelectedResult()
    {
        if (_results.SelectedItem is not SearchHit hit)
        {
            if (_results.Items.Count == 1 && _results.Items[0] is SearchHit only)
                hit = only;
            else
                return;
        }

        if (hit.Kind == "Customer")
            ShowScreen(new CustomerDetailScreen(hit.Id));
        else if (hit.Kind == "Device")
            ShowScreen(new DeviceDetailScreen(hit.Id));
        HideResults();
    }

    private void HideResults()
    {
        _resultsPanel.Visible = false;
    }
}
