Repository
==========

The library contains two repository objects, the `RepoDb.BaseRepository<TEntity, TDbConnection>` and the `RepoDb.DbRepository<TDbConnection>`.

DbRepository
------------

A base object for all shared-based repositories.

.. highlight:: c#

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

Since the repository is shared, the operations within this repository is also shared.

::

	// Getting a customer record where Id = 10045
	var customer = repository.Query<Customer>(c => c.Id == 10045);

	// Getting all orders of customer 10045
	var order = repository.Query<Order>(o => o.CustomerId == 10045);

BaseRepository
--------------

An abstract class for all entity-based repositories.

.. highlight:: c#

See sample code below on how to directly create a `DbRepository` object.

::

	public class CustomerRepository : BaseRepository<Customer, SqlConnection>
		base(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
	{
	}

Then, call it somewhere.

::

	var repository = new CustomerRepository();

Since the repository is only for single entity, then it can only be used the target entity.

::

	// Getting all customers
	var customers = repository.Query();

	// Getting a customer record where Id = 10045
	var customer = repository.Query(c => c.Id == 10045);

Async
-----

The call to `Async` methods of `BaseRepository` and `DbRepository` objects are a bit different when compared to calling the `Async` method of `DbConnection` object.

::

	// Call the async method
	var result = repository.QueryAsync();

	// Do your other logics here

	// Get the result
	var customers = result.Result.Extract();

The result is an instance of `ActionResultExtractor<IEnumerable<T>>` The `Extract` method is used to extract the result of the operation.

**Note**: It is very important to call the `Extract` method after calling the repository `Async` methods. This is to ensure dispose the underlying `DbConnection` object used during the actual operation.

Operations
----------

All repository operations has abstracted the connection operations (extended methods).

Please refer to `Connection <https://repodb.readthedocs.io/en/latest/pages/connection.html>`_ object documentation.