# OpenWeather MAUI 9.0 App Assignment

A simple .NET MAUI 9.0 weather app that displays current weather using OpenWeather API and device location.

## Prerequisites

- .NET 9.0 SDK installed
- MAUI 9.0 workload installed:
  ```bash
  dotnet workload install maui
  ```

## Assignment Requirements Completed ✅

- ✅ Create a .NET MAUI Weather application using OpenWeather
- ✅ Create a free account on OpenWeather to receive free API key
- ✅ Apply for and use the Free Student Initiative 
- ✅ Use the Free CurrentWeather API to receive the weather data from OpenWeather
- ✅ Use the Geolocation Services built into MAUI Essentials to receive current GPS Location
- ✅ Create a visual user experience of the data in MAUI
- ✅ Commit the changes
- ✅ Push to the Remote Git Repository

## Setup Instructions

### 1. Get OpenWeather API Key
1. Go to https://openweathermap.org/
2. Create a free account
3. Apply for Student Initiative: https://docs.openweather.co.uk/our-initiatives/student-initiative *(Do this first - may have delays)*
4. Get your API key from the dashboard

### 2. Configure the App
1. Open `MainPage.xaml.cs`
2. Find the line: `private const string ApiKey = "YOUR_API_KEY_HERE";`
3. Replace `YOUR_API_KEY_HERE` with your actual OpenWeather API key

### 3. Run the App
```bash
dotnet restore
dotnet build
dotnet run -f net9.0-android
```

## How to Use

1. Launch the app
2. Tap "Get Current Weather" button
3. Allow location permissions when prompted
4. View your current weather data

## Features

- 📍 Gets your current GPS location using MAUI Essentials
- 🌤️ Fetches weather data from OpenWeather CurrentWeather API
- 📱 Displays temperature, weather description, humidity, and wind speed
- 🎨 Simple, clean visual interface
- ⚡ Cross-platform (Android, iOS, Windows, macOS)

## API Used

- **OpenWeather Current Weather API**: https://openweathermap.org/current
- **Endpoint**: `https://api.openweathermap.org/data/2.5/weather`
- **Parameters**: lat, lon, appid, units=metric

## Technologies

- .NET 9.0 MAUI
- MAUI Essentials (Geolocation)
- HttpClient (API calls)
- System.Text.Json (JSON parsing)

## File Structure

```
OpenWeatherApp/
├── MainPage.xaml              # UI layout
├── MainPage.xaml.cs           # All logic and weather classes
├── Platforms/Android/         # Android permissions
├── OpenWeatherApp.csproj      # Project configuration  
└── README.md                  # This file
```

## Assignment Links

- OpenWeather API: https://openweathermap.org/
- Student Initiative: https://docs.openweather.co.uk/our-initiatives/student-initiative
- Current Weather API: https://openweathermap.org/current
- MAUI Geolocation: https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/geolocation