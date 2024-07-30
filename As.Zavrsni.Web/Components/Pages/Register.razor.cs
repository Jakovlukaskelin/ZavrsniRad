using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Domain.Entites;
using Microsoft.AspNetCore.Components;

namespace As.Zavrsni.Web.Components.Pages
{
    public partial class Register : ComponentBase
    {

        [SupplyParameterFromForm]
        private RegisterFormModel Model { get; set; } = new RegisterFormModel();
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IZavrsniDbContext DbContext { get; set; }
        public async Task RegisterUser()
        {  
            var user = new User
            {
                Username = Model.UserName,
                Password = Model.Password
            };

           DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/");
        }
    }
}
