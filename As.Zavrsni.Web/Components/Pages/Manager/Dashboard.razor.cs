using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;

namespace As.Zavrsni.Web.Components.Pages.Manager
{
    public partial class Dashboard
    {
        [Inject]
        private NavigationManager NavManager { get; set; }
        private string? currentUrl;
        protected override void OnInitialized()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;

        }
        private void Logout()
        {

            NavManager.NavigateTo("/login");
        }
        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }



        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
