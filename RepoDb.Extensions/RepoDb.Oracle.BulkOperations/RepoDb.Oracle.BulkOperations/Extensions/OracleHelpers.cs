using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// Mapping resolution, identity read/write, and misc helpers shared by every Oracle bulk operation.
    /// Consolidates what the PostgreSQL bulk package spreads across <c>NpgsqlHelpers.cs</c> and parts of
    /// <c>NpgsqlText.cs</c>'s "Helpers" region, kept together here since this package is intentionally
    /// leaner and does not need the extra indirection.
    /// </summary>
    internal static class OracleHelpers
    {
        #region Mappings

        /// <summary>
        /// Resolves the effective bulk-insert mappings for an entity-based call: one <see cref="OracleBulkInsertMapItem"/>
        /// per entity property that also exists as a column on the target table, honoring <paramref name="includeIdentity"/>.
        /// </summary>
        public static IEnumerable<OracleBulkInsertMapItem> GetMappings(DbFieldCollection dbFields,
            Type entityType,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (var property in PropertyCache.Get(entityType))
            {
                var mappedName = property.GetMappedName();
                var dbField = dbFields.GetByUnquotedName(mappedName.AsUnquoted(true, dbSetting)) ??
                    dbFields.GetByName(mappedName);

                if (dbField == null)
                {
                    continue;
                }

                if (dbField.IsIdentity && includeIdentity == false)
                {
                    continue;
                }

                yield return new OracleBulkInsertMapItem(mappedName,
                    dbField.Name,
                    TryGetExplicitOracleDbType(property.PropertyInfo));
            }
        }

        /// <summary>
        /// Resolves the effective bulk-insert mappings for a dictionary-based (expando/anonymous) entity call.
        /// </summary>
        public static IEnumerable<OracleBulkInsertMapItem> GetMappings(IDictionary<string, object> dictionary,
            DbFieldCollection dbFields,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (var key in dictionary.Keys)
            {
                var dbField = dbFields.GetByUnquotedName(key.AsUnquoted(true, dbSetting)) ??
                    dbFields.GetByName(key);

                if (dbField == null)
                {
                    continue;
                }

                if (dbField.IsIdentity && includeIdentity == false)
                {
                    continue;
                }

                yield return new OracleBulkInsertMapItem(key, dbField.Name, null);
            }
        }

        /// <summary>
        /// Resolves the effective bulk-insert mappings for a <see cref="DataTable"/>-based call.
        /// </summary>
        public static IEnumerable<OracleBulkInsertMapItem> GetMappings(DataTable table,
            DbFieldCollection dbFields,
            bool includeIdentity,
            IDbSetting dbSetting)
        {
            foreach (DataColumn column in table.Columns)
            {
                var dbField = dbFields.GetByUnquotedName(column.ColumnName.AsUnquoted(true, dbSetting)) ??
                    dbFields.GetByName(column.ColumnName);

                if (dbField == null)
                {
                    continue;
                }

                if (dbField.IsIdentity && includeIdentity == false)
                {
                    continue;
                }

                yield return new OracleBulkInsertMapItem(column.ColumnName, dbField.Name, null);
            }
        }

        #endregion

        #region Row Building

        /// <summary>
        /// The set of <see cref="Field"/> objects for an entity type, restricted to properties that also
        /// exist as a column on the target table. Shared by BulkMerge/BulkUpdate/BulkDelete's Base
        /// orchestration to decide what to stage.
        /// </summary>
        public static IList<Field> GetEntityFields(Type entityType,
            DbFieldCollection dbFields,
            IDbSetting dbSetting) =>
            PropertyCache.Get(entityType)
                .Select(property => property.AsField())
                .Where(field => dbFields.GetByUnquotedName(field.Name.AsUnquoted(true, dbSetting)) != null)
                .AsList();

        /// <summary>
        /// The set of <see cref="Field"/> objects for a dictionary-based (expando/anonymous) entity,
        /// restricted to keys that also exist as a column on the target table.
        /// </summary>
        public static IList<Field> GetDictionaryFields(IDictionary<string, object> dictionary,
            DbFieldCollection dbFields,
            IDbSetting dbSetting) =>
            dictionary.Keys
                .Select(key => new Field(key))
                .Where(field => dbFields.GetByUnquotedName(field.Name.AsUnquoted(true, dbSetting)) != null)
                .AsList();

        /// <summary>
        /// The set of <see cref="Field"/> objects for a <see cref="DataTable"/>, restricted to columns
        /// that also exist as a column on the target table.
        /// </summary>
        public static IList<Field> GetDataTableFields(DataTable table,
            DbFieldCollection dbFields,
            IDbSetting dbSetting) =>
            table.Columns.Cast<DataColumn>()
                .Select(column => new Field(column.ColumnName))
                .Where(field => dbFields.GetByUnquotedName(field.Name.AsUnquoted(true, dbSetting)) != null)
                .AsList();

        /// <summary>
        /// Extracts one <see cref="object"/> array per entity, values ordered to match <paramref name="fields"/>,
        /// with an optional trailing 0-based input-order index (for the staging table's <c>__RepoDb_OrderColumn</c>).
        /// </summary>
        public static List<object[]> BuildRows<TEntity>(IList<TEntity> entities,
            IList<Field> fields,
            bool isDictionary,
            IDictionary<string, Func<object, object>> gettersByMappedName,
            bool includeOrderColumn)
            where TEntity : class
        {
            var width = fields.Count + (includeOrderColumn ? 1 : 0);
            var rows = new List<object[]>(entities.Count);

            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var row = new object[width];

                for (var c = 0; c < fields.Count; c++)
                {
                    if (isDictionary)
                    {
                        (entity as IDictionary<string, object>)?.TryGetValue(fields[c].Name, out row[c]);
                    }
                    else if (gettersByMappedName.TryGetValue(fields[c].Name, out var getter))
                    {
                        row[c] = getter(entity);
                    }
                }

                if (includeOrderColumn)
                {
                    row[fields.Count] = i;
                }

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// <see cref="BuildRows"/>'s <see cref="DataTable"/> counterpart.
        /// </summary>
        public static List<object[]> BuildRows(IList<DataRow> rows,
            IList<Field> fields,
            bool includeOrderColumn)
        {
            var width = fields.Count + (includeOrderColumn ? 1 : 0);
            var result = new List<object[]>(rows.Count);

            for (var i = 0; i < rows.Count; i++)
            {
                var row = new object[width];

                for (var c = 0; c < fields.Count; c++)
                {
                    row[c] = rows[i][fields[c].Name];
                }

                if (includeOrderColumn)
                {
                    row[fields.Count] = i;
                }

                result.Add(row);
            }

            return result;
        }

        /// <summary>
        /// Converts already-extracted rows (see <see cref="BuildRows{TEntity}"/>/<see cref="BuildRows(IList{DataRow}, IList{Field}, bool)"/>)
        /// into a <see cref="DataTable"/> suitable for <see cref="OracleBulkCopy"/>, with one column per
        /// entry in <paramref name="columns"/> (in the same order the row values were extracted in). Every
        /// column is typed as <see cref="object"/> so that whatever CLR value each row already carries
        /// (string, byte[], DateTime, decimal, etc.) flows through to <see cref="OracleBulkCopy"/> as-is;
        /// unlike the array-bind path this replaces, there is no per-column explicit <see cref="OracleDbType"/>
        /// override available here - OracleBulkCopy infers the wire type from the value itself.
        /// </summary>
        public static DataTable ToDataTable(IList<object[]> rows,
            IList<Field> columns,
            IDbSetting dbSetting)
        {
            var table = new DataTable();

            foreach (var column in columns)
            {
                table.Columns.Add(column.Name.AsUnquoted(true, dbSetting), typeof(object));
            }

            foreach (var row in rows)
            {
                table.Rows.Add(row);
            }

            return table;
        }

        #endregion

        #region Identity Read/Write

        /// <summary>
        /// Sets, back onto each entity, the identity value returned for it - keyed positionally by the
        /// entity's original 0-based index in the input sequence.
        /// </summary>
        public static void SetIdentities(Type entityType,
            System.Collections.IEnumerable entities,
            DbField identityField,
            IReadOnlyDictionary<int, object> identitiesByIndex,
            IDbSetting dbSetting)
        {
            if (identityField == null || identitiesByIndex == null || identitiesByIndex.Count == 0)
            {
                return;
            }

            // Dictionary/ExpandoObject-based entities have no PropertyInfo to write through - set the key
            // directly instead (using the identity column's own, unconverted name/value).
            var isDictionary = TypeCache.Get(entityType).IsDictionaryStringObject();
            var property = isDictionary ? null : GetIdentityProperty(entityType, identityField, dbSetting);

            if (isDictionary == false && property == null)
            {
                return;
            }

            var underlyingType = property == null ? null : Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var index = 0;

            foreach (var entity in entities)
            {
                if (identitiesByIndex.TryGetValue(index, out var value) && value != null && value != DBNull.Value)
                {
                    if (isDictionary)
                    {
                        if (entity is IDictionary<string, object> dictionary)
                        {
                            dictionary[identityField.Name] = value;
                        }
                    }
                    else
                    {
                        property.SetValue(entity, ConvertIdentityValue(value, underlyingType));
                    }
                }

                index++;
            }
        }

        /// <summary>
        /// Sets, back onto each <see cref="DataRow"/>, the identity value returned for it - keyed positionally
        /// by the row's original 0-based index (the <see cref="DataTable.Rows"/> ordinal it was read at).
        /// </summary>
        public static void SetDataTableIdentities(DataTable table,
            DbField identityField,
            IReadOnlyDictionary<int, object> identitiesByIndex,
            IDbSetting dbSetting)
        {
            if (identityField == null || identitiesByIndex == null || identitiesByIndex.Count == 0)
            {
                return;
            }

            var columnName = table.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .FirstOrDefault(name =>
                    string.Equals(name.AsUnquoted(true, dbSetting), identityField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));

            if (columnName == null)
            {
                if (table.Columns.Contains(identityField.Name) == false)
                {
                    return;
                }
                columnName = identityField.Name;
            }

            for (var i = 0; i < table.Rows.Count; i++)
            {
                if (identitiesByIndex.TryGetValue(i, out var value) && value != null && value != DBNull.Value)
                {
                    table.Rows[i][columnName] = Convert.ChangeType(value, table.Columns[columnName].DataType);
                }
            }
        }

        private static PropertyInfo GetIdentityProperty(Type entityType,
            DbField identityField,
            IDbSetting dbSetting) =>
            PropertyCache.Get(entityType)?
                .FirstOrDefault(p =>
                    string.Equals(p.GetMappedName().AsUnquoted(true, dbSetting), identityField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))?
                .PropertyInfo;

        private static object ConvertIdentityValue(object value,
            Type targetType)
        {
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            if (targetType.IsEnum)
            {
                return Enum.ToObject(targetType, Convert.ToInt64(value));
            }

            return Convert.ChangeType(value, targetType);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Whether the table's primary key column is also its identity column (i.e. a classic auto-generated
        /// surrogate key, as opposed to a natural/assigned primary key).
        /// </summary>
        public static bool IsPrimaryAnIdentity(DbFieldCollection dbFields)
        {
            var primary = dbFields?.GetPrimary();
            var identity = dbFields?.GetIdentity();

            return primary != null && identity != null && primary == identity;
        }

        /// <summary>
        /// Order-independent-safe hashcode of a field sequence, used to build <see cref="LocalCommandTextCache"/> keys.
        /// </summary>
        public static int EnumerableGetHashCode(IEnumerable<Field> fields)
        {
            var hashCode = new HashCode();

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    hashCode.Add(field);
                }
            }

            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Resolves an explicit <see cref="OracleDbType"/> bind type for a property, honoring the same
        /// <c>[OracleDbType]</c> attribute the rest of this Oracle provider already respects (see
        /// <see cref="OracleDbTypeAttribute"/>). Returns <c>null</c> when the property carries no such
        /// attribute, in which case the caller should fall back to ODP.NET's own value-based inference.
        /// </summary>
        public static OracleDbType? TryGetExplicitOracleDbType(PropertyInfo propertyInfo) =>
            propertyInfo?.GetCustomAttribute<OracleDbTypeAttribute>()?.OracleDbType;

        #endregion
    }
}
