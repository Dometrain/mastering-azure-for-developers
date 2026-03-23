using FluentValidation;
using System.Reflection;
using TravelInspiration.API.Destinations.Shared.Behaviours;
using TravelInspiration.API.Destinations.Shared.Metrics;
using TravelInspiration.API.Destinations.Shared.Slices;

namespace TravelInspiration.API.Destinations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    { 
        services.RegisterSlices();

        var currentAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(currentAssembly)
            .RegisterServicesFromAssemblies(currentAssembly)
                .AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>))
                .AddOpenBehavior(typeof(ModelValidationBehaviour<,>))
                .AddOpenBehavior(typeof(HandlerPerformanceMetricBehaviour<,>));
        }); 
        services.AddValidatorsFromAssembly(currentAssembly);
        services.AddSingleton<HandlerPerformanceMetric>(); 
        return services;
    } 
}
