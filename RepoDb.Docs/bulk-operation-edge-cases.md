# Edge Case on Bulk Operations

RepoDb is addressing a very important edge-case that many ORM in .NET space does not provide.

## Parameters

All the bulk operations are flexible to accept different kind of paramters.
 
#### DataReader

```csharp
using (var sourceConnection = new OracleConnection("OracleConnectionString"))
{
	using (var reader = sourceConnection.ExecuteReader("SELECT * FROM Person"))
	{
		using (var destinationConnection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
		{
			var rowsInserted = connection.BulkInsert(people);
		}
	}
}
```

#### DataTable

Assuming the `ConvertToDataTable()` method is existing.

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(ConvertToDataTable(people));
}
```

#### IEnumerable&lt;T&gt;

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(ConvertToDataTable(people));
}
```

## Identity Columns

Both the [BulkInsert](https://repodb.net/operation/bulkinsert) and [BulkMerge](https://repodb.net/operation/bulkmerge) operations are specialized in a way of bulk-processing the data and still be able to retrieve the identities back to the caller.

This is a very important common-case to everyone. Imagine bulk inserting huge datasets and have all the identities referenced back to your application.

To enable this, simply set the `isReturnIdentity` argument to `true` when calling the operations.

#### BulkInsert

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people, isReturnIdentity: true);
}
```

#### BulkMerge

```csharp
var people = GetPeople();
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkMerge(people, isReturnIdentity: true);
}
```

## Multi-Threading

This is a scenario where you would like to process a big dataset and you decided to execute those in parallel without affecting the other scenarios and behaviors.

In RepoDb, when doing a bulk operation, a pseudo-temporary table is being created as a stepping stone to bring data first to the database. Then, use this pseudo-temporary table to promote the changes back to the target table.

You can leverage the underlying RDBMS capability by specifying whether to use the TEMP pseudo-temporary table or the physical pseudo-temporary table. To do this, simply set the `usePhysicalPseudoTempTable` argument to `true`.

#### BulkInsert

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people, usePhysicalPseudoTempTable: true);
}
```

#### BulkMerge

```csharp
var people = GetPeople();
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkMerge(people, usePhysicalPseudoTempTable: true);
}
```

**Note:** Use the TEMP pseudo-temporary table if you are working in multi-threading environment. Otherwise, always use the physical pseudo-temporary table to maximize the advantages of having the physical table.