using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Web;
using TravelInspiration.Client.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVaultUri"] ??
        throw new InvalidOperationException("Missing configuration value KeyVaultUri")),
    new DefaultAzureCredential(), 
    new AzureKeyVaultConfigurationOptions
    {
        ReloadInterval = TimeSpan.FromMinutes(5) // Poll Key Vault every 5 minutes
    });

// Direct Key Vault secret retrieval
//var keyVaultUri = new Uri(builder.Configuration["KeyVaultUri"] ??
//    throw new InvalidOperationException("Missing configuration value KeyVaultUri"));
//var secretClient = new SecretClient(keyVaultUri,
//    new DefaultAzureCredential());
//var clientSecretResponse = await secretClient
//    .GetSecretAsync("TravelInspirationWebClientClientSecret", "version");
//var clientSecretValue = clientSecretResponse.Value.Value;

// for automatic secret pick up: EntraId--ClientSecret instead of 
// TravelInspirationWebClientClientSecret 

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
        options.ClientSecret = builder.Configuration["TravelInspirationWebClientClientSecret"] ??
            throw new InvalidOperationException("Missing configuration value TravelInspirationWebClientClientSecret");

    });

builder.Services.AddSingleton<IEncryptionService, KeyEncryptionService>();
builder.Services.AddSingleton<ISigningService, CertificateSigningService>();

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
