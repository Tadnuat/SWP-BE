using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = null!;

    public bool DeleteStatus { get; set; } = false;
    public string? Otp { get; set; } 
    public DateTime? OtpExpiration { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<Notification> Notifications { get; set; }
}
