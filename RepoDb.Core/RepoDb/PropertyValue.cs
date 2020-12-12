namespace RepoDb
{
    /// <summary>
    /// A class that is being used to handle the extracted value of the class property. It is referencing the instance of the <see cref="ClassProperty"/> object.
    /// </summary>
    public class PropertyValue
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="property">The actual property object.</param>
        public PropertyValue(string name,
            object value,
            ClassProperty property)
        {
            Name = name;
            Value = value;
            Property = property;
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets the actual property object.
        /// </summary>
        public ClassProperty Property { get; }
    }
}
