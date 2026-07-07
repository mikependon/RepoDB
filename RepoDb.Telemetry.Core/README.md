# RepoDb.Telemetry.Core

Core building blocks for capturing operation-level telemetry (Insert, Query, Update, Delete, etc.) from RepoDB and publishing it to an insights collector.

This package is not meant to be used standalone by most users — it defines the contracts and reusable pieces that a telemetry implementation is built on. If you just want telemetry working out of the box, use [`RepoDb.Telemetry.Default`](../RepoDb.Telemetry.Default/README.md) instead. Use this package directly only if you want to customize how telemetry is published or captured.

## Key Types

- **`TelemetryOption`** — settings for a telemetry session: `Application`, `Group`, `Host`, `ApiKey`, and `Frequency` (how often buffered data is flushed).
- **`TelemetryItem`** — the shape of a single captured telemetry record (operation name, statement, elapsed time, session id, client, source, etc.).
- **`TelemetryTrace`** — an `ITrace` implementation that hooks into RepoDB's before/after execution events, buffers them in memory, and flushes on a timer.
- **`IPublisherRepository`** — the contract for sending buffered `TelemetryItem`s to a collector.
- **`TelemetryPublisherRepository`** — the default `IPublisherRepository`; gzip-compresses and POSTs the payload to `{host}/v1/telemetry/publish`, adding an `X-API-Key` header when an API key is provided.

## Getting Started

Create an option, start a trace, and attach it to RepoDB:

```csharp
var option = new TelemetryOption("MyApp")
{
    Host = "https://your-collector-host",
    ApiKey = "YOUR_API_KEY",
    Group = "Default",
    Frequency = TimeSpan.FromSeconds(5)
};

var trace = new TelemetryTrace(option, errorCallback: ex => logger.LogError(ex, "Telemetry error"));
trace.Start();
```

Attach the trace to an individual call:

```csharp
connection.QueryAll<Customer>(trace: trace);
```

Or register it so it applies to every operation across the app:

```csharp
GlobalConfiguration
    .Setup(new GlobalConfigurationOptions { UseRegisteredGlobalTraces = true });

GlobalTraceRegistration.Register(trace);
```

## Custom Publishing

To send telemetry somewhere other than the default HTTP collector, implement `IPublisherRepository` and use it from your own `ITrace` implementation (following the same pattern as `TelemetryTrace`):

```csharp
public class MyPublisherRepository : IPublisherRepository
{
    public void Publish(TelemetryItem telemetryItem) { /* send to your sink */ }
    public void PublishAsync(TelemetryItem telemetryItem, CancellationToken cancellationToken = default) { /* ... */ }
    public void PublishMany(IEnumerable<TelemetryItem> telemetryItems) { /* ... */ }
    public void PublishManyAsync(IEnumerable<TelemetryItem> telemetryItems, CancellationToken cancellationToken = default) { /* ... */ }
}
```
