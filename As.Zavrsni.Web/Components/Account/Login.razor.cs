using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace As.Zavrsni.Web.Components.Account
{
    public partial class Login : ComponentBase
    {
       
        [SupplyParameterFromForm]
        LoginModel Model { get; set; } = new LoginModel();

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IAuthService AuthService { get; set; }
        [Inject]
        private UserService UserService { get; set; }
        private string username;
        public string ErrorMessage { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Model ??= new();



        }
        private async Task HandleLogin()
        {
            try
            {
                
                var user = await AuthService.Login(Model.Username, Model.Password);
                UserService.SetLoggedInUsername(Model.Username);
                if (user != null)
                {
                    switch (user.RoleId)
                    {
                        case 1:
                            NavigationManager.NavigateTo("/waiter");
                            break;
                        case 3:
                            NavigationManager.NavigateTo("/dashboard");
                            break;
                        case 2:
                            NavigationManager.NavigateTo("/chef");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ErrorMessage = "Korisničko ime ili lozinka je netočno";
                }
                
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
        }

    }

}
