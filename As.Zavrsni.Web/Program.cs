using As.Zavrsni.Aplication;
using As.Zavrsni.Aplication.Interface;
using As.Zavrsni.Presistance;
using As.Zavrsni.Web.Components;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ZavrsniDbContext>(options =>
    options.UseSqlServer(connectionString));

 builder.Services.AddScoped<IZavrsniDbContext, ZavrsniDbContext>();

builder.Services.AddSyncfusionBlazor();

SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0N+XU9Hc1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3pTdUdlWXZceXBURmBYUg==;Mgo+DSMBMAY9C3t2UFhhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTX5QdENjUH1XdHVVQ2Fe;MzE4NTA5MUAzMjM1MmUzMDJlMzBBWDBvc3JXRm0xdXlGZVNLdzN5WXNUMjVUVWNkYW5aVHJUejhvSDFHTlMwPQ==");
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
