using Azure.Identity;
using TravelInspiration.API.Destinations;
using TravelInspiration.API.Destinations.Shared.Slices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.RegisterApplicationServices();

// Alternative: APPLICATIONINSIGHTS_AUTHENTICATION_STRING / Authorization=AAD;ClientId=4eecc25a-7bb2-488d-b031-ff1bef89ba76

builder.Services.Configure<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>(config =>
{
    config.SetAzureTokenCredential(new DefaultAzureCredential(
        new DefaultAzureCredentialOptions()
            { ManagedIdentityClientId = "4eecc25a-7bb2-488d-b031-ff1bef89ba76" }));
});

builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("DestinationsReadRoleIsRequired", policy =>
        policy.RequireRole("Destinations.Read"));

builder.Services.AddAuthentication()
    .AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}
app.UseStatusCodePages();
 
app.MapSliceEndpoints();

app.Run();