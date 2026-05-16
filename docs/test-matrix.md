# Test Matrix

## 1. Regole di lettura

- Questo documento è derivato dalla fase di planning: le evidenze sotto sono previste e non ancora eseguite.
- `Manuale`, `Automatico ora`, `Automatico più avanti`: usare `Si` o `No`.
- `Evidenza prevista`: comando, nota di verifica o `Da eseguire`.
- `Automatico ora` indica controlli realistici da introdurre durante l'iterazione corrente, senza richiedere una infrastruttura di UI automation avanzata.

## 2. Matrice principale

| ID | Requisito o scenario | Categoria | Manuale | Automatico ora | Automatico più avanti | Iterazione target | Evidenza prevista | Note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| TM-01 | Avvio app e navigazione tra Home, Standings, Teams, News da Shell | Navigation | Si | No | No | IT-01 | Da eseguire | Smoke iniziale di bootstrap MAUI |
| TM-02 | Build del progetto MAUI e validazione di XAML e route | Build | No | Si | No | IT-01 | `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` | Check minimo da ripetere a ogni iterazione |
| TM-03 | Hero Section pre‑gara (F1): countdown, meteo, immagine circuito, nome evento | Hero Section | Si | Si | No | IT-02 | `dotnet test` (mock) | Verifica UI e mapping dati statici/mock |
| TM-04 | Hero Section live (F1): nome sessione, stato, dati live, mappa circuito | Hero Section | Si | Si | No | IT-02 | `dotnet test` (mock) | Dati live simulati |
| TM-05 | Hero Section mostra loading, error, empty e success in modo esplicito | UI state | Si | Si | No | IT-02 | `dotnet test` | Copre tutti gli stati della Hero |
| TM-06 | Cambio categoria (F1 → WEC → MotoGP) aggiorna Hero Section e accent color | Category Switcher | Si | No | Si | IT-03 | Da eseguire | UI automation rimandata; verifica manuale immediata |
| TM-07 | CategoryProvider sincronizza la categoria attiva su tutta l’app | Global state | Si | Si | No | IT-03 | `dotnet test` | Test del provider con mock di messaggistica |
| TM-08 | Standings piloti e costruttori visualizzati per la categoria attiva | Standings | Si | Si | No | IT-04 | `dotnet test` | Verifica mapping DTO → modello UI |
| TM-09 | Standings salvati in SQLite e consultabili offline dopo primo fetch | Persistence | Si | Si | No | IT-04 | `dotnet test` | Test repository SQLite e verifica manuale offline |
| TM-10 | Fallback su dati locali quando le API remote non sono raggiungibili | Persistence / Error | Si | Si | No | IT-04 | `dotnet test` | Mock di errore rete, controllo che vengano mostrati i dati SQLite |
| TM-11 | Elenco team e dettaglio team con statistiche per la categoria attiva | Teams | Si | Si | No | IT-04 | `dotnet test` | Copre anche navigazione verso dettaglio con `teamId` |
| TM-12 | Live Track Widget: rendering mappa circuito con posizione in tempo reale (GraphicsView + IDrawable) | Live Track | Si | No | Si | IT-05 | Da eseguire | TrackMapDrawable con dots da `/v1/location`, polling 3s |
| TM-13 | News Feed: elenco notizie motorsport, aggiornato da API (Rss2Json) | News | Si | Si | No | IT-05 | `dotnet test` (mock) | Test del mapping e della gestione stati |
| TM-14 | Gestione loading, error, empty, success su News e Live Track | UI state | Si | Si | No | IT-05 | `dotnet test` | ✅ IT-05 completato |
| TM-15 | Consultazione offline: standings e team già salvati restano visibili senza rete | Device / Persistence | Si | No | Si | IT-06 | Da eseguire | Verifica end‑to‑end della modalità offline di base |
| TM-16 | Smoke end‑to‑end: navigazione Home→Schedule→Leaderboards→News→Team→Driver→Live→Settings | Device | Si | No | Si | IT-06 | Da eseguire | Verifica manuale finale su Android |
| TM-17 | Coerenza stati UI (loading/error/empty/data) su tutte le sezioni | UI state | Si | No | No | IT-06 | ✅ Completato | HomePage, Schedule, Leaderboards, News, TeamDetail, DriverDetail, PastRace, TeamGPSessions, Live, Settings |
| TM-18 | Header CHICANE sottile e uniforme su tutti i 4 tab + icona ⚙️ | Device | Si | No | No | IT-06 | ✅ Completato | Padding `20,6,20,4`, gear FontSize=20 |
| TM-19 | Build stabile del progetto dopo hardening | Build | No | Si | No | IT-06 | `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` | ✅ 0 errori |
| TM-20 | LIVE guard: pill countdown non naviga se non in modalità live | Navigation | Si | No | No | IT-06 | ✅ Completato | `if (!IsLive) return;` in NavigateToLiveAsync |
| TM-21 | Meteo OpenWeather entro 7 giorni con DateTimeKind UTC | Hero Section | Si | No | No | IT-06 | ✅ Completato | UTC normalize + refresh on resume |
| TM-22 | SettingsPage: cache breakdown (DB/Images/Total) + SVUOTA CACHE cancella tutto | Persistence | Si | No | No | IT-06 | ✅ Completato | ClearAllCacheAsync cancella chicane.db* + img_cache/ |
| TM-23 | Env: API keys in `env` (gitignored), caricato da OpenAppPackageFileAsync | Security | Si | No | No | IT-06 | ✅ Completato | Env.cs con diagnostic logging `[Env] OK — 4 keys loaded` |

## 3. Aree minime da coprire

- Hero Section: stati pre‑gara, live, loading, error, empty, success; visualizzazione countdown, meteo, mappa.
- Category Switcher: cambio categoria, propagazione accent color, aggiornamento dati.
- Standings & Teams: visualizzazione, persistenza SQLite, fallback offline, navigazione verso dettaglio.
- Live Track & News: rendering mappa (SkiaSharp), feed notizie, gestione stati.
- Navigation: Shell con Home, Standings, Teams, News; parametri di navigazione (`teamId`, categoria).
- Persistence: salvataggio e recupero standings/teams/calendario in SQLite; consultazione offline di base.
- Device: smoke Android, assenza rete su sezioni con persistenza, tema e rotazione.

## 4. Note su test automatici

Controlli automatici realistici da introdurre presto:

- `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` come smoke check minimo;
- test unitari su ViewModels per gestione stati (HeroSection, Standings, Teams, News, LiveTrack);
- test del service layer e del mapping JSON con risposte simulate delle API motorsport;
- test del layer SQLite per standings, teams e calendario (creazione, lettura, fallback).

Controlli automatici da rimandare a più avanti:

- UI automation end‑to‑end sulla navigazione, sulla tendina categorie e sulla mappa circuito;
- verifiche device‑specific più approfondite;
- test automatici per notifiche push, widget home screen e altre estensioni post‑MVP.

Per una strategia automatica più profonda è opportuno passare successivamente alla skill `maui-automatic-testing`.
