[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/kjGBNOdE)
 <div align="center">
 

# Chicane: F1 Live Companion

App `.NET MAUI` Android-first per seguire la Formula 1 in tempo reale: classifiche, calendario, live timing, meteo e news. Progetto didattico sviluppato con agenti AI, skill riusabili e workflow Man-in-the-Loop.

 [![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/maui/)
 [![Workflow](https://img.shields.io/badge/workflow-Man--in--the--Loop-0F766E)](docs/method/man-in-the-loop.md)
 [![Skills](https://img.shields.io/badge/skills-8%20locali-2563EB)](.agents/skills/)
 [![Docs](https://img.shields.io/badge/docs-spec%20%7C%20plan%20%7C%20architecture%20%7C%20tests-7C3AED)](docs/)
 [![Version](https://img.shields.io/badge/version-1.1.0-E10600?logo=github&logoColor=white)]()
 [![Backend](https://img.shields.io/badge/backend-ASP.NET%20Core%2010%20%2B%20Redis-512BD4?logo=dotnet&logoColor=white)]()
 [![HTTPS](https://img.shields.io/badge/HTTPS-DuckDNS%20%2B%20Let's%20Encrypt-0F766E)]()

 [Panoramica](#panoramica) ·
 [Architettura](#architettura) ·
 [Stack Di Skill](#stack-di-skill) ·
 [Workflow](#workflow) ·
 [Versioni](#versioni) ·
 [Documenti](#documenti)

 </div>

 ---

 
 ## Panoramica

**Chicane** è una companion app Formula 1 completa, con client `.NET MAUI` (Android) e backend `ASP.NET Core 10` su infrastruttura Docker.

L'app offre:

| Funzionalità | Descrizione |
|-------------|-------------|
| **Home** | Prossimo evento F1, countdown, meteo, card GP con tracciato, top piloti e team |
| **Schedule** | Calendario GP futuri e passati, risultati Top 3, sessioni per meeting |
| **Leaderboard** | Classifica piloti e costruttori con foto e punti |
| **Live** | Posizioni in tempo reale, mappa tracciato, intervalli, commentario gara |
| **Driver & Team** | Dettaglio pilota con storico risultati, profilo team con statistiche stagione |
| **News** | Aggregatore RSS multi-fonte con filtro F1 (Sky Sports, Motorsport, FormulaPassion, FuoriTraiettoria, P300.it) |
| **Settings** | Gestione cache (DB / immagini), info app |

### Stack

| Layer | Tecnologia |
|-------|-----------|
| **Client** | `.NET MAUI` Android, MVVM con CommunityToolkit.Mvvm, SQLite cache-first, Shell navigation |
| **Backend** | `ASP.NET Core 10` Web API, Docker multi-stage, Redis cache |
| **Proxy** | NGINX Proxy Manager su LXC separato, DuckDNS + Let's Encrypt SSL |
| **Infra** | Proxmox VE, 2 LXC Debian 13, Docker Compose, rete locale 192.168.1.x |
| **Dati** | [OpenF1 API](https://openf1.org/), OpenWeatherMap, RSS2JSON |

 
 ## Architettura

```
┌──────────────┐     HTTPS      ┌──────────────┐     HTTP      ┌────────────────┐
│  MAUI Client │ ────────────→  │  NGINX Proxy │ ──────────→  │  ASP.NET Core  │
│  (Android)   │                │  DuckDNS SSL │              │  Backend :5000  │
│  SQLite CF   │                │ 192.168.1.201│              │  192.168.1.205  │
└──────────────┘                └──────────────┘              └───────┬────────┘
                                                                      │
                                              ┌───────────────────────┼───────────────┐
                                              │                       │               │
                                         ┌────▼─────┐          ┌─────▼──────┐  ┌─────▼──────┐
                                         │  Redis   │          │  OpenF1    │  │ RSS2JSON + │
                                         │  Cache   │          │  API       │  │ OpenWeather│
                                         └──────────┘          └────────────┘  └────────────┘
```

### Pattern proxy trasparente

Il backend **non** aggrega né trasforma i dati OpenF1. Agisce come **proxy trasparente**:

```
Client → /api/proxy/drivers?session_key=123 → Backend → Redis cache? → OpenF1 → risposta identica
```

Il client parla il formato OpenF1 nativo. Il backend aggiunge solo autenticazione e cache Redis condivisa (TTL: 3s live, 30min standings, 1h schedule, 24h results). Zero segreti sul client.

### Cache multi-livello

- **Redis** (condiviso tra tutti i client): TTL variabili per tipo dato
- **SQLite** (per dispositivo): cache-first locale per dati immutabili (risultati, calendario)
- **Immagini**: download asincrono con cache locale


 ## Perché Esiste

Nel contesto didattico del progetto, l'AI non viene trattata come un generatore automatico di codice. Viene usata come collaboratore dentro un processo controllato.

Il repository mostra come:

- trasformare un'idea di app motorsport in un `PRD` verificabile;
 - derivare dal `PRD` un piano in iterazioni piccole;
 - usare skill specializzate invece di prompt generici;
 - tenere separati prodotto, architettura, test e implementazione;
 - migliorare le skill con eval, baseline e refinement successivi;
 - mantenere responsabilità umana su decisioni, review, test, merge e push.

 
 ## Stack Di Skill


 | Skill | Badge | Quando usarla |
 | --- | --- | --- |
 | [`find-skills`](.agents/skills/find-skills/) | ![discover](https://img.shields.io/badge/discover-skills-2563EB) | Cercare o installare skill esterne |
 | [`skill-creator`](.agents/skills/skill-creator/) | ![creator](https://img.shields.io/badge/create-skill-7C3AED) | Creare, testare e raffinare skill |
 | [`prd`](.agents/skills/prd/) | ![prd](https://img.shields.io/badge/base-PRD-4B5563) | Skill PRD generica usata come confronto |
 | [`maui-prd`](.agents/skills/maui-prd/) | ![spec](https://img.shields.io/badge/output-docs%2Fspec.md-0F766E) | Definire o aggiornare il PRD MAUI |
 | [`prd-to-plan`](.agents/skills/prd-to-plan/) | ![plan](https://img.shields.io/badge/output-plan%20%7C%20architecture%20%7C%20tests-0F766E) | Trasformare il PRD in piano, architettura e test matrix |
 | [`maui-expert`](.agents/skills/maui-expert/) | ![maui](https://img.shields.io/badge/MAUI-MVVM%20%7C%20Shell%20%7C%20XAML-512BD4) | Scrivere o rivedere codice `.NET MAUI` |
 | [`maui-automatic-testing`](.agents/skills/maui-automatic-testing/) | ![tests](https://img.shields.io/badge/testing-ViewModel%20%7C%20Service%20%7C%20Build-B45309) | Definire test automatici realistici per una slice |
 | [`man-in-the-loop-workflow`](.agents/skills/man-in-the-loop-workflow/) | ![workflow](https://img.shields.io/badge/process-Planning%20to%20Git-0F766E) | Governare ogni iterazione con scope, review, test e documentazione |

 Le skill reali vivono in [`.agents/skills/`](.agents/skills/). La cartella [`.claude/skills/`](.claude/skills/) contiene link simbolici verso quelle stesse skill, così Claude Code può usarle senza creare doppioni.

 ## Workflow

 ```mermaid
 flowchart LR
     subgraph Product["Definizione e pianificazione"]
         direction TB
         Idea["Idea app"]
         PRD["maui-prd<br/>docs/spec.md"]
         Plan["prd-to-plan<br/>plan + architecture + test-matrix"]
         Iteration["man-in-the-loop-workflow<br/>iterazione controllata"]

         Idea --> PRD --> Plan --> Iteration
     end

     subgraph Delivery["Implementazione e chiusura"]
         direction TB
         Build["maui-expert<br/>codice MAUI"]
         Test["maui-automatic-testing<br/>test automatici e manuali"]
         Docs["docs/iterations<br/>chiusura e Git"]
         Next["iterazione successiva"]

         Build --> Test --> Docs --> Next
     end

     Product --> Delivery
 ```


 ## Versioni

| Versione | Data | Descrizione |
|----------|------|-------------|
| **v1.1.0** | Maggio 2026 | Backend ASP.NET Core 10 con proxy trasparente, cache Redis, RSS news multi-fonte, HTTPS DuckDNS, zero segreti sul client |
| **v1.0.0** | Maggio 2026 | MVP: Home, Schedule, Leaderboard, Live, Driver/Team detail, SQLite cache-first, font custom, UI hardening |

 
 ## Documenti

| Area | File | Descrizione |
|------|------|-------------|
| Prodotto | [docs/spec.md](docs/spec.md) | PRD e requisiti |
| Piano | [docs/plan.md](docs/plan.md) | Iterazioni pianificate |
| Architettura | [docs/architecture.md](docs/architecture.md) | Design dell'app |
| Test | [docs/test-matrix.md](docs/test-matrix.md) | Matrice dei test |
| Iterazioni | [docs/iterations/](docs/iterations/) | Log delle iterazioni completate |
| Metodo | [docs/method/man-in-the-loop.md](docs/method/man-in-the-loop.md) | Workflow didattico |
| History | [docs/history/](docs/history/) | Sessioni ed eval |
