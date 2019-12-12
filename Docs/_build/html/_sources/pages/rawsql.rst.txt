Raw SQL
=======

Raw SQL is a feature which ables you to execute a raw SQL statements directly into the database.

The following are the execute methods which can be accessed via `DbConnection` extended methods.

* `ExecuteNonQuery <https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery>`_
* `ExecuteQuery <https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery>`_
* `ExecuteQueryMultiple <https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple>`_
* `ExecuteReader <https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader>`_
* `ExecuteScalar <https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar>`_

Below is a sample call which uses a **Dynamic** object as a parameter.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName;";
		var param = new
		{
			FirstName = "John",
			LastName = "Doe"
		};

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Parameters
----------

Executing a raw SQL statements support different kind of parameters.

Via **ExpandoObject** (as **Dynamic** Object).

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName;";
		var param = (dynamic)new ExpandoObject();

		// Set each parameter
		param.FirstName = "John";
		param.LastName = "Doe";

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via **ExpandoObject** as **Dictionary<string, object>**.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName;";
		var param = new ExpandoObject() as IDictionary<string, object>;

		// Set each parameter
		param.Add("FirstName", "John");
		param.Add("LastName", "Doe");

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via **Dictionary<string, object>**.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName;";
		var param = new Dictionary<string, object>
		{
			{ "FirstName", "John" },
			{ "LastName", "Doe" }
		};
		
		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via explicit **QueryGroup** or **QueryField** or **IEnumerable<QueryField>**.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName;";
		var param = new QueryGroup(new []
		{
			new QueryField("FirstName", "John"),
			new QueryField("LastName", "Doe")
		});
		
		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Array Values
------------

An array values can also be passed a part of the execution.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE Id IN (@Keys);";
		var param = new { Keys = new [] { 10045, 10102, 11004 }};

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via **ExpandoObject** (as **Dynamic** Object).

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE Id IN (@Keys);";
		var param = (dynamic)new ExpandoObject();

		// Set each parameter
		param.Keys = new [] { 10045, 10102, 11004 };

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via **ExpandoObject** as **Dictionary<string, object>**.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE Id IN (@Keys);";
		var param = new ExpandoObject() as IDictionary<string, object>;

		// Set each parameter
		param.Add("Keys", new [] { 10045, 10102, 11004 });

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via **Dictionary<string, object>**.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE Id IN (@Keys);";
		var param = new Dictionary<string, object>
		{
			{ "Keys", new [] { 10045, 10102, 11004 } },
		};

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}

Via explicit **QueryGroup** or **QueryField** or **IEnumerable<QueryField>**.

.. code-block:: c#
	:linenos:

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE Id IN (@Keys);";
		var param = new QueryGroup(new QueryField("Keys", new [] { 10045, 10102, 11004 })),

		// Execute the SQL
		var result = connection.ExecuteQuery<Person>(commandText, param);
	}
