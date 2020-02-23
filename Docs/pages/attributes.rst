Attributes
==========

The library contains certain attributes to support the custom implementation as per scenario basis.

Map
---

Is used to map an equivalent object from the database.

.. code-block:: c#
	:linenos:

	[Map("[dbo].[Customer]")]
	public class Customer
	{
	}

It is also used to map an equivalent column from the database.

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Map("CustomerId")]
		public int Id { get; set; }
	}

Primary
-------

Is used to define a primary key property in the class.

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Primary]
		public int Id { get; set; }
	}

The following primary property identification processed will be used in any case.

1. If the `PrimaryAttribute` is not defined, it then checks for database table primary key.
2. If the database table primary key is not present, it then checks for `Id` property.
3. If the `Id` property is not present, it then checks for the `Class.Name` + `Id` property.
4. If all properties defined above are not present, it then checks for the `Mapped.Name` + `Id` property.

Identity
--------

Is used to define an identity key property in the class.

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Identity]
		public int Id { get; set; }
	}

PropertyHandler
---------------

Is used to define a a handler for the property transformation.

.. code-block:: c#
	:linenos:

	public class CustomerExtraInfo
	{
		public string CompleteAddress { get; set; }
		public string Description { get; set; }
		public string Notes { get; set; }
		public string Preferrences { get; set; }
	}

	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[PropertyHandler(typeof(CustomerExtraInfoHandler)]
		public CustomerExtraInfo ExtraInfo { get; set; }
	}

In which the type `CustomerExtraInfoHandler` is a customized property handler that is used to handle the transformation of `ExtraInfo` property.

**Note**: Any of the attribute mentioned above is only being used to support the special scenarios and requirements defined by the businesses. **They are really not necessary!** The library is intelligent enough to identify the characteristics of your columns (`Primary`, `Identity`) by touching the database once and caching everything in the memory.

TypeMap
-------

Is used to define a property-level mapping of database type.

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Primary]
		public int Id { get; set; }

		[TypeMap(DbType.Binary)]
		public byte[] Image { get; set; }
	}

NpgsqlTypeMap
-------------

Is used to define a property-level mapping of database type for PostgreSql.

**Note:** `Only at package RepoDb.PostgreSql.`

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Primary]
		public int Id { get; set; }

		[NpgsqlTypeMap(NpgsqlDbType.Array)]
		public byte[] Image { get; set; }
	}

MySqlTypeMap
------------

Is used to define a property-level mapping of database type for MySql.

**Note:** `Only at package RepoDb.MySql.`

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Primary]
		public int Id { get; set; }

		[MySqlTypeMap(MySqlDbType.Blob)]
		public byte[] Image { get; set; }
	}

MicrosoftSqlServerTypeMap
-------------------------

Is used to define a property-level mapping of database type for Microsoft.Data.

**Note:** `Only at package RepoDb.SqlServer.`

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Primary]
		public int Id { get; set; }

		[MicrosoftSqlServerTypeMap(SqlDbType.Binary)]
		public byte[] Image { get; set; }
	}

SystemSqlServerTypeMap
----------------------

Is used to define a property-level mapping of database type for System.Data.

**Note:** `Only at package RepoDb.SqlServer.`

.. code-block:: c#
	:linenos:

	public class Customer
	{
		[Primary]
		public int Id { get; set; }

		[SystemSqlServerTypeMap(SqlDbType.Binary)]
		public byte[] Image { get; set; }
	}
