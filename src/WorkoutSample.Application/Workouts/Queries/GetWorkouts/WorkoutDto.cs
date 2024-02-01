namespace WorkoutSample.Application.Workouts.Queries.GetWorkouts;

public class WorkoutDto
{
    public DateOnly Date { get; set; }
    public IReadOnlyCollection<ExerciseDto> Exercises { get; set; } = [];
}