﻿using As.Zavrsni.Aplication.Model;
using As.Zavrsni.Web.Components.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Syncfusion.Blazor.Navigations;

namespace As.Zavrsni.Web.Components.Pages.Manager
{
    public partial class Manager : IDisposable
    {
        [Inject]
        private NavigationManager NavManager { get; set; }

        public Users users { get; set; } = new Users(); 

        private string? currentUrl;

        protected override void OnInitialized()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
            LoadUser(); 
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        private void LoadUser()
        {
            
            users.username = users.username;
        }
        private void Logout()
        {

            NavManager.NavigateTo("/login");
        }
        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
