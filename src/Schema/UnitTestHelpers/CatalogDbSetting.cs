using RepoDb.DbSettings;

namespace RepoDb.Schema.UnitTestHelpers
{
    /// <summary>
    /// Provides an in-memory catalog test fixture.
    /// </summary>
    public sealed class CatalogDbSetting : BaseDbSetting
    {
        #region Constructors

        /// <summary>
        /// Creates a catalog setting with an optional provider.
        /// </summary>
        /// <param name="provider">The catalog provider.</param>
        public CatalogDbSetting(string provider = null)
        {
            Provider = provider;
        }

        #endregion
    }
}
