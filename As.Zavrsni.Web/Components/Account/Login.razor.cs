using As.Zavrsni.Aplication.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace As.Zavrsni.Web.Components.Account
{
    public partial class Login : ComponentBase
    {
        [CascadingParameter]
        public HttpContext HttpContext { get; set; } = default!;


        [SupplyParameterFromForm]
        LoginModel Model { get; set; } = new LoginModel();

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IAuthService AuthService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            Model ??= new();



        }
        private async Task HandleLogin()
        {
            var user = await AuthService.Login(Model.Username, Model.Password);
            if (user != null)
            {
                switch (user.RoleId)
                {
                    case 1:
                        NavigationManager.NavigateTo("/waiter");
                        break;
                    case 2:
                        NavigationManager.NavigateTo("/manager");
                        break;
                    case 3:
                        NavigationManager.NavigateTo("/chef");
                        break;
                    default:

                        break;
                }
            }
            else
            {
                throw new Exception("Invalid username or password");
            }
        }

    }

}
