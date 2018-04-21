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
        DataReaderIntIndexer,
        /// <summary>
        /// A System.Data.Common.DbDataReader.get_Item(string) method.
        /// </summary>
        [CreateMethodInfo(TypeTypes.DbDataReader, "get_Item", new[] { typeof(string) })]
        DataReaderStringIndexer
    }
}
