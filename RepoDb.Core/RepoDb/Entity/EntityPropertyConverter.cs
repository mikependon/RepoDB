using System;
using RepoDb.Interfaces;

namespace RepoDb.Entity
{
    internal class EntityPropertyConverter : IConverter
    {
        /// <summary>
        /// 
        /// </summary>
        internal object ToProperty { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal object ToObject { get; private set; }

        public IConverter Convert<TProperty>(Func<TProperty, object> func)
        {
            ToObject = func;
            return this;
        }

        public IConverter Convert<TFrom, TProperty>(Func<TFrom, TProperty> func)
        {
            ToProperty = func;
            return this;
        }
    }
}
