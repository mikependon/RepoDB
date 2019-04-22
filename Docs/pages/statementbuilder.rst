StatementBuilder
================

The library supports statement building injection, allowing the developers to override the default query statement the library is using. By default, the library is using the `SqlDbStatementBuilder` that is only working for SQL Server databases.

QueryBuilder
------------

.. highlight:: none

A query builder is an helper object used when creating a query statement in the statement builders. It contains important methods that is very useful to fluently construct the statement.

By default, the library is using the `QueryBuilder<TEntity>` object when composing the statement.

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
		if (fields == null || fields.Any() == false)
		{
			throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
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
				queryBuilder.WriteText($"AND (ROWNUM <= {top})");
			}
			else
			{
				queryBuilder.WriteText($"(ROWNUM <= {top})");
			}
		}

		// Build the filter and ordering
		queryBuilder
			.OrderByFrom(orderBy)
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

CreateBatchQuery
----------------

.. highlight:: none

This method is used to compose a SQL statement for `BatchQuery` operation.

::

	public string CreateBatchQuery(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		int page,
		int rowsPerBatch,
		IEnumerable<OrderField> orderBy = null,
		QueryGroup where = null,
		string hints = null)
	{
		...
	}

CreateCount
-----------

.. highlight:: none

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

.. highlight:: none

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

.. highlight:: none

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

.. highlight:: none

This method is used to compose a SQL statement for `DeleteAll` operation.

::

	public string CreateDeleteAll(QueryBuilder queryBuilder,
		string tableName)
	{
		...
	}

CreateInsert
------------

.. highlight:: none

This method is used to compose a SQL statement for `Insert` operation.

::

	public string CreateInsert(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields = null,
		DbField primaryField = null)
	{
		...
	}

CreateMerge
-----------

.. highlight:: none

This method is used to compose a SQL statement for `Merge` operation.

::

	public string CreateMerge(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		IEnumerable<Field> qualifiers = null,
		DbField primaryField = null)
	{
		...
	}

CreateQuery
-----------

.. highlight:: none

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

CreateTruncate
--------------

.. highlight:: none

This method is used to compose a SQL statement for `Truncate` operation.

::

	public string CreateTruncate(QueryBuilder queryBuilder,
		string tableName)
	{
		...
	}

CreateUpdate
------------

.. highlight:: none

This method is used to compose a SQL statement for `Update` operation.

::

	public string CreateUpdate(QueryBuilder queryBuilder,
		string tableName,
		IEnumerable<Field> fields,
		QueryGroup where = null,
		DbField primaryField = null)
	{
		...
	}

Cutomizing a Builder
--------------------

.. highlight:: c#

The main reason why the library supports the statement builder is to allow the developers override the default statement builder of the library. By default, the library statement builder is only limited for SQL Server providers (as SQL Statements). However, it will fail if the library is being used to access the Oracle, MySql or any other providers.

To create a custom statement builder, simply create a class and implements the `Interfaces.IStatementBuilder` interface.

::

	public class OracleDbStatementBuilder : IStatementBuilder
	{
		// Implements the IStatementBuilder methods here
	}

Once the custom statement builder is created, it then can be used as an injectable object into the repository. See sample below injecting a statement builder for Oracle provider.

::

	var statementBuilder = new OracleDbStatementBuilder();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", statementBuilder);

With the code snippets above, everytime the repository operation methods is being called, the `OracleStatementBuilder` corresponding method will be executed.

SqlDbStatementBuilder
---------------------

.. highlight:: c#

By default, the library is using the `SqlDbStatementBuilder` object for the statement builder. As discussed above, when creating a custom statement builder, it can then be injected as an object in the repository. However, if the developer wants to map the statement builder by provider level, this feature comes into the play.

The mapper is of static type `StatementBuilderMapper`.

The following are the methods of this object.

- **Get**: returns the instance of statement builder by type (of type `System.Data.IDbConnection`).
- **Map**: maps the custom statement builder to a type (of type `System.Data.IDbConnection`).

Mapping a statement builder enables the developer to map the custom statement builder by provider level. 

Let say for example, if the developers created the following repositories:

 - CustomerRepository (for `SqlConnection`)
 - ProductRepository (for `SqlConnection`)
 - OrderRepository (for `OracleConnection`)
 - CompanyRepository (for `OleDbConnection`)

Then, by mapping a custom statement builders, it will enable the library to summon the statement builder based on the provider of the repository. With the following repositories defined above, the developers must implement atleast two (2) custom statement builder (one for Oracle provider and one for OleDb provider).

Let say the developer created 2 new custom statement builders named:

 - OracleStatementBuilder
 - OleDbStatementBuilder

The developers can now map the following statement builders into the repositories by provider level. Below is the sample way on how to do it.

::

	StatementBuilderMapper.Map(typeof(OracleConnection), new OracleStatementBuilder());
	StatementBuilderMapper.Map(typeof(OleDbConnection), new OleDbStatementBuilder());

The object `StatementBuilderMapper.Map` is callable everywhere in the application as it was implemented in s static way. Make sure to call it once, or else, an exception will be thrown.
