using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Exceptions;
using RepoDb.Enumerations;
using System.Reflection;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A base class for all <see cref="IStatementBuilder"/>-based objects.
    /// </summary>
    public abstract class BaseStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="BaseStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public BaseStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
        {
            DbSetting = dbSetting ?? throw new NullReferenceException("The database setting cannot be null.");
            ConvertFieldResolver = convertFieldResolver;
            AverageableClientTypeResolver = averageableClientTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the database setting object that is currently in used.
        /// </summary>
        protected IDbSetting DbSetting { get; }

        /// <summary>
        /// Gets the resolver used to convert the <see cref="Field"/> object.
        /// </summary>
        protected IResolver<Field, IDbSetting, string> ConvertFieldResolver { get; }

        /// <summary>
        /// Gets the resolver that is being used to resolve the type to be averageable type.
        /// </summary>
        protected IResolver<Type, Type> AverageableClientTypeResolver { get; }

        #endregion

        #region Virtual/Common

        #region CreateAverage

        /// <summary>
        /// Creates a SQL Statement for 'Average' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for average operation.</returns>
        public virtual string CreateAverage(string tableName,
            Field field,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }
            else
            {
                field.Type = AverageableClientTypeResolver?.Resolve(field.Type ?? DbSetting.AverageableType);
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Average(field, DbSetting, ConvertFieldResolver)
                .WriteText($"AS {"AverageValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateAverageAll

        /// <summary>
        /// Creates a SQL Statement for 'AverageAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for average-all operation.</returns>
        public virtual string CreateAverageAll(string tableName,
            Field field,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }
            else
            {
                field.Type = AverageableClientTypeResolver?.Resolve(field.Type ?? DbSetting.AverageableType);
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Average(field, DbSetting, ConvertFieldResolver)
                .WriteText($"AS {"AverageValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateCount

        /// <summary>
        /// Creates a SQL Statement for 'Count' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count operation.</returns>
        public virtual string CreateCount(string tableName,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Count(null, DbSetting)
                .WriteText($"AS {"CountValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for 'CountAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public virtual string CreateCountAll(string tableName,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Count(null, DbSetting)
                .WriteText($"AS {"CountValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateDelete

        /// <summary>
        /// Creates a SQL Statement for 'Delete' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for delete operation.</returns>
        public virtual string CreateDelete(string tableName,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for 'DeleteAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public virtual string CreateDeleteAll(string tableName,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateExists

        /// <summary>
        /// Creates a SQL Statement for 'Exists' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for exists operation.</returns>
        public virtual string CreateExists(string tableName,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .TopFrom(1)
                .WriteText($"1 AS {("ExistsValue").AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateInsert

        /// <summary>
        /// Creates a SQL Statement for 'Insert' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public virtual string CreateInsert(string tableName,
            IEnumerable<Field> fields = null,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of insertable fields must not be null or empty for '{tableName}'.");
            }

            // Primary Key
            if (primaryField != null &&
                primaryField.HasDefaultValue == false &&
                !string.Equals(primaryField.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isPresent = fields
                    .FirstOrDefault(f =>
                        string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                if (isPresent == false)
                {
                    throw new PrimaryFieldNotFoundException($"Primary field '{primaryField.Name}' must be present from the list.");
                }
            }

            // Insertable fields
            var insertableFields = fields
                .Where(f =>
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(insertableFields, 0, DbSetting)
                .CloseParen()
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateInsertAll

        /// <summary>
        /// Creates a SQL Statement for 'InsertAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public virtual string CreateInsertAll(string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Validate the multiple statement execution
            ValidateMultipleStatementExecution(batchSize);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException("The list of fields cannot be null or empty.");
            }

            // Primary Key
            if (primaryField != null &&
                primaryField.HasDefaultValue == false &&
                !string.Equals(primaryField.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isPresent = fields
                    .FirstOrDefault(f =>
                        string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                if (isPresent == false)
                {
                    throw new PrimaryFieldNotFoundException($"As the primary field '{primaryField.Name}' is not an identity nor has a default value, it must be present on the insert operation.");
                }
            }

            // Insertable fields
            var insertableFields = fields
                .Where(f =>
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear();

            // Compose
            builder
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Select()
                .FieldsFrom(insertableFields, DbSetting)
                .From()
                .OpenParen()
                .Values();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .OpenParen()
                    .ParametersFrom(insertableFields, index, DbSetting)
                    .WriteText(
                        string.Concat(", ",
                            $"{DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index}"))
                    .CloseParen();

                if (index < batchSize - 1)
                {
                    builder
                        .WriteText(",");
                }
            }

            // Close
            builder
                .CloseParen()
                .As("T")
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .WriteText(string.Concat(", ", "__RepoDb_OrderColumn".AsQuoted(DbSetting)))
                .CloseParen()
                .OrderBy()
                .WriteText("__RepoDb_OrderColumn".AsQuoted(DbSetting))
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateMax

        /// <summary>
        /// Creates a SQL Statement for 'Max' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for maximum operation.</returns>
        public virtual string CreateMax(string tableName,
            Field field,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Max(field, DbSetting)
                .WriteText($"AS {"MaxValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateMaxAll

        /// <summary>
        /// Creates a SQL Statement for 'MaxAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for maximum-all operation.</returns>
        public virtual string CreateMaxAll(string tableName,
            Field field,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Max(field, DbSetting)
                .WriteText($"AS {"MaxValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateMin

        /// <summary>
        /// Creates a SQL Statement for 'Min' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for minimum operation.</returns>
        public virtual string CreateMin(string tableName,
            Field field,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Min(field, DbSetting)
                .WriteText($"AS {"MinValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateMinAll

        /// <summary>
        /// Creates a SQL Statement for 'MinAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for minimum-all operation.</returns>
        public virtual string CreateMinAll(string tableName,
            Field field,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Min(field, DbSetting)
                .WriteText($"AS {"MinValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateQuery

        /// <summary>
        /// Creates a SQL Statement for 'Query' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for query operation.</returns>
        public virtual string CreateQuery(string tableName,
            IEnumerable<Field> fields,
            QueryGroup? where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .TopFrom(top)
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateQueryAll

        /// <summary>
        /// Creates a SQL Statement for 'QueryAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for query operation.</returns>
        public virtual string CreateQueryAll(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<OrderField> orderBy = null,
            string? hints = null)
        {
            // Guard the target table
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OrderByFrom(orderBy, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateSum

        /// <summary>
        /// Creates a SQL Statement for 'Sum' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be summarized.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum operation.</returns>
        public virtual string CreateSum(string tableName,
            Field field,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Sum(field, DbSetting)
                .WriteText($"AS {"SumValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateSumAll

        /// <summary>
        /// Creates a SQL Statement for 'SumAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be summarized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum-all operation.</returns>
        public virtual string CreateSumAll(string tableName,
            Field field,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Sum(field, DbSetting)
                .WriteText($"AS {"SumValue".AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateTruncate

        /// <summary>
        /// Creates a SQL Statement for 'Truncate' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public virtual string CreateTruncate(string tableName)
        {
            // Guard the target table
            GuardTableName(tableName);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Truncate()
                .Table()
                .TableNameFrom(tableName, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateUpdate

        /// <summary>
        /// Creates a SQL Statement for 'Update' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for update operation.</returns>
        public virtual string CreateUpdate(string tableName,
            IEnumerable<Field> fields,
            QueryGroup? where = null,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Gets the updatable fields
            var updatableFields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Check if there are updatable fields
            if (updatableFields.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Update()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .Set()
                .FieldsAndParametersFrom(updatableFields, 0, DbSetting)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateUpdateAll

        /// <summary>
        /// Creates a SQL Statement for 'UpdateAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for update-all operation.</returns>
        public virtual string CreateUpdateAll(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Validate the multiple statement execution
            ValidateMultipleStatementExecution(batchSize);

            // Ensure the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of fields cannot be null or empty.");
            }

            // Check the qualifiers
            if (qualifiers?.Any() == true)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers.Where(field =>
                    fields?.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers.Any() == true)
                {
                    throw new InvalidQualifiersException($"The qualifiers '{unmatchesQualifiers.Select(field => field.Name).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }
            else
            {
                if (primaryField != null)
                {
                    // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                    var isPresent = fields.FirstOrDefault(f =>
                        string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                    // Throw if not present
                    if (isPresent == false)
                    {
                        throw new InvalidQualifiersException($"There are no qualifier field objects found for '{tableName}'. Ensure that the " +
                            $"primary field is present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                    }

                    // The primary is present, use it as a default if there are no qualifiers given
                    qualifiers = primaryField.AsField().AsEnumerable();
                }
                else
                {
                    // Throw exception, qualifiers are not defined
                    throw new NullReferenceException($"There are no qualifier field objects found for '{tableName}'.");
                }
            }

            // Gets the updatable fields
            fields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    qualifiers.FirstOrDefault(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

            // Check if there are updatable fields
            if (fields.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .Update()
                    .TableNameFrom(tableName, DbSetting)
                    .HintsFrom(hints)
                    .Set()
                    .FieldsAndParametersFrom(fields, index, DbSetting)
                    .WhereFrom(qualifiers, index, DbSetting)
                    .End();
            }

            // Return the query
            return builder.GetString();
        }

        #endregion

        #endregion

        #region Abstract

        #region CreateBatchQuery

        /// <summary>
        /// Creates a SQL Statement for 'BatchQuery' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public abstract string CreateBatchQuery(string tableName,
            IEnumerable<Field> fields,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup? where = null,
            string? hints = null);

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for 'Merge' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public abstract string CreateMerge(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field>? qualifiers = null,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null);

        #endregion

        #region CreateMergeAll

        /// <summary>
        /// Creates a SQL Statement for 'MergeAll' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public abstract string CreateMergeAll(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field>? qualifiers = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null);

        #endregion

        #region CreateSkipQuery

        /// <summary>
        /// Creates a SQL Statement for 'BatchQuery' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public abstract string CreateSkipQuery(string tableName,
            IEnumerable<Field> fields,
            int skip,
            int take,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup? where = null,
            string? hints = null);

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <exception cref="NullReferenceException"></exception>
        protected void GuardTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new NullReferenceException("The name of the table could be null.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected void GuardPrimary(DbField field)
        {
            if (field?.IsPrimary == false)
            {
                throw new InvalidOperationException("The field is not defined as primary.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected void GuardIdentity(DbField field)
        {
            if (field?.IsIdentity == false)
            {
                throw new InvalidOperationException("The field is not defined as identity.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hints"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected void GuardHints(string? hints = null)
        {
            if (!string.IsNullOrWhiteSpace(hints) && !DbSetting.AreTableHintsSupported)
            {
                throw new NotSupportedException("The table hints are not supported on this database provider statement builder.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchSize"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected void ValidateMultipleStatementExecution(int batchSize = Constant.DefaultBatchOperationSize)
        {
            if (DbSetting.IsMultiStatementExecutable == false && batchSize > 1)
            {
                throw new NotSupportedException($"Multiple execution is not supported based on the current database setting '{DbSetting.GetType().FullName}'. Consider setting the batchSize to 1.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primaryDbField"></param>
        /// <param name="identityDbField"></param>
        protected DbField GetReturnKeyColumnAsDbField(DbField primaryDbField,
            DbField identityDbField)
        {
            switch (GlobalConfiguration.Options.KeyColumnReturnBehavior)
            {
                case KeyColumnReturnBehavior.Primary:
                    return primaryDbField;
                case KeyColumnReturnBehavior.Identity:
                    return identityDbField;
                case KeyColumnReturnBehavior.PrimaryOrElseIdentity:
                    return primaryDbField ?? identityDbField;
                case KeyColumnReturnBehavior.IdentityOrElsePrimary:
                    return identityDbField ?? primaryDbField;
                default:
                    throw new InvalidOperationException(nameof(GlobalConfiguration.Options.KeyColumnReturnBehavior));
            }
        }

        #endregion
    }
}
