<div align="center">

# Chicane · F1 Live Companion

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android%2014%2B-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/maui/)
[![ASP.NET Core 10](https://img.shields.io/badge/ASP.NET%20Core-10-512BD4?logo=dotnet&logoColor=white)](https://devblogs.microsoft.com/dotnet/announcing-asp-net-core-in-dotnet-10/)
[![Redis](https://img.shields.io/badge/Redis-7%20Alpine-DC382D?logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)](https://docker.com)
[![MIT License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

Your Formula 1 companion. Real-time standings, race calendar, live timing, weather forecasts, and motorsport news in one app.

[Why Chicane?](#-why-chicane) ·
[Features](#-features) ·
[Architecture](#-architecture) ·
[Stack](#-stack) ·
[Setup](#-setup) ·
[License](#-license)

</div>

---

## 🏁 Why Chicane?

The official Formula 1 app comes with **ads**, **mandatory accounts**, and a **bloated UI** that burns battery and data. Chicane is the opposite:

| | Official F1 App | Chicane |
|---|---|---|
| **Open source** | ❌ Closed source | ✅ Every line visible |
| **Ads** | ❌ Full-screen video ads | ✅ Zero ads |
| **Account required** | ❌ Must sign up | ✅ No account, no login |
| **Offline** | ❌ Needs constant connection | ✅ Cache-first SQLite |
| **Self-hostable** | ❌ Impossible | ✅ Docker Compose, 2 commands |
| **Data privacy** | ❌ Tracks everything | ✅ No telemetry, no analytics |
| **Size** | ❌ 400+ MB | ✅ Lean APK, no bloat |
| **Track WiFi** | ❌ Barely loads | ✅ Works on spotty circuits |
| **Code quality** | ❌ Black box | ✅ MVVM, clean architecture |

Chicane is built for **real F1 fans** who want data — not a marketing funnel. It's **open source**, **self-hostable**, and **respects your privacy**. No accounts, no ads, no nonsense.

## 📱 Features

| Tab | Description |
|-----|-------------|
| **Home** | Next F1 session with live countdown, circuit weather, and horizontal GP cards showing upcoming race weekends with track images, dates, and locations. Top 3 driver and constructor standings at a glance. |
| **Schedule** | Full season calendar split into upcoming and past races. Each GP card shows the circuit layout, session times (Practice, Qualifying, Sprint, Race), and podium results for completed weekends. |
| **Leaderboard** | Driver and constructor championship standings with driver headshots, team colours, and live points. Toggle between pilots and teams with a single tap. |
| **Live** | Real-time position board during active sessions, track map with driver dots, telemetry per driver (speed, throttle, gear, DRS), and commentary stream from race control. Auto-polls every 3 seconds. |
| **Driver Detail** | Per-driver profile with full name, team, headshot, points, championship position, and filterable race history across all seasons. |
| **Team Detail** | Constructor profile with team logo, season stats, current drivers, and race results with podium finishes highlighted. |
| **News** | Multi-source RSS aggregator pulling from 5 motorsport feeds (Sky Sports F1, Motorsport Italia, FormulaPassion, FuoriTraiettoria, P300.it) with an F1 keyword relevance filter. |
| **Settings** | Cache management with breakdown by database size, image cache, and total. One-tap cache clearing. |

## 🏗 Architecture

```
┌──────────────────┐        HTTPS         ┌──────────────────┐        HTTP          ┌──────────────────────┐
│   MAUI Client    │ ──────────────────→  │  NGINX Proxy     │ ──────────────────→  │  ASP.NET Core 10     │
│   (Android)      │   DuckDNS + Let's    │  Manager         │   192.168.1.205:5000 │  Backend             │
│                  │   Encrypt SSL        │  192.168.1.201    │                      │                      │
│ SQLite cache-first│ ←────────────────── │                  │ ←──────────────────  │                      │
└──────────────────┘                      └──────────────────┘                      └──────────┬───────────┘
                                                                                                 │
                                                                          ┌──────────────────────┼───────────────┐
                                                                          │                      │               │
                                                                     ┌────▼─────┐          ┌─────▼──────┐  ┌─────▼──────┐
                                                                     │  Redis   │          │  OpenF1    │  │ RSS2JSON + │
                                                                     │  Cache   │          │  API       │  │ OpenWeather│
                                                                     └──────────┘          └────────────┘  └────────────┘
```

### Transparent proxy

The backend does **not** transform or aggregate OpenF1 data. It acts as a transparent relay:

```
Client → GET /api/proxy/drivers?session_key=123
         → Backend checks Redis (hit?) → return cached
         → Redis miss → OpenF1 API → store in Redis → return
```

The client speaks **native OpenF1 JSON format** (845-line `OpenF1ApiService` class). The backend adds only authentication and shared caching.

### Cache layers

| Layer | Technology | Scope | TTL |
|-------|-----------|-------|-----|
| **Redis** | Shared LXC container | All devices | 3s live · 30min standings · 1h schedule · 24h results |
| **SQLite** | Per-device | Single app instance | Cache-first, never re-fetches immutable data |

## 🛠 Stack

### Client
- **.NET MAUI** · Android 14+ target
- **MVVM** pattern with [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/)
- **Shell** navigation with query parameters
- **SQLite** via [sqlite-net-pcl](https://www.nuget.org/packages/sqlite-net-pcl/) for local cache
- **System.Text.Json** for API parsing
- **GraphicsView** + `IDrawable` for custom track map rendering
- **RefreshView** + `CollectionView` for pull-to-refresh lists

### Backend
- **ASP.NET Core 10** Web API
- **Redis** 7 Alpine for shared response caching
- **Docker** multi-stage build + Docker Compose
- **NGINX Proxy Manager** for reverse proxy and Let's Encrypt SSL
- **DuckDNS** dynamic DNS

### Infrastructure
- **Proxmox VE** hypervisor
- 2 × **LXC Debian 13** containers (backend + NGINX)
- Router port forwarding (TCP 80/443 → NGINX LXC)

### APIs
- **[OpenF1](https://openf1.org/)** — race data, standings, sessions, live timing
- **[OpenWeatherMap](https://openweathermap.org/)** — circuit weather forecasts (≤7 days)
- **[RSS2JSON](https://rss2json.com/)** — RSS feed aggregation

## 🚀 Setup

### Backend

```bash
# Clone repo
git clone git@github.com:Xamelindoo/Chicane.git
cd Chicane

# Create .env with API keys
cat > .env << EOF
OPENF1_USERNAME=your_email
OPENF1_PASSWORD=your_password
OPENWEATHER_API_KEY=your_owm_key
RSS2JSON_API_KEY=your_rss2json_key
EOF

# Deploy with Docker
docker compose build --no-cache && docker compose up -d

# Verify
curl http://localhost:5000/api/proxy/drivers?session_key=11281
curl http://localhost:5000/api/news
curl http://localhost:5000/api/weather?city=Monza&country=IT
```

### Client

```bash
# Build for Android
dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj -f net10.0-android
```

> **Note**: The client's `MauiProgram.cs` sets the HttpClient base address to `https://chicane-api.duckdns.org/`. Change this to your backend URL for self-hosting.

## 📸 Screenshots

Screenshots coming soon. Place them in `docs/screenshots/`:
- `home.png` — Home page with countdown and GP cards
- `schedule.png` — Race calendar
- `leaderboard.png` — Driver standings
- `live.png` — Live timing with track map
- `driver.png` — Driver detail
- `team.png` — Team detail
- `news.png` — News feed

## 📄 License

MIT © 2026 Maximilian Cichello
