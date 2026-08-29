// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using RepoDb.Attributes;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration used to define the ordering of the query field.
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// The ascending order.
        /// </summary>
        [Text("ASC")] Ascending = 720208773,
        /// <summary>
        /// The descending order.
        /// </summary>
        [Text("DESC")] Descending = 1249030520
    }
}
