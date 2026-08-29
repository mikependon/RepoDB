#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define the mappings for the target class or .NET CLR type in a fluent manner.
    /// </summary>
    public static class FluentMapper
    {
        /// <summary>
        /// Defines the target class where to apply the mappings.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>An instance of <see cref="EntityMapFluentDefinition{TEntity}"/> object.</returns>
        public static EntityMapFluentDefinition<TEntity> Entity<TEntity>()
            where TEntity : class => new();

        /// <summary>
        /// Defines the target .NET CLR type where to apply the mappings.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <returns>An instance of <see cref="TypeMapFluentDefinition{TType}"/> object.</returns>
        public static TypeMapFluentDefinition<TType> Type<TType>() => new();
    }
}
