Property Attributes
===================

.. highlight:: c#

A `PrimaryAttribute` is used to define the class property as primary property of the `DataEntity` object. Located at `RepoDb.Attributes` namespace.

::

	public class Employee : DataEntity
	{
		[Primary]
		public int Id { get; set; }
	}

The following primary property identification will be used in any case.

1. If the `PrimaryAttribute` is not defined, it checks for `Id` property. If present, it will then become the default primary property.
2. If the `Id` property is not present, it checks for the `Class.Name` + `Id` property. In the case above, it should be `EmployeeId`. If present, it will then become the default property.
3. If both properties are not present, it then checks for the `Mapped.Name` + `Id` property. In the case above, it should be `EmployeeTableId`. If present, it will then become the default property.

If all of the conditions are not met, then the `DataEntity` will have no primary property. It usually fails if call the certain repository operations that requires the `PrimaryKey` (i.e: `Update`, `Delete`).

Identity Property
-----------------

To define an identity property, simply sets the `isIdentity` parameter of the `PrimaryAttribute` during the implementation.

::

	public class Employee : DataEntity
	{
		[Primary(true)]
		public int Id { get; set; }
	}

Ignoring a Property
-------------------

.. highlight:: c#

An `IgnoreAttribute` is used to mark a class property to be ignoreable during the actual repository operation. Located at `RepoDb.Attributes` namespace.

Example: If of type command `Insert` and `Update` is defined on the `IgnoreAttribute` of the class property named `CreatedDate`, then the property will not be a part of the `Insert` and `Update` operation of the repository.

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