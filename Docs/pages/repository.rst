Working with Repository
=======================

The library contains two repository objects, the `RepoDb.BaseRepository<TEntity, TDbConnection>` and the `RepoDb.DbRepository<TDbConnection>`.

DbRepository Class
------------------

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

BaseRepository Class
--------------------

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
	var customers = repository.Query<Customer>();

	// Getting a customer record where Id = 10045
	var customer = repository.Query<Customer>(c => c.Id == 10045);
