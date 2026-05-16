# AGENTS.md

## Project context

Chicane is a .NET MAUI Android-first F1 companion app with an ASP.NET Core 10 backend.
iOS support is optional and secondary.

The backend acts as a transparent proxy to the OpenF1 API with Redis caching.
The client uses cache-first SQLite for local persistence. No secrets on the client.

## Technical preferences

- Framework UI: .NET MAUI
- Architecture: MVVM
- Navigation: Shell
- Local persistence: Preferences + SQLite (sqlite-net-pcl)
- Remote calls: HttpClient (async)
- JSON parsing: System.Text.Json
- MVVM toolkit: CommunityToolkit.Mvvm
- Focus: robustness, readability, code consistency

## Rules

- Always propose a plan before large changes.
- Limit each iteration to one well-defined feature.
- Do not add NuGet packages without explicit motivation.
- Do not put logic in code-behind if it belongs in a ViewModel or Service.
- Always handle loading, error, and empty states.
- Do not remove existing code without explanation.
- Avoid unnecessary duplication.
- Prefer clear, consistent names (PascalCase classes/properties, camelCase locals).
- Update documentation when project behaviour changes.
- Use git branches for each iteration; write semantic commits.

## Output format

For each significant request, return:

1. Short plan;
2. Files to create or modify;
3. Implementation;
4. Risks and checks;
5. Suggested manual tests.

## Coding style

- Small classes with clear responsibility;
- Services separated from ViewModels;
- ViewModels with explicit state properties (IsBusy, ErrorMessage, HasData);
- Async methods where appropriate;
- Error handling must not be silent;
- Comments only when truly useful, never repeating what the code says.

## Anti-patterns

- REST logic in View or code-behind;
- Mixed Shell and non-Shell navigation;
- Inconsistent field/property naming;
- NuGet dependencies added without review;
- Overly broad refactoring in a single iteration.
