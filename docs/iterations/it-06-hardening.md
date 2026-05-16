# IT-06 — Hardening MVP e gestione stati UI

**Branch**: `it_06_hardening_ui_deepseek`  
**Data**: maggio 2026  
**Stato**: in corso

---

## 1. Fix applicati

### 1.1 LIVE guard — pill countdown non naviga più se non in live

**File**: `ViewModels/HomeViewModel.cs:375`

```csharp
if (!IsLive) return;
if (SessionKey <= 0) return;
```

Prima il `TapGestureRecognizer` sul Border esterno navigava sempre. Ora la navigazione è bloccata se `IsLive == false`.

---

### 1.2 Meteo — DateTimeKind UTC + refresh on resume

**File**: `Services/OpenWeatherService.cs:22`

```csharp
var utcDate = eventDate.Kind == DateTimeKind.Utc ? eventDate : eventDate.ToUniversalTime();
var daysUntil = (utcDate - DateTime.UtcNow).TotalDays;
if (daysUntil > 7) return null;
```

I `DateTime` da SQLite/OpenF1 sono `Unspecified` — normalizzandoli a UTC il calcolo `daysUntil` non è più sfalsato.

**File**: `Views/HomePage.xaml.cs:22`

```csharp
else if (viewModel.HasData)
    _ = viewModel.RefreshWeatherIfStaleAsync();
```

`OnAppearing` ora ri-chiama il meteo se era rimasto "Check later".

**File**: `ViewModels/HomeViewModel.cs:451`

```csharp
public async Task RefreshWeatherIfStaleAsync()
{
    if (WeatherCondition != "Check later") return;
    // ...
}
```

---

### 1.3 Countdown — cache meeting invalidation per anno

**File**: `Services/OpenF1ApiService.cs:14,260`

```csharp
private int? _meetingsCacheYear;

private async Task<List<OpenF1Meeting>> GetMeetingsAsync(int year)
{
    if (_meetingsCache is not null && _meetingsCacheYear == year)
        return _meetingsCache;
    // ...
    _meetingsCacheYear = year;
}
```

Prima la cache meeting ignorava l'anno — poteva restituire meeting 2025 per query 2026.

---

### 1.4 Header CHICANE — più sottile + gear icon su tutti i tab

**File**: `HomePage.xaml`, `SchedulePage.xaml`, `LeaderboardsPage.xaml`, `NewsPage.xaml`

```xml
Padding="20,6,20,4"  <!-- prima era 20,12,20,8 -->
```

Gear `FontSize="20"` (allineato a CHICANE) aggiunto su tutti e 4 i tab principali.

**File**: `ScheduleViewModel.cs`, `LeaderboardsViewModel.cs`, `NewsViewModel.cs`

`OpenSettingsCommand` → `Shell.GoToAsync("settings")` aggiunto.

---

### 1.5 Stati UI — Loading/Error/Empty completati

**HomePage.xaml** — aggiunti blocchi Loading, Error, Empty prima del RefreshView:

```xml
<VerticalStackLayout Grid.Row="1" IsVisible="{Binding IsBusy}" ... />
<VerticalStackLayout Grid.Row="1" IsVisible="{Binding HasError}" ... />
<VerticalStackLayout Grid.Row="1" IsVisible="{Binding IsEmptyState}" ... />
<RefreshView Grid.Row="1" IsVisible="{Binding HasData}" ... />
```

**TeamDetailPage.xaml** + **TeamDetailViewModel.cs** — aggiunto Empty state:

```csharp
IsEmptyState = Drivers.Count == 0 && RaceGroups.Count == 0;
```

---

### 1.6 Cache Settings — breakdown DB/Images + clear totale

**File**: `Services/CacheService.cs`, `Services/ICacheService.cs`

```csharp
(double TotalMB, double DbMB, double ImgMB) GetCacheInfo();
Task ClearAllCacheAsync();
```

Il breakdown mostra:
- `Database: X MB` (chicane.db + .db-wal + .db-shm + .db-journal)
- `Images: Y MB` (img_cache/)
- `Total: X+Y MB`

"SVUOTA CACHE" cancella tutto (DB + immagini). Al prossimo avvio il DB si ricrea vuoto.

---

### 1.7 Env — rename `.env` → `env` + diagnostic logging

**File**: `Services/Env.cs`, `Resources/Raw/env`

```csharp
FileSystem.Current.OpenAppPackageFileAsync("env")
```

Il file `.env` con dot poteva dare problemi su Android. Rinominato. Aggiunto log diagnostico `[Env] OK — 4 keys loaded` / `[Env] FAILED: ...`.

**File**: `.gitignore`

```
**/.env
!**/.env.example
**/Resources/Raw/.env
**/Resources/Raw/env
!**/Resources/Raw/env.example
```

---

## 2. Riepilogo file

### Modificati

| File | Cosa |
|------|------|
| `.gitignore` | Aggiunte regole `env` |
| `Views/HomePage.xaml` | Header thinner + gear + stati Loading/Error/Empty/Data |
| `Views/HomePage.xaml.cs` | Weather refresh on resume |
| `Views/SchedulePage.xaml` | Header thinner + gear icon |
| `Views/LeaderboardsPage.xaml` | Header thinner + gear icon |
| `Views/NewsPage.xaml` | Header thinner + gear icon |
| `Views/TeamDetailPage.xaml` | Empty state |
| `Views/SettingsPage.xaml` | Cache breakdown (DB/Images/Total) |
| `ViewModels/HomeViewModel.cs` | LIVE guard + weather refresh + StopCountdown public |
| `ViewModels/ScheduleViewModel.cs` | OpenSettingsCommand |
| `ViewModels/LeaderboardsViewModel.cs` | OpenSettingsCommand |
| `ViewModels/NewsViewModel.cs` | OpenSettingsCommand |
| `ViewModels/TeamDetailViewModel.cs` | IsEmptyState assignment |
| `ViewModels/SettingsViewModel.cs` | DbSizeMB / ImgSizeMB / ClearAllCache |
| `Services/OpenF1ApiService.cs` | MeetingsCacheYear invalidation |
| `Services/OpenWeatherService.cs` | DateTimeKind UTC normalize |
| `Services/CacheService.cs` | ClearAllCacheAsync + breakdown |
| `Services/ICacheService.cs` | Nuova interfaccia GetCacheInfo / ClearAllCache |
| `Services/Env.cs` | Read "env" + diagnostic logging |
| `Resources/Raw/env.example` | Template aggiornato |

### Rinominati

| Da | A |
|----|----|
| `Resources/Raw/.env` | `Resources/Raw/env` |
| `Resources/Raw/.env.example` | `Resources/Raw/env.example` |

---

## 3. Test manuali suggeriti

### 3.1 Countdown + meteo

- [ ] Avviare app — il countdown mostra l'evento reale (non "NEXT RACE")
- [ ] Se la gara è entro 7 giorni, il meteo mostra previsione con icona e temperatura
- [ ] Navigare via e tornare su Home — il meteo si aggiorna se era "Check later"

### 3.2 LIVE guard

- [ ] In modalità pre-race, tappare la pill countdown → NON deve navigare
- [ ] In modalità live (quando disponibile), tappare "SEGUI LIVE" → naviga a LivePage

### 3.3 Header

- [ ] Verificare su tutti e 4 i tab: header è sottile e uniforme (stessa altezza)
- [ ] L'icona ⚙️ è presente e allineata in ogni tab
- [ ] Tappare ⚙️ su ogni tab → apre SettingsPage

### 3.4 Stati UI

- [ ] Home: con rete spenta, compare errore + RETRY
- [ ] Home: al primo load, spinner "🏎️ Loading..." visibile (se lento)
- [ ] TeamDetail: team senza dati → messaggio "No team data available"

### 3.5 Cache

- [ ] Settings: mostra DB + Images + Total separati
- [ ] "SVUOTA CACHE" → tutto va a 0 MB
- [ ] Dopo clear, tornare su Home → i dati si ricaricano dalle API

### 3.6 Security

- [ ] `env` non è tracciato da git (`git status` non lo mostra)
- [ ] `env.example` è tracciato con placeholder
- [ ] Build log mostra `[Env] OK — 4 keys loaded`
