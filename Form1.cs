using RepairTrack.Data;
using RepairTrack.Data.Entities;
using RepairTrack.Data.Repositories;


namespace RepairTrack;

public partial class Form1 : Form
{
    private readonly TextBox _log = new()
    {
        Multiline = true,
        ReadOnly = true,
        Dock = DockStyle.Fill,
        ScrollBars = ScrollBars.Vertical,
        Font = new Font("Consolas", 10)
    };

    public Form1()
    {
        InitializeComponent();
        Text = "Repair Track - Progress Test";
        Width = 700;
        Height = 450;
        Controls.Add(_log);
        RunChecks();
    }

    private void RunChecks()
    {
        _log.AppendText("Step 0: window works\r\n");
        var c = new Customer { FullName = "Test Person" };
        _log.AppendText("Step 1: made customer " + c.FullName + "\r\n");
        using var db = new RepairTrackContext();
        _log.AppendText("Step 2: customers in database = " + db.Customers.Count() + "\r\n");
        var customerRepo = new CustomerRepository();
        foreach (var cu in customerRepo.GetAll())
            _log.AppendText($"4a: {cu.FullName} - {cu.Devices.Count} device(s)\r\n");
        _log.AppendText("4a: search 'juan' found " + customerRepo.Search("juan").Count + "\r\n");
        var deviceRepo = new DeviceRepository();
        foreach (var d in deviceRepo.SearchBySerial("SN-"))
            _log.AppendText($"4b: {d.SerialNumber} belongs to {d.Customer.FullName}\r\n");
        var jobRepo = new JobOrderRepository();
        foreach (var j in jobRepo.GetAll())
            _log.AppendText($"4c: job {j.JobId} - {j.Status} - {j.Device.Customer.FullName}\r\n");
        _log.AppendText("4c: jobs in 'Received' = " + jobRepo.CountByStatus("Received") + "\r\n");
        var paymentRepo = new PaymentRepository();
        _log.AppendText("4d: total paid for job 1 = " + paymentRepo.GetTotalPaid(1).ToString("N2") + "\r\n");
    }
}