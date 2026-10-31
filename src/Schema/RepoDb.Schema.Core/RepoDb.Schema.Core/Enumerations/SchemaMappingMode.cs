namespace RepoDb.Schema.Enumerations
{
    /// <summary>Defines how source schema names are mapped to the destination.</summary>
    public enum SchemaMappingMode
    {
        /// <summary>Keeps schema names, requiring destination schema support.</summary>
        Preserve = 0,
        /// <summary>Removes schema names.</summary>
        Ignore = 1,
        /// <summary>Prefixes table names with the schema name and an underscore.</summary>
        Prefix = 2
    }
}
