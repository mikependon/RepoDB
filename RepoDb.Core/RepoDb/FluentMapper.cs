namespace RepoDb
{
    /// <summary>
    /// A class that is used to define a mapping for the target data entity in a fluent way.
    /// </summary>
    public static class FluentMapper
    {
        /// <summary>
        /// Defines the target data entity where to apply the mappings.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>An instance of <see cref="EntityMapFluentDefinition{TEntity}"/> object.</returns>
        public static EntityMapFluentDefinition<TEntity> Entity<TEntity>()
            where TEntity : class
        {
            return new EntityMapFluentDefinition<TEntity>();
        }

        /// <summary>
        /// Defines the target .NET CLR type where to apply the mappings.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <returns>An instance of <see cref="TypeMapFluentDefinition{TType}"/> object.</returns>
        public static TypeMapFluentDefinition<TType> Type<TType>()
        {
            return new TypeMapFluentDefinition<TType>();
        }
    }
}
