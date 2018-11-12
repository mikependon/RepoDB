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

Ignore
------

Is used to ignore certain properties from the actual database operation.

.. highlight:: c#

::

	public class Customer
	{
		[Primary, Ignore(Command.Insert | Command.Update)]
		public int Id { get; set; }
		
		public string Name { get; set; }

		[Ignore(Command.Query | Command.Update)]
		public DateTime CreatedDate { get; set; }
	}

When the operation `Repository.Insert` is called, then following SQL statement will be composed prior to the actual execution in the database.

::

	// Ignoring the `Id` field in Insert operation
	INSERT INTO [dbo].[Customer] ([Name], [CreatedDate]) VALUES (@Name, CreatedDate);

When the operation `Repository.Query` is called, then following SQL statement will be composed prior to the execution in the database.

::

	// Ignoring the `CreatedDate` field in Query operation
	SELECT [Id], [Name] FROM [dbo].[Customer] WHERE (.....); // WHERE part will vary on the expression passed during the calls

When the operation `Repository.Update` is called, then following SQL statement will be composed prior to the execution in the database.

::

	// Ignoring the `Id` and `CreatedDate` fields in Update operation
	UPDATE [dbo].[Customer] SET [Name] = @Name WHERE (.....); // WHERE part will vary on the expression passed during the calls

Below are the list of operational commands that can be defined in the `IgnoreAttribute`.

* None
* BatchQuery
* BulkInsert
* Count
* Delete
* DeleteAll
* InlineMerge
* InlineInsert
* InlineUpdate
* Insert
* Merge
* Query
* Update

All commands specified above can be defined together in a single `IgnoreAttribute` by using the pipe character (`|`) as the separator.

**Note**: The `RepoDb` is an attribute-less library. It will work without specifying the attributes we discussed earlier. Attributes are only being used to implement the propery way to handle the special scenarios defined by the business requirements.

Foreign
-------

An attribute used to define a foreign relationship for the recursive property of the data entity object.

.. highlight:: c#

::

	public class Order
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
		public IEnumerable<OrderItem> OrderItems { get; set; }
	}

	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[Foreign("CustomerId")] // Here, we are referring that the Order has a CustomerId property
		public IEnumerable<Order> Orders { get; set; }
	}
