namespace WorkoutSample.Api.ViewModels;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public UserDto User { get; set; } = null!;
    public List<ExerciseDto> Exercises { get; set; } = [];
}