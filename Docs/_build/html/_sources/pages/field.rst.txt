Field
=====

An object that signifies as data field in the query statement.

Creating a new Instance
-----------------------

The constructor accepts 2 parameters, a `Name` and a `Type`.

.. highlight:: c#

By literal string:

::

	var field = new Field(nameof(Customer.Id));

Or

	var field = new Field(nameof(Customer.Id), typeof(int));

The library uses the `Type` parameter to be resolved when any of the operation is being called.

AsEnumerable
------------

Converts an instance of `Field` object into an `IEnumerable<Field>` object.

.. highlight:: c#

::

	// Initialize a field
	var field = new Field(nameof(Customer.Id));

	// Convert to enumerable
	var fields = field.AsEnumerable();

From
----

Is used to parse an array of strings and convert it back as an enumerable.

.. highlight:: c#

By literal strings:

::

	var fields = Field.From("Id", "Name");

By class property:

::

	var fields = Field.From(nameof(Customer.Id), nameof(Customer.Name));

**Note**:  When using this method, the `Type` parameter is not being set. The library will then not set the `DbType` value of the `DbCommand` object. By default, ADO.NET uses the `DbType.String` value.

Usage of Field
--------------

The field object is mostly used as a queryable fields and qualifiers at some operations.

Being the fields in the `Query` operation via table name:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query("Order", Field.From("Id", "CustomerId", "ProductId"));
	}

Being the qualifers in the `Merge` operation:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Merge<Order>(new Order
		{
			Id = 1002,
			CustomerId = 10045,
			ProductId = 12,
			Quantity = 5,
			LastUpdatedUtc = DateTime.UtcNow
		},
		qualifiers: Field.From(nameof(Order.Id)));
	}

Being the target fields and qualifers in the `MergeAll` operation via table name:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var entities = new[]
		{
			new
			{
				Id = 1002,
				CustomerId = 10045,
				ProductId = 12,
				Quantity = 5,
				LastUpdatedUtc = DateTime.UtcNow
			},
			new
			{
				Id = 1003,
				CustomerId = 10224,
				ProductId = 19,
				Quantity = 2,
				LastUpdatedUtc = DateTime.UtcNow
			}
		};
		connection.Merge(tableName: "Order",
			entities: entities,
			qualifiers: Field.From("Id"),
			fields: Field.From("CustomerId", "ProductId", "Quantity", "LastUpdatedUtc"));
	}

Being the target fields and qualifers in the `MergeAll` operation via table name:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var entities = new[]
		{
			new
			{
				Id = 1002,
				Quantity = 5,
				LastUpdatedUtc = DateTime.UtcNow
			},
			new
			{
				Id = 1003,
				Quantity = 2,
				LastUpdatedUtc = DateTime.UtcNow
			}
		};
		connection.Merge(tableName: "Order",
			entities: entities,
			qualifiers: Field.From("Id"),
			fields: Field.From("Quantity", "LastUpdatedUtc"));
	}