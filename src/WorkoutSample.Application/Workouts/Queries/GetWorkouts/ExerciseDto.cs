namespace WorkoutSample.Application.Workouts.Queries.GetWorkouts;

public class ExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Reps { get; set; }
    public int Sets { get; set; }
    public int Weight { get; set; }
}