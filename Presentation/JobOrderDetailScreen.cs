using RepairTrack.Data.Entities;
using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class JobOrderDetailScreen : UserControl
{
    private readonly int _jobId;
    private readonly JobOrderService _jobs = new();
    private readonly PaymentService _payments = new();
    private readonly TextBox _cost = Ui.Input(140);
    private readonly TextBox _issue = Ui.Input();
    private readonly TextBox _payAmount = Ui.Input(140);
    private readonly DateTimePicker _payDate = new()
    {
        Format = DateTimePickerFormat.Short,
        Width = 140,
        Font = Ui.BodyFont
    };
    private readonly DataGridView _historyGrid = Ui.Grid();
    private readonly DataGridView _paymentGrid = Ui.Grid();
    private readonly Label _totalPaid = new();
    private readonly Label _balance = new();
    private readonly Button _backStatus = Ui.SecondaryButton("Step back", 150);
    private readonly Button _forwardStatus = Ui.PrimaryButton("Step forward", 180);
    private readonly Panel _tracker = new() { Height = 72, Dock = DockStyle.Top, BackColor = Ui.PanelBackground };
    private JobOrder? _job;

    public JobOrderDetailScreen(int jobId)
    {
        _jobId = jobId;
        _issue.Multiline = true;
        _issue.Height = 90;
        _issue.ScrollBars = ScrollBars.Vertical;
        _issue.Width = 420;
        BuildUi();
        LoadJob();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var headerHost = new Panel { Dock = DockStyle.Top, Height = 64, Name = "headerHost" };

        var trackerWrap = new Panel { Dock = DockStyle.Top, Height = 88, BackColor = Ui.PanelBackground, Padding = new Padding(16, 8, 16, 8) };
        _tracker.Dock = DockStyle.Top;
        _tracker.Height = 48;
        _tracker.Resize += (s, e) =>
        {
            if (_job != null)
                LayoutSteps(Array.IndexOf(JobOrderService.StatusSteps, _job.Status), JobOrderService.StatusSteps.Length);
        };
        var statusBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Ui.PanelBackground };
        _backStatus.Width = 200;
        _forwardStatus.Width = 230;
        _backStatus.Location = new Point(16, 4);
        _forwardStatus.Location = new Point(224, 4);
        _backStatus.Click += (s, e) => MoveStatus(false);
        _forwardStatus.Click += (s, e) => MoveStatus(true);
        statusBar.Controls.Add(_backStatus);
        statusBar.Controls.Add(_forwardStatus);
        trackerWrap.Controls.Add(statusBar);
        trackerWrap.Controls.Add(_tracker);

        var columns = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Ui.WindowBackground
        };
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

        columns.Controls.Add(BuildLeft(), 0, 0);
        columns.Controls.Add(BuildRight(), 1, 0);

        Controls.Add(columns);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        Controls.Add(trackerWrap);
        Controls.Add(headerHost);
    }

    private Panel BuildLeft()
    {
        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };

        var issuePanel = new Panel { Dock = DockStyle.Top, Height = 170, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        issuePanel.Controls.Add(new Label
        {
            Text = "Issue description",
            Font = Ui.SectionFont,
            ForeColor = Ui.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        _issue.Location = new Point(0, 28);
        var saveIssue = Ui.SecondaryButton("Save issue", 120);
        saveIssue.Location = new Point(0, 124);
        saveIssue.Click += (s, e) => SaveIssue();
        issuePanel.Controls.Add(_issue);
        issuePanel.Controls.Add(saveIssue);

        var historyPanel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        historyPanel.Controls.Add(_historyGrid);
        historyPanel.Controls.Add(new Label
        {
            Text = "Status history",
            Font = Ui.SectionFont,
            ForeColor = Ui.TextPrimary,
            Dock = DockStyle.Top,
            Height = 28
        });
        _historyGrid.Columns.Add("status", "STATUS");
        _historyGrid.Columns.Add("when", "DATE AND TIME");

        left.Controls.Add(historyPanel);
        left.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        left.Controls.Add(issuePanel);
        return left;
    }

    private Panel BuildRight()
    {
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8, 0, 0, 0) };

        var money = new Panel { Dock = DockStyle.Top, Height = 200, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        money.Controls.Add(new Label
        {
            Text = "Financial summary",
            Font = Ui.SectionFont,
            ForeColor = Ui.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        money.Controls.Add(new Label
        {
            Text = "Total cost",
            Font = Ui.SmallFont,
            ForeColor = Ui.TextSecondary,
            AutoSize = true,
            Location = new Point(0, 36)
        });
        _cost.Location = new Point(0, 54);
        var saveCost = Ui.SecondaryButton("Save cost", 110);
        saveCost.Location = new Point(150, 52);
        saveCost.Click += (s, e) => SaveCost();
        money.Controls.Add(_cost);
        money.Controls.Add(saveCost);

        _totalPaid.AutoSize = true;
        _totalPaid.Font = Ui.BodyFont;
        _totalPaid.ForeColor = Ui.TextDefault;
        _totalPaid.Location = new Point(0, 100);
        _balance.AutoSize = true;
        _balance.Font = Ui.HeadingFont;
        _balance.Location = new Point(0, 128);
        money.Controls.Add(_totalPaid);
        money.Controls.Add(_balance);

        var payPanel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        _paymentGrid.Columns.Add("when", "DATE");
        _paymentGrid.Columns.Add("amount", "AMOUNT");
        payPanel.Controls.Add(_paymentGrid);

        var record = new Panel { Dock = DockStyle.Bottom, Height = 96, BackColor = Ui.FooterBackground };
        record.Controls.Add(new Label
        {
            Text = "Record payment",
            Font = Ui.SectionFont,
            ForeColor = Ui.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        });
        _payAmount.PlaceholderText = "Amount";
        _payAmount.Location = new Point(0, 32);
        _payDate.Location = new Point(150, 34);
        var recordBtn = Ui.PrimaryButton("Record", 110);
        recordBtn.Location = new Point(300, 28);
        recordBtn.Click += (s, e) => RecordPayment();
        record.Controls.Add(_payAmount);
        record.Controls.Add(_payDate);
        record.Controls.Add(recordBtn);

        payPanel.Controls.Add(record);
        payPanel.Controls.Add(new Label
        {
            Text = "Payments",
            Font = Ui.SectionFont,
            ForeColor = Ui.TextPrimary,
            Dock = DockStyle.Top,
            Height = 28
        });

        right.Controls.Add(payPanel);
        right.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        right.Controls.Add(money);
        return right;
    }

    private void LoadJob()
    {
        _job = _jobs.GetById(_jobId);
        if (_job == null)
        {
            MessageBox.Show("Job order not found.", "Repair Track", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Ui.Open(this, new JobOrdersScreen());
            return;
        }

        BuildHeader(_job);
        LayoutSteps(Array.IndexOf(JobOrderService.StatusSteps, _job.Status), JobOrderService.StatusSteps.Length);
        FillStatusButtons(_job.Status);
        _issue.Text = _job.IssueDescription;
        _cost.Text = _job.TotalCost.ToString("0.00");

        decimal paid = _jobs.GetTotalPaid(_job);
        decimal balance = _jobs.GetBalance(_job);
        _totalPaid.Text = "Total paid  " + Ui.Money(paid);
        _balance.Text = "Balance  " + Ui.Money(balance);
        _balance.ForeColor = Ui.BalanceColor(balance);

        _historyGrid.Rows.Clear();
        foreach (var row in _job.StatusHistory.OrderBy(h => h.ChangedAt))
            _historyGrid.Rows.Add(row.Status, row.ChangedAt.ToString("MMM d, yyyy  h:mm tt"));

        _paymentGrid.Rows.Clear();
        foreach (var pay in _job.Payments.OrderBy(p => p.PaymentDate).ThenBy(p => p.PaymentId))
            _paymentGrid.Rows.Add(pay.PaymentDate.ToString("MMM d, yyyy"), Ui.Money(pay.AmountPaid));
    }

    private void BuildHeader(JobOrder job)
    {
        Panel? host = null;
        foreach (Control child in Controls)
        {
            if (child.Name == "headerHost")
                host = (Panel)child;
        }
        if (host == null)
            return;
        host.Controls.Clear();

        var back = Ui.SecondaryButton("Back", 90, Ui.Back);
        back.Click += (s, e) => Ui.Open(this, new JobOrdersScreen());
        var password = Ui.SecondaryButton("Password", 120, Ui.Lock);
        password.Click += (s, e) => Ui.ShowPassword(job.Device.Password);

        string subtitle = job.Device.Customer.FullName + "  ·  " +
            Ui.DeviceTitle(job.Device.DeviceType, job.Device.Brand) + "  ·  " +
            job.Device.SerialNumber + "  ·  received " + job.DateReceived.ToString("MMM d, yyyy");

        var header = Ui.Header(IdFormatter.JobId(job.JobId, job.DateReceived), subtitle, back, password);
        header.Dock = DockStyle.Fill;
        host.Controls.Add(header);
    }

    private void LayoutSteps(int currentIndex, int count)
    {
        _tracker.Controls.Clear();
        if (_tracker.Width < 20)
            return;
        int stepWidth = Math.Max(80, (_tracker.Width - 32) / count);
        for (int i = 0; i < count; i++)
        {
            string name = JobOrderService.StatusSteps[i];
            var step = new Panel
            {
                Width = stepWidth - 8,
                Height = 40,
                Location = new Point(8 + i * stepWidth, 4),
                BackColor = i <= currentIndex ? Ui.StatusBack(name) : Ui.DisabledBackground
            };
            step.Controls.Add(new Label
            {
                Text = name,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = Ui.SmallFont,
                ForeColor = i <= currentIndex ? Ui.StatusFore(name) : Ui.TextMuted
            });
            _tracker.Controls.Add(step);
        }
    }

    private void FillStatusButtons(string current)
    {
        string? prev = _jobs.GetPreviousStatus(current);
        string? next = _jobs.GetNextStatus(current);
        _backStatus.Enabled = prev != null;
        _forwardStatus.Enabled = next != null;
        _backStatus.Text = prev == null ? "Step back" : "Back to " + prev;
        _forwardStatus.Text = next == null ? "Completed" : "Move to " + next;
        _backStatus.BackColor = _backStatus.Enabled ? Ui.PanelBackground : Ui.DisabledBackground;
        _forwardStatus.BackColor = _forwardStatus.Enabled ? Ui.Primary : Ui.DisabledBackground;
        _forwardStatus.ForeColor = _forwardStatus.Enabled ? Ui.TextOnPrimary : Ui.TextMuted;
    }

    private void MoveStatus(bool forward)
    {
        if (_job == null)
            return;
        string? target = forward ? _jobs.GetNextStatus(_job.Status) : _jobs.GetPreviousStatus(_job.Status);
        if (target == null)
            return;

        var result = _jobs.ChangeStatus(_jobId, target);
        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Repair Track", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        LoadJob();
    }

    private void SaveIssue()
    {
        var result = _jobs.UpdateIssue(_jobId, _issue.Text);
        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        LoadJob();
    }

    private void SaveCost()
    {
        if (!Ui.TryParseMoney(_cost.Text, out decimal amount) || amount < 0)
        {
            MessageBox.Show("Enter a valid total cost.", "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var result = _jobs.UpdateCost(_jobId, amount);
        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        LoadJob();
    }

    private void RecordPayment()
    {
        if (!Ui.TryParseMoney(_payAmount.Text, out decimal amount))
        {
            MessageBox.Show("Enter a valid payment amount.", "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var result = _payments.Record(_jobId, amount, DateOnly.FromDateTime(_payDate.Value));
        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        _payAmount.Text = "";
        LoadJob();
    }
}
