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

Used to define a primary key from the class object.

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

If all of the conditions above were not met, then the `DataEntity` will have no primary property. It somehow fails if the repository operation of like `Delete` and `Update` has been called without explicitly specifying the expressions for the `WHERE` parameter.

Identity
--------

Used to define an identity key from the class object.

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

**Note**: The `RepoDb` is an attribute-less library. It will work without specifying the attributes we discussed earlier. Attributes are only being used to implement the propery way to handle the special scenarios defined by the business requirements.
