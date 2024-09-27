using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public string StartLocation { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public string? TransportMethod { get; set; }

    public DateTime? DepartureDate { get; set; }

    public DateTime? ArrivalDate { get; set; }

    public string Status { get; set; } = null!;

    public decimal? TotalWeight { get; set; }

    public int? TotalKoiFish { get; set; }

    public bool DeleteStatus { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderStaff> OrderStaffs { get; set; } = new List<OrderStaff>();

}
