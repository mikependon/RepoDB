using System;
using System.Globalization;

namespace RepoDb.IntegrationTests
{
    /// <summary>
    /// store current culture and set CultureInfo.DefaultThreadCurrentCulture for unit test case.
    /// restore original culture when dispose.
    /// </summary>
    internal sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo originalCulture;
        private readonly CultureInfo originalUICulture;

        public CultureScope(string cultureName)
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        }

        public void Dispose()
        {
            CultureInfo.CurrentUICulture = originalUICulture;
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
