Persistency
===========

A connection persistency is used ensure that only single connection object is used all throughout the lifetime of the repository object.

.. code-block:: c#
	:linenos:

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", ConnectionPersistency.Instance))
	{
		// Call the operations here
	}

PerCall
-------

A new instance of a connection is being instantiated and disposed on every call of the repository operation. This is the default persistency value of the repository.

.. code-block:: c#
	:linenos:

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		
		// Query the customers
		var customers = repository.Query<Customer>(top: 10, orderBy: OrderField.Parse(new { CreatedDate = Order.Descending }));
		
		// Iterate the Customers and query their respective Orders
		customers.ToList().ForEach(customer =>
		{

			// Query the Customer Orders
			var orders = repository.Query<Order>(o => o.CustomerId == customer.Id);

			// Iterates the Orders
			orders.ToList().ForEach(order =>
			{
				
				// Update the updated date field
				order.LastUpdatedUtc = DateTime.UtcNow;

				// Update the order
				repository.Update<Order>(order);

			});

		});

	}

The codes above creates multiple connection from the data database during every call of the repository operations.

The new connection object has been created at this point.

- When querying the recent 10 `Customer` records from the database.
- When querying every `Order` of the `Customer`.
- When doing an inline-update operation on the `UpdatedDate` field of each `Order` record.

If every `Customer` object has 2 `Order` records each, then the number of connection created is of total 31 new database connection.

**Note**: It is not necesarry to call the `Dispose` method of the repository if the value of the `ConnectionPersistency` is `PerCall` as the operation is disposing each connection object after use.

Instance
--------

A single connection object is being used until the lifetime of the repository. Repository lifetime is of until the `Dispose` method has been called.

.. code-block:: c#
	:linenos:

	using (var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", ConnectionPersistency.Instance))
	{
		
		// Query the customers
		var customers = repository.Query<Customer>(top: 10, orderBy: OrderField.Parse(new { CreatedDate = Order.Descending }));
		
		// Iterate the Customers and query their respective Orders
		customers.ToList().ForEach(customer =>
		{

			// Query the Customer Orders
			var orders = repository.Query<Order>(o => o.CustomerId == customer.Id);

			// Iterates the Orders
			orders.ToList().ForEach(order =>
			{
				
				// Update the updated date field
				order.LastUpdatedUtc = DateTime.UtcNow;

				// Update the order
				repository.Update<Order>(order);

			});

		});

	}

In the code above, notice that the value of `ConnectionPersistency.Instance` was passed. This signals the repository to only used single connection object until its lifetime ends.

In all the repository operation calls above, only single connection is being used.

The first database connection is not created immediately in the constructor. It is being created when the first repository operation method was called. In the code above, the database connection was only created at the time of querying the list of the `Customer`.

The succeeding operation calls only reused the connection object created on the first call.

**Note**: The `Repository.Dispose()` method must be called after using the repository to avoid an orphaned open database connection.
