#region Copyright Attributions

// Copyright (c) 2024 Bert Huijben and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace System.Runtime.CompilerServices
{
#if !NET
    // Required to allow init properties in netstandard
    internal sealed class IsExternalInit : Attribute
    {
    }
#endif
}
