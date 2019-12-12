QueryGroup
==========

A query group object is used to group an expression when composing a tree expressions. It is equivalent to a grouping on a `WHERE` statement in SQL Statements.

.. code-block:: c#
	:linenos:

	// Last 3 months customer with CustomerId >= 10045
	var query = new QueryGroup
	(
		new []
		{
			new QueryField("CustomerId", Operation.GreaterThanOrEqual, 10045),
			new QueryField("CreatedDate", Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3))
		},
		null, // Child QueryGroups
		Conjunction.And
	);

QueryField
----------

A query field is used as a field in the expression tree. It contains the actual field and the value for the equation.

.. code-block:: c#
	:linenos:

	// A field 'CustomerId' with value >= 10045
	var queryField = new QueryField("CustomerId", Operation.GreaterThanOrEqual, 10045);

Operations
----------

The query operation defines the operation to be used by the query expression (field level) during the actual execution. It is located at `RepoDb.Enumerations` namespace.

Let us say a connection object was created this way.

.. code-block:: c#
	:linenos:

	var connection = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");

Equal
-----

Part of the expression tree used to determine the `equality` of the field and data.

Dynamic way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new { Id == 10045 });

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.Id == 10045);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 10045 ));

NotEqual
--------

Part of the expression tree used to determine the `inequality` of the field and data.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.Name != "Anna Fullerton");

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Name", Operation.NotEqual, "Anna Fullerton" });

LessThan
--------

Part of the expression tree used to determine whether the field value is `less than` of the defined value.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.Id < 100);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.LessThan, 100 });

GreaterThan
-----------

Part of the expression tree used to determine whether the field value is `greater than` of the defined value.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.Id > 0);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.GreaterThan, 0 });

LessThanOrEqual
---------------

Part of the expression tree used to determine whether the field value is `less than or equal` of the defined value.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.Id <= 100);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>.Query(new QueryField("Id", Operation.LessThanOrEqual, 100 });

GreaterThanOrEqual
------------------

Part of the expression tree used to determine whether the field value is `greater than or equal` of the defined value.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.Id >= 100);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.GreaterThanOrEqual, 0 });

Like
----

Part of the expression tree used to determine whether the field is `identitical` to a given value.

Expression way:

.. code-block:: c#
	:linenos:

	// Contains (LIKE '%VAL%')
	var result = connection.Query<Customer>(c => c.Name.Contains("Anna"));
	
	// Contains (LIKE 'VAL%')
	var result = connection.Query<Customer>(c => c.Name.StartsWith("Anna"));

	// Contains (LIKE '%VAL')
	var result = connection.Query<Customer>(c => c.Name.EndsWith("Anna"));

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Name", Operation.Like, "Anna%" });

NotLike
-------

Part of the expression tree used to determine whether the field is `not identitical` to a given value. An opposite of `Operation.Like`.

Expression way:

.. code-block:: c#
	:linenos:

	// Contains (LIKE '%VAL%')
	var result = connection.Query<Customer>(c => !c.Name.Contains("Anna"));
	
	// Contains (LIKE 'VAL%')
	var result = connection.Query<Customer>(c => !c.Name.StartsWith("Anna"));

	// Contains (LIKE '%VAL')
	var result = connection.Query<Customer>(c => !c.Name.EndsWith("Anna"));

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Name", Operation.NotLike, "Anna%" });

Between
-------

Part of the expression tree used to determine whether the field value is `between` 2 given values.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.CreatedDate >= Date1 && c.CreatedDate <= Date2);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("CreatedDate", Operation.Between, new [] { Date1, Date2 } });

or

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.Between, new [] { 10045, 10075 } });

NotBetween
----------


Part of the expression tree used to determine whether the field value is `not between` 2 given values. An opposite of `Operation.Between`.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => c.CreatedDate < Date1 || c.CreatedDate > Date2);

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("CreatedDate", Operation.NotBetween, new [] { Date1, Date2 } });

or

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.NotBetween, new [] { 10045, 10075 } });

In
--

Part of the expression tree used to determine whether the field value is `in` given values.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => (new [] { 10045, 10046, 10047, 10048 }).Contains(c.Id));

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.In, new [] { 10045, 10046, 10047, 10048 } });

NotIn
-----

Part of the expression tree used to determine whether the field value is `not in` given values. An opposite of `Operation.In`. See sample below.

Expression way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(c => !(new [] { 10045, 10046, 10047, 10048 }).Contains(c.Id));

Explicit way:

.. code-block:: c#
	:linenos:

	var result = connection.Query<Customer>(new QueryField("Id", Operation.NotIn, new [] { 10045, 10046, 10047, 10048 } });

IsForUpdate
-----------

This method is used to make the instance of `QueryGroup` or `QueryField` object to become an expression for `Update` operations.

It is very useful when calling the update via `TableName` in which targetting specific field is conflicting with the actual valued-parameters.

Let us say the table `Customer` is existing in the database.

.. code-block:: c#
	:linenos:

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT PRIMARY IDENTITY(1, 1),
		[Name] NVARCHAR(256) NOT NULL
	)

And the update operation has been called like below.

.. code-block:: c#
	:linenos:

	// Calls
	var expression = new QueryField("Id", 10045);
	connection.Update("Customer", new { Name = "John Doe" }, expression);

	// Generated SQL
	UPDATE [dbo].[Customer] SET Name = @Name WHERE (Id = @Id);

In which the value of `@Name` parameter is `John Doe` and the value of `@Id` parameter is `10045`.

However, what if the expression/condition is based on the `Name` field itself?

.. code-block:: c#
	:linenos:

	// Calls
	var expression = new QueryField("Name", "Jay Doe");
	connection.Update("Customer", new { Name = "John Doe" }, expression);

	// Would you expect it like this?
	UPDATE [dbo].[Customer] SET Name = @Name WHERE (Name = @Name);

That is wrong. By calling the `IsForUpdate` method, the returned SQL would be different.

.. code-block:: c#
	:linenos:

	// Calls
	var expression = new QueryField("Name", "Jay Doe");
	expression.IsForUpdate();
	connection.Update("Customer", new { Name = "John Doe" }, expression);

	// Would you expect it like this?
	UPDATE [dbo].[Customer] SET Name = @Name WHERE (Name = @_Name);

In which the value of `@Name` parameter is `John Doe` and the value of `@_Name` parameter is `Jay Doe`.

**Note:** The library is automatically calling this method in all `Update` operations.

Reusability
-----------

The instance of `QueryGroup` and `QueryField` is reusable with certain conditions.

.. code-block:: c#
	:linenos:

	// Initialize the field
	var field = new QueryField(nameof(Customer.Id), 10045);

	// Use first at inline-update
	var affectedRows = connection.InlineUpdate(new { LastUpdatedDate = DateTime.UtcNow }, field);

	// Reset before reuse
	field.Reset();

	// Use again in query
	var customer = connection.Query<Customer>(field).First();

Behind the scene, the library is modifying the state of the `QueryField` object before using it to actual operation.

By calling the `Reset` method, the state of the `QueryField` will be reset back to normal (as it was as newly instantiated). This condition also applies to the `QueryGroup` object.

Below is a code to reset an `IEnumerable<QueryField>` objects.

.. code-block:: c#
	:linenos:

	// Initialize the fields
	var fields = new []
	{
		new QueryField(nameof(Order.CustomerId), 10045),
		new QueryField(nameof(Order.ProductId), 12)
	};

	// Do updates first using the fields variable
	...

	// Reset all before reuse
	fields.ResetAll();

	// Do query using the same fields
	...
