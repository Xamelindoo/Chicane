# Project Plan — Chicane

## Completed iterations

| # | Name | Branch | Status |
|---|------|--------|--------|
| IT-01 | Bootstrap | — | ✅ Done |
| IT-02 | Hero Section | — | ✅ Done |
| IT-03 | Multi-series → F1-only | — | ✅ Done (later simplified) |
| IT-04 | SQLite + Real APIs | — | ✅ Done |
| IT-05 | Live Timing & Track Map | `it-05_live_data_deepseek` | ✅ Done |
| IT-06 | UI Hardening | `it_06_hardening_ui_deepseek` | ✅ Done |
| IT-07 | Proxy Backend | `backend_creation` | ✅ Done |

## Roadmap (future)

- **v1.1.0** — iOS support, push notifications for race start
- **v1.2.0** — Driver comparison, qualifying results, tyre strategy
- **v2.0.0** — Multi-series support (WEC, MotoGP), user preferences

## Technical constraints

- .NET MAUI Android-first (iOS optional)
- MVVM with CommunityToolkit.Mvvm
- Shell navigation
- SQLite cache-first (sqlite-net-pcl)
- HTTP calls via System.Text.Json
- Backend: ASP.NET Core 10, Redis, Docker
- Proxy architecture: no data transformation on backend
- No secrets on client
