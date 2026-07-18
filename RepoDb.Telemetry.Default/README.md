# RepoDb.Telemetry.Default

Drop-in telemetry for RepoDB. Wires up a default `ITrace` that captures every operation (Insert, Query, Update, Delete, etc.) and publishes it to an insights collector — no custom `ITrace` implementation required.

Built on top of [`RepoDb.Telemetry.Core`](../RepoDb.Telemetry.Core/README.md).

## Getting Started

Install the package:

```
dotnet add package RepoDb.Telemetry.Default
```

Enable it once at application startup:

```csharp
GlobalConfiguration
    .Setup(new GlobalConfigurationOptions { UseRegisteredGlobalTraces = true })
    .UseDefaultTelemetry(
        host: "https://your-collector-host",
        apiKey: "YOUR_API_KEY",
        applicationName: "MyApp",
        groupName: "Default");
```

That's it — every operation across all connections in the app is now traced automatically.

> `UseRegisteredGlobalTraces = true` is required. It tells RepoDB to run every globally registered tracer (this one included) for every operation, without having to pass a `trace` argument to each call.

## How It Works

- Each operation's start/end is captured as a `TelemetryItem` (application, group, operation name, statement, elapsed time, client machine, source assembly, etc.).
- Items are buffered in memory and flushed on an interval (`Frequency`, default 5 seconds).
- On flush, the batch is JSON-serialized, gzip-compressed, and POSTed to `{host}/v1/telemetry/publish`, with an `X-API-Key` header when an API key is supplied.
- Publish failures never throw — they're routed to the optional `errorCallback` and `logger`.

## The `DefaultTelemetryItem` Model

Every captured operation is represented as a `DefaultTelemetryItem` — a concrete, RepoDB-specific subclass of `RepoDb.Telemetry.Core`'s `TelemetryItem` (it adds no fields of its own; it exists so the default pipeline has its own type to construct, serialize, and evolve independently of the core package). Each instance carries:

| Property | Description |
|---|---|
| `Application` | Name of the application that produced the telemetry (from your `DefaultTelemetryOption`). |
| `Group` | The logical group the application is categorized under. |
| `SessionId` | Unique identifier correlating an operation's before/after execution logs. |
| `Operation` | The operation name (e.g. `Insert`, `Query`, `Update`, `Delete`). |
| `StartTime` | UTC timestamp of when the operation started executing. |
| `Statement` | The SQL statement (or equivalent) that was executed. |
| `Elapsed` | Total elapsed time of the operation, in milliseconds. |
| `IsCancelled` | Whether the operation was cancelled from within a trace callback. |
| `Client` | The machine name the operation ran on. |
| `Source` | The name of the assembly that hosts/consumes RepoDB (your application). |
| `Version` | The version of that assembly. |

These items are what gets buffered, batched, and serialized to JSON before being published to your collector.

## Not an OpenTelemetry Collector — By Design

This package intentionally does **not** ship an OpenTelemetry (OTel) exporter or rely on the OTel SDK. `DefaultTelemetryTrace` hooks directly into RepoDB's `ITrace` before/after execution events and serializes its own lightweight `DefaultTelemetryItem` payload straight to HTTP — there's no OTel `Span`/`Activity` machinery, no resource/attribute mapping layer, and no dependency on the OTel collector protocol in the hot path.

That's a deliberate tradeoff, not an oversight:

- **Performance.** Operations are captured with a handful of property assignments and buffered in memory; nothing allocates spans, contexts, or goes through an exporter pipeline per call. For a library whose whole value proposition is being a thin, fast layer over ADO.NET, that overhead matters.
- **Tight coupling to the library.** The capture shape (`DefaultTelemetryItem`) mirrors RepoDB's own operation model (session id, statement, elapsed time, cancellation) instead of being generalized to fit an industry-wide schema, keeping the implementation simple and directly meaningful for RepoDB users.

An OTel-based collector is planned as a separate, opt-in package for enterprise-grade telemetry scenarios (distributed tracing across services, vendor-neutral export to existing observability stacks, etc.). Until then, `RepoDb.Telemetry.Default` is the fast, zero-fuss path to seeing what your RepoDB operations are doing.

## Configuration

For more control, pass a `DefaultTelemetryOption` instead of individual arguments:

```csharp
GlobalConfiguration
    .Setup(new GlobalConfigurationOptions { UseRegisteredGlobalTraces = true })
    .UseDefaultTelemetry(
        new DefaultTelemetryOption("MyApp")
        {
            Host = "https://your-collector-host",
            ApiKey = "YOUR_API_KEY",
            Group = "Default",
            Frequency = TimeSpan.FromSeconds(10)
        },
        errorCallback: ex => logger.LogError(ex, "Telemetry publish failed"),
        logger: serilogLogger);
```

| Property | Description | Default |
|---|---|---|
| `Application` | Name of the application producing telemetry. | *(required)* |
| `Group` | Logical grouping for the dashboard. | `"Default"` |
| `Host` | Collector endpoint to publish to. | `http://localhost:5000` |
| `ApiKey` | API key sent via `X-API-Key`. Leave empty if the collector doesn't require one. | `null` |
| `Frequency` | How often buffered telemetry is flushed. | `5` seconds |

`UseDefaultTelemetry` is idempotent — calling it again reuses the same underlying tracer instance rather than starting a new one.

## What's with Docker Compose

The `docker-compose.yml` at the root of this project spins up a complete, self-hosted telemetry stack: a Postgres database, the HTTP collector your app publishes to, a query API, two background workers (a file data sinker and a purger), and a Grafana-based visualization dashboard.

Most settings are supplied as environment variables using the `${VAR:-default}` pattern, so they can be overridden without touching the compose file — either by exporting them in your shell or by placing a `.env` file next to `docker-compose.yml`. A few values (like the Postgres password on the `pgsql` service itself) are hardcoded and would need a direct edit to the compose file to change.

### pgsql

The Postgres database backing the entire stack — the collector writes to it, and the query, filedatasinker, purger, and visualization services all read from it.

| Environment Variable | Description | Default |
|---|---|---|
| `POSTGRES_PASSWORD` | Password for the Postgres superuser, used to initialize the database. Hardcoded in the compose file rather than sourced from a `${...}` variable, so changing it means editing `docker-compose.yml` directly — and keeping it in sync with `REPODB_PG_PASSWORD` used elsewhere. | `RepoDB2026` |

### collector

The HTTP endpoint your application publishes telemetry to (this is the `host` you pass to `UseDefaultTelemetry`). It validates the API key and writes incoming batches to Postgres.

| Environment Variable | Description | Default |
|---|---|---|
| `API_KEY` | API key required via the `X-API-Key` header on publish requests. Should match the `apiKey` your app passes to `UseDefaultTelemetry`. Sourced from `REPODB_API_KEY`. | `RepoDB2026` |
| `CONNECTION_STRING` | PostgreSQL connection string used to persist published telemetry into the `repodb_insights` database. The password segment is sourced from `REPODB_PG_PASSWORD`. | `postgresql://postgres:RepoDB2026@pgsql:5432/repodb_insights` |

### query

The read API that serves telemetry data to the visualization dashboard.

| Environment Variable | Description | Default |
|---|---|---|
| `API_KEY` | API key required to authenticate query requests. Sourced from `REPODB_API_KEY`. | `RepoDB2026` |
| `CONNECTION_STRING` | PostgreSQL connection string used to read telemetry data. The password segment is sourced from `REPODB_PG_PASSWORD`. | `postgresql://postgres:RepoDB2026@pgsql:5432/repodb_insights` |
| `DIRECTORY_PATH` | Path inside the container where telemetry files (staged by `filedatasinker`) are read from. Backed by the shared `telemetry_data` volume. | `/tmp/repodb/telemetry` |
| `ALLOWED_ORIGINS` | CORS-allowed origin permitted to call the query API — normally the URL the visualization dashboard is served from. Sourced from `REPODB_VISUALIZATION_ORIGIN`. | `http://localhost:3000` |

### filedatasinker

A background worker that periodically exports telemetry data from Postgres into flat files for downstream consumption by the `query` service.

| Environment Variable | Description | Default |
|---|---|---|
| `CONNECTION_STRING` | PostgreSQL connection string the sinker reads telemetry from. The password segment is sourced from `REPODB_PG_PASSWORD`. | `postgresql://postgres:RepoDB2026@pgsql:5432/repodb_insights` |
| `DIRECTORY_PATH` | Path inside the container where exported telemetry files are written. Backed by the shared `telemetry_data` volume. | `/tmp/repodb/telemetry` |
| `FREQUENCY_IN_MINUTES` | How often, in minutes, the sinker runs its export pass. Sourced from `REPODB_FILEDATASINKER_FREQUENCY_IN_MINUTES`. | `60` |

### purger

A background worker that deletes telemetry records older than a configured retention window, keeping the Postgres database from growing unbounded.

| Environment Variable | Description | Default |
|---|---|---|
| `CONNECTION_STRING` | PostgreSQL connection string the purger deletes records through. The password segment is sourced from `REPODB_PG_PASSWORD`. | `postgresql://postgres:RepoDB2026@pgsql:5432/repodb_insights` |
| `RETENTION_PERIOD_IN_MINUTES` | How long, in minutes, telemetry records are kept before being eligible for deletion. Sourced from `REPODB_PURGER_RETENTION_PERIOD_IN_MINUTES`. | `10080` (7 days) |
| `FREQUENCY_IN_MINUTES` | How often, in minutes, the purger runs its cleanup pass. Sourced from `REPODB_PURGER_FREQUENCY_IN_MINUTES`. | `5` |

### visualization

The Grafana-based dashboard used to browse and chart the telemetry your app has published.

| Environment Variable | Description | Default |
|---|---|---|
| `GF_SECURITY_ADMIN_PASSWORD` | Admin password for signing into the Grafana dashboard. | `RepoDB2026` |
| `REPODB_PG_HOST` | Postgres host the dashboard's data source connects to. | `pgsql` |
| `REPODB_PG_PORT` | Postgres port the dashboard's data source connects to. | `5432` |
| `REPODB_PG_PASSWORD` | Postgres password used by the dashboard's data source connection. | `RepoDB2026` |
| `REPODB_COMPANY_NAME` | Company name shown in the dashboard's branding. | `Your Company Name` |
| `REPODB_COMPANY_LOGO` | URL of the logo shown in the dashboard's branding. | RepoDB's logo on GitHub |
| `REPODB_API_KEY` | API key the dashboard uses when calling the collector/query API. | `RepoDB2026` |

### Volumes and networks

Two named volumes persist state across restarts: `pgdata` (the Postgres data directory) and `telemetry_data` (the shared directory `filedatasinker` writes to and `query` reads from). All services share a single bridge network, `repodb`, so they can reach each other by service name (e.g. `pgsql`, `collector`).
