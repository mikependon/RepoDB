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

Expression way:

::

	// Contains (LIKE '%VAL%')
	var result = connection.Query<Customer>(c => c.Name.Contains("Anna"));
	
	// Contains (LIKE 'VAL%')
	var result = connection.Query<Customer>(c => c.Name.StartsWith("Anna"));

	// Contains (LIKE '%VAL')
	var result = connection.Query<Customer>(c => c.Name.EndsWith("Anna"));

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Name", Operation.Like, "Anna%" });

NotLike
-------

.. highlight:: c#

Part of the expression tree used to determine whether the field is `not identitical` to a given value. An opposite of `Operation.Like`.

Expression way:

::

	// Contains (LIKE '%VAL%')
	var result = connection.Query<Customer>(c => !c.Name.Contains("Anna"));
	
	// Contains (LIKE 'VAL%')
	var result = connection.Query<Customer>(c => !c.Name.StartsWith("Anna"));

	// Contains (LIKE '%VAL')
	var result = connection.Query<Customer>(c => !c.Name.EndsWith("Anna"));

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Name", Operation.NotLike, "Anna%" });

Between
-------

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `between` 2 given values.

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

Expression way:

::

	var result = connection.Query<Customer>(c => (new [] { 10045, 10046, 10047, 10048 }).Contains(c.Id));

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.In, new [] { 10045, 10046, 10047, 10048 } });

NotIn
-----

.. highlight:: c#

Part of the expression tree used to determine whether the field value is `not in` given values. An opposite of `Operation.In`. See sample below.

Expression way:

::

	var result = connection.Query<Customer>(c => !(new [] { 10045, 10046, 10047, 10048 }).Contains(c.Id));

Explicit way:

::

	var result = connection.Query<Customer>(new QueryField("Id", Operation.NotIn, new [] { 10045, 10046, 10047, 10048 } });