```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9168/25H2/2025Update/HudsonValley2)
Intel Core Ultra 7 255HX 2.40GHz, 1 CPU, 20 logical and 20 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| ORM        | Method            | Rows | Mean      | StdDev    | Error     | Min      | Max       | Gen0     | Gen1     | Gen2     | Allocated |
|----------- |------------------ |----- |----------:|----------:|----------:|---------:|----------:|---------:|---------:|---------:|----------:|
| Dapper     | GetAll            | 5004 |  4.956 ms | 1.0671 ms | 1.6133 ms | 3.446 ms |  6.224 ms | 136.0000 |  88.0000 |        - |   2.03 MB |
| Linq2db    | SelectAll         | 5004 |  4.959 ms | 0.4163 ms | 0.6293 ms | 4.296 ms |  5.685 ms | 106.0000 |        - |        - |   1.61 MB |
| RepoDB     | ExecuteQueryAll   | 5004 |  5.056 ms | 0.5122 ms | 0.7744 ms | 4.161 ms |  5.806 ms | 112.0000 |  94.0000 |        - |   1.69 MB |
| Dapper     | QueryAll          | 5004 |  5.134 ms | 0.5355 ms | 0.8999 ms | 4.326 ms |  6.033 ms | 136.0000 | 112.0000 |        - |   2.03 MB |
| RepoDB     | QueryAll          | 5004 |  5.172 ms | 0.6466 ms | 0.9775 ms | 4.543 ms |  6.290 ms | 112.0000 |  94.0000 |        - |   1.69 MB |
| EFCore     | NoTrackingGetAll  | 5004 |  5.187 ms | 0.6705 ms | 1.0136 ms | 4.252 ms |  6.394 ms | 168.0000 |  12.0000 |        - |   2.53 MB |
| NHibernate | CreateSQLQueryAll | 5004 |  5.219 ms | 1.4000 ms | 2.1166 ms | 3.570 ms |  7.644 ms | 194.0000 | 142.0000 |        - |   2.92 MB |
| NHibernate | QueryAll          | 5004 | 10.095 ms | 0.7987 ms | 1.2075 ms | 9.062 ms | 11.522 ms | 444.0000 | 442.0000 | 222.0000 |   5.77 MB |
| EFCore     | FromSqlRawGetAll  | 5004 | 13.071 ms | 2.8964 ms | 4.3790 ms | 8.873 ms | 18.124 ms | 438.0000 | 436.0000 | 172.0000 |   5.19 MB |
