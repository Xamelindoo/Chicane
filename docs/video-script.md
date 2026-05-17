# Video Presentation Script — Chicane

**Duration**: 2–3 minutes  
**Tools**: OBS Studio (free screen recorder) + your voice, or use an AI video generator

---

## Slide 1: Title (0:00–0:08)

**[Show app logo/splash]**

> Chicane — your Formula 1 companion. Open source, no ads, no account required.

---

## Slide 2: The Problem (0:08–0:30)

**[Show side-by-side: official F1 app screenshot (bloated) vs Chicane screenshot]**

> The official Formula 1 app is over 400 megabytes, full of video ads, and forces you to create an account just to see the race schedule.
>
> Chicane is different. It's open source, lightweight, and does one thing well: deliver F1 data fast.

---

## Slide 3: Architecture Overview (0:30–0:50)

**[Show architecture diagram from README]**

> Under the hood, Chicane is a .NET MAUI Android app connected to an ASP.NET Core backend. The backend acts as a transparent proxy to the OpenF1 API — forwarding requests without modifying the data.
>
> All API keys live on the server, never on the client. Redis cache on the backend means data loads instantly, even if multiple devices are connected.

---

## Slide 4: Home Screen (0:50–1:10)

**[Screen recording: open the app, show Home screen]**

> When you open Chicane, you see the next F1 session with a live countdown. Scroll horizontally for upcoming GP cards with circuit images and date ranges.
>
> Below, the top 3 drivers and constructors. Everything is cached locally — open it anywhere, even at the circuit with bad WiFi.

---

## Slide 5: Schedule & Calendar (1:10–1:30)

**[Show Schedule tab, toggle upcoming/past, tap a GP]**

> The Schedule tab shows the full season. Toggle between upcoming and past races. Tap any GP to see session times — Practice, Qualifying, Sprint, Race.
>
> Past races show the podium top 3. All data is cache-first: fetch once, read forever.

---

## Slide 6: Live Timing (1:30–1:50)

**[Show Live tab with positions, track map]**

> During an active session, Chicane shows real-time driver positions with gap displays. The track map renders driver locations updated every 3 seconds.
>
> Tap a driver for telemetry — speed, throttle, gear, DRS. Race control messages scroll at the bottom.

---

## Slide 7: Driver & Team Detail (1:50–2:10)

**[Show driver detail → race history filter → team detail]**

> Tap any driver for their full profile with championship points and a filterable race history. Podium positions get gold, silver, and bronze badges.
>
> Team detail shows constructor stats, season performance, and race results at a glance.

---

## Slide 8: News & Settings (2:10–2:25)

**[Show News tab, pull-to-refresh, then Settings]**

> The News tab aggregates articles from five motorsport sources — Sky Sports F1, Motorsport Italia, FormulaPassion, and more — filtered for F1 relevance.
>
> Settings shows your cache breakdown. One tap clears everything.

---

## Slide 9: Tech Stack (2:25–2:40)

**[Show stack diagram / bullet list]**

> Built with .NET MAUI for Android, ASP.NET Core 10 on the backend, Redis for caching, Docker for deployment. All running on Proxmox containers with NGINX and Let's Encrypt SSL.
>
> The code is open source under the MIT license. Self-hostable in two Docker commands.

---

## Slide 10: Closing (2:40–3:00)

**[Show GitHub repo / QR code]**

> Chicane proves you don't need a bloated, ad-filled app to follow Formula 1. Clean code, smart caching, zero tracking.
>
> Check out the repo at github.com/Xamelindoo/Chicane. MIT licensed, contributions welcome.
>
> Thank you.

---

## How to record

### Option A: OBS Studio (recommended)
1. Download [OBS Studio](https://obsproject.com/) (free)
2. Set up a scene with your Android screen (via scrcpy or USB mirroring)
3. Record your screen while reading the script
4. Record your voice separately or live

### Option B: AI Video Generator
1. Take screenshots of each screen in the app
2. Use [Synthesia](https://synthesia.io) or [HeyGen](https://heygen.com) — upload images + script, AI generates narrated video
3. More polished but need account / credits

### Option C: Canva
1. Create a presentation with Canva
2. Use the "Present and Record" feature to record voiceover per slide
3. Export as MP4

---

## Useful clips to record

| Timestamp | What to record |
|-----------|---------------|
| 0:00 | App icon / splash |
| 0:50 | Home screen — scroll through countdown, GP cards, standings |
| 1:10 | Schedule tab — upcoming → past toggle |
| 1:30 | Live tab — positions, track map, tap driver for telemetry |
| 1:50 | Driver detail — profile, race history filter |
| 2:10 | News tab — scroll, pull-to-refresh |
| 2:25 | Settings screen |
| 2:40 | GitHub repo page |
