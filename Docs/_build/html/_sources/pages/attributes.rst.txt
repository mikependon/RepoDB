Property Attributes
===================

.. highlight:: c#

A `PrimaryAttribute` is used to define the class property as primary property of the `DataEntity` object. Located at `RepoDb.Attributes` namespace.

::

	[Map("[dbo].[Employee]")]
	public class Employee : DataEntity
	{
		[Primary]
		public int Id { get; set; }
	}

The following primary property identification processed will be used in any case.

1. If the `PrimaryAttribute` is not defined, it checks for `Id` property. If present, it will then become the default primary property.
2. If the `Id` property is not present, it checks for the `Class.Name` + `Id` property. In the case above, it should be `EmployeeId`. If present, it will then become the default property.
3. If both properties are not present, it then checks for the `Mapped.Name` + `Id` property. In the case above, it should be `EmployeeId`. If present, it will then become the default property.

If all of the conditions are not met, then the `DataEntity` will have no primary property. It usually fails if the operation of like `Delete` and `Update` has been called without explicitly specifying the expressions for the `WHERE` parameter.

Identity Property
-----------------

By default, upon defining a `PrimaryAttribute`, the property is automatically an identity. To unset, simply sets the `isIdentity` parameter of the `PrimaryAttribute` to `false`.

::

	[Map("[dbo].[Employee]")]
	public class Employee : DataEntity
	{
		[Primary] // This is identity by default, can be set to [Primary(false)]
		public int Id { get; set; }
	}

Ignoring a Property
-------------------

.. highlight:: c#

An `IgnoreAttribute` is used to mark a class property to be ignoreable during the actual repository operation. Located at `RepoDb.Attributes` namespace.

Example: If of type command `Insert` and `Update` is defined on the `IgnoreAttribute` of the class property named `CreatedDate`, then the property will not be a part of the `Insert` and `Update` operation of the repository.

Below is a sample class that has certain columns with `Ignore` attributes defined.

::

	[Map("[dbo].[Employee]")]
	public class Employee : DataEntity
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
	INSERT INTO [dbo].[Employee] (Name, CreatedDate) VALUES (@Name, CreatedDate);

When the operation `Repository.Query` and `Repository.Update` is called, then following SQL statement will be composed prior to the execution in the database.

::

	// Ignoring the `CreatedDate` field in Query operation
	INSERT INTO [dbo].[Employee] (Name, CreatedDate) VALUES (@Name, CreatedDate);

or

::

	// Ignoring the `Id` and `CreatedDate` fields in Update operation
	UPDATE [dbo].[Employee] SET Name = @Name WHERE (.....); // WHERE part will vary on the expression passed during the calls

Below are the commands that can be defined using the `IgnoreAttribute`.

* None
* Query
* Insert
* Update
* Delete
* Merge
* BatchQuery
* InlineUpdate