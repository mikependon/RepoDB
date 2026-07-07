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
