Connection Object
=================

.. highlight:: c#

The library has abstracted everything from `ADO.NET` when it comes to the connection object.

Creating a Connection
---------------------

Via repository:

::

	var repository = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen();
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		// Use the connection here
	}

Or, the traditional way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Use the connection here
	}

CreateCommand Method
--------------------

.. highlight:: c#

Creates a command object.

::

	// Variables
	var customers = (IEnumerable<Customer>)null;

	// Open a connection
	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Create a command object
		var command = connection.CreateCommand("SELECT TOP 100 * FROM [dbo].[Customer];", CommandType.Text, 500, null);

		// Use the command object here
	}

EnsureOpen Method
-----------------

.. highlight:: c#

Is used to ensure that the connection object is in `Open`.

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
