# Architecture & Decision Log — Chicane

> Reference document for presentations and technical deep-dives.  
> Covers the complete code structure, data flow, and rationale behind every architectural decision.

---

## Project Structure

```
src/
├── Chicane.Mobile/           # .NET MAUI Android client (MVVM, Shell, SQLite cache-first)
│   ├── Views/                # 12 XAML pages (no business logic)
│   ├── ViewModels/           # 12 ViewModels (state, commands, orchestration)
│   ├── Services/             # 22 services (API, cache, repos, infrastructure)
│   ├── Models/               # 30+ domain models, API DTOs, SQLite records
│   ├── Platforms/Android/    # Android manifest, MainActivity
│   └── Resources/            # Fonts (TitilliumWeb, SpaceMono, Outfit), images, styles
│
├── Chicane.Backend/          # ASP.NET Core 10 Web API (proxy, Redis, RSS aggregation)
│   ├── Controllers/          # 3 controllers: Proxy, News, Weather
│   ├── Services/             # 3 services: OpenF1 (auth+proxy), RSS, Weather
│   └── Models/               # OpenWeatherMap DTO
│
├── docker-compose.yml        # Backend (port 5000) + Redis 7 Alpine
└── deploy.sh                 # git pull → build → up -d
```

---

## Client: ViewModel → View Map

| Tab / Page | ViewModel | Key Dependencies | Shell Route |
|---|---|---|---|
| **Home** | `HomeViewModel` (496 loc) | IF1DataService, IWeatherService, CachedScheduleService, INewsService | `/home` |
| **Schedule** | `ScheduleViewModel` (112 loc) | OpenF1ApiService (concrete) | `/schedule` |
| **Live** | `LiveViewModel` (314 loc) | OpenF1ApiService, IDispatcherTimer (3s poll) | `/live?sessionKey=` |
| **Leaderboards** | `LeaderboardsViewModel` (163 loc) | ITeamsService, StandingsRepository | `/leaderboards?tab=` |
| **Driver** | `DriverDetailViewModel` (184 loc) | OpenF1ApiService | `/driver-detail?driverNumber=` |
| **Team** | `TeamDetailViewModel` (168 loc) | ITeamsService | `/team-detail?teamName=` |
| **GP Sessions** | `MeetingDetailViewModel` (82 loc) | OpenF1ApiService | `/meeting-detail?meetingKey=` |
| **Past Race** | `PastRaceDetailViewModel` (112 loc) | IRaceCalendarService | `/past-race-detail?sessionKey=` |
| **Team Races** | `TeamRaceListViewModel` (87 loc) | ITeamsService | `/team-race-list?teamName=` |
| **Team GP** | `TeamGPSessionsViewModel` (139 loc) | ITeamsService, OpenF1ApiService | `/team-gp-sessions?meetingKey=&teamName=` |
| **News** | `NewsViewModel` (92 loc) | INewsService | `/news` |
| **Settings** | `SettingsViewModel` (67 loc) | ICacheService | `/settings` |

All ViewModels follow the same pattern: `[ObservableProperty]` state fields (`IsBusy`, `HasError`/`ErrorMessage`, `HasData`, `IsEmptyState`), `[RelayCommand]` for actions, `[QueryProperty]` for Shell parameter binding.

---

## Client: Service Layer

### API Services (remote calls)

| Service | Lines | Role | Calls |
|---|---|---|---|
| **OpenF1ApiService** | 849 | Core data client. Implements IF1DataService + IRaceCalendarService. Parses native OpenF1 JSON. | `GET /api/proxy/...` via backend |
| **Rss2JsonService** | 34 | Thin wrapper for news | `GET /api/news` via backend |
| **OpenWeatherService** | 28 | Thin wrapper for weather | `GET /api/weather` via backend |

### Cache-First Services (decorator pattern)

| Service | Lines | Cache Strategy |
|---|---|---|
| **CachedF1DataService** | 120 | SQLite → API fallback for standings; pass-through for live data |
| **CachedScheduleService** | 137 | SQLite → API for schedule/past races; weekends always fresh |
| **CachedTeamsService** | 301 | SQLite → API → MockTeamsService fallback |

### Repositories (6 SQLite tables)

| Repository | Table | Strategy |
|---|---|---|
| StandingsRepository | `DriverStandings` | Upsert by position |
| ConstructorStandingsRepository | `ConstructorStandings` | Upsert by position |
| ScheduleRepository | `RaceSchedule` | Delete-and-replace |
| TeamsRepository | `Teams` | Upsert by team name |
| SessionResultRepository | `SessionResults` | Delete-and-replace (immutable) |
| RaceWeekendRepository | `RaceWeekends` | Delete-all-and-replace |

### Infrastructure

| Service | Role |
|---|---|
| `DatabaseService` | SQLite schema (6 tables), versioned migration (`db_schema_version`) |
| `ImageCacheService` | SHA256-hashed local file cache for driver headshots / team logos |
| `CacheService` | Measures + clears cache (DB + images) |
| `MockTeamsService` | Empty fallback stub for non-F1 (unused in current F1-only mode) |

---

## Backend: Controller Map

| Controller | Route | Method | TTL | Upstream API |
|---|---|---|---|---|
| **ProxyController** | `GET /api/proxy/{**path}` | GET | 3s–24h (5 tiers) | `api.openf1.org/v1/` |
| **NewsController** | `GET /api/news` | GET | 15 min | `api.rss2json.com/v1/api.json` |
| **WeatherController** | `GET /api/weather?city=&country=&date=` | GET | 30 min | `api.openweathermap.org/data/2.5/forecast` |

### Redis TTL Tiers (ProxyController)

| TTL | Trigger (path contains) | Use case |
|---|---|---|
| **3s** | `position`, `intervals`, `car_data`, `location`, `race_control`, `pit`, `stints`, `overtakes`, `laps` | Live data |
| **30min** | `championship`, `standings` | Race weekend |
| **1h** | `meetings`, `sessions`, `schedule` | Season calendar |
| **24h** | `session_result`, `results` | Immutable results |
| **5min** | (default) | Everything else |

---

## Data Flow: End-to-End

```
┌─────────────────────────────────────────────────────────────────┐
│                        MAUI CLIENT (Android)                     │
│                                                                  │
│  View (XAML) → ViewModel → CachedService → SQLite?              │
│                                            ├─ HIT → return      │
│                                            └─ MISS → API call   │
│                                                                  │
│  OpenF1ApiService.GetStringAsync(                                │
│    "https://chicane-api.duckdns.org/api/proxy/drivers?...")      │
└──────────────────────────────┬──────────────────────────────────┘
                               │ HTTPS (DuckDNS + Let's Encrypt)
                               ▼
┌──────────────────────────────────────────────────────────────────┐
│               NGINX Proxy Manager (LXC 192.168.1.201)            │
│               Reverse proxy → Backend :5000                       │
└──────────────────────────────┬──────────────────────────────────┘
                               │ HTTP (internal)
                               ▼
┌──────────────────────────────────────────────────────────────────┐
│               ASP.NET Core 10 Backend (LXC 192.168.1.205)        │
│                                                                  │
│  ProxyController:                                                │
│    cacheKey = "drivers?session_key=123"                           │
│    Redis check → HIT? return cached                              │
│    Redis miss → OpenF1Service.GetRawStringAsync()                │
│      → Authenticate (Bearer token)                               │
│      → GET api.openf1.org/v1/drivers?session_key=123             │
│      → Store in Redis (TTL varies by endpoint)                    │
│      → Return raw JSON to client                                  │
└──────────────────────────────────────────────────────────────────┘
```

**Key insight**: The backend does NOT deserialise, transform, or aggregate OpenF1 data. It relays raw JSON verbatim. The client's 849-line `OpenF1ApiService` handles all parsing using native OpenF1 DTOs.

---

## Architectural Decisions

### 1. Proxy vs Aggregator Backend

**Chosen: Transparent proxy**

| Factor | Rationale |
|---|---|
| Existing client code | The 849-line `OpenF1ApiService` was already battle-tested with native OpenF1 JSON. An aggregator would require rewriting parsing on both backend AND client. |
| Backend simplicity | 3 controllers, 3 services, 0 data transformation. The proxy `OpenF1Service` is 52 lines (was 287 with aggregation). |
| No schema divergence | Client and server never disagree on JSON format. The `{**path}` catch-all route handles any future OpenF1 endpoint automatically. |
| Bug surface | Zero mapping bugs. Every 404/500 is a network issue, not a format mismatch. |

**Trade-off**: Client does more parsing, but that's where parsing belongs.

---

### 2. SQLite Cache-First on Client

**Chosen: SQLite with layered wrappers (CachedF1DataService, CachedScheduleService, CachedTeamsService)**

| Factor | Rationale |
|---|---|
| Circuit WiFi | F1 tracks have notoriously bad connectivity. Cache-first means the app works offline after first load. |
| Immutable data | Past race results, standings, and season calendar never change. Once fetched, they're never re-fetched. |
| Reduced backend load | Multiple views of the same data don't generate API calls. Only cache misses hit the backend. |
| Image pre-download | Background tasks download headshots/logos to local disk after cache writes. |

**Trade-off**: Slightly more complex service layer (3 cached wrappers), but this is isolated and well-documented.

---

### 3. F1-Only vs Multi-Series

**Chosen: F1-only (v1.0.0)**

| Factor | Rationale |
|---|---|
| Real API data | Only F1 has a reliable, free API (OpenF1). WEC, WRC, MotoGP were mock-only. |
| Code simplicity | Multi-series required per-category routing, mock services, category-switching UI — all for fake data. |
| User focus | F1 fans don't need WEC results cluttering the interface. |
| Deferred | Multi-series planned for v2.0.0 if real APIs become available. |

---

### 4. Zero Secrets on Client

**Chosen: Backend-only API keys**

| Factor | Rationale |
|---|---|
| Security | OpenF1 credentials, OpenWeatherMap key, RSS2JSON key must never leave the server. |
| Removed from client | `OpenF1TokenService`, `Env.cs`, `secrets.json` — all deleted. Client's `HttpClient` has no auth headers. |
| Backend .env | All secrets in `.env` (gitignored), injected via Docker Compose `environment:`. |

**Trade-off**: Backend is required (can't use OpenF1 directly), but this also enables shared Redis caching.

---

### 5. Shell Navigation vs Custom

**Chosen: MAUI Shell**

| Factor | Rationale |
|---|---|
| Built-in patterns | `TabBar`, `Routing.RegisterRoute`, `[QueryProperty]` — all needed for 4-tab + 8 detail push architecture. |
| Industry standard | Shell is the recommended MAUI navigation pattern. Custom routing would be redundant. |
| Flyout disabled | 4-tab layout doesn't need hamburger menu. |

---

### 6. CommunityToolkit.Mvvm

**Chosen: Yes — all 12 ViewModels use source generators**

| Factor | Rationale |
|---|---|
| Boilerplate elimination | `[ObservableProperty]` replaces manual INPC. `[RelayCommand]` replaces ICommand boilerplate. |
| Computed properties | `[NotifyPropertyChangedFor(nameof(HasError))]` keeps dependent properties in sync. |
| Microsoft-maintained | De facto standard for .NET MAUI MVVM. |

---

### 7. Docker + LXC Proxmox

**Chosen: Self-hosted on Proxmox VE**

| Factor | Rationale |
|---|---|
| Zero cloud costs | No AWS/Azure bills. Only electricity. |
| Full control | Every layer from hypervisor to application is under our control. |
| Docker Compose simplicity | 2 services in one file. `docker compose up -d` deploys everything. |
| NGINX Proxy Manager | Auto Let's Encrypt SSL cert renewal. DuckDNS dynamic DNS. |
| 2 LXC containers | Lightweight isolation: app+Redis on one, reverse proxy on another. |

---

## News Pipeline

```
5 RSS Feeds → RSS2JSON API → Backend Rss2JsonService
                                ├── Fetch each feed
                                ├── Parse items (title, description, image, source, date)
                                ├── Safety: custom DateTime parser for malformed pubDate
                                ├── F1 Filter: 23 inclusion keywords + 13 exclusion keywords
                                │     Inclusion: F1, Verstappen, Monza, qualifying...
                                │     Exclusion: MotoGP, WEC, TCR, IndyCar, NASCAR...
                                └── Sort by date, take top 20 → Redis (15min) → Client
```

**Feeds**: Sky Sports F1 (rss/12433), Motorsport Italia (rss/f1/news), FormulaPassion (/feed), FuoriTraiettoria (/feed), P300.it (/feed)

---

## Statistics

| Metric | Count |
|---|---|
| C# source files (Mobile) | ~75 |
| C# source files (Backend) | 9 |
| ViewModels | 12 |
| XAML Views | 12 |
| Service classes | 22 |
| Domain Models | 30+ |
| SQLite tables | 6 |
| Backend controllers | 3 |
| Backend services | 3 |
| Shell tabs | 4 |
| Shell routes | 8 |
| External APIs | 3 |
| RSS feeds | 5 |
| F1 keyword filters | 23 + 13 |
| Docker services | 2 |
| LXC containers | 2 |
| Redis TTL tiers | 5 |
