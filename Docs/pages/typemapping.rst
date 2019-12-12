Type Mapping
============

.. highlight: c#

Is being used to map the `.NET CLR Types` into its equivalent `System.Data.DbType` database types.

TypeMapper
----------

A static class used to map the .NET CLR Types into database types.

.. code-block:: c#
	:linenos:

	TypeMapper.Map(...);

Map
---

Is used to add a mapping between .NET CLR Type and database type.

Code below shows how to map the `System.DateTime` type to a `System.Data.DbType.DateTime2` database type.

.. code-block:: c#
	:linenos:

	TypeMapper.Map(typeof(DateTime), DbType.DateTime2);

and a `System.Decimal` type into `System.Data.DbType.Double` database type.

.. code-block:: c#
	:linenos:
	
	TypeMapper.Map(typeof(Decimal), DbType.Double);

Unmap
-----

Is used remove a mapping of targetted .NET CLR Type from the collection.

.. code-block:: c#
	:linenos:

	TypeMapper.Unmap(typeof(DateTime));

ConversionType
--------------

A property that is used to set the conversion type when converting the instance of `DbDataReader` object into its destination .NET CLR types.

.. code-block:: c#
	:linenos:

	TypeMapper.ConversionType = ConversionType.Automatic;

The default value is `RepoDb.Enumerations.ConversionType.Default`, which means that the conversion is strict and there is no additional implied logic during the conversion.

Given with the class named `Customer` as defined below.

.. code-block:: c#
	:linenos:

	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
	}

This will `succeed` if the table is defined like below.

.. code-block:: c#
	:linenos:

	CREATE TABLE [dbo].[Customer]
	{
		[Id] INT IDENTITY(1, 1) NOT NULL,
		[Name] NVARCHAR(32) NOT NULL,
		[Age] INT NOT NULL
	}

This will `fail` if the table is defined like below.

.. code-block:: c#
	:linenos:

	CREATE TABLE [dbo].[Customer]
	{
		[Id] INT IDENTITY(1, 1) NOT NULL,
		[Name] NVARCHAR(32) NOT NULL,
		[Age] NVARCHAR(8) NOT NULL -- There is no explicit converter between STRING and INT
	}

Setting the value to `RepoDb.Enumerations.ConversionType.Automatic` will address the mapping issue defined above, as long as the value of the `[Age]` column is convertible to an int.

**Note**: Using the `Automatic` conversion would affect the performance of the library.