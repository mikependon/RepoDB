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