Query Group Expressions
=======================

.. highlight:: c#

A query group object is used to group an expression when composing a tree expressions. It is equivalent to a grouping on a `WHERE` statement in SQL Statements.

Below are the constructor parameters.

- **queryFields**: the list of `QueryField` objects to be included in the expression composition. It stands as `[FieldName] = @FiedName` when it comes to SQL Statement compositions.
- **queryGroups**: the list of child `QueryGroup` objects to be included in the expresson composition. It stands as the `([FieldName] = @FieldName AND [FieldName1] = @FieldName1)` when it comes to SQL Statement compositions.
- **conjunction**: the conjuction to be used when grouping the fields. It stands as the `AND` or `OR` in the SQL Statement compositions.

As mentioned above, below is a sample code to create a query group object.

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

Query Operations
----------------

.. highlight:: c#

The query operation defines the operation to be used by the query expression (field level) during the actual execution. It is located at `RepoDb.Enumerations` namespace.

List of Operations:

- Operation.Equal
- Operation.NotEqual
- Operation.LessThan
- Operation.LessThanOrEqual
- Operation.GreaterThan
- Operation.GreaterThanOrEqual
- Operation.Like
- Operation.NotLike
- Operation.Between
- Operation.NotBetween
- Operation.In
- Operation.NotIn
- Operation.All
- Operation.Any

Operation.Equal
---------------

.. highlight:: c#

Part of the expression tree used to determine the `equality` of the field and data.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = 10045 });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Id = new { Operation = Operation.Equal, Value = 10045 }
	});

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.Equal, 10045 ));

Operation.NotEqual
------------------

.. highlight:: c#

Part of the expression tree used to determine the `inequality` of the field and data.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Name = new { Operation = Operation.NotEqual, Value = "Anna Fullerton" }
	});

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Name", Operation.NotEqual, "Anna Fullerton" });

Operation.LessThan
------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `less than` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.LessThan, Value = 100 } });


Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.LessThan, 100 });

Operation.GreaterThan
---------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `greater than` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.GreaterThan, Value = 0 } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.GreaterThan, 0 });

Operation.LessThanOrEqual
-------------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `less than or equal` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.LessThanOrEqual, Value = 100 } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>.Query(new QueryField("Id", Operation.LessThanOrEqual, 100 });

Operation.GreaterThanOrEqual
----------------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `greater than or equal` of the defined value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.GreaterThanOrEqual, Value = 0 } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.GreaterThanOrEqual, 0 });

Operation.Like
--------------

.. highlight:: c#

Part of the expression tree used to determine whether the field is `identitical` to a given value.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Name = new { Operation = Operation.Like, Value = "Anna%" } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Name", Operation.Like, "Anna%" });

Operation.NotLike
-----------------

.. highlight:: c#

Part of the expression tree used to determine whether the field is `not identitical` to a given value. An opposite of `Operation.Like`.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Name = new { Operation = Operation.NotLike, Value = "Anna%" } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Name", Operation.NotLike, "Anna%" });

Operation.Between
-----------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `between` 2 given values.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { CreatedDate = new { Operation = Operation.Between, Value = new [] { Date1, Date2 } } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.Between, Value = new [] { 10045, 10075 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("CreatedDate", Operation.Between, new [] { Date1, Date2 } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.Between, new [] { 10045, 10075 } });

Operation.NotBetween
--------------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not between` 2 given values. An opposite of `Operation.Between`.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { CreatedDate = new { Operation = Operation.NotBetween, Value = new [] { Date1, Date2 } } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.NotBetween, Value = new [] { 10045, 10075 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("CreatedDate", Operation.NotBetween, new [] { Date1, Date2 } });

or

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.NotBetween, new [] { 10045, 10075 } });

Operation.In
------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `in` given values.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.In, Value = new [] { 10045, 10046, 10047, 10048 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.In, new [] { 10045, 10046, 10047, 10048 } });

Operation.NotIn
---------------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not in` given values. An opposite of `Operation.In`. See sample below.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new { Id = new { Operation = Operation.NotIn, Value = new [] { 10045, 10046, 10047, 10048 } } });

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new QueryField("Id", Operation.NotIn, new [] { 10045, 10046, 10047, 10048 } });

Operation.All
-------------

.. highlight:: c#

Part of the expression tree used to determine whether `all` the field values satisfied the criteria.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
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


Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>
	(
		new QueryField[]
		{
			new QueryField("Name", Operation.Like, "Anna%"),
			new QueryField("Name", Operation.NotEqual, "Tom Hawks"),
			new QueryField("Name", Operation.NotIn, new string[] { "Frank Myers", "Joe Austin" })
		}
	);

The `Operation.All` only works at the `dynamic` expression tree to simply the composition of the statement. Passing a list of `QueryField` in the `QueryGroup` object will do the same when calling it explicitly.

Operation.Any
-------------

.. highlight:: c#

Part of the expression tree used to determine whether `any` of the field values satisfied the criteria.

Dynamic way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>(new
	{
		Name = new
		{
			Operation = Operation.Any, // Works as OR
			Value = new object[]
			{
				new { Operation = Operation.Like, Value = "Anna%" },
				new { Operation = Operation.NotEqual, Value = "Tom Hawks" },
				new { Operation = Operation.In, Value = new string[] { "Frank Myers", "Joe Austin" } }
			}
		}
	});

Explicit way:

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var result = repository.Query<Customer>
	(
		new QueryField[]
		{
			new QueryField("Name", Operation.Like, "Anna%"),
			new QueryField("Name", Operation.NotEqual, "Tom Hawks"),
			new QueryField("Name", Operation.In, new string[] { "Frank Myers", "Joe Austin" })
		},
		null, // List of QueryGroups
		Conjunction.Or
	);

The `Operation.Any` only works at the `dynamic` expression tree to simply the composition of the statement. Passing a list of `QueryField` in the `QueryGroup` object will do the same when calling it explicitly.