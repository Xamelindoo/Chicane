# Chicane · F1 Live Companion

### Maximilian Cichello · v1.0.0 · May 2026

---

## The Problem

Official F1 apps are **bloated, ad-ridden, and data-hungry**:

- **Forced accounts** — Must sign up before seeing anything
- **Full-screen video ads** — Between every screen
- **400+ MB** — Battery drain and storage waste
- **Always online** — Useless at circuits with spotty WiFi
- **Closed source** — Zero transparency on what code does with your data

---

## The Solution

**Chicane** — a Formula 1 companion app that is:

- **Open source** — Every line of code on GitHub, MIT licensed
- **Zero ads, zero tracking** — No telemetry, no analytics, no marketing
- **Cache-first** — SQLite local storage, loads instantly even offline
- **No account** — Just open and go. No email, no login, no password
- **Self-hostable** — Run your own backend with Docker Compose

---

## Architecture

```
MAUI Android  ◄─HTTPS─►  NGINX (DuckDNS + SSL)  ◄──►  ASP.NET Core 10 Backend
                                                         ├── /api/proxy → OpenF1
   SQLite (cache-first)                                  ├── /api/news  → RSS2JSON (5 feeds)
                                                         └── /api/weather → OpenWeatherMap
                                                              ▲
                                                        Redis Cache (shared)
```

### Key decisions

- **Transparent proxy** — Backend relays OpenF1 responses verbatim, zero data transformation
- **No secrets on client** — All API keys live on the backend `.env`
- **Dual cache** — Redis (shared, all devices) + SQLite (per-device)
- **Proxy, not aggregator** — Client uses battle-tested 845-line `OpenF1ApiService` class

---

## Features

| Home | Schedule | Leaderboard | Live | News |
|---|---|---|---|---|
| Next session countdown | Full season calendar | Driver standings | Real-time positions | 5-source RSS |
| Circuit weather | Upcoming + past toggle | Constructor standings | Track map with dots | F1 keyword filter |
| GP cards with images | Session times | Headshots + team colours | Telemetry per driver | English + local feeds |
| Top 3 drivers/teams | Podium results | Pull-to-refresh | Race control commentary | Pull-to-refresh |

**Also**: Driver detail with race history · Team detail with season stats · Settings with cache management

---

## Stack

| Layer | Technology |
|---|---|
| **Client** | .NET MAUI (Android), MVVM + CommunityToolkit.Mvvm, Shell navigation, SQLite |
| **Backend** | ASP.NET Core 10, Redis 7, Docker multi-stage |
| **Infra** | Proxmox VE, 2× LXC Debian 13, Nginx Proxy Manager, Let's Encrypt |
| **APIs** | OpenF1, OpenWeatherMap, RSS2JSON |

---

## Live Demo

### What to show

1. **Home** — Countdown to next F1 session, GP cards with circuit images, driver/team standings
2. **Schedule** — Upcoming GP list, tap one → session schedule with dates and times
3. **Leaderboard** — Toggle between drivers and constructors, tap driver → detail page
4. **Driver Detail** — Profile, race history with filter (Practice/Quali/Sprint/Race)
5. **Team Detail** — Constructor profile, season stats, podium highlights
6. **News** — Pull-to-refresh, articles from 5 motorsport sources
7. **Settings** — Cache breakdown, clear cache

---

## Development Process

Built iteratively with AI-assisted workflow:

| Iteration | Topic | Status |
|---|---|---|
| IT-01 | Bootstrap + Shell navigation | ✅ |
| IT-02 | Hero Section + countdown | ✅ |
| IT-03 | Category system (F1-only) | ✅ |
| IT-04 | SQLite cache-first + real APIs | ✅ |
| IT-05 | Live timing + track map | ✅ |
| IT-06 | UI hardening + error states | ✅ |
| IT-07 | Proxy backend + Docker deploy | ✅ |

**Tools**: OpenCode AI agents, Claude Code, custom skill system (MAUI expert, automatic testing)

---

## Roadmap

- **v1.1** — English UI default, news localization by country, settings picker
- **v1.2** — Push notifications for race start, iOS support
- **v1.3** — Driver comparison tool, qualifying results, tyre strategy
- **v2.0** — Multi-series support (WEC, MotoGP), user preferences

---

## Links

- **GitHub** — [github.com/Xamelindoo/Chicane](https://github.com/Xamelindoo/Chicane)
- **License** — MIT
- **Backend** — ASP.NET Core 10 + Redis + Docker

---

## Q&A

> *Why .NET MAUI instead of React Native / Flutter?*

MAUI gives native Android performance, C# end-to-end (client + backend), and zero JavaScript bridge overhead.

> *Why a backend at all? Why not call OpenF1 directly?*

The backend adds **shared caching** (Redis), **authentication** (OpenF1 Bearer tokens), and keeps **API keys off the client**. It also aggregates news and weather.

> *What about iOS?*

The MAUI project already targets iOS — it compiles and runs. Android-first because the target audience (F1 fans at circuits) overwhelmingly uses Android.

> *How do I run it myself?*

```
git clone git@github.com:Xamelindoo/Chicane.git
docker compose build && docker compose up -d
dotnet build src/Chicane.Mobile -f net10.0-android
```
