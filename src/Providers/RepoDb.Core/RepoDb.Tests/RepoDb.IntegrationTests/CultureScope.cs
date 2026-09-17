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
    /// Store current culture and set CultureInfo.DefaultThreadCurrentCulture for unit test case.
    /// restore original culture when dispose.
    /// </summary>
    internal sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo originalCulture;
        private readonly CultureInfo originalUICulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureScope"/> class.
        /// </summary>
        /// <param name="cultureName">The name of the culture to set.</param>
        public CultureScope(
            string cultureName)
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="CultureScope"/> class and restores the original culture settings.
        /// </summary>
        public void Dispose()
        {
            CultureInfo.CurrentUICulture = originalUICulture;
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
