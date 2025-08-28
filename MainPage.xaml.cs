using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using System.Text.Json;

namespace OpenWeatherApp;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;

    // Replace with your OpenWeather API key
    private const string ApiKey = "cc8098a8853318f3f0a8b00c00f6cfdc";
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();

        // Add headers to match browser behavior
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "OpenWeatherApp/1.0");
    }

    private async void OnGetWeatherClicked(object sender, EventArgs e)
    {
        await GetWeatherAsync();
    }

    private async Task GetWeatherAsync()
    {
        try
        {
            // Show loading
            SetLoadingState(true);
            HideError();

            // Step 1: Get current location using MAUI Essentials Geolocation
            var location = await GetCurrentLocationAsync();
            if (location == null)
            {
                ShowError("Could not get your location. Please enable location permissions.");
                return;
            }

            // Display location coordinates
            LocationLabel.Text = $"Location: {location.Latitude:F2}, {location.Longitude:F2}";

            System.Diagnostics.Debug.WriteLine($"Location obtained: {location.Latitude}, {location.Longitude}");

            // Step 2: Get weather data from OpenWeather API
            var weather = await GetWeatherDataAsync(location.Latitude, location.Longitude);
            if (weather == null)
            {
                ShowError("Could not get weather data. Please check your internet connection and API key.");
                return;
            }

            // Step 3: Display weather data visually
            DisplayWeatherData(weather);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetWeatherAsync error: {ex}");
            ShowError($"Error: {ex.Message}");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            };

            var location = await Geolocation.GetLocationAsync(request);

            return location;
        }
        catch (FeatureNotSupportedException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Geolocation not supported: {ex.Message}");
            ShowError("Geolocation is not supported on this device.");
            return null;
        }
        catch (FeatureNotEnabledException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Geolocation not enabled: {ex.Message}");
            ShowError("Geolocation is not enabled. Please enable location services.");
            return null;
        }
        catch (PermissionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location permission denied: {ex.Message}");
            ShowError("Location permission denied. Please grant location permissions.");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
            ShowError($"Location error: {ex.Message}");
            return null;
        }
    }

    private async Task<WeatherResponse?> GetWeatherDataAsync(double latitude, double longitude)
    {
        try
        {
            // Build OpenWeather API URL
            var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric";

            // Add debug output to see what's happening
            System.Diagnostics.Debug.WriteLine($"Making API call to: {url}");

            // Make API call with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var response = await _httpClient.GetStringAsync(url);

            System.Diagnostics.Debug.WriteLine($"API Response received: {response}");

            // Check if response is valid
            if (string.IsNullOrWhiteSpace(response))
            {
                System.Diagnostics.Debug.WriteLine("Empty response from API");
                return null;
            }

            // Deserialize JSON response
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(response, options);
            System.Diagnostics.Debug.WriteLine($"Successfully parsed weather data for: {weatherData?.Name}");

            return weatherData;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Network error: {ex.Message}");
            ShowError("Network error. Check internet connection.");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Request timeout: {ex.Message}");
            ShowError("Request timed out. Try again.");
            return null;
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"JSON parsing error: {ex.Message}");
            ShowError("Error parsing weather data from server.");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"General API error: {ex.Message}");
            ShowError($"API error: {ex.Message}");
            return null;
        }
    }

    private void DisplayWeatherData(WeatherResponse weather)
    {
        try
        {
            // Display location
            CityLabel.Text = $"{weather.Name}, {weather.Sys?.Country}";

            // Display temperature
            TemperatureLabel.Text = $"{Math.Round(weather.Main.Temp)}°C";

            // Display weather description
            var description = weather.Weather?.FirstOrDefault()?.Description ?? "No description";
            DescriptionLabel.Text = char.ToUpper(description[0]) + description.Substring(1);

            // Display additional details
            FeelsLikeLabel.Text = $"Feels like {Math.Round(weather.Main.FeelsLike)}°C";
            HumidityLabel.Text = $"Humidity: {weather.Main.Humidity}%";
            WindLabel.Text = $"Wind: {weather.Wind?.Speed ?? 0} m/s";

            // Show details section
            DetailsStack.IsVisible = true;

            System.Diagnostics.Debug.WriteLine("Weather data displayed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error displaying weather data: {ex.Message}");
            ShowError("Error displaying weather information.");
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        GetWeatherBtn.IsEnabled = !isLoading;
        GetWeatherBtn.Text = isLoading ? "Getting Weather..." : "Get Current Weather";
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
        DetailsStack.IsVisible = false;

        System.Diagnostics.Debug.WriteLine($"Error shown to user: {message}");
    }

    private void HideError()
    {
        ErrorLabel.IsVisible = false;
    }
}

// Simple weather data classes for OpenWeather API response
public class WeatherResponse
{
    public string Name { get; set; } = string.Empty;
    public MainData Main { get; set; } = new();
    public WeatherInfo[]? Weather { get; set; }
    public WindData? Wind { get; set; }
    public SysData? Sys { get; set; }
}

public class MainData
{
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
}

public class WeatherInfo
{
    public string Description { get; set; } = string.Empty;
}

public class WindData
{
    public double Speed { get; set; }
}

public class SysData
{
    public string Country { get; set; } = string.Empty;
}