Connection
==========

The library has abstracted everything from `ADO.NET` when it comes to the connection object.

BatchQuery
----------

Query the data from the database by batch.

.. highlight:: c#

Expression way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(o => o.CustomerId == 10045, 0, 24);
	}

Explicit way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.BatchQuery<Order>(new QueryField(nameof(Order.CustomerId), 10045), 0, 24);
	}

BulkInsert
----------

Bulk-inserting the list of data entity objects in the database.

.. highlight:: c#

Let us say a variable named `orders` is present, which is an enumerable of data entity.

::

	var orders = new List<Order>();

From there, a data entity record can be added.

::

	orders.Add(new Order()
	{
		Quantity = 2,
		ProductId = 12,
		CreatedDate = DateTime.UtcNow,
		UpdatedDate = DateTime.UtcNow
	});

Then simply call the `BulkInsert` operation, passing the enumerable object of the data entity.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.BulkInsert<Order>(entities);
	}

The result would be the number of rows affected by the `BulkInsert` in the database.

This operation also support the `DbDataReader` object.

.. highlight:: c#

Let us say a data reader object named `reader` has been initiated from the source connection.

::

	using (var sourceConnection = new SqlConnection(@"Server=.;Database=Northwind_Old;Integrated Security=SSPI;").EnsureOpen())
	{
		using (var reader = sourceConnection.ExecuteReader("SELECT Quantity, ProductId, GETUTCDATE() AS CreatedDate, GETUTCDATE() AS UpdatedDate FROM [dbo].[Order];"))
		{
			// Do the stuffs here
		}
	}

Then simply call the `BulkInsert` operation by simply passing the data reader object as the parameter.

::

	using (var destinationConnection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.BulkInsert<Order>(reader);
	}

Count
-----

Counts the number of rows from the database.

.. highlight:: c#

Expression way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Count<Order>(o => o.Id >= 1000 && o.CreatedDate >= DateTime.UtcNow.Date.AddMonths(-1) });
	}

Explicit way:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var queryGroup = new QueryGroup(new []
		{
			new QueryField(nameof(Order.Id), Operation.GreaterThan, 1000),
			new QueryField(nameof(Order.CreatedDate), Operation.GreaterThan, DateTime.UtcNow.Date.AddMonths(-1)),
		});
		connection.Count<Order>(queryGroup);
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
	}

Delete
------

Deletes a data in the database based on the given query expression.

.. highlight:: c#

Via DataEntity:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = GetCustomer(1005);
		var affectedRows = connection.Delete<Customer>(customer);
	}

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(1005);
	}
	
Deleting a data entity without a primary key will throw a `PrimaryFieldNotFoundException` exception.

**Note**: By leaving the `WHERE` parameter to blank would delete all records. Exactly the same as `DeleteAll` operation.

Expression way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(c => c.Id == 1005);
	}
	
Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Delete<Customer>(new QueryField(nameof(Customer.Id), 1005));
	}

DeleteAll
---------

Deletes all records from the database.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.DeleteAll<Customer>();
	}

EnsureOpen
----------

.. highlight:: c#

Ensure that the connection object is on open state.

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
		var parameters = new
		{
			OrderId = 1002,
			Quantity = 5,
			LastUpdatedUtc = DateTime.UtcNow
		};
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
		var parameters = new
		{
			OrderId = 1002,
			Quantity = 5,
			LastUpdatedUtc = DateTime.UtcNow
		};
		var result = connection.ExecuteNonQuery("[dbo].[sp_update_order_quantity]", parameters, commandType: CommandType.StoredProcedure);
	}

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

ExecuteQueryMultiple
--------------------

Executes a multiple query statement from the database and allows the user to extract the result to a target data entity.

.. highlight:: c#

::

    using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
    {
        var commandText = "SELECT * FROM Customer WHERE Id = @CustomerId; SELECT * FROM [Order] WHERE CustomerId = @CustomerId;";
        using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 1 }))
        {
			// Extract the first result
            var customers = result.Extract<Customer>();
            customers?
                .ToList()
                .ForEach(c =>
				{
					// Do something here for the target Customer
				});

			// Advance to the next result
            result.NextResult();

			// Extract the second result
            var orders = result.Extract<Order>();
            orders?
                .ToList().ForEach(o =>
				{
					// Do something here for Orders
				});
        }
    }

The method `ExtractNext` is used to simply the extraction of the next result. By default, this throws an `InvalidOperationException` if there is no next resultset in the `DbDataReader`.

.. highlight:: c#

::

    using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
    {
        var commandText = "SELECT * FROM Customer WHERE Id = @CustomerId; SELECT * FROM [Order] WHERE CustomerId = @CustomerId;";
        using (var result = connection.ExecuteQueryMultiple(commandText, new { CustomerId = 1 }))
        {
			// Extract the first result
            var customers = result.Extract<Customer>();
            customers?
                .ToList()
                .ForEach(c =>
				{
					// Do something here for the target Customer
				});

			// Extract the second result through 'ExtractNext' method
            var orders = result.ExtractNext<Order>();
            orders?
                .ToList().ForEach(o =>
				{
					// Do something here for Orders
				});
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
		var maxId = connection.ExecuteScalar("SELECT MAX([Id]) AS MaxId FROM [dbo].[Customer];");
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
		var maxId = connection.ExecuteReader("[dbo].[sp_get_latest_customer_id]", commandType: CommandType.StoredProcedure));
	}

InlineInsert
------------

Inserts a data in the database by targetting certain fields only.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Not really an order object, instead, it is a dynamic object
		var entity = new
		{
			CustomerId = 10045,
			ProductId = 35,
			Quantity = 5,
			CreatedDate = DateTime.UtcNow
		};

		// Call the operation and define which object you are targetting
		var id = connection.InlineInsert<Order>(entity);
	}

InlineMerge
-----------

Merges a data in the database by targetting certain fields only.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Not really an order object, instead, it is a dynamic object
		var entity = new
		{
			Id = 1002,
			CustomerId = 10045,
			ProductId = 35,
			Quantity = 5,
			CreatedDate = DateTime.UtcNow
		};

		// Call the operation and define which object you are targetting
		var id = connection.InlineMerge<Order>(entity, new []
		{
			Field.Parse<Order>(o => o.Id),
			Field.Parse<Order>(o => o.CustomerId)
		});
	}

In the second parameter, the `Field.From` method can also be used.

::
	
	var id = connection.InlineMerge<Order>(entity, Field.From(nameof(Order.Id), nameof(Order.CustomerId)));

Or, via a literal array of string.

::

	var id = connection.InlineMerge<Order>(entity, Field.From("Id", "CustomerId"));

Or, via a single field expression can be used as well.

::

	var id = connection.InlineMerge<Order>(entity, o => o.CustomerId); // Only works for single qualifier

**Note**: The second parameter can be omitted if the data entity has a primary key.

InlineUpdate
------------

Updates a data in the database by targetting certain fields only.

.. highlight:: c#

Let us say a dynamic entity is defined.

::

	// Not really an Customer object, instead, it is a dynamic object
	var entity = new
	{
		Name = "Anna Fullerton",
		UpdatedDate = DateTime.UtcNow
	};

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Call the operation and define which object you are targetting
		var id = connection.InlineUpdate<Customer>(entity, 10045);
	}

Expression way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Call the operation and define which object you are targetting
		var id = connection.InlineUpdate<Customer>(entity, o => o.Id == 10045);
	}

Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Call the operation and define which object you are targetting
		var id = connection.InlineUpdate<Customer>(entity, new QueryField(nameof(Customer.Id), 10045));
	}

Insert
------

Inserts a data in the database.

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
		connection.Insert(order);
	}

IsForProvider
-------------

Checks whether the current used connection object is targetting a specific DB provider.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var isSql = connection.IsForProvider(Provider.Sql);
	}

Merge
-----

Merges an existing data entity object in the database. By default, this operation uses the primary key property as the qualifier.

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

Or, via a single field expression can be used as well.

::

	var id = connection.Merge<Order>(entity, o => o.CustomerId); // Only works for single qualifier

**Note**: The second parameter can be omitted if the data entity has a primary key.

Query
-----

Query a data from the database.

.. highlight:: c#

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Query<Customer>(10045).FirstOrDefault();
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

Truncate
--------

Truncates a table from the database.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var customer = connection.Truncate<Customer>();
	}

Update
------

Updates a data in the database based on the given query expression.

.. highlight:: c#

Let us say an `Order` object was queried from the database.

::
	
		// Query a data from the database
		var order = connection.Query<Order>(1002).FirstOrDefault();

		// Set the target properties
		order.Quantity = 5;
		order.UpdateDate = DateTime.UtcNow;

Via DataEntity:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update(order);
	}

Note: This call will throw an exception if the data entity does not have a primary key.

Via PrimaryKey:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update(order, 1002);
	}

Expression way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update(order, o => o.Id == 1002);
	}

Explicit way:

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var affectedRows = connection.Update(order, new QueryField(nameof(Order.Id), 1002));
	}
