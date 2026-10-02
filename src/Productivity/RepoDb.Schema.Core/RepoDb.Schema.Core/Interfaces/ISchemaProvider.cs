#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Options;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the schema provider of the library.
    /// </summary>
    public interface ISchemaProvider
    {
        /// <summary>
        /// Creates a schema using the extractor.
        /// </summary>
        /// <param name="extractor">The extractor that is used to extract the schema.</param>
        /// <param name="callback">The callback used to configure the <see cref="CreateSchemaOptions"/>.</param>
        void Create(ISchemaExtractor extractor,
            Action<CreateSchemaOptions> callback);
    }
}
