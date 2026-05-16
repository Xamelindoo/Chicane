# IT-07 — Proxy Backend Architecture

**Date**: May 2026 · **Branch**: `backend_creation`  
**Status**: Completed

## Goal

Introduce an ASP.NET Core 10 backend that acts as a **transparent proxy** to the OpenF1 API, with Redis caching and zero client-side secrets.

## Architecture

```
MAUI Client → HTTPS (DuckDNS) → NGINX Proxy Manager → Backend :5000
                                                            ├── /api/proxy/{**path} → OpenF1 (Redis cache)
                                                            ├── /api/news            → RSS2JSON (5 feeds, F1 filter)
                                                            └── /api/weather         → OpenWeatherMap
```

## Key decisions

- **Proxy, not aggregator**: The backend relays OpenF1 responses verbatim. The client uses its original `OpenF1ApiService` (845 lines, battle-tested) with only the BaseAddress changed.
- **Redis TTL**: 3s live data, 30min standings, 1h schedule, 24h results.
- **Zero client secrets**: `OpenF1TokenService`, `Env.cs`, and `secrets.json` removed from mobile app. All API keys live on the backend via `.env`.
- **Infrastructure**: 2 LXC containers (Debian 13) on Proxmox — backend + NGINX. DuckDNS + Let's Encrypt for HTTPS.

## Backend files

- `src/Chicane.Backend/Controllers/ProxyController.cs` — transparent `/api/proxy/{**path}` relay
- `src/Chicane.Backend/Controllers/WeatherController.cs` — `/api/weather` with Redis cache
- `src/Chicane.Backend/Controllers/NewsController.cs` — `/api/news` RSS aggregator
- `src/Chicane.Backend/Services/OpenF1Service.cs` — Bearer auth + raw forwarding
- `src/Chicane.Backend/Services/OpenWeatherService.cs` — OpenWeatherMap proxy
- `src/Chicane.Backend/Services/Rss2JsonService.cs` — multi-feed RSS with F1 keyword filter

## Client changes

- Restored original `OpenF1ApiService` (845 lines) — native OpenF1 format parsing
- Removed `OpenF1TokenService`, `Env.cs`, `secrets.json`
- BaseAddress: `https://chicane-api.duckdns.org/api/proxy/`
- `OpenWeatherService` and `Rss2JsonService` → thin wrappers calling backend endpoints

## Dead code removal

- 7 aggregator controllers deleted (Home, Standings, Schedule, Teams, Drivers, Sessions, Live)
- `ChicaneApiClient` removed
- 4 dead model/mapper files cleaned up
- Backend `OpenF1Service`: 287 → 52 lines (-82%)

## Deploy

```bash
scp -r src/Chicane.Backend/ root@<lxc>:/opt/chicane-backend/
ssh root@<lxc> "cd /opt/chicane-backend && docker compose build --no-cache && docker compose up -d"
```
