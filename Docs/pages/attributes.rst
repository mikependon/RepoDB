Property Attributes
===================

.. highlight:: c#

A `PrimaryAttribute` is used to define the class property to become the primary property of the `DataEntity` object. Located at `RepoDb.Attributes` namespace.

::

	[Map("[dbo].[Customer]")]
	public class Customer : DataEntity
	{
		[Primary]
		public int Id { get; set; }
	}

The following primary property identification processed will be used in any case.

1. If the `PrimaryAttribute` is not defined, it checks for `Id` property. If present, it will then become the default primary property.
2. If the `Id` property is not present, it checks for the `Class.Name` + `Id` property. In the case above, it should be `CustomerId`. If present, it will then become the default property.
3. If both properties are not present, it then checks for the `Mapped.Name` + `Id` property. In the case above, it should be `CustomerId`. If present, it will then become the default property.

If all of the conditions above were not met, then the `DataEntity` will have no primary property. It somehow fails if the repository operation of like `Delete` and `Update` has been called without explicitly specifying the expressions for the `WHERE` parameter.

Ignoring a Property
-------------------

.. highlight:: c#

An `IgnoreAttribute` is used to mark a class property to be ignoreable during the actual execution of the repository operation. Located at `RepoDb.Attributes` namespace.

Example: If the type command `Insert` and `Update` is defined on the `IgnoreAttribute` of the class property named `CreatedDate`, then the property `CreatedDate` will be excluded on the `Insert` and `Update` operation of the repository.

Below is a sample class that has certain columns with `Ignore` attributes defined.

::

	[Map("[dbo].[Customer]")]
	public class Customer : DataEntity
	{
		[Primary]
		[Ignore(Command.Insert | Command.Update)]
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

Below are the list of commands that can be defined in the `IgnoreAttribute`.

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