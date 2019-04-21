Attributes
==========

The library contains certain attributes to support the custom implementation as per scenario basis.

Map
---

Is used to map an equivalent object from the database.

.. highlight:: c#

::

	[Map("[dbo].[Customer]")]
	public class Customer
	{
	}

It is also used to map an equivalent column from the database.

.. highlight:: c#

::

	public class Customer
	{
		[Map("CustomerId")]
		public int Id { get; set; }
	}

Primary
-------

Is used to define a primary key property in the class.

.. highlight:: c#

::

	public class Customer
	{
		[Primary]
		public int Id { get; set; }
	}

The following primary property identification processed will be used in any case.

1. If the `PrimaryAttribute` is not defined, it checks for `Id` property. If present, it will then become the default primary property.
2. If the `Id` property is not present, it checks for the `Class.Name` + `Id` property. In the case above, it should be `CustomerId`. If present, it will then become the default property.
3. If both properties are not present, it then checks for the `Mapped.Name` + `Id` property. In the case above, it should be `CustomerId`. If present, it will then become the default property.

Identity
--------

Is used to define an identity key property in the class.

.. highlight:: c#

::

	public class Customer
	{
		[Identity]
		public int Id { get; set; }
	}

TypeMap
-------

Is used to define a property-level mapping of database type.

.. highlight:: c#

::

	public class Customer
	{
		[Primary]
		public int Id { get; set; }

		[TypeMap(DbType.Binary)]
		public byte[] Image { get; set; }
	}

**Note**: Any of the attribute mentioned above is only being used to support the special scenarios and requirements defined by the businesses. **They are really not necessary!** The library is intelligent enough to identify the characteristics of your columns (`Primary`, `Identity`) by touching the database once and caching everything in the memory.
