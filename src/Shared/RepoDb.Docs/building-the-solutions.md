# Building the Solutions

In this page, we will guide you on how to build the RepoDB solutions.

- [Prerequisites](#prerequisites)
- [Clone the Repository](#clone-the-repository)
- [Starting a Database via Docker](#starting-a-database-via-docker)
- [RepoDb.Core](#building-the-repodbcore)
- [RepoDb.ClickHouse](#building-the-repodbclickhouse)
- [RepoDb.ClickHouse.BulkOperations](#building-the-repodbclickhousebulkoperations)
- [RepoDb.Db2](#building-the-repodbdb2)
- [RepoDb.Db2.BulkOperations](#building-the-repodbdb2bulkoperations)
- [RepoDb.EnterpriseDb](#building-the-repodbenterprisedb)
- [RepoDb.Firebird](#building-the-repodbfirebird)
- [RepoDb.Firebird.BulkOperations](#building-the-repodbfirebirdbulkoperations)
- [RepoDb.MariaDb](#building-the-repodbmariadb)
- [RepoDb.MariaDb.BulkOperations](#building-the-repodbmariadbbulkoperations)
- [RepoDb.MariaDbConnector](#building-the-repodbmariadbconnector)
- [RepoDb.MariaDbConnector.BulkOperations](#building-the-repodbmariadbconnectorbulkoperations)
- [RepoDb.MySql](#building-the-repodbmysql)
- [RepoDb.MySql.BulkOperations](#building-the-repodbmysqlbulkoperations)
- [RepoDb.MySqlConnector](#building-the-repodbmysqlconnector)
- [RepoDb.MySqlConnector.BulkOperations](#building-the-repodbmysqlconnectorbulkoperations)
- [RepoDb.Oracle](#building-the-repodboracle)
- [RepoDb.Oracle.BulkOperations](#building-the-repodboraclebulkoperations)
- [RepoDb.PostgreSql](#building-the-repodbpostgresql)
- [RepoDb.PostgreSql.BulkOperations](#building-the-repodbpostgresqlbulkoperations)
- [RepoDb.SapHana](#building-the-repodbsaphana)
- [RepoDb.SapHana.BulkOperations](#building-the-repodbsaphanabulkoperations)
- [RepoDb.SqlServer](#building-the-repodbsqlserver)
- [RepoDb.SqlServer.BulkOperations](#building-the-repodbsqlserverbulkoperations)
- [RepoDb.Sqlite.Microsoft](#building-the-repodbsqlitemicrosoft)
- [RepoDb.Vertica](#building-the-repodbvertica)
- [RepoDb.Vertica.BulkOperations](#building-the-repodbverticabulkoperations)

## Prerequisites

- **Git** - follow this [guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git) to install it.
- **.NET SDK** - the solutions target everything from `netstandard2.0` up to `.NET 10`; install the latest SDK from [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).
- **Docker** - most provider solutions below run their integration tests against a database started via the [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root; install [Docker](https://www.docker.com/) if you don't already have it.

## Clone the Repository

```
> mkdir c:\src
> cd c:\src
> git clone https://github.com/mikependon/RepoDB.git
```

## Starting a Database via Docker

Every provider except SQLite has a matching service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root, all sharing the password `RepoDB2026`. Start only the service(s) you need:

```
> cd c:\src\RepoDB
> docker compose up -d <service>
```

Each provider section below names its own `<service>` and the port/credentials it exposes.

## Building the [RepoDb.Core](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Core)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Core
> dotnet build RepoDb.sln -v n
```

#### Pre-requisites

RepoDb.Core's integration tests run against SQL Server. Start the `mssql` service (see [Starting a Database via Docker](#starting-a-database-via-docker)):

```
> docker compose up -d mssql
```

This exposes the `sa` user (password `RepoDB2026`) on port `1433`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Core/RepoDb.Tests/RepoDb.IntegrationTests)

Start the `mssql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mssql
```

Add the environment variables under `System`.

- REPODB_SQLSVR_CONSTR_MASTER = `Server=tcp:127.0.0.1,1433;Database=master;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;`
- REPODB_SQLSVR_CONSTR = `Server=tcp:127.0.0.1,1433;Database=RepoDb;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests
> dotnet build RepoDb.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Core/RepoDb.Tests/RepoDb.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests
> dotnet build RepoDb.UnitTests.csproj -v n
```

Execute the unit tests.

```
> dotnet test RepoDb.UnitTests.csproj -v n
```

## Building the [RepoDb.ClickHouse](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.ClickHouse)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.ClickHouse
> dotnet build RepoDb.ClickHouse.sln -v n
```

#### Pre-requisites

```
> docker compose up -d clickhouse
```

This exposes the `default` user (password `RepoDB2026`) over HTTP on port `8123`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.ClickHouse.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.ClickHouse/RepoDb.ClickHouse.IntegrationTests)

Start the `clickhouse` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d clickhouse
```

Add the environment variables under `System`.

- REPODB_CLICKHOUSE_CONSTR_SYSTEM = `Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=default;Protocol=http;UseCustomDecimals=false;`
- REPODB_CLICKHOUSE_CONSTR = `Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=RepoDb;Protocol=http;UseCustomDecimals=false;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.ClickHouse\RepoDb.ClickHouse.IntegrationTests
> dotnet build RepoDb.ClickHouse.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.ClickHouse.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.ClickHouse.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.ClickHouse/RepoDb.ClickHouse.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.ClickHouse\RepoDb.ClickHouse.UnitTests
> dotnet build RepoDb.ClickHouse.UnitTests.csproj -v n
> dotnet test RepoDb.ClickHouse.UnitTests.csproj -v n
```

## Building the [RepoDb.ClickHouse.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.ClickHouse.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.ClickHouse.BulkOperations
> dotnet build RepoDb.ClickHouse.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `clickhouse` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.ClickHouse.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.ClickHouse.BulkOperations/RepoDb.ClickHouse.BulkOperations.IntegrationTests)

Start the `clickhouse` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d clickhouse
```

Add the environment variables under `System`.

- REPODB_CLICKHOUSE_CONSTR_SYSTEM = `Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=default;Protocol=http;UseCustomDecimals=false;`
- REPODB_CLICKHOUSE_CONSTR = `Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=RepoDb;Protocol=http;UseCustomDecimals=false;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.ClickHouse.BulkOperations\RepoDb.ClickHouse.BulkOperations.IntegrationTests
> dotnet build RepoDb.ClickHouse.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.ClickHouse.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.Db2](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Db2)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Db2
> dotnet build RepoDb.Db2.sln -v n
```

#### Pre-requisites

```
> docker compose up -d db2
```

This exposes the `db2inst1` user (password `RepoDB2026`) against the `REPODB` database on port `50000`, matching the default connection string used by the tests below. Make sure the connection string includes `HostVarParameters=True;`, as `RepoDb.Db2` binds parameters using `:Name`-style host variables which the IBM.Data.Db2 driver does not recognize by default.

#### Building and executing the [RepoDb.Db2.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Db2/RepoDb.Db2.IntegrationTests)

Start the `db2` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d db2
```

Add the environment variable under `System`.

- REPODB_Db2_CONSTR = `Server=localhost:50000;Database=REPODB;UID=db2inst1;PWD=RepoDB2026;HostVarParameters=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Db2\RepoDb.Db2.IntegrationTests
> dotnet build RepoDb.Db2.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Db2.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.Db2.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Db2/RepoDb.Db2.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Db2\RepoDb.Db2.UnitTests
> dotnet build RepoDb.Db2.UnitTests.csproj -v n
> dotnet test RepoDb.Db2.UnitTests.csproj -v n
```

## Building the [RepoDb.Db2.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Db2.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Db2.BulkOperations
> dotnet build RepoDb.Db2.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `db2` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.Db2.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Db2.BulkOperations/RepoDb.Db2.BulkOperations.IntegrationTests)

Start the `db2` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d db2
```

Add the environment variable under `System`.

- REPODB_Db2_CONSTR = `Server=localhost:50000;Database=REPODB;UID=db2inst1;PWD=RepoDB2026;HostVarParameters=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Db2.BulkOperations\RepoDb.Db2.BulkOperations.IntegrationTests
> dotnet build RepoDb.Db2.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Db2.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.EnterpriseDb](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.EnterpriseDb)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.EnterpriseDb
> dotnet build RepoDb.EnterpriseDb.sln -v n
```

#### Pre-requisites

The `enterprisedb` image requires an EDB subscription. Log in first, then start the service:

```
> docker login docker.enterprisedb.com --username k8s --password <EDB_SUBSCRIPTION_TOKEN>
> docker compose up -d enterprisedb
```

This exposes the `enterprisedb` user (password `RepoDB2026`) against the `edb` database on port `5444`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.EnterpriseDb.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.EnterpriseDb/RepoDb.EnterpriseDb.IntegrationTests)

Start the `enterprisedb` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker login docker.enterprisedb.com --username k8s --password <EDB_SUBSCRIPTION_TOKEN>
> docker compose up -d enterprisedb
```

Add the environment variables under `System`.

- REPODB_EDB_CONSTR_SYSTEM = `Server=127.0.0.1;Port=5444;Database=edb;User Id=enterprisedb;Password=RepoDB2026;`
- REPODB_EDB_CONSTR = `Server=127.0.0.1;Port=5444;Database=RepoDb;User Id=enterprisedb;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.EnterpriseDb\RepoDb.EnterpriseDb.IntegrationTests
> dotnet build RepoDb.EnterpriseDb.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.EnterpriseDb.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.EnterpriseDb.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.EnterpriseDb/RepoDb.EnterpriseDb.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.EnterpriseDb\RepoDb.EnterpriseDb.UnitTests
> dotnet build RepoDb.EnterpriseDb.UnitTests.csproj -v n
> dotnet test RepoDb.EnterpriseDb.UnitTests.csproj -v n
```

> RepoDb.EnterpriseDb has no separate `.BulkOperations` package.

## Building the [RepoDb.Firebird](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Firebird)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Firebird
> dotnet build RepoDb.Firebird.sln -v n
```

#### Pre-requisites

```
> docker compose up -d firebird
```

This exposes the `SYSDBA` user (password `RepoDB2026`) against `repodb.fdb` on port `3050`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.Firebird.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Firebird/RepoDb.Firebird.IntegrationTests)

Start the `firebird` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d firebird
```

Add the environment variable under `System`.

- REPODB_FIREBIRD_CONSTR = `DataSource=127.0.0.1;Port=3050;Database=/firebird/data/repodb.fdb;User=SYSDBA;Password=RepoDB2026;Charset=UTF8;Pooling=false;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Firebird\RepoDb.Firebird.IntegrationTests
> dotnet build RepoDb.Firebird.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Firebird.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.Firebird.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Firebird/RepoDb.Firebird.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Firebird\RepoDb.Firebird.UnitTests
> dotnet build RepoDb.Firebird.UnitTests.csproj -v n
> dotnet test RepoDb.Firebird.UnitTests.csproj -v n
```

## Building the [RepoDb.Firebird.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Firebird.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Firebird.BulkOperations
> dotnet build RepoDb.Firebird.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `firebird` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.Firebird.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Firebird.BulkOperations/RepoDb.Firebird.BulkOperations.IntegrationTests)

Start the `firebird` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d firebird
```

Add the environment variable under `System`.

- REPODB_FIREBIRD_CONSTR = `DataSource=127.0.0.1;Port=3050;Database=/firebird/data/repodb.fdb;User=SYSDBA;Password=RepoDB2026;Charset=UTF8;Pooling=false;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Firebird.BulkOperations\RepoDb.Firebird.BulkOperations.IntegrationTests
> dotnet build RepoDb.Firebird.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Firebird.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.MariaDb](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDb)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDb
> dotnet build RepoDb.MariaDb.sln -v n
```

#### Pre-requisites

```
> docker compose up -d mariadb
```

This exposes the `root` user (password `RepoDB2026`) on host port `3307` (mapped from the container's `3306`), matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.MariaDb.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDb/RepoDb.MariaDb.IntegrationTests)

Start the `mariadb` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mariadb
```

Add the environment variables under `System`.

- REPODB_MARIADB_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3307;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MARIADB_CONSTR = `Server=127.0.0.1;Port=3307;Database=RepoDb;User ID=root;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDb\RepoDb.MariaDb.IntegrationTests
> dotnet build RepoDb.MariaDb.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MariaDb.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.MariaDb.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDb/RepoDb.MariaDb.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDb\RepoDb.MariaDb.UnitTests
> dotnet build RepoDb.MariaDb.UnitTests.csproj -v n
> dotnet test RepoDb.MariaDb.UnitTests.csproj -v n
```

## Building the [RepoDb.MariaDb.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDb.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDb.BulkOperations
> dotnet build RepoDb.MariaDb.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `mariadb` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.MariaDb.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDb.BulkOperations/RepoDb.MariaDb.BulkOperations.IntegrationTests)

Start the `mariadb` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mariadb
```

Add the environment variables under `System`.

- REPODB_MARIADB_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3307;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MARIADB_CONSTR = `Server=127.0.0.1;Port=3307;Database=RepoDb;User ID=root;Password=RepoDB2026;AllowLoadLocalInfile=True;AllowUserVariables=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDb.BulkOperations\RepoDb.MariaDb.BulkOperations.IntegrationTests
> dotnet build RepoDb.MariaDb.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MariaDb.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.MariaDbConnector](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDbConnector)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDbConnector
> dotnet build RepoDb.MariaDbConnector.sln -v n
```

#### Pre-requisites

`RepoDb.MariaDbConnector` targets the same MariaDB server as `RepoDb.MariaDb` (just through the `MySqlConnector` driver instead of `MySqlData`). Start the `mariadb` service as described above.

> Please ignore this pre-requisite if you have done it already in a prior section.

#### Building and executing the [RepoDb.MariaDbConnector.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDbConnector/RepoDb.MariaDbConnector.IntegrationTests)

Start the `mariadb` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mariadb
```

Add the environment variables under `System`.

- REPODB_MARIADB_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3307;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MARIADB_CONSTR = `Server=127.0.0.1;Port=3307;Database=RepoDb;User ID=root;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDbConnector\RepoDb.MariaDbConnector.IntegrationTests
> dotnet build RepoDb.MariaDbConnector.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MariaDbConnector.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.MariaDbConnector.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDbConnector/RepoDb.MariaDbConnector.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDbConnector\RepoDb.MariaDbConnector.UnitTests
> dotnet build RepoDb.MariaDbConnector.UnitTests.csproj -v n
> dotnet test RepoDb.MariaDbConnector.UnitTests.csproj -v n
```

## Building the [RepoDb.MariaDbConnector.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDbConnector.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDbConnector.BulkOperations
> dotnet build RepoDb.MariaDbConnector.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `mariadb` service as described in a prior section.

> Please ignore this pre-requisite if you have done it already in a prior section.

#### Building and executing the [RepoDb.MariaDbConnector.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MariaDbConnector.BulkOperations/RepoDb.MariaDbConnector.BulkOperations.IntegrationTests)

Start the `mariadb` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mariadb
```

Add the environment variables under `System`.

- REPODB_MARIADB_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3307;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MARIADB_CONSTR = `Server=127.0.0.1;Port=3307;Database=RepoDb;User ID=root;Password=RepoDB2026;AllowLoadLocalInfile=True;AllowUserVariables=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MariaDbConnector.BulkOperations\RepoDb.MariaDbConnector.BulkOperations.IntegrationTests
> dotnet build RepoDb.MariaDbConnector.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MariaDbConnector.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.MySql](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySql)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySql
> dotnet build RepoDb.MySql.sln -v n
```

#### Pre-requisites

```
> docker compose up -d mysql
```

This exposes the `root` user (password `RepoDB2026`) on port `3306`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.MySql.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySql/RepoDb.MySql.IntegrationTests)

Start the `mysql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mysql
```

Add the environment variables under `System`.

- REPODB_MYSQL_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3306;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MYSQL_CONSTR = `Server=127.0.0.1;Port=3306;Database=RepoDb;User ID=root;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySql\RepoDb.MySql.IntegrationTests
> dotnet build RepoDb.MySql.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MySql.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.MySql.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySql/RepoDb.MySql.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySql\RepoDb.MySql.UnitTests
> dotnet build RepoDb.MySql.UnitTests.csproj -v n
> dotnet test RepoDb.MySql.UnitTests.csproj -v n
```

## Building the [RepoDb.MySql.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySql.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySql.BulkOperations
> dotnet build RepoDb.MySql.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `mysql` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.MySql.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySql.BulkOperations/RepoDb.MySql.BulkOperations.IntegrationTests)

Start the `mysql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mysql
```

Add the environment variables under `System`.

- REPODB_MYSQL_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3306;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MYSQL_CONSTR = `Server=127.0.0.1;Port=3306;Database=RepoDb;User ID=root;Password=RepoDB2026;AllowLoadLocalInfile=True;AllowUserVariables=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySql.BulkOperations\RepoDb.MySql.BulkOperations.IntegrationTests
> dotnet build RepoDb.MySql.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MySql.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.MySqlConnector](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySqlConnector)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySqlConnector
> dotnet build RepoDb.MySqlConnector.sln -v n
```

#### Pre-requisites

`RepoDb.MySqlConnector` targets the same MySQL server as `RepoDb.MySql` (just through the `MySqlConnector` driver instead of `MySql.Data`). Start the `mysql` service as described above.

> Please ignore this pre-requisite if you have done it already in a prior section.

#### Building and executing the [RepoDb.MySqlConnector.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySqlConnector/RepoDb.MySqlConnector.IntegrationTests)

Start the `mysql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mysql
```

Add the environment variables under `System`.

- REPODB_MYSQL_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3306;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MYSQL_CONSTR = `Server=127.0.0.1;Port=3306;Database=RepoDb;User ID=root;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySqlConnector\RepoDb.MySqlConnector.IntegrationTests
> dotnet build RepoDb.MySqlConnector.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MySqlConnector.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.MySqlConnector.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySqlConnector/RepoDb.MySqlConnector.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySqlConnector\RepoDb.MySqlConnector.UnitTests
> dotnet build RepoDb.MySqlConnector.UnitTests.csproj -v n
> dotnet test RepoDb.MySqlConnector.UnitTests.csproj -v n
```

## Building the [RepoDb.MySqlConnector.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySqlConnector.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySqlConnector.BulkOperations
> dotnet build RepoDb.MySqlConnector.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `mysql` service as described in a prior section.

> Please ignore this pre-requisite if you have done it already in a prior section.

#### Building and executing the [RepoDb.MySqlConnector.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.MySqlConnector.BulkOperations/RepoDb.MySqlConnector.BulkOperations.IntegrationTests)

Start the `mysql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mysql
```

Add the environment variables under `System`.

- REPODB_MYSQL_CONSTR_SYSTEM = `Server=127.0.0.1;Port=3306;Database=sys;User ID=root;Password=RepoDB2026;`
- REPODB_MYSQL_CONSTR = `Server=127.0.0.1;Port=3306;Database=RepoDb;User ID=root;Password=RepoDB2026;AllowLoadLocalInfile=True;AllowUserVariables=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.MySqlConnector.BulkOperations\RepoDb.MySqlConnector.BulkOperations.IntegrationTests
> dotnet build RepoDb.MySqlConnector.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.MySqlConnector.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.Oracle](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Oracle)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Oracle
> dotnet build RepoDb.Oracle.sln -v n
```

#### Pre-requisites

```
> docker compose up -d oracle
```

This exposes the `system` user (password `RepoDB2026`) against the `FREEPDB1` pluggable database on port `1521`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.Oracle.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Oracle/RepoDb.Oracle.IntegrationTests)

Start the `oracle` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d oracle
```

Add the environment variable under `System`.

- REPODB_ORACLE_CONSTR = `User Id=system;Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Oracle\RepoDb.Oracle.IntegrationTests
> dotnet build RepoDb.Oracle.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Oracle.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.Oracle.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Oracle/RepoDb.Oracle.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Oracle\RepoDb.Oracle.UnitTests
> dotnet build RepoDb.Oracle.UnitTests.csproj -v n
> dotnet test RepoDb.Oracle.UnitTests.csproj -v n
```

## Building the [RepoDb.Oracle.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Oracle.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Oracle.BulkOperations
> dotnet build RepoDb.Oracle.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `oracle` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.Oracle.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Oracle.BulkOperations/RepoDb.Oracle.BulkOperations.IntegrationTests)

Start the `oracle` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d oracle
```

Add the environment variables under `System`.

- REPODB_ORACLE_CONSTR = `User Id=system;Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;`
- REPODB_ORACLE_CONSTR_BULK = `User Id=system;Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Oracle.BulkOperations\RepoDb.Oracle.BulkOperations.IntegrationTests
> dotnet build RepoDb.Oracle.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Oracle.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.PostgreSql](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.PostgreSql)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.PostgreSql
> dotnet build RepoDb.PostgreSql.sln -v n
```

#### Pre-requisites

```
> docker compose up -d postgresql
```

This exposes the `postgres` user (password `RepoDB2026`) on port `5432`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.PostgreSql.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.PostgreSql/RepoDb.PostgreSql.IntegrationTests)

Start the `postgresql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d postgresql
```

Add the environment variables under `System`.

- REPODB_PGSQL_CONSTR_POSTGRES = `Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=RepoDB2026;`
- REPODB_PGSQL_CONSTR = `Server=127.0.0.1;Port=5432;Database=RepoDb;User Id=postgres;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.PostgreSql\RepoDb.PostgreSql.IntegrationTests
> dotnet build RepoDb.PostgreSql.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.PostgreSql.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.PostgreSql.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.PostgreSql/RepoDb.PostgreSql.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.PostgreSql\RepoDb.PostgreSql.UnitTests
> dotnet build RepoDb.PostgreSql.UnitTests.csproj -v n
> dotnet test RepoDb.PostgreSql.UnitTests.csproj -v n
```

## Building the [RepoDb.PostgreSql.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.PostgreSql.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.PostgreSql.BulkOperations
> dotnet build RepoDb.PostgreSql.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `postgresql` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.PostgreSql.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.PostgreSql.BulkOperations/RepoDb.PostgreSql.BulkOperations.IntegrationTests)

Start the `postgresql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d postgresql
```

Add the environment variables under `System`.

- REPODB_PGSQL_CONSTR_POSTGRES = `Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=RepoDB2026;`
- REPODB_PGSQL_CONSTR_BULK = `Server=127.0.0.1;Port=5432;Database=RepoDbBulk;User Id=postgres;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.PostgreSql.BulkOperations\RepoDb.PostgreSql.BulkOperations.IntegrationTests
> dotnet build RepoDb.PostgreSql.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.PostgreSql.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.SapHana](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SapHana)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SapHana
> dotnet build RepoDb.SapHana.sln -v n
```

#### Pre-requisites

```
> docker compose up -d saphana
```

Connect to port `39041` - the HANA Express tenant ("HXE") database's own SQL port - rather than `39013` (`SYSTEMDB`), which redirects clients to the tenant using the container's internal Docker-network address that isn't reachable from the host. This exposes the `SYSTEM` user (password `RepoDB2026`), matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.SapHana.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SapHana/RepoDb.SapHana.IntegrationTests)

Start the `saphana` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d saphana
```

Add the environment variable under `System`.

- REPODB_SAPHANA_CONSTR = `Server=localhost:39041;UserID=SYSTEM;Password=RepoDB2026;Current Schema=REPODB;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SapHana\RepoDb.SapHana.IntegrationTests
> dotnet build RepoDb.SapHana.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SapHana.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.SapHana.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SapHana/RepoDb.SapHana.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SapHana\RepoDb.SapHana.UnitTests
> dotnet build RepoDb.SapHana.UnitTests.csproj -v n
> dotnet test RepoDb.SapHana.UnitTests.csproj -v n
```

## Building the [RepoDb.SapHana.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SapHana.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SapHana.BulkOperations
> dotnet build RepoDb.SapHana.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `saphana` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.SapHana.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SapHana.BulkOperations/RepoDb.SapHana.BulkOperations.IntegrationTests)

Start the `saphana` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d saphana
```

Add the environment variables under `System`.

- REPODB_SAPHANA_CONSTR_BULK = `Server=localhost:39041;UserID=SYSTEM;Password=RepoDB2026;Current Schema=REPODB;`
- REPODB_SAPHANA_CONSTR = `Server=localhost:39041;UserID=SYSTEM;Password=RepoDB2026;Current Schema=REPODB;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SapHana.BulkOperations\RepoDb.SapHana.BulkOperations.IntegrationTests
> dotnet build RepoDb.SapHana.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SapHana.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.SqlServer](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SqlServer)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SqlServer
> dotnet build RepoDb.SqlServer.sln -v n
```

#### Pre-requisites

```
> docker compose up -d mssql
```

This exposes the `sa` user (password `RepoDB2026`) on port `1433`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.SqlServer.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SqlServer/RepoDb.SqlServer.IntegrationTests)

Start the `mssql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mssql
```

Add the environment variables under `System`.

- REPODB_SQLSVR_CONSTR_MASTER = `Server=tcp:127.0.0.1,1433;Database=master;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;`
- REPODB_SQLSVR_CONSTR = `Server=tcp:127.0.0.1,1433;Database=RepoDb;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SqlServer\RepoDb.SqlServer.IntegrationTests
> dotnet build RepoDb.SqlServer.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SqlServer.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.SqlServer.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SqlServer/RepoDb.SqlServer.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SqlServer\RepoDb.SqlServer.UnitTests
> dotnet build RepoDb.SqlServer.UnitTests.csproj -v n
> dotnet test RepoDb.SqlServer.UnitTests.csproj -v n
```

## Building the [RepoDb.SqlServer.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SqlServer.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SqlServer.BulkOperations
> dotnet build RepoDb.SqlServer.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `mssql` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.SqlServer.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.SqlServer.BulkOperations/RepoDb.SqlServer.BulkOperations.IntegrationTests)

Start the `mssql` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d mssql
```

Add the environment variables under `System`.

- REPODB_SQLSVR_CONSTR_MASTER = `Server=tcp:127.0.0.1,1433;Database=master;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;`
- REPODB_SQLSVR_CONSTR_BULK = `Server=tcp:127.0.0.1,1433;Database=RepoDbBulk;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.SqlServer.BulkOperations\RepoDb.SqlServer.BulkOperations.IntegrationTests
> dotnet build RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.Sqlite.Microsoft](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Microsoft)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Sqlite.Microsoft
> dotnet build RepoDb.Sqlite.Microsoft.sln -v n
```

#### Pre-requisites

None - SQLite needs no Docker service or separate install. By default the tests use a local file at `C:\SqLite\Databases\RepoDb.db` (create that folder first), or run entirely in-memory if `REPODB_SQLITE_IS_IN_MEMORY` is set.

#### Building and executing the [RepoDb.Sqlite.Microsoft.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Microsoft/RepoDb.Sqlite.Microsoft.IntegrationTests)

Add the environment variable under `System`.

- REPODB_SQLITE_IS_IN_MEMORY = `TRUE`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Sqlite.Microsoft\RepoDb.Sqlite.Microsoft.IntegrationTests
> dotnet build RepoDb.Sqlite.Microsoft.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Sqlite.Microsoft.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.Sqlite.Microsoft.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Microsoft/RepoDb.Sqlite.Microsoft.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Sqlite.Microsoft\RepoDb.Sqlite.Microsoft.UnitTests
> dotnet build RepoDb.Sqlite.Microsoft.UnitTests.csproj -v n
> dotnet test RepoDb.Sqlite.Microsoft.UnitTests.csproj -v n
```

> RepoDb.Sqlite.Microsoft has no separate `.BulkOperations` package.

## Building the [RepoDb.Vertica](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Vertica)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Vertica
> dotnet build RepoDb.Vertica.sln -v n
```

#### Pre-requisites

```
> docker compose up -d vertica
```

This exposes the `dbadmin` user (password `RepoDB2026`) against the `RepoDb` database on port `5433`, matching the default connection string used by the tests below.

#### Building and executing the [RepoDb.Vertica.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Vertica/RepoDb.Vertica.IntegrationTests)

Start the `vertica` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d vertica
```

Add the environment variable under `System`.

- REPODB_VERTICA_CONSTR = `Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Vertica\RepoDb.Vertica.IntegrationTests
> dotnet build RepoDb.Vertica.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Vertica.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.Vertica.UnitTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Vertica/RepoDb.Vertica.UnitTests)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Vertica\RepoDb.Vertica.UnitTests
> dotnet build RepoDb.Vertica.UnitTests.csproj -v n
> dotnet test RepoDb.Vertica.UnitTests.csproj -v n
```

## Building the [RepoDb.Vertica.BulkOperations](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Vertica.BulkOperations)

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Vertica.BulkOperations
> dotnet build RepoDb.Vertica.BulkOperations.sln -v n
```

#### Pre-requisites

Start the `vertica` service as described in the prior section.

> Please ignore this pre-requisite if you have done it already in the prior section.

#### Building and executing the [RepoDb.Vertica.BulkOperations.IntegrationTests](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Vertica.BulkOperations/RepoDb.Vertica.BulkOperations.IntegrationTests)

Start the `vertica` service defined in [docker-compose.yml](https://github.com/mikependon/RepoDB/blob/master/docker-compose.yml) at the repository root (skip if already running).

```
> docker compose up -d vertica
```

Add the environment variable under `System`.

- REPODB_VERTICA_CONSTR = `Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;Pooling=false;`

Build the integration tests.

```
> cd c:\src\RepoDB\src\Providers\RepoDb.Vertica.BulkOperations\RepoDb.Vertica.BulkOperations.IntegrationTests
> dotnet build RepoDb.Vertica.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.Vertica.BulkOperations.IntegrationTests.csproj -v n
```
