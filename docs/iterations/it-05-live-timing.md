# IT-05 — Live Timing & Track Map

**Date**: May 2026 · **Branch**: `it-05_live_data_deepseek`  
**Status**: Completed

## Goal

Add a Live page showing real-time driver positions, telemetry, commentary, and a circuit track map during active F1 sessions.

## What was built

- **LivePage** with Shell navigation via `sessionKey` query parameter
- Real-time **position board** (22 drivers) with gap/sector displays
- **Telemetry panel** per driver (speed, throttle, braking, gear, RPM, DRS)
- **Commentary bar** (race control messages, chronologic)
- **Track map** using `GraphicsView` + `TrackMapDrawable` (custom `IDrawable`)
- **3-second polling** via `IDispatcherTimer` with start/stop lifecycle
- **LIVE guard** on HomePage — "SEGUI LIVE" button only appears during active sessions
- Arrow ▲/▼ toggle for position changes

## Key decisions

- Polling frequency: 3s (balance between freshness and server load)
- Track map: client-side rendering with driver dots positioned by (X,Y) coordinates
- Live detection: session `DateStart <= UtcNow <= DateEnd`

## Files

- `src/Chicane.Mobile/Views/LivePage.xaml`
- `src/Chicane.Mobile/ViewModels/LiveViewModel.cs`
- `src/Chicane.Mobile/Models/TrackMapDrawable.cs`
- `src/Chicane.Mobile/Models/LiveDriverRow.cs`
- `src/Chicane.Mobile/Models/LiveSessionData.cs`
- `src/Chicane.Mobile/Models/DriverGap.cs`
