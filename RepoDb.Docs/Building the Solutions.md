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
> dotnet build RepoDb.Core.sln
```

#### Building and executing the [RepoDb.IntegrationTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.IntegrationTests)

Add the environment variables under `System`.

- REPODB_CONSTR_MASTER = <master_connection_string>
- REPODB_CONSTR = <repodb_connection_string>

Build the integration tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.IntegrationTests
> dotnet build RepoDb.IntegrationTests.csproj
```

Execute the integration tests.

```
> dotnet test RepoDb.IntegrationTests.csproj -v n
```

#### Building and executing the [RepoDb.UnitTests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.UnitTests)

Build the unit tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests
> dotnet build RepoDb.UnitTests.csproj
```

Execute the unit tests.

```
> cd c:\src\RepoDb\RepoDb.Core\RepoDb.Tests\RepoDb.UnitTests
> dotnet test RepoDb.UnitTests.csproj -v n
```
