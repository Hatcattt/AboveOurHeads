# 🌍 Above Our Heads

[![CI](https://github.com/Hatcattt/AboveOurHeads/actions/workflows/ci.yml/badge.svg)](https://github.com/Hatcattt/AboveOurHeads/actions/workflows/ci.yml)

Ever wondered what’s happening **above your head**? 🛰️

**Above Our Heads** is a satellite tracking project built in **C#** and **.NET 10**, designed to be modular and extensible.
It started as a simple console experiment and is evolving toward a cross-platform experience with **.NET MAUI** (Android / Windows; iOS & macOS planned) and even web via **Blazor**, letting you explore satellites orbiting Earth from multiple interfaces.

This project uses **TLE (Two-Line Element) files** to calculate satellite positions and leverages the [SGP.NET library](https://github.com/parzivail/SGP.NET) for orbital calculations.

---

## 🚀 Current & Planned Features

Right now, the project focuses on **retrieving and displaying basic satellite information**.
You can already see satellite data either in a **console app** or via a **Blazor web page**.

**Current features include:**

* Retrieve satellite predictions and positions from TLE data
* Display satellite info in console (ISS as example)
* Simple Blazor web interface showing:

  * Name, NORAD ID
  * Position (Latitude, Longitude, Altitude)
  * Velocity (km/s and km/h)
  * Last updated timestamp

**Planned features:**

* Real-time satellite tracking with auto-refresh
* Interactive map visualization
* Custom satellite watchlist
* Notifications for satellite passes
* Offline mode with local caching
* Technical dashboard with altitude, speed, TLE data, and more

> Currently, the console and Blazor pages provide a working proof-of-concept. MAUI UI and interactive features will be added gradually.

---

## 🧱 Tech Stack & Architecture

* **Language:** C#, XAML
* **Version:** .NET 10
* **Framework:** .NET MAUI (mobile / desktop) & Blazor Web (server-side experimental)
* **Architecture:** Modular, multi-project solution for maintainability and scalability
* **Pattern:** MVVM (for MAUI), with services decoupled from UI
* **Orbit calculations:** [SGP.NET](https://github.com/parzivail/SGP.NET)

### 🗂️ Current Project Structure

The solution is organized in **multiple projects**:

```
/AboveOurHeads
│
├── .Core/               # Shared interfaces, models
├── .Services/           # SatelliteService, TleService, caching
├── .ConsoleApp/         # Temporary console UI for testing services
├── .MAUI/               # .NET MAUI cross-platform app
├── .BlazorWebApp/       # Early visual experiments with Blazor
└── .Tests/              # Unit tests for services (coverage in progress)
```

**Key design goals:**

* Keep services and business logic **decoupled** from UI
* Make the project **modular**: you can replace or extend UI (console, MAUI, web) without changing core logic
* Use caching (`IMemoryCache`) to improve performance and avoid unnecessary API calls
* Follow **clean code principles** to ensure maintainability as the project grows

---

## 🛠️ Build, Tests & CI/CD

Even if the full MAUI UI is still under development, you can **test the core functionality** via the console app or Blazor web page.

This project includes a **GitHub Actions CI pipeline** that runs automatically on pull requests and merges to `main`. The CI:

- Restores dependencies
- Builds the solution
- Runs all unit tests

You can also test locally:

### 1️⃣ Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) installed
* Optional: IDE such as **Visual Studio 2022/2026** or **VS Code**

### 2️⃣ Test the Console App

```bash
cd AboveOurHeads.ConsoleApp
dotnet restore
dotnet build
dotnet run
```

> Real-time satellite data (e.g., ISS) will be displayed every 2 seconds.

### 3️⃣ Test the Blazor Server App

```bash
cd BlazorWebApp\BlazorWebApp\
dotnet restore
dotnet build
dotnet run
```

> Open your browser at `https://localhost:5001` (or the provided URL) then under satellite tab to see a **working web page** displaying satellite information, including position, velocity, and last updated timestamp.

### 4️⃣ Run Unit Tests

```bash
cd AboveOurHeads.Tests
dotnet test
```

> Current tests cover main services like `TleProvider`. `TleParser`. More tests will be added progressively.

---

## 📸 Screenshots

*(Coming soon — first UI previews for MAUI & Blazor in development)*

---

## 🪪 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE.txt) file for details.