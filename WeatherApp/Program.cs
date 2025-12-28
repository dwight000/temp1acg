using System.Net.Http.Json;
using System.Text.Json.Serialization;

// Modern .NET 9 Weather App for Ellicott City, MD
// Uses: top-level statements, records, primary constructors, collection expressions,
// pattern matching, HttpClient best practices, and more!

const double lat = 39.2673;
const double lon = -76.7983;
const string location = "Ellicott City, MD";

// Create HttpClientHandler with system proxy configuration
var handler = new HttpClientHandler
{
    UseProxy = true,
    UseDefaultCredentials = true,
    Proxy = System.Net.WebRequest.GetSystemWebProxy()
};

using HttpClient client = new(handler)
{
    DefaultRequestHeaders = { { "User-Agent", "WeatherChecker/2.0" } },
    Timeout = TimeSpan.FromSeconds(30)
};

var checker = new WeatherChecker(client);
await checker.CheckWeatherAsync(lat, lon, location);

// File-scoped namespace with modern record types
file class WeatherChecker(HttpClient httpClient)
{
    public async Task CheckWeatherAsync(double latitude, double longitude, string locationName)
    {
        Console.WriteLine($"Checking weather for {locationName}...");
        Console.WriteLine(new string('=', 60));

        try
        {
            var forecast = await GetForecastAsync(latitude, longitude);
            var rainStatus = AnalyzeRainInNext5Hours(forecast);

            DisplayRainStatus(rainStatus);
            DisplayDetailedForecast(forecast);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    private async Task<ForecastData> GetForecastAsync(double lat, double lon)
    {
        // Get grid endpoint
        var pointsUrl = $"https://api.weather.gov/points/{lat},{lon}";
        var pointsData = await httpClient.GetFromJsonAsync<PointsResponse>(pointsUrl)
            ?? throw new InvalidOperationException("Failed to get points data");

        // Get hourly forecast
        var forecastUrl = pointsData.Properties.ForecastHourly;
        var forecast = await httpClient.GetFromJsonAsync<ForecastData>(forecastUrl)
            ?? throw new InvalidOperationException("Failed to get forecast data");

        return forecast;
    }

    private RainStatus AnalyzeRainInNext5Hours(ForecastData forecast)
    {
        // Collection expression - modern C# 12 feature
        string[] rainKeywords = ["rain", "shower", "drizzle", "precipitation", "storm", "thunderstorm"];

        var next5Hours = forecast.Properties.Periods.Take(5);
        var rainyPeriods = new List<RainyPeriod>();

        foreach (var period in next5Hours)
        {
            // Pattern matching with modern features
            var hasRain = rainKeywords.Any(keyword =>
                period.ShortForecast.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                period.DetailedForecast.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            if (hasRain)
            {
                rainyPeriods.Add(new RainyPeriod(
                    period.Name,
                    period.ShortForecast,
                    period.Temperature,
                    period.WindSpeed,
                    period.ProbabilityOfPrecipitation?.Value ?? 0
                ));
            }
        }

        return new RainStatus(rainyPeriods.Any(), rainyPeriods, next5Hours.ToList());
    }

    private static void DisplayRainStatus(RainStatus status)
    {
        // Pattern matching with switch expression
        var message = status switch
        {
            { HasRain: true, RainyPeriods.Count: > 0 } =>
                $"⚠️  RAIN EXPECTED in the next 5 hours! ({status.RainyPeriods.Count} period(s))",
            _ => "✓ No rain expected in the next 5 hours"
        };

        Console.WriteLine(message);
        Console.WriteLine();

        if (status.HasRain)
        {
            foreach (var period in status.RainyPeriods)
            {
                Console.WriteLine($"  {period.Time}: {period.Forecast}");
                Console.WriteLine($"    🌡️  Temp: {period.Temperature}°F | 💨 Wind: {period.WindSpeed}");
                if (period.PrecipitationChance > 0)
                {
                    Console.WriteLine($"    💧 Precipitation chance: {period.PrecipitationChance}%");
                }
                Console.WriteLine();
            }
        }
    }

    private static void DisplayDetailedForecast(ForecastData forecast)
    {
        Console.WriteLine("\nNext 5 hours forecast:");
        Console.WriteLine(new string('-', 60));

        foreach (var period in forecast.Properties.Periods.Take(5))
        {
            // Raw string literal (C# 11 feature) for better formatting
            var details = $$"""
                {{period.Name}}: {{period.ShortForecast}}
                  🌡️  {{period.Temperature}}°{{period.TemperatureUnit}} | 💨 {{period.WindSpeed}} {{period.WindDirection}}
                  {{period.DetailedForecast}}
                """;

            Console.WriteLine(details);
            Console.WriteLine();
        }
    }
}

// Modern record types with primary constructors
file record PointsResponse(PointsProperties Properties);
file record PointsProperties(string ForecastHourly);

file record ForecastData(ForecastProperties Properties);
file record ForecastProperties(List<Period> Periods);

file record Period(
    string Name,
    [property: JsonPropertyName("startTime")] DateTime StartTime,
    int Temperature,
    [property: JsonPropertyName("temperatureUnit")] string TemperatureUnit,
    [property: JsonPropertyName("windSpeed")] string WindSpeed,
    [property: JsonPropertyName("windDirection")] string WindDirection,
    [property: JsonPropertyName("shortForecast")] string ShortForecast,
    [property: JsonPropertyName("detailedForecast")] string DetailedForecast,
    [property: JsonPropertyName("probabilityOfPrecipitation")] PrecipitationInfo? ProbabilityOfPrecipitation
);

file record PrecipitationInfo(int Value);

// Status records for analysis results
file record RainStatus(bool HasRain, List<RainyPeriod> RainyPeriods, List<Period> AllPeriods);
file record RainyPeriod(
    string Time,
    string Forecast,
    int Temperature,
    string WindSpeed,
    int PrecipitationChance
);
