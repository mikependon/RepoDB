using RepoDb.Extensions;
using RepoDb.Interfaces;

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
    }
}