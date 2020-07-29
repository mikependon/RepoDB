# RepoDb Limitations

We are frank and direct on our intention to the .NET community. RepoDb is a micro-ORM that you may consider have some advance features built from advance concepts/thingking. But, on top of these things, we are also having some limitations that may not work at all use-cases.

This page may not be the source of truth of all as the other use-cases may not yet discovered. We will update this page to futher once we gathered the use-cases that are not supported. This page will also answer some of the FAQs towards this library.

## Support to JOIN Query

## Clustered Primary Keys

The default support to this will never be implemented as RepoDb tend to sided the other scenario that is completely oppositing and defaultly eliminateing this use-case.

When you do the push operation in RepoD (i.e.: [Insert](https://repodb.net/operation/insert), [Delete](https://repodb.net/operation/delete), [Update](https://repodb.net/operation/update), [Merge](https://repodb.net/operation/merge) and etc), it uses the PK as the qualifiers.

So when you do save a row like below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var id = connection.Insert<Person>(new Person { Name = "John Doe" });
}
```

The return value is the ID of that row, whether the ID column is an identity or not. There, we cannot return the value of the Clustered PK. This is also true for the other operations, specially for the [Bulk Operations](https://repodb.net/feature/bulkoperations).

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

## State Tracking
