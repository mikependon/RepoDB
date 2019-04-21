Connection
==========

The library has abstracted everything from `ADO.NET` when it comes to the connection object.

BatchQuery
----------

Queries a data from the database by batch.

.. highlight:: c#

Dynamic way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(0,
			24,
			OrderField.Parse(new { Id = Order.Ascending }),
			new { CustomerId = 10045 });
	}

Expression way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(0,
			24,
			OrderField.Ascending<Order>(o => o.Id),
			o => o.CustomerId = 10045);
	}

Explicit way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(0,
			24,
			new OrderField("Id", Order.Ascending).AsEnumerable(),
			new QueryField(nameof(Order.CustomerId), 10045));
	}

BulkInsert
----------

Bulk insert a list of data entity objects into the database.

.. highlight:: c#

Create a list to hold the data entities.

::

	var orders = new List<Order>();

Add each item to be bulk-inserted.

::

	orders.Add(new Order()
	{
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	});

Call the `BulkInsert` operation to insert the data.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.BulkInsert<Order>(orders);
	}

The result would be the number of rows affected by the `BulkInsert` in the database.

This operation also support the `DbDataReader` object. However, by using this, the result would always be `-1`.

.. highlight:: c#

Initiate a `DbDataReader` object from the source connection.

::

	using (var sourceConnection = new SqlConnection(@"Server=.;Database=Northwind_Old;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT Quantity, ProductId, GETUTCDATE() AS CreatedDate, GETUTCDATE() AS UpdatedDate FROM [dbo].[Order];"))
		{
			// Do the stuffs here
		}
	}

Call the `BulkInsert` operation by passing the `DbDataReader` object as the parameter.

::

	using (var destinationConnection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Via entity type
		var affectedRows = connection.BulkInsert<Order>(reader);

		// Via table name
		var affectedRows = connection.BulkInsert(reader, nameof(Order));
	}

Count
-----

Counts the number of table data from the database.

.. highlight:: c#

Dynamic way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.Count<Order>(new { CustomerId = 10045 });
	}

Expression way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.Count<Order>(o => o.CustomerId == 10045);
	}

Explicit way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.Count<Order>(new QueryField(nameof(Order.CustomerId), 10045));
	}

Records can all also be counted via table name.

Dynamic way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.Count("Order", new { CustomerId = 10045 });
	}

Explicit way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.Count("Order", new QueryField(nameof(Order.CustomerId), 10045));
	}

**Note**: By setting the `where` argument to blank would count all the records. Exactly the same as `CountAll` operation.

CountAll
--------

Counts all the table data from the database.

.. highlight:: c#

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.CountAll<Order>();
	}

with hints.

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.Count<Order>(SqlTableHints.NoLock);
	}

All records can all also be counted via table name.

Dynamic way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.CountAll("Order");
	}

with hints.

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var counted = connection.CountAll("Order", SqlTableHints.NoLock);
	}

CreateCommand
-------------

.. highlight:: c#

Creates a command object.

::

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

.. highlight:: c#

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(10045);
	}

Via Dynamic:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(new { Id = 10045 });
	}
	
Expression way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(c => c.Id == 10045);
	}
	
Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(new QueryField(nameof(Order.CustomerId), 10045));
	}

Records can also be deleted via table name.

Via Dynamic:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete("Customer", new { Id = 10045 });
	}
	
Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete("Customer", new QueryField(nameof(Order.CustomerId), 10045));
	}

**Note**: By setting the `where` argument to blank would delete all the records. Exactly the same as `DeleteAll` operation.

DeleteAll
---------

Deletes all the data from the database.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.DeleteAll<Customer>();
	}

All records can also be deleted via table name.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.DeleteAll("Customer");
	}
	
EnsureOpen
----------

.. highlight:: c#

Ensures the connection object is open.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Use the connection here
	}
	
ExecuteNonQuery
---------------

.. highlight:: c#

Executes a query from the database. It uses the underlying method `IDbCommand.ExecuteNonQuery` and returns the number of affected rows during the execution.

::

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

::

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

The instance of ExpandoObject and IDictionary<string, object> can also be used as parameter.

Via ExpandoObject as dynamic.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Create the parameters
		var parameters = (dynamic)new ExpandoObject();

		// Set each parameter
		param.OrderId = 1002;
		param.Quantity = 5;
		param.LastUpdatedUtc = DateTime.UtcNow

		// Create the parameters
		var result = connection.ExecuteNonQuery("[dbo].[sp_update_order_quantity]", parameters, commandType: CommandType.StoredProcedure);
	}

Via ExpandoObject as Dictionary<string, object>.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Create the parameters
		var parameters = new ExpandoObject() as IDictionary<string, object>;

		// Add each parameter
		param.Add("OrderId", 1002);
		param.Add("Quantity", 5);
		param.Add("LastUpdatedUtc", DateTime.UtcNow);

		// Pass the parameters
		var result = connection.ExecuteNonQuery("[dbo].[sp_update_order_quantity]", parameters, commandType: CommandType.StoredProcedure);
	}


Via Dictionary<string, object>.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Create the parameters
		var parameters = new Dictionary<string, object>
		{
			{ "OrderId", 1002 },
			{ "Quantity", 5 },
			{ "LastUpdatedUtc", DateTime.UtcNow }
		};

		// Pass the parameters
		var result = connection.ExecuteNonQuery("[dbo].[sp_update_order_quantity]", parameters, commandType: CommandType.StoredProcedure);
	}

**Note**: The passing of the `ExpandoObject` and `IDictionary<string, object>` parameter is also supported in `ExecuteQuery`, `ExecuteScalar` and `ExecuteReader` methods.

ExecuteQuery
------------

Executes a query from the database. It uses the underlying method `IDbCommand.ExecuteReader` and converts the result back to an enumerable list of dynamic objects.

.. highlight:: c#

::

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

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var result = connection.ExecuteNonQuery("[dbo].[sp_get_customer]",
			new { CustomerId = 10045 },
			commandType: CommandType.StoredProcedure);
	}

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

Note: Calling the `ExecuteQuery` via dynamic is a bit slower compared to a .NET CLR Type-based calls.

ExecuteQueryMultiple
--------------------

Executes a multiple query statement from the database and allows the user to extract the result to a target data entity.

.. highlight:: c#

::

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

.. highlight:: c#

::

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

.. highlight:: c#

::

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

.. highlight:: c#

::

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

::

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

.. highlight:: c#

::

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

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var maxId = Convert.ToInt64(connection.ExecuteScalar("[dbo].[sp_get_latest_customer_id]", commandType: CommandType.StoredProcedure)));
	}

A dynamic typed-based call is also provided, see below.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var maxId = connection.ExecuteScalar<long>("[dbo].[sp_get_latest_customer_id]", commandType: CommandType.StoredProcedure));
	}

Insert
------

Inserts a new data in the database.

.. highlight:: c#

::

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

A dynamic typed-based call is also provided when calling this method, see below.

::

	// The first type is the entity type, the second type is the result type
	var id = connection.Insert<Order, long>(order);

**Certain** columns can also be inserted via table name calls.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			CustomerId = 10045,
			ProductId = 12
			Quantity = 2,
			CreatedDate = DateTime.UtcNow
		};
		var id = connection.Insert<long>("Order", entity);
	}

**Note**: Use the table name based if the scenario is to only insert targetted columns.

Merge
-----

Merges a data entity object into an existing data in the database.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var order = connection.Query<Order>(1);
		order.Quantity = 5;
		UpdatedDate = DateTime.UtcNow;
		connection.Merge(order, Field.Parse<Order>(o => o.Id));
	}

In the second parameter, the `Field.From` method can also be used.

::
	
	var id = connection.Merge<Order>(entity, Field.From(nameof(Order.Id)));

Or, via a literal array of string.

::

	var id = connection.Merge<Order>(entity, Field.From("Id"));

**Note**: The second parameter can be omitted if the data entity has a primary key.

**Certain** columns can also be merged via table name calls.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			Id = 1,
			Quantity = 5,
			UpdatedDate = DateTime.UtcNow
		};
		connection.Merge("Order", entity, Field.From("Id"));
	}

**Note**: Use the table name based if the scenario is to only merge targetted columns.

Query
-----

Queries a data from the database.

.. highlight:: c#

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(10045).FirstOrDefault();
	}
	
Via Dynamic:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(new { Id = 10045 }).FirstOrDefault();
	}

Expression way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(c => c.Id == 10045);
	}

Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customers = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 10045));
	}

QueryMultiple
-------------

Query a multiple resultsets from the database.

Below is an example of how to query a customer where the `Id` field is equals to `10045`, and at the same time, querying all the orders connected to this customer since yesterday.
The result is an instance of a `Tuple` object.

.. highlight:: c#

::

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

.. highlight:: c#

::

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

.. highlight:: c#

::

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

.. highlight:: c#

::

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

.. highlight:: c#

::

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

Truncate
--------

Truncates a table from the database.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Truncate<Customer>();
	}

Table can also be truncated via table name.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Truncate("Customer");
	}

Update
------

Updates an existing data in the database.

.. highlight:: c#

Let us say an `Order` object was queried from the database.

::

	// Query a data from the database
	var order = connection.Query<Order>(1002).FirstOrDefault();

	// Set the target properties
	order.Quantity = 5;
	order.UpdateDate = DateTime.UtcNow;

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update<Order>(order, 1002);
	}

Note: This call will throw an exception if the data entity does not have a primary key.

Via Dynamic:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update<Order>(order, new { Id = 1002 });
	}

Expression way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update<Order>(order, o => o.Id == 1002);
	}

Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update(order, new QueryField(nameof(Order.Id), 1002));
	}

Record can also be updated via table name.

Dynamic way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			Quantity = 5,
			UpdateDate = DateTime.UtcNow
		};
		var affectedRows = connection.Update("Order", entity, new { Id = 1002 });
	}

Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Instantiate a dynamic object (not really an "Order" object)
		var entity = new
		{
			Quantity = 5,
			UpdateDate = DateTime.UtcNow
		};
		var affectedRows = connection.Update("Order", entity, new QueryField("Id", 1002));
	}
