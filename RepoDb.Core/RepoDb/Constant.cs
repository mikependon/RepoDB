namespace RepoDb
{
    /// <summary>
    /// A class that holds the constant values for <see cref="RepoDb"/> library.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// The default value of the batch insert size.
        /// </summary>
        public const int DefaultBatchInsertSize = 10;

        /// <summary>
        /// The default value of the cache expiration in minutes.
        /// </summary>
        public const int DefaultCacheItemExpirationInMinutes = 180;

        /// <summary>
        /// The default prefix to the parameters.
        /// </summary>
        public const string DefaultParameterPrefix = "@";
    }
}
