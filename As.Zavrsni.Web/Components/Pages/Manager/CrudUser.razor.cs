using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Model;
using As.Zavrsni.Domain.Entites;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Grids;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace As.Zavrsni.Web.Components.Pages.Manager
{
    public partial class CrudUser : IDisposable
    {
        private SfGrid<User> Grid;
        private List<User> Users = new List<User>();
        private User currentUser = new User();
        private bool isDialogVisible = false;
        private bool isEdit = false;
        private string? currentUrl;
        [Inject]
        private IZavrsniDbContext DbContext { get; set; }

        protected override async Task OnInitializedAsync()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
            await LoadUsers();
        }

        private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            await InvokeAsync(StateHasChanged);
        }

        private async Task LoadUsers()
        {
            Users = await DbContext.Users.ToListAsync();
        }

        private void OpenAddUserDialog()
        {
            isEdit = false;
            currentUser = new User();
            isDialogVisible = true;
        }

        private void OpenEditUserDialog(User user)
        {
            isEdit = true;
            currentUser = new User
            {
                
                Username = user.Username,
                Password = user.Password
            };
            isDialogVisible = true;
        }

        private async Task SaveUser()
        {
            if (isEdit)
            {
                var user = await DbContext.Users.FindAsync(currentUser.UserId);
                if (user != null)
                {
                    user.Username = currentUser.Username;
                    user.Password = currentUser.Password;
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                await DbContext.Users.AddAsync(currentUser);
                await DbContext.SaveChangesAsync();
            }

            await LoadUsers();
             await Grid.Refresh();
            isDialogVisible = false;
        }

        private async Task DeleteUser(User user)
        {
            var userToDelete = await DbContext.Users.FindAsync(user.UserId);
            if (userToDelete != null)
            {
                DbContext.Users.Remove(userToDelete);
                await DbContext.SaveChangesAsync();
            }

            await LoadUsers();
           await Grid.Refresh();
        }

        private void CloseDialog()
        {
            isDialogVisible = false;
        }

        private void Logout()
        {
            NavigationManager.NavigateTo("/login");
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
