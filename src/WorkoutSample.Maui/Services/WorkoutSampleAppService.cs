using System.Text.Json;
using Ardalis.Result;
using WorkoutSample.Maui.Contracts;
using WorkoutSample.Maui.Models;

namespace WorkoutSample.Maui.Services;

public class WorkoutSampleAppService(IWorkoutSampleApiClient workoutSampleApiClient) : IWorkoutSampleAppService
{
    public async Task<Result<bool>> RegisterUserAsync(string name, string email, string password)
    {
        var registerUserModel = new RegisterUserModel
        {
            Name = name,
            Email = email,
            Password = password
        };
        var response = await workoutSampleApiClient.RegisterUserAsync(registerUserModel);
        if (response.IsSuccess)
        {
            Settings.UserBasicDetails.Name = name;
            Settings.UserBasicDetails.Email = email;
            return true;
        }

        return Result<bool>.Error("Failed to register user");
    }

    public async Task<Result<LoginResponseModel>> LoginAsync(string email, string password)
    {
        var loginModel = new LoginModel
        {
            Email = email,
            Password = password
        };
        var response = await workoutSampleApiClient.LoginAsync(loginModel);
        if (response.IsSuccess)
        {
            Settings.UserBasicDetails.AccessToken = response.Value.AccessToken;
            Settings.UserBasicDetails.RefreshToken = response.Value.RefreshToken;
            Settings.UserBasicDetails.Email = email;
            var userDetailsString = JsonSerializer.Serialize(Settings.UserBasicDetails);
            await SecureStorage.SetAsync(nameof(Settings.UserBasicDetails), userDetailsString);
            return response;
        }

        return Result<LoginResponseModel>.Error("Failed to login");
    }

    public async Task<Result<UserInfoModel>> GetUserInfoAsync()
    {
        var response = await workoutSampleApiClient.GetUserInfoAsync(Settings.UserBasicDetails.AccessToken);
        if (response.IsSuccess)
        {
            Settings.UserBasicDetails.Name = response.Value.Name;
            var userDetailsString = JsonSerializer.Serialize(Settings.UserBasicDetails);
            await SecureStorage.SetAsync(nameof(Settings.UserBasicDetails), userDetailsString);
            return response;
        }

        return Result<UserInfoModel>.Error("Failed to get user info");
    }

    public async Task<Result<bool>> RefreshTokenAsync()
    {
        var isTokenRefreshed = false;
        var refreshTokenRequest = new RefreshTokenRequestModel
        {
            AccessToken = Settings.UserBasicDetails.AccessToken,
            RefreshToken = Settings.UserBasicDetails.RefreshToken,
        };
        var response = await workoutSampleApiClient.RefreshTokenAsync(refreshTokenRequest);
        if (response.IsSuccess)
        {
            Settings.UserBasicDetails.AccessToken = response.Value.AccessToken;
            Settings.UserBasicDetails.RefreshToken = response.Value.RefreshToken;

            var userDetailsString = JsonSerializer.Serialize(Settings.UserBasicDetails);
            await SecureStorage.SetAsync(nameof(Settings.UserBasicDetails), userDetailsString);
            isTokenRefreshed = true;
        }

        return isTokenRefreshed;
    }

    public async Task<Result<WorkoutsVm>> GetWorkoutsAsync(DateOnly startDate, DateOnly endDate)
    {
        var result =
            await workoutSampleApiClient.GetWorkoutsAsync(Settings.UserBasicDetails.AccessToken, startDate, endDate);
        return result;
    }
}