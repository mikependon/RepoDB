using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Exceptions;

namespace RepoDb
{
    /// <summary>
    /// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
    /// </summary>
    public sealed class SqlStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlStatementBuilder"/> object.
        /// </summary>
        public SqlStatementBuilder() { }

        #region CreateBatchQuery

        /// <summary>
        /// Creates a SQL Statement for batch query operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to query.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public string CreateBatchQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // There should be fields
            if (fields == null || fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Validate order by
            if (orderBy == null || orderBy?.Any() != true)
            {
                throw new InvalidOperationException("The argument 'orderBy' is required.");
            }

            // Validate the page
            if (page < 0)
            {
                throw new InvalidOperationException("The page must be equals or greater than 0.");
            }

            // Validate the page
            if (rowsPerBatch < 1)
            {
                throw new InvalidOperationException($"The rows per batch must be equals or greater than 1.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
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
                .FieldsFrom(fields)
                .From()
                .TableNameFrom(tableName)
                .HintsFrom(hints)
                .WhereFrom(where)
                .CloseParen()
                .Select()
                .FieldsFrom(fields)
                .From()
                .WriteText("CTE")
                .WriteText(string.Concat("WHERE ([RowNumber] BETWEEN ", (page * rowsPerBatch) + 1, " AND ", (page + 1) * rowsPerBatch, ")"))
                .OrderByFrom(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateCount

        /// <summary>
        /// Creates a SQL Statement for count operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for count operation.</returns>
        public string CreateCount(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(1) AS [Counted]")
                .From()
                .TableNameFrom(tableName)
                .HintsFrom(hints)
                .WhereFrom(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for count-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public string CreateCountAll(QueryBuilder queryBuilder,
            string tableName,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(1) AS [Counted]")
                .From()
                .TableNameFrom(tableName)
                .HintsFrom(hints)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateDelete

        /// <summary>
        /// Creates a SQL Statement for delete operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <returns>A sql statement for delete operation.</returns>
        public string CreateDelete(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName)
                .WhereFrom(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for delete-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public string CreateDeleteAll(QueryBuilder queryBuilder,
            string tableName)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateInsert

        /// <summary>
        /// Creates a SQL Statement for insert operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <returns>A sql statement for insert operation.</returns>
        [Obsolete("Please use the overloaded method.")]
        public string CreateInsert(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            DbField primaryField = null)
        {
            return CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: primaryField,
                identityField: null);
        }

        /// <summary>
        /// Creates a SQL Statement for insert operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public string CreateInsert(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields == null || fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of insertable fields must not be null for '{tableName}'.");
            }

            // Ensure the primary is on the list if it is not an identity
            if (primaryField != null)
            {
                if (primaryField != identityField)
                {
                    var isPresent = fields.FirstOrDefault(f => f.Name.ToLower() == primaryField.Name.ToLower()) != null;
                    if (isPresent == false)
                    {
                        throw new InvalidOperationException("The non-identity primary field must be present during insert operation.");
                    }
                }
            }

            // Variables needed
            var databaseType = "BIGINT";
            var insertableFields = fields
                .Where(f => f.Name.ToLower() != identityField?.Name.ToLower());

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToSqlDbTypeResolver().Resolve(identityField.Type);
                databaseType = new SqlDbTypeToStringNameResolver().Resolve(dbType);
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName)
                .OpenParen()
                .FieldsFrom(insertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(insertableFields)
                .CloseParen()
                .End();
            var result = identityField != null ? string.Concat("CONVERT(", databaseType, ", SCOPE_IDENTITY())") : primaryField != null ? primaryField.Name.AsParameter() : "NULL";
            queryBuilder
                .Select()
                .WriteText(result)
                .As("[Result]")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateInsertAll

        /// <summary>
        /// Creates a SQL Statement for insert-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public string CreateInsertAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchInsertSize,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields == null || fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of insertable fields must not be null for '{tableName}'.");
            }

            // Ensure the primary is on the list if it is not an identity
            if (primaryField != null)
            {
                if (primaryField != identityField)
                {
                    var isPresent = fields.FirstOrDefault(f => f.Name.ToLower() == primaryField.Name.ToLower()) != null;
                    if (isPresent == false)
                    {
                        throw new InvalidOperationException("The non-identity primary field must be present during insert operation.");
                    }
                }
            }

            // Variables needed
            var databaseType = "BIGINT";
            var insertableFields = fields
                .Where(f => f.Name.ToLower() != identityField?.Name.ToLower());

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToSqlDbTypeResolver().Resolve(identityField.Type);
                databaseType = new SqlDbTypeToStringNameResolver().Resolve(dbType);
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                queryBuilder.Insert()
                    .Into()
                    .TableNameFrom(tableName)
                    .OpenParen()
                    .FieldsFrom(insertableFields)
                    .CloseParen()
                    .Values()
                    .OpenParen()
                    .ParametersFrom(insertableFields, index)
                    .CloseParen()
                    .End();

                // Set the return field
                if (identityField != null)
                {
                    var returnValue = string.Concat(identityField.UnquotedName.AsParameter(index), " = ",
                        "CONVERT(", databaseType, ", SCOPE_IDENTITY())");
                    queryBuilder
                        .Set()
                        .WriteText(returnValue)
                        .End();
                }
            }

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for merge operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <returns>A sql statement for merge operation.</returns>
        [Obsolete("Please use the overloaded method.")]
        public string CreateMerge(QueryBuilder queryBuilder,
             string tableName,
             IEnumerable<Field> fields,
             IEnumerable<Field> qualifiers = null,
             DbField primaryField = null)
        {
            return CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: null);
        }

        /// <summary>
        /// Creates a SQL Statement for merge operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public string CreateMerge(QueryBuilder queryBuilder,
             string tableName,
             IEnumerable<Field> fields,
             IEnumerable<Field> qualifiers = null,
             DbField primaryField = null,
             DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields == null || fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of insertable fields must not be null for '{tableName}'.");
            }

            // Check the qualifiers
            if (qualifiers != null)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers?.Where(field =>
                    fields?.FirstOrDefault(f =>
                        field.Name.ToLower() == f.Name.ToLower()) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers?.Any() == true)
                {
                    throw new InvalidQualiferFieldsException($"The qualifiers '{unmatchesQualifiers.Select(field => field.Name).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }
            else
            {
                if (primaryField != null)
                {
                    // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                    var isPresent = fields?.FirstOrDefault(f => f.Name.ToLower() == primaryField.Name.ToLower()) != null;

                    // Throw if not present
                    if (isPresent == false)
                    {
                        throw new InvalidQualiferFieldsException($"There are no qualifier fields found for '{tableName}'. Ensure that the " +
                            $"primary field is present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                    }

                    // The primary is present, use it as a default if there are no qualifiers given
                    qualifiers = Field.From(primaryField.UnquotedName);
                }
                else
                {
                    // Throw exception, qualifiers are not defined
                    throw new NullReferenceException($"There are no qualifier fields found for '{tableName}'.");
                }
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => field.Name.ToLower() != identityField?.Name.ToLower());
            var updateableFields = fields
                .Where(field => field.Name.ToLower() != primaryField?.Name.ToLower() && field.Name.ToLower() != identityField?.Name.ToLower());

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName)
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
                            .Join(" AND "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S")
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "S")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateQuery

        /// <summary>
        /// Creates a SQL Statement for query operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for query operation.</returns>
        public string CreateQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            if (orderBy != null)
            {
                // Check if the order fields are present in the given fields
                var unmatchesOrderFields = orderBy?.Where(orderField =>
                    fields?.FirstOrDefault(f =>
                        orderField.Name.ToLower() == f.Name.ToLower()) == null);

                // Throw an error we found any unmatches
                if (unmatchesOrderFields?.Any() == true)
                {
                    throw new InvalidOperationException($"The order fields '{unmatchesOrderFields.Select(field => field.Name).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .TopFrom(top)
                .FieldsFrom(fields)
                .From()
                .TableNameFrom(tableName)
                .HintsFrom(hints)
                .WhereFrom(where)
                .OrderByFrom(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateQueryAll

        /// <summary>
        /// Creates a SQL Statement for query-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for query operation.</returns>
        public string CreateQueryAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<OrderField> orderBy = null,
            string hints = null)
        {
            // Guard the target table
            GuardTableName(tableName);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            if (orderBy != null)
            {
                // Check if the order fields are present in the given fields
                var unmatchesOrderFields = orderBy?.Where(orderField =>
                    fields?.FirstOrDefault(f =>
                        orderField.Name.ToLower() == f.Name.ToLower()) == null);

                // Throw an error we found any unmatches
                if (unmatchesOrderFields?.Any() == true)
                {
                    throw new InvalidOperationException($"The order fields '{unmatchesOrderFields.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .FieldsFrom(fields)
                .From()
                .TableNameFrom(tableName)
                .HintsFrom(hints)
                .OrderByFrom(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateTruncate

        /// <summary>
        /// Creates a SQL Statement for truncate operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public string CreateTruncate(QueryBuilder queryBuilder,
            string tableName)
        {
            // Guard the target table
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Truncate()
                .Table()
                .TableNameFrom(tableName)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateUpdate

        /// <summary>
        /// Creates a SQL Statement for inline-update operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <returns>A sql statement for update operation.</returns>
        [Obsolete("Please use the overloaded method.")]
        public string CreateUpdate(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            DbField primaryField = null)
        {
            return CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: primaryField,
                identityField: null);
        }

        /// <summary>
        /// Creates a SQL Statement for inline-update operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for update operation.</returns>
        public string CreateUpdate(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Append the proper prefix
            where?.PrependAnUnderscoreAtTheParameters();

            // Gets the updatable fields
            var updatableFields = fields
                .Where(f => (f.Name.ToLower() != primaryField?.Name.ToLower() && f.Name.ToLower() != identityField?.Name.ToLower()));

            // Check if there are updatable fields
            if (updatableFields?.Any() != true)
            {
                throw new InvalidOperationException($"There are no updatable fields found for '{tableName}'.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Update()
                .TableNameFrom(tableName)
                .Set()
                .FieldsAndParametersFrom(updatableFields)
                .WhereFrom(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region Helper

        /// <summary>
        /// Throws an exception if the table name is null or empty.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        private void GuardTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new NullReferenceException("The name of the table could be null.");
            }
        }

        /// <summary>
        /// Throws an exception if the primary field is not really a primary field.
        /// </summary>
        /// <param name="field">The instance of the primary field.</param>
        private void GuardPrimary(DbField field)
        {
            if (field?.IsPrimary == false)
            {
                throw new InvalidOperationException("The field is not defined as primary.");
            }
        }

        /// <summary>
        /// Throws an exception if the identity field is not really an identity field.
        /// </summary>
        /// <param name="field">The instance of the identity field.</param>
        private void GuardIdentity(DbField field)
        {
            if (field?.IsIdentity == false)
            {
                throw new InvalidOperationException("The field is not defined as primary.");
            }
        }

        #endregion
    }
}
