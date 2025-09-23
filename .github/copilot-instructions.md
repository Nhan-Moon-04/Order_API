Project: RestaurantApp (Angular 20)

Summary

- Angular 20 single-page application scaffolded with Angular CLI. Frontend talks to a backend API using `HttpClient` and `environment.apiUrl`.
- Project uses the new Angular `signal()` reactive primitives everywhere for component state.

What to know up-front

- Dev server: `npm run start` (runs `ng serve --proxy-config proxy.conf.js`). Use this to run the app locally; API calls are proxied by `proxy.conf.js` during local dev.
- Build: `npm run build` (standard `ng build`). Tests: `npm run test` (Karma).
- Codebase layout: `src/app/` contains pages and components. Notable folders: `admin/` (admin pages like `area-prices`), `components/`, `services/`, `model/`, and `shared/`.

Patterns & Conventions

- Signals for state: components declare UI state with `signal<T>(initial)` and read via `foo()` and set via `foo.set(value)`. Example: `dishNames = signal<string[]>([])` and `this.dishNames.set(response.data)`.
- Dependency injection via `inject()` inside components (not constructor injection). Example: `private http = inject(HttpClient)`.
- HTTP calls: use `HttpClient` with typed responses and `catchError` + `of` to return fallbacks. Backend base URL is `environment.apiUrl`.
- Debounce / user input: some components use RxJS `Subject` + `debounceTime` + `distinctUntilChanged()` for input suggestions (e.g., `dishNameInputSubject`). Keep this pattern when adding similar features.
- Template style: templates mix standard Angular templates with a custom-ish templating style (e.g., `@if (condition) { ... }`) — be careful when editing HTML; prefer small, localized edits and run the dev server to catch template compile errors.

API patterns and naming

- Backend endpoints follow `/AreaDishPrices/*`, `/Dishes/*`, `/Areas`, `/Tables/*` patterns. Look at `area-prices.component.ts` for examples calling: `${environment.apiUrl}/AreaDishPrices/Search` and `${environment.apiUrl}/AreaDishPrices/GetDishNames`.
- POST payload conventions: many endpoints expect JSON bodies like `{ searchString: ..., pageIndex: ..., pageSize: ... }`. Inspect `model/*.ts` request interfaces for exact shapes (e.g., `AreaDishPriceSearchRequest` in `src/app/model/area-dish-price.model.ts`).

Files to inspect when changing behavior

- `src/app/admin/area-prices/area-prices.component.ts` — complex component showing common patterns (signals, HttpClient, Subject debounce, pagination). Use it as the canonical example.
- `src/app/services/*.ts` — service wrappers that call backend endpoints; follow their patterns for new API integrations.
- `src/environments/environment*.ts` — contains `apiUrl` used across the app.

Testing and runtime checks

- After edits, run `npm run start` to catch template and compile-time errors quickly.
- Watch for template compile errors about missing component methods referenced in templates — templates are strict and will report missing symbols.

Editing rules for AI agents

- Minimal diffs: keep changes local and avoid broad reformatting. Preserve existing signal names and public method names unless renaming is necessary and usages are updated.
- Use existing conventions: `inject(HttpClient)` instead of constructor injection; use `signal()` for new state; use `Subject` + RxJS operators for debounced input flows.
- When editing templates that reference component methods, implement those methods in the component first or add safe fallbacks to avoid template compile errors.

Examples to follow

- Adding a debounced suggestion input: mirror `dishNameInputSubject` usage — create a `Subject<string>()`, pipe `debounceTime(300), distinctUntilChanged()`, subscribe and call a `load...Suggestions` method that POSTs `{ searchString: areaId || '', searchName: term }` to `AreaDishPrices/GetDishNames` and sets `dishNames` signal.
- Updating filters: set `signal` values with `.set(...)`, reset `pageIndex` via `this.pageIndex.set(1)`, then call `getAllAreaDishPrices(1)` to refresh results.

Do not assume

- Do not assume backend accepts areaId under a different field name — inspect models or existing calls. When in doubt, follow the payload shapes already used in the codebase (search requests use `searchString` or modeled request interfaces).

Where to ask for help

- If unsure about an API contract, consult `src/app/model/*.ts` for request/response interfaces or ask the backend owner. The frontend code has many examples of shapes used for requests.

If you modify this file

- Merge any existing human-written guidance rather than overwriting it. Keep this document concise and focused on actionable, project-specific patterns.

End
