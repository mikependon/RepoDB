# Atomic, Batch and Bulk Operations

RepoDb supports the different sets of operations by default. With these operations, you can maximize the different ways of implementation to make your application more performant and efficient.

## Atomic Operations

This operation refers to a single minute execution to accomplish the job. To be specific, if you have a list of rows, then you are executing it against the database in an atomic way.

So, if you created a list of Person like below.

```csharp
var people = CreatePeople(30);
```

Then you execute it like below, embedded with transaction.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	using (var transaction = connection.EnsureOpen().BeginTransaction())
	{
		foreach (var person in people)
		{
			var id = connection.Insert(p, transaction: transaction);
		}
		transaction.Commit();
	}
}
```

#### Advantage

If your datasets is small, like lesser than 50, the atomic execution is much faster and optimal.

## Batch Operations

Advantage: Situation like Network, Number of Columns, Latency

## Bulk Operations

Advantage: Million of rows