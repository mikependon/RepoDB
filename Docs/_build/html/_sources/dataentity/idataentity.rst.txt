IDataEntity Interface
---------------------

.. highlight:: c#

An interface used to implement to mark a class to be the base class of all `Data Entity` object. Located at `RepoDb.Interfaces` namespace.

See sample codes below:

::

	public interface IClassName : IDataEntity
	{
		int Id { get; set; }
		...
	}