using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WeatherClient;
using System;
using System.Net.Http;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http.Json;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredSessionStorage(); 

// Add authentication services
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // Add this line to ensure it can be injected elsewhere

await builder.Build().RunAsync();

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorage;

    public CustomAuthenticationStateProvider(HttpClient httpClient, ISessionStorageService sessionStorage)
    {
        _httpClient = httpClient;
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await _sessionStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrEmpty(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

        return new AuthenticationState(new ClaimsPrincipal());
    }

    public async Task<LoginResult> Login(string username, string password)
    {
        var loginRequest = new { username, password };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:5091/api/token", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();

            // Store the token in local storage
            await _sessionStorage.SetItemAsync("authToken", token);

            // Set token in HTTP client header
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
             return new LoginResult { Success = true };
        }
        else
        {
            return new LoginResult { Success = false, ErrorMessage = "Failed to log in. Please check your credentials." };
        }
    }
    public async Task Logout()
    {
        // Clear authentication token from session storage
        await _sessionStorage.RemoveItemAsync("authToken");

        // Remove token from HTTP client header
        _httpClient.DefaultRequestHeaders.Authorization = null;

        // Notify authentication state changed
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
}
public class LoginResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
} 