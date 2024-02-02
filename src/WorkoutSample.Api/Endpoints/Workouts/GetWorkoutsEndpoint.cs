using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutSample.Api.Endpoints.Workouts;

public class GetWorkoutsEndpoint : Endpoint<
    GetWorkoutsEndpoint.WorkoutsRequest,
    GetWorkoutsEndpoint.WorkoutsResponse>
{
    public override void Configure()
    {
        Get("/");
        Group<WorkoutsGroup>();
    }

    public override async Task HandleAsync(WorkoutsRequest req, CancellationToken ct)
    {
    }

    public class WorkoutsResponse
    {
    }

    public class WorkoutsRequest
    {
        [QueryParam] public DateOnly WorkoutDate { get; set; }
    }
}