using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to convert property values.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Set from property type to target converter.
        /// </summary>
        /// <typeparam name="TProperty">from property type</typeparam>
        /// <param name="func">from converter</param>
        /// <returns>sql property</returns>
        IConverter Convert<TProperty>(Func<TProperty, object> func);

        /// <summary>
        /// Set to property type from source converter.
        /// </summary>
        /// <typeparam name="TFrom">from property type</typeparam>
        /// <typeparam name="TProperty">to property type</typeparam>
        /// <param name="func">to converter</param>
        /// <returns>sql property</returns>
        IConverter Convert<TFrom, TProperty>(Func<TFrom, TProperty> func);
    }
}