Hints
=====

This feature is to allow the caller to further optimize the execution of the query when the `Query` operation has been called.

Below is an example of how to do a query (dirty-read) of customers where name starts with `Joh`.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(c => c.Name.StartsWith("Joh"), hints: SqlTableHints.NoLock);
	}

The `hints` argument is used define a query-optimizer in the SQL Statement query. It is equivalent to the SQL Server query hints. The caller can also write its own hints via literal string. See below.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(c => c.Name.StartsWith("Joh"), hints: "WITH (NOLOCK)");
	}

Below is a scenario to query all the customers from the database that ignores all the data that are under different transactions and with maximizing the index named `NCIX_Customer$FirstName$LastName`.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.QueryAll<Customer>(hints: "WITH (INDEX(NCIX_Customer$FirstName$LastName), READPAST)");
	}

A default class named `SqlTableHints` is provided to simplify the passing of the parameters. This class only contains the table hints for SQL Server.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.QueryAll<Customer>(hints: SqlTableHints.NoLock);
		var customers = connection.QueryAll<Customer>(hints: SqlTableHints.ReadPast);
		var customers = connection.QueryAll<Customer>(hints: SqlTableHints.ReadCommitted);
		...
	}

The library only supports table hints for SQL Server through `SqlServerStatementBuilder` and `SqlServerTableHints` class.

If this parameter is specified in `MySql`, `SqLite` and `PostgreSql`, the `NotSupportedException` exception will be thrown.

**Note**: The `hints` are also supported when calling the `BatchQuery`, `Count`, `CountAll`, `QueryAll` and the `QueryMultiple` operations.