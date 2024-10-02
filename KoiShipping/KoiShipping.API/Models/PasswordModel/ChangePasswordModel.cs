namespace KoiShipping.API.Models.PasswordModel

    {
        public class ChangePasswordModel
        {
            public string OldPassword { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        }
    }

