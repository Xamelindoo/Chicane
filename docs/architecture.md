# Architecture — Chicane

## 1. Overview

Chicane follows a client-server architecture where the MAUI Android client communicates exclusively through an ASP.NET Core backend acting as a transparent proxy to external APIs.

```
MAUI Client ↔ NGINX Proxy Manager ↔ ASP.NET Core Backend ↔ Redis
                                                          ↔ OpenF1 API
                                                          ↔ OpenWeatherMap
                                                          ↔ RSS2JSON
```

## 2. Client (MAUI)

### Folder structure

```
src/Chicane.Mobile/
├── Views/            # XAML pages with compiled bindings, no business logic
├── ViewModels/       # MVVM state, commands, service orchestration
├── Services/         # API access, caching, repositories, data orchestration
├── Models/           # Domain models, DTOs, storage records
└── Resources/        # Fonts, raw assets, styles
```

### Key patterns

- **MVVM**: CommunityToolkit.Mvvm with `[ObservableProperty]` and `[RelayCommand]`
- **Shell navigation**: Query parameters for detail pages (sessionKey, driverNumber, teamName)
- **Cache-first**: Cached services wrap API services, check SQLite first, fetch/update on miss
- **Image caching**: `ImageCacheService` downloads and caches remote images locally
- **UI states**: All ViewModels expose `IsBusy`, `HasError`, `HasData`, `IsEmptyState`

## 3. Backend (ASP.NET Core 10)

### Folder structure

```
src/Chicane.Backend/
├── Controllers/      # ProxyController, NewsController, WeatherController
├── Services/         # OpenF1Service (auth + proxy), Rss2JsonService, OpenWeatherService
├── Models/           # DTOs for external API responses
└── Program.cs        # DI, CORS, Redis, Swagger
```

### Proxy pattern

The backend does not aggregate or transform data. It acts as a transparent relay:

```
Client → GET /api/proxy/drivers?session_key=123
         Backend: check Redis → cache hit? return cached
         Backend: cache miss → authenticate to OpenF1 → forward request → store in Redis → return raw JSON
```

### Controllers

| Controller | Route | Function |
|-----------|-------|----------|
| ProxyController | `GET /api/proxy/{**path}` | Transparent relay to OpenF1 with Redis caching |
| NewsController | `GET /api/news` | RSS aggregation from 5 feeds with F1 keyword filter |
| WeatherController | `GET /api/weather?city=&country=&date=` | OpenWeatherMap proxy with Redis cache |

### Redis TTL

| Data type | TTL |
|-----------|-----|
| Live data (positions, intervals, car data, race control, etc.) | 3s |
| Standings (drivers, constructors) | 30min |
| Schedule (meetings, sessions) | 1h |
| Results (session results) | 24h |
| Default | 5min |

## 4. Infrastructure

- **Proxmox VE** hypervisor
- 2 × LXC Debian 13 containers: `chicane-backend` (192.168.1.205) and `chicane-nginx` (192.168.1.201)
- **Docker Compose**: backend (.NET 10) + Redis 7 Alpine
- **NGINX Proxy Manager**: reverse proxy, Let's Encrypt SSL
- **DuckDNS**: `chicane-api.duckdns.org`
- Router TCP port forwarding 80/443 → NGINX LXC
