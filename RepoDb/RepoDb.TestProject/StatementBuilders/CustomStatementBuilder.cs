using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb.TestProject.StatementBuilders
{
    public class CustomStatementBuilder : IStatementBuilder
    {
        public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null, int? page = default(int?), int? rowsPerBatch = default(int?), IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null, bool? overrideIgnore = false) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null, IEnumerable<Field> qualifiers = null, bool? overrideIgnore = false) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null, QueryGroup where = null, bool? overrideIgnore = false) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null, IEnumerable<OrderField> orderBy = null, int? top = default(int?)) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateTruncate<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }
    }
}
