Field
=====

An object that signifies as data field in the query statement.

Creating a new Instance
-----------------------

Constructor only accepts a single string parameter.

.. highlight:: c#

By literal string:

::

	var field = new Field("Id");

By class property:

::

	var field = new Field(nameof(Customer.Id));

AsEnumerable
------------

Converts an instance of a `Field` into an `IEnumerable<Field>` object.

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

Usage of Field
--------------

The field object is useful on certain operations.

Being the qualifers in the `InlineMerge` Operation:

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
		Field.From(nameof(Order.CustomerId), nameof(Order.ProductId)); // If CustomerId and ProductId is unique
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
		Field.From(nameof(Order.Id)));
	}
