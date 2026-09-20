#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using MySql.Data.Types;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.PropertyHandlers.MySql
{
    /// <summary>
    /// A property handler that maps the MySQL spatial types (<c>GEOMETRY</c>, <c>POINT</c>, <c>LINESTRING</c>, <c>POLYGON</c>, <c>MULTIPOINT</c>, <c>MULTILINESTRING</c>, <c>MULTIPOLYGON</c> and <c>GEOMETRYCOLLECTION</c>) into a <see cref="MySqlGeometry"/> property.
    /// </summary>
    public class MySqlObjectToGeometryPropertyHandler : IPropertyHandler<object, MySqlGeometry>
    {
        /// <summary>
        /// Converts the spatial value, as returned by the driver, into a <see cref="MySqlGeometry"/>.
        /// </summary>
        /// <param name="input">The spatial value of the column, as returned by the driver (a <see cref="MySqlGeometry"/>, or a <see cref="byte"/> array in the MySQL internal format, which is a 4-byte SRID followed by the WKB).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="MySqlGeometry"/>, or a <see cref="MySqlGeometry"/> whose <see cref="MySqlGeometry.IsNull"/> is <c>true</c> when the value is <c>null</c> or not a supported spatial value.</returns>
        public MySqlGeometry Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case MySqlGeometry geometry:
                    return geometry;
                case byte[] bytes:
                    return new MySqlGeometry(MySqlDbType.Geometry, bytes);
                default:
                    return new MySqlGeometry(true);
            }
        }

        /// <summary>
        /// Converts the <see cref="MySqlGeometry"/> into the value written into the spatial column.
        /// </summary>
        /// <param name="input">The <see cref="MySqlGeometry"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="byte"/> array of the <see cref="MySqlGeometry.Value"/> (the MySQL internal format, which is a 4-byte SRID followed by the WKB), or <c>null</c> when its <see cref="MySqlGeometry.IsNull"/> is <c>true</c>.
        /// The bytes are written instead of the <see cref="MySqlGeometry"/> itself as the parameter of a spatial column is created as a binary parameter, for which the driver does not serialize a <see cref="MySqlGeometry"/> object.</returns>
        public object Set(MySqlGeometry input,
            PropertyHandlerSetOptions options) =>
            input.IsNull ? null : input.Value;
    }
}
