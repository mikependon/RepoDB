#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;

namespace RepoDb
{
    /// <summary>
    /// Shared constants for the trimming and NativeAOT annotations of the library.
    /// </summary>
    internal static class Trimming
    {
        /// <summary>
        /// The members of a data entity (or result) type that are accessed via reflection by the library: the public
        /// properties (mapping, getters and setters) and the public constructors (materialization).
        /// </summary>
        public const DynamicallyAccessedMemberTypes Entity =
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicConstructors;

        /// <summary>
        /// The members of a handler type (i.e.: <see cref="Interfaces.IPropertyHandler{TInput, TResult}"/> and
        /// <see cref="Interfaces.IClassHandler{TEntity}"/>) that are accessed via reflection by the library: the public
        /// parameterless constructor (instantiation) and the implemented interfaces (validation).
        /// </summary>
        public const DynamicallyAccessedMemberTypes Handler =
            DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
            DynamicallyAccessedMemberTypes.Interfaces;

        /// <summary>
        /// The message for the APIs that reflect over the runtime type of a plain <see cref="object"/>.
        /// </summary>
        public const string ObjectReflectionMessage =
            "The properties of the runtime type of the passed object (i.e.: an anonymous type or a class instance passed as a 'param', 'what', 'where' or 'entity' argument) " +
            "are discovered via reflection and might be trimmed. When trimming or publishing with NativeAOT, prefer the generic or the lambda expression based overloads. " +
            "This warning can be safely suppressed when the passed object is null, a primitive (key) value, a Dictionary<string, object>, an ExpandoObject, " +
            "a QueryField, an enumerable of QueryField or a QueryGroup.";

        /// <summary>
        /// The message for the APIs that require dynamic code generation.
        /// </summary>
        public const string DynamicCodeMessage =
            "This API constructs generic types or methods at runtime that may not be available when publishing with NativeAOT.";
    }
}
