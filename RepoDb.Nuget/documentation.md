### Stable Release

RepoDb stable release is at version **1.1.0**.

### Snippets

Write less optimal codes.
```
var repository = new DbRepository<SqlConnection>(connectionString);
var people = repository.Query<Person>(new
{
	LastName = { Operation = Operation.Like, Value = "Ander%" }
},
top: 100,
orderBy: OrderField.Parse(new
{
	FirstName = Order.Ascending,
	BirtDate = Order.Descending
});
```

### Documentation
Click [here](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Documents/documentation_release_v1.md) for the documentation.

### Goal

We aim to let .NET developers limit the implementation of SQL Statements within the application. We believe that as a .NET developer, one should only focus on .NET and Business scenario development and not on writing repetitive SQL Statements. Ofcourse, unless you are working closely with SQL Server Management Studio.

### Notes

 - We will keep it simple as possible (KISS principle)
 - We will make it fast as possible
 - We will never make complex implementations (specially for Queries and Methods)
 - We will avoid developing complex JOINs (until it is needed and requested by the community)
 - We will never ever do try-catch inside the library

### Features

 - Transaction
 - Asynchronous Operation
 - Type Mapping
 - Field Mapping
 - Multiple Mapping
 - Expression Tree
 - Caching
 - Tracing
 - SQL Statement Builder

### Operations

 - BatchQuery
 - BatchQueryAsync
 - Count
 - CountAsync
 - CountBig
 - CountBigAsync
 - Query
 - QueryAsync
 - Insert
 - InsertAsync
 - Delete
 - DeleteAsync
 - Update
 - UpdateAsync
 - InlineUpdate
 - InlineUpdateAsync
 - Merge
 - MergeAsync
 - BulkInsert
 - BulkInsertAsync
 - ExecuteReader
 - ExecuteReaderAsync
 - ExecuteQuery
 - ExecuteQueryAsync
 - ExecuteNonQuery
 - ExecuteNonQueryAsync
 - ExecuteScalar
 - ExecuteScalarAsync