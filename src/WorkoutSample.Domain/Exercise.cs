namespace WorkoutSample.Domain;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Reps { get; set; }
    public int Sets { get; set; }
    public int Weight { get; set; }

    public Guid WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
}