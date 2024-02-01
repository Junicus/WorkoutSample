using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutSample.Application.Exercises.Commands.CreateExercise;
using WorkoutSample.Application.Exercises.Queries.GetExcercise;
using WorkoutSample.Application.Workouts.Commands.CreateWorkout;
using WorkoutSample.Application.Workouts.Queries.GetWorkout;
using WorkoutSample.Application.Workouts.Queries.GetWorkouts;

namespace WorkoutSample.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class WorkoutsController(ISender sender) : ControllerBase
{
    [HttpGet("{workoutDate}", Name = "GetWorkouts")]
    public async Task<WorkoutsVm> Get(DateOnly workoutDate)
    {
        return await sender.Send(new GetWorkouts { Date = workoutDate });
    }

    [HttpGet("{id:guid}", Name = "GetWorkout")]
    public async Task<WorkoutDto> Get(Guid id)
    {
        return await sender.Send(new GetWorkout { Id = id });
    }

    [HttpPost]
    public async Task<IResult> CreateWorkout(CreateWorkout command)
    {
        var workoutId = await sender.Send(command);
        return Results.CreatedAtRoute("GetWorkout", new { id = workoutId.ToString() }, workoutId);
    }

    [HttpGet("{id:guid}/exercises/{exerciseId:guid}", Name = "GetExercise")]
    public async Task<ExerciseDto> GetExercise(Guid id, Guid exerciseId)
    {
        return await sender.Send(new GetExercise { Id = exerciseId });
    }

    [HttpPost("{id:guid}/exercises")]
    public async Task<IResult> CreateExercise(Guid id, CreateExercise command)
    {
        command.WorkoutId = id;
        var exerciseId = await sender.Send(command);
        return Results.CreatedAtRoute("GetExercise", new { id = id, exerciseId = exerciseId.ToString() }, exerciseId);
    }
}