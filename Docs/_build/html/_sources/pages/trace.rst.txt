Working with Trace
==================

.. highlight:: c#

One of the unique built-in feature of the library is tracing. It allows developers to do debugging or tracing on the operations while executing it against the database.

With tracing, the developers can able to create its own `Trace` object and inject in the repository.

ITrace Interface
----------------

This interface is the heart of library's tracing feature. It resides from `RepoDb.Interfaces` namespace. This interface is required to be implemented at the custom trace classes to enable the tracing, then, the custom class must be injected in the repository.

The `ITrace` interface has the follow trace methods.

- **AfterBatchQuery**: called after the `Repository.BatchQuery` operation has been executed.
- **AfterBulkInsert**: called after the `Repository.BulkInsert` operation has been executed.
- **AfterCount**: called after the `Repository.Count` operation has been executed.
- **AfterCountBig**: called after the `Repository.CountBig` operation has been executed.
- **AfterDelete**: called after the `Repository.Delete` operation has been executed.
- **AfterExecuteNonQuery**: called after the `Repository.ExecuteNonQuery` operation has been executed.
- **AfterExecuteQuery**: called after the `Repository.ExecuteQuery` operation has been executed.
- **AfterExecuteReader**: called after the `Repository.ExecuteReader` operation has been executed.
- **AfterExecuteScalar**: called after the `Repository.ExecuteScalar` operation has been executed.
- **AfterInlineUpdate**: called after the `Repository.InlineUpdate` operation has been executed.
- **AfterInsert**: called after the `Repository.Insert` operation has been executed.
- **AfterMerge**: called after the `Repository.Merge` operation has been executed.
- **AfterQuery**: called after the `Repository.Query` operation has been executed.
- **AfterUpdate**: called after the `Repository.Update` operation has been executed.
 
Note: All trace methods mentioned above accepts the parameter named `log` of type `RepoDb.Interfaces.ITraceLog`.
 
- **BeforeBatchQuery**: called before the `Repository.BatchQuery` actual execution.
- **BeforeBulkInsert**: called before the `Repository.BulkInsert` actual execution.
- **BeforeCount**: called before the `Repository.Count` actual execution.
- **BeforeCountBig**: called before the `Repository.CountBig` actual execution.
- **BeforeDelete**: called before the `Repository.Delete` actual execution.
- **BeforeExecuteNonQuery**: called before the `Repository.ExecuteNonQuery` actual execution.
- **BeforeExecuteQuery**: called before the `Repository.ExecuteQuery` actual execution.
- **BeforeExecuteReader**: called before the `Repository.ExecuteReader` actual execution.
- **BeforeExecuteScalar**: called before the `Repository.ExecuteScalar` actual execution.
- **BeforeInlineUpdate**: called before the `Repository.InlineUpdate` actual execution.
- **BeforeInsert**: called before the `Repository.Insert` actual execution.
- **BeforeMerge**: called before the `Repository.Merge` actual execution.
- **BeforeQuery**: called before the `Repository.Query` actual execution.
- **BeforeUpdate**: called before the `Repository.Update` actual execution.
 
Note: All trace methods mentioned above accepts the parameter named `log` of type `RepoDb.Interfaces.ICancellableTraceLog`.

ITraceLog Interface
-------------------

This interface and deriving objects are used by the `RepoDb.Interfaces.ITrace` object as a method argument during the `After` operations.

Below are the properties of `ITraceLog` object.

- **Method**: a `System.Reflection.MethodBase` object that holds the pointer to the actual method that triggers the execution of the operation.
- **Result**: an object that holds the result of the execution.
- **Parameter**: an object that defines the parameters used when executing the operation.
- **Statement**: the actual query statement used in the execution.
- **ExecutionTime**: a `System.Timespan` object that holds the time length of actual execution.

ICancellableTraceLog Interface
------------------------------

This interface and deriving objects are used by the `RepoDb.Interfaces.ITrace` object as a method argument at the `Before` operations. It inherits the `RepoDb.Interfaces.ITrace` interface.

Below are the properties of `ICancellableTraceLog` object.

- **IsCancelled**: a property used to identify whether the operation is canceled.
- **IsThrowException**: a property used to identify whether an exception is thrown after cancelation. Exception being thrown is of type `RepoDb.Exceptions.CancelledExecutionException`.

Creating a Custom Trace Object
------------------------------
 
.. highlight:: c#

Below is a sample customized `Trace` object.

::

	public class NorthwindDatabaseTrace : ITrace
	{
		public void BeforeBatchQuery(ICancellableTraceLog log)
		{
			throw new NotImplementedException();
		}

		public void AfterBatchQuery(ITraceLog log)
		{
			throw new NotImplementedException();
		}

		public void BeforeBulkInsert(ICancellableTraceLog log)
		{
			throw new NotImplementedException();
		}

		public void AfterBulkInsert(ITraceLog log)
		{
			throw new NotImplementedException();
		}

		...
	}

Below is the way on how to inject a Trace class in the repository.

::

	var trace = new NorthwindDatabaseTrace();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"
		0, // commandTimeout
		null, // cache
		trace, // trace
		null, // statementBuilder
	);

Once the customized Trace object has been injected, a breakpoint can be placed in any of the methods of the custom Trace class, it is debug-gable once the debugger hits the breakpoint.

Canceling an Operation
----------------------

To cancel an operation, simply call the method named `Cancel` of type `RepoDb.Interfaces.ICancelableTraceLog` in any `Before` operation.

Below is a sample code that calls the `Cancel` method of the `BeforeQuery` operation if any of the specified keywords from the variable named `keywords` is found from the statement.

.. highlight:: c#

::

	public void BeforeQuery(ICancellableTraceLog log)
	{
		var keywords = new[] { "INSERT", "DELETE", "UPDATE", "DROP", "MERGE", "ALTER" };
		if (keywords.Any(keyword => log.Statement.Contains(keyword)))
		{
			Console.WriteLine("A suspicious statement has been injected on the Query operations.");
			log.Cancel(true);
		}
	}

By passing the value of `true` in the parameter when calling the `Cancel` method, it would signal the library to throw an `RepoDb.Exception.CancelledExecutionException` exception object back to the caller.