# Chicane — Iteration Summary

## IT-01 — Bootstrap
**Branch**: `main` · **Status**: ✅ Complete

Initialize the .NET MAUI project with Shell navigation, basic pages (Home, Standings, Teams, News), MVVM structure, and font setup. Sets the foundation for all subsequent iterations.

**Key files**: `MauiProgram.cs`, `AppShell.xaml`, `App.xaml`

---

## IT-02 — Hero Section
**Branch**: `it-02` · **Status**: ✅ Complete

Build the full-screen Hero Section with pre-race state (countdown, event name, weather placeholder, circuit info) and live state (session data). Implements `IDispatcherTimer` for real-time countdown.

**Key files**: `HomePage.xaml`, `HomeViewModel.cs`, `RaceEvent.cs`

---

## IT-03 — Multi-Series Category System
**Branch**: `it-03` · **Status**: ✅ Complete (later simplified to F1-only)

Implements support for F1, WEC, WRC, and MotoGP with per-category accent colors, dispatching services, and a category switcher. This was later pared down to F1-only to focus on real API data quality over mock data breadth.

**Key files**: `CategoryProvider`, theme system

---

## IT-04 — SQLite Persistence & Real APIs
**Branch**: `it-04` · **Status**: ✅ Complete

Replace mock data with real API integration (OpenF1, OpenWeatherMap, RSS2JSON), implement SQLite cache-first architecture with 6 repository tables, add image caching (`ImageCacheService`), and build the driver detail / team detail pages with race history.

**Key decisions**:
- Cache-first pattern: SQLite → API fallback
- Immutable data (past results, calendar) never re-fetched
- Background image downloading with independent error handling
- Team logo assets embedded for 11 F1 teams

**Key files**: `CacheService.cs`, `DatabaseService.cs`, `ImageCacheService.cs`, all repositories, `CachedF1DataService.cs`, `CachedTeamsService.cs`, `OpenF1ApiService.cs`

---

## IT-05 — Live Timing & Track Map
**Branch**: `it-05_live_data_deepseek` · **Status**: ✅ Complete

Build the Live page with real-time driver positions, telemetry (speed, throttle, gear, RPM, DRS), race control commentary stream, and a custom circuit track map rendered via `GraphicsView` + `TrackMapDrawable` (`IDrawable`). Polls OpenF1 every 3 seconds.

**Key decisions**:
- 3-second polling interval (balance between freshness and server load)
- Client-side track map rendering with driver position dots
- Live detection: session `DateStart <= UtcNow <= DateEnd`
- "SEGUI LIVE" button on Home only visible during active sessions

**Key files**: `LivePage.xaml`, `LiveViewModel.cs`, `TrackMapDrawable.cs`, `LiveDriverRow.cs`

---

## IT-06 — UI Hardening
**Branch**: `it_06_hardening_ui_deepseek` · **Status**: ✅ Complete

Polish the entire app with explicit UI state management (loading/error/empty/data), compact gradient headers with settings gear icon on all 4 tabs, cache management in Settings (breakdown + clear all), weather DateTimeKind UTC normalization, and meetings cache year invalidation.

**Key improvements**:
- Loading/Error/Empty/Data states on every page
- Thinner header (padding `20,6,20,4`) with gear icon (⚙️)
- Settings: cache breakdown (DB/Images/Total MB) + "CLEAR CACHE"
- Weather: normalize DateTimeKind to UTC, refresh on resume
- Meetings cache: year-based invalidation
- LIVE guard: countdown pill doesn't navigate unless session is active

**Key files**: `HomePage.xaml`, `SettingsPage.xaml`, `SettingsViewModel.cs`, `CacheService.cs`

---

## IT-07 — Proxy Backend Architecture
**Branch**: `backend_creation` · **Status**: ✅ Complete

Introduce an ASP.NET Core 10 backend acting as a transparent proxy to the OpenF1 API, with Redis shared caching, RSS news aggregation, and weather proxying. Deploy on self-hosted Proxmox infrastructure with NGINX reverse proxy and Let's Encrypt SSL.

**Key decisions**:
- **Transparent proxy**: Backend relays OpenF1 responses verbatim — no data transformation. Client uses its original 849-line `OpenF1ApiService` (only `BaseAddress` changed).
- **Zero client secrets**: `OpenF1TokenService`, `Env.cs`, and `secrets.json` removed from the mobile app. All API keys live on the backend via `.env`.
- **Redis TTL tiers**: 3s live data, 30min standings, 1h schedule, 24h results.
- **Dual cache**: Shared Redis (all devices) + per-device SQLite (cache-first).
- **Infrastructure**: 2 LXC containers on Proxmox — `chicane-backend` (app + Redis) and `chicane-nginx` (reverse proxy + SSL). DuckDNS for dynamic DNS.

**Performance optimization (IT-07.1)**:
- Parallelized `HomeViewModel.LoadAsync` with `Task.WhenAll` (4 independent tasks)
- Fixed `GetNextEventAsync` to use `GetSessionsAsync` with in-memory cache
- Restored SQLite cache for GP weekend cards with smart invalidation
- Added Redis priming to `deploy.sh`
- Result: **5s → <500ms** load time for cached scenarios

**News pipeline (IT-07.3)**:
- 5 RSS feeds: Sky Sports F1, Motorsport Italia, FormulaPassion, FuoriTraiettoria, P300.it
- F1 keyword filter: 23 inclusion keywords + 13 exclusion keywords
- Safe DateTime parsing for malformed RSS dates

**Team points fix (IT-07.2)**:
- Sprint sessions now use correct scoring (8-7-6-5-4-3-2-1)
- Race sessions use standard scoring (25-18-15-12-10-8-6-4-2-1)
- Header total always from API `constructorStandings.PointsCurrent` (authoritative)

**English UI (IT-07.5)**:
- 11 Italian strings translated to English
- All XAML labels and ViewModel strings updated

**Key files**: `ProxyController.cs`, `NewsController.cs`, `WeatherController.cs`, `OpenF1Service.cs`, `Rss2JsonService.cs`, `docker-compose.yml`, `deploy.sh`

---

## Version History

| Version | Date | Highlights |
|---------|------|------------|
| **v1.0.0** | May 2026 | Complete app: Home, Schedule, Leaderboard, Live, Driver/Team detail, News, Settings, SQLite cache-first |
| **v1.0.1** | May 2026 | 4x performance optimization: parallel loading, sessions cache, SQLite weekends, Redis priming |
| **v1.0.2** | May 2026 | Sprint-aware team points, English UI, comment cleanup, dead code removal |
