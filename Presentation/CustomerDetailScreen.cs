using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class CustomerDetailScreen : UserControl
{
    private readonly int _customerId;
    private readonly CustomerService _customers = new();
    private readonly DeviceService _devices = new();
    private readonly JobOrderService _jobs = new();

    public CustomerDetailScreen(int customerId)
    {
        _customerId = customerId;
        BuildUi();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var customer = _customers.GetById(_customerId);
        if (customer == null)
        {
            Controls.Add(new Label
            {
                Text = "Customer not found.",
                Font = Ui.TitleFont,
                ForeColor = Ui.TextPrimary,
                AutoSize = true,
                Location = new Point(0, 0)
            });
            return;
        }

        var back = Ui.SecondaryButton("Back", 90, Ui.Back);
        back.Click += (s, e) => Ui.Open(this, new CustomersScreen());
        var edit = Ui.PrimaryButton("Edit", 100, Ui.Edit);
        edit.Click += (s, e) => Ui.Open(this, new CustomerEditScreen(_customerId));

        var header = Ui.Header(customer.FullName, IdFormatter.CustomerId(customer.CustomerId), back, edit);

        var info = new Panel { Dock = DockStyle.Top, Height = 88, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        info.Controls.Add(InfoLine("Phone", string.IsNullOrWhiteSpace(customer.Phone) ? "—" : customer.Phone, 0));
        info.Controls.Add(InfoLine("Address", string.IsNullOrWhiteSpace(customer.Address) ? "—" : customer.Address, 32));

        var addDevice = Ui.PrimaryButton("Add Device", 130, Ui.Add);
        addDevice.Click += (s, e) => Ui.Open(this, new DeviceEditScreen(_customerId, null, AfterDeviceSave.CustomerDetail));
        var deviceHeader = Ui.Header("Devices", "Devices registered to this customer.", addDevice);
        deviceHeader.Dock = DockStyle.Top;

        var deviceGrid = Ui.Grid();
        deviceGrid.Columns.Add("type", "DEVICE TYPE");
        deviceGrid.Columns.Add("brand", "BRAND");
        deviceGrid.Columns.Add("serial", "SERIAL NUMBER");
        foreach (var device in _devices.GetByCustomer(_customerId))
        {
            int row = deviceGrid.Rows.Add(device.DeviceType, device.Brand ?? "", device.SerialNumber);
            deviceGrid.Rows[row].Tag = device.DeviceId;
        }
        deviceGrid.CellClick += (s, e) =>
        {
            if (e.RowIndex < 0)
                return;
            if (deviceGrid.Rows[e.RowIndex].Tag is int deviceId)
                Ui.Open(this, new DeviceDetailScreen(deviceId));
        };
        var devicePanel = new Panel { Dock = DockStyle.Top, Height = 200, BackColor = Ui.PanelBackground, Padding = new Padding(8) };
        devicePanel.Controls.Add(deviceGrid);

        var jobHeader = Ui.Header("Job history", "Repair jobs for this customer's devices.");
        jobHeader.Dock = DockStyle.Top;

        var jobGrid = Ui.Grid();
        jobGrid.Columns.Add("job", "JOB ID");
        jobGrid.Columns.Add("device", "DEVICE");
        jobGrid.Columns.Add("status", "STATUS");
        jobGrid.Columns.Add("received", "DATE RECEIVED");
        jobGrid.Columns.Add("cost", "TOTAL COST");
        jobGrid.Columns.Add("balance", "BALANCE DUE");
        foreach (var job in _jobs.GetByCustomer(_customerId))
        {
            int row = jobGrid.Rows.Add(
                IdFormatter.JobId(job.JobId, job.DateReceived),
                Ui.DeviceTitle(job.Device.DeviceType, job.Device.Brand),
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
        Controls.Add(devicePanel);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        Controls.Add(deviceHeader);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        Controls.Add(info);
        Controls.Add(header);
    }

    private static Label InfoLine(string caption, string value, int top)
    {
        return new Label
        {
            Text = caption + ":  " + value,
            Font = Ui.BodyFont,
            ForeColor = Ui.TextDefault,
            AutoSize = true,
            Location = new Point(0, top)
        };
    }
}
