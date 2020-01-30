using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Linq;

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
            Validate(handlerType);
            HandlerType = handlerType;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the handler that is being used.
        /// </summary>
        public Type HandlerType { get; }

        #endregion

        #region Methods

        private void Validate(Type handlerType)
        {
            var @interface = handlerType.GetInterfaces()
                .Where(e => e.FullName.StartsWith("RepoDb.Interfaces.IPropertyHandler"))
                .FirstOrDefault();
            if (@interface == null)
            {
                throw new InvalidTypeException($"Type '{handlerType.FullName}' must implement the '{typeof(IPropertyHandler<,>).FullName}' interface.");
            }
        }

        #endregion
    }
}
