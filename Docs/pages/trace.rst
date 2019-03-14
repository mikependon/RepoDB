Trace
=====

Allow the developers to do `Debugging` or `Tracing` on the operations while executing it against the database

ITrace
------

Is an interface used to create a custom `Trace` class object.

TraceLog
--------

An object that holds the value of the operations if the `Tracing` is enabled.

CancellableTraceLog
-------------------

An object that holds the value of the operations with extended members to support the cancellation of the operations.

Custom Trace
------------

Below is a sample customized `Trace` object.
 
.. highlight:: c#

::

	public class NorthwindDatabaseTrace : ITrace
	{
		...
	}

Below is the way on how to inject a Trace class in the repository.

::

	var trace = new NorthwindDatabaseTrace();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", trace);

A breakpoint can be placed in any of the methods of the custom `Trace` class, the debugger will hit the breakpoint once the operation has been called.

Cancel
------

To cancel an operation, simply call the `Cancel` method of type `CancelableTraceLog` in any `Before` operation.

.. highlight:: c#

::

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