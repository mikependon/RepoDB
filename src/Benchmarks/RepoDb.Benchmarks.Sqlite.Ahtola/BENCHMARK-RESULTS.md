# Ahtola vs Microsoft.Data.Sqlite — RepoDB benchmark comparison

Providers compared:

| Project | ADO.NET driver | RepoDB provider |
|---|---|---|
| `RepoDb.Benchmarks.Sqlite.Microsoft` | `Microsoft.Data.Sqlite` (native `e_sqlite3`) | `RepoDb.Sqlite.Microsoft` |
| `RepoDb.Benchmarks.Ahtola` | `Devolutions.Ahtola.Data.Sqlite` 0.9.0 (`Local Provider=Managed`) | `RepoDb.Ahtola` |

Both use the same on-disk schema, seed data (5004 `Person` rows) and benchmark code.

## Environment

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200)
Intel Core Ultra 9 285HX 2.80GHz, 24 cores
.NET 10.0.12, X64 RyuJIT x86-64-v3
```

The default `ShortRun` job (UnrollFactor = 500) is impractical on Ahtola, so both runs override the job:

```powershell
# Microsoft.Data.Sqlite (RepoDB + Dapper), Ahtola (Dapper)
dotnet run -c Release -- --filter '*.RepoDb.*' '*.Dapper.*' --unrollFactor 1 --invocationCount 16 --iterationCount 10 --warmupCount 2 --join
# Ahtola (RepoDB) — lighter job because write benchmarks take seconds per op
dotnet run -c Release -- --filter '*.RepoDb.*' --unrollFactor 1 --invocationCount 4 --iterationCount 6 --warmupCount 1 --join
```

## Results (mean per operation)

| ORM | Method | Rows | Microsoft.Data.Sqlite | Ahtola | Ahtola / MDS |
|---|---|---:|---:|---:|---:|
| RepoDB | ExecuteQueryFirst | 1 | 23.5 µs | 1.85 ms | 79× |
| RepoDB | QueryObjectsFirst | 1 | 46.5 µs | 1.76 ms | 38× |
| RepoDB | QueryDynamicFirst | 1 | 47.1 µs | 1.75 ms | 37× |
| RepoDB | QueryLinqFirst | 1 | 58.7 µs | 1.41 ms | 24× |
| Dapper | QueryFirst | 1 | 31.6 µs | 2.34 ms | 74× |
| Dapper | QueryLinqFirst | 1 | 32.5 µs | 2.80 ms | 86× |
| RepoDB | QueryAll | 5004 | 4.30 ms | 8.71 ms | **2.0×** |
| RepoDB | ExecuteQueryAll | 5004 | 4.81 ms | 9.64 ms | **2.0×** |
| Dapper | GetAll | 5004 | 5.08 ms | 1,588 ms | 313× |
| Dapper | QueryAll | 5004 | 5.32 ms | 1,728 ms | 325× |
| RepoDB | InsertAll | 10 | 2.57 ms | 11.1 ms | 4.3× |
| RepoDB | InsertAll | 100 | 3.85 ms | 73.3 ms | 19× |
| RepoDB | InsertAll | 1000 | 5.70 ms | 3,202 ms | 561× |
| RepoDB | UpdateAll | 10 | 0.101 ms | 168 ms | 1,665× |
| RepoDB | UpdateAll | 100 | 0.373 ms | 1,339 ms | 3,589× |
| RepoDB | UpdateAll | 1000 | 3.75 ms | 12,594 ms | 3,362× |
| Dapper | ExecuteUpdateAll | 10 | 0.259 ms | 538 ms | 2,080× |
| Dapper | ExecuteUpdateAll | 100 | 2.53 ms | 6,042 ms | 2,388× |
| Dapper | ExecuteUpdateAll | 1000 | 24.3 ms | ~51,000 ms¹ | ~2,100× |

¹ Warmup measurement only; the full run would have taken over 2 hours.

Allocations follow the same pattern. For example, RepoDB `UpdateAll(1000)` allocates **17.9 GB** on Ahtola vs **2.1 MB** on MDS, and `QueryAll(5004)` allocates 7.2 MB vs 2.3 MB.

## Analysis

* **Bulk reads through RepoDB are the best case (~2× slower).** RepoDB compiles one typed reader
  per result shape, so it calls `GetFieldType` once per query and then uses typed getters
  (`GetInt64`, `GetString`, ...). Ahtola's typed getters are fast.
* **Dapper bulk reads are ~300× slower** because Dapper reads every cell through
  `IDataRecord.GetValue`. On Ahtola 0.9.0, `GetValue` and `GetFieldType` cost about 300–400 µs per call,
  versus about 1 µs for the typed getters (measured with a raw ADO.NET probe).
* **Single-row queries are 25–85× slower**, dominated by the fixed cost of opening a connection
  (about 17 ms the first time, then roughly 1.3–1.8 ms per open/query/close cycle in the benchmark loop) plus one
  `GetFieldType` round per column.
* **Writes are the weakest area (hundreds to thousands × slower).** Each Dapper update runs as its own
  auto-committed statement, costing about 54 ms per commit on Ahtola versus about 25 µs on MDS. RepoDB batches statements,
  which helps for small batches. However, write cost grows super-linearly with batch size, and so do the very
  large allocation volumes, which points to per-statement or per-page copying inside the managed engine.

## Takeaways

* `RepoDb.Ahtola` is **functionally complete**: 680/680 integration tests and 103/103 unit tests pass.
* For **read-mostly workloads through RepoDB's typed materializer**, Ahtola is within about 2× of native SQLite.
* For **write-heavy workloads**, or code paths that use `GetValue` (Dapper, `ExecuteReader` +
  `GetValue`, `dynamic` results), Ahtola 0.9.0 is orders of magnitude slower. Improving
  `GetValue`/`GetFieldType` and commit and write throughput in Ahtola would close most of the gap.
