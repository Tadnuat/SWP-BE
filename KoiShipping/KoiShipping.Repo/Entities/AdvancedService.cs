using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class AdvancedService
{
    public int AdvancedServiceId { get; set; }

    public string AServiceName { get; set; } = null!;
    public string Description { get; set; }

    public decimal Price { get; set; }

    public bool DeleteStatus { get; set; }

    public virtual ICollection<AserviceOrderD> AserviceOrderDs { get; set; } = new List<AserviceOrderD>();
}
