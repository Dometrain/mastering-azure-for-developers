using Microsoft.Identity.Web;
using TravelInspiration.Client.Web.DelegatingHandlers;
using TravelInspiration.Client.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register EasyAuthProvider
builder.Services.AddScoped<IEasyAuthProvider, EasyAuthProvider>();
builder.Services.AddTransient<EasyAuthTokenRefreshHandler>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("EntraId"))
    .EnableTokenAcquisitionToCallDownstreamApi()
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
}).AddMicrosoftIdentityAppAuthenticationHandler("ItinerariesApiHandler",
    builder.Configuration.GetSection("ItinerariesApi"));

builder.Services.AddHttpClient("UserBasedItinerariesApiClient", config =>
{
    config.BaseAddress = new Uri(builder.Configuration["ItinerariesApi:Root"] ??
        throw new InvalidOperationException("Missing configuration value: ItinerariesApi:Root"));
}).AddHttpMessageHandler<EasyAuthTokenRefreshHandler>();

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
