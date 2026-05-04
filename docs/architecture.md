
# Architettura - Chicane

## 1. Obiettivo architetturale

Costruire una app `.NET MAUI` Android-first per motorsport, semplice da spiegare, estendere e testare, separando chiaramente:

- UI XAML e navigazione (Shell);
- logica di stato e orchestrazione nei ViewModels;
- integrazione remota con API motorsport (OpenF1, Ergast, OpenWeather, mock);
- persistenza locale di standings, teams e calendario (SQLite).

L'architettura deve supportare il MVP approvato (Hero Section, standings, teams, news, live track, multi-categoria) senza anticipare le funzionalità post-MVP, ma lasciando isolamento sufficiente per estensioni future.

## 2. Struttura del repository e del progetto


### Cartelle principali

- `docs/`: specifica, piano, architettura, matrice test, iterazioni.
- `src/`: progetto applicativo MAUI reale.
- `src/Chicane.Mobile/`: root del progetto MAUI.


### Responsabilità per area

- `src/Chicane.Mobile/Views/`: pagine XAML (Home/Hero, Standings, Teams, News, LiveTrackWidget, CategorySwitcher), layout, binding e componenti visuali senza business logic.
- `src/Chicane.Mobile/ViewModels/`: stato della UI, comandi, orchestrazione di servizi e navigazione.
- `src/Chicane.Mobile/Services/`: accesso API motorsport, persistenza locale, verifica connettività, nessuna logica di presentazione.
- `src/Chicane.Mobile/Models/`: modelli di dominio motorsport (evento, sessione, standings, team, news, meteo, ecc.) e DTO di integrazione; i DTO possono essere separati in sottocartelle se il volume cresce.
- `src/Chicane.Mobile/Resources/`: stili, placeholder, immagini statiche, temi.
- `src/Chicane.Mobile/Platforms/Android/`: configurazioni specifiche Android.


Poiché oggi `src/` è vuota, questa struttura è proposta ma coerente con il workflow del repository e con la futura implementazione del progetto.


## 3. Pattern applicativi

- `.NET MAUI` single-project come base applicativa.
- MVVM con `CommunityToolkit.Mvvm` per proprietà osservabili e comandi.
- Shell navigation per viste top-level (Home/Hero, Standings, Teams, News).
- XAML con compiled bindings dove possibile.
- `HttpClient` asincrono per provider API motorsport.
- `System.Text.Json` per parsing e mapping difensivo delle risposte.
- Nessuna business logic nei code-behind.


## 4. Componenti principali

### Views

- `HomePage`/`HeroSection`: Hero Section dinamica con countdown, meteo, immagine circuito, stato live, mappa circuito.
- `StandingsPage`: classifiche piloti e costruttori.
- `TeamsPage`: elenco e dettaglio team, stats e notizie correlate.
- `NewsPage`: feed notizie motorsport per categoria.
- `LiveTrackWidget`: mappa circuito con posizione in tempo reale.
- `CategorySwitcher`: tendina superiore per cambio categoria motorsport.

### ViewModels

- `HeroSectionViewModel`: gestisce countdown, stato evento/sessione, meteo, dati live, mappa circuito.
- `StandingsViewModel`: gestisce standings piloti/costruttori, loading, error, empty, success.
- `TeamsViewModel`: gestisce elenco e dettaglio team, stats, notizie.
- `NewsViewModel`: gestisce feed notizie motorsport per categoria.
- `LiveTrackWidgetViewModel`: gestisce dati live per la mappa circuito.
- `CategorySwitcherViewModel`: gestisce cambio categoria e sincronizzazione CategoryProvider.

Ogni ViewModel espone proprietà di stato esplicite come `IsBusy`, `ErrorMessage`, `HasData`, `IsEmpty` o equivalenti specifici per la schermata.


### Services

- servizi remoti API motorsport: recupero eventi, sessioni, standings, teams, news, meteo, live data (OpenF1, Ergast, OpenWeather, mock).
- servizio di mapping DTO -> modelli di dominio motorsport, con gestione centralizzata dei campi mancanti.
- servizio database locale basato su `sqlite-net-pcl`, responsabile di creare e condividere una singola `SQLiteAsyncConnection` per standings, teams e calendario.
- repository locale standings, teams, calendario sopra la connessione condivisa SQLite.
- utilizzo di `IConnectivity` nativo di .NET MAUI, iniettato via DI solo nei servizi che gestiscono fallback/aggiornamento dati.


### Models e DTO

- modelli di dominio motorsport: evento, sessione, standings, team, news, meteo, live data, ecc.
- DTO specifici delle API motorsport separati dai modelli usati dalla UI.
- mapping responsabile della conversione di `null`, campi assenti e placeholder visuali coerenti.


## 5. Navigazione

### Route Shell

- `Home` (Hero Section), `Standings`, `Teams`, `News` come sezioni principali Shell.
- `LiveTrackWidget` e `CategorySwitcher` come componenti accessibili da Home/Hero.

### Parametri di navigazione

- verso dettaglio team: `teamId` obbligatorio.
- verso dettaglio evento/sessione: `eventId` o `sessionId`.
- cambio categoria: parametro categoria motorsport (F1, WEC, WRC, MotoGP) propagato tramite CategoryProvider.


## 6. Stato della UI

### Loading

- Hero Section: loading esplicito durante fetch dati evento/sessione.
- Standings/Teams/News/LiveTrack: loading durante fetch o sync dati.

### Error

- errori remoti mostrati come stati pagina o messaggi con retry.
- fallback su dati locali (SQLite) se API non disponibili.
- nessun errore tecnico grezzo o stack trace in UI.

### Empty

- Nessun evento/sessione imminente.
- Nessuna classifica/team/news disponibile.

### Success

- Dati evento/sessione, standings, teams, news, live track caricati e leggibili anche se parziali.


## 7. Dati e integrazioni

### Chiamate remote

- provider base: OpenF1, Ergast, OpenWeather, mock API motorsport.
- endpoint previsti per il MVP:
  - eventi, sessioni, standings, teams, news, meteo, live data.

Il progetto deve avere un solo boundary di servizio verso le API motorsport, così da poter introdurre provider alternativi senza riscrivere i ViewModels.

### Parsing JSON

- uso di `System.Text.Json` con DTO dedicati.
- mapping difensivo centralizzato per evitare null-handling disperso nei ViewModels.
- trasformazione dei dati API in modelli adatti alla UI prima di arrivare alle Views.

### Persistenza locale

- SQLite come storage locale per standings, teams, calendario.
- wrapper previsto: `sqlite-net-pcl`, coerente con le preferenze tecniche del repository.
- un unico database locale con tabelle distinte e sufficiente per il MVP.
- una sola `SQLiteAsyncConnection` condivisa, inizializzata da un servizio database registrato come singleton in `MauiProgram`.
- standings, teams, calendario: memorizzano snapshot locale dei dati principali.
- delete singolo e clear totale esposti dal layer locale senza duplicare logica in UI.

Non è previsto un repository generico: il layer locale resta esplicito e limitato ai casi d'uso del MVP, così da mantenere il codice leggibile e didattico.

Per le immagini remote, nel MVP è preferibile affidarsi al comportamento standard di `Image` e alla cache HTTP di base della piattaforma, usando placeholder visuali senza introdurre librerie dedicate se non motivate da problemi reali.


## 8. Dependency injection e composition root

`MauiProgram.cs` è il punto di composizione del progetto. Qui dovranno essere registrati:

- servizi remoti API motorsport;
- servizio database SQLite condiviso;
- servizi o repository locali per standings, teams, calendario;
- `IConnectivity` nativo di .NET MAUI, senza wrapper custom aggiuntivo;
- logging standard MAUI, se necessario;
- ViewModels e pagine.

Linee guida iniziali:

- servizi remoti e locali con lifetime applicativo;
- ViewModels e Views con lifetime leggero, coerente con la navigazione MAUI;
- nessuna istanziazione manuale di servizi nelle pagine.


## 9. Error handling e logging

- Le eccezioni di rete, parsing e persistenza devono essere intercettate dal layer di servizio o dal ViewModel, non propagate direttamente alla UI.
- I ViewModels traducono i guasti in messaggi comprensibili e stati coerenti.
- I log diagnostici possono usare il logging standard disponibile in MAUI, senza introdurre telemetria o analytics nel MVP.
- Il fallback su dati locali (SQLite) non deve mai compromettere la disponibilità del contenuto già salvato.


## 10. Decisioni confermate

- SQLite resta la tecnologia di storage locale del MVP ed è previsto l'uso di `sqlite-net-pcl` come wrapper pratico sopra SQLite.
- Il progetto userà una sola `SQLiteAsyncConnection` condivisa, creata e inizializzata da un servizio database singleton registrato in `MauiProgram`.
- Sopra questa connessione vivranno repository dedicati e separati per standings, teams, calendario.
- Non verrà introdotto un repository generico o un ulteriore strato ORM: per questo progetto la soluzione più leggibile è avere un layer locale piccolo e specifico.
- Non verrà introdotto un wrapper custom della connettività nel MVP.
- Il controllo della rete per il refresh userà direttamente `IConnectivity` di .NET MAUI tramite dependency injection, confinato ai servizi che orchestrano il fallback/aggiornamento dati.

Questa scelta è considerata abbastanza testabile e poco dipendente dalla piattaforma per il perimetro del MVP; verrà rivalutata solo se, durante l'implementazione, emergerà un limite concreto.

Non risultano al momento ulteriori `TBD` architetturali bloccanti per l'avvio del MVP.
