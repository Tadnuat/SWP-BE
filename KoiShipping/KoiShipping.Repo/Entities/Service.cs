using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class Service
{
    public int ServiceId { get; set; }

    public string TransportMethod { get; set; } = null!;

    public string WeightRange { get; set; } = null!;

    public decimal FastDelivery { get; set; }

    public decimal EconomyDelivery { get; set; }

    public decimal ExpressDelivery { get; set; }

    public bool DeleteStatus { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
