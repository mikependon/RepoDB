using System;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to hold the definition of the <see cref="DbCommand"/> parameters.
    /// </summary>
    internal class CommandParameter
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandParameter"/> class.
        /// </summary>
        /// <param name="field">The <see cref="Field"/> object that is connected.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="mappedToType">The parent type where this parameter is mapped.</param>
        public CommandParameter(Field field,
            object value,
            Type mappedToType)
        {
            Field = field;
            Value = value;
            MappedToType = mappedToType;
        }

        /// <summary>
        /// The field that is connected.
        /// </summary>
        public Field Field { get; set; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The parent type where this parameter is mapped.
        /// </summary>
        public Type MappedToType { get; set; }
    }
}
