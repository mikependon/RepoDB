Working with Repository
=======================

The library contains two repository objects, the `RepoDb.BaseRepository<TEntity, TDbConnection>` and the `RepoDb.DbRepository<TDbConnection>`.

The latter is the heart of the library as it contains all the operations that is being used by all other repositories within or outside the library.

DbRepository Class
------------------

.. highlight:: c#

A base object for all shared-based repositories. This object is usually being inheritted if the derived class is meant for shared-based operations. This object is used by `RepoDb.BaseRepository<TEntity, TDbConnection>` as an underlying repository for all of its operations. Located at `RepoDb` namespace.

This means that, the `RepoDb.BaseRepository<TEntity, TDbConnection>` is only abstracting the operations of the `RepoDb.DbRepository<TDbConnection>` object in all areas.

Below are the constructor parameters:

- **connectionString**: the connection string to connect to.
- **commandTimeout (optional)**: the command timeout in seconds. It is being used to set the value of the `DbCommand.CommandTimeout` object prior to the execution of the operation.
- **cache (optional)**: the cache object to be used by the repository. By default, the repository is using the `RepoDb.MemoryCache` object.
- **trace (optional)**: the trace object to be used by the repository. The default is `null`.
- **statementBuilder (optional)**: the statement builder object to be used by the repository. By default, the repository is using the `RepoDb.SqlDbStatementBuilder` object.

This repository can be instantiated directly or indirectly. Indirectly means, it should be abstracted first before instantiation.

See sample code below on how to directly create a `DbRepository` object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");

Another way of creating a `DbRepository` is by abstracting it through derived classes. See sample code below.

::

	public class NorthwindDbRepository : DbRepository<SqlConnection>
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

Then, call it somewhere.

::

	var repository = new NorthwindRepository();

It is recommended to create a contracted interface for `DbRepository` in order for it to be dependency injectable.

BaseRepository Class
--------------------

.. highlight:: c#

An abstract class for all entity-based repositories. This object is usually being inheritted if the derived class is meant for entity-based operations. Located at `RepoDb` namespace.

The operational scope of this repository is only limited to its defined target `DataEntity` object.

Below are the constructor parameters:

- **connectionString**: the connection string to connect to.
- **commandTimeout (optional)**: the command timeout in seconds. It is being used to set the value of the `DbCommand.CommandTimeout` object prior to the execution of the operation.
- **cache (optional)**: the cache object to be used by the repository. By default, the repository is using the `RepoDb.MemoryCache` object.
- **trace (optional)**: the trace object to be used by the repository. The default is `null`.
- **statementBuilder (optional)**: the statement builder object to be used by the repository. By default, the repository is using the `RepoDb.SqlDbStatementBuilder` object.

See sample code below on how to directly create a `DbRepository` object.

::

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

Then, call it somewhere.

::

	var repository = new CustomerRepository();

It is recommended to create a contracted interface for `BaseRepository` in order for it to be dependency injectable.

Creating a Connection
---------------------

.. highlight:: c#

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

Connection.EnsureOpen
---------------------

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

Connection.ExecuteReader
------------------------

.. highlight:: c#

This connection extension method is use to execute a SQL statement query from the database in fast-forward access. This method returns an instance of `System.Data.IDataReader` object.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteReader()` method.

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

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.
- **trace**: the trace object to be used on this operation.

Connection.ExecuteQuery
-----------------------

.. highlight:: c#

This connection extension method is use to execute a SQL statement query from the database in fast-forward access. It returns an enumerable list of `dynamic` or `RepoDb.DataEntity` object.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteReader()` method.

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

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.
- **trace**: the trace object to be used on this operation.

Connection.ExecuteNonQuery
--------------------------

.. highlight:: c#

This connection extension method is used to execute a non-queryable SQL statement. It returns an `int` that holds the number of affected rows during the execution.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteNonQuery()` method.

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

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.
- **trace**: the trace object to be used on this operation.

Connection.ExecuteScalar
------------------------

.. highlight:: c#

This connection extension method is used to execute a query statement that returns single value of type `System.Object`.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteScalar()` method.

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

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.
- **trace**: the trace object to be used on this operation.

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

**Note**: The multiple mapping also supports the Stored Procedure by binding it to the entity object.