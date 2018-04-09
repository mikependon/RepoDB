using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    public interface IStatementBuilder
    {
        string CreateDelete<TEntity>(IQueryGroup queryGroup)
            where TEntity : IDataEntity;
        string CreateInsert<TEntity>()
            where TEntity : IDataEntity;
        string CreateMerge<TEntity>(IEnumerable<IField> qualifiers)
            where TEntity : IDataEntity;
        string CreateQuery<TEntity>(IQueryGroup queryGroup, int? top = 0, IEnumerable<IOrderField> orderBy = null)
            where TEntity : IDataEntity;
        string CreateUpdate<TEntity>(IQueryGroup queryGroup)
            where TEntity : IDataEntity;
    }
}
