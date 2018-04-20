namespace RepoDb.Reflection
{
    /// <summary>
    /// An array of type of Type being cached.
    /// </summary>
    public enum TypeArrayTypes : short
    {
        /// <summary>
        /// An array of System.Data.Common.DbDataReader types.
        /// </summary>
        DataReaderTypes,
        /// <summary>
        /// An array of System.Object types.
        /// </summary>
        ObjectTypes,
        /// <summary>
        /// An array of System.String types.
        /// </summary>
        StringTypes
    }
}
