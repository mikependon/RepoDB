using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a handler for the property transformation.
    /// </summary>
    public class PropertyHandlerAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyHandlerAttribute"/> class.
        /// </summary>
        /// <param name="handlerType">The type of the handler.</param>
        public PropertyHandlerAttribute(Type handlerType)
        {
            HandlerType = handlerType;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the handler that is being used.
        /// </summary>
        public Type HandlerType { get; }

        #endregion
    }
}
