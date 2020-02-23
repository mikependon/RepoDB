Connection
==========

The library has abstracted everything from `ADO.NET` when it comes to the connection object.

**Note**: All the operations provided has an equivalent `Async` and/or `TableName` methods.

Average
-------

Averages the target field from the database table.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Average<Order>(e => e.Price,
			new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Average<Order>(e => e.Price,
			e => e.CustomerId == 10045 && e.OrderDate == DateTime.UtcNow.Date);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Average<Order>(e => e.Price, queryGroup);
	}

Records can all also be averaged via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Average("Order", new Field("Price"), new { CustomerId = 10045 });
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Average("Order", new Field("Price"), queryGroup);
	}

**Note**: By setting the `where` argument to blank would average all the records. Exactly the same as `AverageAll` operation.

AverageAll
----------

Averages the target field from all data of the database table.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.AverageAll<Order>(e => e.Price);
	}

All records can all also be averaged via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.AverageAll("Order", new Field("Price"));
	}

BatchQuery
----------

Queries a data from the database by batch.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(0,
			24,
			OrderField.Parse(new { Id = Order.Ascending }),
			new { CustomerId = 10045 });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(0,
			24,
			OrderField.Ascending<Order>(o => o.Id),
			o => o.CustomerId = 10045);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(0,
			24,
			new OrderField("Id", Order.Ascending).AsEnumerable(),
			new QueryField(nameof(Order.CustomerId), 10045));
	}

Targetted columns can also be batch-queried via table-name-based calls.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery("Order",
			0,
			24,
			new OrderField("Id", Order.Ascending).AsEnumerable(),
			new { CustomerId = 10045 },
			fields: Field.From("Id", "Name", "Address"));
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery("Order",
			0,
			24,
			new OrderField("Id", Order.Ascending).AsEnumerable(),
			new QueryField(nameof(Order.CustomerId), 10045),
			fields: Field.From("Id", "Name", "Address"));
	}

BulkDelete
----------

Bulk delete a list of data entity objects from the database.

**Note:** `Only at package RepoDb.SqlServer.BulkOperations.`

The result would be the number of rows affected by `BulkDelete` operation from the database.

Via DataEntities:

.. code-block:: c#
	:linenos:

	var orders = new List<Order>();

	orders.Add(new Order()
	{
		Id = 1,
		...
	},
	...);

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkDelete<Order>(orders);
		// or
		var result = connection.BulkDelete("Order", orders);
	}

Via DataReader:

.. code-block:: c#
	:linenos:

	using (var sourceConnection = new SqlConnection(@"Server=.;Database=Northwind_Old;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT Id FROM [dbo].[Order];"))
		{
			using (var destinationConnection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
			{
				var result = destinationConnection.BulkDelete<Order>(reader);
				// or
				var result = destinationConnection.BulkDelete("Order", reader);
			}
		}
	}

Via PrimaryKeys:

.. code-block:: c#
	:linenos:
	
	var primaryKeys = new[] { 10045, 10077, ..., 10089, 10092 };

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkDelete<Order>(primaryKeys);
		// or
		var result = connection.BulkDelete("Order", primaryKeys);
	}

Via DataTable:

.. code-block:: c#
	:linenos:

	var ordersTable = GetOrdersDataTable();

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkDelete<Order>(ordersTable);
		// or
		var result = connection.BulkDelete("Order", ordersTable);
	}

BulkInsert
----------

Bulk insert a list of data entity objects into the database.

**Note:** `Only at package RepoDb.SqlServer.BulkOperations.`

The result would be the number of rows inserted by the `BulkInsert` operation in the database.

Via DataEntities:

.. code-block:: c#
	:linenos:

	var orders = new List<Order>();

	orders.Add(new Order()
	{
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	},
	...);

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkInsert<Order>(orders);
		// or
		var result = connection.BulkInsert("Order", orders);
	}

Via DataReader:

.. code-block:: c#
	:linenos:

	using (var sourceConnection = new SqlConnection(@"Server=.;Database=Northwind_Old;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT Quantity, ProductId, GETUTCDATE() AS CreatedDate, GETUTCDATE() AS UpdatedDate FROM [dbo].[Order];"))
		{
			using (var destinationConnection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
			{
				var result = destinationConnection.BulkInsert<Order>(reader);
				// or
				var result = destinationConnection.BulkInsert("Order", reader);
			}
		}
	}

Via DataTable:

.. code-block:: c#
	:linenos:

	var ordersTable = GetOrdersDataTable();

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkInsert<Order>(ordersTable);
		// or
		var result = connection.BulkInsert("Order", ordersTable);
	}

BulkMerge
---------

Bulk merge a list of data entity objects into the database.

**Note:** `Only at package RepoDb.SqlServer.BulkOperations.`

The result would be the number of rows affected by the `BulkMerge` operation from the database.

Via DataEntities:

.. code-block:: c#
	:linenos:

	var orders = new List<Order>();

	orders.Add(new Order()
	{
		Id = 1,
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	},
	...);

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkMerge<Order>(orders);
		// or
		var result = connection.BulkMerge("Order", orders);
	}

Via DataReader:

.. code-block:: c#
	:linenos:

	using (var sourceConnection = new SqlConnection(@"Server=.;Database=Northwind_Old;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT Id, Quantity, ProductId, GETUTCDATE() AS CreatedDate, GETUTCDATE() AS UpdatedDate FROM [dbo].[Order];"))
		{
			using (var destinationConnection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
			{
				var result = destinationConnection.BulkMerge<Order>(reader);
				// or
				var result = destinationConnection.BulkMerge("Order", reader);
			}
		}
	}

Via DataTable:

.. code-block:: c#
	:linenos:

	var ordersTable = GetOrdersDataTable();

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkMerge<Order>(ordersTable);
		// or
		var result = connection.BulkMerge("Order", ordersTable);
	}

BulkUpdate
----------

Bulk update a list of data entity objects into the database.

**Note:** `Only at package RepoDb.SqlServer.BulkOperations.`

The result would be the number of rows affected by the `BulkUpdate` operation from the database.

Via DataEntities:

.. code-block:: c#
	:linenos:

	var orders = new List<Order>();

	orders.Add(new Order()
	{
		Id = 1,
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	},
	...);

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkUpdate<Order>(orders);
		// or
		var result = connection.BulkUpdate("Order", orders);
	}

Via DataReader:

.. code-block:: c#
	:linenos:

	using (var sourceConnection = new SqlConnection(@"Server=.;Database=Northwind_Old;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT Id, Quantity, ProductId, GETUTCDATE() AS CreatedDate, GETUTCDATE() AS UpdatedDate FROM [dbo].[Order];"))
		{
			using (var destinationConnection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
			{
				var result = destinationConnection.BulkUpdate<Order>(reader);
				// or
				var result = destinationConnection.BulkUpdate("Order", reader);
			}
		}
	}

Via DataTable:

.. code-block:: c#
	:linenos:

	var ordersTable = GetOrdersDataTable();

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.BulkUpdate<Order>(ordersTable);
		// or
		var result = connection.BulkUpdate("Order", ordersTable);
	}

**Impotant Notes:**

The arguments `qualifiers` and `usePhysicalPseudoTempTable` is given in the `BulkDelete`, `BulkMerge` and `BulkUpdate`.

The argument `qualifiers` is used to define the qualifier fields to be used during the operation. If it is not given, the `PrimaryKey` or `IdentityKey` columns will be used instead.

The argument `usePhysicalPseudoTempTable` is used to define whether an actual physical pseudo-table will be created during the actual operation, otherwise a temporary table will be created.

Count
-----

Counts the number of table data from the database.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Count<Order>(new { CustomerId = 10045 });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Count<Order>(o => o.CustomerId == 10045);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Count<Order>(new QueryField(nameof(Order.CustomerId), 10045));
	}

Records can all also be counted via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Count("Order", new { CustomerId = 10045 });
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Count("Order", new QueryField(nameof(Order.CustomerId), 10045));
	}

**Note**: By setting the `where` argument to blank would count all the records. Exactly the same as `CountAll` operation.

CountAll
--------

Counts all the table data from the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.CountAll<Order>();
	}

All records can all also be counted via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.CountAll("Order");
	}

CreateCommand
-------------

Creates a command object.

.. code-block:: c#
	:linenos:

	// Variables
	var customers = (IEnumerable<Customer>)null;

	// Open a connection
	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Create a command object
		var command = connection.CreateCommand("SELECT TOP 100 * FROM [dbo].[Customer];", CommandType.Text, 500, null);

		// Use the command object here
		...
	}

Delete
------

Deletes an existing data from the database.

Via DataEntity:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(10045);
		...
		var result = connection.Delete(customer);
	}

Via PrimaryKey:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Delete<Customer>(10045);
	}

**Note**: The library uses the `PrimaryKey` as the default qualifier for `Delete` operation. This also applies when deleting the `data entity` object itself. This call will throw an exception if the data entity does not have a primary key.

Via Dynamic:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Delete<Customer>(new { Id = 10045 });
	}
	
Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Delete<Customer>(c => c.Id == 10045);
	}
	
Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Delete<Customer>(new QueryField(nameof(Order.CustomerId), 10045));
	}

Records can also be deleted via table name.

Via Dynamic:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Delete("Customer", new { Id = 10045 });
	}
	
Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Delete("Customer", new QueryField(nameof(Order.CustomerId), 10045));
	}

**Note**: By setting the `where` argument to blank would delete all the records. Exactly the same as `DeleteAll` operation.

DeleteAll
---------

Deletes all the data from the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.DeleteAll<Customer>();
	}

All records can also be deleted via table name.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.DeleteAll("Customer");
	}
	
EnsureOpen
----------

Ensures the connection object is open.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Use the connection here
	}
	
ExecuteNonQuery
---------------

Executes a query from the database. It uses the underlying method `IDbCommand.ExecuteNonQuery` and returns the number of affected rows during the execution.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"UPDATE O
			SET O.Quantity = @Quantity
				, O.LastUpdatedUtc = @LastUpdatedUtc
			FROM [dbo].[Order] O
			WHERE (O.Id = @OrderId);";

		// Set the parameters
		var parameters = new
		{
			OrderId = 1002,
			Quantity = 5,
			LastUpdatedUtc = DateTime.UtcNow
		};

		// Execute the command text
		var result = connection.ExecuteNonQuery(commandText, parameters);
	}

Let us say the stored procedure below exists.

.. code-block:: sql
	:linenos:

	DROP PROCEDURE IF EXISTS [dbo].[sp_update_order_quantity];
	GO

	CREATE PROCEDURE [dbo].[sp_update_order_quantity]
	(
		@OrderId INT
		, @Quantity INT
	)
	AS
	BEGIN
		UPDATE O
		SET O.Quantity = @Quantity
			, O.LastUpdatedUtc = GETUTCDATE()
		FROM [dbo].[Order] O
		WHERE (O.Id = @OrderId);
	END

Below is the code on how to execute a stored procedure mentioned above:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the parameters
		var parameters = new
		{
			OrderId = 1002,
			Quantity = 5,
			LastUpdatedUtc = DateTime.UtcNow
		};

		// Call the procedure
		var result = connection.ExecuteNonQuery("[dbo].[sp_update_order_quantity]", parameters, commandType: CommandType.StoredProcedure);
	}

ExecuteQuery
------------

Executes a query from the database. It uses the underlying method `IDbCommand.ExecuteReader` and converts the result back to an enumerable list of dynamic objects.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"SELECT * FROM [dbo].[Customer] WHERE CustomerId = @CustomerId;";
		var result = connection.ExecuteQuery<Order>(commandText, new { CustomerId = 10045 });
	}

Let us say the stored procedure below exists.

.. code-block:: sql
	:linenos:

	DROP PROCEDURE IF EXISTS [dbo].[sp_get_customer];
	GO

	CREATE PROCEDURE [dbo].[sp_get_customer]
	(
		@CustomerId INT
	)
	AS
	BEGIN
		SELECT *
		FROM [dbo].[Customer] C
		WHERE (C.Id = @CustomerId);
	END

Below is the code on how to execute a stored procedure mentioned above:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.ExecuteNonQuery("[dbo].[sp_get_customer]",
			new { CustomerId = 10045 },
			commandType: CommandType.StoredProcedure);
	}

An `ExecuteQuery` method can directly return an enumerable list of data entity object. No need to use the `ExecuteReader` method.

.. code-block:: c#
	:linenos:

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
	
No need for the class to have the exact match of the properties (also applicable in all fetch operations like `BatchQuery`, `ExecuteQueryMultiple`, `Query` and `QueryMultiple` operations).

.. code-block:: c#
	:linenos:

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
	
.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.ExecuteQuery<ComplexOrder>("SELECT * FROM [dbo].[Order] WHERE CustomerId = @CustomerId;", new { CustomerId = 10045 });
	}

Or, if a complex stored procedure is present.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.ExecuteQuery<ComplexOrder>("[dbo].[sp_name]", new { CustomerId = 10045 }, commandType: CommandType.StoredProcedure);
	}

The `ExecuteQuery` method can also return a list of dynamic objects.

.. code-block:: c#
	:linenos:

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

.. code-block:: c#
	:linenos:

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

**Note**: Calling the `ExecuteQuery` via dynamic is a bit slower compared to a .NET CLR Type-based calls.

ExecuteQueryMultiple
--------------------

Executes a multiple query statement from the database and allows the user to extract the result to a target data entity.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"SELECT * FROM Customer WHERE Id = @CustomerId;
			SELECT * FROM [Order] WHERE CustomerId = @CustomerId;";

		using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 10045 }))
		{
			// Extract the first result
			var customers = result.Extract<Customer>();

			// Extract the second result
			var orders = result.Extract<Order>();
		}
	}

The method `Scalar` is used to extract the value of the first column of the first row of the `DbDataReader` object.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"SELECT * FROM Customer WHERE Id = @CustomerId;
			SELECT COUNT(*) FROM [Order] WHERE CustomerId = @CustomerId;";

		using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 10045 }))
		{
			// Extract the first result
			var customers = result.Extract<Customer>();

			// Extract the second result
			var ordersCount = (int)result.Scalar();
		}
	}

This method can also be used to combine the calls with Stored Procedure.

.. code-block:: sql
	:linenos:

	CREATE PROCEDURE [dbo].[sp_get_customer_orders]
	(
		@CustomerId INT
	)
	AS
	BEGIN
		SELECT *
		FROM [dbo].[Order]
		WHERE (CustomerId = @CustomerId);
	END

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var commandText = @"SELECT * FROM Customer WHERE Id = @CustomerId;
			EXEC [dbo].[sp_get_customer_orders] @CustomerId;";
		using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 10045 }))
		{
			// Extract the first result
			var customers = result.Extract<Customer>();

			// Extract the second result
			var orders = result.Extract<Order>();
		}
	}

ExecuteReader
-------------

Executes a query from the database. It uses the underlying method `IDbCommand.ExecuteReader` and returns the instance of the data reader.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer] WHERE CustomerId = @CustomerId;", new { CustomerId = 10045 }))
		{
			// Use the data reader here
		}
	}

Let us say the stored procedure below exists.

.. code-block:: sql
	:linenos:

	DROP PROCEDURE IF EXISTS [dbo].[sp_get_customer];
	GO

	CREATE PROCEDURE [dbo].[sp_get_customer]
	(
		@CustomerId INT
	)
	AS
	BEGIN
		SELECT *
		FROM [dbo].[Customer] C
		WHERE (C.Id = @CustomerId);
	END

Below is the code on how to execute a stored procedure mentioned above:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = connection.ExecuteReader("[dbo].[sp_get_customer]", new { CustomerId = 10045 }, commandType: CommandType.StoredProcedure))
		{
			// Use the data reader here
		}
	}

ExecuteScalar
-------------

Executes a query from the database. It uses the underlying method `IDbCommand.ExecuteScalar` and returns the first occurence value (first column of first row) of the execution.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var maxId = Convert.ToInt64(connection.ExecuteScalar("SELECT MAX([Id]) AS MaxId FROM [dbo].[Customer];"));
	}
	
Let us say the stored procedure below exists.

.. code-block:: sql
	:linenos:

	DROP PROCEDURE IF EXISTS [dbo].[sp_get_latest_customer_id];
	GO

	CREATE PROCEDURE [dbo].[sp_get_latest_customer_id]
	AS
	BEGIN
		SELECT MAX(Id) FROM [dbo].[Customer];
	END

Below is the code on how to execute a stored procedure mentioned above:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var maxId = Convert.ToInt64(connection.ExecuteScalar("[dbo].[sp_get_latest_customer_id]", commandType: CommandType.StoredProcedure)));
	}

A dynamic typed-based call is also provided, see below.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var maxId = connection.ExecuteScalar<long>("[dbo].[sp_get_latest_customer_id]", commandType: CommandType.StoredProcedure));
	}

Exists
------

Check whether the records are existing in the table.

Via DataEntity:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(10045);
		...
		var result = connection.Exists(customer);
	}

Via PrimaryKey:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Exists<Customer>(10045);
	}

Via Dynamic:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Exists<Customer>(new { Id = 10045 });
	}
	
Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Exists<Customer>(c => c.Id == 10045);
	}
	
Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Exists<Customer>(new QueryField(nameof(Order.CustomerId), 10045));
	}

Records can also be checked via table name.

Via Dynamic:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Exists("Customer", new { Id = 10045 });
	}
	
Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Exists("Customer", new QueryField(nameof(Order.CustomerId), 10045));
	}

GetDbSetting
------------

Gets the associated `IDbSetting` object that is currently mapped for the target `IDbConnection` object.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var setting = connection.GetDbSetting();
	}

GetDbHelper
-----------

Gets the associated `IDbHelper` object that is currently mapped for the target `IDbConnection` object.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var helper = connection.GetDbHelper();
	}

GetStatementBuilder
-------------------

Gets the associated `IStatementBuilder` object that is currently mapped for the target `IDbConnection` object.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var builder = connection.GetStatementBuilder();
	}

Insert
------

Inserts a new data in the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var order = new Order()
		{
			CustomerId = 10045,
			ProductId = 12
			Quantity = 2,
			CreatedDate = DateTime.UtcNow
		};
		var id = Convert.ToInt64(connection.Insert(order));
	}

The return value would be the newly generated `Identity` value, otherwise the value of `PrimaryKey`. If both are not present, then it will return `null`.

**Note**: The `identity` column will automatically be filled with newly generated `identity` value right after the insert.

A dynamic typed-based call is also provided when calling this method, see below.

.. code-block:: c#
	:linenos:

	// The first type is the entity type, the second type is the result type
	var id = connection.Insert<Order, long>(order);

**Note**: The first generic type is the type of `data entity` object, the second generic type is the type of the `result`.

Certain columns can also be inserted via table name calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var entity = new
		{
			CustomerId = 10045,
			ProductId = 12
			Quantity = 2,
			CreatedDate = DateTime.UtcNow
		};
		var id = connection.Insert<long>("Order", entity);
	}

**Note**: Use the table-name-based calls if the scenario is to only insert targetted columns.

InsertAll
---------

Inserts multiple data in the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = new List<Order>();
		for (var i = 0; i < 100; i++)
		{
			orders.Add(new Order()
			{
				CustomerId = 10045,
				ProductId = 12
				Quantity = 2,
				Price = 35.50
				CreatedDate = DateTime.UtcNow,
				LastUpdatedUtc = DateTime.UtcNow
			});
		}
		connection.InsertAll(orders);
	}

**Note**: All data entities `identity` fields will automatically be filled with its newly generated identity values.

Certain columns can also be inserted via table-name-based calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = new List<dynamic>();
		for (var i = 0; i < 100; i++)
		{
			orders.Add(new
			{
				CustomerId = 10045,
				ProductId = 12
				Quantity = 2,
				CreatedDate = DateTime.UtcNow
			});
		}
		var id = connection.InsertAll<long>("Order",
			entities: orders,
			fields: Field.From("CustomerId", "ProductId", "Quantity", "CreatedDate"));
	}

**Why passing the fields arguments?**

By default, the library will use the database columns of the `Order` entity. If the values of some fields were not specified, then the `InsertAll` operation will set it to `null` in the database.

**Note**: Use the table-name-based calls if the scenario is to only insert targetted columns.

Max
---

Maximizes the target field from the database table.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Max<Order>(e => e.Price,
			new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Max<Order>(e => e.Price,
			e => e.CustomerId == 10045 && e.OrderDate == DateTime.UtcNow.Date);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Max<Order>(e => e.Price, queryGroup);
	}

Records can all also be maximized via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Max("Order", new Field("Price"), new { CustomerId = 10045 });
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Max("Order", new Field("Price"), queryGroup);
	}

**Note**: By setting the `where` argument to blank would count all the records. Exactly the same as `MaxAll` operation.

MaxAll
------

Maximizes the target field from all data of the database table.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.MaxAll<Order>(e => e.Price);
	}

All records can all also be maximized via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.MaxAll("Order", new Field("Price"));
	}

Merge
-----

Merges a data entity or dynamic object into the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var order = connection.Query<Order>(1);
		order.Quantity = 5;
		UpdatedDate = DateTime.UtcNow;
		connection.Merge(order, Field.Parse<Order>(o => o.Id));
	}

By default, the `Merge` operation is using the data entity `PrimaryKey` as the qualifier if the second parameter is omitted.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var order = connection.Query<Order>(1);
		order.Quantity = 5;
		UpdatedDate = DateTime.UtcNow;
		connection.Merge(order);
	}

The qualifiers can also be set with the combination of multiple fields. When using this, please note that the qualifiers are also corresponding with the table index for performance purposes.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var order = connection.Query<Order>(1);
		order.Quantity = 5;
		UpdatedDate = DateTime.UtcNow;
		connection.Merge(order, Field.From("CustomerId", "ProductId"));
	}

**Note**: The data entity `identity` field will automatically be filled with newly generated identity value if the `Merge` operation has inserted a new record in the database.

Certain columns can also be merged via table name calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			Id = 1,
			Quantity = 5,
			UpdatedDate = DateTime.UtcNow
		};
		connection.Merge("Order", entity);
	}

**Note**: Use the table-name-based calls if the scenario is to only merge targetted columns.

MergeAll
--------

Merges the multiple data entity or dynamic objects into the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			order.Quantity = 5;
			order.LastUpdatedUtc = DateTime.UtcNow;
		}
		connection.MergeAll(orders, Field.From("Id"));
	}

Same as `Merge` operation, the `MergeAll` operation is also using the `PrimaryKey` as the default qualifier if the argument is not provided during the calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			order.Quantity = 5;
			order.LastUpdatedUtc = DateTime.UtcNow;
		}
		connection.MergeAll(orders);
	}

Also, multiple columns can be used as the qualifiers for `MergeAll` operation.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			order.Quantity = 5;
			order.LastUpdatedUtc = DateTime.UtcNow;
		}
		connection.MergeAll(orders, Field.From("CustomerId", "ProductId"));
	}

**Note**: The data entities `identity` fields will automatically be filled with its newly generated identity values.

All fields are being merged when calling the typed-based method. However, certain columns can be merged when using the table name calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		var mergeables = new List<dynamic>();
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			mergeables.Add(new
			{
				Id = order.Id,
				Quantity = 5,
				LastUpdatedUtc = DateTime.UtcNow
			});
		}
		connection.MergeAll("Order",
			entities: mergeables,
			fields: Field.From("Id", "Quantity", "LastUpdatedUtc"));
	}

**Why passing the fields arguments?**

Be aware on this behavior since this is a merge operation. By default, the library will use the database columns of the `Order` entity. If the values to that fields were not specified, then the `MergeAll` operation will set it to `null` in the database. By setting the `fields` argument, it will only merge the listed `fields` in the batch operations.

**Note**: Use the table-name-based calls if the scenario is to only merge targetted columns.

Min
---

Minimizes the target field from the database table.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Min<Order>(e => e.Price,
			new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Min<Order>(e => e.Price,
			e => e.CustomerId == 10045 && e.OrderDate == DateTime.UtcNow.Date);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Min<Order>(e => e.Price, queryGroup);
	}

Records can all also be minimized via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Min("Order", new Field("Price"), new { CustomerId = 10045 });
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Min("Order", new Field("Price"), queryGroup);
	}

**Note**: By setting the `where` argument to blank would count all the records. Exactly the same as `MinAll` operation.

MinAll
------

Minimizes the target field from all data of the database table.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.MinAll<Order>(e => e.Price);
	}

All records can all also be minimized via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.MinAll("Order", new Field("Price"));
	}

Query
-----

Queries a data from the database.

Via PrimaryKey:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(10045).FirstOrDefault();
	}
	
Via Dynamic:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(new { Id = 10045 }).FirstOrDefault();
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(c => c.Id == 10045);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 10045));
	}
	
With ordering.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orderBy = new
		{
			Id = Order.Ascending
		};
		var orders = connection.Query<Order>(new { CustomerId = 10045 }, orderBy: orderBy);
	}

With hint.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(new { CustomerId = 10045 }, hints: SqlTableHints.NoLock);
	}

Certain columns can also be queried via table-name-based calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orderBy = new
		{
			Id = Order.Ascending
		};
		var orders = connection.Query("Order",
			new { CustomerId = 10045 },
			fields: Field.From("Id", "CustomerId", "CreatedDateUtc"),
			orderBy: orderBy);
	}

**Note**: By setting the `where` argument to blank would query all the records. Exactly the same as `QueryAll` operation.

QueryAll
--------

Query all the data from the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.QueryAll<Customer>();
	}
	
With ordering.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orderBy = new
		{
			Id = Order.Ascending
		};
		var customers = connection.QueryAll<Customer>(orderBy);
	}

With hint.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.QueryAll<Customer>(SqlTableHints.NoLock);
	}

Certain columns can also be queried via table-name-based calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orderBy = new
		{
			Id = Order.Ascending
		};
		var orders = connection.QueryAll("Order",
			fields: Field.From("Id", "CustomerId", "CreatedDateUtc"),
			orderBy: orderBy);
	}

QueryMultiple
-------------

Query a multiple resultsets from the database.

Below is an example of how to query a customer where the `Id` field is equals to `10045`, and at the same time, querying all the orders connected to this customer since yesterday.
The result is an instance of a `Tuple` object.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// The parent Id
		var customerId = 10045;

		// Get the parent customer, and the child objects
		var result = connection.QueryMultiple<Customer, Order>(
			customer => customer.Id == customerId,
			order => order.CustomerId == customerId);

		// Read the customer
		var customer = result.Item1.FirstOrDefault();

		// Read the orders
		var orders = result.Item2.ToList();
		orders.ForEach(order =>
		{
			// Do the stuffs for the 'order' here
		});
	}

This method has supported until the last tupled dynamic type of the `Tuple` class. The current maximum tupled dynamic type is 7.

.. code-block:: c#
	:linenos:

	DbConnection.Query<T1, T2, T3, T4, T5, T6, T7>(
		where1: <Expression for T1>,
		where2: <Expression for T2>,
		where3: <Expression for T3>,
		where4: <Expression for T4>,
		where5: <Expression for T5>,
		where6: <Expression for T6>,
		where7: <Expression for T7>;

Notice above, there were `where<T<Num>>` arguments. These arguments are targetting the specific index of the type on the 'QueryMultiple' operation. This method is not meant for joining the result of each type, but instead, it is used to execute the query execution at once.

Below is an example of how to query the list of customers based on different US states.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.QueryMultiple<Customer, Customer, Customer, Customer, Customer, Customer, Customer>(
			where1: c => c.State == "California",
			where2: c => c.State == "Florida",
			where3: c => c.State == "Texas",
			where4: c => c.State == "Washington",
			where5: c => c.State == "Michigan",
			where6: c => c.State == "Arizona",
			where7: c => c.State == "New York");

		// Read the customers through its equivalent 'Item<N>' property
		var californiaCustomers = result.Item1;
		var floridaCustomers = result.Item2;
		var texasCustomers = result.Item3;
		var washingtonCustomers = result.Item4;
		var michiganCustomers = result.Item5;
		var arizonaCustomers = result.Item6;
		var newYorkCustomers = result.Item7;
	}

Notice as well, there are other arguments defined like `orderBy<N>`, `top<N>` and `hints<N>`. These are the targetted arguments if the caller wants to define the behavior of the query for that target type based on the element-index provided.

Below is the implementation of the the 2 target types tupled.

.. code-block:: c#
	:linenos:

	DbConnection.Query<T1, T2>(
		where1: <Expression for T1>,
		where2: <Expression for T2>,
		orderBy1: <Optional OrderExpression for T1>,
		top1: <Optional RowFilter for T1>,
		hints1: <Optional QueryOptimizer for T1>,
		orderBy2: <Optional OrderExpression for T2>,
		top2: <Optional RowFilter for T2>,
		hints2: <Optional QueryOptimizer for T2>);

Below is a example of how to do a query that returns a 100 customers from `California` ordered by their `SSID` optimized by `NOLOCK` keyword, and also, a list of 1000 customers from `Florida` with `READPAST` query optimizer ordered by their `LastName` followed by `FirstName`.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.QueryMultiple<Customer, Customer>(
			where1: c => c.State == "California",
			orderBy: OrderField.Parse(new { SsId = Order.Ascending }), /* At RepoDb.Enumerations */
			top1: 100,
			hints1: SqlTableHints.NoLock, /* Can write WITH (NOLOCK) */,
			where2: c => c.State == "Florida",
			orderBy2: OrderField.Parse(new { LastName = Order.Ascending, FirstName Order.Ascending }), /* At RepoDb.Enumerations */
			top2: 1000,
			hints2: "WITH (READPAST) /* Can use SqlTableHints.ReadPast */
		);

		// Read the customers through its equivalent 'Item<N>' property
		var californiaCustomers = result.Item1;
		var floridaCustomers = result.Item2;
	}

**Note**: This method does not support the `Object-Based` query tree expression.

Sum
---

Summarizes the target field from the database table.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Sum<Order>(e => e.Price,
			new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Sum<Order>(e => e.Price,
			e => e.CustomerId == 10045 && e.OrderDate == DateTime.UtcNow.Date);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Sum<Order>(e => e.Price, queryGroup);
	}

Records can all also be summarized via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Sum("Order", new Field("Price"), new { CustomerId = 10045 });
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup
		(
			new QueryField(nameof(Order.CustomerId), 10045),
			new QueryField(nameof(Order.OrderDate), DateTime.UtcNow.Date)
		);
		var result = connection.Sum("Order", new Field("Price"), queryGroup);
	}

**Note**: By setting the `where` argument to blank would count all the records. Exactly the same as `SumAll` operation.

SumAll
------

Summarizes the target field from all data of the database table.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.SumAll<Order>(e => e.Price);
	}

All records can all also be summarized via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.SumAll("Order", new Field("Price"));
	}

Truncate
--------

Truncates a table from the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Truncate<Customer>();
	}

Table can also be truncated via table name.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Truncate("Customer");
	}

Update
------

Updates an existing data in the database.

Let us say an `Order` object was queried from the database.

.. code-block:: c#
	:linenos:

	// Query a data from the database
	var order = connection.Query<Order>(1002).FirstOrDefault();

	// Set the target properties
	order.Quantity = 5;
	order.UpdateDate = DateTime.UtcNow;

Via DataEntity:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Update<Order>(order);
	}

Via PrimaryKey:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Update<Order>(order, 1002);
	}

**Note**: The library uses the `PrimaryKey` as the default qualifier for `Update` operation. This also applies when updating the `data entity` object itself. This call will throw an exception if the data entity does not have a `primary key`.

Via Dynamic:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Update<Order>(order, new { Id = 1002 });
	}

Expression way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Update<Order>(order, o => o.Id == 1002);
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.Update(order, new QueryField(nameof(Order.Id), 1002));
	}

Record can also be updated via table name.

Dynamic way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			Quantity = 5,
			UpdateDate = DateTime.UtcNow
		};
		var result = connection.Update("Order", entity, new { Id = 1002 });
	}

Explicit way:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			Quantity = 5,
			UpdateDate = DateTime.UtcNow
		};
		var result = connection.Update("Order", entity, new QueryField("Id", 1002));
	}

UpdateAll
---------

Updates existing multiple data in the database.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			order.Quantity = 5;
			order.LastUpdatedUtc = DateTime.UtcNow;
		}
		connection.UpdateAll(orders);
	}

**Note**: The library uses the `PrimaryKey` as the default qualifier for `UpdateAll` operation. This call will throw an exception if the data entity does not have a `primary key` and the `qualifiers` were not provided.

The qualifiers can also be set when calling the `UpdateAll` operation.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			order.Quantity = 5;
			order.LastUpdatedUtc = DateTime.UtcNow;
		}
		connection.UpdateAll(orders, Field.From("CustomerId", "OrderId"));
	}

With the qualifiers above, the `UpdateAll` operation is using both `CustomerId` and `OrderId` fields as the qualifiers. The SQL is something like below.

.. code-block:: c#
	:linenos:

	UPDATE [Order] SET Quantity = @Quantity, LastUpdatedUtc = @LastUpdatedUtc WHERE CustomerId = @CustomerId AND OrderId = @OrderId;

**Note**: All fields are being updated when calling the typed-based method.

Certain columns can also be updated via table name calls.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query<Order>(o => o.CustomerId == 10045);
		var updatables = new List<dynamic>();
		for (var i = 0; i < 100; i++)
		{
			var order = orders.ElementAt(i);
			updatables.Add(new
			{
				Id = order.Id,
				Quantity = 5,
				LastUpdatedUtc = DateTime.UtcNow
			});
		}
		connection.UpdateAll("Order",
			entities: updatables,
			fields: Field.From("Id", "Quantity", "LastUpdatedUtc"));
	}

**Why passing the fields arguments?**

Be aware on this behavior since this is an update operation. By default, the library will use the database columns of the `Order` entity. If the values to that fields has not been set, then `UpdateAll` operation will set it to `null` in the database. By setting the `fields` argument, it will only update the listed `fields` in the batch operations.

**Note**: Use the table-name-based calls if the scenario is to only merge targetted columns.
