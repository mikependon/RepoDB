```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 255HX 2.40GHz, 1 CPU, 20 logical and 20 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Dry      : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| ORM    | Method           | Rows | Mean      | StdDev    | Error     | Min       | Max       | Iterations | Gen0     | Gen1     | Allocated  |
|------- |----------------- |----- |----------:|----------:|----------:|----------:|----------:|-----------:|---------:|---------:|-----------:|
| Dapper | ExecuteUpdateAll | 10   |        NA |        NA |        NA |        NA |        NA |         NA |       NA |       NA |         NA |
| Dapper | QueryFirst       | 1    |  9.102 ms | 0.0465 ms | 0.0782 ms |  9.019 ms |  9.151 ms |      9.000 |        - |        - |    5.01 KB |
| Dapper | GetAll           | 5004 |  9.594 ms | 0.0517 ms | 0.0870 ms |  9.499 ms |  9.678 ms |      9.000 | 136.0000 | 104.0000 | 2087.87 KB |
| Dapper | QueryAll         | 5004 |  9.664 ms | 0.1155 ms | 0.1747 ms |  9.493 ms |  9.857 ms |     10.000 | 136.0000 | 104.0000 | 2087.83 KB |
| Dapper | QueryLinqFirst   | 1    |  9.780 ms | 0.0720 ms | 0.1089 ms |  9.717 ms |  9.912 ms |     10.000 | 136.0000 |  98.0000 | 2087.98 KB |
| Dapper | QueryAll         | 5004 | 14.848 ms | 0.0000 ms |        NA | 14.848 ms | 14.848 ms |      1.000 |        - |        - | 2089.23 KB |
| Dapper | QueryFirst       | 1    | 24.647 ms | 0.0000 ms |        NA | 24.647 ms | 24.647 ms |      1.000 |        - |        - |    6.59 KB |
| Dapper | QueryLinqFirst   | 1    | 24.659 ms | 0.0000 ms |        NA | 24.659 ms | 24.659 ms |      1.000 |        - |        - | 2089.35 KB |
| Dapper | GetAll           | 5004 | 50.605 ms | 0.0000 ms |        NA | 50.605 ms | 50.605 ms |      1.000 |        - |        - | 2089.23 KB |

Benchmarks with issues:
  UpdateAllDapperBenchmarks.ExecuteUpdateAll: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [Rows=10]
