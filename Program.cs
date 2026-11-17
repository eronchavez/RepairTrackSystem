using RepairTrack.Data;
using RepairTrack.Presentation;

namespace RepairTrack;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!DatabaseChecker.TryConnect(out string error))
        {
            MessageBox.Show(error, "Repair Track - Cannot start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new MainForm());
    }
}