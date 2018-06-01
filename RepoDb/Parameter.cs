using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// An object that holds the value of the field parameter.
    /// </summary>
    public class Parameter : IParameter
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Parameter</i> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public Parameter(string name, object value)
        {
            Name = $"_{name}";
            Value = value;
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
        /// Stringify the current object. Will return the format of <b>Name (Value)</b> text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"@{Name} ({Value})";
        }
    }
}