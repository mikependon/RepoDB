using RepoDb.Attributes;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A type of Type being cached.
    /// </summary>
    public enum TypeTypes : short
    {
        /// <summary>
        /// A type of the current executing assembly.
        /// </summary>
        [Text("System.Reflection.Assembly")]
        Assembly,
        /// <summary>
        /// A System.Boolean type.
        /// </summary>
        [Text("System.Boolean")]
        Boolean,
        /// <summary>
        /// A System.Convert type.
        /// </summary>
        [Text("System.Convert")]
        Convert,
        /// <summary>
        /// A System.Data.DataRow type.
        /// </summary>
        [Text("System.Data.DataRow")]
        DataRow,
        /// <summary>
        /// A System.Data.DataTable type.
        /// </summary>
        [Text("System.Data.DataTable")]
        DataTable,
        /// <summary>
        /// A System.Collections.Generic.IDictionary`2 type.
        /// </summary>
        [Text("System.Collections.Generic.IDictionary`2")]
        DictionaryStringObject,
        /// <summary>
        /// A System.DbNull type.
        /// </summary>
        [Text("System.DBNull")]
        DBNull,
        /// <summary>
        /// A RepoDb.ReflectionType.ObjectConverter type.
        /// </summary>
        [Text("RepoDb.Reflection.ObjectConverter")]
        ObjectConverter,
        /// <summary>
        /// A System.Data.Common.DbDataReader type.
        /// </summary>
        [Text("System.Data.Common.DbDataReader")]
        DbDataReader,
        /// <summary>
        /// An System.Dynamic.ExpandoObject type.
        /// </summary>
        [Text("System.Dynamic.ExpandoObject")]
        ExpandoObject,
        /// <summary>
        /// An System.Reflection.FieldInfo type.
        /// </summary>
        [Text("System.Reflection.FieldInfo")]
        FieldInfo,
        /// <summary>
        /// An System.Guid type.
        /// </summary>
        [Text("System.Guid")]
        Guid,
        /// <summary>
        /// An System.Reflection.MethodInfo type.
        /// </summary>
        [Text("System.Reflection.MethodInfo")]
        MethodInfo,
        /// <summary>
        /// An System.Nullable(GenericType) type.
        /// </summary>
        [Text("System.Nullable`1")]
        NullableGeneric,
        /// <summary>
        /// An System.Object type.
        /// </summary>
        [Text("System.Object")]
        Object,
        /// <summary>
        /// An System.Reflection.PropertyInfo type.
        /// </summary>
        [Text("System.Reflection.PropertyInfo")]
        PropertyInfo,
        /// <summary>
        /// A System.String type.
        /// </summary>
        [Text("System.String")]
        String,
        /// <summary>
        /// A System.Type type.
        /// </summary>
        [Text("System.Type")]
        Type
    }
}
