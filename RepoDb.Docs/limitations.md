# RepoDb Limitations

We are frank and direct on our intention to the .NET community. RepoDb is a micro-ORM that you may consider have some advance features built from advance concepts/thingking. But, on top of these things, we are also having some limitations that may not work at all use-cases.

**Disclaimer:** This page may not be the source of truth (as of writing this) as the other use-cases may not yet discovered. We will update this page even futher once we gathered the use-cases that cannot be supported. This page will also answer some of the FAQs towards this library.

## Topics Covered

- [Support to JOIN Query](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/limitations.md#support-to-join-query)
- [Clustered Primary Keys](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/limitations.md#clustered-primary-keys)
- [Computed Columns](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/limitations.md#computed-columns)
- [State Tracking](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/limitations.md#state-tracking)

## Support to JOIN Query

We understand the reality that without having a support to JOIN Query will somehow eliminate the coccepts of ORM in the library. The correct term maybe is Object-Mapper (OM) library, rather than Object/Relational Mapper (ORM) library. Though we consider RepoDb as ORM due to the fact of its flexible features. We tend to leave to the users on how will they implement the JOIN Query, on their own perusal.

We see that 99% of the problems of the RDBMS data providers are managing the relationships. These includes the constraints, delegations, cascading and many more. To maintain the robustness of the library and put the control to the users when doing the things, we purposely not support this feature (for now), up until we have a much better way of doing it ahead of any other existing libraries.

**Example**

You would like to retrieve the related data of the Supplier record.

Given with these classes.

```csharp
public class Address
{
    public int Id { get; set; }
    ...
}

public class Product
{
    public int Id { get; set; }
    ...
}

public class Warehouse
{
    public int Id { get; set; }
    ...
}

public class Supplier
{
    public int Id { get; set; }
    public IEnumerable<Address> Addresses { get; set; }
    public IEnumerable<Product> Products { get; set; }
    public IEnumerable<Warehouse> Warehouses { get; set; }
}
```

You write the code below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var supplier = connection
		.Query<Customer>(e => e.Name == "Amazon")
		.Include<Address>()
		.Include<Product>()
		.Include<Warehouse>();
}
```

And have these scripts executed by ORM.

```
> SELECT * FROM [Supplier] WHERE Name = 'Amazon';
> SELECT A.* FROM [Address] A INNER JOIN [Supplier] S ON S.Id = A.SupplierId WHERE A.Name = 'Amazon';
> SELECT P.* FROM [Product] P INNER JOIN [Supplier] S ON S.Id = P.SupplierId WHERE P.Name = 'Amazon';
> SELECT W.* FROM [Warehouse] W INNER JOIN [Warehouse] S ON S.Id = W.SupplierId WHERE A.Name = 'Amazon';
```

We do not want to control the implementation for now, but instead we leave it all to you. We do not know yet whether the solution of multiple execution is acceptable to the community with the use of CTE, LEFT JOIN, OUTER APPLY or whatever techniques.

**SkipQuery**

Though SkipQuery seems to be working in this case, solving the problem beyond N+1. Let us say you write the code below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var supplier = connection
		.Query<Customer, Address, Product, Warehouse>(e => e.Name == "Amazon")
		.SplitFor<Address>(e => e.Id)
		.SplitFor<Product>(e => e.Id)
		.SplitFor<Wharehouse>(e => e.Id)
}
```

That may execute this LEFT JOIN query from the database.

```csharp
> SELECT S.*
>	, A.*
>	, P.*
>	, W.*
> FROM [Supplier] S
> LEFT JOIN [Address] A ON A.SupplierId = S.Id
> LEFT JOIN [Product] P ON P.SupplierId = S.Id
> LEFT JOIN [Warehouse] W ON W.SupplierId = S.Id
> WHERE S.Name = 'Amazon';
```

Is still not the most optimal thing to do as it needed a lot of process on the data afterwards. Like grouping the main object and the other N+X values.

**Alternative Solution**

We tend to ask the community to use the [QueryMultiple](https://repodb.net/operation/querymultiple) operation to solve this.

```csharp
using (var connection = new SqlConnection(connectionString).EnsureOpen())
{
	var result = connection.QueryMultiple<Supplier, Address, Product, Warehouse>(s => s.Id == 10045,
		a => a.SupplierId == 10045,
		p => p.SupplierId == 10045,
		w => w.SupplierId == 10045);
	var supplier = result.Item1.FirstOrDefault();
	var addresses = result.Item2.AsList();
	var products = result.Item3.AsList();
	var warehouses = result.Item4.AsList();
	
	// Do the stuffs here
}
```

Or via the [ExecuteMultiple](https://repodb.net/operation/executequerymultiple) operation.

```csharp
using (var connection = new SqlConnection(connectionString).EnsureOpen())
{
	using (var result = connection.ExecuteQueryMultiple(@"SELECT * FROM [dbo].[Supplier] WHERE [Id] = @SupplierId;
		SELECT * FROM [dbo].[Address] WHERE SupplierId = @SupplierId;
		SELECT * FROM [dbo].[Product] WHERE SupplierId = @SupplierId;
		SELECT * FROM [dbo].[Warehouse] WHERE SupplierId = @SupplierId;"))
	{
		var supplier = result.Extract<Person>().FirstOrDefault();
		var addresses = result.Extract<Address>().AsList();
		var products = result.Extract<Product>().AsList();
		var warehouses = result.Extract<Warehouse>().AsList();
		
		// Do the stuffs here
	}
}
```

The good thing to this is controlled by you, and that is a very important case to us.

## Clustered Primary Keys

The default support to this will never be implemented as RepoDb tend to sided the other scenario that is eliminating this use-case. When you do the push operation in RepoDb (i.e.: [Insert](https://repodb.net/operation/insert), [Delete](https://repodb.net/operation/delete), [Update](https://repodb.net/operation/update), [Merge](https://repodb.net/operation/merge) and etc), it uses the PK as the qualifiers.

**Scenario 1 - Insert**

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var id = connection.Insert<Person>(new Person { Name = "John Doe" });
}
```

The return value is the ID of that row, whether the ID column is an identity (or not). The value of the Clustered PK cannot be returned. This is also true for the other operations, specially for the [Bulk Operations](https://repodb.net/feature/bulkoperations).

**Scenario 2 - Update**

Another example is for update operation, we tend to defaultly use the PK as the qualifier. See the code below

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Update<Person>(new Person { Name = "John Doe" }, 10045);
}
```

In which, the 10045 is pointed to a single column in the DB, which is the PK.

Therefore, the code below will fail if you have a clustered index in the Name and DateOfBirth column.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Update<Person>(new Person { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01"), Address = "New York" });
}
```

RepoDb will instead ask you to do it this way, targetting the Clustered PK as the qualifiers.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var person = new Person { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01"), Address = "New York" };
    var affectedRows = connection.Update(person, e => e.Name = person.Name && e.DateOfBirth = person.DateOfBirth);
}
```

**Scenario 3 - Delete**

Same goes the Update scenario, we use the PK as the default qualifier.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Delete<Person>(10045);
}
```

It is impossible to map the value if you have Clustered PK.

> There are lot of scenarios that makes RepoDb unusable for the use-case of having a table with Clustered PKs.

## Computed Columns


## State Tracking
