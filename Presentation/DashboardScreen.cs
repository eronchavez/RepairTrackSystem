using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class DashboardScreen : UserControl
{
    private readonly DashboardService _dashboard = new();
    private readonly JobOrderService _jobs = new();
    private readonly Label _countReceived = new();
    private readonly Label _countInRepair = new();
    private readonly Label _countReady = new();
    private readonly Label _countCompleted = new();
    private readonly DataGridView _grid = Ui.Grid();

    public DashboardScreen()
    {
        BuildUi();
        LoadData();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var newJob = Ui.PrimaryButton(" New Job Order", 170, Ui.Add);
        newJob.Click += (s, e) => Ui.Open(this, new JobOrderNewScreen());

        var header = Ui.Header(
            "Dashboard",
            "Here's what's happening in your shop.",
            newJob);

        var cards = new TableLayoutPanel { Dock = DockStyle.Top, Height = 100, ColumnCount = 4, RowCount = 1 };
        for (int i = 0; i < 4; i++)
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        cards.Controls.Add(MakeCard("Received", "Awaiting assessment", _countReceived, Ui.Document), 0, 0);
        cards.Controls.Add(MakeCard("In Repair", "Currently in progress", _countInRepair, Ui.Repair), 1, 0);
        cards.Controls.Add(MakeCard("Ready for Pickup", "Notify customers", _countReady, Ui.Package), 2, 0);
        cards.Controls.Add(MakeCard("Completed", "This month", _countCompleted, Ui.Check), 3, 0);

        var spacer = new Panel { Dock = DockStyle.Top, Height = 16 };

        var tablePanel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        _grid.Columns.Add("job", "JOB ID");
        _grid.Columns.Add("customer", "CUSTOMER");
        _grid.Columns.Add("device", "DEVICE");
        _grid.Columns.Add("status", "STATUS");
        _grid.Columns.Add("cost", "TOTAL COST");
        _grid.Columns.Add("balance", "BALANCE DUE");
        _grid.CellClick += (s, e) => OpenJob(e.RowIndex);
        tablePanel.Controls.Add(_grid);
        tablePanel.Controls.Add(new Label
        {
            Text = "Recent active job orders",
            Font = Ui.SectionFont,
            ForeColor = Ui.TextPrimary,
            Dock = DockStyle.Top,
            Height = 36
        });

        Controls.Add(tablePanel);
        Controls.Add(spacer);
        Controls.Add(cards);
        Controls.Add(header);
    }

    private Panel MakeCard(string title, string subtitle, Label number, string icon)
    {
        number.Text = "0";
        number.Location = new Point(14, 54);
        number.AutoSize = true;
        number.Font = Ui.CardNumberFont;
        number.ForeColor = Ui.TextPrimary;

        var card = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Margin = new Padding(0, 0, 12, 0) };
        var badge = Ui.IconBadge(icon, Ui.IconPrimary, Ui.IconPrimaryBackground, 28);
        badge.Location = new Point(card.Width - 40, 10);
        card.Resize += (s, e) => badge.Location = new Point(Math.Max(8, card.Width - 40), 10);
        card.Controls.Add(new Label
        {
            Text = title,
            Location = new Point(14, 12),
            AutoSize = true,
            Font = Ui.CardTitleFont,
            ForeColor = Ui.TextPrimary
        });
        card.Controls.Add(new Label
        {
            Text = subtitle,
            Location = new Point(14, 34),
            AutoSize = true,
            ForeColor = Ui.TextMuted,
            Font = Ui.SmallFont
        });
        card.Controls.Add(number);
        card.Controls.Add(badge);
        return card;
    }

    private void LoadData()
    {
        _countReceived.Text = _dashboard.CountReceived().ToString();
        _countInRepair.Text = _dashboard.CountInRepair().ToString();
        _countReady.Text = _dashboard.CountReadyForPickup().ToString();
        _countCompleted.Text = _dashboard.CountCompletedThisMonth().ToString();

        _grid.Rows.Clear();
        foreach (var job in _dashboard.GetRecentActive())
        {
            int row = _grid.Rows.Add(
                IdFormatter.JobId(job.JobId, job.DateReceived),
                job.Device.Customer.FullName,
                Ui.DeviceTitle(job.Device.DeviceType, job.Device.Brand),
                job.Status,
                Ui.Money(job.TotalCost),
                Ui.Money(_jobs.GetBalance(job)));
            _grid.Rows[row].Tag = job.JobId;
        }
    }

    private void OpenJob(int rowIndex)
    {
        if (rowIndex < 0)
            return;
        if (_grid.Rows[rowIndex].Tag is int jobId)
            Ui.Open(this, new JobOrderDetailScreen(jobId));
    }
}
