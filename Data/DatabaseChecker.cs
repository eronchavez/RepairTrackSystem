using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net.Sockets;

namespace RepairTrack.Data;

public static class DatabaseChecker
{
    public static bool TryConnect(out string errorMessage)
    {
        try
        {
            using var db = new RepairTrackContext();
            db.Database.OpenConnection();
            _ = db.Customers.FirstOrDefault();
            _ = db.Devices.FirstOrDefault();
            _ = db.JobOrders.FirstOrDefault();
            _ = db.JobStatusHistories.FirstOrDefault();
            _ = db.Payments.FirstOrDefault();
            errorMessage = "";
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = BuildFriendlyMessage(ex);
            return false;
        }
    }

    private static string BuildFriendlyMessage(Exception ex)
    {
        for (Exception? e = ex; e != null; e = e.InnerException)
        {
            if (e.Message == "MISSING_CONNECTION_STRING")
                return "App.config is missing or has no 'RepairTrackDb' connection string.\n\nCopy App.config.example to App.config and put your password in it.";
            if (e is PostgresException pg)
            {
                switch (pg.SqlState)
                {
                    case "28P01": return "PostgreSQL rejected the password.\n\nCheck the password in App.config.";
                    case "3D000": return "The database 'repairtrack' was not found.\n\nRun the Stage 1 script in pgAdmin first.";
                    case "42P01": return "A table is missing in the database.\n\nRun the Stage 1 table script in pgAdmin.";
                    case "42703": return "A column name in the code does not match the database.\n\nDetails: " + pg.MessageText;
                }
            }
            if (e is SocketException)
                return "Cannot reach PostgreSQL.\n\nMake sure the PostgreSQL service is running, then start the app again.";
        }
        return "Could not connect to the database.\n\nDetails: " + ex.Message;
    }
}