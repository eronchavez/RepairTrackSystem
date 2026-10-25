using System.Configuration;
using Microsoft.EntityFrameworkCore;
using RepairTrack.Data.Entities;

namespace RepairTrack.Data;

public class RepairTrackContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<JobOrder> JobOrders => Set<JobOrder>();
    public DbSet<JobStatusHistory> JobStatusHistories => Set<JobStatusHistory>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string? cs = ConfigurationManager.ConnectionStrings["RepairTrackDb"]?.ConnectionString;
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("MISSING_CONNECTION_STRING");
        options.UseNpgsql(cs);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("customers");
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.CustomerId).HasColumnName("customer_id");
            e.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(45);
            e.Property(x => x.Address).HasColumnName("address");
        });

        modelBuilder.Entity<Device>(e =>
        {
            e.ToTable("devices");
            e.HasKey(x => x.DeviceId);
            e.Property(x => x.DeviceId).HasColumnName("device_id");
            e.Property(x => x.CustomerId).HasColumnName("customer_id");
            e.Property(x => x.DeviceType).HasColumnName("device_type").HasMaxLength(45).IsRequired();
            e.Property(x => x.Brand).HasColumnName("brand").HasMaxLength(45);
            e.Property(x => x.SerialNumber).HasColumnName("serial_number").HasMaxLength(45).IsRequired();
            e.Property(x => x.Password).HasColumnName("password").HasMaxLength(45);
            e.HasOne(x => x.Customer).WithMany(c => c.Devices)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JobOrder>(e =>
        {
            e.ToTable("job_orders");
            e.HasKey(x => x.JobId);
            e.Property(x => x.JobId).HasColumnName("job_id");
            e.Property(x => x.DeviceId).HasColumnName("device_id");
            e.Property(x => x.IssueDescription).HasColumnName("issue_description").IsRequired();
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(45).IsRequired();
            e.Property(x => x.DateReceived).HasColumnName("date_received").HasColumnType("timestamp");
            e.Property(x => x.DateReleased).HasColumnName("date_released").HasColumnType("timestamp");
            e.Property(x => x.TotalCost).HasColumnName("total_cost").HasColumnType("numeric(10,2)");
            e.HasOne(x => x.Device).WithMany(d => d.JobOrders)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JobStatusHistory>(e =>
        {
            e.ToTable("job_status_history");
            e.HasKey(x => x.HistoryId);
            e.Property(x => x.HistoryId).HasColumnName("history_id");
            e.Property(x => x.JobId).HasColumnName("job_id");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(45).IsRequired();
            e.Property(x => x.ChangedAt).HasColumnName("changed_at").HasColumnType("timestamp");
            e.HasOne(x => x.JobOrder).WithMany(j => j.StatusHistory)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("payments");
            e.HasKey(x => x.PaymentId);
            e.Property(x => x.PaymentId).HasColumnName("payment_id");
            e.Property(x => x.JobId).HasColumnName("job_id");
            e.Property(x => x.AmountPaid).HasColumnName("amount_paid").HasColumnType("numeric(10,2)");
            e.Property(x => x.PaymentDate).HasColumnName("payment_date").HasColumnType("date");
            e.HasOne(x => x.JobOrder).WithMany(j => j.Payments)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}