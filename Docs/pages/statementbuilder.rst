Working with StatementBuilder
=============================

The library supports statement building injection, allowing the developers to override the default query statement the library is using. By default, the library is using the `RepoDb.SqlDbStatementBuilder` class that implements the `RepoDb.Interfaces.IStatementBuilder` interface.

In order to override the statement builder, the developer must create a class that implements the `RepoDb.Interfaces.IStatementBuilder` interface. This allows the class to be injectable in the repository and implements the necessary methods needed by all operations.

A `QueryBuilder` object comes along the way when the custom statement builder is being created. This object is a helper object when composing the actual SQL Statements. See the `QueryBuilder` documentation.

Below are the methods of the `IStatementBuilder` interface.

- **CreateBatchQuery**: called when creating a `BatchQuery` statement.
- **CreateCount**: called when creating a `Count` statement.
- **CreateCountBig**: called when creating a `CountBig` statement.
- **CreateDelete**: called when creating a `Delete` statement.
- **CreateInlineUpdate**: called when creating a `InlineUpdate` statement.
- **CreateInsert**: called when creating a `Insert` statement.
- **CreateMerge**: called when creating a `Merge` statement.
- **CreateQuery**: called when creating a `Query` statement.
- **CreateUpdate**: called when creating a `Update` statement.

QueryBuilder Object
-------------------

.. highlight:: none

A query builder is an helper object used when creating a query statement in the statement builders. It contains important methods that is very useful to fluently construct the statement.

By default, the library is using the `RepoDb.QueryBuilder<TEntity>` object(implements the `RepoDb.QueryBuilder<TEntity>` when composing the statement.

Below is a sample code that creates a SQL Statement for the `Query` operation for `Oracle` data provide.

::

	public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
	{
		// Create an initial SELECT statement
		queryBuilder.Clear()
			.Select()
			.Fields(Command.Query)
			.From()
			.Table(Command.Query);
            
		// Add all fields for WHERE
		if (where != null)
		{
			queryBuilder.Where(where);
		}
            
		// Add the LIMIT (TOP in SQL Server)
		if (top > 0)
		{
			// In Oracle, SELECT [Fields] FROM [Table] WHERE [Fields] AND ROWNUM <=(Rows)
			queryBuilder.WriteText($"AND (ROWNUM <= {top})");
		}
            
		// End the builder
		queryBuilder.End();

		// Return the Statement
		return queryBuilder.ToString();
	}

As of today, the methods of this `RepoDb.QueryBuilder` object might be limited as it only varies on the current supported data provider SQL Server Data Provider.

CreateBatchQuery Method
-----------------------

.. highlight:: none

This method is being called when the `BatchQuery` operation of the repository is being called.

Below are the arguments of `CreateBatchQuery` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **where**: the expression used when creating a statement (of type `RepoDb.QueryGroup`).
- **page**: the page number implied when creating a statement.
- **rowsPerBatch**: the size of the rows implied when creating a statement.
- **orderBy**: the fields used in the `ORDER BY` when creating a statement.

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateBatchQuery` method.

::

	public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.With()
			.WriteText("CTE")
			.As()
			.OpenParen()
			.Select()
			.RowNumber()
			.Over()
			.OpenParen()
			.OrderBy(orderBy)
			.CloseParen()
			.As("[RowNumber],")
			.Fields(Command.BatchQuery)
			.From()
			.Table(Command.BatchQuery)
			.Where(where)
			.CloseParen()
			.Select()
			.Fields(Command.BatchQuery)
			.From()
			.WriteText("CTE")
			.WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
			.OrderBy(orderBy)
			.End();
		return queryBuilder.GetString();
	}

CreateCount Method
------------------

.. highlight:: none

This method is being called when the `Count` operation of the repository is being called.

Below are the arguments of `CreateCount` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **where**: the expression used when creating a statement (of type `RepoDb.QueryGroup`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateCount` method.

::

	public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.Count()
			.WriteText("(*) AS [Counted]")
			.From()
			.Table(Command.Count)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateCountBig Method
---------------------

.. highlight:: none

This method is being called when the `CountBig` operation of the repository is being called.

Below are the arguments of `CreateCountBig` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **where**: the expression used when creating a statement (of type `RepoDb.QueryGroup`).

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateCountBig` method.

::

	public string CreateCountBig<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.CountBig()
			.WriteText("(*) AS [Counted]")
			.From()
			.Table(Command.CountBig)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateDelete Method
-------------------

.. highlight:: none

This method is being called when the `Delete` operation of the repository is being called.

Below are the arguments of `CreateDelete` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateDelete` method.

::

	public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Delete()
			.From()
			.Table(Command.Delete)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateInlineUpdate Method
-------------------------

.. highlight:: none

This method is being called when the `InlineUpdate` operation of the repository is being called.

Below are the arguments of `CreateInlineUpdate` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **fields**: the list of fields to be updated when composing a statement (on enumerable of type `RepoDb.Interfaces.Field`).
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).
- **overrideIgnore**: the flag used to identify whether all the ignored fields will be included in the operation when composing a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInlineUpdate` method.

::

	public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, QueryGroup where, bool? overrideIgnore = false) where TEntity : DataEntity
	{
		if (overrideIgnore == false)
		{
			var properties = PropertyCache.Get<TEntity>(Command.InlineUpdate)
				.Select(property => property.GetMappedName());
			var unmatches = fields?.Where(field =>
				properties?.FirstOrDefault(property =>
					field.Name.ToLower() == property.ToLower()) == null);
			if (unmatches?.Count() > 0)
			{
				throw new InvalidOperationException($"The following columns ({unmatches.Select(field => field.AsField()).Join(", ")}) " +
					$"are not updatable for entity ({DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate)}).");
			}
		}
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Update()
			.Table(Command.InlineUpdate)
			.Set()
			.FieldsAndParameters(fields)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

CreateInsert Method
-------------------

.. highlight:: none

This method is being called when the `Insert` operation of the repository is being called.

Below are the arguments of `CreateInsert` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.QueryBuilder<TEntity>`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInsert` method.

::

	public string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		var primary = PrimaryPropertyCache.Get<TEntity>();
		queryBuilder
			.Clear()
			.Insert()
			.Into()
			.Table(Command.Insert)
			.OpenParen()
			.Fields(Command.Insert)
			.CloseParen()
			.Values()
			.OpenParen()
			.Parameters(Command.Insert)
			.CloseParen()
			.End();
		if (primary != null)
		{
			var result = primary.IsIdentity() ? "SCOPE_IDENTITY()" : $"@{primary.GetMappedName()}";
			queryBuilder
				.Select()
				.WriteText(result)
				.As("[Result]")
				.End();
		}
		return queryBuilder.GetString();
	}

CreateMerge Method
------------------

.. highlight:: none

This method is being called when the `Merge` operation of the repository is being called.

Below are the arguments of `CreateMerge` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **qualifiers**: the list of fields to be used as a qualifiers when composing a statement (on enumerable of type `RepoDb.Interfaces.Field`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateMerge` method.

::

	public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		if (qualifiers == null)
		{
			var primaryKey = PrimaryPropertyCache.Get<TEntity>();
			if (primaryKey != null)
			{
				qualifiers = new Field(primaryKey.Name).AsEnumerable();
			}
		}
		queryBuilder
			.Clear()
			// MERGE T USING S
			.Merge()
			.Table(Command.Merge) 
			.As("T")
			.Using()
			.OpenParen()
			.Select()
			.ParametersAsFields(Command.None) // All fields must be included for selection
			.CloseParen()
			.As("S")
			// QUALIFIERS
			.On()
			.OpenParen()
			.WriteText(qualifiers?
				.Select(
					field => field.AsJoinQualifier("S", "T"))
						.Join($" {Constant.And.ToUpper()} "))
			.CloseParen()
			// WHEN NOT MATCHED THEN INSERT VALUES
			.When()
			.Not()
			.Matched()
			.Then()
			.Insert()
			.OpenParen()
			.Fields(Command.Merge)
			.CloseParen()
			.Values()
			.OpenParen()
			.Parameters(Command.Merge)
			.CloseParen()
			// WHEN MATCHED THEN UPDATE SET
			.When()
			.Matched()
			.Then()
			.Update()
			.Set()
			.FieldsAndAliasFields(Command.Merge, "S")
			.End();
		return queryBuilder.GetString();
	}

CreateQuery Method
------------------

.. highlight:: none

This method is being called when the `Query` operation of the repository is being called.

Below are the arguments of `CreateQuery` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).
- **top**: the value that identifies the number of rows to be returned when composing a statement.
- **orderBy**: the fields used in the `ORDER BY` when creating a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateQuery` method.

::

	public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.Top(top)
			.Fields(Command.Query)
			.From()
			.Table(Command.Query)
			.Where(where)
			.OrderBy(orderBy)
			.End();
		return queryBuilder.GetString();
	}

CreateUpdate Method
-------------------

.. highlight:: none

This method is being called when the `Update` operation of the repository is being called.

Below are the arguments of `CreateUpdate` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateUpdate` method.

::

	public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Update()
			.Table(Command.Update)
			.Set()
			.FieldsAndParameters(Command.Update)
			.Where(where)
			.End();
		return queryBuilder.GetString();
	}

Creating a custom Statement Builder
-----------------------------------

.. highlight:: none

The main reason why the library supports the statement builder is to allow the developers override the default statement builder of the library. By default, the library statement builder is only limited for SQL Server providers (as SQL Statements). However, it will fail if the library is being used to access the Oracle, MySql or any other providers.

To create a custom statement builder, simply create a class and implements the `RepoDb.Interfaces.IStatementBuilder` interface.

::
	
	public class OracleDbStatementBuilder : IStatementBuilder
	{
		public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int? top = 0,
			IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int page,
			int rowsPerBatch, IEnumerable<OrderField> orderby) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateCountBig<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, QueryGroup where, bool? overrideIgnore = false) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}

		public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
		{
			throw new NotImplementedException();
		}
	}

Once the custom statement builder is created, it then can be used as an injectable object into the repository. See sample below injecting a statement builder for Oracle provider.

::

	var statementBuilder = new OracleDbStatementBuilder();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;"
		0, // commandTimeout
		null, // cache
		null, // trace
		statementBuilder, // statementBuilder
	);

With the code snippets above, everytime the repository operation methods is being called, the `OracleStatementBuilder` corresponding method will be executed.

Mapping a Statement Builder
---------------------------

.. highlight:: c#

By default, the library is using the `RepoDb.SqlDbStatementBuilder` object for the statement builder. As discussed above, when creating a custom statement builder, it can then be injected as an object in the repository. However, if the developer wants to map the statement builder by provider level, this feature comes into the play.

The mapper is of static type `RepoDb.StatementBuilderMapper`.

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
