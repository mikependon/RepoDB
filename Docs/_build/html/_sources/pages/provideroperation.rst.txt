Provider Operation
==================

A feature which allow the developers to override the DB Provider specific operations (i.e: `BulkInsert`).

**How is it being used?**

Some operations of the `Repositories` or extended methods of the `DbConnection` object is using a DB Provider specific functionality. Bulk-insert is one of the operation
that is widely used for SQL Server, Oracle, MySql (and any other DB Provider that supports Bulk-Insert), but it is not commonly used in `OLE DB` providers. By overriding,
the default implementation of the library, a developer can write and customize its own way of implementing this.

**Why BulkInsert is a DB Provider specific feature?**

Considering `ADO.NET`, by default, it has included the `SqlBulkCopy` class that allow developers to implement bulk-operation for SQL Server databases. The library is also
using the `SqlBulkCopy` class to support the SQL Server bulk-insert operation.

For other provider to get supported, a specialized `<Provider>BulkCopy` class must be implemented that uses the proper way of doing the bulk-operations.

DbOperationProviderMapper
-------------------------

This class is used to map the `Type` of database provider into an instance of `IDbOperationProvider` object.

By default, the `SqlDbOperationProvider` class is provided by the library which is mainly used for SQL Server DB providers.

A code below is called in the static constructor of this class.

.. highlight:: c#

::

	static DbOperationProviderMapper()
	{
		// Default for SqlConnection
		Add(typeof(SqlConnection), new SqlDbOperationProvider());
	}

Notice, this class itself has defaulted the `SqlConnection` mappings into `SqlDbOperationProvider` object.

A code below is a simple call to map a customized `IDbOperationProvider` class named `OracleDbOperationProvider` into an `Oracle` DB provider.

::

	DbOperationProviderMapper.Add(typeof(OracleConnection), new OracleDbOperationProvider(), true);

The last `boolean` argument is used to override an existing mapping (if present). Otherwise, an exception will be thrown.

IDbOperationProvider
--------------------

An interface used to mark the class to become a database operation provider. Below is a sample code that implements this interface.

.. highlight:: c#

::

	public class OracleDbOperationProvider : IDbOperationProvider
	{
		public int BulkInsert<TEntity>(IDbConnection connection,
			IEnumerable<TEntity> entities,
			IEnumerable<BulkInsertMapItem> mappings = null,
			OracleBulkCopyOptions options = OracleBulkCopyOptions.Default,
			int? bulkCopyTimeout = null,
			int? batchSize = null,
			IDbTransaction transaction = null)
			where TEntity : class
		{
			...
		}

		public int BulkInsert<TEntity>(IDbConnection connection,
			DbDataReader reader,
			IEnumerable<BulkInsertMapItem> mappings = null,
			OracleBulkCopyOptions options = OracleBulkCopyOptions.Default,
			int? bulkCopyTimeout = null,
			int? batchSize = null,
			IDbTransaction transaction = null)
			where TEntity : class
		{
			...
		}

		public int BulkInsert(IDbConnection connection,
			string tableName,
			DbDataReader reader,
			IEnumerable<BulkInsertMapItem> mappings = null,
			OracleBulkCopyOptions options = OracleBulkCopyOptions.Default,
			int? bulkCopyTimeout = null,
			int? batchSize = null,
			IDbTransaction transaction = null)
		{
			...
		}
	}

Once the class `OracleDbOperationProvider` has been mapped to Oracle DB Provider, then the library will automatically use it in Oracle bulk-insert operations.

SqlDbOperationProvider
----------------------

This class is used by the `Repositories` and other extended methods of the `DbConnection` object to execute a DB Provider specific operations. As of today, the
current identified provider specific operation is the `BulkInsert` operation. By default, the library has mapped this class into a `SqlConnection` DB provider.

Below is the implementation of this class.

.. highlight:: c#

::

	public class SqlDbOperationProvider : IDbOperationProvider
	{
		// Sync

		public int BulkInsert<TEntity>(IDbConnection connection,
			DbDataReader reader,
			IEnumerable<BulkInsertMapItem> mappings = null,
			SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
			int? bulkCopyTimeout = null,
			int? batchSize = null,
			IDbTransaction transaction = null)
			where TEntity : class
		{
			...
		}

		// Async

		public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection,
			DbDataReader reader,
			IEnumerable<BulkInsertMapItem> mappings = null,
			SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
			int? bulkCopyTimeout = null,
			int? batchSize = null,
			IDbTransaction transaction = null)
			where TEntity : class
		{
			...
		}
	}

Click `here <https://github.com/mikependon/RepoDb/blob/master/RepoDb/RepoDb/DbOperationProviders/SqlServer/BulkInsert.cs>`_ to see the actual class implementation for SQL Server `BulkInsert` operation.