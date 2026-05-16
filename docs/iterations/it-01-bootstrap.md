# IT-01 - Bootstrap progetto MAUI e Shell di base

## Obiettivo
Creare il progetto MAUI in `src/Chicane.Mobile/`, con le quattro sezioni principali (Home/Hero, Standings, Teams, News) e struttura MVVM minima pronta per le iterazioni successive.

## Piano
- creare il progetto .NET MAUI in `src/Chicane.Mobile/` con `dotnet new maui`;
- aggiungere `CommunityToolkit.Mvvm` come dipendenza NuGet;
- impostare `App`, `AppShell` con `TabBar` a 4 voci e `MauiProgram` con DI;
- predisporre cartelle `Models`, `Services`, `ViewModels`, `Views`;
- creare le pagine iniziali `HomePage`, `StandingsPage`, `TeamsPage`, `NewsPage` come placeholder MVVM-safe con `x:DataType`;
- creare i ViewModel con proprietà di stato `IsBusy`, `ErrorMessage`, `HasData`, `IsEmptyState` e `HasError`;
- applicare il design system "Carbon & Neon" dal progetto Stitch di riferimento (tema dark, palette `#0A0A0A`/`#E10600`, stili `Border`, tipografia);
- rimuovere i file template (`MainPage`, `dotnet_bot.png`) e creare placeholder SVG per icona e splash.

## File coinvolti nella specifica
- `docs/spec.md` (sezioni 1-2)
- `docs/plan.md` (IT-01)
- `docs/architecture.md` (sezioni 2, 3, 5, 6, 8)
- `docs/test-matrix.md` (TM-01, TM-02)

## File creati
- `src/Chicane.Mobile/Chicane.Mobile.csproj`
- `src/Chicane.Mobile/App.xaml` + `App.xaml.cs`
- `src/Chicane.Mobile/AppShell.xaml` + `AppShell.xaml.cs`
- `src/Chicane.Mobile/MauiProgram.cs`
- `src/Chicane.Mobile/Views/HomePage.xaml` + `HomePage.xaml.cs`
- `src/Chicane.Mobile/Views/StandingsPage.xaml` + `StandingsPage.xaml.cs`
- `src/Chicane.Mobile/Views/TeamsPage.xaml` + `TeamsPage.xaml.cs`
- `src/Chicane.Mobile/Views/NewsPage.xaml` + `NewsPage.xaml.cs`
- `src/Chicane.Mobile/ViewModels/HomeViewModel.cs`
- `src/Chicane.Mobile/ViewModels/StandingsViewModel.cs`
- `src/Chicane.Mobile/ViewModels/TeamsViewModel.cs`
- `src/Chicane.Mobile/ViewModels/NewsViewModel.cs`
- `src/Chicane.Mobile/Resources/Styles/Colors.xaml`
- `src/Chicane.Mobile/Resources/Styles/Styles.xaml`
- `src/Chicane.Mobile/Resources/AppIcon/appicon.svg`
- `src/Chicane.Mobile/Resources/AppIcon/appiconfg.svg`
- `src/Chicane.Mobile/Resources/Splash/splash.svg`

## File rimossi
- `src/Chicane.Mobile/MainPage.xaml` + `MainPage.xaml.cs`
- `src/Chicane.Mobile/Resources/Images/dotnet_bot.png`

## File modificati
- Nessuno (primo commit del progetto MAUI)

## Design system applicato (Stitch reference)
- Progetto Stitch: "Chicane Live Racing Dashboard" (ID: `15126559521206418777`)
- Tema: Dark mode, fondo `#0A0A0A`, accent `#E10600` (Racing Red)
- Tipografia: OpenSans Regular/Semibold (placeholder per Inter/Space Mono)
- Componenti: `Border` con `RoundRectangle CornerRadius="4"` (al posto di `Frame`, obsoleto in .NET 9)
- Shell: TabBar sfondo nero, titolo Racing Red attivo, grigio inattivo

## Test eseguiti
- [x] `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` — compila per Android, iOS, macOS, Windows
- [x] Verifica struttura cartelle coerente con `docs/architecture.md`
- [x] Verifica assenza business logic nei code-behind
- [x] Verifica ViewModel con proprietà di stato standard (`IsBusy`, `ErrorMessage`, `HasData`, `IsEmptyState`, `HasError`)
- [x] Verifica XAML con `x:DataType` per compiled bindings
- [x] Verifica DI registrata in `MauiProgram` per ViewModels e Pages
- [ ] Avvio app su emulatore Android (TM-01) — richiede ambiente Android configurato
- [ ] Navigazione tra le 4 sezioni — richiede avvio app

## Problemi trovati
- `Frame` è obsoleto in .NET 9+: sostituito con `Border` in XAML views e styles.
- File `splash.svg` e `appicon.svg` rimossi accidentalmente: ricreati come placeholder SVG.
- `git checkout -b feature develop` falliva perché `develop` non aveva commit: risolto con commit iniziale della documentazione.

## Correzioni effettuate
- Sostituito `Frame` → `Border` con `RoundRectangle` per `StrokeShape`.
- Ricreati placeholder SVG per AppIcon e Splash.
- Creato branch `develop` con commit iniziale, poi feature branch da lì.

## Esito
Completato con riserva: build pulita su tutti i target. Verifica su emulatore Android rinviata per mancanza di ambiente configurato in questa sessione.
