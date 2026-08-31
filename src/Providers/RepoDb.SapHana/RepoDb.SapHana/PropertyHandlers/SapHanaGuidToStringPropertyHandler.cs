#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.SapHana.PropertyHandlers
{
    /// <summary>
    /// A <see cref="IPropertyHandler{TInput, TResult}"/> that converts a <see cref="Guid"/> data entity
    /// property to/from a <see cref="string"/> for binding against a SAP HANA <c>NVARCHAR(36)</c> column.
    /// </summary>
    public class SapHanaGuidToStringPropertyHandler : IPropertyHandler<string, Guid>
    {
        /// <summary>
        /// Converts the <see cref="string"/> value read back from the <c>NVARCHAR(36)</c> column into a <see cref="Guid"/>.
        /// </summary>
        public Guid Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? Guid.Empty : Guid.Parse(input);

        /// <summary>
        /// Converts the <see cref="Guid"/> data entity property value into a <see cref="string"/> before it is
        /// bound to the underlying <c>HanaParameter</c>.
        /// </summary>
        public string Set(Guid input,
            PropertyHandlerSetOptions options) =>
            input.ToString();
    }
}
