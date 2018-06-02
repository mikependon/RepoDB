using RepoDb.Attributes;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An enumeration for type of <i>System.Type</i>.
    /// </summary>
    public enum TypeTypes : short
    {
        /// <summary>
        /// A type of the current executing assembly.
        /// </summary>
        [Text("System.Reflection.Assembly")]
        Assembly,
        /// <summary>
        /// A <i>System.Boolean</i> type.
        /// </summary>
        [Text("System.Boolean")]
        Boolean,
        /// <summary>
        /// A <i>System.Convert</i> type.
        /// </summary>
        [Text("System.Convert")]
        Convert,
        /// <summary>
        /// A <i>System.Data.DataRow</i> type.
        /// </summary>
        [Text("System.Data.DataRow")]
        DataRow,
        /// <summary>
        /// A <i>System.Data.DataTable</i> type.
        /// </summary>
        [Text("System.Data.DataTable")]
        DataTable,
        /// <summary>
        /// A <i>System.Collections.Generic.IDictionary`2</i> type.
        /// </summary>
        [Text("System.Collections.Generic.IDictionary`2")]
        DictionaryStringObject,
        /// <summary>
        /// A <i>System.DbNull</i> type.
        /// </summary>
        [Text("System.DBNull")]
        DBNull,
        /// <summary>
        /// A <i>RepoDb.ReflectionType.ObjectConverter</i> type.
        /// </summary>
        [Text("RepoDb.Reflection.ObjectConverter")]
        ObjectConverter,
        /// <summary>
        /// A <i>System.Data.Common.DbDataReader</i> type.
        /// </summary>
        [Text("System.Data.Common.DbDataReader")]
        DbDataReader,
        /// <summary>
        /// A <i>System.Dynamic.ExpandoObject</i> type.
        /// </summary>
        [Text("System.Dynamic.ExpandoObject")]
        ExpandoObject,
        /// <summary>
        /// A <i>System.Reflection.FieldInfo</i> type.
        /// </summary>
        [Text("System.Reflection.FieldInfo")]
        FieldInfo,
        /// <summary>
        /// A <i>System.Guid</i> type.
        /// </summary>
        [Text("System.Guid")]
        Guid,
        /// <summary>
        /// A <i>System.Reflection.MethodInfo</i> type.
        /// </summary>
        [Text("System.Reflection.MethodInfo")]
        MethodInfo,
        /// <summary>
        /// A <i>System.Nullable(GenericType)</i> type.
        /// </summary>
        [Text("System.Nullable`1")]
        NullableGeneric,
        /// <summary>
        /// A <i>System.Object</i> type.
        /// </summary>
        [Text("System.Object")]
        Object,
        /// <summary>
        /// A <i>System.Reflection.PropertyInfo</i> type.
        /// </summary>
        [Text("System.Reflection.PropertyInfo")]
        PropertyInfo,
        /// <summary>
        /// A <i>System.String</i> type.
        /// </summary>
        [Text("System.String")]
        String,
        /// <summary>
        /// A <i>System.Type</i> type.
        /// </summary>
        [Text("System.Type")]
        Type
    }
}
