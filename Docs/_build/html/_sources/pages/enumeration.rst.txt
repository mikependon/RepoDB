Enumeration
===========

The library supports the enumeration to be a class property in fetching, inserting and modifying the data from the database.

As String
---------

This is the default mapping, the value of enum is being saved as `String` in the database.

.. highlight:: c#

::

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT IDENTITY(1, 1) PRIMARY
		, [Name] NVARCHAR(128) NOT NULL
		, [Gender] NVARCHAR(8)
	);

::

	public enum Gender
	{
		Male,
		Female
	}

::

	[Map("[dbo].[Customer]")]
	public class Customer
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Gender Gender { get; set; }
	}

When the operations of like `Query`, `Insert`, `Merge`, `Update` is being invoked, the behavior of the `Gender` property will be passed as `Male` or `Female` in the database.

As Numeric
----------

The value of enum is being saved as a `Targetted-Typed` in the database.

.. highlight:: c#

::

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT IDENTITY(1, 1) PRIMARY
		, [Name] NVARCHAR(128) NOT NULL
		, [Gender] INT /* SMALLINT, BIGINT, BIT */
	);

::

	public enum Gender
	{
		Male = 1,
		Female = 2
	}

::

	[Map("[dbo].[Customer]")]
	public class Customer
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Gender Gender { get; set; }
	}

When the operations of like `Query`, `Insert`, `Merge`, `Update` is being invoked, the behavior of the `Gender` property will be passed as `1 for Male` or `2 for Female` in the database.

Unmapped Behaviors
------------------

Below are the list of unmapped behaviors.

**Database string values are not a part of the Enum**

1. The operation `Query` will **not find** the desired record from the database if the Enum is used as an expression.
2. The operation `Query` will **fail** if there are rows returned from the database; the value is not convertible.
3. The operation `Insert`, `Merge`, `Update` will **succeed** and will passed the Enum value instead; database record is not intact.

**Database numeric values are not a part of the Enum**

1. The operation `Query` will **not find** the desired record from the database if the Enum is used as an expression.
2. The operation `Query` will **succeed** if there are rows returned from the database; the value is not intact to Enum.
3. The operation `Insert`, `Merge`, `Update` will **succeed** will always be succeeded.

