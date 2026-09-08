```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9168/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 255HX 2.40GHz, 1 CPU, 20 logical and 20 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| ORM     | Method          | Rows | Mean     | StdDev    | Error     | Min      | Max      | Iterations | Gen0     | Gen1     | Allocated |
|-------- |---------------- |----- |---------:|----------:|----------:|---------:|---------:|-----------:|---------:|---------:|----------:|
| Dapper  | QueryAll        | 5004 | 3.895 ms | 0.1224 ms | 0.2341 ms | 3.681 ms | 4.054 ms |      8.000 | 136.0000 | 112.0000 |   2.03 MB |
| Linq2db | SelectAll       | 5004 | 4.020 ms | 0.6356 ms | 0.9610 ms | 3.251 ms | 5.017 ms |     10.000 | 106.0000 |        - |   1.61 MB |
| Dapper  | GetAll          | 5004 | 4.105 ms | 0.2929 ms | 0.4428 ms | 3.786 ms | 4.636 ms |     10.000 | 136.0000 |  88.0000 |   2.03 MB |
| RepoDB  | QueryAll        | 5004 | 4.382 ms | 0.8691 ms | 1.4605 ms | 3.308 ms | 6.108 ms |      9.000 | 112.0000 |  94.0000 |   1.69 MB |
| RepoDB  | ExecuteQueryAll | 5004 | 4.753 ms | 0.7880 ms | 1.1913 ms | 3.837 ms | 6.132 ms |     10.000 | 112.0000 |  94.0000 |   1.69 MB |
