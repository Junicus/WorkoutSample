using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutSample.Domain;

namespace WorkoutSample.Infrastructure.Persistence.Configuration;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasOne<Workout>(e => e.Workout)
            .WithMany(e => e.Exercises)
            .HasForeignKey(e => e.WorkoutId)
            .HasPrincipalKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(250)
            .IsRequired();
    }
}