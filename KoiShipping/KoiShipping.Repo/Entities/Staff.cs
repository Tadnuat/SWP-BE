using System;
using System.Collections.Generic;

namespace KoiShipping.Repo.Entities;

public partial class Staff
{
    public int StaffId { get; set; }

    public string StaffName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public string? Phone { get; set; }

    public string Status { get; set; } = null!;

    public bool DeleteStatus { get; set; }

    public virtual ICollection<OrderStaff> OrderStaffs { get; set; } = new List<OrderStaff>();

}
