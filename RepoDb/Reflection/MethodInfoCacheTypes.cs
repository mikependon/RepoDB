namespace RepoDb.Reflection
{
    /// <summary>
    /// A type of System.Reflection.MethodInfo being cached.
    /// </summary>
    public enum MethodInfoCacheTypes : short
    {
        /// <summary>
        /// A System.Convert.ToString method.
        /// </summary>
        ConvertToStringMethod,
        /// <summary>
        /// A System.Data.Common.DbDataReader.GetIndexer method.
        /// </summary>
        DataReaderGetItemMethod
    }
}
