Creating a DataEntity
=====================

A `DataEntity` is a DTO class that is being used to feed the operations of the repository. It is equivalent to a single row in the database.

DataEntity Class
----------------

.. highlight:: c#

It acts as the base class of all `DataEntity` objects. Located at `RepoDb` namespace.

Below is a sample code for a `Customer` data entity object.

::

	public class Customer : DataEntity
	{
	}