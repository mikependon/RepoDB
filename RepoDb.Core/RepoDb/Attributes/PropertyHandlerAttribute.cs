#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a handler for the property transformation.
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerType"></param>
        private void Validate(Type handlerType)
        {
            if (handlerType?.IsInterfacedTo(StaticType.IPropertyHandler) != true)
            {
                throw new InvalidTypeException($"Type '{handlerType.FullName}' must implement the '{StaticType.IPropertyHandler}' interface.");
            }
        }

        #endregion
    }
}
