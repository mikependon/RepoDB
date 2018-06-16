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
		public int Id { get; set; }
		...
	}

IDataEntity Interface
---------------------

.. highlight:: c#

An interface used to implement to mark a class to be the base class of all `Data Entity` object. Located at `RepoDb.Interfaces` namespace.

::

	public interface IClassName : IDataEntity
	{
		int Id { get; set; }
		...
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

Working wth Repository
======================

The library contains two repository objects, the `RepoDb.BaseRepository<TEntity, TDbConnection>` and the `RepoDb.DbRepository<TDbConnection>`.

The latter is the heart of the library as it contains all the operations that is being used by all other repositories within or outside the library.

DbRepository Class
------------------

.. highlight:: c#

A base object for all shared-based repositories.

::

	public interface INorthwindDbRepository : IDbRepository<SqlConnection>
	{
	}

	public class NorthwindDbRepository : DbRepository<SqlConnection>
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}


See the documentation below.

.. toctree::

   Repository/DbRepository

BaseRepository Class
--------------------

.. highlight:: c#

An abstract class for all entity-based repositories.

::

	public interface ICustomerRepository : IBaseRepository<Customer, SqlConnection>
	{
	}

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>, ICustomerRepository
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

See the documentation below.

.. toctree::

   Repository/BaseRepository


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
		new QueryField("CustomerId", Operation.GreaterThanOrEqual, 10045),
		new QueryField("CreatedDate", Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3)),
		null, // Child QueryGroups
		Conjunction.And
	);

Dynamic Query Expression
------------------------

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
		new QueryField("CustomerId", Operation.GreaterThanOrEqual, 10045),
		new QueryField("CreatedDate", Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3)),
		null, // Child QueryGroups
		Conjunction.And
	);

Query Operations
----------------

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

Part of the expression tree used to determine the `equality` of the field and data.

Dynamic way:

::

	var result = repository.Query(new { Id = 10045 });

or

::

	var result = repository.Query(new
	{
		Id = new { Operation = Operation.Equal, Value = 10045 }
	});

Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.Equal, 10045 ));

Operation.NotEqual
------------------

Part of the expression tree used to determine the `inequality` of the field and data.

Dynamic way:

::

	var result = repository.Query(new
	{
		Name = new { Operation = Operation.NotEqual, Value = "Anna Fullerton" }
	});

Explicit way:

::

	var result = repository.Query(new QueryField("Name", Operation.NotEqual, "Anna Fullerton" });

Operation.LessThan
------------------

Part of the expression tree used to determine whether the field value is `less than` of the defined value.

Dynamic way:

::

	var result = repository.Query(new { Id = new { Operation = Operation.LessThan, Value = 100 } });


Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.LessThan, 100 });

Operation.GreaterThan
---------------------

Part of the expression tree used to determine whether the field value is `greater than` of the defined value.

Dynamic way:

::

	var result = repository.Query(new { Id = new { Operation = Operation.GreaterThan, Value = 0 } });

Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.GreaterThan, 0 });

Operation.LessThanOrEqual
-------------------------

Part of the expression tree used to determine whether the field value is `less than or equal` of the defined value.

Dynamic way:

::

	var result = repository.Query(new { Id = new { Operation = Operation.LessThanOrEqual, Value = 100 } });

Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.LessThanOrEqual, 100 });

Operation.GreaterThanOrEqual
----------------------------

Part of the expression tree used to determine whether the field value is `greater than or equal` of the defined value.

Dynamic way:

::

	var result = repository.Query(new { Id = new { Operation = Operation.GreaterThanOrEqual, Value = 0 } });

Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.GreaterThanOrEqual, 0 });

Operation.Like
--------------

Part of the expression tree used to determine whether the field is `identitical` to a given value.

Dynamic way:

::

	var result = repository.Query(new { Name = new { Operation = Operation.Like, Value = "Anna%" } });

Explicit way:

::

	var result = repository.Query(new QueryField("Name", Operation.Like, "Anna%" });

Operation.NotLike
-----------------

Part of the expression tree used to determine whether the field is `not identitical` to a given value. An opposite of `Operation.Like`.

Dynamic way:

::

	var result = repository.Query(new { Name = new { Operation = Operation.NotLike, Value = "Anna%" } });

Explicit way:

::

	var result = repository.Query(new QueryField("Name", Operation.NotLike, "Anna%" });

Operation.Between
-----------------

Part of the expression tree used to determine whether the field value is `between` 2 given values.

Dynamic way:

::

	var result = repository.Query(new { CreatedDate = new { Operation = Operation.Between, Value = new [] { Date1, Date2 } } });

or

::

	var result = repository.Query(new { Id = new { Operation = Operation.Between, Value = new [] { 10045, 10075 } } });

Explicit way:

::

	var result = repository.Query(new QueryField("CreatedDate", Operation.Between, new [] { Date1, Date2 } });

or

::

	var result = repository.Query(new QueryField("Id", Operation.Between, new [] { 10045, 10075 } });

Operation.NotBetween
--------------------

Part of the expression tree used to determine whether the field value is `not between` 2 given values. An opposite of `Operation.Between`.

Dynamic way:

::

	var result = repository.Query(new { CreatedDate = new { Operation = Operation.NotBetween, Value = new [] { Date1, Date2 } } });

or

::

	var result = repository.Query(new { Id = new { Operation = Operation.NotBetween, Value = new [] { 10045, 10075 } } });

Explicit way:

::

	var result = repository.Query(new QueryField("CreatedDate", Operation.NotBetween, new [] { Date1, Date2 } });

or

::

	var result = repository.Query(new QueryField("Id", Operation.NotBetween, new [] { 10045, 10075 } });

Operation.In
------------

Part of the expression tree used to determine whether the field value is `in` given values.

Dynamic way:

::

	var result = repository.Query(new { Id = new { Operation = Operation.In, Value = new [] { 1, 2, 3, 4 } } });

Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.In, new [] { 1, 2, 3, 4 } });

Operation.NotIn
---------------

Part of the expression tree used to determine whether the field value is `not in` given values. An opposite of `Operation.In`. See sample below.

Dynamic way:

::

	var result = repository.Query(new { Id = new { Operation = Operation.NotIn, Value = new [] { 1, 2, 3, 4 } } });

Explicit way:

::

	var result = repository.Query(new QueryField("Id", Operation.NotIn, new [] { 1, 2, 3, 4 } });

Operation.All
-------------

Part of the expression tree used to determine whether `all` the field values satisfied the criteria.

Dynamic way:

::

	var result = repository.Query(new
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

	var result = repository.Query
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

Part of the expression tree used to determine whether `any` of the field values satisfied the criteria.

Dynamic way:

::

	var result = repository.Query(new
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

	var result = repository.Query
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

