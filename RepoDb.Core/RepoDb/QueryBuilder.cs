using System.Collections.Generic;
using RepoDb.Extensions;
using System.Linq;
using System.Text;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class used to compose a SQL Query Statement.
    /// </summary>
    public class QueryBuilder
    {
        // A StringBuilder's capacity grows dynamically as required (e.g. during append operations), but there's a 
        // performance penalty to be paid every time this happens (memory allocation + copy). The initial capacity
        // of a StringBuilder buffer is only 16 characters by default - too small to hold any meaningful query string,
        // so let's increase this to something more sensible. This should improve overall performance at the expense
        // of higher memory usage for short queries.

        //TODO: Tune this value
        private const int INITIAL_STRINGBUILDER_CAPACITY = 256;
        private readonly StringBuilder stringBuilder = new StringBuilder(INITIAL_STRINGBUILDER_CAPACITY);

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
            return stringBuilder.Length > 0 ? stringBuilder.ToString(1, stringBuilder.Length - 1) : null;
        }

        /// <summary>
        /// Clears the current composed SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Clear()
        {
            stringBuilder.Clear();
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
            stringBuilder.AppendLine();
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
            stringBuilder.Append(string.Concat(" ", value));
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
        /// Appends a word AVG to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Average(Field field,
            IDbSetting dbSetting)
        {
            return Average(field, dbSetting, null);
        }

        /// <summary>
        /// Appends a word AVG to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="convertResolver">The converter that is being used to convert the <see cref="Field"/> object before the aggregation.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Average(Field field,
            IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertResolver)
        {
            var name = convertResolver == null ? field.Name.AsField(dbSetting) :
                convertResolver.Resolve(field, dbSetting);
            return Append(string.Concat("AVG (", name, ")"));
        }

        /// <summary>
        /// Appends a word MIN to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Min(Field field,
            IDbSetting dbSetting)
        {
            return Min(field, dbSetting, null);
        }

        /// <summary>
        /// Appends a word MIN to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="convertResolver">The converter that is being used to convert the <see cref="Field"/> object before the aggregation.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Min(Field field,
            IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertResolver)
        {
            var name = convertResolver == null ? field.Name.AsField(dbSetting) :
                convertResolver.Resolve(field, dbSetting);
            return Append(string.Concat("MIN (", name, ")"));
        }

        /// <summary>
        /// Appends a word MAX to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Max(Field field,
            IDbSetting dbSetting)
        {
            return Max(field, dbSetting, null);
        }

        /// <summary>
        /// Appends a word MAX to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="convertResolver">The converter that is being used to convert the <see cref="Field"/> object before the aggregation.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Max(Field field,
            IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertResolver)
        {
            var name = convertResolver == null ? field.Name.AsField(dbSetting) :
                convertResolver.Resolve(field, dbSetting);
            return Append(string.Concat("MAX (", name, ")"));
        }

        /// <summary>
        /// Appends a word SUM to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Sum(Field field,
            IDbSetting dbSetting)
        {
            return Sum(field, dbSetting, null);
        }

        /// <summary>
        /// Appends a word SUM to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="convertResolver">The converter that is being used to convert the <see cref="Field"/> object before the aggregation.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Sum(Field field,
            IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertResolver)
        {
            var name = convertResolver == null ? field.Name.AsField(dbSetting) :
                convertResolver.Resolve(field, dbSetting);
            return Append(string.Concat("SUM (", name, ")"));
        }

        /// <summary>
        /// Appends a word COUNT to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Count(Field field,
            IDbSetting dbSetting)
        {
            var name = field != null ? field.Name.AsField(dbSetting) : "*";
            return Append(string.Concat("COUNT (", name, ")"));
        }

        /// <summary>
        /// Appends a word COUNT_BIG to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder CountBig(Field field,
            IDbSetting dbSetting)
        {
            var name = field != null ? field.Name.AsField(dbSetting) : "*";
            return Append(string.Concat("COUNT_BIG (", name, ")"));
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
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsFrom<TEntity>(IDbSetting dbSetting)
            where TEntity : class
        {
            return FieldsFrom(PropertyCache.Get<TEntity>()?.AsFields(), dbSetting);
        }

        /// <summary>
        /// Append a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsFrom(IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            return Append(fields?.Select(f => f.Name).AsFields(dbSetting).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndParametersFrom<TEntity>(int index,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return Append(FieldCache.Get<TEntity>()?.AsFieldsAndParameters(index, dbSetting).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndParametersFrom(IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return Append(fields?.AsFieldsAndParameters(index, dbSetting).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="leftAlias">The alias to be prepended for each field in the left.</param>
        /// <param name="rightAlias">The alias to be prepended for each field in the right.</param>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndAliasFieldsFrom<TEntity>(string leftAlias,
            string rightAlias,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => property.GetMappedName());
            return Append(fields?.AsFieldsAndAliasFields(leftAlias, rightAlias, dbSetting).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="leftAlias">The alias to be prepended for each field in the left.</param>
        /// <param name="rightAlias">The alias to be prepended for each field in the right.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndAliasFieldsFrom(IEnumerable<Field> fields,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting)
        {
            return Append(fields?.AsFieldsAndAliasFields(leftAlias, rightAlias, dbSetting).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder AsAliasFieldsFrom<TEntity>(string alias,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => property.GetMappedName());
            return Append(fields?.AsAliasFields(alias, dbSetting).Join(", "));
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder AsAliasFieldsFrom(IEnumerable<Field> fields,
            string alias,
            IDbSetting dbSetting)
        {
            return Append(fields?.AsAliasFields(alias, dbSetting).Join(", "));
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
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder GroupByFrom(IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            return Append(string.Concat("GROUP BY ", fields?.AsFields(dbSetting).Join(", ")));
        }

        /// <summary>
        /// Appends a word HAVING COUNT and a conditional field to the SQL Query Statement.
        /// </summary>
        /// <param name="queryField">The conditional field object used for composition.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder HavingCountFrom(QueryField queryField,
            int index,
            IDbSetting dbSetting)
        {
            return Append(string.Concat("HAVING COUNT(", queryField.Field.Name, ") ", queryField.GetOperationText(), ", ", queryField.AsParameter(index, dbSetting)));
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
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder OrderByFrom(IEnumerable<OrderField> orderBy,
            IDbSetting dbSetting)
        {
            return OrderByFrom(orderBy, null, dbSetting);
        }

        /// <summary>
        /// Appends a word ORDER BY and the stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="orderBy">The list of order fields to be stringified.</param>
        /// <param name="alias">The aliases to be prepended for each field.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder OrderByFrom(IEnumerable<OrderField> orderBy,
            string alias,
            IDbSetting dbSetting)
        {
            return orderBy?.Any() == true ?
                Append(string.Concat("ORDER BY ", orderBy.Select(orderField => orderField.AsField(alias, dbSetting)).Join(", "))) :
                this;
        }

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder As()
        {
            return As(null);
        }

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <param name="alias">The alias to be prepended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder As(string alias)
        {
            return string.IsNullOrWhiteSpace(alias) ? Append("AS") : Append(string.Concat("AS ", alias));
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
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder JoinQualifiersFrom(Field field,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting)
        {
            return Append(field.AsJoinQualifier(leftAlias, rightAlias, dbSetting));
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
        /// Appends the mapped entity name to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TableNameFrom<TEntity>(IDbSetting dbSetting)
            where TEntity : class
        {
            return TableNameFrom(ClassMappedNameCache.Get<TEntity>(), dbSetting);
        }

        /// <summary>
        /// Appends the target name to the SQL Query Statement.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TableNameFrom(string tableName,
            IDbSetting dbSetting)
        {
            return Append(tableName?.AsQuoted(true, dbSetting));
        }

        /// <summary>
        /// Append the mapped properties name to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersFrom<TEntity>(int index,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return ParametersFrom(FieldCache.Get<TEntity>(), index, dbSetting);
        }

        /// <summary>
        /// Append the stringified field parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersFrom(IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return Append(fields?.AsParameters(index, dbSetting).Join(", "));
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersAsFieldsFrom<TEntity>(int index,
            IDbSetting dbSetting)
            where TEntity : class
        {
            var fields = PropertyCache
                .Get<TEntity>()?
                .Select(property => property.AsField());
            return ParametersAsFieldsFrom(fields, index, dbSetting);
        }

        /// <summary>
        /// Append the stringified parameter as fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersAsFieldsFrom(IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return Append(fields?.AsParametersAsFields(index, dbSetting).Join(", "));
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
        /// Appends a word TOP with the number of rows to the SQL Query Statement.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TopFrom(int? rows)
        {
            return rows > 0 ? Append(string.Concat("TOP (", rows, ")")) : this;
        }

        /// <summary>
        /// Appends a word LIMIT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Limit()
        {
            return Limit(null);
        }

        /// <summary>
        /// Appends a word LIMIT to the SQL Query Statement.
        /// </summary>
        /// <param name="take">The number of rows to be taken.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Limit(int? take)
        {
            if (take > 0)
            {
                return Append(string.Concat("LIMIT ", take));
            }
            else
            {
                return Append("LIMIT");
            }
        }

        /// <summary>
        /// Appends a word LIMIT (Rows, Skip) with the number of rows to be skipped and return the SQL Query Statement.
        /// </summary>
        /// <param name="take">The number of rows to be taken.</param>
        /// <param name="skip">The number of rows to be skipped.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder LimitTake(int? take,
            int? skip)
        {
            return skip > 0 ? Append(string.Concat("LIMIT ", skip, ", ", take)) : Append(string.Concat("LIMIT ", take));
        }

        /// <summary>
        /// Appends a word LIMIT with the number of rows to be skipped and return the SQL Query Statement.
        /// </summary>
        /// <param name="take">The number of rows to be taken.</param>
        /// <param name="skip">The number of rows to be skipped.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder LimitOffset(int? take,
            int? skip)
        {
            return skip > 0 ? Append(string.Concat("LIMIT ", take, " OFFSET ", skip)) : Append(string.Concat("LIMIT ", take));
        }

        /// <summary>
        /// Appends a word OFFSET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Offset()
        {
            return Offset(null);
        }

        /// <summary>
        /// Appends a word OFFSET to the SQL Query Statement.
        /// </summary>
        /// <param name="skip">The number of rows to be skipped.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Offset(int? skip)
        {
            if (skip > 0)
            {
                return Append(string.Concat("OFFSET ", skip));
            }
            else
            {
                return Append("OFFSET");
            }
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
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WhereFrom(QueryGroup queryGroup,
            IDbSetting dbSetting)
        {
            return WhereFrom(queryGroup, 0, dbSetting);
        }

        /// <summary>
        /// Appends a word WHERE and the stringified values of the <see cref="QueryGroup"/> to the SQL Query Statement.
        /// </summary>
        /// <param name="queryGroup">The query group to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WhereFrom(QueryGroup queryGroup,
            int index,
            IDbSetting dbSetting)
        {
            return (queryGroup?.GetFields(true)?.Any() == true) ? Append(string.Concat("WHERE ", queryGroup.GetString(index, dbSetting))) : this;
        }

        /// <summary>
        /// Appends a word WHERE and the stringified values of the <see cref="QueryGroup"/> to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WhereFrom(IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting)
        {
            return (fields?.Any() == true) ? Append(string.Concat("WHERE (",
                fields.Select(f => f.Name.AsFieldAndParameter(index, dbSetting)).Join(" AND "), ")")) : this;
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
            return string.IsNullOrWhiteSpace(hints) == false ? Append(hints) : this;
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
        /// Appends a word ASC to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Ascending()
        {
            return Append("ASC");
        }

        /// <summary>
        /// Appends a word DESC to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Descending()
        {
            return Append("DESC");
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

        /// <summary>
        /// Appends a word REPLACE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Replace()
        {
            return Append("REPLACE");
        }

        /// <summary>
        /// Appends a word RETURNING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Returning()
        {
            return Append("RETURNING");
        }

        /// <summary>
        /// Appends a word CONFLICT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Conflict()
        {
            return Append("CONFLICT");
        }

        /// <summary>
        /// Appends a word ON CONFLICT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder OnConflict()
        {
            return Append("ON CONFLICT");
        }

        /// <summary>
        /// Appends a word ON CONFLICT ON (fieldname) to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The instances of the <see cref="Field"/> objects to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder OnConflict(IEnumerable<Field> fields
            , IDbSetting dbSetting)
        {
            var fieldNames = fields?
                .Select(f => f.Name.AsQuoted(dbSetting))
                .Join(", ");
            return Append(string.Concat("ON CONFLICT (", fieldNames, ")"));
        }

        /// <summary>
        /// Appends a word DO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Do()
        {
            return Append("DO");
        }

        /// <summary>
        /// Appends a word DO NOTHING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder DoNothing()
        {
            return Append("DO NOTHING");
        }

        /// <summary>
        /// Appends a word DO UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder DoUpdate()
        {
            return Append("DO UPDATE");
        }
    }
}