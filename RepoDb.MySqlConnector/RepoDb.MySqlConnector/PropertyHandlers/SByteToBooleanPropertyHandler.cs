#region Copyright Attributions

// Copyright (c) 2026 Paolo Bassi and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.MySqlConnector.PropertyHandlers
{
    public class SByteToBooleanPropertyHandler : IPropertyHandler<SByte, bool>
    {
        public bool Get(SByte input, PropertyHandlerGetOptions options)
        {
            return input != 0;
        }

        public SByte Set(bool input, PropertyHandlerSetOptions options)
        {
            return input ? (SByte)1 : (SByte)0;
        }
    }
}
