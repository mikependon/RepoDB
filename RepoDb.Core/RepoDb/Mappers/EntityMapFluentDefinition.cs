using RepoDb.Attributes.Parameter;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define the class mappings (ie: Table, Column, Primary, Identity, DB Type and Property Handler).
    /// Use this class if you wish to avoid decorating an attribute into the models.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public class EntityMapFluentDefinition<TEntity>
        where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the current type of the class.
        /// </summary>
        public Type EntityType { get; } = typeof(TEntity);

        #endregion

        #region Table

        /// <summary>
        /// Defines a mapping between a class and a database object (i.e.: Table, View).
        /// </summary>
        /// <param name="name">The name of the database object (i.e.: Table, View).</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Table(string name) =>
            Table(name, false);

        /// <summary>
        /// Defines a mapping between a class and a database object.
        /// </summary>
        /// <param name="name">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Table(string name,
            bool force)
        {
            ClassMapper.Add<TEntity>(name, force);
            return this;
        }

        #endregion

        #region Column

        /*
         * Expression
         */

        /// <summary>
        /// Defines a mapping between a class property and a database column.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Column(Expression<Func<TEntity, object>> expression,
            string columnName) =>
            Column(expression, columnName, false);

        /// <summary>
        /// Defines a mapping between a class property and a database column.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Column(Expression<Func<TEntity, object>> expression,
            string columnName,
            bool force)
        {
            PropertyMapper.Add<TEntity>(expression, columnName, force);
            return this;
        }

        /*
         * PropertyName
         */

        /// <summary>
        /// Defines a mapping between a class property and a database column (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Column(string propertyName,
            string columnName) =>
            Column(propertyName, columnName, false);

        /// <summary>
        /// Defines a mapping between a class property and a database column (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Column(string propertyName,
            string columnName,
            bool force)
        {
            PropertyMapper.Add<TEntity>(propertyName, columnName, force);
            return this;
        }

        /*
         * Field
         */

        /// <summary>
        /// Defines a mapping between a class property and a database column (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Column(Field field,
            string columnName) =>
            Column(field, columnName, false);

        /// <summary>
        /// Defines a mapping between a class property and a database column (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="columnName">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Column(Field field,
            string columnName,
            bool force)
        {
            PropertyMapper.Add<TEntity>(field, columnName, force);
            return this;
        }

        #endregion

        #region Primary

        /*
         * Expression
         */

        /// <summary>
        /// Defines the class primary property.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Primary(Expression<Func<TEntity, object>> expression) =>
            Primary(expression, false);

        /// <summary>
        /// Defines the class primary property.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Primary(Expression<Func<TEntity, object>> expression,
            bool force)
        {
            PrimaryMapper.Add<TEntity>(expression, force);
            return this;
        }

        /*
         * PropertyName
         */

        /// <summary>
        /// Defines the class primary property (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Primary(string propertyName) =>
            Primary(propertyName, false);

        /// <summary>
        /// Defines the class primary property (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Primary(string propertyName,
            bool force)
        {
            PrimaryMapper.Add<TEntity>(propertyName, force);
            return this;
        }

        /*
         * Field
         */

        /// <summary>
        /// Defines the class primary property (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Primary(Field field) =>
            Primary(field, false);

        /// <summary>
        /// Defines the class primary property (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Primary(Field field,
            bool force)
        {
            PrimaryMapper.Add<TEntity>(field, force);
            return this;
        }

        #endregion

        #region Identity

        /*
         * Expression
         */

        /// <summary>
        /// Defines the class identity property.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Identity(Expression<Func<TEntity, object>> expression) =>
            Identity(expression, false);

        /// <summary>
        /// Defines the class identity property.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Identity(Expression<Func<TEntity, object>> expression,
            bool force)
        {
            IdentityMapper.Add<TEntity>(expression, force);
            return this;
        }

        /*
         * PropertyName
         */

        /// <summary>
        /// Defines the class identity property (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Identity(string propertyName) =>
            Identity(propertyName, false);

        /// <summary>
        /// Defines the class identity property (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Identity(string propertyName,
            bool force)
        {
            IdentityMapper.Add<TEntity>(propertyName, force);
            return this;
        }

        /*
         * Field
         */

        /// <summary>
        /// Defines the class identity property (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Identity(Field field) =>
            Identity(field, false);

        /// <summary>
        /// Defines the class identity property (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> Identity(Field field,
            bool force)
        {
            IdentityMapper.Add<TEntity>(field, force);
            return this;
        }

        #endregion

        #region DbType

        /*
         * Expression
         */

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="System.Data.DbType"/> object.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbType">The target database type.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> DbType(Expression<Func<TEntity, object>> expression,
            DbType? dbType) =>
            DbType(expression, dbType, false);

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="System.Data.DbType"/> object.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> DbType(Expression<Func<TEntity, object>> expression,
            DbType? dbType,
            bool force)
        {
            TypeMapper.Add<TEntity>(expression, dbType, force);
            return this;
        }

        /*
         * PropertyName
         */

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="System.Data.DbType"/> object (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> DbType(string propertyName,
            DbType? dbType) =>
            DbType(propertyName, dbType, false);

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="System.Data.DbType"/> object (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> DbType(string propertyName,
            DbType? dbType,
            bool force)
        {
            TypeMapper.Add<TEntity>(propertyName, dbType, force);
            return this;
        }

        /*
         * Field
         */

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="System.Data.DbType"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> DbType(Field field,
            DbType? dbType) =>
            DbType(field, dbType, false);


        /// <summary>
        /// Defines a mapping between a class property and a <see cref="System.Data.DbType"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> DbType(Field field,
            DbType? dbType,
            bool force)
        {
            TypeMapper.Add<TEntity>(field, dbType, force);
            return this;
        }

        #endregion

        #region ClassHandler

        /// <summary>
        /// Defines a mapping between a class and a <see cref="IClassHandler{TEntity}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IClassHandler{TEntity}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IClassHandler{TEntity}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TClassHandler">The type of the <see cref="IClassHandler{TEntity}"/>.</typeparam>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> ClassHandler<TClassHandler>()
            where TClassHandler : new() =>
            ClassHandler(new TClassHandler());

        /// <summary>
        /// Defines a mapping between a class and a <see cref="IClassHandler{TEntity}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IClassHandler{TEntity}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IClassHandler{TEntity}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TClassHandler">The type of the <see cref="IClassHandler{TEntity}"/>.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> ClassHandler<TClassHandler>(bool force)
            where TClassHandler : new() =>
            ClassHandler(new TClassHandler(), force);

        /// <summary>
        /// Defines a mapping between a class and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <typeparam name="TClassHandler">The type of the <see cref="IClassHandler{TEntity}"/>.</typeparam>
        /// <param name="classHandler">The instance of the <see cref="IClassHandler{TEntity}"/>.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> ClassHandler<TClassHandler>(TClassHandler classHandler) =>
            ClassHandler<TClassHandler>(classHandler, false);

        /// <summary>
        /// Defines a mapping between a class and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <typeparam name="TClassHandler">The type of the <see cref="IClassHandler{TEntity}"/>.</typeparam>
        /// <param name="classHandler">The instance of the <see cref="IClassHandler{TEntity}"/>.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> ClassHandler<TClassHandler>(TClassHandler classHandler,
            bool force)
        {
            ClassHandlerMapper.Add<TEntity, TClassHandler>(classHandler, force);
            return this;
        }

        #endregion

        #region PropertyHandler

        /*
         * Expression
         */

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="IPropertyHandler{TInput, TResult}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IPropertyHandler{TInput, TResult}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IPropertyHandler{TInput, TResult}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Expression<Func<TEntity, object>> expression)
            where TPropertyHandler : new() =>
            PropertyHandler(expression, new TPropertyHandler());

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="IPropertyHandler{TInput, TResult}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IPropertyHandler{TInput, TResult}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IPropertyHandler{TInput, TResult}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            bool force)
            where TPropertyHandler : new() =>
            PropertyHandler(expression, new TPropertyHandler(), force);

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the <see cref="IPropertyHandler{TInput, TResult}"/>.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler) =>
            PropertyHandler<TPropertyHandler>(expression, propertyHandler, false);

        /// <summary>
        /// Defines a mapping between a class property and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the <see cref="IPropertyHandler{TInput, TResult}"/>.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler,
            bool force)
        {
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(expression, propertyHandler, force);
            return this;
        }

        /*
         * PropertyName
         */

        /// <summary>
        /// Adds a <see cref="IPropertyHandler{TInput, TResult}"/> mapping into a class property (via property name). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IPropertyHandler{TInput, TResult}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IPropertyHandler{TInput, TResult}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(string propertyName)
            where TPropertyHandler : new() =>
            PropertyHandler(propertyName, new TPropertyHandler(), false);

        /// <summary>
        /// Adds a <see cref="IPropertyHandler{TInput, TResult}"/> mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="propertyHandler">The instance of the <see cref="IPropertyHandler{TInput, TResult}"/>.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler) =>
            PropertyHandler<TPropertyHandler>(propertyName, propertyHandler, false);

        /// <summary>
        /// Adds a <see cref="IPropertyHandler{TInput, TResult}"/> mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="propertyHandler">The instance of the <see cref="IPropertyHandler{TInput, TResult}"/>.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler,
            bool force)
        {
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(propertyName, propertyHandler, force);
            return this;
        }

        /*
         * Field
         */

        /// <summary>
        /// Adds a <see cref="IPropertyHandler{TInput, TResult}"/> mapping into a class property (via <see cref="Field"/> object). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IPropertyHandler{TInput, TResult}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IPropertyHandler{TInput, TResult}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Field field)
            where TPropertyHandler : new() =>
            PropertyHandler(field, new TPropertyHandler(), false);

        /// <summary>
        /// Adds a <see cref="IPropertyHandler{TInput, TResult}"/> mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="propertyHandler">The instance of the <see cref="IPropertyHandler{TInput, TResult}"/>.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler) =>
            PropertyHandler<TPropertyHandler>(field, propertyHandler, false);

        /// <summary>
        /// Adds a <see cref="IPropertyHandler{TInput, TResult}"/> mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the <see cref="IPropertyHandler{TInput, TResult}"/>.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="propertyHandler">The instance of the <see cref="IPropertyHandler{TInput, TResult}"/>.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyHandler<TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler,
            bool force)
        {
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(field, propertyHandler, force);
            return this;
        }

        #endregion

        #region PropertyValueAttributes

        /*
         * Expression
         */

        /// <summary>
        /// Defines the class property parameter value attributes.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="attributes">The list of property value attributes to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyValueAttributes(Expression<Func<TEntity, object>> expression,
            IEnumerable<PropertyValueAttribute> attributes) =>
            PropertyValueAttributes(expression, attributes, false);

        /// <summary>
        /// Defines the class property parameter value attributes.
        /// </summary>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="attributes">The list of property value attributes to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyValueAttributes(Expression<Func<TEntity, object>> expression,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
        {
            PropertyValueAttributeMapper.Add<TEntity>(expression, attributes, force);
            return this;
        }

        /*
         * PropertyName
         */

        /// <summary>
        /// Defines the class property parameter value attributes (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="attributes">The list of property value attributes to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyValueAttributes(string propertyName,
            IEnumerable<PropertyValueAttribute> attributes) =>
            PropertyValueAttributes(propertyName, attributes, false);

        /// <summary>
        /// Defines the class property parameter value attributes (via property name).
        /// </summary>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="attributes">The list of property value attributes to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyValueAttributes(string propertyName,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
        {
            PropertyValueAttributeMapper.Add<TEntity>(propertyName, attributes, force);
            return this;
        }

        /*
         * Field
         */

        /// <summary>
        /// Defines the class property parameter value attributes (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="attributes">The list of property value attributes to be mapped.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyValueAttributes(Field field,
            IEnumerable<PropertyValueAttribute> attributes) =>
            PropertyValueAttributes(field, attributes, false);

        /// <summary>
        /// Defines the class property parameter value attributes (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="attributes">The list of property value attributes to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public EntityMapFluentDefinition<TEntity> PropertyValueAttributes(Field field,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
        {
            PropertyValueAttributeMapper.Add<TEntity>(field, attributes, force);
            return this;
        }

        #endregion
    }
}
