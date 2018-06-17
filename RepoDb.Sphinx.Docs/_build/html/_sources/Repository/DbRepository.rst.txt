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

It is recommended to create a contracted interface for `DbRepository` in order for it to be dependency injectable.

See sample code below the way on how to create an interface and implement it directly to the derived class.

::

	public interface INorthwindDbRepository : IDbRepository<SqlConnection>
	{
	}

	public class NorthwindDbRepository : DbRepository<SqlConnection>, INorthwindDbRepository
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}