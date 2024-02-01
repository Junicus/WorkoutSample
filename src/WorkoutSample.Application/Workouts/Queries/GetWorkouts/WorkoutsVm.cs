namespace WorkoutSample.Application.Workouts.Queries.GetWorkouts;

public class WorkoutsVm
{
    public IReadOnlyCollection<WorkoutDto> Workouts { get; init; } = [];
}