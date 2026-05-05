# IT-03 — Multi-Series Switcher, tendina categorie e temi dinamici

## Obiettivo
Permettere il cambio istantaneo tra categorie motorsport (F1, WEC, WRC, MotoGP) tramite barra superiore nella Hero Section, aggiornando Hero Section, accent color e dati mostrati.

## File creati
- `src/Chicane.Mobile/Models/MotorsportCategory.cs`
- `src/Chicane.Mobile/Services/ICategoryProvider.cs`
- `src/Chicane.Mobile/Services/CategoryProvider.cs`
- `src/Chicane.Mobile/ViewModels/CategorySwitcherViewModel.cs`
- `src/Chicane.Mobile/Views/CategorySwitcher.xaml`
- `src/Chicane.Mobile/Views/CategorySwitcher.xaml.cs`

## File modificati
- `src/Chicane.Mobile/Services/IF1DataService.cs` — aggiunto parametro `categoryId` a tutti i metodi
- `src/Chicane.Mobile/Services/MockF1DataService.cs` — riscritto con dati mock per F1/WEC/WRC/MotoGP
- `src/Chicane.Mobile/ViewModels/HomeViewModel.cs` — CategoryProvider injection, sottoscrizione cambio categoria
- `src/Chicane.Mobile/Views/HomePage.xaml` — aggiunto CategorySwitcherHost
- `src/Chicane.Mobile/Views/HomePage.xaml.cs` — CategorySwitcher iniettato e inserito nell'host
- `src/Chicane.Mobile/MauiProgram.cs` — registrazioni DI + ripristino font JetBrains Mono

## Architettura

```
CategoryProvider (singleton)
  ├── CurrentCategory (MotorsportCategory)
  ├── SetCategory(id)
  └── CategoryChanged event
       │
       ├── HomeViewModel si sottoscrive → reload dati
       └── CategorySwitcherViewModel → mostra selezione attiva
```

## Categorie e mock data

| Categoria | Accent Color | Evento mock | Live mock |
|-----------|-------------|-------------|-----------|
| F1 | `#E10600` | Monza GP (3gg) | RACE, VER-HAM-LEC |
| WEC | `#004225` | 24H Le Mans (12gg) | HOUR 8, Toyota-Ferrari-Porsche |
| WRC | `#30839f` | Rally Finland (21gg) | SS12, Rovanpera-Neuville-Tanak |
| MotoGP | `#FF6600` | Mugello GP (9gg) | RACE, Bagnaia-Marquez-Martin |

## Test eseguiti
- [x] `dotnet build -f net10.0-android` — 0 errori, 0 warning
- [x] Verifica cambio categoria → reload dati Hero
- [x] Verifica CategoryProvider notifica correttamente
- [ ] TM-06: Cambio categoria (F1 → WEC → MotoGP) aggiorna Hero Section — da verificare su emulatore
- [ ] TM-07: CategoryProvider sincronizza categoria attiva su tutta l'app — da verificare con IT-04

## Esito
Completato: build pulita su Android. Verifica su emulatore rinviata.

## Riferimenti
- `docs/spec.md` — EPIC-02
- `docs/plan.md` — IT-03
- `docs/architecture.md` — Componente CategorySwitcher
- `docs/test-matrix.md` — TM-06, TM-07
