using ClickHouse.Driver.ADO;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="ClickHouseConnection"/> object.
    /// </summary>
    /// <remarks>
    /// <see cref="ClickHouseConnection"/> here is <c>ClickHouse.Driver.ADO.ClickHouseConnection</c> - RepoDb no
    /// longer owns a subclass of its own.
    /// </remarks>
    public static partial class ClickHouseGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for ClickHouse.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseClickHouse(this GlobalConfiguration globalConfiguration)
        {
            ClickHouseBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
