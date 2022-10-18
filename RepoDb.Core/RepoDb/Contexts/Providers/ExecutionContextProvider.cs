using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Contexts.Providers
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ExecutionContextProvider
    {
        #region KeyColumnReturnBehavior

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        public static Field GetTargetReturnColumnAsField(Type entityType,
            IEnumerable<DbField> dbFields)
        {
            var primaryField = GetPrimaryAsReturnKeyField(entityType, dbFields);
            var identityField = GetIdentityAsReturnKeyField(entityType, dbFields);

            switch (GlobalConfiguration.Options.KeyColumnReturnBehavior)
            {
                case KeyColumnReturnBehavior.Primary:
                    return primaryField;
                case KeyColumnReturnBehavior.Identity:
                    return identityField;
                case KeyColumnReturnBehavior.PrimaryOrElseIdentity:
                    return primaryField ?? identityField;
                case KeyColumnReturnBehavior.IdentityOrElsePrimary:
                    return identityField ?? primaryField;
                default:
                    throw new InvalidOperationException(nameof(GlobalConfiguration.Options.KeyColumnReturnBehavior));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static Field GetPrimaryAsReturnKeyField(Type entityType,
            IEnumerable<DbField> dbFields) =>
            PrimaryCache.Get(entityType)?.AsField() ??
                dbFields?.FirstOrDefault(f => f.IsPrimary)?.AsField();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static Field GetIdentityAsReturnKeyField(Type entityType,
            IEnumerable<DbField> dbFields) =>
            IdentityCache.Get(entityType)?.AsField() ??
                dbFields?.FirstOrDefault(f => f.IsIdentity)?.AsField();

        #endregion
    }
}
