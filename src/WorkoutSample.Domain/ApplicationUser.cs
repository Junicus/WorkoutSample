using Microsoft.AspNetCore.Identity;

namespace WorkoutSample.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public List<Workout> Workouts { get; set; } = [];
}