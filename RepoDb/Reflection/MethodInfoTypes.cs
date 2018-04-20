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
        ConvertToStringMethod,
        /// <summary>
        /// A System.Data.Common.DbDataReader.GetIndexer method.
        /// </summary>
        DataReaderGetItemMethod,
        /// <summary>
        /// A System.Reflection.PropertyInfo.SetValue method.
        /// </summary>
        PropertySetValueMethod
    }
}
