namespace KoiShipping.API.Models.StaffModel
{
    public class ResponseStaffModel
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; }
        public bool DeleteStatus { get; set; }
    }
}
