# RepoDb.Telemetry.Extended

Extended telemetry capture for RepoDB, delivered separately from `RepoDb.Telemetry.Default`.
`ExtendedTelemetryItem` inherits `DefaultTelemetryItem` and adds severity, a message,
thread and distributed tracing identifiers, database metadata, row counts, and custom properties.

## Configuration

```csharp
using RepoDb.Telemetry.Extended;
using System.Data;

GlobalConfiguration.Setup().UseExtendedTelemetry(new ExtendedTelemetryOption("PayrollSystem")
{
    Host = "http://localhost:5000",
    ApiKey = "collector-api-key",
    Enrich = (item, log) =>
    {
        item.ConnectionName = "Northwind (SQL Server)";
        item.Database = "Northwind";
        item.CommandType = CommandType.Text;
        item.Properties["Operation"] = log.Key;
    }
});
```

Global configuration registers one singleton; later calls retain its original settings.
Use `new ExtendedTelemetryTrace(option)` when an independent trace instance is needed.
Do not register Default telemetry as well unless duplicate events are intended.

## Captured fields

The shared fields retain their existing names: `StartTime`, `Operation`, and `Elapsed`
represent time, event, and duration in milliseconds.

The library captures the context of an operation when it starts, and the trace copies it:
`ThreadId`, `ParameterCount`, the current W3C Activity's `TraceId` and `SpanId`, the machine
name (`Client`) and its `IpAddress`, the type of the connection (`ConnectionType`, i.e.
`SqlConnection`), and its `Database`. `TraceId` identifies the whole operation across services,
and `SpanId` identifies one step within it. A span is not created for the database operation,
so the operations that run within the same Activity share both identifiers; use `SessionId`
to tell them apart. They remain null when no W3C Activity is active.

Collection results expose `RowsReturned` without enumeration. Numeric results from
known write operations expose `RowsAffected`. Scalar values and deferred sequences
leave row counts unknown. Parameter values are not collected.

`Level` defaults to Serilog's `Information`, and is `Error` when the operation fails.
`Message`, `ConnectionName` (the friendly name that the application gives to the connection),
`ConnectionType`, `Database`, `CommandType`, and `Table` can be supplied or replaced through
`Enrich`. SQL is not parsed to guess command metadata. Enrichment runs on the calling thread
before execution; callbacks should be fast and not throw. Connection names should exclude
credentials. Warning classification and messages are caller supplied; elapsed duration is
populated during publishing.

When an operation fails, the trace is still told that it has completed, and the exception is
thrown back to the caller. The item records `ErrorType`, `ErrorMessage`, and `ErrorStackTrace`,
sets `Level` to `Error`, uses the error message as `Message` unless the application gave one,
and leaves `RowsAffected` and `RowsReturned` unknown. The `Execute*` extension methods of
`IDbConnection` (`ExecuteNonQuery`, `ExecuteScalar`, `ExecuteReader`, `ExecuteQuery`,
`ExecuteQueryFirst`, `ExecuteQuerySingle` and `ExecuteQueryMultiple`, sync and async) report
failures. `ExecuteReader` reports only the failure to execute, not one raised while the caller reads the rows.

`Properties` supports string, Boolean, int, long, double, decimal, DateTime, Guid,
and JsonElement values using source-generated JSON metadata. Represent other custom
values as JsonElement for trimming and NativeAOT compatibility. Severity and command
type enums are serialized numerically, following the existing JSON conventions.

## Collector protocol

Requests use `POST /v1/telemetry/extended/publish` with an optional `X-API-Key` header.
The body is a Base64-encoded gzip-compressed JSON array, matching the Default transport.
The collector must implement this endpoint and accept the extended schema; this
package does not add a server endpoint. Publishing failures reach the configured
error callback and optional logger.

## Build and test

The library targets .NET Standard 2.0 and .NET 8, 9, and 10. Tests target .NET 8, 9,
and 10 and include real loopback HTTP capture-to-publish checks without an external
database or collector.

```powershell
dotnet build RepoDb.Telemetry.Extended.sln
dotnet test --project RepoDb.Telemetry.Extended.UnitTests/RepoDb.Telemetry.Extended.UnitTests.csproj
```
