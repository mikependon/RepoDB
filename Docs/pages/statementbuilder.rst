Working with StatementBuilder
=============================

The library supports statement building injection, allowing the developers to override the default query statement the library is using. By default, the library is using the `RepoDb.SqlDbStatementBuilder` that is only working for SQL Server databases.

In order to override the statement builder, the developer must create a class that implements the `RepoDb.Interfaces.IStatementBuilder` interface. This allows the class to be injectable in the repository and implements the necessary methods needed by all operations.

A `QueryBuilder` object comes along the way when the custom statement builder is being created. This object is a helper object when composing the actual SQL Statements. See the `QueryBuilder` documentation.

Below are the methods of the `IStatementBuilder` interface.

- **CreateBatchQuery**: called when creating a `BatchQuery` statement.
- **CreateCount**: called when creating a `Count` statement.
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

By default, the library is using the `RepoDb.QueryBuilder<TEntity>` object when composing the statement.

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
		var queryProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Query);
		var batchQueryProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.BatchQuery)
			.Where(property => queryProperties.Contains(property));
		var fields = batchQueryProperties.Select(property => new Field(property.Name));
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
			.Fields(fields)
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
			.CountBig()
			.WriteText("(1) AS [Counted]")
			.From()
			.Table(Command.Count)
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
- **fields**: the list of fields to be updated when composing a statement (on enumerable of type `RepoDb.Field`).
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).
- **overrideIgnore**: the flag used to identify whether all the ignored fields will be included in the operation when composing a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInlineUpdate` method.

::

	public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, QueryGroup where, bool? overrideIgnore = false) where TEntity : DataEntity
	{
		if (overrideIgnore == false)
		{
			var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update);
			var inlineUpdateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineUpdate)
				.Where(property => property != DataEntityExtension.GetPrimaryProperty<TEntity>() && updateableProperties.Contains(property))
				.Select(property => property.GetMappedName());
			var unmatchesProperties = fields?.Where(field =>
				inlineUpdateableProperties?.FirstOrDefault(property =>
					field.Name.ToLower() == property.ToLower()) == null);
			if (unmatchesProperties?.Count() > 0)
			{
				throw new InvalidOperationException($"The following columns ({unmatchesProperties.Select(field => field.AsField()).Join(", ")}) " +
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
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
		var isPrimaryIdentity = primary.IsIdentity();
		var fields = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
			.Where(property => !(isPrimaryIdentity && property == primary))
			.Select(p => new Field(p.Name));
		queryBuilder
			.Clear()
			.Insert()
			.Into()
			.Table(Command.Insert)
			.OpenParen()
			.Fields(fields)
			.CloseParen()
			.Values()
			.OpenParen()
			.Parameters(fields)
			.CloseParen()
			.End();
		var result = isPrimaryIdentity ? "SCOPE_IDENTITY()" : $"@{primary.GetMappedName()}";
		queryBuilder
			.Select()
			.WriteText(result)
			.As("[Result]")
			.End();
		return queryBuilder.GetString();
	}

CreateMerge Method
------------------

.. highlight:: none

This method is being called when the `Merge` operation of the repository is being called.

Below are the arguments of `CreateMerge` method.

- **queryBuilder**: the builder used when composing a statement (of type `RepoDb.QueryBuilder<TEntity>`).
- **qualifiers**: the list of fields to be used as a qualifiers when composing a statement (on enumerable of type `RepoDb.Field`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateMerge` method.

::

	public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
		var isPrimaryIdentity = primary.IsIdentity();
		if (qualifiers == null && primary != null)
		{
			qualifiers = new Field(primary?.Name).AsEnumerable();
		}
		var insertProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
			.Where(property => !(isPrimaryIdentity && property == primary));
		var updateProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
			.Where(property => property != primary);
		var mergeProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge);
		var mergeInsertableFields = mergeProperties
			.Where(property => insertProperties.Contains(property))
			.Select(property => new Field(property.Name));
		var mergeUpdateableFields = mergeProperties
			.Where(property => updateProperties.Contains(property))
			.Select(property => new Field(property.Name));
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
						.Join($" {StringConstant.And.ToUpper()} "))
			.CloseParen()
			// WHEN NOT MATCHED THEN INSERT VALUES
			.When()
			.Not()
			.Matched()
			.Then()
			.Insert()
			.OpenParen()
			.Fields(mergeInsertableFields)
			.CloseParen()
			.Values()
			.OpenParen()
			.Parameters(mergeInsertableFields)
			.CloseParen()
			// WHEN MATCHED THEN UPDATE SET
			.When()
			.Matched()
			.Then()
			.Update()
			.Set()
			.FieldsAndAliasFields(mergeUpdateableFields, "S")
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
		var fields = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update)
			.Where(property => property != DataEntityExtension.GetPrimaryProperty<TEntity>())
			.Select(p => new Field(p.Name));
		queryBuilder
			.Clear()
			.Update()
			.Table(Command.Update)
			.Set()
			.FieldsAndParameters(fields)
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
