# AI Prompt Log — Chicane

> Documented interactions with AI agents (OpenCode / Claude) during the development of Chicane.  
> Each entry describes the problem context, the AI's response, my critical evaluation, and the final outcome.

---

## Prompt 1 — Proxy Architecture Decision

**Context**: The backend was initially built as an "aggregator" (8 controllers combining OpenF1 data into custom endpoints). The client's 849-line `OpenF1ApiService` had to be rewritten to parse the new aggregated format — causing bugs everywhere (wrong dates, missing images, broken standings).

**Prompt**: "Il client originale chiama OpenF1 direttamente. Se il backend facesse da proxy trasparente, inoltrando le richieste senza toccare il JSON, il client resterebbe identico?"

**AI Response**: "Sì, il proxy trasparente è l'approccio più semplice. Il backend riceve `/api/proxy/{**path}` e lo inoltra a `api.openf1.org/v1/{path}` con autenticazione Bearer. Il client cambia solo il `BaseAddress`. Si può aggiungere Redis caching tra client e OpenF1 per ridurre le chiamate API."

**Critical evaluation**: Corretto al 100%. L'approccio aggregator era stato una scelta sbagliata iniziale — aggiungeva complessità senza valore. Il proxy ha eliminato 7 controller (Home, Standings, Schedule, Teams, Drivers, Sessions, Live), ridotto l'`OpenF1Service` da 287 a 52 righe, e ripristinato il client al suo stato testato.

**Outcome**: Backend ridotto a 3 controller (Proxy, News, Weather). Client identico al pre-backend. Redis cache con TTL differenziati (3s live, 30min standings, 1h schedule, 24h results).

---

## Prompt 2 — Performance Optimization (5s → <500ms)

**Context**: Dopo il deploy del proxy, l'app impiegava 5-6 secondi al primo avvio. Il `HomeViewModel.LoadAsync()` faceva 7 chiamate API in serie, anche quando i dati erano già disponibili in cache.

**Prompt**: "L'app ci mette 5-6 secondi ad aprirsi. Cosa sta succedendo?"

**AI Response**: "Analisi del `LoadAsync`: 7 chiamate `await` seriali, di cui 4 indipendenti. `GetNextEventAsync` bypassa la cache in-memory per le sessions. `GetUpcomingWeekendsAsync` non usa la cache SQLite. Proposta: parallelizzare con `Task.WhenAll`, usare `GetSessionsAsync` con cache, ripristinare SQLite per weekends."

**Critical evaluation**: Diagnosi precisa. La parallelizzazione era la soluzione principale, ma l'AI ha anche identificato la missing cache sessions (che richiedeva 200-500ms extra ogni avvio) e la SQLite weekends rimossa per un bug precedente (immagini mancanti).

**Outcome**: 4 fix applicati: (1) parallelizzazione con `Task.WhenAll`, (2) uso di `GetSessionsAsync` con in-memory cache, (3) ripristino cache SQLite per weekends con invalidazione basata su `CircuitImageUrl`, (4) Redis priming nel deploy.sh. Risultato: primo avvio ~2s, avvii successivi <500ms.

---

## Prompt 3 — Team Points Bug (215 ≠ 180)

**Context**: La pagina "Race Results" dei team mostrava punti per gara che, sommati, non corrispondevano al totale ufficiale (Mercedes: 215pts calcolati vs 180pts reali).

**Prompt**: "I punti team per gara sommano a 215 ma l'API dice 180. Perché?"

**AI Response**: "Il bug è in `CachedTeamsService.cs` — i punti per gara sono calcolati con una funzione `F1Points(position)` hardcoded (25-18-15-12-10-8-6-4-2-1). Non tiene conto di: Sprint sessions (che hanno una tabella punti diversa: 8-7-6-5-4-3-2-1), fastest lap bonus (+1), penalità post-gara. Inoltre, i punti costruttori non sono la somma dei punti piloti — sono due classifiche separate. La discrepanza di 35 punti (~215-180) è probabilmente dovuta a 2 Sprint trattate come gare piene (2 × (25-8) = 34)."

**Critical evaluation**: L'AI ha identificato correttamente la causa. La tabella Sprint (8 per P1) vs Race (25 per P1) era la fonte principale dell'errore. Le penalità e il fastest lap sono marginali e accettabili come approssimazione.

**Outcome**: Modificata la funzione `RacePoints(position, sessionName)` con due tabelle: 25-18-15... per Race, 8-7-6-5... per Sprint. Il totale header viene sempre dall'API (`constructorStandings.PointsCurrent`) ed è il valore autoritativo.

---

## Prompt 4 — RSS News Pipeline (5 feeds, F1 filter)

**Context**: La tab News era vuota. L'RSS2JSON API key era mancante nel backend `.env`. Dopo averla aggiunta, le notizie arrivavano ma includevano MotoGP, TCR, e notizie non-F1.

**Prompt**: "Le news mostrano anche Moto3, TCR, MotoGP. Voglio SOLO F1."

**AI Response**: "Aggiungere un filtro a doppio livello: (1) keyword F1 positive (F1, Verstappen, Monza, qualifying...), (2) keyword negative per escludere altre categorie (MotoGP, WEC, TCR, IndyCar, NASCAR...). Inoltre, usare feed F1-specifici: Sky Sports F1 (12433), non il feed generico (12040)."

**Critical evaluation**: La soluzione a doppio filtro è elegante. Il feed Sky Sports F1 12433 era quello corretto — il precedente 12040 era un feed sportivo generico. Le keyword negative risolvono il problema dei siti generalisti (FormulaPassion, P300.it) che coprono più categorie.

**Outcome**: 5 feed RSS configurati (Sky Sports F1, Motorsport Italia, FormulaPassion, FuoriTraiettoria, P300.it), 23 keyword positive + 13 negative. Il backend `Rss2JsonService` include anche un parser sicuro per date (`SafeDateTime`) perché Motorsport Italia restituiva date in formato non standard.

---

## Prompt 5 — Docker + LXC Infrastructure

**Context**: Il backend doveva essere deployato su un server reale (Proxmox VE con container LXC Debian 13) con HTTPS pubblico.

**Prompt**: "Come deployo il backend ASP.NET su LXC Proxmox con HTTPS?"

**AI Response**: "Docker Compose per backend + Redis su LXC 192.168.1.205. NGINX Proxy Manager su LXC separato 192.168.1.201 come reverse proxy con Let's Encrypt SSL. DuckDNS per dynamic DNS gratuito su `chicane-api.duckdns.org`. Port forwarding router TCP 80/443 → NGINX LXC."

**Critical evaluation**: Architettura pulita e a costo zero. La separazione backend/NGINX su due LXC è best practice per sicurezza. DuckDNS + Let's Encrypt gestiscono automaticamente il rinnovo dei certificati. Unica complessità: il deploy via `scp` è manuale (non c'è CI/CD), ma per un progetto personale è più che sufficiente.

**Outcome**: Backend raggiungibile via HTTPS su `chicane-api.duckdns.org/api/proxy/...`. Redis condiviso tra tutti i client. `deploy.sh` con Redis priming post-deploy. Zero costi cloud.

---

## Prompt 6 — English UI Default Language

**Context**: L'app aveva testo UI misto italiano/inglese (residuo dello sviluppo iniziale). Per la pubblicazione come progetto open-source, serviva inglese come default.

**Prompt**: "Traduci tutti i testi italiani nell'app in inglese."

**AI Response**: "Identificati 11 testi italiani in 6 file (5 XAML + 1 ViewModel). Sostituzioni: SEGUI LIVE → FOLLOW LIVE, PROSSIME GARE → UPCOMING RACES, CLASSIFICA PILOTI → DRIVER STANDINGS, SVUOTA CACHE → CLEAR CACHE, ecc."

**Critical evaluation**: Traduzione precisa e contestuale. L'AI ha notato che la localizzazione futura (con API di traduzione) richiederebbe file `.resx` separati, ma per ora il refactoring diretto è più rapido e pulito.

**Outcome**: 11 stringhe tradotte. App completamente in inglese. Pronta per pubblico internazionale. La localizzazione multi-lingua (con API) è pianificata per una release futura.

---

## Summary

| # | Topic | Iteration | AI Role | Outcome |
|---|-------|-----------|---------|---------|
| 1 | Proxy architecture | IT-07 | Decision analysis | Client zero-changes, backend -82% code |
| 2 | Performance | IT-07.1 | Root cause diagnosis | 5s → <500ms load time |
| 3 | Team points | IT-07.2 | Bug analysis + fix | Sprint/race scoring distinction |
| 4 | News pipeline | IT-07.3 | Pipeline design + filtering | 5 feeds, F1-only, safe parsing |
| 5 | Infrastructure | IT-07.4 | Architecture design | Zero-cost self-hosted HTTPS |
| 6 | English UI | IT-07.5 | Translation + localization plan | 11 strings, international-ready |
