
# Piano di Progetto - Chicane

## 1. Sintesi operativa

### Obiettivo del progetto

Realizzare Chicane come app .NET MAUI Android-first per appassionati di motorsport, con Hero Section immersiva, countdown precisi, telemetria live, classifiche, notizie e gestione multi-categoria (F1, WEC, WRC, MotoGP). L'app mantiene una netta separazione tra View e ViewModel tramite CommunityToolkit.Mvvm e un'estetica "Carbon & Neon".

La prima iterazione prevede il bootstrap del progetto MAUI reale in `src/Chicane.Mobile/` e la struttura base delle sezioni principali.

### Vincoli principali

- `.NET MAUI` con target principale Android; iOS opzionale e secondario.
- Architettura MVVM con `CommunityToolkit.Mvvm`.
- Navigazione basata su Shell.
- `HttpClient` asincrono e `System.Text.Json`.
- Integrazione API: OpenF1 (F1), Ergast, OpenWeather, e mock per sviluppo offline.
- Temi dinamici: Cambio istantaneo dell'accent color in base alla categoria attiva.
- Stati UI espliciti: loading, error, empty, success.
- Persistenza: SQLite per classifiche, team e calendario.
- Nessuna autenticazione, sincronizzazione cloud o funzionalità social nel v1.

### Dipendenze esterne

- Provider API (OpenF1, Ergast, OpenWeather, ecc.).
- SkiaSharp per rendering mappa circuito (Live Track Widget).
- CommunityToolkit.Mvvm per messaggistica globale (CategoryProvider).
- Layer SQLite locale per classifiche e calendario.

## 2. Sequenza delle iterazioni


| Iterazione | Obiettivo verificabile | Dipendenze | Rischio | Stato |
| --- | --- | --- | --- | --- |
| IT-01 | Bootstrap MAUI, Shell e CategoryProvider | Nessuna | basso | completata |
| IT-02 | Hero Section Dinamica (F1) con countdown, meteo e immagine circuito | IT-01 | medio | completata |
| IT-03 | Multi-Series Switcher, tendina categorie e temi dinamici | IT-02 | medio | completata |
| IT-04 | Standings & Teams con persistenza SQLite | IT-03 | basso | pianificata |
| IT-05 | Live Track Widget (SkiaSharp) e News Feed | IT-02, IT-04 | alto | pianificata |
| IT-06 | Hardening MVP e gestione stati UI | IT-01, IT-02, IT-03, IT-04, IT-05 | medio | pianificata |

## 3. Dettaglio iterazioni


### IT-01 - Bootstrap progetto MAUI e Shell di base

**Obiettivo verificabile**

Creare il progetto MAUI in `src/Chicane.Mobile/`, con le quattro sezioni principali (Home/Hero, Standings, Teams, News) e il sistema di messaggistica per il cambio categoria motorsport.

**In scope**

- creare il progetto `.NET MAUI` in `src/Chicane.Mobile/`;
- impostare `App`, `AppShell` e `MauiProgram`;
- predisporre cartelle `Models`, `Services`, `ViewModels`, `Views`, `Resources`;
- creare le pagine iniziali `Home`, `Standings`, `Teams`, `News` come placeholder MVVM-safe;
- impostare dependency injection minima per servizi e ViewModels futuri.


**Out of scope**

- integrazione API motorsport;
- logica live, standings, news reali;


**File o aree probabili**

- `src/Chicane.Mobile/Chicane.Mobile.csproj`
- `src/Chicane.Mobile/App.xaml`
- `src/Chicane.Mobile/App.xaml.cs`
- `src/Chicane.Mobile/AppShell.xaml`
- `src/Chicane.Mobile/AppShell.xaml.cs`
- `src/Chicane.Mobile/MauiProgram.cs`
- `src/Chicane.Mobile/Views/`
- `src/Chicane.Mobile/ViewModels/`


**Dipendenze**

- `docs/spec.md` approvato;
- workload MAUI funzionante sull'ambiente locale.


**Criteri di accettazione**

- [ ] L'app compila e si avvia su Android.
- [ ] Le sezioni `Home`, `Standings`, `Teams` e `News` sono raggiungibili da Shell.
- [ ] Le pagine create usano bindings e ViewModels, senza business logic nei code-behind.
- [ ] La struttura delle cartelle del progetto è coerente con `docs/architecture.md`.


**Verifiche principali**

- manuale: avvio app e navigazione tra le quattro sezioni principali;
- automatico: `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj`.


**Rischi**

- introdurre logica di feature dentro il bootstrap invece di mantenere uno scheletro minimo;
- creare una struttura iniziale incoerente che obblighi refactor precoci nelle iterazioni successive.


### IT-02 - Hero Section Dinamica (F1)

**Obiettivo verificabile**

Implementare la Hero Section dinamica per la categoria F1: mostra countdown, nome evento, meteo previsto, immagine circuito, e in modalità live mostra nome sessione, stato, dati live e mappa circuito.

**In scope**

- visualizzazione Hero Section a schermo intero con immagine circuito;
- countdown preciso al prossimo evento F1;
- meteo previsto e nome evento;
- stato live con nome sessione, giri, posizioni, tempi, mappa circuito (mock dati);
- gestione esplicita di loading, error, empty, success.

**Out of scope**

- switch categoria motorsport;
- dati reali da API (mock o dati statici per UI).


**File o aree probabili**

- `src/Chicane.Mobile/Views/HomePage.xaml`
- `src/Chicane.Mobile/ViewModels/HomeViewModel.cs`
- `src/Chicane.Mobile/ViewModels/HeroSectionViewModel.cs`
- `src/Chicane.Mobile/Services/`
- `src/Chicane.Mobile/Models/`
- `src/Chicane.Mobile/Resources/`

**Dipendenze**

- completamento di IT-01;
- connettività Internet e disponibilità API motorsport/mock.

**Criteri di accettazione**

- [ ] La Hero Section mostra countdown, nome evento, meteo e immagine circuito.
- [ ] In modalità live, mostra nome sessione, stato, dati live e mappa circuito.
- [ ] Gestione esplicita di loading, error, empty, success.


**Verifiche principali**

- manuale: verifica Hero Section in modalità countdown e live;
- automatico: `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` e controlli unitari del mapping/service layer se il progetto test viene introdotto nello stesso slice.


**Rischi**

- dati mock o API motorsport non coerenti o incompleti;
- immagini remote lente o assenti che peggiorano la leggibilità della Hero Section.


### IT-03 - Multi-Series Switcher, tendina categorie e temi dinamici


**Obiettivo verificabile**

Permettere il cambio istantaneo tra categorie motorsport (F1, WEC, WRC, MotoGP) tramite tendina superiore, aggiornando Hero Section, accent color e dati mostrati.


**In scope**

- implementare tendina superiore per cambio categoria motorsport;
- aggiornare Hero Section in base alla categoria selezionata;
- sincronizzare CategoryProvider globale;
- mock dati per categorie diverse.


**Out of scope**

- salvataggio persistente di preferiti;
- supporto offline esteso.

**File o aree probabili**

- `src/Chicane.Mobile/AppShell.xaml`
- `src/Chicane.Mobile/Views/CategorySwitcher.xaml`
- `src/Chicane.Mobile/ViewModels/CategorySwitcherViewModel.cs`

**Dipendenze**

- completamento di IT-02;

**Criteri di accettazione**

- [ ] Il cambio categoria aggiorna Hero Section, accent color e dati mostrati.
- [ ] Il CategoryProvider sincronizza la categoria attiva in tutta l'app.

**Verifiche principali**

- manuale: cambio categoria e verifica aggiornamento Hero Section e tema;
- automatico: `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj`.


**Rischi**

- propagazione errata della categoria selezionata;
- differenze tra dati disponibili tra categorie che impattano il rendering.


### IT-04 - Standings & Teams con persistenza SQLite


**Obiettivo verificabile**

Visualizzare classifiche piloti e costruttori (standings) e dettagli team, con persistenza locale in SQLite per consultazione offline di base.


**In scope**

- visualizzazione standings piloti e costruttori per categoria selezionata;
- dettaglio team con stats e notizie correlate;
- persistenza locale SQLite per standings, team e calendario;
- gestione offline di base per standings e team.


**Out of scope**

- note personali;
- provider alternativi;
- modalità offline estesa oltre alla consultazione dei dati persistiti.

**File o aree probabili**

- `src/Chicane.Mobile/Views/StandingsPage.xaml`
- `src/Chicane.Mobile/ViewModels/StandingsViewModel.cs`
- `src/Chicane.Mobile/Views/TeamsPage.xaml`
- `src/Chicane.Mobile/ViewModels/TeamsViewModel.cs`

**Dipendenze**

- completamento di IT-03;
- disponibilità del layer SQLite locale.

**Criteri di accettazione**

- [ ] Standings e Teams sono consultabili anche offline (dati SQLite persistiti).
- [ ] I dati si aggiornano da remoto quando disponibili.
- [ ] Gestione errori e fallback su dati locali.

**Verifiche principali**

- manuale: consultazione standings e teams online/offline;
- automatico: `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` e test di persistenza dati.


**Rischi**

- schema locale troppo povero o troppo ampio rispetto ai dati necessari;
- divergenza temporanea tra snapshot locale e dati remoti aggiornati.


### IT-05 - Live Track Widget (SkiaSharp) e News Feed


**Obiettivo verificabile**

Visualizzare la mappa del circuito con posizione in tempo reale (mock o API se disponibili) e integrare feed notizie motorsport per la categoria selezionata.


**In scope**

- implementare Live Track Widget con SkiaSharp;
- visualizzare posizione in tempo reale (mock o API);
- integrare feed notizie motorsport per categoria;
- gestione loading, error, empty per news e live track.


**Out of scope**

- suggerimenti intelligenti o analytics;
- sincronizzazione cloud;
- ordinamenti avanzati o filtri;
- estensioni offline oltre al comportamento già previsto nel MVP.

**File o aree probabili**

- `src/Chicane.Mobile/Views/LiveTrackWidget.xaml`
- `src/Chicane.Mobile/ViewModels/LiveTrackWidgetViewModel.cs`
- `src/Chicane.Mobile/Views/NewsPage.xaml`
- `src/Chicane.Mobile/ViewModels/NewsViewModel.cs`

**Dipendenze**

- completamento di IT-02;
- pattern di persistenza locale consolidato in IT-04.

**Criteri di accettazione**

- [ ] Live Track Widget mostra la posizione in tempo reale sulla mappa circuito.
- [ ] News Feed mostra notizie aggiornate per la categoria selezionata.
- [ ] Gestione loading, error, empty per news e live track.

**Verifiche principali**

- manuale: verifica news e live track in condizioni di rete e offline;
- automatico: `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj`.


**Rischi**

- dati live non aggiornati o non sincronizzati con la mappa;
- feed news non aggiornato o non coerente con la categoria selezionata.


### IT-06 - Hardening MVP e gestione stati UI


**Obiettivo verificabile**

Stabilizzare il MVP, chiudere i principali gap trasversali di UI state, verificare la coerenza dei flussi e completare una baseline di verifiche manuali e automatiche prima del post-MVP.


**In scope**

- rifinire testi, placeholder, retry e feedback di errore sulle quattro schermate principali;
- verificare coerenza dei flussi tra Home, Standings, Teams e News;
- correggere eventuali difetti emersi dalle verifiche dell'MVP;
- eseguire smoke manuale Android e verifiche di build;
- aggiornare evidenze di test e log iterazione nella fase di esecuzione.


**Out of scope**

- filtri, ordinamento, note personali, offline mode estesa;
- provider alternativi;
- UI automation profonda se non richiesta esplicitamente.

**File o aree probabili**

- `src/Chicane.Mobile/Views/`
- `src/Chicane.Mobile/ViewModels/`
- `src/Chicane.Mobile/Services/`
- `docs/test-matrix.md`
- `docs/iterations/` per i log applicativi dell'MVP.

**Dipendenze**

- completamento funzionale di IT-01, IT-02, IT-03, IT-04 e IT-05.


**Criteri di accettazione**

- [ ] Tutti i flussi MVP risultano eseguibili senza blocchi funzionali maggiori.
- [ ] Tutte le sezioni MVP espongono stati UI coerenti con lo spec.
- [ ] La build Android del progetto riesce stabilmente.
- [ ] Le verifiche manuali prioritarie del MVP sono pronte per essere registrate nel test matrix esecutivo.


**Verifiche principali**

- manuale: smoke completo end-to-end su Android/emulatore;
- automatico: `dotnet build src/Chicane.Mobile/Chicane.Mobile.csproj` ed eventuale `dotnet test` se il progetto test è stato introdotto.


**Rischi**

- usare questa iterazione come contenitore di refactor larghi non pianificati;
- rinviare problemi di stato UI che dovrebbero essere chiusi nelle iterazioni precedenti.

## 4. Roadmap post-MVP

Le estensioni oltre il MVP devono mantenere l'ordine deciso in `docs/spec.md`:


1. Notifiche pre-gara;
2. Widget Home Screen (Android/iOS) con countdown categoria preferita;
3. Archivio storico risultati stagioni precedenti;
4. Profili piloti dettagliati con statistiche di carriera.

Queste fasi non devono essere assorbite dentro le iterazioni MVP sopra elencate.
