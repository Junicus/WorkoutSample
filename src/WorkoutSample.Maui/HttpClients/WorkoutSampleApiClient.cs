using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ardalis.Result;
using WorkoutSample.Maui.Contracts;
using WorkoutSample.Maui.Models;

namespace WorkoutSample.Maui.HttpClients;

public class WorkoutSampleApiClient(HttpClient httpClient) : IWorkoutSampleApiClient
{
    public async Task<Result<LoginResponseModel>> LoginAsync(LoginModel model)
    {
        var response = await httpClient.PostAsJsonAsync("users/login", model);
        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
            return Result<LoginResponseModel>.Success(loginResponse!);
        }

        var contentStr = await response.Content.ReadAsStringAsync();
        return Result<LoginResponseModel>.Error("Failed to login");
    }

    public async Task<Result> RegisterUserAsync(RegisterUserModel model)
    {
        var response = await httpClient.PostAsJsonAsync("users/register", model);
        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        var contentStr = await response.Content.ReadAsStringAsync();
        return Result.Error("Failed to register user");
    }

    public async Task<Result<RefreshTokenResponseModel>> RefreshTokenAsync(RefreshTokenRequestModel model)
    {
        var response = await httpClient.PostAsJsonAsync("users/refresh", model);
        if (response.IsSuccessStatusCode)
        {
            var contentStr = await response.Content.ReadAsStringAsync();
            var refreshTokenResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponseModel>();
            return Result<RefreshTokenResponseModel>.Success(refreshTokenResponse!);
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return Result<RefreshTokenResponseModel>.Error("Failed to refresh token");
    }

    public async Task<Result<UserInfoModel>> GetUserInfoAsync(string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.GetAsync("users/userinfo");
        if (response.IsSuccessStatusCode)
        {
            var userInfo = await response.Content.ReadFromJsonAsync<UserInfoModel>();
            return Result<UserInfoModel>.Success(userInfo!);
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return Result<UserInfoModel>.Error("Failed to get user info");
    }

    public async Task<Result<WorkoutsVm>> GetWorkoutsAsync(string accessToken, DateOnly startDate,
        DateOnly endDate)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.GetAsync($"workouts?startDate={startDate}&endDate={endDate}");
        if (response.IsSuccessStatusCode)
        {
            var workouts = await response.Content.ReadFromJsonAsync<WorkoutsVm>();
            return Result<WorkoutsVm>.Success(workouts!);
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return Result<WorkoutsVm>.Error("Failed to get workouts");
    }
}