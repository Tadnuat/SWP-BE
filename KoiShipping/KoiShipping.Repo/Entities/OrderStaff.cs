using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class OrderStaff
{
    public int OrderStaffsId { get; set; }

    public int OrderId { get; set; }

    public int StaffId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
