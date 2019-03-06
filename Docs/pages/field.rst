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
		Field.From(nameof(Order.Id))); // Field is being used as qualifier
	}
