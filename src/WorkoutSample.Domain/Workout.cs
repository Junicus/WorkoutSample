namespace WorkoutSample.Domain;

public class Workout
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public List<Exercise> Exercises { get; set; } = [];
}