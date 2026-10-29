#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema.Attributes
{
    /// <summary>
    /// Marks metadata reconstructed by the schema definition reader rather than serialized independently.
    /// Source identity properties may be written as overrides when they differ from the document source.
    /// This attribute describes the portable definition contract, not general-purpose serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class DefinitionIgnoreAttribute : Attribute
    {
    }
}
