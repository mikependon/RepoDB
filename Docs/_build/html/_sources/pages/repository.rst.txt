Working with Repository
=======================

The library contains two repository objects, the `RepoDb.BaseRepository<TEntity, TDbConnection>` and the `RepoDb.DbRepository<TDbConnection>`.

The latter is the heart of the library as it contains all the operations that is being used by all other repositories within or outside the library.

DbRepository Class
------------------

.. highlight:: c#

A base object for all shared-based repositories. This object is usually being inheritted if the derived class is meant for shared-based operations. This object is used by `RepoDb.BaseRepository<TEntity, TDbConnection>` as an underlying repository for all of its operations. Located at `RepoDb` namespace.

This means that, the `RepoDb.BaseRepository<TEntity, TDbConnection>` is only abstracting the operations of the `RepoDb.DbRepository<TDbConnection>` object in all areas.

Below are the constructor parameters:

- **connectionString**: the connection string to connect to.
- **commandTimeout (optional)**: the command timeout in seconds. It is being used to set the value of the `DbCommand.CommandTimeout` object prior to the execution of the operation.
- **cache (optional)**: the cache object to be used by the repository. By default, the repository is using the `RepoDb.MemoryCache` object.
- **trace (optional)**: the trace object to be used by the repository. The default is `null`.
- **statementBuilder (optional)**: the statement builder object to be used by the repository. By default, the repository is using the `RepoDb.SqlDbStatementBuilder` object.

This repository can be instantiated directly or indirectly. Indirectly means, it should be abstracted first before instantiation.

See sample code below on how to directly create a `DbRepository` object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");

Another way of creating a `DbRepository` is by abstracting it through derived classes. See sample code below.

::

	public class NorthwindDbRepository : DbRepository<SqlConnection>
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

Then, call it somewhere.

::

	var repository = new NorthwindRepository();

BaseRepository Class
--------------------

.. highlight:: c#

An abstract class for all entity-based repositories. This object is usually being inheritted if the derived class is meant for entity-based operations. Located at `RepoDb` namespace.

The operational scope of this repository is only limited to its defined target `DataEntity` object.

Below are the constructor parameters:

- **connectionString**: the connection string to connect to.
- **commandTimeout (optional)**: the command timeout in seconds. It is being used to set the value of the `DbCommand.CommandTimeout` object prior to the execution of the operation.
- **cache (optional)**: the cache object to be used by the repository. By default, the repository is using the `RepoDb.MemoryCache` object.
- **trace (optional)**: the trace object to be used by the repository. The default is `null`.
- **statementBuilder (optional)**: the statement builder object to be used by the repository. By default, the repository is using the `RepoDb.SqlDbStatementBuilder` object.

See sample code below on how to directly create a `DbRepository` object.

::

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

Then, call it somewhere.

::

	var repository = new CustomerRepository();
