using RepoDb.Options;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define the globalized configurations for the library.
    /// </summary>
    public sealed class GlobalConfiguration
    {
        private static readonly GlobalConfiguration _instance;

        /// <summary>
        /// 
        /// </summary>
        static GlobalConfiguration()
        {
            _instance = new GlobalConfiguration();
        }

        #region Methods

        /// <summary>
        /// Setup the globalized configurations for the application.
        /// </summary>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration Setup() => Setup(new());

        /// <summary>
        /// Setup the globalized configurations for the application.
        /// </summary>
        /// <param name="options">The option class that contains the value for the configurations.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration Setup(GlobalConfigurationOptions options)
        {
            Options = options;
            return _instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the globalized configurations.
        /// </summary>
        public static GlobalConfigurationOptions Options { get; private set; } = new();

        #endregion
    }
}
