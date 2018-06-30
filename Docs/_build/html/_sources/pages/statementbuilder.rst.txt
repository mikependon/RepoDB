Working with StatementBuilder
=============================

The library supports statement building injection, allowing the developers to override the default query statement the library is using. By default, the library is using the `RepoDb.SqlDbStatementBuilder` that is only working for SQL Server databases.

In order to override the statement builder, the developer must create a class that implements the `RepoDb.Interfaces.IStatementBuilder` interface. This allows the class to be injectable in the repository and implements the necessary methods needed by all operations.

A `QueryBuilder` object comes along the way when the custom statement builder is being created. This object is a helper object when composing the actual SQL Statements. See the `QueryBuilder` documentation.

Below are the methods of the `IStatementBuilder` interface.

- **CreateBatchQuery**: called when creating a `BatchQuery` statement.
- **CreateCount**: called when creating a `Count` statement.
- **CreateDelete**: called when creating a `Delete` statement.
- **CreateDeleteAll**: called when creating a `Delete` statement.
- **CreateInlineInsert**: called when creating a `InlineInsert` statement.
- **CreateInlineMerge**: called when creating a `InlineMerge` statement.
- **CreateInlineUpdate**: called when creating a `InlineUpdate` statement.
- **CreateInsert**: called when creating a `Insert` statement.
- **CreateMerge**: called when creating a `Merge` statement.
- **CreateQuery**: called when creating a `Query` statement.
- **CreateTruncate**: called when creating a `Truncate` statement.
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
            
		// Add the ROWNUM (TOP in SQL Server)
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
		var queryProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Query);
		var batchQueryProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.BatchQuery)
			.Where(property => queryProperties.Contains(property));
		var fields = batchQueryProperties.Select(property => new Field(property.Name));

		// Build the SQL Statement
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
			.OrderByFrom(orderBy)
			.CloseParen()
			.As("[RowNumber],")
			.FieldsFrom(Command.BatchQuery)
			.From()
			.TableFrom(Command.BatchQuery)
			.WhereFrom(where)
			.CloseParen()
			.Select()
			.FieldsFrom(fields)
			.From()
			.WriteText("CTE")
			.WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
			.OrderByFrom(orderBy)
			.End();

		// Return the query
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
			.TableFrom(Command.Count)
			.WhereFrom(where)
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
			.TableFrom(Command.Delete)
			.WhereFrom(where)
			.End();
		return queryBuilder.GetString();
	}

CreateDeleteAll Method
----------------------

.. highlight:: none

This method is being called when the `DeleteAll` operation of the repository is being called.

Below are the arguments of `CreateDeleteAll` method.

- **queryBuilder**: the builder used when creating a statement (of type `RepoDb.QueryBuilder<TEntity>`).

See below the actual implementation of `SqlDbStatementBuilder` object for `CreateDeleteAll` method.

::

	public string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Delete()
			.From()
			.TableFrom(Command.DeleteAll)
			.End();
		return queryBuilder.GetString();
	}

CreateInlineInsert Method
-------------------------

.. highlight:: none

This method is being called when the `InlineInsert` operation of the repository is being called.

Below are the arguments of `CreateInlineInsert` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
- **fields**: the list of the fields to be a part of the inline merge operation in SQL Statement composition.
- **overrideIgnore**: the flag used to identify whether all the ignored fields will be included in the operation when composing a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInlineInsert` method.

::

	public string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, bool? overrideIgnore = false)
		where TEntity : DataEntity
	{
		return CreateInlineInsert<TEntity>(queryBuilder, fields, overrideIgnore, false);
	}

	internal string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields,
		bool? overrideIgnore = false, bool isPrimaryIdentity = false)
		where TEntity : DataEntity
	{
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
		var hasFields = isPrimaryIdentity ? fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower()) : fields.Any();

		// Check if there are fields
		if (hasFields == false)
		{
			throw new InvalidOperationException($"No inline insertable fields found at type '{typeof(TEntity).FullName}'.");
		}

		// Check for the unmatches
		if (overrideIgnore == false)
		{
			var insertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert);
			var inlineInsertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineInsert)
				.Where(property => insertableProperties.Contains(property))
				.Select(property => property.GetMappedName());
			var unmatchesProperties = fields?.Where(field =>
				inlineInsertableProperties?.FirstOrDefault(property =>
					field.Name.ToLower() == property.ToLower()) == null);
			if (unmatchesProperties.Count() > 0)
			{
				throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
					$"inline insertable for object '{DataEntityExtension.GetMappedName<TEntity>(Command.InlineInsert)}'.");
			}
		}

		// Check for the primary key
		if (primary != null && isPrimaryIdentity)
		{
			fields = fields?
				.Where(field => field.Name.ToLower() != primary.Name.ToLower())
					.Select(field => field);
		}

		// Build the SQL Statement
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Insert()
			.Into()
			.TableFrom(Command.Insert)
			.OpenParen()
			.FieldsFrom(fields)
			.CloseParen()
			.Values()
			.OpenParen()
			.ParametersFrom(fields)
			.CloseParen()
			.End();
		var result = isPrimaryIdentity ? "SCOPE_IDENTITY()" : (primary != null) ? $"@{primary.GetMappedName()}" : "NULL";
		queryBuilder
			.Select()
			.WriteText(result)
			.As("[Result]")
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

CreateInlineMerge Method
------------------------

.. highlight:: none

This method is being called when the `InlineMerge` operation of the repository is being called.

Below are the arguments of `CreateInlineMerge` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
- **fields**: the list of the fields to be a part of the inline merge operation in SQL Statement composition.
- **qualifiers**: the list of the qualifier fields to be used by the inline merge operation on a SQL Statement.
- **overrideIgnore**: the flag used to identify whether all the ignored fields will be included in the operation when composing a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInlineUpdate` method.

::

	public string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, bool? overrideIgnore = false)
		where TEntity : DataEntity
	{
		return CreateInlineMerge<TEntity>(queryBuilder, fields, qualifiers, overrideIgnore, false);
	}

	internal string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, IEnumerable<Field> qualifiers,
		bool? overrideIgnore = false, bool isPrimaryIdentity = false)
		where TEntity : DataEntity
	{
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();

		// Get all target properties
		var mergeableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge);
		var inlineMergeableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineMerge)
			.Where(property => mergeableProperties.Contains(property));

		// Check for the unmatches
		if (overrideIgnore == false)
		{
			var unmatchesProperties = fields?.Where(field =>
				inlineMergeableProperties?.FirstOrDefault(property => field.Name.ToLower() == property.GetMappedName().ToLower()) == null);
					//.Where(property => property.Name.ToLower() != primary?.GetMappedName().ToLower());
			if (unmatchesProperties.Count() > 0)
			{
				throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
					$"inline mergeable for object '{DataEntityExtension.GetMappedName<TEntity>(Command.InlineMerge)}'.");
			}
		}

		// Use the primary for qualifiers if there is no any
		if (qualifiers == null && primary != null)
		{
			qualifiers = Field.From(primary.GetMappedName());
		}

		// Get all target fields
		var insertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
			.Where(property => !(isPrimaryIdentity && property == primary));
		var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update)
			.Where(property => property != primary);
		var mergeInsertableFields = mergeableProperties
			.Where(property => insertableProperties.Contains(property) &&
				mergeableProperties.Contains(property) &&
					inlineMergeableProperties.Contains(property))
			.Where(property =>
				fields.Any(field => field.Name.ToLower() == property.GetMappedName().ToLower()))
			.Select(property => new Field(property.Name));
		var mergeUpdateableFields = mergeableProperties
			.Where(property => updateableProperties.Contains(property) &&
				mergeableProperties.Contains(property) &&
					inlineMergeableProperties.Contains(property))
			.Where(property =>
				fields.Any(field => field.Name.ToLower() == property.GetMappedName().ToLower()))
			.Select(property => new Field(property.Name));

		// Check if there are inline mergeable fields (for insert)
		if (!mergeInsertableFields.Any())
		{
			throw new InvalidOperationException($"No inline mergeable fields (for insert) found at type '{typeof(TEntity).FullName}'.");
		}

		// Check if there are inline mergeable fields (for update)
		if (!mergeUpdateableFields.Any())
		{
			throw new InvalidOperationException($"No inline mergeable fields (for update) found at type '{typeof(TEntity).FullName}'.");
		}

		// Build the SQL Statement
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			// MERGE T USING S
			.Merge()
			.TableFrom(Command.Merge)
			.As("T")
			.Using()
			.OpenParen()
			.Select()
			.ParametersAsFieldsFrom(fields)
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
			.FieldsFrom(mergeInsertableFields)
			.CloseParen()
			.Values()
			.OpenParen()
			.ParametersFrom(mergeInsertableFields)
			.CloseParen()
			// WHEN MATCHED THEN UPDATE SET
			.When()
			.Matched()
			.Then()
			.Update()
			.Set()
			.FieldsAndAliasFieldsFrom(mergeUpdateableFields, "S")
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

CreateInlineUpdate Method
-------------------------

.. highlight:: none

This method is being called when the `InlineUpdate` operation of the repository is being called.

Below are the arguments of `CreateInlineUpdate` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
- **fields**: the list of the fields to be a part of the inline merge operation in SQL Statement composition.
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).
- **overrideIgnore**: the flag used to identify whether all the ignored fields will be included in the operation when composing a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInlineUpdate` method.

::

	public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields,
		QueryGroup where, bool? overrideIgnore = false)
		where TEntity : DataEntity
	{
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
		var hasFields = fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower());

		// Check if there are fields
		if (hasFields == false)
		{
			throw new InvalidOperationException($"No inline updatable fields found at type '{typeof(TEntity).FullName}'.");
		}

		// Get the target properties
		var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update);
		var inlineUpdateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineUpdate)
			.Where(property => property != primary && updateableProperties.Contains(property))
			.Select(property => property.GetMappedName());

		// Check for the unmatches
		if (overrideIgnore == false)
		{
			var unmatchesProperties = fields?.Where(field =>
				inlineUpdateableProperties?.FirstOrDefault(property => field.Name.ToLower() == property.ToLower()) == null);
			if (unmatchesProperties.Count() > 0)
			{
				throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
					$"inline updateable for object '{DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate)}'.");
			}
		}

		// Build the SQL Statement
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Update()
			.TableFrom(Command.InlineUpdate)
			.Set()
			.FieldsAndParametersFrom(fields)
			.WhereFrom(where)
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

CreateInsert Method
-------------------

.. highlight:: none

This method is being called when the `Insert` operation of the repository is being called.

Below are the arguments of `CreateInsert` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateInsert` method.

::

	public string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder)
		where TEntity : DataEntity
	{
		return CreateInsert(queryBuilder, false);
	}

	internal string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, bool isPrimaryIdentity)
		where TEntity : DataEntity
	{
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
		var fields = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
			.Where(property => !(isPrimaryIdentity && property == primary))
			.Select(p => new Field(p.Name));

		// Build the SQL Statement
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Insert()
			.Into()
			.TableFrom(Command.Insert)
			.OpenParen()
			.FieldsFrom(fields)
			.CloseParen()
			.Values()
			.OpenParen()
			.ParametersFrom(fields)
			.CloseParen()
			.End();
		var result = isPrimaryIdentity ? "SCOPE_IDENTITY()" : (primary != null) ? $"@{primary.GetMappedName()}" : "NULL";
		queryBuilder
			.Select()
			.WriteText(result)
			.As("[Result]")
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

CreateMerge Method
------------------

.. highlight:: none

This method is being called when the `Merge` operation of the repository is being called.

Below are the arguments of `CreateMerge` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
- **qualifiers**: the list of fields to be used as a qualifiers when composing a statement (on enumerable of type `RepoDb.Field`).
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateMerge` method.

::

	public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers)
		where TEntity : DataEntity
	{
		return CreateMerge(queryBuilder, qualifiers);
	}

	internal string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers, bool isPrimaryIdentity)
		where TEntity : DataEntity
	{
		var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();

		// Add the primary key as the default qualifier
		if (qualifiers == null && primary != null)
		{
			qualifiers = Field.From(primary.GetMappedName());
		}

		// Get the target properties
		var insertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
			.Where(property => !(isPrimaryIdentity && property == primary));
		var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge)
			.Where(property => property != primary);
		var mergeableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge);
		var mergeInsertableFields = mergeableProperties
			.Where(property => insertableProperties.Contains(property))
			.Select(property => new Field(property.Name));
		var mergeUpdateableFields = mergeableProperties
			.Where(property => updateableProperties.Contains(property))
			.Select(property => new Field(property.Name));

		// Build the SQL Statement
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			// MERGE T USING S
			.Merge()
			.TableFrom(Command.Merge)
			.As("T")
			.Using()
			.OpenParen()
			.Select()
			.ParametersAsFieldsFrom(Command.None) // All fields must be included for selection
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
			.FieldsFrom(mergeInsertableFields)
			.CloseParen()
			.Values()
			.OpenParen()
			.ParametersFrom(mergeInsertableFields)
			.CloseParen()
			// WHEN MATCHED THEN UPDATE SET
			.When()
			.Matched()
			.Then()
			.Update()
			.Set()
			.FieldsAndAliasFieldsFrom(mergeUpdateableFields, "S")
			.End();

		// Return the query
		return queryBuilder.GetString();
	}

CreateQuery Method
------------------

.. highlight:: none

This method is being called when the `Query` operation of the repository is being called.

Below are the arguments of `CreateQuery` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
- **where**: the expression used when composing a statement (of type `RepoDb.QueryGroup`).
- **top**: the value that identifies the number of rows to be returned when composing a statement.
- **orderBy**: the fields used in the `ORDER BY` when creating a statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateQuery` method.

::

	public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null)
		where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Select()
			.TopFrom(top)
			.FieldsFrom(Command.Query)
			.From()
			.TableFrom(Command.Query)
			.WhereFrom(where)
			.OrderByFrom(orderBy)
			.End();
		return queryBuilder.GetString();
	}

CreateTruncate Method
---------------------

.. highlight:: none

This method is being called when the `Truncate` operation of the repository is being called.

Below are the arguments of `CreateTruncate` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
 
See below the actual implementation of `SqlDbStatementBuilder` object for `CreateTruncate` method.

::

	public string CreateTruncate<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
	{
		queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
		queryBuilder
			.Clear()
			.Truncate()
			.Table()
			.TableFrom(Command.Delete)
			.End();
		return queryBuilder.GetString();
	}

CreateUpdate Method
-------------------

.. highlight:: none

This method is being called when the `Update` operation of the repository is being called.

Below are the arguments of `CreateUpdate` method.

- **queryBuilder**: an instance of query builder used to build the SQL statement.
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
			.TableFrom(Command.Update)
			.Set()
			.FieldsAndParametersFrom(fields)
			.WhereFrom(where)
			.End();
		return queryBuilder.GetString();
	}

Creating a custom Statement Builder
-----------------------------------

.. highlight:: c#

The main reason why the library supports the statement builder is to allow the developers override the default statement builder of the library. By default, the library statement builder is only limited for SQL Server providers (as SQL Statements). However, it will fail if the library is being used to access the Oracle, MySql or any other providers.

To create a custom statement builder, simply create a class and implements the `RepoDb.Interfaces.IStatementBuilder` interface.

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
