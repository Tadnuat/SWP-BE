using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class AserviceOrderD
{
    public int AserviceOrderId { get; set; } // Tự động tăng

    public int OrderDetailId { get; set; }

    public int AdvancedServiceId { get; set; }

    public virtual AdvancedService AdvancedService { get; set; } = null!;

    public virtual OrderDetail OrderDetail { get; set; } = null!;
}
