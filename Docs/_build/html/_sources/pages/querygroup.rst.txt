QueryGroup
==========

A query group object is used to group an expression when composing a tree expressions. It is equivalent to a grouping on a `WHERE` statement in SQL Statements.

.. highlight:: c#

::

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

Operations
----------

The query operation defines the operation to be used by the query expression (field level) during the actual execution. It is located at `RepoDb.Enumerations` namespace.

.. highlight:: c#

Let us say a connection object was created this way.

::

	var connection = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");

Equal
-----

.. highlight:: c#

Part of the expression tree used to determine the `equality` of the field and data.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = 10045 });

or

::

	var result = connection.Query<Customer>(new
	{
		Id = new { Operation = Operation.Equal, Value = 10045 }
	});

Expression way:

::

	var result = connection.Query<Customer>(c => c.Id == 10045);

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.Equal, 10045 ));

NotEqual
--------

.. highlight:: c#

Part of the expression tree used to determine the `inequality` of the field and data.

Dynamic way:

::

	var result = connection.Query<Customer>(new
	{
		Name = new { Operation = Operation.NotEqual, Value = "Anna Fullerton" }
	});

Expression way:

::

	var result = connection.Query<Customer>(c => c.Name != "Anna Fullerton");

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Name", Operation.NotEqual, "Anna Fullerton" });

LessThan
--------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `less than` of the defined value.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.LessThan, Value = 100 } });

Expression way:

::

	var result = connection.Query<Customer>(c => c.Id < 100);

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.LessThan, 100 });

GreaterThan
-----------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `greater than` of the defined value.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.GreaterThan, Value = 0 } });

Expression way:

::

	var result = connection.Query<Customer>(c => c.Id > 0);

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.GreaterThan, 0 });

LessThanOrEqual
---------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `less than or equal` of the defined value.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.LessThanOrEqual, Value = 100 } });

Expression way:

::

	var result = connection.Query<Customer>(c => c.Id <= 100);

Explicit way:

::

	var result = connection.Query<Customer>.Query(new QueryField("Id", Operation.LessThanOrEqual, 100 });

GreaterThanOrEqual
------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `greater than or equal` of the defined value.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.GreaterThanOrEqual, Value = 0 } });

Expression way:

::

	var result = connection.Query<Customer>(c => c.Id >= 100);

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.GreaterThanOrEqual, 0 });

Like
----

.. highlight:: c#

Part of the expression tree used to determine whether the field is `identitical` to a given value.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Name = new { Operation = Operation.Like, Value = "Anna%" } });

Expression way:

::

	// Contains (LIKE '%VAL%')
	var result = connection.Query<Customer>(c => c.Name.Contains("Anna")); // Supported at version above 1.7.0-beta1
	
	// Contains (LIKE 'VAL%')
	var result = connection.Query<Customer>(c => c.Name.StartsWith("Anna")); // Supported at version above 1.7.0-beta1

	// Contains (LIKE '%VAL')
	var result = connection.Query<Customer>(c => c.Name.EndsWith("Anna")); // Supported at version above 1.7.0-beta1

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Name", Operation.Like, "Anna%" });

NotLike
-------

.. highlight:: c#

Part of the expression tree used to determine whether the field is `not identitical` to a given value. An opposite of `Operation.Like`.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Name = new { Operation = Operation.NotLike, Value = "Anna%" } });

Expression way:

::

	// Contains (LIKE '%VAL%')
	var result = connection.Query<Customer>(c => !c.Name.Contains("Anna")); // Supported at version above 1.7.0-beta1
	
	// Contains (LIKE 'VAL%')
	var result = connection.Query<Customer>(c => !c.Name.StartsWith("Anna")); // Supported at version above 1.7.0-beta1

	// Contains (LIKE '%VAL')
	var result = connection.Query<Customer>(c => !c.Name.EndsWith("Anna")); // Supported at version above 1.7.0-beta1

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Name", Operation.NotLike, "Anna%" });

Between
-------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `between` 2 given values.

Dynamic way:

::

	var result = connection.Query<Customer>(new { CreatedDate = new { Operation = Operation.Between, Value = new [] { Date1, Date2 } } });

or

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.Between, Value = new [] { 10045, 10075 } } });

Expression way:

::

	var result = connection.Query<Customer>(c => c.CreatedDate >= Date1 && c.CreatedDate <= Date2);

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("CreatedDate", Operation.Between, new [] { Date1, Date2 } });

or

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.Between, new [] { 10045, 10075 } });

NotBetween
----------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not between` 2 given values. An opposite of `Operation.Between`.

Dynamic way:

::

	var result = connection.Query<Customer>(new { CreatedDate = new { Operation = Operation.NotBetween, Value = new [] { Date1, Date2 } } });

or

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.NotBetween, Value = new [] { 10045, 10075 } } });
	
Expression way:

::

	var result = connection.Query<Customer>(c => c.CreatedDate < Date1 || c.CreatedDate > Date2);

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("CreatedDate", Operation.NotBetween, new [] { Date1, Date2 } });

or

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.NotBetween, new [] { 10045, 10075 } });

In
--

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `in` given values.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.In, Value = new [] { 10045, 10046, 10047, 10048 } } });
	
Expression way:

::

	var result = connection.Query<Customer>(c => (new [] { 10045, 10046, 10047, 10048 }).Contains(c.Id)); // Supported at version above 1.7.0-beta1

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.In, new [] { 10045, 10046, 10047, 10048 } });

NotIn
-----

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not in` given values. An opposite of `Operation.In`. See sample below.

Dynamic way:

::

	var result = connection.Query<Customer>(new { Id = new { Operation = Operation.NotIn, Value = new [] { 10045, 10046, 10047, 10048 } } });
	
Expression way:

::

	var result = connection.Query<Customer>(c => !(new [] { 10045, 10046, 10047, 10048 }).Contains(c.Id)); // Supported at version above 1.7.0-beta1

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.NotIn, new [] { 10045, 10046, 10047, 10048 } });

All
---

.. highlight:: c#

Part of the expression tree used to determine whether `all` the field values satisfied the criteria.

Dynamic way:

::

	var result = connection.Query<Customer>(new
	{
		Name = new
		{
			Operation = Operation.All, // Works as AND
			Value = new object[]
			{
				new { Operation = Operation.Like, Value = "Anna%" },
				new { Operation = Operation.NotEqual, Value = "Tom Hawks" },
				new { Operation = Operation.NotIn, Value = new string[] { "Frank Myers", "Joe Austin" } }
			}
		}
	});

Expression way:

::

	// Same operations (Equal)
	var result = connection.Query<Customer>(c => (new [] { "Anna", "Tom Hawks", "Frank Myers", "Joe Austin" }).All(c.Name));

	// Different operations (Equal, Like, NotEqual, GreaterThan, etc)
	var result = connection.Query<Customer>(c => c.Name.Contains("Anna") && c.Name != "Tom Hawks" && !(new [] { "Frank Myers", "Joe Austin" }).Contains(c.Name)); // Supported at version above 1.7.0-beta1

Explicit way:

::

	var result = connection.Query<Customer>
	(
		new QueryField[]
		{
			new QueryField("Name", Operation.Like, "Anna%"),
			new QueryField("Name", Operation.NotEqual, "Tom Hawks"),
			new QueryField("Name", Operation.NotIn, new string[] { "Frank Myers", "Joe Austin" })
		}
	);

The `Operation.All` only works at the `dynamic` expression tree to simply the composition of the statement. Passing a list of `QueryField` in the `QueryGroup` object will do the same when calling it explicitly.

Any
---

.. highlight:: c#

Part of the expression tree used to determine whether `any` of the field values satisfied the criteria.

Dynamic way:

::

	var result = connection.Query<Customer>(new
	{
		Name = new
		{
			Operation = Operation.Any, // Works as OR
			Value = new object[]
			{
				new { Operation = Operation.Like, Value = "Anna%" },
				new { Operation = Operation.Equal, Value = "Tom Hawks" },
				new { Operation = Operation.In, Value = new string[] { "Frank Myers", "Joe Austin" } }
			}
		}
	});

Expression way:

::

	// Same operations (Equal)
	var result = connection.Query<Customer>(c => (new [] { "Anna", "Tom Hawks", "Frank Myers", "Joe Austin" }).Any(c.Name));

	// Different operations (Equal, Like, NotEqual, GreaterThan, etc)
	var result = connection.Query<Customer>(c => c.Name.Contains("Anna") || c.Name == "Tom Hawks" || (new [] { "Frank Myers", "Joe Austin" }).Contains(c.Name)); // Supported at version above 1.7.0-beta1

Explicit way:

::

	var result = connection.Query<Customer>
	(
		new QueryField[]
		{
			new QueryField("Name", Operation.Like, "Anna%"),
			new QueryField("Name", Operation.Equal, "Tom Hawks"),
			new QueryField("Name", Operation.In, new string[] { "Frank Myers", "Joe Austin" })
		},
		null, // List of QueryGroups
		Conjunction.Or
	);

The `Operation.Any` only works at the `dynamic` expression tree to simply the composition of the statement. Passing a list of `QueryField` in the `QueryGroup` object will do the same when calling it explicitly.