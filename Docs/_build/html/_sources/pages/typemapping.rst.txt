Type Mapping
============

.. highlight: c#

It is a feature used to map the `.NET CLR Types` into its equivalent `System.Data.DbType` database types.

Code below shows how to map the `System.DateTime` type to a `System.Data.DbType.DateTime2` database type.

::

	TypeMapper.Map(typeof(DateTime), DbType.DateTime2);

and a `System.Decimal` type into `System.Data.DbType.Double` database type.

::
	
	TypeMapper.AddMap(new TypeMap(typeof(Decimal), DbType.Double));

**Note**: The `TypeMapper` class is callable anywhere in the application as it was implemented as a static class.