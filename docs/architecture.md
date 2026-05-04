<<<<<<< HEAD
# Architettura

## 1. Obiettivo architetturale

Costruire una app `.NET MAUI` Android-first che resti semplice da spiegare, estendere e testare, separando chiaramente:

- UI XAML e navigazione;
- logica di stato nei ViewModels;
- integrazione remota con Google Books;
- persistenza locale di favoriti e cronologia.

L'architettura deve supportare il MVP approvato senza anticipare le funzionalità post-MVP, ma lasciando abbastanza isolamento per introdurle in seguito senza rifattorizzazioni invasive.

## 2. Struttura del repository e del progetto

### Cartelle principali

- `docs/`: specifica, piano, architettura, matrice test, iterazioni.
- `src/`: conterrà il progetto applicativo MAUI reale.
- `src/BookScout.Mobile/`: root proposta del progetto MAUI da creare in IT-01.

### Responsabilità per area

- `src/BookScout.Mobile/Views/`: pagine XAML, layout, binding e componenti visuali senza business logic.
- `src/BookScout.Mobile/ViewModels/`: stato della UI, comandi, orchestrazione di servizi e navigazione.
- `src/BookScout.Mobile/Services/`: accesso API, persistenza locale, eventuale verifica connettività, nessuna logica di presentazione.
- `src/BookScout.Mobile/Models/`: modelli di dominio e DTO di integrazione; i DTO possono essere separati in sottocartelle se il volume cresce.
- `src/BookScout.Mobile/Resources/`: stili, placeholder, immagini statiche, temi.
- `src/BookScout.Mobile/Platforms/Android/`: configurazioni specifiche Android.

Poiché oggi `src/` è vuota, questa struttura è proposta ma coerente con il workflow del repository e con la futura implementazione del progetto.

=======

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


>>>>>>> it_01_bootstrap_project_deepseek
## 3. Pattern applicativi

- `.NET MAUI` single-project come base applicativa.
- MVVM con `CommunityToolkit.Mvvm` per proprietà osservabili e comandi.
<<<<<<< HEAD
- Shell navigation per rotte top-level e dettaglio.
- XAML con compiled bindings dove possibile.
- `HttpClient` asincrono per il provider remoto.
- `System.Text.Json` per parsing e mapping difensivo delle risposte.
- Nessuna business logic nei code-behind.

=======
- Shell navigation per viste top-level (Home/Hero, Standings, Teams, News).
- XAML con compiled bindings dove possibile.
- `HttpClient` asincrono per provider API motorsport.
- `System.Text.Json` per parsing e mapping difensivo delle risposte.
- Nessuna business logic nei code-behind.


>>>>>>> it_01_bootstrap_project_deepseek
## 4. Componenti principali

### Views

<<<<<<< HEAD
- `SearchPage`: ricerca e risultati nella stessa pagina con `SearchBar` e `CollectionView`.
- `BookDetailPage`: dettaglio esteso del libro e azione sui favoriti.
- `FavoritesPage`: elenco dei libri salvati localmente.
- `HistoryPage`: elenco delle query recenti con replay e pulizia.

### ViewModels

- `SearchViewModel`: gestisce query testuale, risultati, retry e stati `loading/error/empty/success`.
- `BookDetailViewModel`: carica il dettaglio, gestisce add/remove favoriti e il refresh remoto in background quando il libro viene aperto dai favoriti.
- `FavoritesViewModel`: carica la lista locale dei favoriti, rimuove elementi e apre il dettaglio da sorgente locale.
- `HistoryViewModel`: legge la cronologia, esegue delete singolo o totale e richiede il replay della query verso `Search`.

Ogni ViewModel dovrà esporre proprietà di stato esplicite come `IsBusy`, `ErrorMessage`, `HasData`, `IsEmpty` o equivalenti più specifici per la schermata.

### Services

- servizio remoto catalogo libri: ricerca per query e dettaglio per `bookId` tramite Google Books API;
- servizio di mapping DTO -> modelli di dominio, con gestione centralizzata dei campi mancanti;
- servizio database locale basato su `sqlite-net-pcl`, responsabile di creare e condividere una singola `SQLiteAsyncConnection` per il progetto;
- repository locale favoriti sopra la connessione condivisa SQLite, con snapshot completa dei dati principali del dettaglio;
- repository locale cronologia sopra la stessa connessione SQLite, con deduplica e ordinamento per recenza;
- utilizzo di `IConnectivity` nativo di .NET MAUI, iniettato via DI solo nel servizio che decide se tentare il refresh remoto in background dai favoriti.

### Models e DTO

- modelli di dominio per lista risultati, dettaglio libro, snapshot favorito e voce cronologia;
- DTO specifici di Google Books separati dai modelli usati dalla UI;
- mapping responsabile della conversione di `null`, campi assenti e placeholder visuali coerenti.

=======
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


>>>>>>> it_01_bootstrap_project_deepseek
## 5. Navigazione

### Route Shell

<<<<<<< HEAD
- `Search`, `Favorites` e `History` come sezioni principali Shell;
- `BookDetailPage` come route dedicata fuori dal livello root, aperta da Search e Favorites.

### Parametri di navigazione

- verso il dettaglio: `bookId` obbligatorio;
- verso il dettaglio da Favorites: `bookId` e `source=favorite`, così il ViewModel può caricare prima la snapshot locale e poi tentare refresh remoto;
- verso Search da History: `queryText` o parametro equivalente per rieseguire la ricerca nella pagina unificata.

Il dettaglio aperto da Search carica direttamente il contenuto remoto. Il dettaglio aperto da Favorites mostra prima il contenuto locale disponibile e, se la rete è accessibile, prova ad aggiornare silenziosamente i dati in background.
=======
- `Home` (Hero Section), `Standings`, `Teams`, `News` come sezioni principali Shell.
- `LiveTrackWidget` e `CategorySwitcher` come componenti accessibili da Home/Hero.

### Parametri di navigazione

- verso dettaglio team: `teamId` obbligatorio.
- verso dettaglio evento/sessione: `eventId` o `sessionId`.
- cambio categoria: parametro categoria motorsport (F1, WEC, WRC, MotoGP) propagato tramite CategoryProvider.

>>>>>>> it_01_bootstrap_project_deepseek

## 6. Stato della UI

### Loading

<<<<<<< HEAD
- `SearchPage`: loading esplicito durante una richiesta remota;
- `BookDetailPage`: loading bloccante all'apertura da Search, non bloccante per refresh da Favorites;
- `FavoritesPage` e `HistoryPage`: loading leggero durante lettura iniziale dei dati locali.

### Error

- errori remoti di Search e Detail mostrati come stati pagina o messaggi con retry;
- fallimento del refresh remoto da Favorites mostrato con feedback visibile ma non invasivo;
=======
- Hero Section: loading esplicito durante fetch dati evento/sessione.
- Standings/Teams/News/LiveTrack: loading durante fetch o sync dati.

### Error

- errori remoti mostrati come stati pagina o messaggi con retry.
- fallback su dati locali (SQLite) se API non disponibili.
>>>>>>> it_01_bootstrap_project_deepseek
- nessun errore tecnico grezzo o stack trace in UI.

### Empty

<<<<<<< HEAD
- Search senza risultati remoti;
- Favorites senza libri salvati;
- History senza ricerche registrate.

### Success

- risultati presenti su Search;
- dettaglio caricato e leggibile anche con campi parziali;
- elenco favoriti o cronologia disponibile;
- refresh riuscito da Favorites gestito in modo silenzioso, senza messaggio di successo obbligatorio.
=======
- Nessun evento/sessione imminente.
- Nessuna classifica/team/news disponibile.

### Success

- Dati evento/sessione, standings, teams, news, live track caricati e leggibili anche se parziali.

>>>>>>> it_01_bootstrap_project_deepseek

## 7. Dati e integrazioni

### Chiamate remote

<<<<<<< HEAD
- provider base: Google Books API;
- endpoint previsti per il MVP:
  - `GET /volumes?q={query}` per ricerca;
  - `GET /volumes/{id}` per dettaglio.

Il progetto deve avere un solo boundary di servizio verso il catalogo libri, così da poter introdurre in futuro Open Library senza riscrivere i ViewModels.

### Parsing JSON

- uso di `System.Text.Json` con DTO dedicati;
- mapping difensivo centralizzato per evitare null-handling disperso nei ViewModels;
=======
- provider base: OpenF1, Ergast, OpenWeather, mock API motorsport.
- endpoint previsti per il MVP:
  - eventi, sessioni, standings, teams, news, meteo, live data.

Il progetto deve avere un solo boundary di servizio verso le API motorsport, così da poter introdurre provider alternativi senza riscrivere i ViewModels.

### Parsing JSON

- uso di `System.Text.Json` con DTO dedicati.
- mapping difensivo centralizzato per evitare null-handling disperso nei ViewModels.
>>>>>>> it_01_bootstrap_project_deepseek
- trasformazione dei dati API in modelli adatti alla UI prima di arrivare alle Views.

### Persistenza locale

<<<<<<< HEAD
- SQLite come storage locale per favoriti e cronologia;
- wrapper previsto: `sqlite-net-pcl`, coerente con le preferenze tecniche del repository;
- un unico database locale con tabelle distinte e sufficiente per il MVP;
- una sola `SQLiteAsyncConnection` condivisa, inizializzata da un servizio database registrato come singleton in `MauiProgram`;
- favoriti: memorizzano identificatore remoto e snapshot locale completa dei campi principali del dettaglio;
- cronologia: memorizza query normalizzata, testo originale e timestamp o ordinamento per recenza;
- delete singolo e clear totale esposti dal layer locale senza duplicare logica in UI.

Non è previsto un repository generico: il layer locale resta esplicito e limitato ai due casi d'uso del MVP, così da mantenere il codice leggibile e didattico.

Per le immagini remote, nel MVP è preferibile affidarsi al comportamento standard di `Image` e alla cache HTTP di base della piattaforma, usando placeholder visuali senza introdurre librerie dedicate se non motivate da problemi reali.

=======
- SQLite come storage locale per standings, teams, calendario.
- wrapper previsto: `sqlite-net-pcl`, coerente con le preferenze tecniche del repository.
- un unico database locale con tabelle distinte e sufficiente per il MVP.
- una sola `SQLiteAsyncConnection` condivisa, inizializzata da un servizio database registrato come singleton in `MauiProgram`.
- standings, teams, calendario: memorizzano snapshot locale dei dati principali.
- delete singolo e clear totale esposti dal layer locale senza duplicare logica in UI.

Non è previsto un repository generico: il layer locale resta esplicito e limitato ai casi d'uso del MVP, così da mantenere il codice leggibile e didattico.

Per le immagini remote, nel MVP è preferibile affidarsi al comportamento standard di `Image` e alla cache HTTP di base della piattaforma, usando placeholder visuali senza introdurre librerie dedicate se non motivate da problemi reali.


>>>>>>> it_01_bootstrap_project_deepseek
## 8. Dependency injection e composition root

`MauiProgram.cs` è il punto di composizione del progetto. Qui dovranno essere registrati:

<<<<<<< HEAD
- servizio remoto catalogo libri;
- servizio database SQLite condiviso;
- servizi o repository locali per favoriti e cronologia;
=======
- servizi remoti API motorsport;
- servizio database SQLite condiviso;
- servizi o repository locali per standings, teams, calendario;
>>>>>>> it_01_bootstrap_project_deepseek
- `IConnectivity` nativo di .NET MAUI, senza wrapper custom aggiuntivo;
- logging standard MAUI, se necessario;
- ViewModels e pagine.

Linee guida iniziali:

- servizi remoti e locali con lifetime applicativo;
- ViewModels e Views con lifetime leggero, coerente con la navigazione MAUI;
- nessuna istanziazione manuale di servizi nelle pagine.

<<<<<<< HEAD
=======

>>>>>>> it_01_bootstrap_project_deepseek
## 9. Error handling e logging

- Le eccezioni di rete, parsing e persistenza devono essere intercettate dal layer di servizio o dal ViewModel, non propagate direttamente alla UI.
- I ViewModels traducono i guasti in messaggi comprensibili e stati coerenti.
- I log diagnostici possono usare il logging standard disponibile in MAUI, senza introdurre telemetria o analytics nel MVP.
<<<<<<< HEAD
- Il refresh remoto dei favoriti non deve mai compromettere la disponibilità del contenuto locale già salvato.
=======
- Il fallback su dati locali (SQLite) non deve mai compromettere la disponibilità del contenuto già salvato.

>>>>>>> it_01_bootstrap_project_deepseek

## 10. Decisioni confermate

- SQLite resta la tecnologia di storage locale del MVP ed è previsto l'uso di `sqlite-net-pcl` come wrapper pratico sopra SQLite.
- Il progetto userà una sola `SQLiteAsyncConnection` condivisa, creata e inizializzata da un servizio database singleton registrato in `MauiProgram`.
<<<<<<< HEAD
- Sopra questa connessione vivranno due repository dedicati e separati: uno per `Favorites`, uno per `History`.
- Non verrà introdotto un repository generico o un ulteriore strato ORM: per questo progetto didattico la soluzione più leggibile è avere un layer locale piccolo e specifico.
- Non verrà introdotto un wrapper custom della connettività nel MVP.
- Il controllo della rete per il refresh da `Favorites` userà direttamente `IConnectivity` di .NET MAUI tramite dependency injection, confinato al servizio che orchestra il refresh remoto non bloccante.
- Questa scelta è considerata abbastanza testabile e abbastanza poco dipendente dalla piattaforma per il perimetro del MVP; verrà rivalutata solo se, durante l'implementazione, emergerà un limite concreto.
=======
- Sopra questa connessione vivranno repository dedicati e separati per standings, teams, calendario.
- Non verrà introdotto un repository generico o un ulteriore strato ORM: per questo progetto la soluzione più leggibile è avere un layer locale piccolo e specifico.
- Non verrà introdotto un wrapper custom della connettività nel MVP.
- Il controllo della rete per il refresh userà direttamente `IConnectivity` di .NET MAUI tramite dependency injection, confinato ai servizi che orchestrano il fallback/aggiornamento dati.

Questa scelta è considerata abbastanza testabile e poco dipendente dalla piattaforma per il perimetro del MVP; verrà rivalutata solo se, durante l'implementazione, emergerà un limite concreto.
>>>>>>> it_01_bootstrap_project_deepseek

Non risultano al momento ulteriori `TBD` architetturali bloccanti per l'avvio del MVP.
