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
