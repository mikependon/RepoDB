using System.Collections.Generic;
using RepoDb.Extensions;
using System.Linq;
using System.Text;

namespace RepoDb
{
    /// <summary>
    /// An object used to compose a SQL Query Statement.
    /// </summary>
    public class QueryBuilder
    {
        // A StringBuilder's capacity grows dynamically as required (e.g. during append operations), but there's a 
        // perfomance penalty to be paid every time this happens (memory allocation + copy). The initial capacity
        // of a StringBuilder buffer is only 16 characters by default - too small to hold any meaningful query string,
        // so let's increase this to somthing more sensible. This should improve overall performance at the expense
        // of higher memory usage for short queries.

        //TODO: Tune this value
        private const int INITIAL_STRINGBUILDER_CAPACITY = 256;
        private readonly StringBuilder m_stringBuilder = new StringBuilder(INITIAL_STRINGBUILDER_CAPACITY);

        /// <summary>
        /// Stringify the current object.
        /// </summary>
        /// <returns>
        /// Returns a string that corresponds to the composed SQL Query Statement. It uses the <see cref="GetString"/>
        /// method as its underlying method call.
        /// </returns>
        public override string ToString()
        {
            return GetString();
        }

        // Custom Methods

        /// <summary>
        /// Gets the string that corresponds to the composed SQL Query Statement.
        /// Starts at index 1 to drop the leading space.
        /// </summary>
        /// <returns>The current instance.</returns>
        public string GetString()
        {
            if (m_stringBuilder.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                return m_stringBuilder.ToString(1, m_stringBuilder.Length - 1);
            }
        }

        /// <summary>
        /// Clears the current composed SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Clear()
        {
            m_stringBuilder.Clear();
            return this;
        }

        /// <summary>
        /// Append a space to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Space()
        {
            return Append(" ");
        }

        /// <summary>
        /// Appends a line terminator to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder NewLine()
        {
            m_stringBuilder.AppendLine();
            return this;
        }

        /// <summary>
        /// Writes a custom text to the SQL Query Statement.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WriteText(string text)
        {
            return Append(text);
        }

        private QueryBuilder Append(string value)
        {
            m_stringBuilder.Append(string.Concat(" ", value));
            return this;
        }

        // Basic Methods

        /// <summary>
        /// Appends a word DELETE word to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Delete()
        {
            return Append("DELETE");
        }

        /// <summary>
        /// Appends a character ";" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder End()
        {
            return Append(";");
        }

        /// <summary>
        /// Appends a word COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Count()
        {
            return Append("COUNT");
        }

        /// <summary>
        /// Appends a word COUNT_BIG to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder CountBig()
        {
            return Append("COUNT_BIG");
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldFrom(Field field)
        {
            return Append(field?.Name);
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsFrom<TEntity>()
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => property.GetUnquotedMappedName());
            return Append(fields?.AsFields().Join(", "));
        }

        /// <summary>
        /// Append a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsFrom(IEnumerable<Field> fields)
        {
            return Append(fields?.Select(f => f.Name).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndParametersFrom<TEntity>(int index = 0, string prefix = Constant.DefaultParameterPrefix)
            where TEntity : class
        {
            return Append(FieldCache.Get<TEntity>()?.AsFieldsAndParameters(index, prefix).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndParametersFrom(IEnumerable<Field> fields, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return Append(fields?.AsFieldsAndParameters(index, prefix).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndAliasFieldsFrom<TEntity>(string alias)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => property.GetUnquotedMappedName());
            return Append(fields?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndAliasFieldsFrom(IEnumerable<Field> fields, string alias)
        {
            return Append(fields?.AsFieldsAndAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <returns>The current instance.</returns>
        public QueryBuilder AsAliasFieldsFrom<TEntity>(string alias)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => property.GetUnquotedMappedName());
            return Append(fields?.AsAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder AsAliasFieldsFrom(IEnumerable<Field> fields, string alias)
        {
            return Append(fields?.AsAliasFields(alias).Join(", "));
        }

        /// <summary>
        /// Appends a word FROM to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder From()
        {
            return Append("FROM");
        }

        /// <summary>
        /// Appends a word GROUP BY and a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder GroupByFrom(IEnumerable<Field> fields)
        {
            return Append(string.Concat("GROUP BY ", fields?.AsFields().Join(", ")));
        }

        /// <summary>
        /// Appends a word HAVING COUNT and a conditional field to the SQL Query Statement.
        /// </summary>
        /// <param name="queryField">The conditional field object used for composition.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder HavingCountFrom(QueryField queryField, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return Append(string.Concat("HAVING COUNT(", queryField.Field.Name, ") ", queryField.GetOperationText(), ", ", queryField.AsParameter(index, prefix)));
        }

        /// <summary>
        /// Appends a word INSERT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Insert()
        {
            return Append("INSERT");
        }

        /// <summary>
        /// Appends a word GROUP BY to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder GroupBy()
        {
            return Append("GROUP BY");
        }

        /// <summary>
        /// Appends a word HAVING COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder HavingCount()
        {
            return Append("HAVING COUNT");
        }

        /// <summary>
        /// Appends a word INTO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Into()
        {
            return Append("INTO");
        }

        /// <summary>
        /// Appends a word VALUES to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Values()
        {
            return Append("VALUES");
        }

        /// <summary>
        /// Appends a word ORDER BY and the stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="orderBy">The list of order fields to be stringified.</param>
        /// <param name="alias">The aliases to be prepended for each field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder OrderByFrom(IEnumerable<OrderField> orderBy = null, string alias = null)
        {
            return (orderBy != null && orderBy.Any()) ?
                Append(string.Concat("ORDER BY ", orderBy.Select(orderField => orderField.AsField()).Join(", "))) :
                this;
        }

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <param name="alias">The alias to be prepended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder As(string alias = null)
        {
            return string.IsNullOrEmpty(alias) ? Append("AS") : Append(string.Concat("AS ", alias));
        }

        /// <summary>
        /// Appends a word WITH to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder With()
        {
            return Append("WITH");
        }

        /// <summary>
        /// Appends a word SET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Set()
        {
            return Append("SET");
        }

        /// <summary>
        /// Appends a word JOIN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Join()
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
        public QueryBuilder JoinQualifiersFrom(Field field, string leftAlias, string rightAlias)
        {
            return Append(field.AsJoinQualifier(leftAlias, rightAlias));
        }

        /// <summary>
        /// Appends a word MERGE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Merge()
        {
            return Append("MERGE");
        }

        /// <summary>
        /// Appends a word TABLE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Table()
        {
            return Append("TABLE");
        }

        /// <summary>
        /// Appends the target name to the SQL Query Statement.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TableNameFrom(string tableName)
        {
            return Append(tableName?.AsQuoted(true));
        }

        /// <summary>
        /// Appends the mapped entity name to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <returns>The current instance.</returns>
        public QueryBuilder TableNameFrom<TEntity>()
            where TEntity : class
        {
            return Append(ClassMappedNameCache.Get<TEntity>());
        }

        /// <summary>
        /// Append the mapped properpties name to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersFrom<TEntity>(int index = 0, string prefix = Constant.DefaultParameterPrefix)
            where TEntity : class
        {
            return ParametersFrom(FieldCache.Get<TEntity>(), index, prefix);
        }

        /// <summary>
        /// Append the stringified field parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersFrom(IEnumerable<Field> fields, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return Append(fields?.AsParameters(index, prefix).Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersAsFieldsFrom<TEntity>(int index = 0, string prefix = Constant.DefaultParameterPrefix)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => property.GetUnquotedMappedName());
            return Append(fields?.AsParametersAsFields(index, prefix).Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="prefix">The prefix to the parameters.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersAsFieldsFrom(IEnumerable<Field> fields, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return Append(fields?.AsParametersAsFields(index, prefix).Join(", "));
        }

        /// <summary>
        /// Appends a word SELECT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Select()
        {
            return Append("SELECT");
        }

        /// <summary>
        /// Appends a word TOP to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Top()
        {
            return Append("TOP");
        }

        /// <summary>
        /// Appends a word ORDER BY to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder OrderBy()
        {
            return Append("ORDER BY");
        }

        /// <summary>
        /// Appends a word WHERE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Where()
        {
            return Append("WHERE");
        }

        /// <summary>
        /// Appends a word TOP and row number to the SQL Query Statement.
        /// </summary>
        /// <param name="rows">The row number to be appended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TopFrom(int? rows = 0)
        {
            return rows > 0 ? Append(string.Concat("TOP (", rows, ")")) : this;
        }

        /// <summary>
        /// Appends a word UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Update()
        {
            return Append("UPDATE");
        }

        /// <summary>
        /// Appends a word USING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Using()
        {
            return Append("USING");
        }

        /// <summary>
        /// Appends a word WHERE and the stringified values of the <see cref="QueryGroup"/> to the SQL Query Statement.
        /// </summary>
        /// <param name="queryGroup">The query group to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WhereFrom(QueryGroup queryGroup, int index = 0)
        {
            return (queryGroup != null) ? Append(string.Concat("WHERE ", queryGroup.GetString(index))) : this;
        }

        /// <summary>
        /// Appends a word WHERE and the stringified values of the <see cref="QueryGroup"/> to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WhereFrom(IEnumerable<Field> fields, int index = 0)
        {
            return (fields?.Any() == true) ? Append(string.Concat("WHERE (",
                fields.Select(f => f.UnquotedName.AsFieldAndParameter(index)).Join(" AND "), ")")) : this;
        }

        /// <summary>
        /// Appends a word ROW_NUMBER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder RowNumber()
        {
            return Append("ROW_NUMBER()");
        }

        /// <summary>
        /// Appends a word OVER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Over()
        {
            return Append("OVER");
        }

        /// <summary>
        /// Appends a word AND to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder And()
        {
            return Append("AND");
        }

        /// <summary>
        /// Appends a word OR to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Or()
        {
            return Append("OR");
        }

        /// <summary>
        /// Appends a character "(" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder OpenParen()
        {
            return Append("(");
        }

        /// <summary>
        /// Appends a character ")" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder CloseParen()
        {
            return Append(")");
        }

        /// <summary>
        /// Appends a word ON to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder On()
        {
            return Append("ON");
        }

        /// <summary>
        /// Appends a word IN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder In()
        {
            return Append("IN");
        }

        /// <summary>
        /// Appends a word BETWEEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Between()
        {
            return Append("BETWEEN");
        }

        /// <summary>
        /// Appends a word WHEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder When()
        {
            return Append("WHEN");
        }

        /// <summary>
        /// Appends a word NOT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Not()
        {
            return Append("NOT");
        }

        /// <summary>
        /// Appends a word MATCHED to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Matched()
        {
            return Append("MATCHED");
        }

        /// <summary>
        /// Appends a word THEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Then()
        {
            return Append("THEN");
        }

        /// <summary>
        /// Appends a word CASE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Case()
        {
            return Append("CASE");
        }

        /// <summary>
        /// Appends a word TRUNCATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Truncate()
        {
            return Append("TRUNCATE");
        }

        /// <summary>
        /// Appends the hints to the SQL Query Statement.
        /// </summary>
        /// <param name="hints">The hints to be appended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder HintsFrom(string hints)
        {
            return string.IsNullOrEmpty(hints) == false ? Append(hints) : this;
        }

        /// <summary>
        /// Appends a word MAX to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Max()
        {
            return Append("MAX");
        }

        /// <summary>
        /// Appends a word MAX and the field to the SQL Query Statement, otherwise an empty string.
        /// </summary>
        /// <param name="field">The target field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder MaxFrom(Field field)
        {
            return field != null ? Append(string.Concat("MAX(", field.Name, ")")) : this;
        }

        /// <summary>
        /// Appends a word MIN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Min()
        {
            return Append("MIN");
        }

        /// <summary>
        /// Appends a word MIN and the field to the SQL Query Statement, otherwise an empty string.
        /// </summary>
        /// <param name="field">The target field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder MinFrom(Field field)
        {
            return field != null ? Append(string.Concat("MIN(", field.Name, ")")) : this;
        }

        /// <summary>
        /// Appends a word AVG to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Avg()
        {
            return Append("AVG");
        }

        /// <summary>
        /// Appends a word AVG and the field to the SQL Query Statement, otherwise an empty string.
        /// </summary>
        /// <param name="field">The target field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder AvgFrom(Field field)
        {
            return field != null ? Append(string.Concat("AVG(", field.Name, ")")) : this;
        }

    }
}