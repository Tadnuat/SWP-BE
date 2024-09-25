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
        modelBuilder.Entity<AdvancedService>(entity =>
        {
            entity.HasKey(e => e.AdvancedServiceId).HasName("PK__Advanced__149CDE6BC56632AA");

            entity.ToTable("Advanced_Service");

            entity.Property(e => e.AdvancedServiceId)
                .ValueGeneratedNever()
                .HasColumnName("AdvancedServiceID");
            entity.Property(e => e.DeleteStatus).HasColumnType("bit"); // Chỉnh sửa kiểu dữ liệu
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });

        modelBuilder.Entity<AserviceOrderD>(entity =>
        {
            entity.HasKey(e => e.AserviceOrderId).HasName("PK__AService__F07E586474FF7EFF");

            entity.ToTable("AService_OrderD");

            entity.Property(e => e.AserviceOrderId)
                .ValueGeneratedNever()
                .HasColumnName("AServiceOrderID");
            entity.Property(e => e.AdvancedServiceId).HasColumnName("AdvancedServiceID");
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

            entity.HasOne(d => d.AdvancedService).WithMany(p => p.AserviceOrderDs)
                .HasForeignKey(d => d.AdvancedServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AServiceOrderD_AdvancedService");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.AserviceOrderDs)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AServiceOrderD_OrderDetail");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8E8F6C811");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.DeleteStatus).HasColumnType("bit"); // Chỉnh sửa kiểu dữ liệu
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAFBDDC96B4");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.ArrivalDate).HasColumnType("datetime");
            entity.Property(e => e.DeleteStatus).HasColumnType("bit"); // Chỉnh sửa kiểu dữ liệu
            entity.Property(e => e.DepartureDate).HasColumnType("datetime");
            entity.Property(e => e.Destination).HasMaxLength(255);
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.StartLocation).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalWeight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TransportMethod).HasMaxLength(50);

            entity.HasOne(d => d.Staff).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Staffs");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__Order_De__D3B9D30C681257B0");

            entity.ToTable("Order_Detail");

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedNever()
                .HasColumnName("OrderDetailID");

            entity.Property(e => e.AttachedItem)
                .HasMaxLength(255);

            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");

            entity.Property(e => e.DeleteStatus)
                .HasColumnType("bit"); // Chỉnh sửa kiểu dữ liệu

            entity.Property(e => e.KoiStatus)
                .HasMaxLength(50);

            entity.Property(e => e.OrderId)
                .HasColumnName("OrderID");

            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)");

            entity.Property(e => e.ServiceId)
                .HasColumnName("ServiceID");

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.Weight)
                .HasColumnType("decimal(10, 2)");

            entity.Property(e => e.ReceiverName)
                .HasMaxLength(255)
                .IsRequired(); // Bắt buộc

            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(20)
                .IsRequired(); // Bắt buộc

            entity.Property(e => e.Rating)
                .HasColumnType("int"); // Có thể null, đánh giá từ 1 đến 5 sao

            entity.Property(e => e.Feedback)
                .HasMaxLength(500); // Có thể null, phản hồi từ khách hàng

            // Thiết lập quan hệ giữa OrderDetail và Customer
            entity.HasOne(d => d.Customer).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Customer");

            // Thiết lập quan hệ giữa OrderDetail và Order
            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");

            // Thiết lập quan hệ giữa OrderDetail và Service
            entity.HasOne(d => d.Service).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Service");
        });

        modelBuilder.Entity<OrderStaff>(entity =>
        {
            entity.HasKey(e => e.OrderStaffsId).HasName("PK__Order_St__BD854B319A2612A9");

            entity.ToTable("Order_Staffs");

            entity.Property(e => e.OrderStaffsId)
                .ValueGeneratedNever()
                .HasColumnName("OrderStaffsID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.StaffId).HasColumnName("StaffID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStaffs_Order");

            entity.HasOne(d => d.Staff).WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStaffs_Staff");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB0EA2310F3E9");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("ServiceID");
            entity.Property(e => e.DeleteStatus).HasColumnType("bit"); // Chỉnh sửa kiểu dữ liệu
            entity.Property(e => e.EconomyDelivery).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ExpressDelivery).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FastDelivery).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TransportMethod).HasMaxLength(50);
            entity.Property(e => e.WeightRange).HasMaxLength(50);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staffs__96D4AAF72282A844");

            entity.Property(e => e.StaffId)
                .ValueGeneratedNever()
                .HasColumnName("StaffID");
            entity.Property(e => e.DeleteStatus).HasColumnType("bit"); // Chỉnh sửa kiểu dữ liệu
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.StaffName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
        });
    OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
