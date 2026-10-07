```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9457)
Intel Core Ultra 7 255HX 2.40GHz, 1 CPU, 20 logical and 20 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| ORM    | Method            | Rows | Mean     | StdDev    | Error      | Min      | Max      | Iterations | Gen0   | Allocated |
|------- |------------------ |----- |---------:|----------:|-----------:|---------:|---------:|-----------:|-------:|----------:|
| RepoDB | ExecuteQueryFirst | 1    | 2.149 ms | 0.0032 ms |  1.4589 ms | 2.147 ms | 2.151 ms |      2.000 | 2.0000 |  46.44 KB |
| RepoDB | QueryLinqFirst    | 1    | 2.153 ms | 0.0003 ms |  0.1490 ms | 2.153 ms | 2.153 ms |      2.000 | 2.0000 |  50.35 KB |
| RepoDB | QueryObjectsFirst | 1    | 2.188 ms | 0.0918 ms | 41.3314 ms | 2.123 ms | 2.253 ms |      2.000 | 2.0000 |  49.56 KB |
| RepoDB | QueryDynamicFirst | 1    | 2.202 ms | 0.0559 ms | 25.1461 ms | 2.162 ms | 2.241 ms |      2.000 | 2.0000 |  49.52 KB |
| Dapper | QueryLinqFirst    | 1    | 2.235 ms | 0.0274 ms | 12.3232 ms | 2.216 ms | 2.255 ms |      2.000 | 4.0000 |  81.07 KB |
| Dapper | QueryFirst        | 1    | 2.251 ms | 0.0622 ms | 28.0163 ms | 2.207 ms | 2.295 ms |      2.000 | 4.0000 |  81.35 KB |
