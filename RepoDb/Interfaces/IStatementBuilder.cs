using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface IStatementBuilder
    {
        string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity;
        string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields, IQueryGroup where, bool? overrideIgnore = false)
            where TEntity : IDataEntity;
        string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder)
            where TEntity : IDataEntity;
        string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers)
            where TEntity : IDataEntity;
        string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0, IEnumerable<IOrderField> orderBy = null)
            where TEntity : IDataEntity;
        string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity;
    }
}
