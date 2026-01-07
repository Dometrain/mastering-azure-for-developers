using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelInspiration.API.Itineraries.DbContexts;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var builtConfig = config.Build();

        config.AddAzureKeyVault(new Uri(builtConfig["KeyVaultUri"] ??
            throw new InvalidOperationException("Missing configuration value KeyVaultUri")),
            new DefaultAzureCredential());
    })
    .ConfigureServices((appBuilder, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            return new OpenApiConfigurationOptions()
            {
                Info = new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Travel inspiration itineraries endpoints",
                        Description = "All travel inspiration API endpoints related to itineraries.",
                    }
            };
        });

        var credential = new DefaultAzureCredential();
        var accessTokenResponse = credential.GetToken(
            new Azure.Core.TokenRequestContext(["https://database.windows.net/.default"]));

        var sqlConnection = new SqlConnection(
            appBuilder.Configuration["TravelInspirationDbConnection"] ??
                 throw new InvalidOperationException("Missing configuration value TravelInspirationDbConnection"))
                { AccessToken = accessTokenResponse.Token };

        services.AddDbContext<TravelInspirationDbContext>(options =>
            options.UseSqlServer(sqlConnection,
                sqlOptions => sqlOptions.EnableRetryOnFailure()));
    })
    .Build();

host.Run();
