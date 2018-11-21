namespace RepoDb
{
    /// <summary>
    /// A class that holds the constant values for <see cref="RepoDb"/> library.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// The default value of the cache expiration in minutes.
        /// </summary>
        public const int DefaultCacheItemExpirationInMinutes = 180;

        /// <summary>
        /// The default number of batches used when querying the data from the database in the recursive query operation.
        /// </summary>
        public const int DefaultRecursiveQueryBatchCount = 256;

        /// <summary>
        /// The default maximum recursion for the recursive query operation.
        /// </summary>
        public const int DefaultRecursiveQueryMaxRecursion = 15;
    }
}
