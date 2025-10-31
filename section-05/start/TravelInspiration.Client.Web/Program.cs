using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using TravelInspiration.Client.Web.ConfigureOptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddClientCredentialsHttpClient("ItinerariesApiClient",
    ClientCredentialsClientName.Parse("ItinerariesClientCredentialsFlow"), client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ItinerariesApiRoot"] ??
             throw new InvalidOperationException("Missing configuration value: ItinerariesApiRoot"));
    });

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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
