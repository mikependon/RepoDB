using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using System.Linq;
using System.Text;

namespace RepoDb
{
    /// <summary>
    /// An object used to compose a SQL Query Statement.
    /// </summary>
    /// <typeparam name="TEntity">An entity where the SQL Query Statement is bound to.</typeparam>
    public sealed class QueryBuilder<TEntity> : IQueryBuilder<TEntity>
        where TEntity : DataEntity
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        /// Stringify the current object.
        /// </summary>
        /// <returns>
        /// Returns a string that corresponds to the composed SQL Query Statement. It uses the <i>GetString</i>
        /// method as its underlying method call.
        /// </returns>
        public override string ToString()
        {
            return GetString();
        }

        // Custom Methods

        /// <summary>
        /// Gets the string that corresponds to the composed SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public string GetString()
        {
            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Clears the current composed SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Clear()
        {
            _stringBuilder.Clear();
            return this;
        }

        /// <summary>
        /// Append a space to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Space()
        {
            return Append(" ");
        }

        /// <summary>
        /// Appends a new-line to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> NewLine()
        {
            return Append("\n");
        }

        /// <summary>
        /// Writes a custom text to the SQL Query Statement.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        /// <returns>The current instance.</returns>
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

        /// <summary>
        /// Appends a word DELETE word to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Delete()
        {
            return Append("DELETE");
        }

        /// <summary>
        /// Appends a character ";" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> End()
        {
            return Append(";");
        }

        /// <summary>
        /// Appends a word COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Count()
        {
            return Append("COUNT");
        }

        /// <summary>
        /// Appends a word COUNT_BIG to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> CountBig()
        {
            return Append("COUNT_BIG");
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Field(Field field)
        {
            return Append(field?.AsField());
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement by command.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Fields(Command command)
        {
            var fields = PropertyMapNameCache.Get<TEntity>(command).ToList();
            return Append(fields?.AsFields().Join(", "));
        }

        /// <summary>
        /// Append a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Fields(IEnumerable<Field> fields)
        {
            return Append(fields?.Select(f => f.AsField()).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields and parameters to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> FieldsAndParameters(Command command)
        {
            return Append(PropertyMapNameCache.Get<TEntity>(command)?.AsFieldsAndParameters().Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> FieldsAndParameters(IEnumerable<Field> fields)
        {
            return Append(fields?.AsFieldsAndParameters().Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields and parameters to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> FieldsAndAliasFields(Command command, string alias)
        {
            return Append(PropertyMapNameCache.Get<TEntity>(command)?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> FieldsAndAliasFields(IEnumerable<Field> fields, string alias)
        {
            return Append(fields?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a word FROM to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> From()
        {
            return Append("FROM");
        }

        /// <summary>
        /// Appends a word GROUP BY and a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> GroupBy(IEnumerable<Field> fields)
        {
            return Append($"GROUP BY {fields?.AsFields().Join(", ")}");
        }

        /// <summary>
        /// Appends a word HAVING COUNT and a conditional field to the SQL Query Statement.
        /// </summary>
        /// <param name="queryField">The conditional field object used for composition.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> HavingCount(QueryField queryField)
        {
            return Append($"HAVING COUNT({queryField.Field.AsField()}) {queryField.GetOperationText()} {queryField.AsParameter()}");
        }

        /// <summary>
        /// Appends a word INSERT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Insert()
        {
            return Append("INSERT");
        }

        /// <summary>
        /// Appends a word INTO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Into()
        {
            return Append("INTO");
        }

        /// <summary>
        /// Appends a word VALUES to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Values()
        {
            return Append("VALUES");
        }

        /// <summary>
        /// Appends a word ORDER BY and the stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="orderBy">The list of order fields to be stringified.</param>
        /// <param name="alias">The aliases to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> OrderBy(IEnumerable<OrderField> orderBy = null, string alias = null)
        {
            return (orderBy != null && orderBy.Any()) ?
                Append($"ORDER BY {orderBy.Select(orderField => orderField.AsField()).Join(", ")}") :
                this;
        }

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <param name="alias">The alias to be prepended.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> As(string alias = null)
        {
            return string.IsNullOrEmpty(alias) ? Append($"AS") : Append($"AS {alias}");
        }

        /// <summary>
        /// Appends a word WITH to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> With()
        {
            return Append("WITH");
        }

        /// <summary>
        /// Appends a word SET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Set()
        {
            return Append("SET");
        }

        /// <summary>
        /// Appends a word JOIN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Join()
        {
            return Append("JOIN");
        }

        /// <summary>
        /// Appends a stringified field as a joined qualifier to the SQL Query Statement with left and right aliases.
        /// </summary>
        /// <param name="field">The field to be stringified.</param>
        /// <param name="leftAlias">The left alias.</param>
        /// <param name="rightAlias">The right alias.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> JoinQualifiers(Field field, string leftAlias, string rightAlias)
        {
            return Append(field.AsJoinQualifier(leftAlias, rightAlias));
        }

        /// <summary>
        /// Appends a word MERGE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Merge()
        {
            return Append("MERGE");
        }

        /// <summary>
        /// Appends the mapped entity name to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Table(Command command)
        {
            return Append($"{ClassMapNameCache.Get<TEntity>(command)}");
        }

        /// <summary>
        /// Append the mapped properpties name to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Parameters(Command command)
        {
            var properties = PropertyMapNameCache.Get<TEntity>(command)?.ToList();
            return Append(properties?.AsParameters().Join(", "));
        }

        /// <summary>
        /// Append the stringified field parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Parameters(IEnumerable<Field> fields)
        {
            return Append(fields?.AsParameters().Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> ParametersAsFields(Command command)
        {
            var properties = PropertyMapNameCache.Get<TEntity>(command)?.ToList();
            return Append(properties?.AsParametersAsFields().Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> ParametersAsFields(IEnumerable<Field> fields)
        {
            return Append(fields?.AsParametersAsFields().Join(", "));
        }

        /// <summary>
        /// Appends a word SELECT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Select()
        {
            return Append("SELECT");
        }

        /// <summary>
        /// Appends a word TOP and row number to the SQL Query Statement.
        /// </summary>
        /// <param name="rows">The row number to be appended.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Top(int? rows = 0)
        {
            return rows > 0 ? Append($"TOP ({rows})") : this;
        }

        /// <summary>
        /// Appends a word UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Update()
        {
            return Append("UPDATE");
        }

        /// <summary>
        /// Appends a word USING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Using()
        {
            return Append("USING");
        }

        /// <summary>
        /// Appends a word WHERE and the stringified values of the Query Group to the SQL Query Statement.
        /// </summary>
        /// <param name="queryGroup">The query group to be stringified.</param>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Where(QueryGroup queryGroup)
        {
            return (queryGroup != null) ? Append($"WHERE {((QueryGroup)queryGroup).FixParameters().GetString()}") : this;
        }

        /// <summary>
        /// Appends a word ROW_NUMBER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> RowNumber()
        {
            return Append("ROW_NUMBER()");
        }

        /// <summary>
        /// Appends a word OVER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Over()
        {
            return Append("OVER");
        }

        /// <summary>
        /// Appends a word AND to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> And()
        {
            return Append("AND");
        }

        /// <summary>
        /// Appends a word OR to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Or()
        {
            return Append("OR");
        }

        /// <summary>
        /// Appends a character "(" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> OpenParen()
        {
            return Append("(");
        }

        /// <summary>
        /// Appends a character ")" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> CloseParen()
        {
            return Append(")");
        }

        /// <summary>
        /// Appends a word ON to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> On()
        {
            return Append("ON");
        }

        /// <summary>
        /// Appends a word IN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> In()
        {
            return Append("IN");
        }

        /// <summary>
        /// Appends a word BETWEEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Between()
        {
            return Append("BETWEEN");
        }

        /// <summary>
        /// Appends a word WHEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> When()
        {
            return Append("WHEN");
        }

        /// <summary>
        /// Appends a word NOT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Not()
        {
            return Append("NOT");
        }

        /// <summary>
        /// Appends a word MATCHED to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Matched()
        {
            return Append("MATCHED");
        }

        /// <summary>
        /// Appends a word THEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Then()
        {
            return Append("THEN");
        }

        /// <summary>
        /// Appends a word CASE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public IQueryBuilder<TEntity> Case()
        {
            return Append("CASE");
        }
    }
}