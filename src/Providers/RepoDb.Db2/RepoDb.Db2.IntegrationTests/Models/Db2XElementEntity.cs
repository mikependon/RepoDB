#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using RepoDb.PropertyHandlers.Db2;

namespace RepoDb.Db2.IntegrationTests.Models
{
    /// <summary>
    /// A minimal model that maps to the "PropertyHandler" table, used to test <see cref="Db2XmlToXElementPropertyHandler"/>.
    /// </summary>
    [Map("PropertyHandler")]
    public class Db2XElementEntity
    {
        public System.Int32 Id { get; set; }

        [PropertyHandler(typeof(Db2XmlToXElementPropertyHandler))]
        public System.Xml.Linq.XElement ColumnXml { get; set; }
    }
}
