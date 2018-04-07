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

        public QueryBuilder() { }

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
            return this.Append(text);
        }

        private IQueryBuilder<TEntity> Append(string value)
        {
            _statement = !string.IsNullOrEmpty(_statement) ? $"{_statement} {value}" : value;
            return this;
        }

        // Basic Methods

        public IQueryBuilder<TEntity> Delete()
        {
            return this.Append("DELETE");
        }

        public IQueryBuilder<TEntity> End()
        {
            return this.Append(";");
        }

        public IQueryBuilder<TEntity> Fields()
        {
            var properties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)?.ToList();
            return this.Append($"{properties?.AsFields().Join(", ")}");
        }

        public IQueryBuilder<TEntity> Field(IField field)
        {
            return this.Append($"[{field.Name}]");
        }

        public IQueryBuilder<TEntity> FieldsAndParameters(Command command)
        {
            return this.Append(DataEntityExtension.GetPropertiesFor<TEntity>(command)?.AsFieldsAndParameters().Join(", "));
        }

        public IQueryBuilder<TEntity> From()
        {
            return this.Append("FROM");
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
            return this.Append("INSERT");
        }

        public IQueryBuilder<TEntity> Into()
        {
            return this.Append("INTO");
        }

        public IQueryBuilder<TEntity> Values()
        {
            return this.Append("VALUES");
        }

        public IQueryBuilder<TEntity> As()
        {
            return this.Append("AS");
        }

        public IQueryBuilder<TEntity> Set()
        {
            return this.Append("SET");
        }

        public IQueryBuilder<TEntity> Join()
        {
            return this.Append("JOIN");
        }

        public IQueryBuilder<TEntity> JoinQualifiers(string leftAlias, string rightAlias)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> Merge(string alias)
        {
            return this.Append("MERGE");
        }

        public IQueryBuilder<TEntity> Table()
        {
            return this.Append($"{DataEntityExtension.GetMappedName<TEntity>()}");
        }

        public IQueryBuilder<TEntity> OrderBy(IEnumerable<IOrderField> fields)
        {
            return this.Append("ORDER BY");
        }

        public IQueryBuilder<TEntity> Parameters()
        {
            var properties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)?.ToList();
            return this.Append($"{properties?.AsParameters().Join(", ")}");
        }

        public IQueryBuilder<TEntity> ParametersAsFields(Command command)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> Select()
        {
            return this.Append("SELECT");
        }

        public IQueryBuilder<TEntity> Top(int rows)
        {
            return this.Append($"TOP ({rows})");
        }

        public IQueryBuilder<TEntity> Update()
        {
            return this.Append("UPDATE");
        }

        public IQueryBuilder<TEntity> Using(string alias)
        {
            return this.Append("USING");
        }

        public IQueryBuilder<TEntity> Where(IQueryGroup queryGroup)
        {
            return this.Append($"WHERE {queryGroup.GetString()}");
        }

        public IQueryBuilder<TEntity> And()
        {
            return this.Append("AND");
        }

        public IQueryBuilder<TEntity> Or()
        {
            return this.Append("OR");
        }

        public IQueryBuilder<TEntity> OpenParen()
        {
            return this.Append("(");
        }

        public IQueryBuilder<TEntity> CloseParen()
        {
            return this.Append(")");
        }

        public IQueryBuilder<TEntity> On()
        {
            return this.Append("ON");
        }
    }
}