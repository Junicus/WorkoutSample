using WorkoutSample.Maui.Models;

namespace WorkoutSample.Maui.Contracts;

public interface IAppService
{
    Task<bool> RefreshToken();
    public Task<AccessTokenResponse?> AuthenticateUser(LoginModel loginModel);
    Task<(bool IsSuccess, string ErrorMessage)> RegisterUser(RegistrationModel registerUser);
    Task<List<WorkoutModel>> GetWorkouts(DateOnly workoutDate);
}