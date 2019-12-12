Transaction
===========

The library has abstracted the `ADO.NET` transaction object.

.. code-block:: c#
	:linenos:

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

Usability
---------

Every connection operation accepts a transaction object as an argument. Once the transaction object is passed, the operation execution context will be a part of that transaction.

.. code-block:: c#
	:linenos:

	// Creates a connection object first (better to use 'using' keyword)
	var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen();

	// Creates a transaction by calling the 'BeginTransaction' method of the connection object
	var transaction = connection.BeginTransaction();

	// Always wrap with try-catch block
	try
	{
		// Call the first operation
		var customer = new Customer()
		{
			Name = "Anna Fullerton",
			CreatedDate = DateTime.UtcNow
		};
		var customerId = Convert.ToInt32(connection.Insert(customer, transaction: transaction));

		// Call the second operation
		var affectedRows = connection.ExecuteNonQuery("[dbo].[sp_allocate_customer_orderable_products_by_location]",
			new { CustomerId = customerId },
			commandType: CommandType.StoredProcedure,
			transaction: transaction);

		// Call the third operation
		if (affectedRows > 0)
		{
			connection.ExecuteQuery<OrderableProduct>("[dbo].[sp_get_customer_orderable_products",
				new { CustomerId = customerId },
				commandType: CommandType.StoredProcedure,
				transaction: transaction);
		}
		else
		{
			connection.Delete<Customer>(customerId);
		}

		// Make sure to commit transaction
		transaction.Commit();
	}
	catch
	{
		// Rollback on any exceptions
		transaction.Rollback();
	}
	finally
	{
		// Always dispose after used
		transaction.Dispose();

		// Dispose the connection as well
		connection.Dispose();
	}