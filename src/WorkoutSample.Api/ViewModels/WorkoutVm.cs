namespace WorkoutSample.Api.ViewModels;

public class WorkoutVm
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public UserVm User { get; set; } = null!;
    public List<ExerciseVm> Exercises { get; set; } = [];
}