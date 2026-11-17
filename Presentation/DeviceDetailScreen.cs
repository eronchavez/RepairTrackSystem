using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class DeviceDetailScreen : UserControl
{
    private readonly int _deviceId;
    private readonly DeviceService _devices = new();
    private readonly JobOrderService _jobs = new();

    public DeviceDetailScreen(int deviceId)
    {
        _deviceId = deviceId;
        BuildUi();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var device = _devices.GetById(_deviceId);
        if (device == null)
        {
            Controls.Add(new Label
            {
                Text = "Device not found.",
                Font = Ui.TitleFont,
                ForeColor = Ui.TextPrimary,
                AutoSize = true
            });
            return;
        }

        var back = Ui.SecondaryButton("Back", 90, Ui.Back);
        back.Click += (s, e) => Ui.Open(this, new CustomerDetailScreen(device.CustomerId));
        var edit = Ui.SecondaryButton("Edit", 100, Ui.Edit);
        edit.Click += (s, e) => Ui.Open(this, new DeviceEditScreen(device.CustomerId, _deviceId, AfterDeviceSave.DeviceDetail));
        var password = Ui.SecondaryButton("Password", 120, Ui.Lock);
        password.Click += (s, e) => Ui.ShowPassword(device.Password);
        var newJob = Ui.PrimaryButton(" New Job Order", 170, Ui.Add);
        newJob.Click += (s, e) => Ui.Open(this, new JobOrderNewScreen(_deviceId));

        string title = Ui.DeviceTitle(device.DeviceType, device.Brand);
        var header = Ui.Header(title, device.SerialNumber, back, password, edit, newJob);

        var info = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        info.Controls.Add(InfoLine("Customer", device.Customer.FullName, 0));
        info.Controls.Add(InfoLine("Type", device.DeviceType, 28));
        info.Controls.Add(InfoLine("Brand", string.IsNullOrWhiteSpace(device.Brand) ? "—" : device.Brand, 56));
        info.Controls.Add(InfoLine("Serial number", device.SerialNumber, 84));
        var customerLink = new Label
        {
            Text = device.Customer.FullName,
            Font = Ui.BodyFont,
            ForeColor = Ui.Link,
            AutoSize = true,
            Location = new Point(90, 0),
            Cursor = Cursors.Hand
        };
        customerLink.Click += (s, e) => Ui.Open(this, new CustomerDetailScreen(device.CustomerId));
        info.Controls.Add(customerLink);

        var jobHeader = Ui.Header("Job history", "Repair jobs for this device.");
        jobHeader.Dock = DockStyle.Top;

        var jobGrid = Ui.Grid();
        jobGrid.Columns.Add("job", "JOB ID");
        jobGrid.Columns.Add("status", "STATUS");
        jobGrid.Columns.Add("received", "DATE RECEIVED");
        jobGrid.Columns.Add("cost", "TOTAL COST");
        jobGrid.Columns.Add("balance", "BALANCE DUE");
        foreach (var job in _jobs.GetByDevice(_deviceId))
        {
            int row = jobGrid.Rows.Add(
                IdFormatter.JobId(job.JobId, job.DateReceived),
                job.Status,
                job.DateReceived.ToString("MMM d, yyyy"),
                Ui.Money(job.TotalCost),
                Ui.Money(_jobs.GetBalance(job)));
            jobGrid.Rows[row].Tag = job.JobId;
        }
        jobGrid.CellClick += (s, e) =>
        {
            if (e.RowIndex < 0)
                return;
            if (jobGrid.Rows[e.RowIndex].Tag is int jobId)
                Ui.Open(this, new JobOrderDetailScreen(jobId));
        };

        var jobPanel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(8) };
        jobPanel.Controls.Add(jobGrid);

        Controls.Add(jobPanel);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        Controls.Add(jobHeader);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        Controls.Add(info);
        Controls.Add(header);
    }

    private static Label InfoLine(string caption, string value, int top)
    {
        return new Label
        {
            Text = caption + ":  " + (caption == "Customer" ? "" : value),
            Font = Ui.BodyFont,
            ForeColor = Ui.TextDefault,
            AutoSize = true,
            Location = new Point(0, top)
        };
    }
}
