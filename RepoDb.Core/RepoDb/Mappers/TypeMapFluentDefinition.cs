using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define a type-level mappings (ie: DB Type and Property Handler). Use this class if you wish to define the mappings in a fluent manner and avoid the models be decorated by the attributes.
    /// </summary>
    /// <typeparam name="TType">The target .NET CLR type to be mapped.</typeparam>
    public class TypeMapFluentDefinition<TType>
    {
        #region Properties

        /// <summary>
        /// Gets the current .NET CLR type.
        /// </summary>
        public Type Type { get; } = typeof(TType);

        #endregion

        #region DbType

        /// <summary>
        /// Defines a mapping between a .NET CLR type and a <see cref="System.Data.DbType"/> object.
        /// </summary>
        /// <param name="dbType">The <see cref="System.Data.DbType"/> object where to map the .NET CLR type.</param>
        /// <returns>The current instance.</returns>
        public TypeMapFluentDefinition<TType> DbType(DbType? dbType) =>
            DbType(dbType, false);

        /// <summary>
        /// Defines a mapping between a .NET CLR type and a <see cref="System.Data.DbType"/> object.
        /// </summary>
        /// <param name="dbType">The <see cref="System.Data.DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public TypeMapFluentDefinition<TType> DbType(DbType? dbType,
            bool force)
        {
            TypeMapper.Add<TType>(dbType, force);
            return this;
        }

        #endregion

        #region PropertyHandler

        /// <summary>
        /// Defines a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure that the default constructor is available for the property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>The current instance.</returns>
        public TypeMapFluentDefinition<TType> PropertyHandler<TPropertyHandler>() =>
            PropertyHandler<TPropertyHandler>(Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Defines a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure that the default constructor is available for the property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public TypeMapFluentDefinition<TType> PropertyHandler<TPropertyHandler>(bool force) =>
            PropertyHandler<TPropertyHandler>(Activator.CreateInstance<TPropertyHandler>(), force);

        /// <summary>
        /// Defines a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <returns>The current instance.</returns>
        public TypeMapFluentDefinition<TType> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler) =>
            PropertyHandler<TPropertyHandler>(propertyHandler, false);

        /// <summary>
        /// Defines a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public TypeMapFluentDefinition<TType> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force)
        {
            PropertyHandlerMapper.Add<TType, TPropertyHandler>(propertyHandler, force);
            return this;
        }

        #endregion
    }
}
