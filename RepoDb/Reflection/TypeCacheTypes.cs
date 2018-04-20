namespace RepoDb.Reflection
{
    /// <summary>
    /// A type of Type being cached.
    /// </summary>
    public enum TypeCacheTypes : short
    {
        /// <summary>
        /// A System.Convert type.
        /// </summary>
        ConvertType,
        /// <summary>
        /// A System.Data.Common.DbDataReader type.
        /// </summary>
        DataReaderType,
        /// <summary>
        /// A type of the current executing assembly.
        /// </summary>
        ExecutingAssemblyType,
        /// <summary>
        /// An System.Reflection.MethodInfo type.
        /// </summary>
        MethodInfo,
        /// <summary>
        /// An System.Object type.
        /// </summary>
        ObjectType,
        /// <summary>
        /// An System.Reflection.PropertyInfo type.
        /// </summary>
        PropertyInfo,
        /// <summary>
        /// A System.String type.
        /// </summary>
        StringType
    }
}
