# Specifica di Progetto - Chicane


## 1. Visione e contesto

### Problema da risolvere

Gli appassionati di motorsport devono spesso consultare molteplici fonti per ottenere countdown precisi, risultati live e classifiche aggiornate. Le app esistenti sono spesso caotiche, piene di pubblicità o con interfacce datate che non trasmettono il feeling tecnologico delle competizioni moderne.

### Obiettivo del progetto

Realizzare Chicane, un'applicazione .NET MAUI ad alte prestazioni, con estetica "Carbon & Neon". L'app offre una visualizzazione immersiva e dinamica per F1, WEC, WRC e MotoGP, con una Hero Section a schermo intero che mostra sempre l'immagine del circuito della prossima gara della categoria selezionata (di default F1). La Hero Section cambia stato tra:
- **Pre-gara**: mostra countdown, nome evento, meteo, data/ora, nome circuito.
- **Live**: mostra nome sessione (es. Prove Libere 1, Qualifica, Gara), stato della sessione, dati live (giri, posizioni, tempi, meteo live, ecc.), mappa circuito con posizione in tempo reale.

L'interfaccia è progettata per essere intuitiva: scorrendo verso il basso si accede a statistiche, classifiche, tempi, notizie e altri dati tecnici. La barra inferiore consente di navigare tra Home, Notizie, Scuderie (con possibilità di selezionare una scuderia per vedere stats e news dedicate). Una tendina superiore a scorrimento permette di cambiare categoria motorsport (F1, WEC, MotoGP, ecc.), sempre accessibile. In alto, o in posizione fissa, è presente un countdown globale all'evento motorsport più vicino (di qualsiasi categoria).

### Utenti target

- Fan accaniti di F1, WEC, WRC e MotoGP che cercano un "muretto box" tascabile.
- Utenti che desiderano un countdown estetico e preciso per i weekend di gara.
- Appassionati che necessitano di consultare rapidamente classifiche costruttori, piloti e notizie.

## 2. Ambito MVP


### Flusso principale da supportare

1. L'utente apre l'app e atterra sulla Hero Section a schermo intero (100vh) della categoria predefinita (F1), con immagine del circuito della prossima gara.
2. In alto è sempre visibile il countdown globale all'evento motorsport più vicino (di qualsiasi categoria).
3. La Hero Section mostra:
	- Se non ci sono sessioni attive: countdown, nome evento, meteo previsto, data/ora, nome circuito.
	- Se una sessione è in corso: nome sessione (es. Prove Libere 1, Qualifica, Gara), stato, dati live (giri, posizioni, tempi, meteo live), mappa circuito con posizione in tempo reale.
4. Scorrendo verso il basso si accede a statistiche, classifiche, tempi, notizie e altri dati tecnici.
5. La barra inferiore permette di navigare tra Home, Notizie, Scuderie (Teams) e altre sezioni.
6. Nella sezione Scuderie, l'utente può selezionare una scuderia per vedere statistiche, notizie e dettagli dedicati.
7. Scorrendo verso il basso (pull-down dall'alto) si apre una tendina minimal per cambiare categoria motorsport (F1, WEC, MotoGP, ecc.), con la prima categoria sempre F1.


### Funzionalità obbligatorie

- Dynamic Hero Section: Visualizzazione a schermo intero con immagine circuito, countdown o dati live, meteo, nome evento/sessione, stato sessione.
- Live Track Widget: Mappa del circuito con indicatore di posizione in tempo reale (Live Mode).
- Multi-Series Support: Gestione di F1, WEC, WRC e MotoGP con temi colore dedicati (#E10600 per F1, #004225 per WEC, #30839f per WRC, #FF6600 per MotoGP).
- Data Integration: Consumo di API remote (OpenF1/Ergast) e sistema di Mock per sviluppo offline.
- Global State Management: CategoryProvider per sincronizzare la serie attiva in tutta l'app.
- UI States: Gestione esplicita di Loading, Error, Empty e Success.
- Barra inferiore per navigazione tra Home, Notizie, Scuderie (Teams) e altre sezioni.
- Sezione Scuderie con possibilità di selezionare una scuderia per vedere stats, notizie e dettagli.
- Tendina superiore a scorrimento per cambiare categoria motorsport, sempre accessibile.
- Countdown globale all'evento motorsport più vicino, sempre visibile.


### Funzionalità opzionali future

- fase post-MVP 1: Notifiche push all'inizio di una gara, incidenti o altro.
- fase post-MVP 2: Widget per la Home (Android/iOS) con il countdown della categoria preferita.
- fase post-MVP 3: Archivio storico dei risultati delle stagioni precedenti.
- fase post-MVP 4: Integrazione profili dettagliati dei piloti con statistiche di carriera.

### Priorità roadmap post-MVP

Il planning successivo dovrà mantenere questo ordine di priorità:

1. Notifiche pre-gara;
2. Widget Home Screen;
3. Archivio storico;
4. Profili piloti;


### Non-obiettivi

- Streaming video o audio delle gare.
- Sistemi di scommesse o acquisto biglietti.
- Social network interno o commenti utenti.
- Supporto offline per i dati live (richiede connessione attiva).
- Parità funzionale iOS rispetto ad Android nel primo rilascio.

## 3. Scenari d'uso principali

### Scenario 1 - L'attesa del semaforo verde

Un utente apre Chicane tre giorni prima di un GP. La Hero Section visualizza un countdown monumentale in font Monospace su sfondo nero profondo. Sotto il timer, sono visibili le condizioni meteo previste per la pista.

### Scenario 2 - Monitoraggio muretto box

Durante la gara, l'utente usa l'app per vedere i distacchi (gap) tra i primi 4 piloti senza dover attendere la grafica TV. La mappa del circuito mostra in quale settore si trova attualmente la testa della corsa.

### Scenario 3 - Consultazione classifiche mondiali

Al termine di un weekend di gara, l'utente apre la sezione Standings per verificare il sorpasso in classifica costruttori, visualizzando i punti totali e il distacco tra le scuderie.

## 4. Requisiti funzionali

- FR-01: L'app deve fornire un countdown preciso per la prossima sessione della serie selezionata.
- FR-02: In modalità Live, l'app deve mostrare Giro Corrente / Giri Totali e i distacchi in secondi (Gaps).
- FR-03: La UI deve utilizzare esclusivamente font Sans-Serif e Monospace per garantire un look tecnico.
- FR-04: Il sistema deve permettere lo switch tra F1, WEC e MotoGP aggiornando istantaneamente il CategoryProvider
- FR-05: Le classifiche devono essere persistite localmente in SQLite per la consultazione offline di base.
- FR-06: L'utente deve poter forzare il refresh dei dati tramite una gesture di "pull-to-refresh" nelle liste.
- FR-07: La navigazione deve essere gestita tramite AppShell per garantire fluidità tra le Tab.
- FR-08: L'app deve mostrare l'intero calendario stagionale per la serie selezionata, distinguendo tra eventi passati (risultati) e futuri.
- FR-09: Il sistema deve permettere all'utente di programmare un avviso locale per l'inizio delle sessioni di qualifica e gara.

## 5. Epic, user stories e criteri di accettazione

### EPIC-01 - La hero section dinamica

**Obiettivo:**

Fornire un impatto visivo immediato e informativo all'apertura dell'app.

**User stories:**

- Come utente, voglio vedere quanto manca alla gara o cosa succede in pista appena apro l'app

**Criteri di accettazione:**

- [ ] Se non ci sono sessioni attive, mostra il Countdown.
- [ ] Se una sessione è "Live", mostra i dati telemetrici e la mappa del circuito.
- [ ] Lo sfondo deve essere #0A0A0A con font JetBrains Mono per i numeri.

### EPIC-02 - Multi-Series Switcher

**Obiettivo:**

Permettere la consultazione fluida di diversi campionati mantenendo la coerenza visiva.

**User stories:**

- Come utente, voglio passare dalla F1 alla MotoGP istantaneamente tramite un menu rapido.

**Criteri di accettazione:**

- [ ] Il menu pull-down superiore deve contenere le icone delle serie disponibili.
- [ ] Il cambio serie deve modificare l'accent color globale dell'applicazione.

## 6. Requisiti non funzionali

### UX e stati UI

- L'interfaccia deve evitare elementi decorativi inutili, privilegiando la leggibilità dei dati tecnici.
- Rispetto rigoroso della palette "Carbon & Neon" ( #0A0A0A, #E10600, Pure White).

### Prestazioni percepite

- Il passaggio tra le Tab della Shell deve essere istantaneo.

### Affidabilità e gestione errori

- In caso di API offline, l'app deve mostrare gli ultimi dati salvati in SQLite con un avviso di "Offline Mode".

### Privacy e dati

- Nel MVP non è prevista autenticazione né invio di dati personali a backend proprietari.
- Preferiti e cronologia sono memorizzati localmente sul dispositivo.
- I preferiti salvano anche i principali dati di dettaglio disponibili al momento del salvataggio, per consentire rilettura locale coerente.
- Eventuali note personali, se introdotte in una fase successiva, devono restare dati locali salvo requisito contrario esplicito.

## 7. Vincoli tecnici di progetto

- App `.NET MAUI` con target principale Android; iOS opzionale e secondario.
- Persistenza locale con SQLite per classifiche, team e calendario;
- Architettura MVVM.
- Navigazione basata su Shell.
- `CommunityToolkit.Mvvm` per la gestione dei ViewModel.
- `HttpClient` asincrono per le chiamate remote alel API.
- `System.Text.Json` per il parsing delle risposte API.
- Persistenza locale con SQLite per classifiche, team e calendario;
- Gestione esplicita di `IsBusy`, error state, empty state e presenza dati nei ViewModel.
- Nessuna logica REST o business logic nei code-behind.
- La UI deve essere ottimizzata per la visualizzazione di dati tabellari e grafici (telemetria).
- Flusso di navigazione: Home (Hero), Standings, Teams e News come viste dedicate gestite da Shell.

## 8. Metriche di successo

- Accesso ai dati live entro 2 secondi dall'apertura in condizioni di rete ottimali.
- Completamento della navigazione tra Home e Standings in meno di 2 tap.
- Persistenza corretta dei dati di classifica dopo il riavvio dell'applicazione.

## 9. Rischi, dipendenze e questioni aperte

### Rischi

- Latenza delle API pubbliche durante i picchi di traffico dei weekend di gara.

### Questioni aperte o `TBD`

TBD: Definizione di un modello dati astratto (RaceSession) che possa mappare uniformemente formati diversi come i GP di F1 e le gare di durata (WEC).

## 10. Passaggio al planning

Il prossimo passo è derivare i documenti di planning a partire da questo spec:

- `docs/plan.md`
- `docs/architecture.md`
- `docs/test-matrix.md`

La skill consigliata per il passo successivo è `prd-to-plan`.
