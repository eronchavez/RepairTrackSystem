using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class JobOrdersScreen : UserControl
{
    private readonly JobOrderService _jobs = new();
    private readonly DataGridView _grid = Ui.Grid();
    private readonly ComboBox _statusFilter = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Width = 180,
        Location = new Point(70, 6)
    };

    public JobOrdersScreen()
    {
        BuildUi();
        LoadRows();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var newJob = Ui.PrimaryButton(" New Job Order", 170, Ui.Add);
        newJob.Click += (s, e) => Ui.Open(this, new JobOrderNewScreen());

        var header = Ui.Header(
            "Job Orders",
            "All repair jobs in the shop.",
            newJob);

        var filterBar = new Panel { Dock = DockStyle.Top, Height = 44 };
        filterBar.Controls.Add(new Label
        {
            Text = "Status:",
            Font = Ui.BodyFont,
            ForeColor = Ui.TextDefault,
            AutoSize = true,
            Location = new Point(0, 10)
        });
        _statusFilter.Font = Ui.BodyFont;
        _statusFilter.BackColor = Ui.PanelBackground;
        _statusFilter.ForeColor = Ui.TextDefault;
        _statusFilter.Items.AddRange(new object[] { "All", "Received", "In Repair", "Ready for Pickup", "Completed" });
        _statusFilter.SelectedIndex = 0;
        _statusFilter.SelectedIndexChanged += (s, e) => LoadRows();
        filterBar.Controls.Add(_statusFilter);

        _grid.Columns.Add("job", "JOB ID");
        _grid.Columns.Add("customer", "CUSTOMER");
        _grid.Columns.Add("device", "DEVICE");
        _grid.Columns.Add("status", "STATUS");
        _grid.Columns.Add("received", "DATE RECEIVED");
        _grid.Columns.Add("cost", "TOTAL COST");
        _grid.Columns.Add("balance", "BALANCE DUE");
        _grid.CellClick += (s, e) => OpenJob(e.RowIndex);

        var tablePanel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(8) };
        tablePanel.Controls.Add(_grid);

        Controls.Add(tablePanel);
        Controls.Add(filterBar);
        Controls.Add(header);
    }

    private void LoadRows()
    {
        _grid.Rows.Clear();
        string selected = _statusFilter.SelectedItem?.ToString() ?? "All";
        foreach (var job in _jobs.GetAll(selected))
        {
            int row = _grid.Rows.Add(
                IdFormatter.JobId(job.JobId, job.DateReceived),
                job.Device.Customer.FullName,
                Ui.DeviceTitle(job.Device.DeviceType, job.Device.Brand),
                job.Status,
                job.DateReceived.ToString("MMM d, yyyy"),
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
