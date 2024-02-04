using Ardalis.Result;
using WorkoutSample.Maui.Models;

namespace WorkoutSample.Maui.Contracts;

public interface IWorkoutSampleApiClient
{
    Task<Result<LoginResponseModel>> LoginAsync(LoginModel model);
    Task<Result> RegisterUserAsync(RegisterUserModel model);
    Task<Result<RefreshTokenResponseModel>> RefreshTokenAsync(RefreshTokenRequestModel model);
    Task<Result<UserInfoModel>> GetUserInfoAsync(string accessToken);
    Task<Result<WorkoutsVm>> GetWorkoutsAsync(string accessToken, DateOnly startDate, DateOnly endDate);
}