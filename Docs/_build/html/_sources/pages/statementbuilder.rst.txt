StatementBuilder
================

The library supports statement building injection, allowing the developers to override the default query statement the library is using. By default, the library is using the `SqlServerStatementBuilder` that is only working for SQL Server databases.

QueryBuilder
------------

.. highlight:: c#

A query builder is an helper object used when creating a query statement in the statement builders. It contains important methods that is very useful to fluently construct the statement.

Below is a sample code that creates a SQL Statement for the `Query` operation for `Oracle` data provider.

::

	public string CreateQuery(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		QueryGroup where = null,
		IEnumerable<OrderField> orderBy = null,
		int? top = null,
		string hints = null)
	{
		// There should be fields
		if (fields?.Any() != true)
		{
			throw new NullReferenceException(string.Concat("The list of queryable fields must not be null for '", tableName, "'."));
		}

		// Build the query
		queryBuilder
			.Clear()
			.Select()
			.TopFrom(top)
			.FieldsFrom(fields)
			.From()
			.TableNameFrom(tableName);
			
		// Build the query optimizers
		if (hints != null)
		{
			// Write the hints here like: queryBuilder.WriteText(hints);
		}
		
		// Add all fields for WHERE
		if (where != null)
		{
			queryBuilder.WhereFrom(where);
		}

		// Add the ROWNUM (TOP in SQL Server)
		if (top > 0)
		{
			// In Oracle, SELECT [Fields] FROM [Table] WHERE [Fields] AND ROWNUM <=(Rows)
			if (where != null)
			{
				queryBuilder.WriteText(string.Concat("AND (ROWNUM <= ", top, ")"));
			}
			else
			{
				queryBuilder.WriteText(string.Concat("(ROWNUM <= ", top, ")"));
			}
		}

		// Build the filter and ordering
		queryBuilder
			.OrderByFrom(orderBy)
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

Customizing a Builder
---------------------

.. highlight:: c#

The main reason why the library supports the statement builder is to allow the developers to implement or override (the default) statement builder of the DB Provider. By default, the library statement builder is only limited for SQL Server providers (as SQL Statements).

However, it will fail if the library is being used to access the SqLite, PostgreSql, MySql or any other DB Providers.

**Note**: The support for other DB Provider is available as Nuget Package as (MySql => RepoDb.MySql, SqLite => RepoDb.SqLite, PostgreSql => RepoDb.PostgreSql).

To create a custom statement builder, simply create a class and implements the `IStatementBuilder` interface.

::

	public class PostgreSqlStatementBuilder : IStatementBuilder
	{
		// Implements the IStatementBuilder methods here
	}

Once the custom statement builder is created, it then can be used as an injectable object into the repository. See sample below injecting a statement builder for PostgreSql DB Provider.

::

	var statementBuilder = new PostgreSqlStatementBuilder();
	var repository = new DbRepository<NpgsqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", statementBuilder);

With the code snippets above, everytime the repository operation methods is being called, the `PostgreSqlStatementBuilder` corresponding method will be executed.

StatementBuilderMapper
----------------------

.. highlight:: c#

This class is used to map an existing `IStatementBuilder` into a specific DB Provider. The mapper is of static class.

::

	StatementBuilderMapper.Map(typeof(MySqlConnection), new MySqlStatementBuilder());

**Note**: By default, the library has mapped the `SqlServerStatementBuilder` object for SQL Server DB Provider (via `SqlConnection`).

Let say, the developers has created the following statement builder:

 - SqlServerStatementBuilder (for `SqlConnection`)
 - SqLiteStatementBuilder (for `SqLiteConnection`)
 - MySqlStatementBuilder (for `MySqlConnection`)
 - PostgreSqlStatementBuilder (for `NpgsqlConnection`)

Then, in order to utilize the mentioned statement builders, the code below must be called.

::

	StatementBuilderMapper.Map(typeof(OracleConnection), new SqlServerStatementBuilder());
	StatementBuilderMapper.Map(typeof(SqLiteConnection), new SqLiteStatementBuilder());
	StatementBuilderMapper.Map(typeof(MySqlConnection), new MySqlStatementBuilder());
	StatementBuilderMapper.Map(typeof(NpgsqlConnection), new PostgreSqlStatementBuilder());

Injecting to Repository
-----------------------

.. highlight:: c#

To inject the statement builder to repository, simply passed the instance of `IStatementBuilder` object when initializing a repository object.

For DbRepository<TDbConnetion>:

::

	var statementBuider = new MySqlStatementBuilder();
	var repository = new DbRepository<MySqlConnection>(ConnectionString, statementBuilder);

For BaseRepository<TEntity, TDbConnetion>:

::

	public class CustomerRepository : DbRepository<Customer, MySqlConnection>
	{
		public CustomerRepository(string connectionString,
			IStatementBuilder builder)
			: base(connectionString, builder) { }

		...
	}
	var statementBuider = new MySqlStatementBuilder();
	var repository = new CustomerRepository<MySqlConnection>(ConnectionString, statementBuilder);

BaseStatementBuilder
--------------------

.. highlight:: C#

This class is a pre-implemented abstract class that can be used as a base implementation of the `IStatementBuilder`.

This is very useful if the developer would like to implement his/her own DB Provider Statement Builder.

Remember that not all implementations are meant for all DB Providers, so the developer must be aware to `override` certain methods if necessary.

**Example**: The `CreateTruncate()` method is using the `TRUNCATE` keyword when composing a SQL Statement.

::

	public virtual string CreateTruncate(QueryBuilder queryBuilder,
		string tableName)
	{
		// Guard the target table
		GuardTableName(tableName);

		// Initialize the builder
		var builder = queryBuilder ?? new QueryBuilder();

		// Build the query
		builder.Clear()
			.Truncate()
			.Table()
			.TableNameFrom(tableName, DbSetting)
			.End();

		// Return the query
		return builder.GetString();
	}

That code will not work for `SqLite`. The developer must `override` that method if he/she is implementing his/her own statement builder.

::

	public override string CreateTruncate(QueryBuilder queryBuilder,
		string tableName)
	{
		// Ensure with guards
		GuardTableName(tableName);

		// Initialize the builder
		var builder = queryBuilder ?? new QueryBuilder();

		// Build the query
		builder.Clear()
			.Clear()
			.Delete()
			.From()
			.TableNameFrom(tableName, DbSetting)
			.End()
			.WriteText("VACUUM")
			.End();

		// Return the query
		return builder.GetString();
	}

**Note**: The `CreateBatchQuery`, `CreateMerge` and `CreateMergeAll` is being implemented as `abstract` method, whereas the others were implemented as `virtual` methods.

CreateAverage
-------------

.. highlight:: C#

This method is used to compose a SQL Statement for `Average` operation.

::

	public string CreateAverage(QueryBuilder queryBuilder,
        string tableName,
        Field field,
        QueryGroup where = null,
        string hints = null)
	{
		...
	}

CreateAverageAll
----------------

.. highlight:: C#

This method is used to compose a SQL Statement for `AverageAll` operation.

::

	public string CreateAverageAll(QueryBuilder queryBuilder,
        string tableName,
        Field field,
        string hints = null)
	{
		...
	}

CreateBatchQuery
----------------

.. highlight:: C#

This method is used to compose a SQL statement for `BatchQuery` operation.

::

	public string CreateBatchQuery(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		int? page,
		int? rowsPerBatch,
		IEnumerable<OrderField> orderBy = null,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateCount
-----------

.. highlight:: C#

This method is used to compose a SQL statement for `Count` operation.

::

	public string CreateCount(QueryBuilder queryBuilder,
		string tableName,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateCountAll
--------------

.. highlight:: C#

This method is used to compose a SQL statement for `CountAll` operation.

::

	public string CreateCountAll(QueryBuilder queryBuilder,
		string tableName,
		string hints = null)
	{
		...
	}

CreateDelete
------------

.. highlight:: C#

This method is used to compose a SQL statement for `Delete` operation.

::

	public string CreateDelete(QueryBuilder queryBuilder,
		string tableName,
		QueryGroup where = null)
	{
		...
	}

CreateDeleteAll
---------------

.. highlight:: C#

This method is used to compose a SQL statement for `DeleteAll` operation.

::

	public string CreateDeleteAll(QueryBuilder queryBuilder,
		string tableName)
	{
		...
	}

CreateExists
------------

.. highlight:: C#

This method is used to compose a SQL statement for `Exists` operation.

::

	public string CreateExists(QueryBuilder queryBuilder,
		string tableName,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateInsert
------------

.. highlight:: C#

This method is used to compose a SQL statement for `Insert` operation.

::

	public string CreateInsert(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields = null,
		DbField primaryField = null,
		DbField identityField = null)
	{
		...
	}
	
CreateInsertAll
---------------

.. highlight:: C#

This method is used to compose a SQL statement for `InsertAll` operation.

::

	public string CreateInsertAll(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields = null,
		int batchSize = Constant.DefaultBatchOperationSize,
		DbField primaryField = null,
		DbField identityField = null)
	{
		...
	}

CreateMax
---------

.. highlight:: C#

This method is used to compose a SQL statement for `Max` operation.

::

	public string CreateMax(QueryBuilder queryBuilder,
		string tableName,
		Field field,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateMaxAll
------------

.. highlight:: C#

This method is used to compose a SQL statement for `MaxAll` operation.

::

	public string CreateMaxAll(QueryBuilder queryBuilder,
		string tableName,
		Field field,
		string hints = null)
	{
		...
	}

CreateMerge
-----------

.. highlight:: C#

This method is used to compose a SQL statement for `Merge` operation.

::

	public string CreateMerge(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		IEnumerable<Field> qualifiers = null,
		DbField primaryField = null,
		DbField identityField = null)
	{
		...
	}

CreateMergeAll
--------------

.. highlight:: C#

This method is used to compose a SQL statement for `MergeAll` operation.

::

	public string CreateMergeAll(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		IEnumerable<Field> qualifiers,
		int batchSize = Constant.DefaultBatchOperationSize,
		DbField primaryField = null,
		DbField identityField = null)
	{
		...
	}

CreateMin
---------

.. highlight:: C#

This method is used to compose a SQL statement for `Min` operation.

::

	public string CreateMin(QueryBuilder queryBuilder,
		string tableName,
		Field field,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateMinAll
------------

.. highlight:: C#

This method is used to compose a SQL statement for `MinAll` operation.

::

	public string CreateMinAll(QueryBuilder queryBuilder,
		string tableName,
		Field field,
		string hints = null)
	{
		...
	}

CreateQuery
-----------

.. highlight:: C#

This method is used to compose a SQL statement for `Query` operation.

::

	public string CreateQuery(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		QueryGroup where = null,
		IEnumerable<OrderField> orderBy = null,
		int? top = null,
		string hints = null)
	{
		...
	}

CreateQueryAll
--------------

.. highlight:: none

This method is used to compose a SQL statement for `QueryAll` operation.

::

	public string CreateQueryAll(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		IEnumerable<OrderField> orderBy = null,
		string hints = null)
	{
		...
	}

CreateSum
---------

.. highlight:: C#

This method is used to compose a SQL statement for `Sum` operation.

::

	public string CreateSum(QueryBuilder queryBuilder,
		string tableName,
		Field field,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateSumAll
------------

.. highlight:: C#

This method is used to compose a SQL statement for `SumAll` operation.

::

	public string CreateSumAll(QueryBuilder queryBuilder,
		string tableName,
		Field field,
		string hints = null)
	{
		...
	}

CreateTruncate
--------------

.. highlight:: C#

This method is used to compose a SQL statement for `Truncate` operation.

::

	public string CreateTruncate(QueryBuilder queryBuilder,
		string tableName)
	{
		...
	}

CreateUpdate
------------

.. highlight:: C#

This method is used to compose a SQL statement for `Update` operation.

::

	public string CreateUpdate(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		QueryGroup where = null,
		DbField primaryField = null,
		DbField identityField = null)
	{
		...
	}
	
CreateUpdateAll
---------------

.. highlight:: C#

This method is used to compose a SQL statement for `UpdateAll` operation.

::

	public string CreateUpdateAll(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		IEnumerable<Field> qualifiers,
		int batchSize = Constant.DefaultBatchOperationSize,
		DbField primaryField = null,
		DbField identityField = null)
	{
		...
	}