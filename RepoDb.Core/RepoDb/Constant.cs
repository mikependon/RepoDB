namespace RepoDb
{
    /// <summary>
    /// A class that holds the constant values for the library.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// The default value of the batch operation size.
        /// </summary>
        public const int DefaultBatchOperationSize = 30;

        /// <summary>
        /// The default value of the cache expiration in minutes.
        /// </summary>
        public const int DefaultCacheItemExpirationInMinutes = 180;

        /// <summary>
        /// The maximum parameters of ADO.Net when executing a command.
        /// </summary>
        public const int MaxParametersCount = 2100;
    }
}
