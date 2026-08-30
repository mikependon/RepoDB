#region Copyright Attributions

// Copyright (c) 2020 fredliex and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
