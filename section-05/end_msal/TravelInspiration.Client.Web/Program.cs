using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration,
        "EntraId")
    .EnableTokenAcquisitionToCallDownstreamApi(
         builder.Configuration.GetSection("ItinerariesApi:UserBasedScopes").Get<string[]>())
    .AddInMemoryTokenCaches();

builder.Services.AddHttpClient("DestinationsApiClient", config =>
{
    config.BaseAddress = new Uri(builder.Configuration["DestinationsApi:Root"] ??
        throw new InvalidOperationException("Missing configuration value: DestinationsApi:Root"));
}).AddMicrosoftIdentityAppAuthenticationHandler("DestinationsApiHandler",
    builder.Configuration.GetSection("DestinationsApi"));

builder.Services.AddHttpClient("ItinerariesApiClient", config =>
{
    config.BaseAddress = new Uri(builder.Configuration["ItinerariesApi:Root"] ??
        throw new InvalidOperationException("Missing configuration value: ItinerariesApiRoot"));
}).AddMicrosoftIdentityUserAuthenticationHandler("ItinerariesApiHandler",
    builder.Configuration.GetSection("ItinerariesApi"));

// Configure TokenValidationParameters from appsettings
builder.Services.Configure<Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions>(
    Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme,
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
