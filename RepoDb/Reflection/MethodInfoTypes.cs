using RepoDb.Attributes;
using System.Reflection;

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
        [CreateMethodInfo(TypeTypes.DataTable, "NewRow")]
        DataTableNewRow,
        /// <summary>
        /// A System.Reflection.FieldInfo.GetValue() method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.FieldInfo, "GetValue", new[] { typeof(object) })]
        FieldInfoGetValue,
        /// <summary>
        /// A System.Guid.Parse() method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Guid, "Parse", new[] { typeof(string) })]
        GuidParse,
        /// <summary>
        /// A RepoDb.Reflection.DbNullConverter.DbNullToNull method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.ObjectConverter, "DbNullToNull", new[] { typeof(object) })]
        ObjectConverterDbNullToNull,
        /// <summary>
        /// A RepoDb.Reflection.DbNullConverter.GetValue method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.ObjectConverter, "GetValue", new[] { typeof(object), typeof(PropertyInfo) })]
        ObjectConverterGetValue,
        /// <summary>
        /// A System.Reflection.PropertyInfo.GetValue() method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.PropertyInfo, "GetValue", new[] { typeof(object) })]
        PropertyInfoGetValue,
        /// <summary>
        /// A System.Type.GetMethod.GetProperty() method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Type, "GetProperty", new[] { typeof(string) })]
        TypeGetProperty,
        /// <summary>
        /// A System.Type.GetType method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Type, "GetType")]
        TypeGetType
    }
}
