using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using TravelInspiration.API.Itineraries.DbContexts;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
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

        // Get a token that allows access to Azure SQL databases 
        var accessTokenResponse = credential.GetToken(
            new TokenRequestContext(["https://database.windows.net/.default"]));

        // Add it when building the connection string
        var sqlConnection = new SqlConnection(
            appBuilder.Configuration.GetConnectionString("TravelInspirationDbConnection"))
            {
                AccessToken = accessTokenResponse.Token
            };

        services.AddDbContext<TravelInspirationDbContext>(options =>
            options.UseSqlServer(sqlConnection,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }));

    })
    .Build();

host.Run();
