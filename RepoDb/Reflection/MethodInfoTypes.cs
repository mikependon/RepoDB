using RepoDb.Attributes;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A type of System.Reflection.MethodInfo being cached.
    /// </summary>
    public enum MethodInfoTypes : short
    {
        /// <summary>
        /// A System.Convert.ToString method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Convert, "ToString", new[] { typeof(object) })]
        ConvertToString,
        /// <summary>
        /// A System.Data.Common.DbDataReader.get_Item(int) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DbDataReader, "get_Item", new[] { typeof(int) })]
        DataReaderIntGetIndexer,
        /// <summary>
        /// A System.Data.Common.DbDataReader.get_Item(string) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DbDataReader, "get_Item", new[] { typeof(string) })]
        DataReaderStringGetIndexer,
        /// <summary>
        /// A System.Data.DataRow.get_Item(int) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "get_Item", new[] { typeof(int) })]
        DataRowIntGetIndexer,
        /// <summary>
        /// A System.Data.DataRow.set_Item(int) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "set_Item", new[] { typeof(int), typeof(object) })]
        DataRowIntSetIndexer,
        /// <summary>
        /// A System.Data.DataRow.get_Item(string) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "get_Item", new[] { typeof(string) })]
        DataRowStringGetIndexer,
        /// <summary>
        /// A System.Data.DataRow.st_Item(string) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "set_Item", new[] { typeof(string), typeof(object) })]
        DataRowStringSetIndexer,
        /// <summary>
        /// A System.Data.DataTable.NewRow() method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataTable, "NewRow", null)]
        DataTableNewRow,
        /// <summary>
        /// A System.Reflection.PropertyInfo.GetValue() method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.PropertyInfo, "GetValue", new[] { typeof(object) })]
        PropertyInfoGetValue
    }
}
