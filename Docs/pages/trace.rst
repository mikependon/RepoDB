Trace
=====

Allow the developers to do `Debugging` or `Tracing` on the operations while executing it against the database

Custom Trace
------------

Below is a sample customized `Trace` object.
 
.. code-block:: c#
	:linenos:

	public class NorthwindDatabaseTrace : ITrace
	{
		...
	}

Below is the way on how to inject a `Trace` class in the connection.

.. code-block:: c#
	:linenos:

	var trace = new NorthwindDatabaseTrace();
	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Query<Order>(o => o.CustomerId == 10045, trace: trace);
	}

Below is the way on how to inject a `Trace` class in the repository.

.. code-block:: c#
	:linenos:

	var trace = new NorthwindDatabaseTrace();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", trace);

A breakpoint can be placed in any of the methods of the custom `Trace` class, the debugger will hit the breakpoint once the operation has been called.

Cancellation
------------

To cancel an operation, simply call the `Cancel` method of type `CancelableTraceLog` in any `Before` operation.

.. code-block:: c#
	:linenos:

	public void BeforeQuery(CancellableTraceLog log)
	{
		var filteredKeywords = new[] { "DROP", "TRUNCATE", "CREATE", "ALTER" };
		if (filteredKeywords.Any(keyword => log.Statement.Contains(keyword)))
		{
			Console.WriteLine("A suspicious statement has been injected on the Query operations.");
			log.Cancel(true);
		}
	}

By passing the value of `true` in the parameter when calling the `Cancel` method, it would signal the library to throw an `Exceptions.CancelledExecutionException` exception object back to the caller.