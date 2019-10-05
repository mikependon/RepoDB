using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.StatementBuilders
{
    public class SqLiteStatementBuilder : IStatementBuilder
    {
        public string CreateBatchQuery(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null, QueryGroup where = null, string hints = null)
        {
            throw new NotImplementedException();
        }

        public string CreateCount(QueryBuilder queryBuilder, string tableName, QueryGroup where = null, string hints = null)
        {
            throw new NotImplementedException();
        }

        public string CreateCountAll(QueryBuilder queryBuilder, string tableName, string hints = null)
        {
            throw new NotImplementedException();
        }

        public string CreateDelete(QueryBuilder queryBuilder, string tableName, QueryGroup where = null)
        {
            throw new NotImplementedException();
        }

        public string CreateDeleteAll(QueryBuilder queryBuilder, string tableName)
        {
            throw new NotImplementedException();
        }

        public string CreateInsert(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields = null, DbField primaryField = null, DbField identityField = null)
        {
            throw new NotImplementedException();
        }

        public string CreateInsertAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields = null, int batchSize = 10, DbField primaryField = null, DbField identityField = null)
        {
            throw new NotImplementedException();
        }

        public string CreateMerge(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers = null, DbField primaryField = null, DbField identityField = null)
        {
            throw new NotImplementedException();
        }

        public string CreateMergeAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, int batchSize = 10, DbField primaryField = null, DbField identityField = null)
        {
            throw new NotImplementedException();
        }

        public string CreateQuery(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, QueryGroup where = null, IEnumerable<OrderField> orderBy = null, int? top = null, string hints = null)
        {
            throw new NotImplementedException();
        }

        #region QueryAll

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
                        string.Equals(orderField.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

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

        public string CreateTruncate(QueryBuilder queryBuilder, string tableName)
        {
            throw new NotImplementedException();
        }

        public string CreateUpdate(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, QueryGroup where = null, DbField primaryField = null, DbField identityField = null)
        {
            throw new NotImplementedException();
        }

        public string CreateUpdateAll(QueryBuilder queryBuilder, string tableName, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, int batchSize = 10, DbField primaryField = null, DbField identityField = null)
        {
            throw new NotImplementedException();
        }

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
                throw new InvalidOperationException("The field is not defined as identity.");
            }
        }

        #endregion
    }
}
