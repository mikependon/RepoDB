```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 255HX 2.40GHz, 1 CPU, 20 logical and 20 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Dry    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| ORM    | Method     | Rows | Mean | StdDev | Error | Min | Max | Iterations |
|------- |----------- |----- |-----:|-------:|------:|----:|----:|-----------:|
| Dapper | QueryFirst | 1    |   NA |     NA |    NA |  NA |  NA |         NA |

Benchmarks with issues:
  GetFirstDapperBenchmarks.QueryFirst: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [Rows=1]
