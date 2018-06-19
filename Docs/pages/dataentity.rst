Creating a DataEntity
=====================

A `DataEntity` is a DTO class used to feed the operations of the repositories. It is equivalent to a single row in a database.

DataEntity Class
----------------

.. highlight:: c#

Acts as the base class of all `Data Entity` object. Located at `RepoDb` namespace.

::

	public class ClassName : DataEntity
	{
	}