Expression Tree
===============

Expression tree is used to define an expression when calling the connection operations.

Explicit
--------

An explicit (or often known as object-based) query expression are using the defined objects:

- RepoDb.QueryGroup
- RepoDb.QueryField
- RepoDb.Enumerations.Conjunction
- RepoDb.Enumerations.Operation

.. highlight:: c#

Below is a pseudo code of an explicit query expression.

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
			new QueryField("Id", Operation.GreaterThanOrEqual, 10045),
			new QueryField("CreatedDate", Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3))
		},
		null, // Child QueryGroups
		Conjunction.And
	);

Dynamic
-------

A dynamic (or often known as dynamic-based) query expression is using a single dynamic object when composing the expression.

Below is a pseudo code of a dynamic query expression.

.. highlight:: c#

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
		Id = new { Operation = Operation.GreaterThanOrEqual, Value = 10045 },
		CreatedDate = new { Operation = Operation.GreaterThanOrEqual, DateTime.UtcNow.Date.AddMonths(-3) }
	};

Linq
----

A Linq query expression (or often known as expression-based) is used as a function-based-expression to construct an expression. It requires a data entity type to compose an expression.

For the pseudo codes, please refer to Microsoft `documentation <https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/>`_.

Actual explicit query tree expression.

::

	// Last 3 months customer with CustomerId >= 10045
	<Customer>(c => c.Id >= 10045 && CreatedDate >= DateTime.UtcNow.Date.AddMonths(-3))



