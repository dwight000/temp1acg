# temp1acg

## Weather Applications

This repository contains two implementations of a weather checker for Ellicott City, MD that checks if rain is expected in the next 5 hours.

### Python Version

**File:** `weather.py`

A simple Python script using the National Weather Service API.

**Requirements:**
- Python 3.x
- `requests` library

**Setup:**
```bash
# Install dependencies
pip install requests
```

**Usage:**
```bash
python3 weather.py
```

### .NET Version

**Directory:** `WeatherApp/`

A modern .NET 9 console application showcasing the latest C# 12 features including top-level statements, record types, primary constructors, collection expressions, raw string literals, and pattern matching.

**Requirements:**
- .NET 9.0 SDK

**Setup:**

If you don't have .NET 9.0 installed, download it from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0) or use the install script:

```bash
# Linux/macOS
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0
```

**Build:**
```bash
cd WeatherApp
dotnet build
```

**Run:**
```bash
cd WeatherApp
dotnet run
```

Or build and run in one step:
```bash
cd WeatherApp
dotnet run
```

---

Both applications use the free National Weather Service API to fetch hourly weather forecasts and analyze them for rain-related conditions. No API key required.