using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    public sealed class SqlDbStatementBuilder : IStatementBuilder
    {
        public SqlDbStatementBuilder() {}

        public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup queryGroup)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Delete()
                .From()
                .Table()
                .Where(queryGroup)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder) where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
            queryBuilder
                .Insert()
                .Into()
                .Table()
                .OpenParen()
                .Fields(Command.Insert)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(Command.Insert)
                .CloseParen()
                .End();
            if (primary != null)
            {
                var result = primary.IsIdentity() ? "SCOPE_IDENTITY()" : $"@{primary.Name}";
                queryBuilder
                    .Select()
                    .WriteText(result)
                    .As("[Result]")
                    .End();
            }
            return queryBuilder.GetString();
        }

        public string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            if (qualifiers == null)
            {
                var primaryKey = DataEntityExtension.GetPrimaryProperty<TEntity>();
                if (primaryKey != null)
                {
                    qualifiers = new Field(primaryKey.Name).AsEnumerable();
                }
            }
            queryBuilder
                // MERGE T USING S
                .Merge()
                .Table()
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFields(Command.None)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T"))
                            .Join($" {Constant.And.ToUpper()} "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .Fields(Command.Insert)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(Command.Insert)
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFields(Command.Update, "S")
                .End();
            return queryBuilder.GetString();
        }

        public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup queryGroup, int? top = 0, IEnumerable<IOrderField> orderBy = null)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Select()
                .Top(top)
                .Fields(Command.Select)
                .From()
                .Table()
                .Where(queryGroup)
                .OrderBy(orderBy)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup queryGroup)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Update()
                .Table()
                .Set()
                .FieldsAndParameters(Command.Update)
                .Where(queryGroup)
                .End();
            return queryBuilder.GetString();
        }
    }
}
