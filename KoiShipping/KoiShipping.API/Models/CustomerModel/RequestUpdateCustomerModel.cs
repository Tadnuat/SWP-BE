namespace KoiShipping.API.Models.CustomerModel
{
    public class RequestUpdateCustomerModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }
        public bool DeleteStatus { get; set; }
    }
}
