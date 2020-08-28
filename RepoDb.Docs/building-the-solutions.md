# Building the Solutions

In this page, we will guide you on how to build the RepoDB Solutions.

- [RepoDb.Core](#building-the-repodbcore)
- [RepoDb.SqlServer](#building-the-repodbsqlserver)
- [RepoDb.SqlServer.BulkOperations](#building-the-repodbsqlserverbulkoperations)
- [RepoDb.SqLite](#building-the-repodbsqlite)
- [RepoDb.MySql](#building-the-repodbmysql)
- [RepoDb.MySqlConnector](#building-the-repodbmysqlconnector)
- [RepoDb.PostgreSql](#building-the-repodbpostgresql)

## Install the Git

To install the [Git](https://git-scm.com/), please follow this [guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Clone the Repository

```
> mkdir c:\src
> cd c:\src
> git clone https://github.com/mikependon/RepoDb.git
```

## Building the [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)

```
> cd c:\src\RepoDb\RepoDb.Core
> dotnet build RepoDb.Core.sln -v n
```

#### Building and executing the [RepoDb.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_MASTER = `master_connection_string`
- REPODB_CONSTR = `repodb_connection_string`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests
> dotnet build RepoDb.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests
> dotnet test RepoDb.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests
> dotnet build RepoDb.UnitTests.csproj -v n
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests
> dotnet test RepoDb.UnitTests.csproj -v n
```

## Building the [RepoDb.SqlServer](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer)

```
> cd c:\src\RepoDb\RepoDb.SqlServer
> dotnet build RepoDb.SqlServer.sln -v n
```

#### Building and executing the [RepoDb.SqlServer.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer/RepoDb.SqlServer.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_MASTER = `master_connection_string`
- REPODB_CONSTR = `repodb_connection_string`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.SqlServer\RepoDb.SqlServer.IntegrationTests
> dotnet build RepoDb.SqlServer.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SqlServer.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.SqlServer.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer/RepoDb.SqlServer.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.SqlServer\RepoDb.SqlServer.UnitTests
> dotnet build RepoDb.SqlServer.UnitTests.csproj -v n
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.SqlServer\RepoDb.SqlServer.UnitTests
> dotnet test RepoDb.UnitTests.csproj -v n
```

## Building the [RepoDb.SqlServer.BulkOperations](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations)

```
> cd c:\src\RepoDb\RepoDb.Extensions\RepoDb.SqlServer.BulkOperations
> dotnet build RepoDb.SqlServer.BulkOperations.sln -v n
```

#### Building and executing the [RepoDb.SqlServer.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer/RepoDb.SqlServer.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_MASTER = `master_connection_string`
- REPODB_CONSTR = `repodb_connection_string`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Extensions\RepoDb.SqlServer.BulkOperations\RepoDb.SqlServer.BulkOperations.IntegrationTests
> dotnet build RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Extensions\RepoDb.SqlServer.BulkOperations\RepoDb.SqlServer.BulkOperations.IntegrationTests
> dotnet test RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.SqLite](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite)

```
> cd c:\src\RepoDb\RepoDb.SqLite
> dotnet build RepoDb.SqLite.sln -v n
```

#### Pre-requisites

First, install the [SqLite](https://www.sqlite.org/) by downloading the package [here](https://www.sqlite.org/download.html).

> Open the `SQLiteStudio` and create a database named `RepoDB`.

#### Building and executing the [RepoDb.SqLite.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite/RepoDb.SqLite.IntegrationTests)

Add the environment variables under `System`.

- REPODB_IS_IN_MEMORY = `TRUE`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.SqLite\RepoDb.SqLite.IntegrationTests
> dotnet build RepoDb.SqLite.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> cd c:\src\RepoDb\RepoDb.SqLite\RepoDb.SqLite.IntegrationTests
> dotnet test RepoDb.SqLite.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.SqLite.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite/RepoDb.SqLite.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.SqLite\RepoDb.SqLite.UnitTests
> dotnet build RepoDb.SqLite.UnitTests.csproj -v n
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.SqLite\RepoDb.SqLite.UnitTests
> dotnet test RepoDb.SqLite.UnitTests.csproj -v n
```

## Building the [RepoDb.MySql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql)

```
> cd c:\src\RepoDb\RepoDb.MySql
> dotnet build RepoDb.MySql.sln -v n
```

#### Pre-requisites

First, install the [MySql](https://www.mysql.com) by downloading the package [here](https://dev.mysql.com/downloads/installer/).

#### Building and executing the [RepoDb.MySql.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.MySql.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_SYS = `sys_connection_string`
- REPODB_CONSTR = `repodb_connection_string`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.MySql\RepoDb.MySql.IntegrationTests
> dotnet build RepoDb.MySql.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> cd c:\src\RepoDb\RepoDb.MySql\RepoDb.MySql.IntegrationTests
> dotnet test RepoDb.MySql.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.MySql.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql/RepoDb.MySql.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.MySql\RepoDb.MySql.UnitTests
> dotnet build RepoDb.MySql.UnitTests.csproj -v n
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.MySql\RepoDb.MySql.UnitTests
> dotnet test RepoDb.MySql.UnitTests.csproj -v n
```

## Building the [RepoDb.MySqlConnector](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySqlConnector)

```
> cd c:\src\RepoDb\RepoDb.MySqlConnector
> dotnet build RepoDb.MySqlConnector.sln -v n
```

#### Pre-requisites

First, install the [MySql](https://www.mysql.com) by downloading the package [here](https://dev.mysql.com/downloads/installer/).

> Please ignore this pre-requisites if you have done it already in the prior section.

#### Building and executing the [RepoDb.MySqlConnector.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySqlConnector/RepoDb.MySqlConnector.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_SYS = `sys_connection_string`
- REPODB_CONSTR = `repodb_connection_string`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.MySqlConnector\RepoDb.MySqlConnector.IntegrationTests
> dotnet build RepoDb.MySqlConnector.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> cd c:\src\RepoDb\RepoDb.MySqlConnector\RepoDb.MySqlConnector.IntegrationTests
> dotnet test RepoDb.MySqlConnector.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.MySqlConnector.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySqlConnector/RepoDb.MySqlConnector.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.MySqlConnector\RepoDb.MySqlConnector.UnitTests
> dotnet build RepoDb.MySqlConnector.UnitTests.csproj -v n
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.MySqlConnector\RepoDb.MySqlConnector.UnitTests
> dotnet test RepoDb.MySqlConnector.UnitTests.csproj -v n
```

## Building the [RepoDb.PostgreSql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql)

```
> cd c:\src\RepoDb\RepoDb.PostgreSql
> dotnet build RepoDb.PostgreSql.sln -v n
```

#### Pre-requisites

First, install the [PostgreSql](https://www.postgresql.org/) by following this [tutorial](https://www.postgresqltutorial.com/install-postgresql/).

#### Building and executing the [RepoDb.PostgreSql.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql/RepoDb.PostgreSql.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_POSTGRESDB = `postgres_connection_string`
- REPODB_CONSTR = `repodb_connection_string`

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.PostgreSql\RepoDb.PostgreSql.IntegrationTests
> dotnet build RepoDb.PostgreSql.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> cd c:\src\RepoDb\RepoDb.PostgreSql\RepoDb.PostgreSql.IntegrationTests
> dotnet test RepoDb.PostgreSql.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.PostgreSql.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql/RepoDb.PostgreSql.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.PostgreSql\RepoDb.PostgreSql.UnitTests
> dotnet build RepoDb.PostgreSql.UnitTests.csproj -v n
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.PostgreSql\RepoDb.PostgreSql.UnitTests
> dotnet test RepoDb.PostgreSql.UnitTests.csproj -v n
```
