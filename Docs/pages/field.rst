Field
=====

An object that signifies as data field in the query statement.

Creating a new Instance
-----------------------

Constructor only accepts a single string parameter.

.. highlight:: c#

::

	var field = new Field("Id");

AsEnumerable
------------

Converts an instance of a `Field` into an `IEnumerable<Field>` object.

.. highlight:: c#

::

	// Initialize a field
	var field = new Field("Id");

	// Convert to enumerable
	var fields = field.AsEnumerable();

From
----

Use the `From` method to parse an array of string together.

.. highlight:: c#

By literal strings:

::

	var fields = Field.Parse("Id", "Name");

By class property:

::

	var fields = Field.Parse(nameof(Customer.Id), nameof(Customer.Name));

Parse (via Expression)
----------------------

Parses a property from the data entity object based on the given `Expression` and converts the result to `Field` object.

.. highlight:: c#

::

	var field = Field.Parse<Customer>(c => c.Id);

Then call the `AsEnumerable()` extension method to convert it to enumerable (if necessary);

::

	var fields = Field.Parse<Customer>(c => c.Id).AsEnumerable();

Parse (via Object)
----------------------

Parse an object and creates an enumerable of `Field` objects. Each field is equivalent to each property of the given object. The parse operation uses a reflection operation.

.. highlight:: c#

Let us say an `Order` record was queried from the database.

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Order variables
		var order = (Order)null;
		
		// Query from the database
		connection.Query<Order>(1002);
	}

Then, the object propery can then be used for parsing.

::

	var field = Field.Parse<Order>(new { order.Id });

Usage of Field
--------------

The field object is useful on certain operations.

Being the qualifers in `InlineMerge` Operation:

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.InlineMerge<Order>(new
		{
			CustomerId = 10045,
			ProductId = 12,
			Quantity = 5,
			LastUpdatedUtc = DateTime.UtcNow
		},
		Field.From(nameof(Order.CustomerId), nameof(Order.ProductId)); // Field is being used as qualifier
	}

Being the qualifers in `Merge` operation:

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
		Field.Parse<Order>(o => o.Id).AsEnumerable()); // Field is being used as qualifier
	}
