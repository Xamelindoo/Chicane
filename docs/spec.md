# Product Specification — Chicane

## 1. Vision

Chicane is a Formula 1 companion app for Android, built with .NET MAUI. It provides real-time race data, standings, calendar, live timing, and news in a clean, ad-free interface.

## 2. Features

### Home
- Next F1 session with live countdown (days/hours/minutes)
- Circuit weather forecast (≤7 days via OpenWeatherMap)
- Horizontal GP cards showing upcoming race weekends with track images, date ranges, and locations
- Top 3 driver and constructor standings at a glance
- "SEGUI LIVE" button when a session is active

### Schedule
- Toggle between upcoming and past race weekends
- Each GP card: circuit image, date range, session schedule (Practice, Qualifying, Sprint, Race)
- Past races show podium (Top 3) with driver initials and race times

### Leaderboard
- Driver standings with headshot, position, team, points
- Constructor standings with team logo, position, points
- Toggle between pilots and teams

### Live
- Real-time position board (22 drivers) with gap displays
- Track map with driver position dots (GraphicsView + IDrawable)
- Telemetry per selected driver (speed, throttle, gear, DRS)
- Race control commentary stream
- 3-second auto-polling

### Driver Detail
- Profile: full name, acronym, team, headshot, points, championship position
- Filterable race history (all sessions: Practice, Qualifying, Sprint, Race)
- P1/P2/P3 badge colours for podium finishes

### Team Detail
- Team profile with logo, season stats, current drivers
- Race results with podium highlighting

### News
- Multi-source RSS aggregator (5 feeds)
- F1 keyword relevance filter
- Sky Sports F1, Motorsport Italia, FormulaPassion, FuoriTraiettoria, P300.it

### Settings
- Cache breakdown (database, images, total)
- One-tap cache clearing

## 3. Non-goals

- WEC, WRC, MotoGP support (F1 only)
- User accounts, social features, push notifications
- iOS support (Android-first, optional)
- Offline mode beyond SQLite cache-first fallback
