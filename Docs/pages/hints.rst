Hints
=====

This feature is to allow the caller to further optimize the execution of the query when the `Query` operation has been called.

Below is an example of how to do a query (dirty-read) of customers where name starts with `Joh`.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(c => c.Name.StartsWith("Joh"), hints: SqlTableHints.NoLock);
	}

The `hints` argument is used define a query-optimizer in the SQL Statement query. It is equivalent to the SQL Server query hints. The caller can also write its own hints via literal string. See below.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(c => c.Name.StartsWith("Joh"), hints: "WITH (NOLOCK)");
	}

Below is a scenario to query all the customers from the database that ignores all the data that are under different transactions and with maximizing the index named `NCIX_Customer$FirstName$LastName`.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(hints: "WITH (INDEX(NCIX_Customer$FirstName$LastName), READPAST)");
	}

A default class named `SqlTableHints` is provided to simplify the passing of the parameters. This class only contains the table hints for SQL Server.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(hints: SqlTableHints.NoLock);
		var customers = connection.Query<Customer>(hints: SqlTableHints.ReadPast);
		var customers = connection.Query<Customer>(hints: SqlTableHints.ReadCommitted);
		...
	}

The query hints are not limited to SQL Server only, it is also applicable to other data providers. However, the library only supports table hints for SQL Server as of the moment through `SqlStatementBuilder` and `SqlTableHints` class.

In order to support the other data providers, the `IStatementBuilder` must be overriden by the caller's custom statement builder (ie: `OracleStatementBuilder`, `MySqlStatementBuilder`) when calling the `DbConnection` extended method. And also, the caller must specify a class table hints for that data provider (ie: `OracleQueryHints`, `MySqlQueryHints`) to simplify the call (though this can be written statically through explicit literal string).

**Note**: The `hints` are also supported when calling the `BatchQuery`, `Count`, `CountAll` and the `QueryMultiple` operations.