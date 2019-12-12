Enumeration
===========

The library supports the enumeration to be a class property in fetching, inserting and modifying the data from the database.

As String
---------

This is the default mapping, the value of enum is being saved as `String` in the database.

.. code-block:: sql
	:linenos:

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT IDENTITY(1, 1) PRIMARY
		, [Name] NVARCHAR(128) NOT NULL
		, [Gender] NVARCHAR(8)
	);

.. code-block:: c#
	:linenos:

	public enum Gender
	{
		Male,
		Female
	}

.. code-block:: c#
	:linenos:

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

.. code-block:: sql
	:linenos:

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT IDENTITY(1, 1) PRIMARY
		, [Name] NVARCHAR(128) NOT NULL
		, [Gender] INT /* SMALLINT, BIGINT, BIT */
	);

.. code-block:: c#
	:linenos:

	public enum Gender
	{
		Male = 1,
		Female = 2
	}

.. code-block:: c#
	:linenos:

	[Map("[dbo].[Customer]")]
	public class Customer
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Gender Gender { get; set; }
	}

When the operations of like `Query`, `Insert`, `Merge`, `Update` is being invoked, the behavior of the `Gender` property will be passed as `1 for Male` or `2 for Female` in the database.

Property Mapping
----------------

This feature enables the library to force save the `Enum` on the desired database type, by targetting the specific class properties.

.. code-block:: sql
	:linenos:

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT IDENTITY(1, 1) PRIMARY
		, [Name] NVARCHAR(128) NOT NULL
		, [Gender] NVARCHAR(16)
	);

.. code-block:: c#
	:linenos:

	public enum Gender
	{
		Male = 1,
		Female = 2
	}

Below is the code to force the `Gender` enumeration to be saved as `INT` in the database, even the `Gender` column is on `NVARCHAR(16)` data type.

.. code-block:: c#
	:linenos:

	[Map("[dbo].[Customer]")]
	public class Customer
	{
		public long Id { get; set; }
		public string Name { get; set; }
		[TypeMap(DbType.Int32)]
		public Gender Gender { get; set; }
	}

**Note**: Enum will only succeed if it is convertible to the target database type.

Enum Mapping
------------

This feature enables the library to force save the `Enum` on the desired database type, by targetting the type of the `Enum`.

.. code-block:: sql
	:linenos:

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT IDENTITY(1, 1) PRIMARY
		, [Name] NVARCHAR(128) NOT NULL
		, [Gender] NVARCHAR(16)
	);

.. code-block:: c#
	:linenos:

	public enum Gender
	{
		Male = 1,
		Female = 2
	}

To save the enum `Gender` as `INT`, the type level mapping must be called.

.. code-block:: c#
	:linenos:

	TypeMapper.Map(typeof(Gender), DbType.Int32);

.. code-block:: c#
	:linenos:

	[Map("[dbo].[Customer]")]
	public class Customer
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Gender Gender { get; set; }
	}

**Note**: Enum will only succeed if it is convertible to the target database type.

Unmapped Behaviors
------------------

Below are the list of unmapped behaviors.

**Database string values are not a part of the Enum**

1. The operation `Query` will **not find** the desired record from the database if the `Enum` is used as an expression.
2. The operation `Query` will **fail** if there are rows returned from the database; the value is not convertible.
3. The operation `Insert`, `Merge`, `Update` will **succeed** and will passed the `Enum` value instead; database record is not intact.

**Database numeric values are not a part of the Enum**

1. The operation `Query` will **not find** the desired record from the database if the `Enum` is used as an expression.
2. The operation `Query` will **succeed** if there are rows returned from the database; the value is not intact to `Enum`.
3. The operation `Insert`, `Merge`, `Update` will **succeed** will always be succeeded.

