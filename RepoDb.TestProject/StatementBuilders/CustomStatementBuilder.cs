using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb.TestProject.StatementBuilders
{
    public class CustomStatementBuilder : IStatementBuilder
    {
        public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup queryGroup) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, bool? overrideIgnore = false) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, bool? overrideIgnore = false) where TEntity : DataEntity
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

        public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup queryGroup, int? top = 0, IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateTruncate<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup queryGroup) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }
    }
}
