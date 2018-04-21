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
        /// A System.Convert type.
        /// </summary>
        [Text("System.Convert")]
        Convert,
        /// <summary>
        /// A System.Data.Common.DbDataReader type.
        /// </summary>
        [Text("System.Data.Common.DbDataReader")]
        DbDataReader,
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
        String
    }
}
