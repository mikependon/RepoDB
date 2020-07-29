# RepoDb Limitations

We are frank and direct on our intention to the .NET community. RepoDb is a micro-ORM that you may consider have some advance features built from advance concepts/thingking. But, on top of these things, we are also having some limitations that may not work at all use-cases.

This page may not be the source of truth of all as the other use-cases may not yet discovered. We will update this page to futher once we gathered the use-cases that are not supported. This page will also answer some of the FAQs towards this library.

## Support to JOIN Query

We understand the reality that without having a support to JOIN Query will somehow eliminate the coccepts of ORM in the library. The correct term maybe is Object-Mapper (OM) library, rather than Object/Relational Mapper (ORM) library. Though we consider RepoDb as ORM due to the fact of its flexible feature. We tend to leave to the users how do they will implement the JOIN Query on their own perusal.

We see that 99% of the problems of the RDBMS data providers are managing relationships. These includes the constraints, delegations, cascading and many more. To maintain the robustness of the library and put the control to the users when doing the things, we purposely not support this feature "for now", up until we have a much better way of doing it ahead of any other existing libraries.

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
        .Query<Customer>(10045)
        .Include<Address>()
        .Include<Product>()
        .Include<Warehouse>();
}
```



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
