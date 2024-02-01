using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutSample.Domain;

namespace WorkoutSample.Infrastructure.Persistence.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasMany(x => x.Workouts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}