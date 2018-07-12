namespace RepoDb
{
    /// <summary>
    /// A class that holds the constant values for <i>RepoDb</i> library.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// The default value of the cache expiration in minutes.
        /// </summary>
        public static readonly int CacheItemExpirationInMinutes = 180;

        /// <summary>
        /// The batches used when querying the data from the database in the recursive query operation.
        /// </summary>
        public static readonly int RecursiveQueryBatchCount = 256;

        /// <summary>
        /// The maximum recursion for the recursive query operation.
        /// </summary>
        public static readonly int RecursiveMaxRecursion = 15;
    }
}
