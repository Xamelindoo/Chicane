# IT-04 — Standings & Teams con persistenza SQLite e API reali

## Obiettivo
Dotare l'app di un layer SQLite locale e integrare API reali (Sportmonks, OpenWeather, RSS News) per F1, mantenendo dati mock per WEC, WRC e MotoGP.

## Architettura dati

```
ViewModel
  ↓
Cached*Service (fetch → SQLite → return)
  ↓
Dispatching*Service (route: f1→Sportmonks, others→Mock)
  ├── SportmonksService (F1)     └── Mock*Service (WEC/WRC/MotoGP)
  ↓
SQLite (chicane.db) — 4 tabelle per categoria
```

## File creati

### SQLite Layer
- `Models/Storage/StandingsRecord.cs`
- `Models/Storage/ConstructorStandingsRecord.cs`
- `Models/Storage/RaceScheduleRecord.cs`
- `Models/Storage/TeamRecord.cs`
- `Models/Storage/StorageMapper.cs`
- `Services/DatabaseService.cs`
- `Services/Repositories/StandingsRepository.cs`
- `Services/Repositories/ConstructorStandingsRepository.cs`
- `Services/Repositories/ScheduleRepository.cs`
- `Services/Repositories/TeamsRepository.cs`
- `Services/CachedF1DataService.cs`
- `Services/CachedTeamsService.cs`
- `Services/CachedScheduleService.cs`

### API Integration
- `Models/Api/ErgastDto.cs`
- `Models/Api/SportmonksDto.cs`
- `Models/Api/OpenWeatherDto.cs`
- `Models/Api/OpenF1Dto.cs`
- `Services/ErgastApiService.cs` (fallback)
- `Services/SportmonksService.cs` (F1 primary, standings+schedule+results)
- `Services/DispatchingF1DataService.cs`
- `Services/DispatchingScheduleService.cs`
- `Services/OpenWeatherService.cs`
- `Services/OpenWeatherMapper.cs`
- `Services/ErgastMapper.cs`
- `Services/SecretsConfig.cs`

### News
- `Models/NewsItem.cs`
- `Services/INewsService.cs`
- `Services/Rss2JsonService.cs` (dual-source: SkySports UK + Motorsport IT)
- `ViewModels/NewsViewModel.cs` (rewritten)
- `Views/NewsPage.xaml` (rewritten)

### Weather
- `Models/WeatherForecast.cs`
- `Services/IWeatherService.cs`

### Fonts
- `Resources/Fonts/Outfit-Regular.ttf`, `Outfit-Bold.ttf`
- `Resources/Fonts/TitilliumWeb-Regular.ttf`, `TitilliumWeb-SemiBold.ttf`, `TitilliumWeb-Bold.ttf`

### Docs
- `docs/iterations/it-04-sqlite-api.md`

## File modificati

- `MauiProgram.cs` — DI registrations (SQLite, API services, dispatching, fonts)
- `ViewModels/HomeViewModel.cs` — weather, circuit map, SimulateLive
- `ViewModels/LeaderboardsViewModel.cs` — CategoryProvider subscription
- `ViewModels/ScheduleViewModel.cs` — CategoryProvider subscription
- `ViewModels/TeamDetailViewModel.cs` — categoryId propagation
- `ViewModels/TeamRaceListViewModel.cs` — categoryId propagation
- `ViewModels/PastRaceDetailViewModel.cs` — categoryId propagation
- `Models/NewsPage.xaml` — rewritten news layout
- `Resources/Styles/Styles.xaml` — Outfit/Titillium font styles
- `App.xaml.cs` — SecretsConfig init (later removed for lazy loading)
- All 8 View XAML files — SpaceMono → TitilliumWeb font migration
- `.gitignore` — `**/secrets.json`

## API Keys (not committed)

| Key | File | Status |
|-----|------|--------|
| Sportmonks | `SecretsConfig.cs` | `git assume-unchanged` |
| OpenWeather | `SecretsConfig.cs` | `git assume-unchanged` |
| Rss2Json | `SecretsConfig.cs` | `git assume-unchanged` |
| NewsAPI (unused) | `SecretsConfig.cs` | Not used anymore |

## Data sources by category

| Categoria | Standings | Schedule | Past Results | News |
|-----------|-----------|----------|-------------|------|
| F1 | Sportmonks | Sportmonks | Sportmonks | Dual RSS |
| WEC | Mock | Mock | Mock | RSS (Motorsport WEC) |
| WRC | Mock | Mock | Mock | RSS (Motorsport WRC) |
| MotoGP | Mock | Mock | Mock | RSS (Motorsport MotoGP) |

## Acceptance criteria

- [x] Standings F1 from Sportmonks API (20+ real drivers)
- [x] Schedule F1 from Sportmonks fixtures
- [x] SQLite persistence with offline fallback
- [x] Weather: OpenWeather (≤7 days) / approximate (8-16 days) / N/A (>16 days)
- [x] News per category via RSS feeds
- [x] Circuit maps (F1 CDN for 10 countries)
- [x] WEC/WRC/MotoGP mock data preserved
- [ ] Full past race detail with real results and standings (partial: results only, standings stub)

## Future migration

OpenF1 Pro (€9.90/mese) will replace Sportmonks for live data, weather, and sessions in IT-05+, keeping Sportmonks as historical/standings backup.
