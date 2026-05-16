[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/kjGBNOdE)
 <div align="center">
 

# Chicane: Motorsport Live Companion
 
Demo didattica per progettare, pianificare e governare una app `.NET MAUI` Android-first dedicata al motorsport (F1, WEC, MotoGP, ecc.) con agenti AI, skill riusabili e workflow Man-in-the-Loop.
 
 [![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/maui/)
 [![Workflow](https://img.shields.io/badge/workflow-Man--in--the--Loop-0F766E)](docs/method/man-in-the-loop.md)
 [![Skills](https://img.shields.io/badge/skills-8%20locali-2563EB)](.agents/skills/)
 [![Docs](https://img.shields.io/badge/docs-spec%20%7C%20plan%20%7C%20architecture%20%7C%20tests-7C3AED)](docs/)
 [![Claude Code](https://img.shields.io/badge/Claude%20Code-symlink%20skills-111827)](.claude/skills/)
 [![Repository](https://img.shields.io/badge/repository-privato-B91C1C?logo=github&logoColor=white)](https://github.com/GreppiDev/maui-project-demo)
 
 [Panoramica](#panoramica) ·
 [Stack Di Skill](#stack-di-skill) ·
 [Workflow](#workflow) ·
 [Strategia Git](#strategia-git) ·
 [Documenti](#documenti) ·
 [Cronologia](#cronologia) ·
 [Come Leggere](#come-leggere)
 
 </div>
 
 ---

 
 ## Panoramica
 
Questo repository non contiene ancora il codice completo dell'app `Chicane`, ma è un laboratorio didattico per mostrare come usare agenti AI senza perdere controllo su analisi, pianificazione, review, test e documentazione.
 
L'app di riferimento è una companion app motorsport `.NET MAUI` Android-first per seguire live dati, classifiche, team, calendario e meteo di F1, WEC, MotoGP e altre serie. Il cuore del repo è il sistema che prepara lo sviluppo:
 
 | Area | Cosa contiene | Link |
 | --- | --- | --- |
| Prodotto | PRD e requisiti di `Chicane` | [docs/spec.md](docs/spec.md) |
 | Pianificazione | Iterazioni, architettura e matrice test | [docs/plan.md](docs/plan.md), [docs/architecture.md](docs/architecture.md), [docs/test-matrix.md](docs/test-matrix.md) |
 | Skill | Skill locali per PRD, planning, MAUI, test e workflow | [`.agents/skills/`](.agents/skills/) |
 | Claude Code | Link simbolici alle skill OpenCode senza duplicazione | [`.claude/skills/`](.claude/skills/) |
 | Metodo | Workflow didattico Man-in-the-Loop | [docs/method/man-in-the-loop.md](docs/method/man-in-the-loop.md) |
 | Evidenze | Sessioni, eval, benchmark e refinement | [docs/history/](docs/history/) |

 
 La cartella [`src/`](src/) è al momento una radice predisposta: il repository documenta soprattutto metodo, skill e progettazione prima dell'implementazione applicativa.

 
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