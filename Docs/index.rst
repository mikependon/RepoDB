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

A repository is used to create a connection object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection())
	{
		// Use the connection here
	}

Connection.EnsureOpen
---------------------

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

Connection.ExecuteScalar
------------------------

Transaction
===========

Creating a Transaction
----------------------

Transaction Commit/Rollback
---------------------------

Expression Tree
===============

QueryGroup
==========

Operations
==========

Operation.Equal
---------------

Operation.NotEqual
------------------

Operation.LessThan
------------------

Operation.GreaterThan
---------------------

Operation.LessThanOrEqual
-------------------------

Operation.GreaterThanOrEqual
----------------------------

Operation.Like
--------------

Operation.NotLike
-----------------

Operation.Between
-----------------

Operation.NotBetween
--------------------

Operation.In
------------

Operation.NotIn
---------------

Operation.All
-------------

Operation.Any
-------------

Repository Operations
=====================

