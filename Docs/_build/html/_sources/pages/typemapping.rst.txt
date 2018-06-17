Type Mapping
============

.. highlight: c#

Type mapping is feature that allows the library to identify which type of .NET is equivalent to the `System.Data.DbType` types. This feature is important to force the library the conversion it will going to specially when running the repository operations.

Below is the way on how to map the `System.DateTime` to be equivalent as `System.Data.DbType.DateTime2`.

::

	TypeMapper.Map(typeof(DateTime), DbType.DateTime2);

and `System.Decimal` into `System.Data.DbType.Double`.

::
	
	TypeMapper.AddMap(new TypeMap(typeof(Decimal), DbType.Double));

**Note**: The class is callable anywhere in the application as it was implemented in a static way.