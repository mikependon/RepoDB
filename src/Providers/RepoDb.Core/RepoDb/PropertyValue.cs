#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the extracted value of the class property. It is referencing the instance of the <see cref="ClassProperty"/> object.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="PropertyValue"/> class.
    /// </remarks>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="property">The actual property object.</param>
    public class PropertyValue(string name,
        object value,
        ClassProperty property)
    {

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value { get; set; } = value;

        /// <summary>
        /// Gets the actual property object.
        /// </summary>
        public ClassProperty Property { get; } = property;
    }
}
