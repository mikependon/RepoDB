Mapping with MapAttribute
=========================

A `MapAttribute` is used to define a mapping of the current `Class` or `Property` equivalent to an `Object` or `Field` name in the database. Located at `RepoDb.Attributes` namespace.

The `Map` attribute second parameter is a command type `of type System.Data.CommandType`. If this parameter is defined, the repository operation execution will then be of that command type. This parameter is only working at the class level implementation.

See Microsoft documentation for `System.Data.CommandType`_ here.

.. _System.Data.CommandType: https://msdn.microsoft.com/en-us/library/system.data.commandtype%28v=vs.110%29.aspx

Mapping a Class
----------------

.. highlight:: c#

By default, the name of the class is used as a default mapped object from the database. However, if the database object name is different from the class name, the `MapAttribute` is then use to map it properly.

Below is a sample code that maps the class named `EmployeeDto` into `Employee` table from the database.

::

	[Map("[dbo].[Employee]", CommandType.Text)]
	public class EmployeeDto : DataEntity
	{
		public int Id { get; set; }
	}

Mapping a Property
------------------

.. highlight:: c#

By default, the name of the property is used as a default mapped field from the database object (table, view or any result set). However, if the database field name is different from the property name, the `MapAttribute` is then use to map it properly.

Below is a sample code that maps the property named `Id` into a `EmployeeId` field `Employee` table from the database.

::

	[Map("[dbo].[Employee]", CommandType.Text)]
	public class EmployeeDto : DataEntity
	{
		[Map("EmployeeId")]
		public int Id { get; set; }
	}
