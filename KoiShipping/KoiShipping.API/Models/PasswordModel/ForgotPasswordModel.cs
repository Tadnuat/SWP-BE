namespace KoiShipping.API.Models.PasswordModel
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }
}
