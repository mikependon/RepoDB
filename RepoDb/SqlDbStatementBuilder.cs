using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb
{
    public sealed class SqlDbStatementBuilder : IStatementBuilder
    {
        public SqlDbStatementBuilder() {}

        public string CreateDelete<TEntity>(IQueryGroup queryGroup)
            where TEntity : IDataEntity
        {
            var queryBuilder = new QueryBuilder<TEntity>();
            queryBuilder
                .Delete()
                .From()
                .Table()
                .Where(queryGroup)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateInsert<TEntity>() where TEntity : IDataEntity
        {
            var queryBuilder = new QueryBuilder<TEntity>();
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
            queryBuilder
                .Insert()
                .Into()
                .Table()
                .OpenParen()
                .Fields()
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters()
                .CloseParen()
                .End();
            if (primary != null)
            {
                var result = primary.IsIdentity() ? "SCOPE_IDENTITY()" : $"@{primary.Name}";
                queryBuilder
                    .Select()
                    .WriteText(result)
                    .As()
                    .WriteText("[Result]")
                    .End();
            }
            return queryBuilder.GetString();
        }

        public string CreateMerge<TEntity>(IEnumerable<IField> qualifiers)
            where TEntity : IDataEntity
        {
            throw new NotImplementedException();
        }

        public string CreateQuery<TEntity>(IQueryGroup queryGroup)
            where TEntity : IDataEntity
        {
            var queryBuilder = new QueryBuilder<TEntity>();
            queryBuilder
                .Select()
                .Fields()
                .From()
                .Table()
                .Where(queryGroup)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateUpdate<TEntity>(IQueryGroup queryGroup)
            where TEntity : IDataEntity
        {
            var queryBuilder = new QueryBuilder<TEntity>();
            queryBuilder
                .Update()
                .Table()
                .Set()
                .FieldsAndParameters(Enumerations.Command.Update)
                .Where(queryGroup)
                .End();
            return queryBuilder.GetString();
        }
    }
}
