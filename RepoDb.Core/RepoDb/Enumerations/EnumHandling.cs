#region Copyright Attributions

// Copyright (c) 2024 Bert Huijben and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Enumerations
{
    /// <summary>
    /// 
    /// </summary>
    public enum EnumHandling
    {
        /// <summary>
        /// Throw an error when encountering non defined enum values. For enums decorated with a <see cref="FlagsAttribute"/> no value check is performed.
        /// </summary>
        ThrowError = 0,
        /// <summary>
        /// Use the default (0) value of the enum when encountering non defined enum values, For enums decorated with a <see cref="FlagsAttribute"/> no value check is performed.
        /// </summary>
        UseDefault = 1,
        /// <summary>
        /// Assumes all matched strings and integer values are valid.
        /// </summary>
        Cast = 2
    }
}
