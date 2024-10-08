using As.Zavrsni.Aplication;
using As.Zavrsni.Aplication.Infrastructure.AutoMapper;
using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Aplication.Services;
using As.Zavrsni.Presistance;
using As.Zavrsni.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using System.Reflection;
using MediatR;
using As.Zavrsni.Aplication.Products.Query;
using As.Zavrsni.Aplication.Infrastructure;
using As.Zavrsni.Aplication.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddSignalR();
builder.Services.AddDbContext<ZavrsniDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IZavrsniDbContext, ZavrsniDbContext>();
builder.Services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetProductListQuery)));
builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<IMediator, Mediator>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(
   options =>  options.LoginPath = "/login" );
SyncfusionLicenseProvider.RegisterLicense("MzQyODQzM0AzMjM2MmUzMDJlMzBTbGRCS2svY3psbGsvWkpmNkUwVGk0L1k2TmM4cHRaazFIaTdQY0JMR01FPQ==");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseEndpoints(endpoints =>
{

     endpoints.MapHub<OrderHub>("/orderHub");
    
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
