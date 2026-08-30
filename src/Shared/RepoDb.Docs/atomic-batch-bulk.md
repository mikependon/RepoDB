# The Atomic, Batch and Bulk Operations

RepoDB supports the different set of operations by default. With these operations, you can maximize the different ways of implementation to make your application more performant and efficient.

## Atomic Operation

This operation refers to a single minute execution to accomplish the job. In most cases, if your dataset is small, then an atomic execution is much faster and optimal.

To be specific, if you had created a list of Person and wish to save it in your database.

```csharp
var people = CreatePeople(30);
```

Then you have to iterate it like below, embedded with a Transaction object.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	using (var transaction = connection.EnsureOpen().BeginTransaction())
	{
		people.ForEach(p => connection.Insert(p, transaction: transaction));
		transaction.Commit();
	}
}
```

The operations of like [Insert](https://repodb.net/operation/insert), [Update](https://repodb.net/operation/update), [Delete](https://repodb.net/operation/delete) and [Merge](https://repodb.net/operation/merge) are all atomic.

## Batch Operation

This operation refers to a single execution of multiple command texts. Imagine executing the 10 INSERT statements in one-go. It allows you to control the number of rows to be processed against the database.

By using this operation, you are able to optimize the execution in response to the following situations.

- Network Latency (On-Premise, Cloud)
- Number of Columns
- Kind of Data (Blob, Plain Text, etc)
- Many More...

To be specific, if you had created a list of Person like below and wish to save it in your database.

```csharp
var people = CreatePeople(1000);
```

And you know that you can maximize the performance when sending 100 rows per batch, then you do it like below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.InsertAll(people, batchSize: 100);
}
```

By default, the execution is wrapped within a Transaction object to make it ACID. The operations of like [InsertAll](https://repodb.net/operation/insertall), [UpdateAll](https://repodb.net/operation/updateall) and [MergeAll](https://repodb.net/operation/mergeall) are all packed-executions.

## Bulk Operation

This operation refers to "a kind of execution" that process all the data at once. This operation is the most optimal operation to be used when processing huge datasets.

The drawback to this is that, it skips all the necessary checks of the underlying RDBMS data provider (i.e.: Logging, Auditing, Constraints and etc).

To be specific, if you had created a list of Person like below and wish to save it in your database.

```csharp
var people = CreatePeople(100000);
```

Then you can bulk process it with the code like below.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people);
}
```

By default, the execution is wrapped within a Transaction object. The operations of like [BulkInsert](https://repodb.net/operation/bulkinsert), [BulkUpdate](https://repodb.net/operation/bulkupdate), [BulkDelete](https://repodb.net/operation/bulkdelete) and [BulkMerge](https://repodb.net/operation/bulkmerge) are all bulk-operations.

## Repository Implementation

To make sure that your repository implementation can handle the smallest-to-the-biggest datasets, the mentioned methods above must properly be used.

We highly recommend to you to have your own standards of when to do the Batch operation. The only requirement is to have your magic number as a standard.

In our case, we used the range of 31-1000.  A list with 30 rows (or less) will be processed by Atomic operations, and a list with more than 1000 rows will be processed by Bulk operations.

See the sample code below for SaveAll.

```csharp
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
        var count = people.Count();
	using (var transaction = connection.EnsureOpen().BeginTransaction())
	{
                if (count <= 30)
                {
		        people.ForEach(p => connection.Insert(p, transaction: transaction));
		}
                else if (count > 30 && count <= 1000)
                {
		        connection.InsertAll(people, transaction: transaction);
		}
                else
                {
		        connection.BulkInsert(people, transaction: transaction);
		}
                transaction.Commit();
	}
}
```

