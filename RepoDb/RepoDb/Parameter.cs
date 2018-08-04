using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// An object that holds the value of the field parameter.
    /// </summary>
    public sealed class Parameter
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Parameter</i> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public Parameter(string name, object value) : this(name, value, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.Parameter</i> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="appendPrefix">The value to identify whether the underscope prefix will be appended.</param>
        internal Parameter(string name, object value, bool appendPrefix)
        {
            Name = name.AsQuotedParameter(true);
            Value = value;
            if (appendPrefix)
            {
                AppendPrefix();
            }
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Force to append prefix on the current parameter object.
        /// </summary>
        internal void AppendPrefix()
        {
            if (!Name.StartsWith("_"))
            {
                Name = $"_{Name}";
            }
        }

        /// <summary>
        /// Stringify the current object. Will return the format of <b>Name (Value)</b> text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"@{Name} ({Value})";
        }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>Parameter</i>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>Parameter</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return Name.GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>Parameter</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Parameter other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>Parameter</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>Parameter</i> object.</param>
        /// <param name="objB">The second <i>Parameter</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Parameter objA, Parameter objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>Parameter</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>Parameter</i> object.</param>
        /// <param name="objB">The second <i>Parameter</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Parameter objA, Parameter objB)
        {
            return (objA == objB) == false;
        }
    }
}