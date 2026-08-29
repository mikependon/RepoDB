using Vertica.Data.VerticaClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings for the <see cref="VerticaConnection"/> object.
    /// </summary>
    public static partial class VerticaConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for Vertica.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseVertica(this GlobalConfiguration globalConfiguration)
        {
            VerticaBootstrap.InitializeInternal();
            return globalConfiguration;
        }
    }
}
