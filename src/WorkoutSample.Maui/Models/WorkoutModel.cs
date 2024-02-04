namespace WorkoutSample.Maui.Models;

public class WorkoutsVm
{
    public List<WorkoutDto> Workouts { get; set; } = new();
}

public class WorkoutDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public List<ExerciseDto> Exercises { get; set; } = new();
}

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Reps { get; set; }
    public int Sets { get; set; }
    public int Weight { get; set; }
}