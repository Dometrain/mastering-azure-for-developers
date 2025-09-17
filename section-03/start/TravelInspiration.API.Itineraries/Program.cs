using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        services.AddDbContext<TravelInspirationDbContext>(options =>
            options.UseSqlServer(appBuilder.Configuration.GetConnectionString("TravelInspirationDbConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));
    })
    .Build();

host.Run();
