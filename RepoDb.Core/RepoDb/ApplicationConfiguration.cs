using RepoDb.Options;

namespace RepoDb
{
    /// <summary>
    /// A static class that is being used to define the globalized configurations for the library.
    /// </summary>
    public static class ApplicationConfiguration
    {
        #region Methods

        /// <summary>
        /// Setup the globalized configurations for the application.
        /// </summary>
        /// <param name="options">The option class that contains the value for the configurations.</param>
        public static void Setup(ApplicationConfigurationOptions options) =>
            Options = options;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the globalized configurations.
        /// </summary>
        public static ApplicationConfigurationOptions Options { get; private set; } = new ApplicationConfigurationOptions();

        #endregion
    }
}
