Raw SQL
=======

Raw SQL is a feature which ables you to execute a raw SQL statements directly into the database.

The following are the execute methods which can be accessed via `DbConnection` extended methods.

* `ExecuteNonQuery <https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery>`_
* `ExecuteQuery <https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery>`_
* `ExecuteReader <https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader>`_
* `ExecuteScalar <https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar>`_

Below is a sample call which uses a **Dynamic** object as a parameter.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName";
		var param = new
		{
			FirstName = "John",
			LastName = "Doe"
		};

		// Execute the SQL
		var result = connection.ExecuteNonQuery(commandText, param);
	}

Executing a raw SQL statements support different kind of parameters.

Via **ExpandoObject** (as **Dynamic** Object).

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName";
		var param = (dynamic)new ExpandoObject();

		// Set each parameter
		param.FirstName = "John";
		param.LastName = "Doe";

		// Execute the SQL
		var result = connection.ExecuteNonQuery(commandText, param);
	}

Via **ExpandoObject** as **Dictionary<string, object>**.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName";
		var param = new ExpandoObject() as IDictionary<string, object>;

		// Set each parameter
		param.Add("FirstName", "John");
		param.Add("LastName", "Doe");

		// Execute the SQL
		var result = connection.ExecuteNonQuery(commandText, param);
	}

Via **Dictionary<string, object>**.

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		// Set the values
		var commandText = "SELECT * FROM [dbo].[Person] WHERE FirstName = @FirstName AND LastName = @LastName";
		var param = new Dictionary<string, object>
		{
			{ "FirstName", "John" },
			{ "LastName", "Doe" }
		};
		
		// Execute the SQL
		var result = connection.ExecuteNonQuery(commandText, param);
	}