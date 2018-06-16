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

An entity class is a DTO class used to feed the operations of the repositories. See documentation below.

.. toctree::

   DataEntity/DataEntity
   DataEntity/IDataEntity

.. highlight:: c#

See sample codes below:

::

	public interface ICustomer : IDataEntity
	{
		int Id { get; set; }
		string Name { get; set; }
		DateTime CreatedDate { get; set; }
	}

	public class Customer : DataEntity, ICustomer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
	}

Mapping with MapAttribute
=========================

A `MapAttribute` is used to define a mapping of the current `Class` or `Property` equivalent to an `Object` or `Field` name in the database. Located at `RepoDb.Attributes` namespace.

.. toctree::

   MapAttribute/ClassMapping
   MapAttribute/PropertyMapping

The `Map` attribute second parameter is a command type `of type System.Data.CommandType`. If this parameter is defined, the repository operation execution will then be of that command type. This parameter is only working at the class level implementation.

See Microsoft documentation for `System.Data.CommandType`_ here.

.. _System.Data.CommandType: https://msdn.microsoft.com/en-us/library/system.data.commandtype%28v=vs.110%29.aspx

Primary Attribute
=================

.. highlight:: c#

A `PrimaryAttribute` is used to define the class property as primary property of the `DataEntity` object. Located at `RepoDb.Attributes` namespace.

See sample codes below:

::

	public class Employee : DataEntity
	{
		[Primary]
		public int Id { get; set; }
	}

Defining an Identity Property
-----------------------------

To define an identity property, simply sets the `isIdentity` parameter of the `PrimaryAttribute` during the implementation.

See sample codes below:

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

See sample codes below:

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

The latter is the **heart** of the library as it contains all the operations that is being used by all other repositories within or outside the library.

.. toctree::

   Repository/DbRepository
   Repository/BaseRepository


DbRepository
------------

BaseRepository
--------------

Creating a Repository
=====================

Creating a Connection
=====================

Connection.EnsureOpen
---------------------

Connection.ExecuteReader
------------------------

Connection.ExecuteQuery
-----------------------

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

