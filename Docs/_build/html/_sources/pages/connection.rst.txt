Connection Object
=================

.. highlight:: c#

The library has abstracted everything from `ADO.NET`, however, some extension methods has been developed to simplify the data access models.

Below are the list of extension methods for `Connection` object.

- **CreateCommand**: is used to create a command object before the actual operation execution. It returns an instance of `System.Data.IDbCommand` object.
- **EnsureOpen**: is used to ensure that the connection object is `Open`. The repository operations are calling this method explicitly prior to the actual execution. This method returns the connection instance itself.

A repository is used to create a connection object.

::

	var repository = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen();
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		// Use the connection here
	}

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Use the connection here
	}

CreateCommand Method
--------------------

.. highlight:: c#

Creates a command object.

The underlying method call of this method is the `System.Data.DbConnection.CreateCommand()` method.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **transaction (optional)**: the transaction object be used when executing the command.

See sample codes below.

::

	// Variables
	var customers = (IEnumerable<Customer>)null;

	// Open a connection
	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		
		// Create a command object
		var command = connection.CreateCommand("SELECT TOP 100 * FROM [dbo].[Customer];", CommandType.Text, 500, null);
		
		// Execute the reader, abstracting the ADO.Net features here
		using (var reader = command.ExecuteReader())
		{
			// Iterate the reader and place back the result to the list
			customers = RepoDb.Reflection.DataReaderConverter.ToEnumerable<Customer>((DbDataReader)reader);
		}
	}

EnsureOpen Method
-----------------

.. highlight:: c#

This method is used to ensure that the connection object is `Open`. The operational methods are calling this method explicitly prior to the actual execution. This method returns the connection instance itself.

The underlying method call of this method is the `System.Data.DbConnection.Open()` method.

See sample codes below.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Use the connection here
	}

Working with StoredProcedure
----------------------------

.. highlight:: c#

Calling a stored procedure is a simple as executing any SQL Statements via repository, and by setting the `CommandType` to `StoredProcedure`.

Say a Stored Procedure below exists in the database.

.. highlight:: sql

::

	DROP PROCEDURE IF EXISTS [dbo].[sp_GetCustomer];
	GO

	CREATE PROCEDURE [dbo].[sp_GetCustomer]
	(
		@Id BIGINT
	)
	AS
	BEGIN

		SELECT Id
			, Name
			, Title
			, UpdatedDate
			, CreatedDate
		FROM [dbo].[Customer]
		WHERE (Id = @Id);

	END

.. highlight:: c#

Below is the way on how to call the Stored Procedure.

Calling via `Repository.ExecuteQuery`.

::

	var repository = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen();
	var customers = repository.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure);
	customers
		.ToList()
		.ForEach(customer =>
		{
			// Process each customer here
		});

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure);
		customers
			.ToList()
			.ForEach(customer =>
			{
				// Process each customer here
			});
	}

Or, via independent `SqlConnection` object extended `ExecuteQuery` method that returns the list of `dynamic` objects.

::
	
	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.ExecuteQuery("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure);
		customers
			.ToList()
			.ForEach(customer =>
			{
				// Process each customer here
			});
	}


Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = connection.ExecuteReader("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure))
		{
			while (reader.Read())
			{
				// Process each row here
			}
		}
	}

**Note**: The multiple mapping also supports the Stored Procedure by binding it to the `DataEntity` object.
