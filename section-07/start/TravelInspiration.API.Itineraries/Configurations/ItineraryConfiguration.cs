using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelInspiration.API.Itineraries.Entities;

namespace TravelInspiration.API.Configurations;

public sealed class ItineraryConfiguration : IEntityTypeConfiguration<Itinerary>
{
    public void Configure(EntityTypeBuilder<Itinerary> builder)
    {
        builder.ToTable("Itineraries");
        builder.Property(i => i.Id).UseIdentityColumn();
        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Description).HasMaxLength(2500);
        builder.Property(i => i.UserId).IsRequired().HasMaxLength(100); 
    }
}
