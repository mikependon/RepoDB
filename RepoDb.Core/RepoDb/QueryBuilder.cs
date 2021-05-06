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
        private readonly StringBuilder stringBuilder = new(INITIAL_STRINGBUILDER_CAPACITY);

        /// <summary>
        /// Stringify the current object.
        /// </summary>
        /// <returns>
        /// Returns a string that corresponds to the composed SQL Query Statement. It uses the <see cref="GetString"/>
        /// method as its underlying method call.
        /// </returns>
        public override string ToString() => GetString();

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
        public QueryBuilder Space() => Append(' ');

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
        public QueryBuilder WriteText(string text) => Append(text);

        private QueryBuilder Append(string value, bool spaceBefore = true)
        {
            if (string.IsNullOrWhiteSpace(value)) return this;

            if (spaceBefore) Space();
            
            stringBuilder.Append(value);
            
            return this;
        }
        
        private QueryBuilder Append(char value)
        {
            stringBuilder.Append(value);
            
            return this;
        }

        private QueryBuilder AppendJoin(IEnumerable<string> values, string separator = ", ", bool spaceBefore = true)
        {
            if (values.IsNullOrEmpty()) return this;
            
            if (spaceBefore) Space();
            
            stringBuilder
#if NET5_0             
                .AppendJoin(separator, values);
#else
                .Append(values.Join(separator));
#endif            
            
            return this;
        }

        // Basic Methods

        /// <summary>
        /// Appends a word DELETE word to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Delete() => Append("DELETE");

        /// <summary>
        /// Appends a character ";" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder End() => Append(";");

        /// <summary>
        /// Appends a word AVG to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Average(Field field, IDbSetting dbSetting) => 
            Average(field, dbSetting, null);

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
            var name = convertResolver == null
                ? field.Name.AsField(dbSetting)
                : convertResolver.Resolve(field, dbSetting);

            return Append("AVG (")
                .Append(name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word MIN to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Min(Field field, IDbSetting dbSetting) => 
            Min(field, dbSetting, null);

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
            var name = convertResolver == null
                ? field.Name.AsField(dbSetting)
                : convertResolver.Resolve(field, dbSetting);
            
            return Append("MIN (")
                .Append(name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word MAX to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Max(Field field, IDbSetting dbSetting) => 
            Max(field, dbSetting, null);

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
            var name = convertResolver == null
                ? field.Name.AsField(dbSetting)
                : convertResolver.Resolve(field, dbSetting);
            
            return Append("MAX (")
                .Append(name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word SUM to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> object to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Sum(Field field, IDbSetting dbSetting) => 
            Sum(field, dbSetting, null);

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
            var name = convertResolver == null
                ? field.Name.AsField(dbSetting)
                : convertResolver.Resolve(field, dbSetting);
            
            return Append("SUM (")
                .Append(name, false)
                .Append(')');
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
            
            return Append("COUNT (")
                .Append(name, false)
                .Append(')');
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
            
            return Append("COUNT_BIG (")
                .Append(name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="field">The list of fields to be stringified.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldFrom(Field field) => Append(field?.Name);

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsFrom<TEntity>(IDbSetting dbSetting)
            where TEntity : class =>
            FieldsFrom(PropertyCache.Get<TEntity>()?.AsFields(), dbSetting);

        /// <summary>
        /// Append a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsFrom(IEnumerable<Field> fields, IDbSetting dbSetting) => 
            AppendJoin(fields?.Select(f => f.Name).AsFields(dbSetting));

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndParametersFrom<TEntity>(int index, IDbSetting dbSetting) 
            where TEntity : class =>
            FieldsAndParametersFrom(FieldCache.Get<TEntity>(), index, dbSetting);

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndParametersFrom(IEnumerable<Field> fields, int index, IDbSetting dbSetting) =>
            AppendJoin(fields?.AsFieldsAndParameters(index, dbSetting));

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
            return AppendJoin(fields?.AsFieldsAndAliasFields(leftAlias, rightAlias, dbSetting));
        }

        /// <summary>
        /// Appends a stringified fields and parameters to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="leftAlias">The alias to be prepended for each field in the left.</param>
        /// <param name="rightAlias">The alias to be prepended for each field in the right.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder FieldsAndAliasFieldsFrom(IEnumerable<Field> fields, string leftAlias, string rightAlias, IDbSetting dbSetting) =>
            AppendJoin(fields?.AsFieldsAndAliasFields(leftAlias, rightAlias, dbSetting));

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
            return AppendJoin(fields?.AsAliasFields(alias, dbSetting));
        }

        /// <summary>
        /// Appends a stringified fields to the SQL Query Statement by command with aliases.
        /// </summary>
        /// <param name="fields">The list fields to be stringified.</param>
        /// <param name="alias">The alias to be prepended for each field.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder AsAliasFieldsFrom(IEnumerable<Field> fields, string alias, IDbSetting dbSetting) =>
            AppendJoin(fields?.AsAliasFields(alias, dbSetting));

        /// <summary>
        /// Appends a word FROM to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder From() => Append("FROM");

        /// <summary>
        /// Appends a word GROUP BY and a stringified fields to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The fields to be stringified.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder GroupByFrom(IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            if (fields.IsNullOrEmpty()) return this;
            
            return Append("GROUP BY")
                .AppendJoin(fields.AsFields(dbSetting));
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
            return Append("HAVING COUNT(")
                .Append(queryField.Field.Name, false)
                .Append(')')
                .Append(queryField.Operation.GetText())
                .Append(',')
                .Append(queryField.AsParameter(index, dbSetting));
        }

        /// <summary>
        /// Appends a word INSERT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Insert() => Append("INSERT");

        /// <summary>
        /// Appends a word GROUP BY to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder GroupBy() => Append("GROUP BY");

        /// <summary>
        /// Appends a word HAVING COUNT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder HavingCount() => Append("HAVING COUNT");

        /// <summary>
        /// Appends a word INTO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Into() => Append("INTO");

        /// <summary>
        /// Appends a word VALUES to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Values() => Append("VALUES");

        /// <summary>
        /// Appends a word ORDER BY and the stringified fields to the SQL Query Statement with aliases.
        /// </summary>
        /// <param name="orderBy">The list of order fields to be stringified.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder OrderByFrom(IEnumerable<OrderField> orderBy, IDbSetting dbSetting) => 
            OrderByFrom(orderBy, null, dbSetting);

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
            if (orderBy.IsNullOrEmpty()) return this;

            return Append("ORDER BY")
                .AppendJoin(orderBy.Select(orderField => orderField.AsField(alias, dbSetting)));
        }

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder As() => As(null);

        /// <summary>
        /// Appends a word AS to the SQL Query Statement with alias.
        /// </summary>
        /// <param name="alias">The alias to be prepended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder As(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return Append("AS");
            }

            return Append("AS")
                .Append(alias);
        }

        /// <summary>
        /// Appends a word WITH to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder With() => Append("WITH");

        /// <summary>
        /// Appends a word SET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Set() => Append("SET");

        /// <summary>
        /// Appends a word JOIN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Join() => Append("JOIN");

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
        public QueryBuilder Merge() => Append("MERGE");

        /// <summary>
        /// Appends a word TABLE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Table() => Append("TABLE");

        /// <summary>
        /// Appends the mapped entity name to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TableNameFrom<TEntity>(IDbSetting dbSetting)
            where TEntity : class =>
            TableNameFrom(ClassMappedNameCache.Get<TEntity>(), dbSetting);

        /// <summary>
        /// Appends the target name to the SQL Query Statement.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TableNameFrom(string tableName, IDbSetting dbSetting) => 
            Append(tableName?.AsQuoted(true, dbSetting));

        /// <summary>
        /// Append the mapped properties name to the SQL Query Statement.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity object bound for the SQL Statement to be created.</typeparam>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersFrom<TEntity>(int index, IDbSetting dbSetting)
            where TEntity : class =>
            ParametersFrom(FieldCache.Get<TEntity>(), index, dbSetting);

        /// <summary>
        /// Append the stringified field parameters to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The list of fields to be stringified.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder ParametersFrom(IEnumerable<Field> fields, int index, IDbSetting dbSetting) => 
            AppendJoin(fields?.AsParameters(index, dbSetting));

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
        public QueryBuilder ParametersAsFieldsFrom(IEnumerable<Field> fields, int index, IDbSetting dbSetting) => 
            AppendJoin(fields?.AsParametersAsFields(index, dbSetting));

        /// <summary>
        /// Appends a word SELECT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Select() => Append("SELECT");

        /// <summary>
        /// Appends a word TOP to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Top() => Append("TOP");

        /// <summary>
        /// Appends a word TOP with the number of rows to the SQL Query Statement.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder TopFrom(int? rows)
        {
            if (rows > 0)
            {
                return Append("TOP (")
                    .Append(rows.ToString(), false)
                    .Append(')');
            }

            return this;
        }

        /// <summary>
        /// Appends a word LIMIT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Limit() => Limit(null);

        /// <summary>
        /// Appends a word LIMIT to the SQL Query Statement.
        /// </summary>
        /// <param name="take">The number of rows to be taken.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Limit(int? take)
        {
            if (take > 0)
            {
                return Append("LIMIT")
                    .Append(take.ToString());
            }

            return Append("LIMIT");
        }

        /// <summary>
        /// Appends a word LIMIT (Rows, Skip) with the number of rows to be skipped and return the SQL Query Statement.
        /// </summary>
        /// <param name="take">The number of rows to be taken.</param>
        /// <param name="skip">The number of rows to be skipped.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder LimitTake(int? take, int? skip)
        {
            if (skip > 0)
            {
                return Append("LIMIT")
                    .Append(skip.ToString())
                    .Append(',')
                    .Append(take.ToString());
            }

            return Append("LIMIT")
                .Append(take.ToString());
        }

        /// <summary>
        /// Appends a word LIMIT with the number of rows to be skipped and return the SQL Query Statement.
        /// </summary>
        /// <param name="take">The number of rows to be taken.</param>
        /// <param name="skip">The number of rows to be skipped.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder LimitOffset(int? take, int? skip)
        {
            if (skip > 0)
            {
                return Append("LIMIT")
                    .Append(take.ToString())
                    .Append("OFFSET")
                    .Append(skip.ToString());
            }
            else
            {
                return Append("LIMIT")
                    .Append(take.ToString());
            }
        }

        /// <summary>
        /// Appends a word OFFSET to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Offset() => Offset(null);

        /// <summary>
        /// Appends a word OFFSET to the SQL Query Statement.
        /// </summary>
        /// <param name="skip">The number of rows to be skipped.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder Offset(int? skip)
        {
            if (skip > 0)
            {
                return Append("OFFSET")
                    .Append(skip.ToString());
            }

            return Append("OFFSET");
        }

        /// <summary>
        /// Appends a word ORDER BY to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder OrderBy() => Append("ORDER BY");

        /// <summary>
        /// Appends a word WHERE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Where() => Append("WHERE");

        /// <summary>
        /// Appends a word UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Update() => Append("UPDATE");

        /// <summary>
        /// Appends a word USING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Using() => Append("USING");

        /// <summary>
        /// Appends a word WHERE and the stringified values of the <see cref="QueryGroup"/> to the SQL Query Statement.
        /// </summary>
        /// <param name="queryGroup">The query group to be stringified.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder WhereFrom(QueryGroup queryGroup, IDbSetting dbSetting) => 
            WhereFrom(queryGroup, 0, dbSetting);

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
            if (queryGroup?.GetFields(true).IsNullOrEmpty() == false)
            {
                return Append("WHERE")
                    .Append(queryGroup.GetString(index, dbSetting));
            }
            
            return this;
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
            if (fields.IsNullOrEmpty()) return this;

            return Append("WHERE (")
                .AppendJoin(fields.Select(f => f.Name.AsFieldAndParameter(index, dbSetting)), " AND ", false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word ROW_NUMBER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder RowNumber() => Append("ROW_NUMBER()");

        /// <summary>
        /// Appends a word OVER to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Over() => Append("OVER");

        /// <summary>
        /// Appends a word AND to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder And() => Append("AND");

        /// <summary>
        /// Appends a word OR to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Or() => Append("OR");

        /// <summary>
        /// Appends a character "(" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder OpenParen() => Append("(");

        /// <summary>
        /// Appends a character ")" to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder CloseParen() => Append(")");

        /// <summary>
        /// Appends a word ON to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder On() => Append("ON");

        /// <summary>
        /// Appends a word IN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder In() => Append("IN");

        /// <summary>
        /// Appends a word BETWEEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Between() => Append("BETWEEN");

        /// <summary>
        /// Appends a word WHEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder When() => Append("WHEN");

        /// <summary>
        /// Appends a word NOT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Not() => Append("NOT");

        /// <summary>
        /// Appends a word MATCHED to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Matched() => Append("MATCHED");

        /// <summary>
        /// Appends a word THEN to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Then() => Append("THEN");

        /// <summary>
        /// Appends a word CASE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Case() => Append("CASE");

        /// <summary>
        /// Appends a word TRUNCATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Truncate() => Append("TRUNCATE");

        /// <summary>
        /// Appends the hints to the SQL Query Statement.
        /// </summary>
        /// <param name="hints">The hints to be appended.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder HintsFrom(string hints) => Append(hints);

        /// <summary>
        /// Appends a word MAX and the field to the SQL Query Statement, otherwise an empty string.
        /// </summary>
        /// <param name="field">The target field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder MaxFrom(Field field)
        {
            if (field is null) return this;

            return Append("MAX(")
                .Append(field.Name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word MIN and the field to the SQL Query Statement, otherwise an empty string.
        /// </summary>
        /// <param name="field">The target field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder MinFrom(Field field)
        {
            if (field is null) return this;

            return Append("MIN(")
                .Append(field.Name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word AVG to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Avg() => Append("AVG");

        /// <summary>
        /// Appends a word ASC to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Ascending() => Append("ASC");

        /// <summary>
        /// Appends a word DESC to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Descending() => Append("DESC");

        /// <summary>
        /// Appends a word AVG and the field to the SQL Query Statement, otherwise an empty string.
        /// </summary>
        /// <param name="field">The target field.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder AvgFrom(Field field)
        {
            if (field is null) return this;

            return Append("AVG(")
                .Append(field.Name, false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word REPLACE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Replace() => Append("REPLACE");

        /// <summary>
        /// Appends a word RETURNING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Returning() => Append("RETURNING");

        /// <summary>
        /// Appends a word CONFLICT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Conflict() => Append("CONFLICT");

        /// <summary>
        /// Appends a word ON CONFLICT to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder OnConflict() => Append("ON CONFLICT");

        /// <summary>
        /// Appends a word ON CONFLICT ON (fieldname) to the SQL Query Statement.
        /// </summary>
        /// <param name="fields">The instances of the <see cref="Field"/> objects to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The current instance.</returns>
        public QueryBuilder OnConflict(IEnumerable<Field> fields, IDbSetting dbSetting)
        {
            if (fields.IsNullOrEmpty()) return this;
            
            var fieldNames = fields
                .Select(f => f.Name.AsQuoted(dbSetting));

            return Append("ON CONFLICT (")
                .AppendJoin(fieldNames, spaceBefore: false)
                .Append(')');
        }

        /// <summary>
        /// Appends a word DO to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder Do() => Append("DO");

        /// <summary>
        /// Appends a word DO NOTHING to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder DoNothing() => Append("DO NOTHING");

        /// <summary>
        /// Appends a word DO UPDATE to the SQL Query Statement.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryBuilder DoUpdate() => Append("DO UPDATE");
    }
}