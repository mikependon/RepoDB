#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a handler for the property transformation.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ClassHandlerAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClassHandlerAttribute"/> class.
        /// </summary>
        /// <param name="handlerType">The type of the handler.</param>
        public ClassHandlerAttribute([DynamicallyAccessedMembers(Trimming.Handler)] Type handlerType)
        {
            Validate(handlerType);
            HandlerType = handlerType;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the handler that is being used.
        /// </summary>
        [DynamicallyAccessedMembers(Trimming.Handler)]
        public Type HandlerType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerType"></param>
        private static void Validate([DynamicallyAccessedMembers(Trimming.Handler)] Type handlerType)
        {
            if (handlerType?.IsInterfacedTo(StaticType.IClassHandler) != true)
            {
                throw new InvalidTypeException($"Type '{handlerType.FullName}' must implement the '{StaticType.IClassHandler}' interface.");
            }
        }

        #endregion
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// An internal contract to access the handler type of the <see cref="ClassHandlerAttribute{T}"/> without reflection.
    /// </summary>
    internal interface IGenericClassHandlerAttribute
    {
        /// <summary>
        /// Gets the type of the handler.
        /// </summary>
        [DynamicallyAccessedMembers(Trimming.Handler)]
        Type HandlerType { get; }
    }

    /// <summary>
    /// An attribute that is used to define a handler for the property transformation.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ClassHandlerAttribute<[DynamicallyAccessedMembers(Trimming.Handler)] T> : Attribute, IGenericClassHandlerAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClassHandlerAttribute{T}"/> class.
        /// </summary>
        public ClassHandlerAttribute() => Validate(typeof(T));

        #region Properties

        /// <summary>
        /// Gets the type of the handler.
        /// </summary>
        [DynamicallyAccessedMembers(Trimming.Handler)]
        public Type HandlerType => typeof(T);

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerType"></param>
        private static void Validate([DynamicallyAccessedMembers(Trimming.Handler)] Type handlerType)
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
