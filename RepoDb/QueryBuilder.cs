using System;
using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb
{
    internal class QueryBuilder<TEntity> : IQueryBuilder<TEntity>
        where TEntity : IDataEntity
    {
        private string _statement;
        
        public override string ToString()
        {
            return GetString();
        }

        // Custom Methods

        public string GetString()
        {
            return _statement.ToString();
        }

        public IQueryBuilder<TEntity> Trim()
        {
            _statement = _statement.Trim();
            return this;
        }

        public IQueryBuilder<TEntity> Space()
        {
            _statement = $"{_statement} ";
            return this;
        }

        public IQueryBuilder<TEntity> NewLine()
        {
            _statement = $"{_statement}\n";
            return this;
        }

        public IQueryBuilder<TEntity> WriteText(string text)
        {
            return Append(text);
        }

        private IQueryBuilder<TEntity> Append(string value)
        {
            _statement = !string.IsNullOrEmpty(_statement) ? $"{_statement} {value}" : value;
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

        public IQueryBuilder<TEntity> Fields(Command command)
        {
            var properties = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.ToList();
            return Append($"{properties?.AsFields().Join(", ")}");
        }

        public IQueryBuilder<TEntity> Field(IField field)
        {
            return Append($"[{field.Name}]");
        }

        public IQueryBuilder<TEntity> FieldsAndParameters(Command command)
        {
            return Append(DataEntityExtension.GetPropertiesFor<TEntity>(command)?.AsFieldsAndParameters().Join(", "));
        }

        public IQueryBuilder<TEntity> FieldsAndAliasFields(Command command, string alias)
        {
            return Append(DataEntityExtension.GetPropertiesFor<TEntity>(command)?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        public IQueryBuilder<TEntity> From()
        {
            return Append("FROM");
        }

        public IQueryBuilder<TEntity> GroupBy(IEnumerable<Field> fields)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> HavingCount(IQueryField queryField)
        {
            throw new NotImplementedException();
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

        public IQueryBuilder<TEntity> Order(IEnumerable<IOrderField> orderFields = null, string alias = null)
        {
            return (orderFields != null && orderFields.Any()) ?
                Append($"ORDER BY {orderFields.Select(orderField => orderField.AsField()).Join(", ")}") :
                this;
        }

        public IQueryBuilder<TEntity> As(string alias)
        {
            return Append($"AS {alias}");
        }

        public IQueryBuilder<TEntity> Set()
        {
            return Append("SET");
        }

        public IQueryBuilder<TEntity> Join()
        {
            return Append("JOIN");
        }

        public IQueryBuilder<TEntity> JoinQualifiers(string leftAlias, string rightAlias)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> Merge()
        {
            return Append("MERGE");
        }

        public IQueryBuilder<TEntity> Table()
        {
            return Append($"{DataEntityExtension.GetMappedName<TEntity>()}");
        }

        public IQueryBuilder<TEntity> Parameters(Command command)
        {
            var properties = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.ToList();
            return Append($"{properties?.AsParameters().Join(", ")}");
        }

        public IQueryBuilder<TEntity> ParametersAsFields(Command command)
        {
            var properties = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.ToList();
            return Append($"{properties?.AsParametersAsFields().Join(", ")}");
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
            return (queryGroup != null) ? Append($"WHERE {queryGroup.FixParameters().GetString()}") : this;
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