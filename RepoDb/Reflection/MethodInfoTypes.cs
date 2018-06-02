using RepoDb.Attributes;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An enumeration for type of <i>System.Reflection.MethodInfo</i>.
    /// </summary>
    public enum MethodInfoTypes : short
    {
        /// <summary>
        /// A <i>System.Convert.ToString</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Convert, "ToString", new[] { typeof(object) })]
        ConvertToString,
        /// <summary>
        /// A <i>System.Data.Common.DbDataReader.GetName(int)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DbDataReader, "GetName", new[] { typeof(int) })]
        DataReaderGetName,
        /// <summary>
        /// A <i>System.Data.Common.DbDataReader.get_Item(int)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DbDataReader, "get_Item", new[] { typeof(int) })]
        DataReaderIntGetIndexer,
        /// <summary>
        /// A <i>System.Data.Common.DbDataReader.get_Item(string)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DbDataReader, "get_Item", new[] { typeof(string) })]
        DataReaderStringGetIndexer,
        /// <summary>
        /// A <i>System.Data.DataRow.get_Item(int)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "get_Item", new[] { typeof(int) })]
        DataRowIntGetIndexer,
        /// <summary>
        /// A <i>System.Data.DataRow.set_Item(int)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "set_Item", new[] { typeof(int), typeof(object) })]
        DataRowIntSetIndexer,
        /// <summary>
        /// A <i>System.Data.DataRow.get_Item(string)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "get_Item", new[] { typeof(string) })]
        DataRowStringGetIndexer,
        /// <summary>
        /// A <i>System.Data.DataRow.st_Item(string)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataRow, "set_Item", new[] { typeof(string), typeof(object) })]
        DataRowStringSetIndexer,
        /// <summary>
        /// A <i>System.Data.DataTable.NewRow()</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DataTable, "NewRow")]
        DataTableNewRow,
        /// <summary>
        /// A <i>System.Collections.Generic.IDictionary`2.Add(string, object)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DictionaryStringObject, "Add", new[] { typeof(string), typeof(object) })]
        DictionaryStringObjectAdd,
        /// <summary>
        /// A <i>System.Reflection.FieldInfo.GetValue()</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.FieldInfo, "GetValue", new[] { typeof(object) })]
        FieldInfoGetValue,
        /// <summary>
        /// A <i>System.Guid.Parse()</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Guid, "Parse", new[] { typeof(string) })]
        GuidParse,
        /// <summary>
        /// A <i>System.Guid.ParseExact()</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Guid, "ParseExact", new[] { typeof(string), typeof(string) })]
        GuidParseExact,
        /// <summary>
        /// A <i>RepoDb.Reflection.DbNullConverter.DbNullToNull</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.ObjectConverter, "DbNullToNull", new[] { typeof(object) })]
        ObjectConverterDbNullToNull,
        /// <summary>
        /// A <i>RepoDb.Reflection.DbNullConverter.GetValue</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.ObjectConverter, "GetValue", new[] { typeof(object), typeof(PropertyInfo) })]
        ObjectConverterGetValue,
        /// <summary>
        /// A <i>System.Reflection.PropertyInfo.GetValue()</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.PropertyInfo, "GetValue", new[] { typeof(object) })]
        PropertyInfoGetValue,
        /// <summary>
        /// A <i>System.Reflection.PropertyInfo.SetValue(object)</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.PropertyInfo, "SetValue", new[] { typeof(object), typeof(object) })]
        PropertyInfoSetValue,
        /// <summary>
        /// A <i>System.Type.GetMethod.GetProperty()</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Type, "GetProperty", new[] { typeof(string) })]
        TypeGetProperty,
        /// <summary>
        /// A <i>System.Type.GetType</i> method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.Type, "GetType")]
        TypeGetType
    }
}
