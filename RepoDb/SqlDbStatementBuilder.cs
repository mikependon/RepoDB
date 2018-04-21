using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RepoDb
{
    public sealed class SqlDbStatementBuilder : IStatementBuilder
    {
        public SqlDbStatementBuilder() { }

        public string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int page, int rowsPerBatch, IEnumerable<IOrderField> orderBy)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .With()
                .WriteText("CTE")
                .As()
                .OpenParen()
                .Select()
                .RowNumber()
                .Over()
                .OpenParen()
                .OrderBy(orderBy)
                .CloseParen()
                .As("[RowNumber],")
                .Fields(Command.Select)
                .From()
                .Table()
                .Where(where)
                .CloseParen()
                .Select()
                .Fields(Command.Select)
                .From()
                .WriteText("CTE")
                .WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
                .OrderBy(orderBy)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .Count()
                .WriteText("(*) AS [Counted]")
                .From()
                .Table()
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(*) AS [Counted]")
                .From()
                .Table()
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .Table()
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields,
            IQueryGroup where, bool? overrideIgnore = false)
            where TEntity : IDataEntity
        {
            if (overrideIgnore == false)
            {
                var properties = PropertyCache.Get<TEntity>(Command.Update)
                    .Select(property => property.GetMappedName());
                var unmatches = fields?.Where(field =>
                    properties?.FirstOrDefault(property =>
                        field.Name.ToLower() == property.ToLower()) == null);
                if (unmatches?.Count() > 0)
                {
                    throw new InvalidOperationException($"The following columns ({unmatches.Select(field => field.AsField()).Join(", ")}) " +
                        $"are not updatable for entity ({DataEntityExtension.GetMappedName<TEntity>()}).");
                }
            }
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .Table()
                .Set()
                .FieldsAndParameters(fields)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            var primary = PrimaryPropertyCache.Get<TEntity>();
            queryBuilder
                .Clear()
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
                var result = primary.IsIdentity() ? "SCOPE_IDENTITY()" : $"@{primary.GetMappedName()}";
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
                var primaryKey = PrimaryPropertyCache.Get<TEntity>();
                if (primaryKey != null)
                {
                    qualifiers = new Field(primaryKey.Name).AsEnumerable();
                }
            }
            queryBuilder
                .Clear()
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

        public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0, IEnumerable<IOrderField> orderBy = null)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .Top(top)
                .Fields(Command.Select)
                .From()
                .Table()
                .Where(where)
                .OrderBy(orderBy)
                .End();
            return queryBuilder.GetString();
        }

        public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .Table()
                .Set()
                .FieldsAndParameters(Command.Update)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }
    }
}
