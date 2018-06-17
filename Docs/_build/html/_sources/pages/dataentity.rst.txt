Creating a DataEntity
=====================

A `DataEntity` is a DTO class used to feed the operations of the repositories. It acts as a single row when it comes to database.

It is recommended that an explicit interface (of type `RepoDb.Interfaces.IDataEntity`) must be implemented in order to make a contracted `DataEntity`.

DataEntity Class
----------------

.. highlight:: c#

Acts as the base class of all `Data Entity` object. Located at `RepoDb` namespace.

::

	public class ClassName : DataEntity
	{
	}

IDataEntity Interface
---------------------

.. highlight:: c#

An interface used to implement to mark a class to be the base class of all `Data Entity` object. Located at `RepoDb.Interfaces` namespace.

::

	public interface IClassName : IDataEntity
	{
	}
