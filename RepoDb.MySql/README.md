[![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)
[![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlDL](https://img.shields.io/nuget/dt/repodb.mysql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?label=unit&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)
[![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?label=integration&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)

## RepoDb.MySql - a hybrid .NET ORM library for MySql.

This is the official repository for **RepoDb.MySql** solution.

## What is with this library

- It has all the functionalities of [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) impelentation.
- It has batch operations; optimized the execution of multiple ***Insert***, ***Merge*** and ***Update***.
- It is a unique and hybrid solution for ***MySql*** data-provider within ***.NET Technology***.
- It is well-covered by Unit and Integration Tests.

## Installation

At the ***Package Manager Console***, write the command below.

```
Install-Package RepoDb.MySql
```

## Getting Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.MySqlBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

### Query

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

### Insert

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new MySqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

Please refer to RepoDb [GitHub](https://github.com/mikependon/RepoDb) page for further information.