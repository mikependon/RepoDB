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
            IEnumerable<DbField> dbFields) =>
            GetReturnKeyField(entityType) ?? GetReturnDbField(dbFields)?.AsField();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static Field GetReturnKeyField(Type entityType)
        {
            var primaryField = PrimaryCache.Get(entityType)?.AsField();
            var identityField = IdentityCache.Get(entityType)?.AsField();

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
        /// <param name="dbFields"></param>
        /// <returns></returns>
        private static DbField GetReturnDbField(IEnumerable<DbField> dbFields)
        {
            var primaryDbField = dbFields?.FirstOrDefault(f => f.IsPrimary);
            var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

            switch (GlobalConfiguration.Options.KeyColumnReturnBehavior)
            {
                case KeyColumnReturnBehavior.Primary:
                    return primaryDbField;
                case KeyColumnReturnBehavior.Identity:
                    return identityDbField;
                case KeyColumnReturnBehavior.PrimaryOrElseIdentity:
                    return primaryDbField ?? identityDbField;
                case KeyColumnReturnBehavior.IdentityOrElsePrimary:
                    return identityDbField ?? primaryDbField;
                default:
                    throw new InvalidOperationException(nameof(GlobalConfiguration.Options.KeyColumnReturnBehavior));
            }
        }

        #endregion
    }
}
