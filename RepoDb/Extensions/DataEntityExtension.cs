using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    public static class DataEntityExtension
    {
        // GetPropertiesFor
        internal static IEnumerable<PropertyInfo> GetPropertiesFor(Type type, Command command)
        {
            return type
                .GetProperties()
                .Where(property => !property.IsIgnored(command));
        }

        internal static IEnumerable<PropertyInfo> GetPropertiesFor<T>(Command command)
            where T : IDataEntity
        {
            return GetPropertiesFor(typeof(T), command);
        }

        internal static IEnumerable<PropertyInfo> GetPropertiesFor(this IDataEntity dataEntity, Command command)
        {
            return GetPropertiesFor(dataEntity.GetType(), command);
        }

        // GetCommandType
        internal static CommandType GetCommandType(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType ?? CommandType.Text;
        }

        internal static CommandType GetCommandType<T>()
            where T : IDataEntity
        {
            return GetCommandType(typeof(T));
        }

        internal static CommandType GetCommandType(this IDataEntity dataEntity)
        {
            return GetCommandType(dataEntity.GetType());
        }

        // GetPropertyByAttribute
        internal static PropertyInfo GetPropertyByAttribute(Type type, Type attributeType)
        {
            return type
                .GetProperties()
                .FirstOrDefault(property => property.GetCustomAttribute(attributeType) != null);
        }

        internal static PropertyInfo GetPropertyByAttribute<T>(Type attributeType)
            where T : IDataEntity
        {
            return GetPropertyByAttribute(typeof(T), attributeType);
        }

        internal static PropertyInfo GetPropertyByAttribute(this IDataEntity dataEntity, Type attributeType)
        {
            return GetPropertyByAttribute(dataEntity.GetType(), attributeType);
        }

        // GetPrimaryProperty
        internal static PropertyInfo GetPrimaryProperty(Type type)
        {
            return GetPropertyByAttribute(type, typeof(PrimaryAttribute)) ??
                type.GetProperties()
                    .FirstOrDefault(
                        property =>
                            string.Equals(property.Name, "Id", StringComparison.InvariantCultureIgnoreCase)) ??
                type.GetProperties()
                    .FirstOrDefault(
                        property =>
                            string.Equals(property.Name, $"{GetMappedName(type)}Id", StringComparison.InvariantCultureIgnoreCase));
        }

        internal static PropertyInfo GetPrimaryProperty<T>()
            where T : IDataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        internal static PropertyInfo GetPrimaryProperty(this IDataEntity dataEntity)
        {
            return GetPrimaryProperty(dataEntity.GetType());
        }

        // GetIdentityProperty
        internal static PropertyInfo GetIdentityProperty(Type type)
        {
            var property = GetPropertyByAttribute(type, typeof(PrimaryAttribute));
            return property != null ? property.IsIdentity() ? property : null : null;
        }

        internal static PropertyInfo GetIdentityProperty<T>()
            where T : IDataEntity
        {
            return GetPrimaryProperty(typeof(T));
        }

        internal static PropertyInfo GetIdentityProperty(this IDataEntity dataEntity)
        {
            return GetPrimaryProperty(dataEntity.GetType());
        }

        // GetMappedName
        internal static string GetMappedName(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.Name ?? type.Name;
        }

        internal static string GetMappedName<T>()
            where T : IDataEntity
        {
            return GetMappedName(typeof(T));
        }

        internal static string GetMappedName(this IDataEntity dataEntity)
        {
            return GetMappedName(dataEntity.GetType());
        }

        // AsObject
        internal static object AsObject(this IDataEntity dataEntity, IQueryGroup queryGroup)
        {
            // TODO: Must consider the groupings here
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            dataEntity.GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.Name] = property.GetValue(dataEntity);
                });
            if (queryGroup != null)
            {
                queryGroup
                    .GetAllQueryFields()?
                    .ToList()
                    .ForEach(queryField =>
                    {
                        expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
                    });
            }
            return (ExpandoObject)expandObject;
        }

        internal static object AsObject(this IDataEntity dataEntity)
        {
            return AsObject(dataEntity, null);
        }

        // AsDataTable
        internal static DataTable AsDataTable<T>(this IEnumerable<T> entities, IDbConnection connection)
            where T : IDataEntity
        {
            //var properties = typeof(T).GetProperties().ToList();
            //var table = new DataTable(GetMappedName(typeof(T)));
            //properties.ForEach(property =>
            //{
            //    table.Columns.Add(property.AsDataColumn());
            //});
            //entities.ToList().ForEach(entity =>
            //{
            //    var row = table.NewRow();
            //    properties.ForEach(property =>
            //    {
            //        row[property.Name] = property.GetValue(entity);
            //    });
            //    table.Rows.Add(row);
            //});
            //return table;
            var mappedName = GetMappedName<T>();
            var table = new DataTable(mappedName);
            using (var command = connection.CreateCommand($"SELECT TOP 1 * FROM {mappedName} WHERE 1 = 0;"))
            {
                using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var schemaTable = reader.GetSchemaTable();
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        var columnName = Convert.ToString(row["ColumnName"]);
                        var dataType = Type.GetType(Convert.ToString(row["DataType"]));
                        table.Columns.Add(new DataColumn(columnName, dataType));
                    }
                }
            }
            var entityType = typeof(T);
            var properties = entityType.GetProperties();
            entities?.ToList().ForEach(entity =>
            {
                var row = table.NewRow();
                foreach (DataColumn column in table.Columns)
                {
                    var property = properties.FirstOrDefault(p => string.Equals(column.ColumnName, p.Name, StringComparison.InvariantCultureIgnoreCase));
                    var value = (object)null;
                    if (property != null)
                    {
                        value = property.GetValue(entity);
                    }
                    row[column] = value ?? DBNull.Value;
                }
                table.Rows.Add(row);
            });
            return table;
        }

        // IsQueryble
        internal static bool IsQueryable(Type type)
        {
            return true; // Always return true for Query
        }

        internal static bool IsQueryable<T>()
            where T : IDataEntity
        {
            return IsQueryable(typeof(T));
        }

        internal static bool IsQueryable(this IDataEntity dataEntity)
        {
            return IsQueryable(dataEntity.GetType());
        }

        // InInsertable
        internal static bool InInsertable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool InInsertable<T>()
            where T : IDataEntity
        {
            return InInsertable(typeof(T));
        }

        internal static bool InInsertable(this IDataEntity dataEntity)
        {
            return InInsertable(dataEntity.GetType());
        }

        // IsUpdateable
        internal static bool IsUpdateable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsUpdateable<T>()
            where T : IDataEntity
        {
            return IsUpdateable(typeof(T));
        }

        internal static bool IsUpdateable(this IDataEntity dataEntity)
        {
            return IsUpdateable(dataEntity.GetType());
        }

        // IsDeletable
        internal static bool IsDeletable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsDeletable<T>()
            where T : IDataEntity
        {
            return IsDeletable(typeof(T));
        }

        internal static bool IsDeletable(this IDataEntity dataEntity)
        {
            return IsDeletable(dataEntity.GetType());
        }

        // IsInsertable
        internal static bool IsInsertable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsInsertable<T>()
            where T : IDataEntity
        {
            return IsInsertable(typeof(T));
        }

        internal static bool IsInsertable(this IDataEntity dataEntity)
        {
            return IsInsertable(dataEntity.GetType());
        }

        // IsMergeable
        internal static bool IsMergeable(Type type)
        {
            var attribute = type.GetCustomAttribute<MapAttribute>();
            return attribute?.CommandType != CommandType.StoredProcedure;
        }

        internal static bool IsMergeable<T>()
            where T : IDataEntity
        {
            return IsMergeable(typeof(T));
        }

        internal static bool IsMergeable(this IDataEntity dataEntity)
        {
            return IsMergeable(dataEntity.GetType());
        }

        // GetSelectStatement

        private static string GetSelectStatement(Type type, IQueryGroup where)
        {
            var statement = new StringBuilder();
            statement.AppendLine("SELECT");
            statement.AppendLine(GetPropertiesFor(type, Command.Select)?.AsFields().Join(", "));
            statement.AppendLine("FROM");
            statement.AppendLine(GetMappedName(type));
            if (where != null)
            {
                statement.AppendLine("WHERE");
                statement.AppendLine(where.FixParameters().GetString());
            }
            statement.AppendLine(";");
            return statement.ToString();
        }

        internal static string GetSelectStatement<T>(IQueryGroup where)
            where T : IDataEntity
        {
            return GetSelectStatement(typeof(T), where);
        }

        internal static string GetSelectStatement(this IDataEntity dataEntity, IQueryGroup where)
        {
            return GetSelectStatement(dataEntity.GetType(), where);
        }

        // GetUpdateStatement

        private static string GetUpdateStatement(Type type, IQueryGroup where)
        {
            var statement = new StringBuilder();
            statement.AppendLine("UPDATE");
            statement.AppendLine(GetMappedName(type));
            statement.AppendLine("SET");
            statement.AppendLine(GetPropertiesFor(type, Command.Update)?.AsFieldsAndParameters().Join(", "));
            if (where != null)
            {
                statement.AppendLine("WHERE");
                statement.AppendLine(where.FixParameters().GetString());
            }
            statement.AppendLine(";");
            return statement.ToString();
        }

        internal static string GetUpdateStatement<T>(IQueryGroup where)
            where T : IDataEntity
        {
            return GetUpdateStatement(typeof(T), where);
        }

        internal static string GetUpdateStatement(this IDataEntity dataEntity, IQueryGroup where)
        {
            return GetUpdateStatement(dataEntity.GetType(), where);
        }

        // GetDeleteStatement

        private static string GetDeleteStatement(Type type, IQueryGroup where)
        {
            var statement = new StringBuilder();
            statement.AppendLine("DELETE");
            statement.AppendLine(GetMappedName(type));
            if (where != null)
            {
                statement.AppendLine("WHERE");
                statement.AppendLine(where.FixParameters().GetString());
            }
            statement.AppendLine(";");
            return statement.ToString();
        }

        internal static string GetDeleteStatement<T>(IQueryGroup where)
            where T : IDataEntity
        {
            return GetDeleteStatement(typeof(T), where);
        }

        internal static string GetDeleteStatement(this IDataEntity dataEntity, IQueryGroup where)
        {
            return GetDeleteStatement(dataEntity.GetType(), where);
        }

        // GetInsertStatement

        private static string GetInsertStatement(Type type)
        {
            var properties = GetPropertiesFor(type, Command.Insert)?.ToList();
            var primary = GetPrimaryProperty(type);
            var statement = new StringBuilder();
            statement.AppendLine("INSERT INTO");
            statement.AppendLine(GetMappedName(type));
            statement.AppendLine("(");
            statement.AppendLine(properties?.AsFields().Join(", "));
            statement.AppendLine(")");
            statement.AppendLine("VALUES");
            statement.AppendLine("(");
            statement.AppendLine(properties?.AsParameters().Join(", "));
            statement.AppendLine(");");
            if (primary != null)
            {
                var result = primary.IsIdentity() ? "CONVERT(INT, SCOPE_IDENTITY())" : $"@{primary.Name}";
                statement.AppendLine("SELECT");
                statement.AppendLine(result);
                statement.AppendLine("AS [Result]");
            }
            statement.AppendLine(";");
            return statement.ToString();
        }

        internal static string GetInsertStatement<T>()
            where T : IDataEntity
        {
            return GetInsertStatement(typeof(T));
        }

        internal static string GetInsertStatement(this IDataEntity dataEntity)
        {
            return GetInsertStatement(dataEntity.GetType());
        }

        // GetMergeStatement

        private static string GetMergeStatement(Type type, IEnumerable<IField> qualifiers)
        {
            var allProperties = GetPropertiesFor(type, Command.None);
            var insertProperties = GetPropertiesFor(type, Command.Insert)?.ToList();
            var updateProperties = GetPropertiesFor(type, Command.Update)?.ToList();
            var qualifierProperties = qualifiers != null ?
                type.GetProperties().Where(property => qualifiers.Select(field => field.Name).Contains(property.Name, StringComparer.InvariantCultureIgnoreCase)) :
                    GetPrimaryProperty(type).ToEnumerable();
            var statement = new StringBuilder();
            statement.AppendLine("MERGE ");
            statement.AppendLine(GetMappedName(type));
            statement.AppendLine("AS T");
            statement.AppendLine("USING");
            statement.AppendLine("(");
            statement.AppendLine("SELECT");
            statement.AppendLine(allProperties?.AsParametersAsFields().Join(", "));
            statement.AppendLine(")");
            statement.AppendLine("AS S");
            statement.AppendLine("ON");
            statement.AppendLine("(");
            statement.AppendLine(qualifierProperties?.Select(property => property.AsJoinQualifier("S", "T")).Join(" AND "));
            statement.AppendLine(")");
            statement.AppendLine("WHEN NOT MATCHED THEN");
            statement.AppendLine("INSERT");
            statement.AppendLine("(");
            statement.AppendLine(insertProperties?.AsFields().Join(", "));
            statement.AppendLine(")");
            statement.AppendLine("VALUES");
            statement.AppendLine("(");
            statement.AppendLine(insertProperties?.Select(property => $"S.{property.AsField()}").Join(", "));
            statement.AppendLine(")");
            statement.AppendLine("WHEN MATCHED THEN");
            statement.AppendLine("UPDATE");
            statement.AppendLine("SET");
            statement.AppendLine(updateProperties?.Select(property => $"{property.AsField()} = S.{property.AsField()}").Join(", "));
            statement.AppendLine(";");
            return statement.ToString();
        }

        internal static string GetMergeStatement<T>(IEnumerable<IField> qualifiers)
            where T : IDataEntity
        {
            return GetMergeStatement(typeof(T), qualifiers);
        }

        internal static string GetMergeStatement(this IDataEntity dataEntity, IEnumerable<IField> qualifiers)
        {
            return GetMergeStatement(dataEntity.GetType(), qualifiers);
        }
    }
}
