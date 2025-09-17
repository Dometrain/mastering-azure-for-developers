using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; 

namespace TravelInspiration.API.Itineraries.DbContexts
{
    /// <summary>
    /// Factory used at design-time (e.g. when running migrations)
    /// </summary>
    public class TravelInspirationDbContextFactory : IDesignTimeDbContextFactory<TravelInspirationDbContext>
    {
        public TravelInspirationDbContext CreateDbContext(string[] args)
        {
            // Get the directory where the assembly is located
            var basePath = Directory.GetCurrentDirectory();

            // Build the configuration from local.settings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get the connection string from the configuration
            var connectionString = configuration.GetConnectionString("TravelInspirationDbConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'TravelInspirationDbConnection' not found in local.settings.json.");
            }

            // Create DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<TravelInspirationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TravelInspirationDbContext(optionsBuilder.Options);
        }
    }
}