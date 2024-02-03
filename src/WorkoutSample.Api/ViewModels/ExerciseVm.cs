namespace WorkoutSample.Api.ViewModels;

public class ExerciseVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Reps { get; set; }
    public int Sets { get; set; }
    public int Weight { get; set; }
}