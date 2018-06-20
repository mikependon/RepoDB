Transaction
===========

.. highlight:: c#

The library has abstracted everything from `ADO.NET` including the `Transaction` object. This means, the `Transaction` object works completely the same as it was with `ADO.NET`.

Transactions can be created by calling the `BeginTransaction` method of the `DbConnection` object.

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	using (var connection = repository.CreateConnection().EnsureOpen())
	{
		var transaction = connection.BeginTransaction();
		try
		{
			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
		}
		finally
		{
			transaction.Dispose();
		}
	}

Every operation of the repository accepts a transaction object as an argument. Once the `Transaction` object is passed, then the repository operation execution context will be a part of that transaction.

See below on how to use a `Transaction` object with multiple operations.

::

	var connectionString = @"Server=.;Database=Northwind;Integrated Security=SSPI;";
	var customerRepository = new CustomerRepository<Customer, SqlConnection>(connectionString);
	var orderRepository = new OrderRepository<Order, SqlConnection>(connectionString);
	using (var connection = customerRepository.CreateConnection().EnsureOpen())
	{
		var transaction = connection.BeginTransaction();
		try
		{
			var customer = new Customer()
			{
				Name = "Anna Fullerton",
				CreatedDate = DateTime.UtcNow
			};
			var customerId = Convert.ToInt32(customerRepository.Insert(customer, transaction: transaction));
			var order = new Order()
			{
				CustomerId = customerId,
				ProductId = 12,
				Quantity = 2,
				CreatedDate = DateTime.UtcNow
			};
			var orderId = Convert.ToInt32(orderRepository.Insert(order, transaction: transaction));
			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
		}
		finally
		{
			transaction.Dispose();
		}
	}

The code snippets above will first insert a `Customer` record in the database and will return the newly added customer `Id`. It will be followed by inserting the `Order` record with the parent `CustomerId` as part of the entity relationship. Then, the transaction will be committed. However, if any exception occurs during the operation, the transaction will rollback all the operations above.

**Note**: Notice that the transaction object were created via `CustomerRepository` and has been used in both repository afterwards. The library will adapt the transaction process of `ADO.NET`. So whether the transaction object is created via an independent `DbConnection` object, as long as the connection is open, then the operation is valid.
