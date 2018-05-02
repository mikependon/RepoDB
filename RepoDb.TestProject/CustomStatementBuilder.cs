using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb.TestProject
{
    public class CustomStatementBuilder : IStatementBuilder
    {
        public string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int page, int rowsPerBatch, IEnumerable<IOrderField> orderBy = null) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup queryGroup) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields, IQueryGroup where, bool? overrideIgnore = false) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup queryGroup, int? top = 0, IEnumerable<IOrderField> orderBy = null) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup queryGroup) where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }
    }
}
