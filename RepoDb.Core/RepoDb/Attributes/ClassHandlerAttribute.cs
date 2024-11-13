﻿using System;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a handler for the property transformation.
    /// </summary>
    public class ClassHandlerAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClassHandlerAttribute"/> class.
        /// </summary>
        /// <param name="handlerType">The type of the handler.</param>
        public ClassHandlerAttribute(Type handlerType)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerType"></param>
        private static void Validate(Type handlerType)
        {
            if (handlerType?.IsInterfacedTo(StaticType.IClassHandler) != true)
            {
                throw new InvalidTypeException($"Type '{handlerType.FullName}' must implement the '{StaticType.IClassHandler}' interface.");
            }
        }

        #endregion
    }

#if NET
    /// <summary>
    /// An attribute that is used to define a handler for the property transformation.
    /// </summary>
    public class ClassHandlerAttribute<T> : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClassHandlerAttribute{T}"/> class.
        /// </summary>
        public ClassHandlerAttribute() => Validate(typeof(T));

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerType"></param>
        private static void Validate(Type handlerType)
        {
            if (handlerType?.IsInterfacedTo(StaticType.IClassHandler) != true)
            {
                throw new InvalidTypeException($"Type '{handlerType.FullName}' must implement the '{StaticType.IClassHandler}' interface.");
            }
        }

        #endregion
    }
#endif
}
