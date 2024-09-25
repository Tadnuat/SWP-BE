﻿using System.ComponentModel.DataAnnotations;

namespace KoiShipping.API.Models.StaffModel
{
    public class RequestCreateStaffModel
    {

        public int StaffId { get; set; } // Include StaffId for easier updates

        public string StaffName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Role { get; set; }

        public string? Phone { get; set; }
        public string Status { get; set; } = null!;
    }
}
