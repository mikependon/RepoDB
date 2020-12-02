using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RepoDb.IntegrationTests
{
    /// <summary>
    /// store current culture and set CultureInfo.DefaultThreadCurrentCulture for unit test case.
    /// restore original culture when dispose.
    /// </summary>
    internal sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo original;
        public CultureScope(string cultureName)
        {
            original = CultureInfo.DefaultThreadCurrentCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        }

        public void Dispose()
        {
            CultureInfo.DefaultThreadCurrentCulture = original;
        }
    }
}
