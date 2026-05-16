# Test Matrix

## Build verification

| ID | Check | Command | Status |
|----|-------|---------|--------|
| B-01 | Backend builds | `dotnet build src/Chicane.Backend/` | ✅ |
| B-02 | Client builds | `dotnet build src/Chicane.Mobile/ -f net10.0-android` | ✅ |

## Smoke tests (manual)

| ID | Scenario | Steps | Status |
|----|----------|-------|--------|
| S-01 | App startup | Launch app → HomePage loads with event, countdown, weather | ✅ |
| S-02 | Schedule navigation | Tap Schedule tab → upcoming GP list visible, toggle to past | ✅ |
| S-03 | Leaderboard | Tap Leaderboard tab → driver standings visible, toggle to teams | ✅ |
| S-04 | News | Tap News tab → article list populated, pull-to-refresh | ✅ |
| S-05 | Settings | Tap gear icon → cache breakdown visible, clear cache works | ✅ |
| S-06 | Driver detail | Tap driver in standings → detail page with race history | ✅ |
| S-07 | Team detail | Tap team in standings → team profile with stats | ✅ |
| S-08 | Live page | Navigate to Live during active session → positions, track map | ✅ |
| S-09 | GP cards | HomePage horizontal scroll → circuit images, dates, locations | ✅ |
| S-10 | Backend proxy | `curl /api/proxy/meetings?year=2026` → valid OpenF1 JSON | ✅ |
| S-11 | Backend weather | `curl /api/weather?city=Monza&country=IT` → forecast JSON | ✅ |
| S-12 | Backend news | `curl /api/news` → F1 articles array | ✅ |

## UI state coverage

| State | Pages covered |
|-------|---------------|
| Loading | Home, Schedule, Leaderboard, News, Team Detail, Driver Detail, Live |
| Error | Home, Schedule, Leaderboard, News, Team Detail, Driver Detail |
| Empty | Home, Schedule, Leaderboard, News, Team Detail, Driver Detail |
| Data | All pages |
