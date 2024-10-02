using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KoiShipping.Repo.Entities;

public partial class KoiShippingContext : DbContext
{
    public KoiShippingContext()
    {
    }

    public KoiShippingContext(DbContextOptions<KoiShippingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdvancedService> AdvancedServices { get; set; }

    public virtual DbSet<AserviceOrderD> AserviceOrderDs { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStaff> OrderStaffs { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=KoiShipping;User Id=sa;Password=12345;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Bảng Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_Customer");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedOnAdd()
                .HasColumnName("CustomerID");

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DeleteStatus).HasColumnType("bit").IsRequired();
        });

        // Bảng Staffs
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK_Staffs");

            entity.ToTable("Staffs");

            entity.Property(e => e.StaffId)
                .ValueGeneratedOnAdd()
                .HasColumnName("StaffID");

            entity.Property(e => e.StaffName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DeleteStatus).HasColumnType("bit").IsRequired();
        });

        // Bảng Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_Order");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderID");

            entity.Property(e => e.StartLocation).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Destination).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TransportMethod).HasMaxLength(50);
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.ArrivalDate).HasColumnType("datetime");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalWeight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalKoiFish).HasColumnType("int");
            entity.Property(e => e.DeleteStatus).HasColumnType("bit").IsRequired();
        });

        // Bảng Service
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK_Service");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ServiceID");

            entity.Property(e => e.TransportMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WeightRange).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FastDelivery).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.EconomyDelivery).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.ExpressDelivery).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.DeleteStatus).HasColumnType("bit").IsRequired();
        });

        // Bảng Advanced_Service
        modelBuilder.Entity<AdvancedService>(entity =>
        {
            entity.HasKey(e => e.AdvancedServiceId).HasName("PK_AdvancedService");

            entity.ToTable("Advanced_Service");

            entity.Property(e => e.AdvancedServiceId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AdvancedServiceID");

            entity.Property(e => e.AServiceName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.DeleteStatus).HasColumnType("bit").IsRequired();
        });

        // Bảng Order_Detail
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK_OrderDetail");

            entity.ToTable("Order_Detail");

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderDetailID");

            entity.Property(e => e.OrderId).HasColumnName("OrderID").IsRequired();
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID").IsRequired();
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID").IsRequired();
            entity.Property(e => e.ServiceName).HasMaxLength(100);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.Quantity).HasColumnType("int").IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.KoiStatus).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AttachedItem).HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DeleteStatus).HasColumnType("bit").IsRequired();
            entity.Property(e => e.ReceiverName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ReceiverPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Rating).HasColumnType("int");
            entity.Property(e => e.Feedback).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()").IsRequired();

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Customer");

            entity.HasOne(d => d.Service)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Service");
        });

        // Bảng AService_OrderD
        modelBuilder.Entity<AserviceOrderD>(entity =>
        {
            entity.HasKey(e => e.AserviceOrderId).HasName("PK_AServiceOrderD");

            entity.ToTable("AService_OrderD");

            entity.Property(e => e.AserviceOrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AServiceOrderID");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID").IsRequired();
            entity.Property(e => e.AdvancedServiceId).HasColumnName("AdvancedServiceID").IsRequired();

            entity.HasOne(d => d.OrderDetail)
                .WithMany(p => p.AserviceOrderDs)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AServiceOrderD_OrderDetail");

            entity.HasOne(d => d.AdvancedService)
                .WithMany(p => p.AserviceOrderDs)
                .HasForeignKey(d => d.AdvancedServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AServiceOrderD_AdvancedService");
        });

        // Bảng Order_Staffs
        modelBuilder.Entity<OrderStaff>(entity =>
        {
            entity.HasKey(e => e.OrderStaffsId).HasName("PK_OrderStaffs");

            entity.ToTable("Order_Staffs");

            entity.Property(e => e.OrderStaffsId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderStaffsID");

            entity.Property(e => e.OrderId).HasColumnName("OrderID").IsRequired();
            entity.Property(e => e.StaffId).HasColumnName("StaffID").IsRequired();

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStaffs_Order");

            entity.HasOne(d => d.Staff)
                .WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStaffs_Staff");
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
