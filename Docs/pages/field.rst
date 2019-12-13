Field
=====

An object that signifies as data field in the query statement.

Creating a new Instance
-----------------------

The constructor accepts 2 parameters, a `Name` and a `Type`.

By literal string:

.. code-block:: c#
	:linenos:

	var field = new Field(nameof(Customer.Id));

Or

.. code-block:: c#
	:linenos:

	var field = new Field(nameof(Customer.Id), typeof(int));

The library uses the `Type` parameter to be resolved when any of the operation is being called.

AsEnumerable
------------

Converts an instance of `Field` object into an `IEnumerable<Field>` object.

.. code-block:: c#
	:linenos:

	// Initialize a field
	var field = new Field(nameof(Customer.Id));

	// Convert to enumerable
	var fields = field.AsEnumerable();

From
----

Is used to parse an array of strings and convert it back as an enumerable.

By literal strings:

.. code-block:: c#
	:linenos:

	var fields = Field.From("Id", "Name");

By class property:

.. code-block:: c#
	:linenos:

	var fields = Field.From(nameof(Customer.Id), nameof(Customer.Name));

**Note**:  When using this method, the `Type` parameter is not being set. The library will then not set the `DbType` value of the `DbCommand` object. By default, ADO.NET uses the `DbType.String` value.

Parse Type
----------

Is used to parse a .NET CLR type and convert it back as an enumerable.

.. code-block:: c#
	:linenos:

	var fields = Field.Parse(typeof(Customer));

Parse Entity
------------

Is used to parse an entity type and convert it back as an enumerable.

.. code-block:: c#
	:linenos:

	var fields = Field.Parse<Customer>();


Parse Object
------------

Is used to parse an object and convert it back as an enumerable.

.. code-block:: c#
	:linenos:

	var customer = new Customer();
	var fields = Field.Parse(customer);

Parse Expression
----------------

Is used to parse an expression and convert it back as an enumerable.

.. code-block:: c#
	:linenos:

	var fields = Field.Parse<Customer>(e => e.Id).AsEnumerable();

Usage of Field
--------------

The field object is mostly used as a queryable fields and qualifiers at some operations.

Being the fields in the `Query` operation via table name:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orders = connection.Query("Order", Field.From("Id", "CustomerId", "ProductId"));
	}

Being the qualifers in the `Merge` operation:

.. code-block:: c#
	:linenos:

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
		qualifiers: Field.From("CustomerId", "ProductId"));
	}

Being the target fields and qualifers in the `MergeAll` operation via table name:

.. code-block:: c#
	:linenos:

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
		connection.MergeAll(tableName: "Order",
			entities: entities,
			qualifiers: Field.From("Id"),
			fields: Field.From("CustomerId", "ProductId", "Quantity", "LastUpdatedUtc"));
	}

Also, being the target fields and qualifers in the `UpdateAll` operation via table name:

.. code-block:: c#
	:linenos:

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
		connection.UpdateAll(tableName: "Order",
			entities: entities,
			fields: Field.From("Id", "Quantity", "LastUpdatedUtc"));
	}
