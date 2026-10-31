using System;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>Maps destination identities without changing the saved source definition.</summary>
    internal static class SchemaDefinitionMapping
    {
        internal static void Validate(SchemaDefinitionInfo definition, IDbSetting setting, SchemaMappingMode mode)
        {
            if (!Enum.IsDefined(typeof(SchemaMappingMode), mode))
                throw new ArgumentOutOfRangeException(nameof(mode));
            var identities = definition.Tables.Select(table => table.Table)
                .Concat(definition.Tables.SelectMany(table => table.ForeignKeys.Select(key => key.ReferencedTable)))
                .Concat(definition.ExternalDependencies ?? Array.Empty<TableInfo>())
                .Where(table => table != null).Distinct().ToList();
            if (mode == SchemaMappingMode.Preserve)
            {
                var qualified = identities.FirstOrDefault(table => !string.IsNullOrWhiteSpace(table.Schema));
                if (qualified != null && setting == null)
                    throw new NotSupportedException("Destination database settings are required to verify schema support.");
                if (qualified != null && !setting.IsSchemaSupported)
                    throw new NotSupportedException($"Cannot preserve schema '{qualified.Schema}' for table '{qualified.Name}': the destination does not support schemas. Use Ignore or Prefix.");
            }
            // Conservatively reject case-only collisions: provider settings do not expose identifier comparison rules.
            var duplicate = identities.GroupBy(table => (Map(table, mode).Schema ?? setting?.DefaultSchema ?? "") + "\0" + Map(table, mode).Name,
                StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
            if (duplicate != null)
                throw new InvalidOperationException($"Schema mapping collision using {mode}: {string.Join(", ", duplicate.Select(table => $"'{table.Schema}.{table.Name}'"))} map to the same destination table. Use Prefix or change the source names.");
        }

        internal static TableSchema Map(TableSchema source, SchemaMappingMode mode)
        {
            if (mode == SchemaMappingMode.Preserve)
                return source;
            return new TableSchema(Map(source.Table, mode))
            {
                Columns = source.Columns,
                PrimaryKey = source.PrimaryKey,
                UniqueConstraints = source.UniqueConstraints,
                CheckConstraints = source.CheckConstraints,
                Indexes = source.Indexes,
                ForeignKeys = source.ForeignKeys.Select(key => new ForeignKeyInfo(key.Name)
                {
                    Columns = key.Columns,
                    ReferencedColumns = key.ReferencedColumns,
                    ReferencedTable = Map(key.ReferencedTable, mode),
                    DeleteRule = key.DeleteRule,
                    UpdateRule = key.UpdateRule
                }).ToList()
            };
        }

        private static TableInfo Map(TableInfo source, SchemaMappingMode mode) =>
            new TableInfo(mode == SchemaMappingMode.Prefix && !string.IsNullOrWhiteSpace(source.Schema)
                ? source.Schema + "_" + source.Name : source.Name,
                mode == SchemaMappingMode.Preserve ? source.Schema : null, source.Database, source.Provider);
    }
}
