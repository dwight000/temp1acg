#!/usr/bin/env python3
"""
Weather checker for Ellicott City, MD
Checks if rain is expected in the next 5 hours
"""

import requests
import json
from datetime import datetime, timedelta

def get_weather_ellicott_city():
    """Get weather forecast for Ellicott City, MD using National Weather Service API"""
    # Ellicott City, MD coordinates
    lat, lon = 39.2673, -76.7983

    try:
        # Get the forecast grid endpoint for this location
        points_url = f"https://api.weather.gov/points/{lat},{lon}"
        response = requests.get(points_url, headers={'User-Agent': 'WeatherChecker/1.0'})
        response.raise_for_status()
        points_data = response.json()

        # Get hourly forecast URL
        forecast_hourly_url = points_data['properties']['forecastHourly']

        # Fetch hourly forecast
        forecast_response = requests.get(forecast_hourly_url, headers={'User-Agent': 'WeatherChecker/1.0'})
        forecast_response.raise_for_status()
        forecast_data = forecast_response.json()

        return forecast_data
    except Exception as e:
        print(f"Error fetching weather data: {e}")
        return None

def check_rain_next_5_hours(forecast_data):
    """Check if rain is expected in the next 5 hours"""
    if not forecast_data:
        return None

    periods = forecast_data['properties']['periods'][:5]  # Next 5 hours

    rain_expected = False
    rain_periods = []

    for period in periods:
        forecast = period['shortForecast'].lower()
        detailed = period['detailedForecast'].lower()

        # Check for rain-related keywords
        rain_keywords = ['rain', 'shower', 'drizzle', 'precipitation', 'storm']
        if any(keyword in forecast or keyword in detailed for keyword in rain_keywords):
            rain_expected = True
            rain_periods.append({
                'time': period['name'],
                'forecast': period['shortForecast'],
                'temp': period['temperature'],
                'wind': period['windSpeed']
            })

    return rain_expected, rain_periods, periods

def main():
    print("Checking weather for Ellicott City, MD...")
    print("=" * 60)

    forecast_data = get_weather_ellicott_city()

    if forecast_data:
        result = check_rain_next_5_hours(forecast_data)
        if result:
            rain_expected, rain_periods, all_periods = result

            if rain_expected:
                print("⚠️  RAIN EXPECTED in the next 5 hours!\n")
                for period in rain_periods:
                    print(f"  {period['time']}: {period['forecast']}")
                    print(f"    Temperature: {period['temp']}°F, Wind: {period['wind']}")
            else:
                print("✓ No rain expected in the next 5 hours\n")

            print("\nNext 5 hours forecast:")
            print("-" * 60)
            for period in all_periods:
                print(f"{period['name']}: {period['shortForecast']}")
                print(f"  Temp: {period['temperature']}°F, Wind: {period['windSpeed']}")
                print()
    else:
        print("Failed to retrieve weather data")

if __name__ == "__main__":
    main()
