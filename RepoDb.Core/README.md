[![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ek0nw?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)
[![CoreVersion](https://img.shields.io/nuget/v/RepoDb?style=flat-square)](https://www.nuget.org/packages/RepoDb)
[![CoreDL](https://img.shields.io/nuget/dt/repodb?style=flat-square)](https://www.nuget.org/packages/RepoDb)
[![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-yf1cx?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx/build/tests)
[![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qksas?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-qksas/build/tests)

## RepoDb - a hybrid ORM library for .NET.

This is the official repository for **RepoDb** solution.

## Installation

At the ***Package Manager Console***, write the command below.

```
Install-Package RepoDb
```

## Getting Started

After the installation, any library operation can then be called.

### Query

```csharp
using (var connection = new SqlConnection(ConnectionString))
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
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

Please refer to RepoDb [GitHub](https://github.com/mikependon/RepoDb) page for further information.