namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the Db2 data provider.
    /// </summary>
    public static partial class Db2GlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for Db2.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDb2(this GlobalConfiguration globalConfiguration)
        {
            Db2Bootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
