Mapping with MapAttribute
=========================

A `MapAttribute` is used to define a mapping between the `Class` and its `Properties` into its equivalent database `Object` and `Fields`. Located at `RepoDb.Attributes` namespace.

Mapping a Class
----------------

.. highlight:: c#

By default, the `Name` of the `Class` is used as a default mapped `Object` from the database. However, if the database `Object Name` is different from the `Class Name`, then the `MapAttribute` is use to map it properly.

Below is a sample code that maps a class named `Customer` into a table named `[dbo].[Customer]` from the database.

::

	[Map("[dbo].[Customer]")]
	public class Customer : DataEntity
	{
	}

Mapping a Property
------------------

.. highlight:: c#

By default, the `Name` of the `Property` is used as a default mapped `Field` from the database `Table`, `View` or any `ResultSet`.

Below is a sample code that maps the property named `Id` into an `CustomerId` field of `[dbo].[Customer]` table.

::

	[Map("[dbo].[Customer]")]
	public class Customer : DataEntity
	{
		[Map("CustomerId")]
		public int Id { get; set; }
	}
