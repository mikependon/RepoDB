Introduction to RepoDb
======================

A dynamic ORM .NET Library used to create an entity-based repository classes when accessing data from the database.

Goal
----

We aim to let .NET developers limit the implementation of SQL Statements within the application.

We believe that as a .NET developer, one should only focus on .NET and Business scenario development and not on writing repetitive SQL Statements.

Of course, unless you are working closely with SQL Server Management Studio.

Principles
----------

* We will keep it simple as possible (KISS principle)
* We will make it fast as possible
* We will never make complex implementations (specially for Queries and Methods)
* We will avoid developing complex JOINs (until it is needed and requested by the community)
* We will never ever do try-catch inside the library
* We will help developers to be more focus on SOLID principle

Why RepoDb?
-----------

* Operations (Asynchronous)
* Type Mapping
* Field Mapping
* Multiple Mapping
* Expression Tree
* Caching
* Tracing
* SQL Statement Builder
* Transactions

Installation Steps
==================

There are two ways of installing the library as standardized by Microsoft. First, via Nuget Package Manager, and secondly, via Package Manager Console.

**Pre-requisite**: A Microsoft Visual Studio has already been installed and a Project Solution has already been created.

Via Nuget Package Manager
-------------------------

Once the project solution is open, then do the following steps.

1. Right click on the solution and click the `Manage Nuget Packages for Solution...`.
2. Select the `Browse` tab.
3. In the `Search` field, type `RepoDb` and press `Enter`.
4. In the right side, select the project you wish the library to install to.
5. Click the `Install` button.

Wait for few seconds until the installation is completed.

Via Package Manager Console
---------------------------

Once the project solution is open, then do the following steps.

1. Select the `View -> Other Windows -> Package Manager Console`.
2. In the `Package Source` drop down field, select `All`.
3. In the `Default Project` drop down field, select the project you wish the library to install to.
4. In the `Package Manager Console`, type the `Install-Package RepoDb`, then press `Enter`.

Wait for few seconds until the installation is completed.

Creating a DataEntity
=====================

A `DataEntity` is a DTO class used to feed the operations of the repositories. It acts as a single row when it comes to database.

It is recommended that an explicit interface (of type `RepoDb.Interfaces.IDataEntity`) must be implemented in order to make a contracted `DataEntity`.

DataEntity Class
----------------

.. highlight:: c#

Acts as the base class of all `Data Entity` object. Located at `RepoDb` namespace.

::

	public class ClassName : DataEntity
	{
	}

IDataEntity Interface
---------------------

.. highlight:: c#

An interface used to implement to mark a class to be the base class of all `Data Entity` object. Located at `RepoDb.Interfaces` namespace.

::

	public interface IClassName : IDataEntity
	{
	}

Mapping with MapAttribute
=========================

A `MapAttribute` is used to define a mapping of the current `Class` or `Property` equivalent to an `Object` or `Field` name in the database. Located at `RepoDb.Attributes` namespace.

The `Map` attribute second parameter is a command type `of type System.Data.CommandType`. If this parameter is defined, the repository operation execution will then be of that command type. This parameter is only working at the class level implementation.

See Microsoft documentation for `System.Data.CommandType`_ here.

.. _System.Data.CommandType: https://msdn.microsoft.com/en-us/library/system.data.commandtype%28v=vs.110%29.aspx

Mapping a Class
----------------

.. highlight:: c#

By default, the name of the class is used as a default mapped object from the database. However, if the database object name is different from the class name, the `MapAttribute` is then use to map it properly.

Below is a sample code that maps the class named `EmployeeDto` into `Employee` table from the database.

::

	[Map("[dbo].[Employee]", CommandType.Text)]
	public class EmployeeDto : DataEntity
	{
		public int Id { get; set; }
	}

Mapping a Property
------------------

.. highlight:: c#

By default, the name of the property is used as a default mapped field from the database object (table, view or any result set). However, if the database field name is different from the property name, the `MapAttribute` is then use to map it properly.

Below is a sample code that maps the property named `Id` into a `EmployeeId` field `Employee` table from the database.

::

	[Map("[dbo].[Employee]", CommandType.Text)]
	public class EmployeeDto : DataEntity
	{
		[Map("EmployeeId")]
		public int Id { get; set; }
	}

Primary Attribute
=================

.. highlight:: c#

A `PrimaryAttribute` is used to define the class property as primary property of the `DataEntity` object. Located at `RepoDb.Attributes` namespace.

::

	public class Employee : DataEntity
	{
		[Primary]
		public int Id { get; set; }
	}

Defining an Identity Property
-----------------------------

To define an identity property, simply sets the `isIdentity` parameter of the `PrimaryAttribute` during the implementation.

::

	public class Employee : DataEntity
	{
		[Primary(true)]
		public int Id { get; set; }
	}

Ignore Attribute
================

.. highlight:: c#

An `IgnoreAttribute` is used to mark a class property to be ignoreable during the actual repository operation. Located at `RepoDb.Attributes` namespace.

Example: If of type command `Insert` and `Update` is defined on the `IgnoreAttribute` of the class property named `CreatedDate`, then the property will not be a part of the `Insert` and `Update` operation of the repository.

::

	public class Employee : DataEntity
	{
		public int Id { get; set; }

		[Ignore(Command.Insert | Command.Update)]
		public DateTime CreatedDate { get; set; }
	}

Below are the commands that can be defined using the `IgnoreAttribute`.

* None
* Query
* Insert
* Update
* Delete
* Merge
* BatchQuery
* InlineUpdate

Multiple Entity Mapping
=======================

This feature is a unique built-in feature of the library that enables the developer to do multiple mapping on a `DataEntity` object into multiple object in the database. This is very usable for some complex requirements that includes like the implementations of `Table`, `Views` and `StoredProcedures` must be mapped into one `DataEntity` object.

The class named `RepoDb.DataEntityMapper` is used when doing a multiple mapping. Below are the methods.

- **For**: used to create a mapping between the data entity and database object. Returns an `RepoDb.DataEntityMapItem` object.
 
The class named `RepoDb.DataEntityMapItem` is used to map the operation level of the repository into the database object. Below are the methods.

- **On**: used to map on which repository command operation and database object the mapping is implemented.
- **Set**: used to set the repository command operation data entity mapping definition. This is the underlying method being called by `On` method.
- **Get**: get the entity mapping definition defined on the repository command operation.

Multi-mapping is bound in an operation-level of the repository. This means that the developer can map the `Query` operation of a `Customer` object into `[dbo].[Customer]` table of the database, whereas the `Delete` operation is mapped into `[dbo].[sp_DeleteCustomer]` database object.

Let say a `Customer` entity object was created in the solution, and the following database objects exist.

 - A table named `Customer`.
 - A stored procedure named `sp_DeleteCustomer`.
 - A stored procedure named `sp_InsertCustomer`, where the logic inside of is joining from different database tables.
 - A view named `vw_Customer`.
 
Developers can simply call the mapper methods when mapping a `Customer` object into these database objects.

Below are the codes for multiple mapping.

::

	DataEntityMapper.For<Stock>()
		.On(Command.Insert, "[dbo].[sp_QueryCustomer]", CommandType.StoredProcedure)
		.On(Command.Delete, "[dbo].[sp_DeleteCustomer]", CommandType.StoredProcedure)
		.On(Command.Query, "[dbo].[vw_Customer]")
		.On(Command.Update, "[dbo].[Customer]")
		.On(Command.BulkInsert, "[dbo].[Customer]", CommandType.TableDirect);

These feature has its own limitations. All mappings could not be done on every command (i.e Count, CountBig, Merge, ExecuteQuery, ExecuteNonQuery, ExecuteReader, ExecuteScalar).

Below are the supported mapping for each command.

- **BatchQuery**: only for a database Table and `CommandType.Text`.
- **Count**: only for a database Table and `CommandType.Text`.
- **CountBig**: only for a database Table and `CommandType.Text`.
- **InlineUpdate**: only for a database Table and `CommandType.Text`.
- **Merge**: only for a database Table and `CommandType.Text`.
- **BulkInsert**: only for a database Table and `CommandType.<Text | TableDirect>`.
- **Insert**: full support.
- **Delete**: full support.
- **Query**: full support.
- **Update**: full support.

Attempt to map to a wrong command would throw an `System.InvalidOperationException` back to the caller.

Type Mapping
============

.. highlight: c#

Type mapping is feature that allows the library to identify which type of .NET is equivalent to the `System.Data.DbType` types. This feature is important to force the library the conversion it will going to specially when running the repository operations.

Below is the way on how to map the `System.DateTime` to be equivalent as `System.Data.DbType.DateTime2`.

::

	TypeMapper.Map(typeof(DateTime), DbType.DateTime2);

and `System.Decimal` into `System.Data.DbType.Double`.

::
	
	TypeMapper.AddMap(new TypeMap(typeof(Decimal), DbType.Double));

**Note**: The class is callable anywhere in the application as it was implemented in a static way.

Working wth Repository
======================

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

See sample code below the way on how to create an interface and implement it directly to the derived class.

::

	public interface INorthwindDbRepository : IDbRepository<SqlConnection>
	{
	}

	public class NorthwindDbRepository : DbRepository<SqlConnection>, INorthwindDbRepository
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

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

See sample code below the way on how to create an interface and implement it directly to the derived class.

::

	public interface ICustomerRepository : IBaseRepository<Customer, SqlConnection>
	{
	}

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>, ICustomerRepository
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

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

Connection.EnsureOpen
---------------------

.. highlight:: c#

This method is used to ensure that the connection object is `Open`. The repository operations are calling this method explicitly prior to the actual execution. This method returns the connection instance itself.

The underlying method call of this method is the `System.Data.DbConnection.Open()` method.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		// No need to open the connection
	}

Connection.ExecuteReader
------------------------

.. highlight:: c#

This connection extension method is use to execute a SQL statement query from the database in fast-forward access. This method returns an instance of `System.Data.IDataReader` object.

The underlying method call of this method is the `System.Data.IDbCommand.ExecuteReader()` method.

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

This connection extension method is use to execute a SQL statement query from the database in fast-forward access. It returns an enumerable list of `dynamic` or `RepoDb.Interfaces.IDataEntity` object.

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

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
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

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
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

Calling a stored procedure is a simple as a	executing any SQL Statements via repository, and by setting the `CommandType` to `StoredProcedure`.

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

Below is the way on how to call the Stored Procedure defined above via `ExecuteQuery`.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var customers =  connection.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure);
		customers
			.ToList()
			.ForEach(customer =>
			{
				// Process each customer here
			});
	}

Or, via `ExeucteReader`.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		using (var reader =  connection.ExecuteReader<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure))
		{
			while (reader.Read())
			{
				// Process each row here
			}
		}
	}

**Note**: The multiple mapping also supports the Stored Procedure by binding it to the entity object.

Transaction
===========

.. highlight:: c#

A transaction object works completely the same as it was with `ADO.NET`. The library only abstracted `ADO.NET` including the transaction objects.

Transactions can be created by calling the `BeginTransaction` method of the `DbConnection` object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var transaction = connection.BeginTransaction();
		try
		{
			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
		}
		finally
		{
			transaction.Dispose();
		}
	}

Every operation of the repository accepts a transaction object as an argument. Once passed, the transaction will become a part of the execution context. See below on how to commit a transaction context with multiple operations.

::

	var connectionString = @"Server=.;Database=Northwind;Integrated Security=SSPI;";
	var customerRepository = new CustomerRepository<Customer, SqlConnection>(connectionString);
	var orderRepository = new OrderRepository<Order, SqlConnection>(connectionString);
	using (var connection = customerRepository.CreateConnection().EnsureOpen())
	{
		var transaction = connection.BeginTransaction();
		try
		{
			var customer = new Customer()
			{
				Name = "Anna Fullerton",
				CreatedDate = DateTime.UtcNow
			};
			var customerId = Convert.ToInt32(customerRepository.Insert(customer, transaction: transaction));
			var order = new Order()
			{
				CustomerId = customerId,
				ProductId = 12,
				Quantity = 2,
				CreatedDate = DateTime.UtcNow
			};
			var orderId = Convert.ToInt32(orderRepository.Insert(order, transaction: transaction));
			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
		}
		finally
		{
			transaction.Dispose();
		}
	}

The code snippets above will first insert a `Customer` record in the database and will return the newly added customer `Id`. It will be followed by inserting the `Order` record with the parent `Customer.Id` as part of the entity relationship. Then, the transaction will be committed. However, if any exception occurs during the operation, the transaction will rollback all the operations above.

**Note**: Notice that the transaction object were created via `CustomerRepository` and has been used in both repository afterwards. The library will adapt the transaction process of `ADO.NET`. So whether the transaction object is created via an independent `DbConnection` object, as long as the connection is open, then the operation is valid.

Expression Tree
===============

.. highlight:: c#

The expression tree is the brain of the library. It defines the best possible way of doing a `WHERE` expression (SQL Statement) by composing it via `dynamic` or `System.Interfaces.IQueryGroup` objects.

Objects used for composing the expression tree.

- **QueryGroup**: used to group an expression.
- **QueryField**: holds the field/value pair values of the expressions.
- **Conjunction**: an enumeration that holds the value whether the expression is on `And` or `Or` operation.
- **Operation**: an enumeration that holds the value what kind of operation is going to be executed on certain expression. It holds the value of like `Equal`, `NotEqual`, `Between`, `GreaterThan` and etc.

Certain repository operations are using the expression trees.

- Repository.BatchQuery
- Repository.Count
- Repository.CountBig
- Repository.Delete
- Repository.ExecuteNonQuery
- Repository.ExecuteQuery
- Repository.ExecuteScalar
- Repository.InlineUpdate
- Repository.Query
- Repository.Update

Certain connection extension methods are using the expression trees.

- DbConnection.ExecuteNonQuery
- DbConnection.ExecuteQuery
- DbConnection.ExecuteReader
- DbConnection.ExecuteScalar

There are two ways of building the expression trees, the explicit way by using `IQueryGroup` objects and dynamic way by using `dynamic` objects.

Explicit Query Expression
-------------------------

An explicit query expression are using the defined objects `RepoDb.QueryGroup`, `RepoDb.QueryField`, `RepoDb.Enumerations.Conjunction` and `RepoDb.Enumerations.Operation` when composing the expression.

Below is a pseudo code of explicit query expression.

::

	// WHERE (Field1 = @Field1 AND Field2 = @Field2) AND ((Field3 = @Field3 OR Field4 = @Field4) AND (Field5 = @Field5 OR Field6 = @Field6));
	var tree = new QueryGroup
	(
		new QueryField[]
		{
			// List of QueryFields
		},
		new QueryGroup[]
		{
			// List of QueryGroups
			new QueryGroup
			(
				new QueryField[]
				{
					// List of QueryFields
				},
				new QueryGroup[]
				{
					// List of QueryGroups
					...
					...
					...
				}
				Conjunction.Or
			),
			new QueryGroup
			(
				new QueryField[]
				{
					// List of QueryFields
				},
				new QueryGroup[]
				{
					// List of QueryGroups
					...
					...
					...
				}
				Conjunction.Or
			)
		},
		Conjunction.And
	);

Actual explicit query tree expression.

::

	// Last 3 months customer with CustomerId >= 10045
	var query = new QueryGroup
	(
		new []
		{
			new QueryField("CustomerId", Operation.GreaterThanOrEqual, 10045),
			new QueryField("CreatedDate", Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3))
		},
		null, // Child QueryGroups
		Conjunction.And
	);

Dynamic Query Expression
------------------------

.. highlight:: c#

A dynamic query expression is using a single dynamic object when composing the expression.

Below is a pseudo code of dynamic query expression.

::

	var tree = new
	{
		Conjunction = Conjunction.And,
		Field1 = "Field1", // QueryField
		Field2 = "Field2", // QueryField
		QueryGroups = new []
		{
			new
			{
				Conjunction = Conjunction.Or,
				Field3 = "Field3", // QueryField
				Field4 = "Field4",
				QueryGroups = new object[]
				{
					...
				}
			},
			new
			{
				Conjunction = Conjunction.Or,
				Field3 = "Field3", // QueryField
				Field4 = "Field4",
				QueryGroups = new object[]
				{
					...
				}
			}
		}
	};

Actual dynamic query tree expression.

::

	// Last 3 months customer with CustomerId >= 10045
	var query = new
	{
		CustomerId = new { Operation = 10045 },
		CreatedDate = new { Operation = Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3) }
	};

QueryGroup
==========

.. highlight:: c#

A query group object is used to group an expression when composing a tree expressions. It is equivalent to a grouping on a `WHERE` statement in SQL Statements.

Below are the constructor parameters.

- **queryFields**: the list of `IQueryField` objects to be included in the expression composition. It stands as `[FieldName] = @FiedName` when it comes to SQL Statement compositions.
- **queryGroups**: the list of child `IQueryGroup` objects to be included in the expresson composition. It stands as the `([FieldName] = @FieldName AND [FieldName1] = @FieldName1)` when it comes to SQL Statement compositions.
- **conjunction**: the conjuction to be used when grouping the fields. It stands as the `AND` or `OR` in the SQL Statement compositions.

As mentioned above, below is a sample code to create a query group object.

::

	// Last 3 months customer with CustomerId >= 10045
	var query = new QueryGroup
	(
		new []
		{
			new QueryField("CustomerId", Operation.GreaterThanOrEqual, 10045),
			new QueryField("CreatedDate", Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3))
		},
		null, // Child QueryGroups
		Conjunction.And
	);

Query Operations
----------------

.. highlight:: c#

The query operation defines the operation to be used by the query expression (field level) during the actual execution. It is located at `RepoDb.Enumerations` namespace.

List of Operations:

- Operation.Equal
- Operation.NotEqual
- Operation.LessThan
- Operation.LessThanOrEqual
- Operation.GreaterThan
- Operation.GreaterThanOrEqual
- Operation.Like
- Operation.NotLike
- Operation.Between
- Operation.NotBetween
- Operation.In
- Operation.NotIn
- Operation.All
- Operation.Any

Operation.Equal
---------------

.. highlight:: c#

Part of the expression tree used to determine the `equality` of the field and data.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = 10045 });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Id = new { Operation = Operation.Equal, Value = 10045 }
	});

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.Equal, 10045 ));

Operation.NotEqual
------------------

.. highlight:: c#

Part of the expression tree used to determine the `inequality` of the field and data.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Name = new { Operation = Operation.NotEqual, Value = "Anna Fullerton" }
	});

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Name", Operation.NotEqual, "Anna Fullerton" });

Operation.LessThan
------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `less than` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.LessThan, Value = 100 } });


Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.LessThan, 100 });

Operation.GreaterThan
---------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `greater than` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.GreaterThan, Value = 0 } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.GreaterThan, 0 });

Operation.LessThanOrEqual
-------------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `less than or equal` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.LessThanOrEqual, Value = 100 } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>.Query(new QueryField("Id", Operation.LessThanOrEqual, 100 });

Operation.GreaterThanOrEqual
----------------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `greater than or equal` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.GreaterThanOrEqual, Value = 0 } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.GreaterThanOrEqual, 0 });

Operation.Like
--------------

.. highlight:: c#

Part of the expression tree used to determine whether the field is `identitical` to a given value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Name = new { Operation = Operation.Like, Value = "Anna%" } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Name", Operation.Like, "Anna%" });

Operation.NotLike
-----------------

.. highlight:: c#

Part of the expression tree used to determine whether the field is `not identitical` to a given value. An opposite of `Operation.Like`.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Name = new { Operation = Operation.NotLike, Value = "Anna%" } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Name", Operation.NotLike, "Anna%" });

Operation.Between
-----------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `between` 2 given values.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { CreatedDate = new { Operation = Operation.Between, Value = new [] { Date1, Date2 } } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.Between, Value = new [] { 10045, 10075 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("CreatedDate", Operation.Between, new [] { Date1, Date2 } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.Between, new [] { 10045, 10075 } });

Operation.NotBetween
--------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not between` 2 given values. An opposite of `Operation.Between`.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { CreatedDate = new { Operation = Operation.NotBetween, Value = new [] { Date1, Date2 } } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.NotBetween, Value = new [] { 10045, 10075 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("CreatedDate", Operation.NotBetween, new [] { Date1, Date2 } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.NotBetween, new [] { 10045, 10075 } });

Operation.In
------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `in` given values.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.In, Value = new [] { 10045, 10046, 10047, 10048 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.In, new [] { 10045, 10046, 10047, 10048 } });

Operation.NotIn
---------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not in` given values. An opposite of `Operation.In`. See sample below.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.NotIn, Value = new [] { 10045, 10046, 10047, 10048 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.NotIn, new [] { 10045, 10046, 10047, 10048 } });

Operation.All
-------------

.. highlight:: c#

Part of the expression tree used to determine whether `all` the field values satisfied the criteria.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Name = new
		{
			Operation = Operation.All, // Works as AND
			Value = new object[]
			{
				new { Operation = Operation.Like, Value = "Anna%" },
				new { Operation = Operation.NotEqual, Value = "Tom Hawks" },
				new { Operation = Operation.NotIn, Value = new string[] { "Frank Myers", "Joe Austin" } }
			}
		}
	});


Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>
	(
		new QueryField[]
		{
			new QueryField("Name", Operation.Like, "Anna%"),
			new QueryField("Name", Operation.NotEqual, "Tom Hawks"),
			new QueryField("Name", Operation.NotIn, new string[] { "Frank Myers", "Joe Austin" })
		}
	);

The `Operation.All` only works at the `dynamic` expression tree to simply the composition of the statement. Passing a list of `IQueryField` in the `IQueryGroup` object will do the same when calling it explicitly.

Operation.Any
-------------

.. highlight:: c#

Part of the expression tree used to determine whether `any` of the field values satisfied the criteria.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Name = new
		{
			Operation = Operation.Any, // Works as OR
			Value = new object[]
			{
				new { Operation = Operation.Like, Value = "Anna%" },
				new { Operation = Operation.NotEqual, Value = "Tom Hawks" },
				new { Operation = Operation.In, Value = new string[] { "Frank Myers", "Joe Austin" } }
			}
		}
	});

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>
	(
		new QueryField[]
		{
			new QueryField("Name", Operation.Like, "Anna%"),
			new QueryField("Name", Operation.NotEqual, "Tom Hawks"),
			new QueryField("Name", Operation.In, new string[] { "Frank Myers", "Joe Austin" })
		},
		null, // List of QueryGroups
		Conjunction.Or
	);

The `Operation.Any` only works at the `dynamic` expression tree to simply the composition of the statement. Passing a list of `IQueryField` in the `IQueryGroup` object will do the same when calling it explicitly.

Repository Operations
=====================

.. highlight:: c#

The repositories contain different operations to manipulate the data from the database. Below are the list of common operations widely used.

- **Query**: used to query a record from the database. It uses the `SELECT` command of SQL.
- **Insert**: used to insert a record in the database. It uses the `INSERT` command of SQL.
- **Update**: used to update a record in the database. It uses the `UPDATE` command of SQL.
- **Delete**: used to delete a record in the database. It uses the `DELETE` command of SQL.
- **Merge**: used to merge a record in the database. It uses the `MERGE` command of SQL.
- **InlineUpdate**: used to do column-based update of the records in the database. It uses the `UPDATE` command of SQL.
- **Count**: used to count all the records in the database. It uses the `SELECT`, `COUNT` command of SQL.
- **CountBig**: used to count (big) all the records in the database. It uses the `SELECT`, `COUNT_BIG` command of SQL.
- **BatchQuery**: used to query a record from the database by batch. It uses the `SELECT` in combination of `ROW_NUMBER` and `ORDER` command of SQL.
- **BulkInsert**: used to bulk-insert the records in the database.
- **ExecuteQuery**: used to read certain records from the database in fast-forward access. It returns an enumerable list of `RepoDb.Interfaces.IDataEntity` objects.
- **ExecuteNonQuery**: used to execute a non-queryable query statement in the database.
- **ExecuteScalar**: used to execute a command that returns a single-object value from the database.

All operations mentioned above has its own corresponding asynchronous operation. Usually, the asynchronous operation is only appended by `Async` keyword. Below are the list of asynchronous operations.

- **QueryAsync**
- **InsertAsync**
- **UpdateAsync**
- **DeleteAsync**
- **MergeAsync**
- **InlineUpdateAsync**
- **CountAsync**
- **CountBigAsync**
- **BatchQueryAsync**
- **BulkInsertAsync**
- **ExecuteQueryAsync**
- **ExecuteNonQueryAsync**
- **ExecuteScalar**

Query Operation
---------------

.. highlight:: c#

This operation is used to query a data from the database and returns an `IEnumerable<TEntity>` object. Below are the parameters.

- **where**: an expression to used to filter the data.
- **transaction**: the transaction object to be used when querying a data.
- **top**: the value used to return certain number of rows from the database.
- **orderBy**: the list of fields to be used to sort the data during querying.
- **cacheKey**: the key of the cache to check.

Below is a sample on how to query a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>();

Above snippet will return all the `Customer` records from the database. The data can filtered using the `where` parameter. See sample below.

Implicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customer = repository.Query<Customer>(1).FirstOrDefault();

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customer = repository.Query<Order>(new { Id = 1 }).FirstOrDefault();


Explicity way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customer = repository.Query<Customer>
	(
		new QueryGroup(new QueryField("Id", 1).AsEnumerable())
	).FirstOrDefault();

Below is the sample on how to query with multiple columns.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>(new { Id = 1, Name = "Anna Fullerton", Conjunction.Or });

Explicity way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new QueryGroup
		(
			new []
			{
				new QueryField("Id", Operation.Equal, 1),
				new QueryField("Name", Operation.Equal, "Anna Fullerton")
			},
			null,
			Conjunction.Or
		)
	);

When querying a data where `Id` field is greater than 50 and less than 100. See sample expressions below.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new { Id = new { Operation = Operation.Between, Value = new int[] { 50, 100 } } }
	);

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new
		{
			QueryGroups = new[]
			{
				new { Id = { Operation = Operation.GreaterThanOrEqual, Value = 50 } },
				new { Id = { Operation = Operation.LessThanOrEqual, Value = 100 } }
			}
		}
	);

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new
		{
			Id = new
			{
				Operation = Operation.All,
				Value = new object[]
				{
					new { Operation = Operation.GreaterThanOrEqual, Value = 50 },
					new { Operation = Operation.LessThanOrEqual, Value = 100 }
				} 
			}
		}
	);

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new QueryGroup
		(
			new []
			{
				new QueryField("Id", Operation.GreaterThanOrEqual, 50),
				new QueryField("Id", Operation.LessThanOrEqual, 100)
			}
		)
	);

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var customers = repository.Query<Customer>
	(
		new QueryGroup
		(
			new QueryField("Id", Operation.Between, new [] { 50, 100 }).AsEnumerable()
		)
	);

**Note**: Querying a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.


Ordering a Query
~~~~~~~~~~~~~~~~

.. highlight:: c#

An ordering is the way of sorting the result of your query in `ascending` or `descending` order, depending on the qualifier fields. Below is a sample snippet that returns the `Stock` records ordered by `ParentId` field in ascending manner and `Name` field is in `descending` manner.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orderBy = new
	{
		CustomerId = Order.Ascending,
		Name = Order.Descending
	};
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, orderBy: OrderField.Parse(orderBy));

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orderBy = new []
	{
		new OrderField("CustomerId", Order.Ascending),
		new OrderField("Name", Order.Descending)
	};
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, orderBy: orderBy);

The `RepodDb.OrderField` is an object that is being used to order a query result. The `Parse` method is used to convert the `dynamic` object to become an `OrderField` instances.

**Note:** When composing a dynamic ordering object, the value of the properties should be equal to `RepoDb.Interfaces.Order` values (`Ascending` or `Descending`). Otherwise, an exception will be thrown during `OrderField.Parse` operation.

Limiting a Query Result
~~~~~~~~~~~~~~~~~~~~~~~

.. highlight:: c#

A top parameter is used to limit the result when querying a data from the database. Below is a sample way on how to use the top parameter.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, top: 100);

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var orders = repository.Query<Order>(new { CustomerId = new { Operation = Operation.GreaterThan, Value = 1 } }, top: 100);

Insert Operation
----------------

.. highlight:: c#

This operation is used to insert a record in the database. It returns an object valued by the `PrimaryKey` column. If the `PrimaryKey` column is identity, this operation will return the newly added identity column value.

Below are the parameters:

- **entity**: the entity object to be inserted.
- **transaction**: the transaction object to be used when inserting a data.

Below is a sample on how to insert a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = new Order()
	{
		CustomerId = 10045,
		ProductId = 12
		Quantity = 2,
		CreatedDate = DateTime.UtcNow
	};
	repository.Insert(order);


Update Operation
----------------

.. highlight:: c#

This operation is used to update an existing record from the database. It returns an `int` value indicating the number of rows affected by the updates.

Below are the parameters:

- **entity**: the entity object to be updated.
- **where**: an expression to used when updating a record.
- **transaction**: the transaction object to be used when updating a data.

Below is a sample on how to update a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = repository.Query<Order>(new { Id = 251 }).FirstOrDefault();
	if (order != null)
	{
		order.Quantity = 5;
		order.UpdateDate = DateTime.UtcNow;
		var affectedRows = repository.Update(order);
	}

Dynamic way (column-based update), or see InlineUpdate documentation:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var affectedRows = repository.InlineUpdate<Order>(new { Quantity = 5, UpdatedDate = DateTime.UtcNow }, new { Id = 251 });

**Note**:  Updating a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

Delete Operation
----------------

.. highlight:: c#

This operation is used to delete an existing record from the database. It returns an `int` value indicating the number of rows affected by the delete.

Below are the parameters:

- **where**: an expression to used when deleting a record.
- **transaction**: the transaction object to be used when deleting a data.

Below is a sample on how to delete a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = repository.Query<Order>(new { Id = "251" }).FirstOrDefault();
	if (order != null)
	{
		var affectedRows = repository.Delete(order);
	}


or by `PrimaryKey`

::

	var affectedRows = repository.Delete<Order>(order.Id);


Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var affectedRows = repository.Delete<Order>(new { Id = "251" });

**Note**: Deleting a record using `PrimaryKey` will throw a `PrimaryFieldNotFoundException` exception back to the caller if the `PrimaryKey` is not found from the entity.

Merge Operation
---------------

.. highlight:: c#

This operation is used to merge an entity from the existing record from the database. It returns an `int` value indicating the number of rows affected by the merge.

Below are the parameters:

- **entity**: the entity object to be merged.
- **qualifiers**: the list of fields to be used as a qualifiers when merging a record.
- **transaction**: the transaction object to be used when merging a data.

Below is a sample on how to merge a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var order = repository.Query<Order>(1);
	order.Quantity = 5;
	UpdatedDate = DateTime.UtcNow;
	repository.Merge(order, Field.Parse(new { order.Id }));

**Note**: The merge is a process of updating and inserting. If the data is present in the database using the qualifiers, then the existing data will be updated, otherwise, a new data will be inserted in the database.

InlineUpdate Operation
----------------------

.. highlight:: c#

This operation is used to do a column-based update of an existing record from the database. It returns an `int` value indicating the number of rows affected by the updates.

Below are the parameters:

- **entity**: the dynamically or entity driven data entity object that contains the target fields to be updated.
- **where**: an expression to used when updating a record.
- **transaction**: the transaction object to be used when updating a data.

Below is a sample on how to update a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var affectedRows = repository.InlineUpdate<Order>(new { Quantity = 5, UpdatedDate = DateTime.UtcNow }, new { Id = "251" });

The code snippets above will update the `Quantity` column of a order records from the dabatase where the value of `Id` column is equals to `251`.

BulkInsert Operation
--------------------

.. highlight:: c#

This operation is used to bulk-insert the entities to the database. It returns an `int` value indicating the number of rows affected by the bulk-inserting.

Below are the parameters:

- **entities**: the list of entities to be inserted.
- **transaction**: the transaction object to be used when doing bulk-insert.

Below is a sample on how to do bulk-insert.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var entities = new List<Order>();
	entities.Add(new Order()
	{
		Id = 251,
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	});
	entities.Add(new Stock()
	{
		Id = 251,
		Quantity = 25,
		ProductId = 15,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	});
	var affectedRows = repository.BulkInsert(entities);

Count and CountBig Operation
----------------------------

.. highlight:: c#

These operations are used to count the number of records from the database. It returns a value indicating the number of counted rows based on the created expression.

Below are the parameters:

- **where**: an expression to used when counting a record. If left `null`, all records from the database will be counted.
- **transaction**: the transaction object to be used when updating a data.

Below is a sample on how to count a data.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var rows = repository.Count<Order>();

The code snippets above will count all the `Order` records from the database.

Below is the sample way to count a records with expression

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var rows = repository.Count<Order>(new { CustomerId = 10045 });

Above code snippets will count all the `Order` records from the database where `CustomerId` is equals to `10045`.

**Note**: The same operation applies to `CountBig` operation. The only difference is that, `CountBig` is returning a `System.Int64` type and the internal SQL statetement is using the `COUNT_BIG` keyword.

BatchQuery Operation
--------------------

.. highlight:: c#

This operation is used to batching when querying a data from the database. It returns an enumerable object of `RepoDb.Interfaces.IDataEntity` objects.

Below are the parameters:

- **where**: an expression to used to filter the data.
- **page**: a zero-based index that signifies the page number of the batch to query.
- **rowsPerBatch**: the number of rows to be returned per batch.
- **orderBy**: the list of fields to be used to sort the data during querying.
- **transaction**: the transaction object to be used when querying a data.

Below is a sample on how to query the first batch of data from the database where the number of rows per batch is 24.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	repository.BatchQuery<Order>(0, 24);

Below is the way to query by batch the data with expression.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	repository.BatchQuery<Order>(new { CustomerId = 10045, 0, 24);

Batching is very important when you are lazy-loading the data from the database. Below is a sample event listener for scroll (objects), doing the batch queries and post-process the data.

::

	var scroller = <Any Customimzed Scroller Object>
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var page = 0;
	var rowsPerBatch = 24;

	scroller.ScrollToEnd += (o, e) =>
	{
		var result = repository.BatchQuery<Order>(new { CustomerId = 10045 }, page, rowsPerBatch);
		Process(result);
		page++;
	};

	void Process(IEnumerable<Order> orders)
	{
		// Process the orders (display on the page)
	}

ExecuteQuery Operation
----------------------

.. highlight:: c#

This connection extension method is used to execute a SQL Statement query from the database in fast-forward access. It returns an `IEnumerable` object with `dynamic` or `RepoDb.Interfaces.IDataEntity` type as its generic type.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution. It could be an entity class or a dynamic object.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.

Below is the way on how to call the operation.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var param = new { CustomerId = 10045 };
	var result = repository.ExecuteQuery<Order>("SELECT * FROM [dbo].[Stock] WHERE CustomerId = @CustomerId;", param);

ExecuteNonQuery Operation
-------------------------

.. highlight:: c#

This connection extension method is used to execute a non-queryable SQL statement query. It returns an `int` that holds the number of affected rows during the execution.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.

Below is the way on how to call the operation.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var param = new
	{
		CustomerId = 10045,
		Quantity = 5,
		UpdatedDate = DateTime.UtcNow
	};
	var result = repository.ExecuteNonQuery("UPDATE [dbo].[Stock] SET Quantity = @Quantity, UpdatedDate = @UpdatedDate WHERE CustomerId = @CustomerId;", param);

ExecuteScalar Operation
-----------------------

.. highlight:: c#

This connection extension method is used to execute a query statement that returns a single value.

Below are the parameters:

- **commandText**: the SQL statement to be used for execution.
- **param**: the parameters to be used for the execution.
- **commandTimeout**: the command timeout in seconds to be used when executing the query in the database.
- **commandType**: the type of command to be used whether it is a `Text`, `StoredProcedure` or `TableDirect`.
- **transaction**: the transaction object be used when executing the command.

Below is the way on how to call the operation.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var param = new { CustomerId = 10045 };
	var id = repository.ExecuteScalar("SELECT MAX([Id]) AS MaxIdByCustomerId FROM [dbo].[Stock] CustomerId = @CustomerId;", param);


Working with Cache
==================

.. highlight:: c#

The library supports caching when querying a data from the database. By the default, the `RepoDb.MemoryCache` is being used by the library. Given the name itself, the library works with memory caching by default. A cache is only working on `Query` operation of the repository. It can be access through the repository `Cache` property (implements `RepoDb.Interfaces.ICache` interface).

A cache key is important in order for the caching to cache the object. It should be unique to every cache item.

Below are the methods of `ICache` object.

- **Add**: accepts an `item` or a `key` and `value` pair parameters. It adds an item to the `Cache` object. If an item is already existing, the item will be overriden.
- **Clear**: clear all items from the cache.
- **Contains**: accepts a `key` parameter. Checks whether the `Cache` object contains an item with the defined key.
- **Get**: accepts a `key` parameter. Returns a cached item object.
- **GetEnumerator**: returns an enumerator for `IEnumerable<ICacheItem>` objects. It contains all the cached items from the `Cache` object.
- **Remove**: accepts a `key` parameter. Removes an entry from the `Cache` object.

One important object when manipulating a cache is the `CacheItem` object (implements `RepoDb.Interfaces.ICacheItem`). It acts as the cache item entry for the cache object.

Below are the constructor arguments of the `ICacheItem` object.

- **key**: the key of the cache.
- **value**: the value object of the cache.

Below are the properties of `ICacheItem` object.

- **Key**: the key of the cache. It returns a `System.String` type.
- **Value**: the cached object of the item. It returns a `System.Object` type. It can be casted back to a defined object type.
- **Timestamp**: the time of the cache last refreshed. It returns a `System.DateTime` object.

The repository caching operation is of the `pseudo` below.

.. highlight:: none

::

	VAR item = null
	IF ($cacheKey is not null) THEN
		set $item = get value from cache where the key equals to $cacheKey
		IF ($item is not null) THEN
			RETURN item
		END IF
	END IF
	VAR $result = query the data from the database
	IF ($result is not null AND $cacheKey is not null) THEN
		Add cache item where:
			Key = $cacheKey
			Value = $result
	END IF
	RETURN $result

Below is the way on how to query and cache the `Stock` data from the database with caching enabled.

Creating a Cache Entry
----------------------

The snippets below declared a variable named `cacheKey`. The value of this variable acts as the key value of the items to be cached by the repository.

.. highlight:: c#

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var cacheKey = "CacheKey.Customers.StartsWith.Anna";
	var result = repository.Query<Customer>(new { Name = new { Operation = Operation.Like, Value = "Anna%" } }, cacheKey: cacheKey);

First, it wil query the data from the database where the `Name` is started at `Anna`. Then, the operation will cache the result into the `Cache` object with the given key at the variable named `cacheKey` (valued `CacheKey.Customers.StartsWith.Anna`).

The next time the same query is executed, the repository automatically returns the cached item if the same key is passed.

Please note that the cache object of the repository is immutable per instance, this means that accessing the cache object directly passing the same cache key would return the same result.

Codes below will return the same result as above assuming the same repository object is used.

::

	var customers = (IEnumerable<Customer>)repository.Cache.Get("CacheKey.Customers.StartsWith.Anna");

Checking a Cache Entry
----------------------

.. highlight:: c#

Code below is the way on how to check if the cached item is present on the `Cache` object, assuming that a repository object has been created already.

::

	var isExists = repository.Cache.Contains("CacheKey");

Iterating the Cache Entries
---------------------------

.. highlight:: c#

Code below is the way on how to retrieve or iterate all the cached items from the `Cache` object, assuming that a repository object has been created already.

::

	// Let's expect that the repository is meant for Customer data entity
	foreach (var item in repository.Cache)
	{
		var item = (IEnumerable<Customer>)item;
		// Process the item here
	}

Removing or Clearing a Cache
----------------------------

.. highlight:: c#

By default, the library does not support the auto-flush of the cache. Those forcing the developers to handle the flushing on its way.

Clearing or removing an entry from a cache is the only way to flush the cached objects.

See below on how to clear the cached item from the `Cache` object, assuming that a repository object has been created already.

::

	repository.Cache.Clear();

Below is the way to remove specific cache item.

::

	repository.Cache.Remove("CacheKey");


Injecting a Custom Cache Object
-------------------------------

.. highlight:: c#

The library supports a cache object injection in the repository level. As mentioned earlier, by default, the library is using the `RepoDb.MemoryCache` object. It can overriden by creating a class and implements the `RepoDb.Interfaces.ICache` interface, and passed it to the `cache` argument of the repository constructor.

Below is the way on how to create a custom `Cache` object.

::

	public class FileCache : ICache
	{
		public FileCache(string location)
		{
			// Add a logic on the constructor
		}

		public void Add(string key, object value)
		{
			// Serialize to a File
		}

		public void Add(ICacheItem item)
		{
			// Serialize to a File
		}

		public void Clear()
		{
			// Delete the Files
		}

		public bool Contains(string key)
		{
			// Check if the Filename exists by Key
		}

		public object Get(string key)
		{
			// Deserialize the File where the FileName is equals to Key, return the object
		}

		public IEnumerator<ICacheItem> GetEnumerator()
		{
			// Get the File.ParentFolder.Files enumerator and deserialize each file
		}

		public void Remove(string key)
		{
			// Delete the File where the FileName is equals to Key
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			// Get the File.ParentFolder.Files enumerator and deserialize each file
		}
	}

Below is the way on how to inject the custom cache to a repository.

::

	var fileCache = new FileCache();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"
		0, // commandTimeout
		fileCache, // cache
		null, // trace
		null, // statementBuilder
	);

The snippets above creates a class named `FileCache` that implements the `ICache` interfaces. By implementing the said interface, the class is now qualified to become a library `Cache` object.

Upon creating a repository, the `fileCache` variable is being passed in the `cache` parameter. This signals the repository to use the `FileCache` class as the cache manager object of the `Query` operation.

**Note:** The caller can activate a debugger on the `FileCache` class to enable debugging. When the callers call the `Query` method and passed a `cacheKey` value on it, the breakpoint will be hit by the debugger if it is placed inside `Add` method of the `FileCache` class.


Working with Trace
==================

.. highlight:: c#

One of the unique built-in feature of the library is tracing. It allows developers to do debugging or tracing on the operations while executing it against the database.

With tracing, the developers can able to create its own `Trace` object and inject in the repository.

ITrace Interface
----------------

This interface is the heart of library's tracing feature. It resides from `RepoDb.Interfaces` namespace. This interface is required to be implemented at the custom trace classes to enable the tracing, then, the custom class must be injected in the repository.

The `ITrace` interface has the follow trace methods.

- **AfterBatchQuery**: called after the `Repository.BatchQuery` operation has been executed.
- **AfterBulkInsert**: called after the `Repository.BulkInsert` operation has been executed.
- **AfterCount**: called after the `Repository.Count` operation has been executed.
- **AfterCountBig**: called after the `Repository.CountBig` operation has been executed.
- **AfterDelete**: called after the `Repository.Delete` operation has been executed.
- **AfterExecuteNonQuery**: called after the `Repository.ExecuteNonQuery` operation has been executed.
- **AfterExecuteQuery**: called after the `Repository.ExecuteQuery` operation has been executed.
- **AfterExecuteReader**: called after the `Repository.ExecuteReader` operation has been executed.
- **AfterExecuteScalar**: called after the `Repository.ExecuteScalar` operation has been executed.
- **AfterInlineUpdate**: called after the `Repository.InlineUpdate` operation has been executed.
- **AfterInsert**: called after the `Repository.Insert` operation has been executed.
- **AfterMerge**: called after the `Repository.Merge` operation has been executed.
- **AfterQuery**: called after the `Repository.Query` operation has been executed.
- **AfterUpdate**: called after the `Repository.Update` operation has been executed.
 
Note: All trace methods mentioned above accepts the parameter named `log` of type `RepoDb.Interfaces.ITraceLog`.
 
- **BeforeBatchQuery**: called before the `Repository.BatchQuery` actual execution.
- **BeforeBulkInsert**: called before the `Repository.BulkInsert` actual execution.
- **BeforeCount**: called before the `Repository.Count` actual execution.
- **BeforeCountBig**: called before the `Repository.CountBig` actual execution.
- **BeforeDelete**: called before the `Repository.Delete` actual execution.
- **BeforeExecuteNonQuery**: called before the `Repository.ExecuteNonQuery` actual execution.
- **BeforeExecuteQuery**: called before the `Repository.ExecuteQuery` actual execution.
- **BeforeExecuteReader**: called before the `Repository.ExecuteReader` actual execution.
- **BeforeExecuteScalar**: called before the `Repository.ExecuteScalar` actual execution.
- **BeforeInlineUpdate**: called before the `Repository.InlineUpdate` actual execution.
- **BeforeInsert**: called before the `Repository.Insert` actual execution.
- **BeforeMerge**: called before the `Repository.Merge` actual execution.
- **BeforeQuery**: called before the `Repository.Query` actual execution.
- **BeforeUpdate**: called before the `Repository.Update` actual execution.
 
Note: All trace methods mentioned above accepts the parameter named `log` of type `RepoDb.Interfaces.ICancellableTraceLog`.

ITraceLog Interface
-------------------

This interface and deriving objects are used by the `RepoDb.Interfaces.ITrace` object as a method argument during the `After` operations.

Below are the properties of `ITraceLog` object.

- **Method**: a `System.Reflection.MethodBase` object that holds the pointer to the actual method that triggers the execution of the operation.
- **Result**: an object that holds the result of the execution.
- **Parameter**: an object that defines the parameters used when executing the operation.
- **Statement**: the actual query statement used in the execution.
- **ExecutionTime**: a `System.Timespan` object that holds the time length of actual execution.

ICancellableTraceLog Interface
------------------------------

This interface and deriving objects are used by the `RepoDb.Interfaces.ITrace` object as a method argument at the `Before` operations. It inherits the `RepoDb.Interfaces.ITrace` interface.

Below are the properties of `ICancellableTraceLog` object.

- **IsCancelled**: a property used to identify whether the operation is canceled.
- **IsThrowException**: a property used to identify whether an exception is thrown after cancelation. Exception being thrown is of type `RepoDb.Exceptions.CancelledExecutionException`.

Creating a Custom Trace Object
------------------------------
 
.. highlight:: c#

Below is a sample customized `Trace` object.

::

	public class NorthwindDatabaseTrace : ITrace
	{
		public void BeforeBatchQuery(ICancellableTraceLog log)
		{
			throw new NotImplementedException();
		}

		public void AfterBatchQuery(ITraceLog log)
		{
			throw new NotImplementedException();
		}

		public void BeforeBulkInsert(ICancellableTraceLog log)
		{
			throw new NotImplementedException();
		}

		public void AfterBulkInsert(ITraceLog log)
		{
			throw new NotImplementedException();
		}

		...
	}

Below is the way on how to inject a Trace class in the repository.

::

	var trace = new NorthwindDatabaseTrace();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"
		0, // commandTimeout
		null, // cache
		trace, // trace
		null, // statementBuilder
	);

Once the customized Trace object has been injected, a breakpoint can be placed in any of the methods of the custom Trace class, it is debug-gable once the debugger hits the breakpoint.

Canceling an Operation
----------------------

To cancel an operation, simply call the method named `Cancel` of type `RepoDb.Interfaces.ICancelableTraceLog` in any `Before` operation.

Below is a sample code that calls the `Cancel` method of the `BeforeQuery` operation if any of the specified keywords from the variable named `keywords` is found from the statement.

.. highlight:: c#

::

	public void BeforeQuery(ICancellableTraceLog log)
	{
		var keywords = new[] { "INSERT", "DELETE", "UPDATE", "DROP", "MERGE", "ALTER" };
		if (keywords.Any(keyword => log.Statement.Contains(keyword)))
		{
			Console.WriteLine("A suspicious statement has been injected on the Query operations.");
			log.Cancel(true);
		}
	}

By passing the value of `true` in the parameter when calling the `Cancel` method, it would signal the library to throw an `RepoDb.Exception.CancelledExecutionException` exception object back to the caller.

Working with StatementBuilder
=============================

The library supports statement building injection, allowing the developers to override the default query statement the library is using. By default, the library is using the `RepoDb.SqlDbStatementBuilder` class that implements the `RepoDb.Interfaces.IStatementBuilder` interface.

In order to override the statement builder, the developer must create a class that implements the `RepoDb.Interfaces.IStatementBuilder` interface. This allows the class to be injectable in the repository and implements the necessary methods needed by all operations.

A `QueryBuilder` object comes along the way when the custom statement builder is being created. This object is a helper object when composing the actual SQL Statements. See the `QueryBuilder` documentation.

Below are the methods of the `IStatementBuilder` interface.

- **CreateBatchQuery**: called when creating a `BatchQuery` statement.
- **CreateCount**: called when creating a `Count` statement.
- **CreateCountBig**: called when creating a `CountBig` statement.
- **CreateDelete**: called when creating a `Delete` statement.
- **CreateInlineUpdate**: called when creating a `InlineUpdate` statement.
- **CreateInsert**: called when creating a `Insert` statement.
- **CreateMerge**: called when creating a `Merge` statement.
- **CreateQuery**: called when creating a `Query` statement.
- **CreateUpdate**: called when creating a `Update` statement.

QueryBuilder Object
-------------------

.. highlight:: none

A query builder is an helper object used when creating a query statement in the statement builders. It contains important methods that is very useful to fluently construct the statement.

By default, the library is using the `RepoDb.QueryBuilder<TEntity>` object(implements the `RepoDb.Interfaces.IQueryBuilder<TEntity>` when composing the statement.

Below is a sample code that creates a SQL Statement for the `Query` operation for `Oracle` data provide.

::

	public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0, IEnumerable<IOrderField> orderBy = null) where TEntity : IDataEntity
	{
		// Create an initial SELECT statement
		queryBuilder.Clear()
			.Select()
			.Fields(Command.Query)
			.From()
			.Table(Command.Query);
            
		// Add all fields for WHERE
		if (where != null)
		{
			queryBuilder.Where(where);
		}
            
		// Add the LIMIT (TOP in SQL Server)
		if (top > 0)
		{
			// In Oracle, SELECT [Fields] FROM [Table] WHERE [Fields] AND ROWNUM <=(Rows)
			queryBuilder.WriteText($"AND (ROWNUM <= {top})");
		}
            
		// End the builder
		queryBuilder.End();

		// Return the Statement
		return queryBuilder.ToString();
	}

The methods of this object might be limited as it varies on the target data provider to be implemented. This object is open for modification soon for extensibility support. We are happy to open this to become inherittable in the near future if this is necessary for the .NET community.

**Note**: The query builder is not inheritable and we suggest not to create a customized query builder.

CreateBatchQuery Method
-----------------------

.. highlight:: none

This method is being called when the `BatchQuery` operation of the repository is being called.

Below are the arguments of `CreateBatchQuery` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **where**: the expression used when creating a statement (of type `RepoDb.Interfaces.IQueryGroup`).
- **page**: the page number implied when creating a statement.
- **rowsPerBatch**: the size of the rows implied when creating a statement.
- **orderBy**: the fields used in the `ORDER BY` when creating a statement.

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateBatchQuery` method.

::

	public string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int page, int rowsPerBatch, IEnumerable<IOrderField> orderBy) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.With()
			.WriteText("CTE")
			.As()
			.OpenParen()
			.Select()
			.RowNumber()
			.Over()
			.OpenParen()
			.OrderBy(orderBy)
			.CloseParen()
			.As("[RowNumber],")
			.Fields(Command.BatchQuery)
			.From()
			.Table(Command.BatchQuery)
			.Where(where)
			.CloseParen()
			.Select()
			.Fields(Command.BatchQuery)
			.From()
			.WriteText("CTE")
			.WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
			.OrderBy(orderBy)
			.End();
		return queryBuilder.GetString();
	}

CreateCount Method
------------------

.. highlight:: none

This method is being called when the `Count` operation of the repository is being called.

Below are the arguments of `CreateCount` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **where**: the expression used when creating a statement (of type `RepoDb.Interfaces.IQueryGroup`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateCount` method.

::

	public string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.Count()
			.WriteText("(*) AS [Counted]")
			.From()
			.Table(Command.Count)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateCountBig Method
---------------------

.. highlight:: none

This method is being called when the `CountBig` operation of the repository is being called.

Below are the arguments of `CreateCountBig` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **where**: the expression used when creating a statement (of type `RepoDb.Interfaces.IQueryGroup`).

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateCountBig` method.

::

	public string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.CountBig()
			.WriteText("(*) AS [Counted]")
			.From()
			.Table(Command.CountBig)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateDelete Method
-------------------

.. highlight:: none

This method is being called when the `Delete` operation of the repository is being called.

Below are the arguments of `CreateDelete` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **where**: the expression used when composing a statement (of type `RepoDb.Interfaces.IQueryGroup`).

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateDelete` method.

::

	public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Delete()
			.From()
			.Table(Command.Delete)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateInlineUpdate Method
-------------------------

.. highlight:: none

This method is being called when the `InlineUpdate` operation of the repository is being called.

Below are the arguments of `CreateInlineUpdate` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **fields**: the list of fields to be updated when composing a statement (on enumerable of type `RepoDb.Interfaces.Field`).
- **where**: the expression used when composing a statement (of type `RepoDb.Interfaces.IQueryGroup`).
- **overrideIgnore**: the flag used to identify whether all the ignored fields will be included in the operation when composing a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInlineUpdate` method.

::

	public string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields, IQueryGroup where, bool? overrideIgnore = false) where TEntity : IDataEntity
	{
		if (overrideIgnore == false)
		{
			var properties = PropertyCache.Get<TEntity>(Command.InlineUpdate)
				.Select(property => property.GetMappedName());
			var unmatches = fields?.Where(field =>
				properties?.FirstOrDefault(property =>
					field.Name.ToLower() == property.ToLower()) == null);
			if (unmatches?.Count() > 0)
			{
				throw new InvalidOperationException($"The following columns ({unmatches.Select(field => field.AsField()).Join(", ")}) " +
					$"are not updatable for entity ({DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate)}).");
			}
		}
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Update()
			.Table(Command.InlineUpdate)
			.Set()
			.FieldsAndParameters(fields)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateInsert Method
-------------------

.. highlight:: none

This method is being called when the `Insert` operation of the repository is being called.

Below are the arguments of `CreateInsert` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInsert` method.

::

	public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		var primary = PrimaryPropertyCache.Get<TEntity>();
		queryBuilder
			.Clear()
			.Insert()
			.Into()
			.Table(Command.Insert)
			.OpenParen()
			.Fields(Command.Insert)
			.CloseParen()
			.Values()
			.OpenParen()
			.Parameters(Command.Insert)
			.CloseParen()
			.End();
		if (primary != null)
		{
			var result = primary.IsIdentity() ? "SCOPE_IDENTITY()" : $"@{primary.GetMappedName()}";
			queryBuilder
				.Select()
				.WriteText(result)
				.As("[Result]")
				.End();
		}
		return queryBuilder.GetString();
	}

CreateMerge Method
------------------

.. highlight:: none

This method is being called when the `Merge` operation of the repository is being called.

Below are the arguments of `CreateMerge` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **qualifiers**: the list of fields to be used as a qualifiers when composing a statement (on enumerable of type `RepoDb.Interfaces.Field`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateMerge` method.

::

	public string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		if (qualifiers == null)
		{
			var primaryKey = PrimaryPropertyCache.Get<TEntity>();
			if (primaryKey != null)
			{
				qualifiers = new Field(primaryKey.Name).AsEnumerable();
			}
		}
		queryBuilder
			.Clear()
			// MERGE T USING S
			.Merge()
			.Table(Command.Merge) 
			.As("T")
			.Using()
			.OpenParen()
			.Select()
			.ParametersAsFields(Command.None) // All fields must be included for selection
			.CloseParen()
			.As("S")
			// QUALIFIERS
			.On()
			.OpenParen()
			.WriteText(qualifiers?
				.Select(
					field => field.AsJoinQualifier("S", "T"))
						.Join($" {Constant.And.ToUpper()} "))
			.CloseParen()
			// WHEN NOT MATCHED THEN INSERT VALUES
			.When()
			.Not()
			.Matched()
			.Then()
			.Insert()
			.OpenParen()
			.Fields(Command.Merge)
			.CloseParen()
			.Values()
			.OpenParen()
			.Parameters(Command.Merge)
			.CloseParen()
			// WHEN MATCHED THEN UPDATE SET
			.When()
			.Matched()
			.Then()
			.Update()
			.Set()
			.FieldsAndAliasFields(Command.Merge, "S")
			.End();
		return queryBuilder.GetString();
	}

CreateQuery Method
------------------

.. highlight:: none

This method is being called when the `Query` operation of the repository is being called.

Below are the arguments of `CreateQuery` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **where**: the expression used when composing a statement (of type `RepoDb.Interfaces.IQueryGroup`).
- **top**: the value that identifies the number of rows to be returned when composing a statement.
- **orderBy**: the fields used in the `ORDER BY` when creating a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateQuery` method.

::

	public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0, IEnumerable<IOrderField> orderBy = null) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.Top(top)
			.Fields(Command.Query)
			.From()
			.Table(Command.Query)
			.Where(where)
			.OrderBy(orderBy)
			.End();
		return queryBuilder.GetString();
	}

CreateUpdate Method
-------------------

.. highlight:: none

This method is being called when the `Update` operation of the repository is being called.

Below are the arguments of `CreateUpdate` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.Interfaces.IQueryBuilder<TEntity>`).
- **where**: the expression used when composing a statement (of type `RepoDb.Interfaces.IQueryGroup`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateUpdate` method.

::

	public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Update()
			.Table(Command.Update)
			.Set()
			.FieldsAndParameters(Command.Update)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

Creating a custom Statement Builder
-----------------------------------

.. highlight:: none

The main reason why the library supports the statement builder is to allow the developers override the default statement builder of the library. By default, the library statement builder is only limited for SQL Server providers (as SQL Statements). However, it will fail if the library is being used to access the Oracle, MySql or any other providers.

To create a custom statement builder, simply create a class and implements the `RepoDb.Interfaces.IStatementBuilder` interface.

::
	
	public class OracleDbStatementBuilder : IStatementBuilder
	{
		public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0,
			IEnumerable<IOrderField> orderBy = null) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int page,
			int rowsPerBatch, IEnumerable<IOrderField> orderby) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields, IQueryGroup where, bool? overrideIgnore = false) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
		{
			throw new NotImplementedException();
		}
	}

Once the custom statement builder is created, it then can be used as an injectable object into the repository. See sample below injecting a statement builder for Oracle provider.

::

	var statementBuilder = new OracleDbStatementBuilder();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"
		0, // commandTimeout
		null, // cache
		null, // trace
		statementBuilder, // statementBuilder
	);

With the code snippets above, everytime the repository operation methods is being called, the `OracleStatementBuilder` corresponding method will be executed.

Mapping a Statement Builder
---------------------------

.. highlight:: c#

By default, the library is using the `RepoDb.SqlDbStatementBuilder` object for the statement builder. As discussed above, when creating a custom statement builder, it can then be injected as an object in the repository. However, if the developer wants to map the statement builder by provider level, this feature comes into the play.

The mapper is of static type `RepoDb.StatementBuilderMapper`.

The following are the methods of this object.

- **Get**: returns the instance of statement builder by type (of type `System.Data.IDbConnection`).
- **Map**: maps the custom statement builder to a type (of type `System.Data.IDbConnection`).

Mapping a statement builder enables the developer to map the custom statement builder by provider level. 

Let say for example, if the developers created the following repositories:

 - CustomerRepository (for `SqlConnection`)
 - ProductRepository (for `SqlConnection`)
 - OrderRepository (for `OracleConnection`)
 - CompanyRepository (for `OleDbConnection`)

Then, by mapping a custom statement builders, it will enable the library to summon the statement builder based on the provider of the repository. With the following repositories defined above, the developers must implement atleast two (2) custom statement builder (one for Oracle provider and one for OleDb provider).

Let say the developer created 2 new custom statement builders named:

 - OracleStatementBuilder
 - OleDbStatementBuilder

The developers can now map the following statement builders into the repositories by provider level. Below is the sample way on how to do it.

::

	StatementBuilderMapper.Map(typeof(OracleConnection), new OracleStatementBuilder());
	StatementBuilderMapper.Map(typeof(OleDbConnection), new OleDbStatementBuilder());

The object `StatementBuilderMapper.Map` is callable everywhere in the application as it was implemented in s static way. Make sure to call it once, or else, an exception will be thrown.
