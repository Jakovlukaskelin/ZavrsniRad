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
using BCrypt.Net;

namespace As.Zavrsni.Web.Components.Pages.Manager
{
    public partial class CrudUser : IDisposable
    {
        private SfGrid<User> Grid;
        private List<User> Users = new List<User>();
        private User currentUser = new User();
        private bool IsAddDialogVisible = false;
        private bool IsEditDialogVisible = false;
        private bool IsEdit = false;
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
            IsEdit = false;
            currentUser = new User();
            IsAddDialogVisible = true;
        }

        private void OpenEditUserDialog(User user)
        {
            IsEdit = true;
            currentUser = new User
            {
                UserId = user.UserId,
                Username = user.Username,
                Password = string.Empty,
                RoleId = user.Role.RoleName == "Admin" ? 1 : 2
            };
            IsEditDialogVisible = true;
        }

        private async Task SaveUser()
        {
            if (IsEdit)
            {
                var user = await DbContext.Users.FindAsync(currentUser.UserId);
                if (user != null)
                {
                    user.Username = currentUser.Username;
                    user.RoleId = currentUser.RoleId;
                    if (!string.IsNullOrWhiteSpace(currentUser.Password))
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(currentUser.Password);
                    }
                   
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                currentUser.Password = BCrypt.Net.BCrypt.HashPassword(currentUser.Password);
                await DbContext.Users.AddAsync(currentUser);
                await DbContext.SaveChangesAsync();
            }

            await LoadUsers();
            await Grid.Refresh();
            IsAddDialogVisible = false;
            IsEditDialogVisible = false;
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
            IsEditDialogVisible = false;
        }

        private void CloseAddDialog()
        {
            IsAddDialogVisible = false;
        }

        private void CloseEditDialog()
        {
            IsEditDialogVisible = false;
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

