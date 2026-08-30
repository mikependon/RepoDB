#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define a conjunction for the query grouping. This enumeration is used at <see cref="QueryGroup"/> object.
    /// </summary>
    public enum Conjunction
    {
        /// <summary>
        /// The (AND) conjunction.
        /// </summary>
        And = 446274343,
        /// <summary>
        /// The (OR) conjunction.
        /// </summary>
        Or = 1382346125
    }
}