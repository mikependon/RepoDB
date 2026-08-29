using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.ParameterName"/>
    /// property via a class property mapping.
    /// </summary>
    public class NameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the mapping that is equivalent to the database object/field.</param>
        public NameAttribute(string name)
            : base(typeof(DbParameter), nameof(DbParameter.ParameterName), name, false)
        { }

        /// <summary>
        /// Gets the mapped name of the equivalent database object/field.
        /// </summary>
        public string Name => (string)Value;

        /// <summary>
        /// In practice this attribute is always excluded from the compiled (entity/dictionary) parameter-assignment
        /// path, since its <c>PropertyName</c> is <see cref="DbParameter.ParameterName"/> (see
        /// <c>Compiler.GetParameterPropertyValueSetterAttributesAssignmentExpressions</c>), so this parameterless
        /// overload is not expected to run for a real parameter-name assignment. It falls back to the "@"-prefixed
        /// SQL-text convention only as a defensive default. Prefer <see cref="GetValue(IDbSetting)"/>, which is what
        /// the runtime (non-compiled) <c>QueryField</c>/dynamic-parameter path actually invokes.
        /// </summary>
        /// <returns></returns>
        internal override object GetValue() => Name.AsParameter();

        /// <summary>
        /// Builds the actual <see cref="DbParameter.ParameterName"/> value for the current provider, honoring
        /// <see cref="IDbSetting.ParameterPrefix"/> (which, for a provider such as ClickHouse, is <see cref="string.Empty"/>
        /// rather than "@").
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal override object GetValue(IDbSetting dbSetting) => Name.AsParameterName(dbSetting);
    }
}