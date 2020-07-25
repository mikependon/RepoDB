# Atomic, Batch and Bulk Operations

RepoDb supports the different set of operations by default. With these operations, you can maximize the different ways of implementation to make your application more performant and efficient.

## Atomic Operations

This operation refers to a single minute execution to accomplish the job. In most cases, if your dataset is small, then an atomic execution is much faster and optimal.

To be specific, if you have created a list of Person and wish to save it into the database.

```csharp
var people = CreatePeople(30);
```

Then you are iterating it like below, embedded with transaction.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	using (var transaction = connection.EnsureOpen().BeginTransaction())
	{
		foreach (var person in people)
		{
			var id = connection.Insert(p, transaction: transaction);
			...
		}
		transaction.Commit();
	}
}
```

The operations of like [Insert](https://repodb.net/operation/insert), [Update](https://repodb.net/operation/update), [Delete](https://repodb.net/operation/delete) and [Merge](https://repodb.net/operation/merge) are all atomic.

## Batch Operations

This operations refers to a single execution of multiple command texts. It allows you to control the number of rows to be processed against the database.

By using this operation, you are able to optimize the execution in response to the following situations.

- Network Latency (On-Premise, Cloud)
- Number of Columns
- Kind of Data (Blob, Plain Text, etc)
- Many More...

To be specific, if you have created a list of Person like below and wish to save it to the database.

```csharp
var people = CreatePeople(1000);
```

And if you know that you can maximize the performance by sending 100 rows per batch. Then you do it like below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.InsertAll(people, batchSize: 100);
}
```

The execution is wrapped within the Transaction to make it more ACID.

The operations of like [InsertAll](https://repodb.net/operation/insertall), [UpdateAll](https://repodb.net/operation/updateall) and [MergeAll](https://repodb.net/operation/mergeall) are all packed-executions.

## Bulk Operations

This operation refers to a kind of execution that process all the data at once. This operation is the most optimal operation to be used when processing few thousand of rows (or even millions).

The drawback to this is that, it skips all the necessary checks of the underlying RDBMS data provider like logging, auditing and constraints.

To be specific, if you have created a list of Person like below and wish to save it to the database.

```csharp
var people = CreatePeople(100000);
```

Then you can bulk insert it like below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people);
}
```
