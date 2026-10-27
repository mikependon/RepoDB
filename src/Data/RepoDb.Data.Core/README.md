<div align="center">
    <a href="https://repodb.net/tutorial/"><image src="RepoDbCore.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/"><strong>RepoDb.Data.Core</strong></a></span>
    <br/>
    <span style="font-size:16px;">The data productivity core package of RepoDB library.</span>
</div>


## Copy progress

`CopyDataTo` and `CopyDataToAsync` count the selected source tables before copying any rows.
The requested table's filter also applies to its count; related tables are counted without that filter.
After each table is counted, the callback receives the accumulated `TotalRowCount`. During this
phase, `Current` and `CurrentBatch` are null, `Items` and `Logs` are empty, and copied rows and bytes are zero.
`TableCount` is already populated. The same progress instance is reused for all callbacks.

After copying begins, `RowCount` reports the cumulative rows sent from successful batches, while
`TotalRowCount` remains the initial source count. `Current` exposes the current table's
`TotalRowCount`, `RowCount`, and `CopiedBytes`; each batch also exposes `CopiedBytes`.
Source changes after counting can cause the final copied count to differ from the initial total.

```csharp
source.CopyDataTo("Person", destination, progressCallback: progress =>
{
    if (progress.Current == null)
    {
        Console.WriteLine($"Counting source rows: {progress.TotalRowCount}");
        return;
    }

    Console.WriteLine($"{progress.RowCount}/{progress.TotalRowCount} rows, " +
        $"approximately {progress.CopiedBytes} bytes copied");
});
```

`CopiedBytes` is a cumulative estimate of the copied values after mappings, interceptors, and
identity exclusions. It includes string characters (two bytes each), binary data, and supported
scalar values. Nulls and unsupported provider-specific values contribute zero. It excludes
object/container overhead and database wire encoding, and is not the current buffer allocation.
Only one batch is retained at a time.

Copying never queries destination row counts. `Target.RowCount` and `SkippedRowCount` remain
null; a provider that silently skips rows cannot be checked through these in-memory counters.
