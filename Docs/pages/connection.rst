Connection Object
=================

.. highlight:: c#

The library has abstracted everything from `ADO.NET`, however, some extension methods has been developed to simplify the data access models.

Below are the list of extension methods for `Connection` object.

- **EnsureOpen**: is used to ensure that the connection object is `Open`. The repository operations are calling this method explicitly prior to the actual execution. This method returns the connection instance itself.
- **ExecuteNonQuery**: is used to execute a non-queryable SQL statement. It returns an `int` that holds the number of affected rows during the execution.
- **ExecuteQuery**: is used to execute a SQL statement query from the database in fast-forward access. It returns an enumerable list of `dynamic` or `RepoDb.DataEntity` object.
- **ExecuteReader**: is used to execute a SQL statement query from the database in fast-forward access. This method returns an instance of `System.Data.IDataReader` object.
- **ExecuteScalar**: is used to execute a query statement that returns single value of type `System.Object`.

A repository is used to create a connection object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection())
	{
		// Use the connection here
	}

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		// Use the connection here
	}

EnsureOpen Method
-----------------

.. highlight:: c#

This method is used to ensure that the connection object is `Open`. The repository operations are calling this method explicitly prior to the actual execution. This method returns the connection instance itself.

The underlying method call of this method is the `System.Data.DbConnection.Open()` method.

See sample codes below.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		// No need to open the connection
	}

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Use the connection here
	}

ExecuteReader Method
--------------------

.. highlight:: c#

This connection extension method is used to execute a SQL statement query from the database in fast-forward access. This method returns an instance of `System.Data.IDataReader` object.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteReader()` method.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

See sample codes below.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE (Id <= @Id);";
		using (var reader = connection.ExecuteReader(commandText, new { Id = 10000 }))
		{
			while (reader.Read())
			{
				// Process the records here
			}
		}
	}

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE (Id <= @Id);";
		using (var reader = connection.ExecuteReader(commandText, new { Id = 10000 }))
		{
			while (reader.Read())
			{
				// Process the records here
			}
		}
	}

ExecuteQuery Method
-------------------

.. highlight:: c#

This connection extension method is used to execute a SQL statement query from the database in fast-forward access. It returns an enumerable list of `dynamic` or `RepoDb.DataEntity` object.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteReader()` method.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

Code below returns an enumerable list of `dynamic` object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE (Id <= @Id);";
		var customers = connection.ExecuteQuery(commandText, new { Id = 10000 }))
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
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE (Id <= @Id);";
		var customers = connection.ExecuteQuery(commandText, new { Id = 10000 }))
		customers
			.ToList()
			.ForEach(customer =>
			{
				// Process each customer here
			});
	}

Code below returns an enumerable list of `Customer` object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE (Id <= @Id);";
		var customers = connection.ExecuteQuery<Customer>(commandText, new { Id = 10000 }))
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
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE (Id <= @Id);";
		var customers = connection.ExecuteQuery<Customer>(commandText, new { Id = 10000 }))
		customers
			.ToList()
			.ForEach(customer =>
			{
				// Process each customer here
			});
	}

ExecuteNonQuery Method
----------------------

.. highlight:: c#

This connection extension method is used to execute a non-queryable SQL statement. It returns an `int` that holds the number of affected rows during the execution.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteNonQuery()` method.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

See sample codes below.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var commandText = @"UPDATE [dbo].[Customer] SET Name = @Name WHERE (Id = @Id);";
		var affectedRows =  connection.ExecuteNonQuery(commandText, new { Id = 10000, Name = "Anna Fullerton" });
	}

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"UPDATE [dbo].[Customer] SET Name = @Name WHERE (Id = @Id);";
		var affectedRows =  connection.ExecuteNonQuery(commandText, new { Id = 10000, Name = "Anna Fullerton" });
	}

ExecuteScalar Method
--------------------

.. highlight:: c#

This connection extension method is used to execute a query statement that returns single value of type `System.Object`.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteScalar()` method.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout (optional)**: the command timeout in seconds to be used when executing the query in the database.
- **commandType (optional)**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction (optional)**: the transaction object be used when executing the command.

See sample codes below.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var commandText = @"SELECT MAX(Id) FROM [dbo].[Customer];";
		var customerMaxId =  connection.ExecuteScalar(commandText);
	}

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"SELECT MAX(Id) FROM [dbo].[Customer];";
		var customerMaxId =  connection.ExecuteScalar(commandText);
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

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure);
	customers
		.ToList()
		.ForEach(customer =>
		{
			// Process each customer here
		});

Or, in a tradional way with independent `SqlConnection` object extended method.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
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
	
	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
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

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
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
