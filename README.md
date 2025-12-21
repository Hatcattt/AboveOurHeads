# 🌍 Above Our Heads

Ever wondered what’s happening **above your head**? 🛰️

**Above Our Heads** is a cross-platform satellite tracker built with **C#** and **.NET MAUI** in **.NET 10**.
It lets you explore, visualize, and follow satellites orbiting the Earth — right from your mobile device or computer.

---

## 🚀 Planned Features

* Real-time satellite tracking
* Interactive map
* Custom satellite watchlist
* Notifications for satellite passes above your location
* Offline mode (local SQLite cache)
* Technical dashboard (altitude, speed, TLE data, etc.)

---

## 🧱 Tech Stack & Structure

* **Language:** C#, XAML
* **Version:** .NET 10
* **Framework:** .NET MAUI (Android / Windows / - iOS & macOS planned)
* **Data Storage:** SQLite
* **Pattern:** MVVM

### 🗂️ Project Structure (Example)

```
/AboveOurHeads
│
├── Data/                # Data sources, database config and models
├── Services/            # tracking services
├── ViewModels/          # MVVM ViewModels (logic & binding)
├── Views/               # XAML pages and UI components
├── Resources/           # Images, styles, ...
├── Helpers/             # Converters, extensions, utilities
└── App.xaml / App.xaml.cs
```

The goal is to keep the project **simple, modular, and maintainable**, without over-engineering.

## 🛠️ Build & Deployment

*(Coming soon)*

---

## 📸 Screenshots

*(Coming soon)*

---

## 🪪 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE.txt) file for details.