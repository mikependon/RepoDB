# 🏢 RepoDb.Benchmarks.SapHana

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, and Linq2Db on SAP HANA. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on SAP HANA, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for SAP HANA workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own SAP HANA configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

> ⚠️ Like [RepoDb.SapHana](../../../Providers/RepoDb.SapHana) itself, this benchmark has not been verified against a live SAP HANA instance. Every query, driver, and dialect choice below is a documented, best-effort mapping rather than something run and confirmed end to end — please report back if you exercise it against a real instance.

## 🐝 NHibernate is not included

NHibernate does ship SAP HANA dialects (`HanaColumnStoreDialect`/`HanaRowStoreDialect`) and drivers (`HanaColumnStoreDriver`/`HanaRowStoreDriver`), but both driver classes hardcode their ADO.NET provider assembly to the legacy, Windows/.NET-Framework-only `Sap.Data.Hana.v4.5.dll` (with native `libadonetHDB.dll`/`libSQLDBCHDB.dll` dependencies) — see [`HanaDriverBase`](https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Driver/HanaDriverBase.cs). That assembly name doesn't match, and can't be swapped for, the modern `Sap.Data.Hana.Net.v6.0` package this benchmark (and [RepoDb.SapHana](../../../Providers/RepoDb.SapHana)) targets for `net10.0`. Functionally, that leaves NHibernate with no working SAP HANA driver for this benchmark's target framework, the same outcome as [RepoDb.Benchmarks.MariaDb](../RepoDb.Benchmarks.MariaDb)/[RepoDb.Benchmarks.ClickHouse](../RepoDb.Benchmarks.ClickHouse) excluding it for a missing dialect — so it's left out here too rather than wired up against a driver that can't load.

## ▶️ Running the Benchmark

1. Start a SAP HANA instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d saphana
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.SapHana
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Server=127.0.0.1:39041;UserID=SYSTEM;Password=RepoDB2026;Current Schema=REPODB;
```

Port `39041` is the HANA Express tenant ("HXE") database's own SQL port - connecting there directly is required in a Docker setup like this repo's `docker-compose.yml`: the SYSTEMDB port (`39013`) redirects clients to the tenant using the container's internal Docker-network address, which isn't reachable from the host. The benchmark creates the `REPODB` schema itself (over a connection string with the `Current Schema` clause stripped) if it doesn't already exist.

To target a different instance or credentials, set this environment variable before running:

```bash
export REPODB_CONSTR="Server=<your-host>:39041;UserID=SYSTEM;Password=<your-password>;Current Schema=REPODB;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [Sap.Data.Hana.Net.v6.0](https://www.nuget.org/packages/Sap.Data.Hana.Net.v6.0), SAP's own .NET Core ADO.NET driver, throughout — including [RepoDb.SapHana](../../../Providers/RepoDb.SapHana), [RepoDb.SapHana.BulkOperations](../../../Providers/RepoDb.SapHana.BulkOperations), and Linq2Db (via `SapHanaTools.GetDataProvider(SapHanaProvider.Unmanaged, ...)` — linq2db has no fluent `DataOptions.UseSapHana()` helper the way it does for MySQL/ClickHouse, so the provider is resolved explicitly instead; `SapHanaProviderAdapter.UnmanagedAssemblyNames` lists `Sap.Data.Hana.Net.v6.0` by name for `net6.0`+ builds). Entity Framework Core uses [Sap.EntityFrameworkCore.Hana.v10.0](https://www.nuget.org/packages/Sap.EntityFrameworkCore.Hana.v10.0), SAP's own official EF Core 10 provider, also built on `Sap.Data.Hana`.

### A couple of things specific to SAP HANA

- **The EF Core package needs an `extern alias`.** `Sap.EntityFrameworkCore.Hana.v10.0` declares zero NuGet dependencies and bundles its own private copy of the `Sap.Data.Hana` ADO.NET driver directly in its `lib` folder (as `Sap.Data.Hana.Net.v10.0.dll`, alongside its own provider and NetTopologySuite DLLs) instead of depending on the standalone driver package - which collides (`CS0433`, ambiguous type) with the `Sap.Data.Hana.Net.v6.0` this project already gets transitively through [RepoDb.SapHana](../../../Providers/RepoDb.SapHana). The [csproj](RepoDb.Benchmarks.SapHana.csproj) aliases that PackageReference (`Aliases="SapEfCore"`) so its bundled driver copy stays hidden behind `extern alias SapEfCore;` in [EFCore/Models/EFCoreContext.cs](EFCore/Models/EFCoreContext.cs) - the only file that needs anything from that package - rather than colliding with the `Sap.Data.Hana.Net.v6.0` types used everywhere else in this project. `Microsoft.EntityFrameworkCore.Relational` (the package that actually owns `ToTable`/`FromSqlRaw`) also has to be referenced explicitly for the same reason: the SAP package doesn't pull it in.
- **Every identifier is quoted and case-sensitive.** Like Vertica and ClickHouse, HANA folds unquoted identifiers to uppercase, so the `"Person"` table and all of its columns are created with quoted mixed case and referenced that way everywhere - raw SQL, and Dapper.Contrib's `[Table("\"Person\"")]` (in this project's own [Models/Person.cs](Models/Person.cs), used only by the Dapper benchmarks — RepoDb and EF Core apply their own provider-aware quoting over the shared [Core Person model](../RepoDb.Benchmarks.Core/Models/Person.cs) instead).
- **Parameters are colon-prefixed (`:Id`), not `@Id`.** `RepoDb.SapHana`'s own `SapHanaDbSetting.ParameterPrefix` is `":"`, matching the underlying `Sap.Data.Hana` ADO.NET driver, so all of the raw SQL in the Dapper and RepoDb benchmarks uses `:Name` placeholders instead of the `@Name` form used by MySQL/SQL Server/PostgreSQL benchmarks in this suite.
- **No `CREATE TABLE IF NOT EXISTS`.** HANA rejects that syntax outright, so [Setup/DatabaseHelper.cs](Setup/DatabaseHelper.cs) checks `SYS.TABLES` for the table's existence before creating it, the same way [RepoDb.SapHana.IntegrationTests](../../../Providers/RepoDb.SapHana/RepoDb.SapHana.IntegrationTests) does.
- **The target schema must exist before HANA will even open a connection against it.** `DatabaseHelper` connects once without the `Current Schema` clause to check `SYS.SCHEMAS` and create the schema if missing, then reconnects with the real connection string for everything else.
