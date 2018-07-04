using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Linq;
using System.Text;

namespace RepoDb
{
    /// <summary>
    /// An object used to compose a SQL Query Statement.
    /// </summary>
    /// <typeparam name="TEntity">An entity where the SQL Query Statement is bound to.</typeparam>
    public class QueryBuilder<TEntity> where TEntity : DataEntity
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
        public QueryBuilder<TEntity> Clear()
        {
            _stringBuilder.Clear();
            return this;
        }

        /// <summary>
        /// Append a space to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Space()
        {
            return Append(" ");
        }

        /// <summary>
        /// Appends a new-line to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> NewLine()
        {
            return Append("\n");
        }

        /// <summary>
        /// Writes a custom text to the SQL Query Statement.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> WriteText(string text)
        {
            return Append(text);
        }

        private QueryBuilder<TEntity> Append(string value)
        {
            _stringBuilder.Append(_stringBuilder.Length > 0 ? $" {value}" : value);
            return this;
        }

        // Basic Methods

        /// <summary>
        /// Appends a word DELETE word to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Delete()
        {
            return Append("DELETE");
        }

        /// <summary>
        /// Appends a character ";" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> End()
        {
            return Append(";");
        }

        /// <summary>
        /// Appends a word COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Count()
        {
            return Append("COUNT");
        }

        /// <summary>
        /// Appends a word COUNT_BIG to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> CountBig()
        {
            return Append("COUNT_BIG");
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Field(Field field)
        {
            return Append(field?.AsField());
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement by command.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> FieldsFrom(Command command)
        {
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.Select(property => property.GetMappedName().AsQuoted(true));
            return Append(fields?.ToList().AsFields().Join(", "));
        }

        /// <summary>
        /// Append a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> FieldsFrom(IEnumerable<Field> fields)
        {
            return Append(fields?.Select(f => f.AsField()).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields and parameters to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> FieldsAndParametersFrom(Command command)
        {
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.Select(property => property.GetMappedName());
            return Append(fields?.AsFieldsAndParameters().Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> FieldsAndParametersFrom(IEnumerable<Field> fields)
        {
            return Append(fields?.AsFieldsAndParameters().Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="command">The mapped command where to get all the fields and parameters to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> FieldsAndAliasFieldsFrom(Command command, string alias)
        {
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.Select(property => property.GetMappedName());
            return Append(fields?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> FieldsAndAliasFieldsFrom(IEnumerable<Field> fields, string alias)
        {
            return Append(fields?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a word FROM to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> From()
        {
            return Append("FROM");
        }

        /// <summary>
        /// Appends a word GROUP BY and a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> GroupByFrom(IEnumerable<Field> fields)
        {
            return Append($"GROUP BY {fields?.AsFields().Join(", ")}");
        }

        /// <summary>
        /// Appends a word HAVING COUNT and a conditional field to the SQL Query Statement.
        /// </summary>
        /// <param name="queryField">The conditional field object used for composition.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> HavingCountFrom(QueryField queryField)
        {
            return Append($"HAVING COUNT({queryField.Field.AsField()}) {queryField.GetOperationText()} {queryField.AsParameter()}");
        }

        /// <summary>
        /// Appends a word INSERT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Insert()
        {
            return Append("INSERT");
        }

        /// <summary>
        /// Appends a word GROUP BY to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> GroupBy()
        {
            return Append("GROUP BY");
        }

        /// <summary>
        /// Appends a word HAVING COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> HavingCount()
        {
            return Append("HAVING COUNT");
        }

        /// <summary>
        /// Appends a word INTO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Into()
        {
            return Append("INTO");
        }

        /// <summary>
        /// Appends a word VALUES to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Values()
        {
            return Append("VALUES");
        }

        /// <summary>
        /// Appends a word ORDER BY and the stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="orderBy">The list of order fields to be stringified.</param>
        /// <param name="alias">The aliases to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> OrderByFrom(IEnumerable<OrderField> orderBy = null, string alias = null)
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
        public QueryBuilder<TEntity> As(string alias = null)
        {
            return string.IsNullOrEmpty(alias) ? Append($"AS") : Append($"AS {alias}");
        }

        /// <summary>
        /// Appends a word WITH to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> With()
        {
            return Append("WITH");
        }

        /// <summary>
        /// Appends a word SET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Set()
        {
            return Append("SET");
        }

        /// <summary>
        /// Appends a word JOIN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Join()
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
        public QueryBuilder<TEntity> JoinQualifiersFrom(Field field, string leftAlias, string rightAlias)
        {
            return Append(field.AsJoinQualifier(leftAlias, rightAlias));
        }

        /// <summary>
        /// Appends a word MERGE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Merge()
        {
            return Append("MERGE");
        }

        /// <summary>
        /// Appends a word TABLE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Table()
        {
            return Append("TABLE");
        }

        /// <summary>
        /// Appends the mapped entity name to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> TableFrom(Command command)
        {
            return Append($"{DataEntityExtension.GetMappedName<TEntity>(command).AsQuoted(true)}");
        }

        /// <summary>
        /// Append the mapped properpties name to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> ParametersFrom(Command command)
        {
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.Select(property => property.GetMappedName());
            return Append(fields?.AsParameters().Join(", "));
        }

        /// <summary>
        /// Append the stringified field parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> ParametersFrom(IEnumerable<Field> fields)
        {
            return Append(fields?.AsParameters().Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement based on the mapped command.
        /// </summary>
        /// <param name="command">The command where the mapping is defined.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> ParametersAsFieldsFrom(Command command)
        {
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(command)?.Select(property => property.GetMappedName());
            return Append(fields?.AsParametersAsFields().Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> ParametersAsFieldsFrom(IEnumerable<Field> fields)
        {
            return Append(fields?.AsParametersAsFields().Join(", "));
        }

        /// <summary>
        /// Appends a word SELECT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Select()
        {
            return Append("SELECT");
        }

        /// <summary>
        /// Appends a word TOP to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Top()
        {
            return Append("TOP");
        }

        /// <summary>
        /// Appends a word ORDER BY to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> OrderBy()
        {
            return Append("ORDER BY");
        }

        /// <summary>
        /// Appends a word WHERE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Where()
        {
            return Append("WHERE");
        }

        /// <summary>
        /// Appends a word TOP and row number to the SQL Query Statement.
        /// </summary>
        /// <param name="rows">The row number to be appended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> TopFrom(int? rows = 0)
        {
            return rows > 0 ? Append($"TOP ({rows})") : this;
        }

        /// <summary>
        /// Appends a word UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Update()
        {
            return Append("UPDATE");
        }

        /// <summary>
        /// Appends a word USING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Using()
        {
            return Append("USING");
        }

        /// <summary>
        /// Appends a word WHERE and the stringified values of the Query Group to the SQL Query Statement.
        /// </summary>
        /// <param name="queryGroup">The query group to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> WhereFrom(QueryGroup queryGroup)
        {
            return (queryGroup != null) ? Append($"WHERE {queryGroup.FixParameters().GetString()}") : this;
        }

        /// <summary>
        /// Appends a word ROW_NUMBER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> RowNumber()
        {
            return Append("ROW_NUMBER()");
        }

        /// <summary>
        /// Appends a word OVER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Over()
        {
            return Append("OVER");
        }

        /// <summary>
        /// Appends a word AND to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> And()
        {
            return Append("AND");
        }

        /// <summary>
        /// Appends a word OR to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Or()
        {
            return Append("OR");
        }

        /// <summary>
        /// Appends a character "(" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> OpenParen()
        {
            return Append("(");
        }

        /// <summary>
        /// Appends a character ")" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> CloseParen()
        {
            return Append(")");
        }

        /// <summary>
        /// Appends a word ON to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> On()
        {
            return Append("ON");
        }

        /// <summary>
        /// Appends a word IN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> In()
        {
            return Append("IN");
        }

        /// <summary>
        /// Appends a word BETWEEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Between()
        {
            return Append("BETWEEN");
        }

        /// <summary>
        /// Appends a word WHEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> When()
        {
            return Append("WHEN");
        }

        /// <summary>
        /// Appends a word NOT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Not()
        {
            return Append("NOT");
        }

        /// <summary>
        /// Appends a word MATCHED to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Matched()
        {
            return Append("MATCHED");
        }

        /// <summary>
        /// Appends a word THEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Then()
        {
            return Append("THEN");
        }

        /// <summary>
        /// Appends a word CASE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Case()
        {
            return Append("CASE");
        }

        /// <summary>
        /// Appends a word TRUNCATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder<TEntity> Truncate()
        {
            return Append("TRUNCATE");
        }

    }
}