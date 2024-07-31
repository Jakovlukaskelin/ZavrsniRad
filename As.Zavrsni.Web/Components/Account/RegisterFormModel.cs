using System.ComponentModel.DataAnnotations;

namespace As.Zavrsni.Web.Components.Account
{
    public class RegisterFormModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password  is required")]
        public string Password { get; set; }
    }
}
