using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb.TestProject
{
    public class CustomStatementBuilder : IStatementBuilder
    {
        public string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, QueryGroup where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, QueryGroup where) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, QueryGroup queryGroup) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, QueryGroup where, bool? overrideIgnore = false) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, QueryGroup queryGroup, int? top = 0, IEnumerable<OrderField> orderBy = null) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, QueryGroup queryGroup) where TEntity : DataEntity
        {
            throw new NotImplementedException();
        }
    }
}
