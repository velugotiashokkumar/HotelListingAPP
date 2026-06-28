using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingApi.Data.Configurations
{
    public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.HasIndex(k => k.Key).IsUnique();
            builder.HasData(
                new ApiKey
                {
                    Id = 1,
                    AppName = "app",
                    CreatedAtUtc = new DateTime(2026, 06, 28),
                    Key = "dXNlcjRAbG9jYWxob3N0LmNvbTpQYXNzd29yZEAxMjM="
                }
            );
        }
    }
}
