using As.Zavrsni.Aplication;
using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Services;
using As.Zavrsni.Presistance;
using As.Zavrsni.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSyncfusionBlazor();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ZavrsniDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IZavrsniDbContext, ZavrsniDbContext>();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(
   options =>  options.LoginPath = "/login" );
SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt8QHFqVkBrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRcQ11iTn9SdkZgWXdec3c=;Mgo+DSMBPh8sVXJ3S0d+X1dPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSH5ScURiWXxdcHxUTmk=;ORg4AjUWIQA/Gnt2VlhhQlJCfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn9Td0JiWH9dc3RdRWRZ;NRAiBiAaIQQuGjN/V0V+XU9HcFRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31SdkRkWH5ec3dVTmVcUw==;Mjc2NDg4MkAzMjMzMmUzMDJlMzBBbk9xNUw1dVJRSVk0bmFiRVRpMHoxNG43Y0pjQ0k3aUU3SWRJOCthYkdrPQ==;Mgo+DSMBMAY9C3t2VlhhQlJCfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn9Td0JiWH9dc3RdT2dc;Mjc2NDg4NEAzMjMzMmUzMDJlMzBHK1R0TzRPYXZhdElsMmVzcHNMbUtKaW9BTDhEYWFsOXU1b3gyUmllWk9vPQ==;Mjc2NDg4NUAzMjMzMmUzMDJlMzBBbk9xNUw1dVJRSVk0bmFiRVRpMHoxNG43Y0pjQ0k3aUU3SWRJOCthYkdrPQ==");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
