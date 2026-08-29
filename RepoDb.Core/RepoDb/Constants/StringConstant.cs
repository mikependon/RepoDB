#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb
{
    /// <summary>
    /// A class that contains all the constant strings.
    /// </summary>
    internal static class StringConstant
    {
        /// <summary>
        /// The text being prepended to the parameter name whenever a <see cref="QueryField"/> (or <see cref="QueryGroup"/>)
        /// is being used for an 'Update' operation (see <see cref="QueryField.IsForUpdate"/> and <see cref="QueryGroup.IsForUpdate"/>).
        /// </summary>
        internal const string UpdateParameterPrefix = "m_";
    }
}
