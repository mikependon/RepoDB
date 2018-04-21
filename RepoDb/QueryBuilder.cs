using System;
using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using System.Linq;
using System.Text;

namespace RepoDb
{
    public class QueryBuilder<TEntity> : IQueryBuilder<TEntity>
        where TEntity : IDataEntity
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override string ToString()
        {
            return GetString();
        }

        // Custom Methods

        public string GetString()
        {
            return _stringBuilder.ToString();
        }

        public IQueryBuilder<TEntity> Clear()
        {
            _stringBuilder.Clear();
            return this;
        }

        public IQueryBuilder<TEntity> Space()
        {
            return Append(" ");
        }

        public IQueryBuilder<TEntity> NewLine()
        {
            return Append("\n");
        }

        public IQueryBuilder<TEntity> WriteText(string text)
        {
            return Append(text);
        }

        private IQueryBuilder<TEntity> Append(string value)
        {
            _stringBuilder.Append(_stringBuilder.Length > 0 ? $" {value}" : value);
            return this;
        }

        // Basic Methods

        public IQueryBuilder<TEntity> Delete()
        {
            return Append("DELETE");
        }

        public IQueryBuilder<TEntity> End()
        {
            return Append(";");
        }

        public IQueryBuilder<TEntity> Count()
        {
            return Append("COUNT");
        }

        public IQueryBuilder<TEntity> CountBig()
        {
            return Append("COUNT_BIG");
        }

        public IQueryBuilder<TEntity> Field(IField field)
        {
            return Append(field?.AsField());
        }

        public IQueryBuilder<TEntity> Fields(Command command)
        {
            var fields = PropertyMapNameCache.Get<TEntity>(command).ToList();
            return Append(fields?.AsFields().Join(", "));
        }

        public IQueryBuilder<TEntity> Fields(IEnumerable<IField> fields)
        {
            return Append(fields?.Select(f => f.AsField()).Join(", "));
        }

        public IQueryBuilder<TEntity> FieldsAndParameters(Command command)
        {
            return Append(PropertyMapNameCache.Get<TEntity>(command)?.AsFieldsAndParameters().Join(", "));
        }

        public IQueryBuilder<TEntity> FieldsAndParameters(IEnumerable<IField> fields)
        {
            return Append(fields?.AsFieldsAndParameters().Join(", "));
        }

        public IQueryBuilder<TEntity> FieldsAndAliasFields(Command command, string alias)
        {
            return Append(PropertyMapNameCache.Get<TEntity>(command)?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        public IQueryBuilder<TEntity> FieldsAndAliasFields(IEnumerable<IField> fields, string alias)
        {
            return Append(fields?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        public IQueryBuilder<TEntity> From()
        {
            return Append("FROM");
        }

        public IQueryBuilder<TEntity> GroupBy(IEnumerable<IField> fields)
        {
            return Append($"GROUP BY {fields?.AsFields().Join(", ")}");
        }

        public IQueryBuilder<TEntity> HavingCount(IQueryField queryField)
        {
            return Append($"HAVING COUNT({queryField.Field.AsField()}) {queryField.GetOperationText()} {queryField.AsParameter()}");
        }

        public IQueryBuilder<TEntity> Insert()
        {
            return Append("INSERT");
        }

        public IQueryBuilder<TEntity> Into()
        {
            return Append("INTO");
        }

        public IQueryBuilder<TEntity> Values()
        {
            return Append("VALUES");
        }

        public IQueryBuilder<TEntity> OrderBy(IEnumerable<IOrderField> orderBy = null, string alias = null)
        {
            return (orderBy != null && orderBy.Any()) ?
                Append($"ORDER BY {orderBy.Select(orderField => orderField.AsField()).Join(", ")}") :
                this;
        }

        public IQueryBuilder<TEntity> As(string alias = null)
        {
            return string.IsNullOrEmpty(alias) ? Append($"AS") : Append($"AS {alias}");
        }

        public IQueryBuilder<TEntity> With()
        {
            return Append("WITH");
        }

        public IQueryBuilder<TEntity> Set()
        {
            return Append("SET");
        }

        public IQueryBuilder<TEntity> Join()
        {
            return Append("JOIN");
        }

        public IQueryBuilder<TEntity> JoinQualifiers(IField field, string leftAlias, string rightAlias)
        {
            return Append(field.AsJoinQualifier(leftAlias, rightAlias));
        }

        public IQueryBuilder<TEntity> Merge()
        {
            return Append("MERGE");
        }

        public IQueryBuilder<TEntity> Table(Command command)
        {
            return Append($"{ClassMapNameCache.Get<TEntity>(command)}");
        }

        public IQueryBuilder<TEntity> Parameters(Command command)
        {
            var properties = PropertyMapNameCache.Get<TEntity>(command)?.ToList();
            return Append(properties?.AsParameters().Join(", "));
        }

        public IQueryBuilder<TEntity> Parameters(IEnumerable<IField> fields)
        {
            return Append(fields?.AsParameters().Join(", "));
        }

        public IQueryBuilder<TEntity> ParametersAsFields(Command command)
        {
            var properties = PropertyMapNameCache.Get<TEntity>(command)?.ToList();
            return Append(properties?.AsParametersAsFields().Join(", "));
        }

        public IQueryBuilder<TEntity> ParametersAsFields(IEnumerable<IField> fields)
        {
            return Append(fields?.AsParametersAsFields().Join(", "));
        }

        public IQueryBuilder<TEntity> Select()
        {
            return Append("SELECT");
        }

        public IQueryBuilder<TEntity> Top(int? rows = 0)
        {
            return rows > 0 ? Append($"TOP ({rows})") : this;
        }

        public IQueryBuilder<TEntity> Update()
        {
            return Append("UPDATE");
        }

        public IQueryBuilder<TEntity> Using()
        {
            return Append("USING");
        }

        public IQueryBuilder<TEntity> Where(IQueryGroup queryGroup)
        {
            return (queryGroup != null) ? Append($"WHERE {queryGroup.Fix().GetString()}") : this;
        }

        public IQueryBuilder<TEntity> RowNumber()
        {
            return Append("ROW_NUMBER()");
        }

        public IQueryBuilder<TEntity> Over()
        {
            return Append("OVER");
        }

        public IQueryBuilder<TEntity> And()
        {
            return Append("AND");
        }

        public IQueryBuilder<TEntity> Or()
        {
            return Append("OR");
        }

        public IQueryBuilder<TEntity> OpenParen()
        {
            return Append("(");
        }

        public IQueryBuilder<TEntity> CloseParen()
        {
            return Append(")");
        }

        public IQueryBuilder<TEntity> On()
        {
            return Append("ON");
        }

        public IQueryBuilder<TEntity> In()
        {
            return Append("IN");
        }

        public IQueryBuilder<TEntity> Between()
        {
            return Append("BETWEEN");
        }

        public IQueryBuilder<TEntity> When()
        {
            return Append("WHEN");
        }

        public IQueryBuilder<TEntity> Not()
        {
            return Append("NOT");
        }

        public IQueryBuilder<TEntity> Matched()
        {
            return Append("MATCHED");
        }

        public IQueryBuilder<TEntity> Then()
        {
            return Append("THEN");
        }
    }
}