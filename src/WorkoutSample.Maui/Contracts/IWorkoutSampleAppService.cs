using Ardalis.Result;
using WorkoutSample.Maui.Models;

namespace WorkoutSample.Maui.Contracts;

public interface IWorkoutSampleAppService
{
    public Task<Result<bool>> RegisterUserAsync(string name, string email, string password);
    public Task<Result<LoginResponseModel>> LoginAsync(string email, string password);
    public Task<Result<UserInfoModel>> GetUserInfoAsync();
    public Task<Result<bool>> RefreshTokenAsync();
    public Task<Result<WorkoutsVm>> GetWorkoutsAsync(DateOnly startDate, DateOnly endDate);
}