using FastEndpoints;

namespace WorkoutSample.Api.Endpoints.Workouts;

public sealed class WorkoutsGroup : Group
{
    public WorkoutsGroup()
    {
        Configure("workouts", ep => { ep.Description(x => x.WithTags("Workouts")); });
    }
}