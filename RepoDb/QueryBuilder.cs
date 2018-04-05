using System;
using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    internal class QueryBuilder<TEntity> : IQueryBuilder<TEntity>
        where TEntity : IDataEntity
    {
        private string _statement;

        public QueryBuilder() { }

        public override string ToString()
        {
            return _statement.ToString();
        }

        private IQueryBuilder<TEntity> Append(string value)
        {
            _statement = !string.IsNullOrEmpty(_statement) ? $"{_statement} {value}" : value;
            return this;
        }

        public IQueryBuilder<TEntity> Delete()
        {
            return this.Append("DELETE ");
        }

        public IQueryBuilder<TEntity> End()
        {
            return this.Append(";");
        }

        public IQueryBuilder<TEntity> Fields()
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> FieldsAndParameters(Command command)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> From()
        {
            return this.Append("FROM ");
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
            return this.Append("INSERT ");
        }

        public IQueryBuilder<TEntity> Join()
        {
            return this.Append("JOIN ");
        }

        public IQueryBuilder<TEntity> JoinQualifiers(string leftAlias, string rightAlias)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> Merge(string alias)
        {
            return this.Append("MERGE ");
        }

        public IQueryBuilder<TEntity> Name()
        {
            return this.Append(DataEntityExtension.GetMappedName<TEntity>());
        }

        public IQueryBuilder<TEntity> OrderBy(IEnumerable<IOrderField> fields)
        {
            return this.Append("ORDER BY ");
        }

        public IQueryBuilder<TEntity> Parameters()
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> ParametersAsFields(Command command)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TEntity> Select()
        {
            return this.Append("SELECT ");
        }

        public IQueryBuilder<TEntity> Top(int rows)
        {
            return this.Append($"TOP ({rows}) ");
        }

        public IQueryBuilder<TEntity> Update()
        {
            return this.Append("UPDATE ");
        }

        public IQueryBuilder<TEntity> Using(string alias)
        {
            return this.Append("USING ");
        }

        public IQueryBuilder<TEntity> Where(IEnumerable<IQueryField> queryFields)
        {
            return this.Append("WHERE ");
        }

        public IQueryBuilder<TEntity> And()
        {
            return this.Append("AND ");
        }

        public IQueryBuilder<TEntity> Or()
        {
            return this.Append("OR ");
        }

        public IQueryBuilder<TEntity> OpenParen()
        {
            return this.Append("(");
        }

        public IQueryBuilder<TEntity> CloseParen()
        {
            return this.Append(") ");
        }

        public IQueryBuilder<TEntity> On()
        {
            return this.Append("ON ");
        }
    }
}