ExecuteQuery (Dynamic)
======================

This method is a dynamic method used to execute a SQL Statements or Stored Procedure from the database. It is a forward only operation.

TEntity
-------

An `ExecuteQuery` method can directly return an enumerable list of data entity object. No need to use the `ExecuteReader` method.

.. highlight:: c#

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.ExecuteQuery<Order>("SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;", new { CustomerId = 10045 });
	}

The class property accessibility is very dynamic through this method. Let us say the order table schema is below.

.. code-block:: sql
	:linenos:

	DROP TABLE IF EXISTS [dbo].[Order];
	GO
	CREATE TABLE [dbo].[Order]
	(
		Id INT
		, CustomerId INT
		, OrderDate DATETIME2(7)
		, Quantity INT
		, CreatedDate DATETIME2(7)
		, UpdatedDate DATETIME2(7)
	);
	GO
	
.. highlight:: c#

No need for the class to have the exact match of the properties (also applicable in `BatchQuery` and `Query` operation).

::

	[Map("[dbo].[Order]")]
	public class ComplexOrder
	{
		// Match properties
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
		
		// Unmatch properties
		public int ProductId { get; set; }
		public int OrderItemId { get; set; }
		public int Price { get; set; }
		public double Total { get; set; }

		// Note: The CreatedDate and UpdatedDate is not defined on this class
	}

Then call the records with the code below.
	
::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.ExecuteQuery<ComplexOrder>("SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;", new { CustomerId = 10045 });
	}

Or, if a complex stored procedure is present.

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.ExecuteQuery<ComplexOrder>("[dbo].[sp_name]", new { CustomerId = 10045 }, commandType: CommandType.StoredProcedure);
	}

Dynamics
--------

The `ExecuteQuery` method can also return a list of dynamic objects.

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Did not passed the <TEntity>
		var orders = connection.ExecuteQuery("SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;", new { CustomerId = 10045 });
		
		// Iterate the orders
		foreach (var order in orders)
		{
			// The 'order' is dynamic
		}
	}

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Did not passed the <TEntity>
		var orders = connection.ExecuteQuery("[dbo].[sp_name]", new { CustomerId = 10045 }, commandType: CommandType.StoredProcedure);

		// Iterate the orders
		foreach (var order in orders)
		{
			// The 'order' is dynamic
		}
	}

Note: Calling the `ExecuteQuery` via dynamic is a bit slower compared to CLR types.

	