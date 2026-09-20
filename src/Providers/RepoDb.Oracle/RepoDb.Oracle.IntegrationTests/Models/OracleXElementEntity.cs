#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.PropertyHandlers.Oracle;

namespace RepoDb.Oracle.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="OracleXmlTypeToXElementPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class OracleXElementEntity
    {
        public System.Int64 Id { get; set; }

        [PropertyHandler(typeof(OracleXmlTypeToXElementPropertyHandler))]
        public System.Xml.Linq.XElement ColumnXml { get; set; }
    }
}
