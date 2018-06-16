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

It is recommended to create a contracted interface for `BaseRepository` in order for it to be dependency injectable.

See sample code below the way on how to create an interface and implement it directly to the derived class.

::

	public interface ICustomerRepository : IBaseRepository<Customer, SqlConnection>
	{
	}

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>, ICustomerRepository
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}
