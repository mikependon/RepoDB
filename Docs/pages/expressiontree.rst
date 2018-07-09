Expression Tree
===============

.. highlight:: c#

The expression tree is the brain of the library. It defines the best possible way of doing a `WHERE` expression (SQL Statement) by composing it via `dynamic` or `RepoDb.QueryGroup` objects.

Objects used for composing the expression tree.

- **QueryGroup**: used to group an expression.
- **QueryField**: it holds the `Field` and `Parameter` objects to be used on the query expressions.
- **Conjunction**: an enumeration that holds the value whether the expression is on `And` or `Or` operation.
- **Operation**: an enumeration that holds the value what kind of operation is going to be executed on certain expression. It holds the value of like `Equal`, `NotEqual`, `Between`, `GreaterThan` and etc.

Certain repository operations are using the expression trees.

- Repository.BatchQuery
- Repository.Count
- Repository.Delete
- Repository.ExecuteNonQuery
- Repository.ExecuteQuery
- Repository.ExecuteScalar
- Repository.InlineInsert
- Repository.InlineMerge
- Repository.InlineUpdate
- Repository.Query
- Repository.Update

Certain connection extension methods are using the expression trees.

- DbConnection.BatchQuery
- DbConnection.Count
- DbConnection.Delete
- DbConnection.ExecuteNonQuery
- DbConnection.ExecuteQuery
- DbConnection.ExecuteReader
- DbConnection.ExecuteScalar
- DbConnection.InlineInsert
- DbConnection.InlineMerge
- DbConnection.InlineUpdate
- DbConnection.Query
- DbConnection.Update

There are two ways of building the expression trees, the explicit way by using `QueryGroup` objects and dynamic way by using `dynamic` objects.

Explicit Expression
-------------------

An explicit query expression are using the defined objects `RepoDb.QueryGroup`, `RepoDb.QueryField`, `RepoDb.Enumerations.Conjunction` and `RepoDb.Enumerations.Operation` when composing the expression.

Below is a pseudo code of explicit query expression.

::

	// WHERE (Field1 = @Field1 AND Field2 = @Field2) AND ((Field3 = @Field3 OR Field4 = @Field4) AND (Field5 = @Field5 OR Field6 = @Field6));
	var tree = new QueryGroup
	(
		new QueryField[]
		{
			// List of QueryFields
		},
		new QueryGroup[]
		{
			// List of QueryGroups
			new QueryGroup
			(
				new QueryField[]
				{
					// List of QueryFields
				},
				new QueryGroup[]
				{
					// List of QueryGroups
					...
					...
					...
				}
				Conjunction.Or
			),
			new QueryGroup
			(
				new QueryField[]
				{
					// List of QueryFields
				},
				new QueryGroup[]
				{
					// List of QueryGroups
					...
					...
					...
				}
				Conjunction.Or
			)
		},
		Conjunction.And
	);

Actual explicit query tree expression.

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

Dynamic Expression
------------------

.. highlight:: c#

A dynamic query expression is using a single dynamic object when composing the expression.

Below is a pseudo code of dynamic query expression.

::

	var tree = new
	{
		Conjunction = Conjunction.And,
		Field1 = "Field1", // QueryField
		Field2 = "Field2", // QueryField
		QueryGroups = new []
		{
			new
			{
				Conjunction = Conjunction.Or,
				Field3 = "Field3", // QueryField
				Field4 = "Field4",
				QueryGroups = new object[]
				{
					...
				}
			},
			new
			{
				Conjunction = Conjunction.Or,
				Field3 = "Field3", // QueryField
				Field4 = "Field4",
				QueryGroups = new object[]
				{
					...
				}
			}
		}
	};

Actual dynamic query tree expression.

::

	// Last 3 months customer with CustomerId >= 10045
	var query = new
	{
		CustomerId = new { Operation = 10045 },
		CreatedDate = new { Operation = Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3) }
	};