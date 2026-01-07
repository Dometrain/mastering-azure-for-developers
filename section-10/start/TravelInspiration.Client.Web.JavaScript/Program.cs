using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddOpenIdConnectAccessTokenManagement();

builder.Services.AddHttpClient("ItinerariesApiClient", config =>
{
    config.BaseAddress = new Uri(builder.Configuration["ItinerariesApiRoot"] ??
        throw new InvalidOperationException("Missing configuration value: ItinerariesApiRoot"));
}).AddUserAccessTokenHandler();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = builder.Configuration["EntraIdConfiguration:Authority"] ??
        throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:Authority");
    options.ClientId = builder.Configuration["EntraIdConfiguration:ClientId"] ??
        throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ClientId");
    options.ClientSecret = options.ClientSecret = builder.Configuration["EntraIdConfiguration:ClientSecret"] ??
        throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ClientSecret");
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.MapInboundClaims = false;
    options.Scope.Add(builder.Configuration["EntraIdConfiguration:ItinerariesApiScope"] ??
        throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ItinerariesApiScope"));
    options.Scope.Add("offline_access");
    options.TokenValidationParameters.NameClaimType = "name";
    options.TokenValidationParameters.RoleClaimType = "role";
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
