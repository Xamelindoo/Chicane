# IT-02 - Hero Section Dinamica (F1)

## Obiettivo
Implementare la Hero Section dinamica per la categoria F1: countdown, nome evento, meteo, immagine circuito in modalità pre-gara e dati live simulati in modalità gara.

## Piano
- creare i modelli: `CircuitInfo`, `WeatherInfo`, `RaceEvent`, `LiveSessionData`, `DriverGap`;
- creare l'interfaccia `IF1DataService` e il servizio mock `MockF1DataService` con dati realistici (Monza GP);
- riscrivere `HomeViewModel` con logica Hero Section: countdown timer, toggle pre-gara/live, stati UI;
- riscrivere `HomePage.xaml` con layout full-screen Hero Section (immagine circuito, countdown, meteo, live data);
- scaricare e registrare il font Space Mono (Regular + Bold) per il countdown e i dati tecnici;
- aggiornare `MauiProgram.cs` con le nuove registrazioni DI e i font.

## File creati
- `src/Chicane.Mobile/Models/CircuitInfo.cs`
- `src/Chicane.Mobile/Models/WeatherInfo.cs`
- `src/Chicane.Mobile/Models/RaceEvent.cs`
- `src/Chicane.Mobile/Models/LiveSessionData.cs`
- `src/Chicane.Mobile/Models/DriverGap.cs`
- `src/Chicane.Mobile/Services/IF1DataService.cs`
- `src/Chicane.Mobile/Services/MockF1DataService.cs`
- `src/Chicane.Mobile/Resources/Fonts/SpaceMono-Regular.ttf`
- `src/Chicane.Mobile/Resources/Fonts/SpaceMono-Bold.ttf`
- `src/Chicane.Mobile/Resources/Images/circuit_placeholder.svg`
- `docs/iterations/it-02-hero-section.md`

## File modificati
- `src/Chicane.Mobile/ViewModels/HomeViewModel.cs` — riscritto con logica Hero Section
- `src/Chicane.Mobile/Views/HomePage.xaml` — riscritto con layout full-screen Hero
- `src/Chicane.Mobile/Views/HomePage.xaml.cs` — aggiunto trigger `LoadCommand` in `OnAppearing`
- `src/Chicane.Mobile/MauiProgram.cs` — registrati font Space Mono, `IF1DataService` → `MockF1DataService`

## Design system applicato (Stitch reference)
- Progetto Stitch: "Chicane Live Racing Dashboard" (ID: `15126559521206418777`), screen "Home (Pre-Race)"
- Countdown: font Space Mono Bold 44pt, formato `DD:HH:MM:SS`
- Weather badge: bordo sottile, sfondo SurfaceLow, icona Unicode + condizione + temperatura
- LIVE badge: sfondo Racing Red, testo bianco uppercase Space Mono
- Position board: CollectionView con posizione rossa, nome pilota, scuderia, gap badge
- Simulate toggle: bordo bianco, sfondo semi-trasparente, effetto "split button"

## Dati mock
- **Evento**: Monza GP, 3 giorni da adesso
- **Meteo**: Rain Expected, 18°C
- **Live**: RACE session, Lap 32/53, leaderboard top 3 (Verstappen, Hamilton, Leclerc)

## Test eseguiti
- [x] `dotnet build -f net10.0-android` — 0 errori, 0 warning
- [x] `dotnet build` (multi-target) — compila su tutti i target
- [x] Verifica MVVM: logica nel ViewModel, code-behind minimo (solo trigger OnAppearing)
- [x] Verifica compiled bindings: `x:DataType` su pagina e `DataTemplate`
- [x] Verifica stati UI: `IsBusy`, `HasError`, `HasData`, `IsPreRace`, `IsLive`
- [x] Verifica font Space Mono registrati e usati per countdown e dati tecnici
- [ ] Avvio app su emulatore Android — Hero Section pre-gara visibile con countdown
- [ ] Toggle "SIMULATE LIVE" — passaggio a modalità live e ritorno pre-gara

## Problemi trovati
- `CharacterSpacing="-0.04em"` non valido in XAML MAUI (CSS value): sostituito con `CharacterSpacing="-1"`
- `ObservableCollection<>` mancava `using System.Collections.ObjectModel` nel ViewModel

## Correzioni effettuate
- CharacterSpacing convertito da em a double
- Aggiunto using per ObservableCollection

## Esito
Completato: build pulita su tutti i target. Verifica su emulatore Android rinviata.
