﻿using As.Zavrsni.Aplication.Interface;
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
                            throw new Exception("User role not recognized");
                    }
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
                
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
        }

    }

}
