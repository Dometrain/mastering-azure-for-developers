using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using TravelInspiration.Client.Web.ConfigureOptions;
using TravelInspiration.Client.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"] ??
        throw new InvalidOperationException("Missing configuration value KeyVaultUri")),
        new DefaultAzureCredential(),
        new AzureKeyVaultConfigurationOptions
        {
            ReloadInterval = TimeSpan.FromMinutes(5)
        });

//// Direct Key Vault secret retrieval
//var keyVaultUri = new Uri(builder.Configuration["KeyVaultUri"] ??
//    throw new InvalidOperationException("Missing configuration value KeyVaultUri"));
//var secretClient = new SecretClient(keyVaultUri,
//    new DefaultAzureCredential());
//var clientSecretResponse = await secretClient
//    .GetSecretAsync("TravelInspirationWebClientClientSecret",
//        "version");
//var clientSecretValue = clientSecretResponse.Value.Value;

builder.Services.AddSingleton<IEncryptionService, KeyEncryptionService>();
builder.Services.AddSingleton<ISigningService, CertificateSigningService>();


builder.Services.AddControllersWithViews();

builder.Services.AddClientCredentialsTokenManagement();

builder.Services.AddSingleton(new DiscoveryCache(
    builder.Configuration["EntraIdConfiguration:Authority"] ??
        throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:Authority"),
    new DiscoveryPolicy { ValidateEndpoints = false }));

builder.Services.AddSingleton<IConfigureOptions<ClientCredentialsClient>, 
    ClientCredentialsClientConfigureOptions>();

builder.Services.AddClientCredentialsHttpClient("DestinationsApiClient", 
    ClientCredentialsClientName.Parse("DestinationsClientCredentialsFlow"), client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["DestinationsApiRoot"] ??
             throw new InvalidOperationException("Missing configuration value: DestinationsApiRoot"));
    });

//builder.Services.AddClientCredentialsHttpClient("ItinerariesApiClient",
//    ClientCredentialsClientName.Parse("ItinerariesClientCredentialsFlow"), client =>
//    {
//        client.BaseAddress = new Uri(builder.Configuration["ItinerariesApiRoot"] ??
//             throw new InvalidOperationException("Missing configuration value: ItinerariesApiRoot"));
//    });

//builder.Services.AddHttpClient("DestinationsApiClient", config =>
//{
//    config.BaseAddress = new Uri(builder.Configuration["DestinationsApiRoot"] ??
//        throw new InvalidOperationException("Missing configuration value: DestinationsApiRoot"));
//});

//builder.Services.AddHttpClient("ItinerariesApiClient", config =>
//{
//    config.BaseAddress = new Uri(builder.Configuration["ItinerariesApiRoot"] ??
//        throw new InvalidOperationException("Missing configuration value: ItinerariesApiRoot"));
//});

//builder.Services.AddHttpClient("EntraIdClient", config =>
//{
//    config.BaseAddress = new Uri(builder.Configuration["EntraIdConfiguration:Authority"] ??
//        throw new InvalidOperationException("Missing configuration value: EntraIdConfiguration:Authority"));
//});

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
    //options.ClientSecret = builder.Configuration["TravelInspirationWebClientClientSecret"] ??
    //    throw new InvalidOperationException("Missing configuration value TravelInspirationWebClientClientSecret");
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.MapInboundClaims = false;
    options.Scope.Add(builder.Configuration["EntraIdConfiguration:ItinerariesApiScope"] ??
        throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ItinerariesApiScope"));
    options.Scope.Add("offline_access");
    options.TokenValidationParameters.NameClaimType = "name";
    options.TokenValidationParameters.RoleClaimType = "role";
    options.Events = new OpenIdConnectEvents
    {
        OnAuthorizationCodeReceived = context =>
        {
            var configuration = context.HttpContext.RequestServices
                .GetRequiredService<IConfiguration>();
            context.TokenEndpointRequest!.ClientSecret = configuration["TravelInspirationWebClientClientSecret"] ??
                throw new InvalidOperationException("Missing configuration value TravelInspirationWebClientClientSecret");

            return Task.CompletedTask;
        }
    };
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
