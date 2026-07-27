using RepoDb.Enumerations.Oracle;

namespace RepoDb
{
    internal static class OracleText
    {
        #region Shared

        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName)
        {
            // The statement should create a physical or memory-based table
            return $"CREATE TABLE {pseudoTableName} SELECT * FROM {tableName} WHERE (1 = 0)";
        }

        public static string GetMergeFromPseudoTableSql(string tableName,
            string pseudoTableName)
        {
            // The statement should return the number of rows affected
            return $"MERGE INTO {tableName} FROM {pseudoTableName}";
        }

        #endregion

        #region Merge

        public static string GetPseudoTableNameForMerge(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType)
        {
            // Return the table with prefix Physical/Memory and suffix Merge for uniqueness
            return pseudoTableType == OracleBulkImportPseudoTableType.Physical ? $"Physical{tableName}Merge" : $"Memory{tableName}Merge";
        }

        #endregion
    }
}
