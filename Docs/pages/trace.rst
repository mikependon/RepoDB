Working with Trace
==================

.. highlight:: c#

One of the unique built-in feature of the library is `Tracing`. It allows developers to do `Debugging` or `Tracing` on the operations while executing it against the database.

With `Tracing`, the developers can able to create its own `Trace` object and inject in the repository.

ITrace Interface
----------------

This interface is used to mark the custom class to become a qualified `Trace` object. It resides from `RepoDb.Interfaces` namespace.

The `ITrace` interface has the following methods.

- **AfterBatchQuery**: called after the `Repository.BatchQuery` operation has been executed.
- **AfterBulkInsert**: called after the `Repository.BulkInsert` operation has been executed.
- **AfterCount**: called after the `Repository.Count` operation has been executed.
- **AfterDelete**: called after the `Repository.Delete` operation has been executed.
- **AfterDeleteAll**: called after the `Repository.DeleteAll` operation has been executed.
- **AfterExecuteNonQuery**: called after the `Repository.ExecuteNonQuery` operation has been executed.
- **AfterExecuteQuery**: called after the `Repository.ExecuteQuery` operation has been executed.
- **AfterExecuteReader**: called after the `Repository.ExecuteReader` operation has been executed.
- **AfterExecuteScalar**: called after the `Repository.ExecuteScalar` operation has been executed.
- **AfterInlineInsert**: called after the `Repository.InlineInsert` operation has been executed.
- **AfterInlineMerge**: called after the `Repository.InlineMerge` operation has been executed.
- **AfterInlineUpdate**: called after the `Repository.InlineUpdate` operation has been executed.
- **AfterInsert**: called after the `Repository.Insert` operation has been executed.
- **AfterMerge**: called after the `Repository.Merge` operation has been executed.
- **AfterQuery**: called after the `Repository.Query` operation has been executed.
- **AfterTruncate**: called after the `Repository.Truncate` operation has been executed.
- **AfterUpdate**: called after the `Repository.Update` operation has been executed.
 
Note: All trace methods mentioned above accepts the parameter named `log` of type `RepoDb.TraceLog`.
 
- **BeforeBatchQuery**: called before the `Repository.BatchQuery` actual execution.
- **BeforeBulkInsert**: called before the `Repository.BulkInsert` actual execution.
- **BeforeCount**: called before the `Repository.Count` actual execution.
- **BeforeDelete**: called before the `Repository.Delete` actual execution.
- **BeforeDeleteAll**: called before the `Repository.DeleteAll` actual execution.
- **BeforeExecuteNonQuery**: called before the `Repository.ExecuteNonQuery` actual execution.
- **BeforeExecuteQuery**: called before the `Repository.ExecuteQuery` actual execution.
- **BeforeExecuteReader**: called before the `Repository.ExecuteReader` actual execution.
- **BeforeExecuteScalar**: called before the `Repository.ExecuteScalar` actual execution.
- **BeforeInlineInsert**: called before the `Repository.InlineInsert` actual execution.
- **BeforeInlineMerge**: called before the `Repository.InlineMerge` actual execution.
- **BeforeInlineUpdate**: called before the `Repository.InlineUpdate` actual execution.
- **BeforeInsert**: called before the `Repository.Insert` actual execution.
- **BeforeMerge**: called before the `Repository.Merge` actual execution.
- **BeforeQuery**: called before the `Repository.Query` actual execution.
- **BeforeTruncate**: called before the `Repository.Truncate` actual execution.
- **BeforeUpdate**: called before the `Repository.Update` actual execution.
 
Note: All trace methods mentioned above accepts the parameter named `log` of type `RepoDb.CancellableTraceLog`.

TraceLog Entry
--------------

This object is the one that holds the value of the repository operations if the `Tracing` is enabled. It is located at `RepoDb` namespace.

Below are the properties of the `TraceLog` object.

- **Method**: a `System.Reflection.MethodBase` object that holds the pointer to the actual method that triggers the execution of the operation.
- **Result**: an object that holds the result of the execution.
- **Parameter**: an object that defines the parameters used when executing the operation.
- **Statement**: the actual query statement used in the execution.
- **ExecutionTime**: a `System.Timespan` object that holds the time length of actual execution.

CancellableTraceLog Entry
------------------------------

This object is a deriving object from the `TraceLog` object, the only different is that, this object is being extended to support the cancellation of the executing operation. It is located at `RepoDb` namespace.

Below are the properties of `CancellableTraceLog` object.

- **IsCancelled**: a property used to identify whether the operation is canceled.
- **IsThrowException**: a property used to identify whether an exception is thrown after cancelation. Exception being thrown is of type `RepoDb.Exceptions.CancelledExecutionException`.

In the screenshot below, you can see a highlighted query expression.
	
.. image:: ../images/trace_code.png

If the trace is enabled, it would create a statement in the background. This statement can be modified during debugging.
	
.. image:: ../images/trace_watch_statement.png

And also, the parameters can be modified as well.
	
.. image:: ../images/trace_watch_parameter.png

Creating a Custom Trace Object
------------------------------
 
.. highlight:: c#

Below is a sample customized `Trace` object.

::

	public class NorthwindDatabaseTrace : ITrace
	{
		public void BeforeBatchQuery(CancellableTraceLog log)
		{
			throw new NotImplementedException();
		}

		public void AfterBatchQuery(TraceLog log)
		{
			throw new NotImplementedException();
		}

		public void BeforeBulkInsert(CancellableTraceLog log)
		{
			throw new NotImplementedException();
		}

		public void AfterBulkInsert(TraceLog log)
		{
			throw new NotImplementedException();
		}

		...
	}

Below is the way on how to inject a Trace class in the repository.

::

	var trace = new NorthwindDatabaseTrace();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", trace);

Once the customized Trace object has been injected, a breakpoint can be placed in any of the methods of the custom Trace class, it is debug-gable once the debugger hits the breakpoint.

Cancelling an Operation
-----------------------

To cancel an operation, simply call the `Cancel` method of type `RepoDb.CancelableTraceLog` in any `Before` operation.

Below is a sample code that calls the `Cancel` method of the `BeforeQuery` operation if any of the specified keywords from the variable named `keywords` is found from the statement.

.. highlight:: c#

::

	public void BeforeQuery(CancellableTraceLog log)
	{
		var keywords = new[] { "INSERT", "DELETE", "UPDATE", "DROP", "MERGE", "ALTER" };
		if (keywords.Any(keyword => log.Statement.Contains(keyword)))
		{
			Console.WriteLine("A suspicious statement has been injected on the Query operations.");
			log.Cancel(true);
		}
	}

By passing the value of `true` in the parameter when calling the `Cancel` method, it would signal the library to throw an `RepoDb.Exceptions.CancelledExecutionException` exception object back to the caller.