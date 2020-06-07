## Building the Solutions

In this page, we will guide you on how to build the RepoDb Solutions.

- [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)
- [RepoDb.SqlServer](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqlServer)
- [RepoDb.SqlServer.BulkOperations](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations)
- [RepoDb.SqLite](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite)
- [RepoDb.MySql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql)
- [RepoDb.MySqlConnector](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySqlConnector)
- [RepoDb.PostgreSql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.PostgreSql)

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

- REPODB_CONSTR_MASTER = <master_connection_string>
- REPODB_CONSTR = <repodb_connection_string>

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests
> dotnet build RepoDb.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
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

- REPODB_CONSTR_MASTER = <master_connection_string>
- REPODB_CONSTR = <repodb_connection_string>

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.SqlServer\RepoDb.SqlServer.IntegrationTests
> dotnet build RepoDb.SqlServer.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SqlServer.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.SqlServer.UnitTests](C:\Users\MichaelP\Source\Repos\GitHub\RepoDb\RepoDb.SqlServer\RepoDb.SqlServer.UnitTests)

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

- REPODB_CONSTR_MASTER = <master_connection_string>
- REPODB_CONSTR = <repodb_connection_string>

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Extensions\RepoDb.SqlServer.BulkOperations\RepoDb.SqlServer.BulkOperations.IntegrationTests
> dotnet build RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SqlServer.BulkOperations.IntegrationTests.csproj -v n
```

## Building the [RepoDb.SqLite](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite)

```
> cd c:\src\RepoDb\RepoDb.SqLite
> dotnet build RepoDb.SqLite.sln -v n
```

#### Pre-requisites

First, install the [SqLite](https://www.sqlite.org/) by downloading the package [here](https://www.sqlite.org/download.html).

Open the `SQLiteStudio` and create a database named `RepoDb`.

#### Building and executing the [RepoDb.SqLite.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite/RepoDb.SqLite.IntegrationTests)

Add the environment variables under `System`.

- REPODB_IS_IN_MEMORY = <true>

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.SqLite\RepoDb.SqLite.IntegrationTests
> dotnet build RepoDb.SqLite.IntegrationTests.csproj -v n
```

Execute the integration tests.

```
> dotnet test RepoDb.SqLite.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.SqLite.UnitTests](C:\Users\MichaelP\Source\Repos\GitHub\RepoDb\RepoDb.SqLite\RepoDb.SqLite.UnitTests)

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
