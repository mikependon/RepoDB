using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.ArrayBindSize"/>
    /// property via an entity property before the actual execution. Only meaningful for
    /// variable-length types (e.g. <c>Varchar2</c>, <c>Clob</c>, <c>Blob</c>) used in an Array Bind
    /// or PL/SQL Associative Array Bind execution; ignored for fixed-length types.
    /// </summary>
    public class ArrayBindSizeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArrayBindSizeAttribute"/> class.
        /// </summary>
        /// <param name="arrayBindSize">The maximum size, per element, of the bound array.</param>
        public ArrayBindSizeAttribute(int[] arrayBindSize)
            : base(typeof(OracleParameter), nameof(OracleParameter.ArrayBindSize), arrayBindSize)
        { }

        /// <summary>
        /// Gets the mapped array-bind size values of the parameter.
        /// </summary>
        public int[] ArrayBindSize => (int[])Value;
    }
}
