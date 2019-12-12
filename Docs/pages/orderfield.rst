OrderField
==========

An object that holds a field for ordering purposes.

Creating a new Instance
-----------------------

The constructor accepts 2 parameters, a `Name` and an `Order` type.

By literal string:

.. code-block:: c#
	:linenos:

	// Ascending
	var orderField = new OrderField("Id", Order.Ascending);
	
	// Descending
	var orderField = new OrderField("Id", Order.Descending);

By class property:

.. code-block:: c#
	:linenos:

	// Ascending
	var orderField = new OrderField(nameof(Customer.Id), Order.Ascending);
	
	// Descending
	var orderField = new OrderField(nameof(Customer.Id), Order.Descending);

AsEnumerable
------------

Converts an instance of `OrderField` object into an `IEnumerable<OrderField>` object.

.. code-block:: c#
	:linenos:

	// Initialize an order field
	var orderField = new OrderField(nameof(Customer.Id), Order.Ascending);

	// Convert to enumerable
	var orderFields = orderField.AsEnumerable();

Ascending
---------

Parses a property from the data entity object based on the given `Expression` and converts the result to an `OrderField` object with `Order.Ascending` value.

.. code-block:: c#
	:linenos:

	var orderField = OrderField.Ascending<Customer>(c => c.Id);

Descending
----------

Parses a property from the data entity object based on the given `Expression` and converts the result to an `OrderField` object with `Order.Descending` value.

.. code-block:: c#
	:linenos:

	var orderField = OrderField.Descending<Customer>(c => c.Id);

Parse Expression
----------------

Parses a property from the data entity object based on the given `Expression` and converts the result to `OrderField` object.

Parse ascending:

.. code-block:: c#
	:linenos:

	var orderField = OrderField.Parse<Customer>(c => c.Id, Order.Ascending);

Parse descending:

.. code-block:: c#
	:linenos:

	var orderField = OrderField.Parse<Customer>(c => c.Id, Order.Descending);

Parse Object
------------

Parses an object properties to be used for ordering. The object can have multiple properties for ordering and each property must have a value of `Enumerations.Order` enumeration.

.. code-block:: c#
	:linenos:

	var orderFields = OrderField.Parse(new
	{
		UpdateDate = Order.Descending,
		FirstName = Order.Ascending
	});

Usage of OrderField
-------------------

The order field object is useful on certain operations.

Being the order fields in `BatchQuery` Operation:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orderBy = new
		{
			UpdatedDate = Order.Descending,
			FirstName = Order.Ascending
		};
		connection.BatchQuery<Order>(0,
			24,
			orderBy,
			new { d > 1000 });
	}

Being the order fields in `Query` operation:

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		var orderBy = new
		{
			Id = Order.Descending
		};
		var customerOrders = connection.Query<Order>(o => o.CustomerId == 10045,
			orderBy: orderBy);
	}
