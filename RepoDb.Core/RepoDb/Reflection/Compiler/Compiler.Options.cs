using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    internal partial class Compiler
    {
        #region ClassHandlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerExpression"></param>
        /// <returns></returns>
        internal static Expression CreateClassHandlerGetOptionsExpression(Expression readerExpression)
        {
            // Get the 'Create' method
            var method = StaticType.ClassHandlerGetOptions.GetMethod("Create",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Set to default
            readerExpression ??= Expression.Default(StaticType.DbDataReader);

            // Call the method
            return Expression.Call(method, readerExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandExpression"></param>
        /// <returns></returns>
        internal static Expression CreateClassHandlerSetOptionsExpression(Expression commandExpression)
        {
            // Get the 'Create' method
            var method = StaticType.ClassHandlerSetOptions.GetMethod("Create",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Set to default
            commandExpression ??= Expression.Default(StaticType.IDbDataParameter);

            // Call the method
            return Expression.Call(method, commandExpression);
        }

        #endregion

        #region PropertyHandlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerExpression"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static Expression CreatePropertyHandlerGetOptionsExpression(Expression readerExpression,
            ClassProperty classProperty) =>
            CreatePropertyHandlerGetOptionsExpression(readerExpression,
                classProperty == null ? null : Expression.Constant(classProperty));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerExpression"></param>
        /// <param name="classPropertyExpression"></param>
        /// <returns></returns>
        internal static Expression CreatePropertyHandlerGetOptionsExpression(Expression readerExpression,
            Expression classPropertyExpression)
        {
            // Get the 'Create' method
            var method = StaticType.PropertyHandlerGetOptions.GetMethod("Create",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Set to default
            readerExpression ??= Expression.Default(StaticType.DbDataReader);
            classPropertyExpression ??= Expression.Default(StaticType.ClassProperty);

            // Call the method
            return Expression.Call(method, readerExpression, classPropertyExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterExpression"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static Expression CreatePropertyHandlerSetOptionsExpression(Expression parameterExpression,
            ClassProperty classProperty) =>
            CreatePropertyHandlerSetOptionsExpression(parameterExpression,
                classProperty == null ? null : Expression.Constant(classProperty));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterExpression"></param>
        /// <param name="classPropertyExpression"></param>
        /// <returns></returns>
        internal static Expression CreatePropertyHandlerSetOptionsExpression(Expression parameterExpression,
            Expression classPropertyExpression)
        {
            // Get the 'Create' method
            var method = StaticType.PropertyHandlerSetOptions.GetMethod("Create",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Set to default
            parameterExpression ??= Expression.Default(StaticType.IDbDataParameter);
            classPropertyExpression ??= Expression.Default(StaticType.ClassProperty);

            // Call the method
            return Expression.Call(method, parameterExpression, classPropertyExpression);
        }

        #endregion
    }
}
