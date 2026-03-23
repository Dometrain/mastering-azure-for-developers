using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "EntraId")
   .EnableTokenAcquisitionToCallDownstreamApi()
   .AddInMemoryTokenCaches();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme,
    options =>
    {
        var nameClaimType = builder.Configuration["EntraId:TokenValidationParameters:NameClaimType"];
        var roleClaimType = builder.Configuration["EntraId:TokenValidationParameters:RoleClaimType"];
        if (!string.IsNullOrEmpty(nameClaimType))
        {
            options.TokenValidationParameters.NameClaimType = nameClaimType;
        }
        if (!string.IsNullOrEmpty(roleClaimType))
        {
            options.TokenValidationParameters.RoleClaimType = roleClaimType;
        }
    });
var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
