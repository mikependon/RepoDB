using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to define the query expression for all repository operations. It holds the instances of field (<i>RepoDb.Field</i>),
    /// parameter (<i>RepoDb.Parameter</i>) and the target operation (<i>RepoDb.Enumeration.Operation</i>) of the query expression.
    /// </summary>
    public class QueryField
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.QueryField</i> object./
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName, object value)
            : this(fieldName, Operation.Equal, value)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.QueryField</i> object./
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        public QueryField(string fieldName, Operation operation, object value)
            : this(fieldName, operation, value, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.QueryField</i> object./
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="appendParameterPrefix">
        /// The value to identify whether the underscope prefix will be appended to the parameter name.
        /// </param>
        internal QueryField(string fieldName, Operation operation, object value, bool appendParameterPrefix)
        {
            Field = new Field(fieldName);
            Operation = operation;
            Parameter = new Parameter(fieldName, value, appendParameterPrefix);
        }

        // Properties

        /// <summary>
        /// Gets the associated field object.
        /// </summary>
        public Field Field { get; }

        /// <summary>
        /// Gets the operation used by this instance.
        /// </summary>
        public Operation Operation { get; }

        /// <summary>
        /// Gets the associated parameter object.
        /// </summary>
        public Parameter Parameter { get; }

        // Methods

        /// <summary>
        /// Force to append prefix on the bound parameter object.
        /// </summary>
        internal void AppendParameterPrefix()
        {
            Parameter?.AppendPrefix();
        }

        /// <summary>
        /// Gets the text value of <i>RepoDb.Attributes.TextAttribute</i> implemented at the <i>Operation</i> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <i>RepoDb.Attributes.TextAttribute</i> text property.</returns>
        public string GetOperationText()
        {
            var textAttribute = typeof(Operation)
                .GetMembers()
                .First(member => member.Name.ToLower() == Operation.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }

        /// <summary>
        /// Stringify the current instance of this object. Will return the stringified format of field and parameter in combine.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Field.ToString()} = {Parameter.ToString()}";
        }

        // Static Methods

        internal static QueryField Parse(string fieldName, object value)
        {
            // The value must always be present
            if (value == null)
            {
                throw new ArgumentNullException($"The value must not be null for field ''{fieldName}''.");
            }

            // Another dynamic object type, get the 'Operation' property
            var properties = value.GetType().GetProperties();
            var operationProperty = properties?.FirstOrDefault(p => p.Name.ToLower() == StringConstant.Operation.ToLower());

            // The property 'Operation' must always be present
            if (operationProperty == null)
            {
                throw new InvalidQueryExpressionException($"Operation property must be present for field ''{fieldName}''.");
            }

            // The property operatoin must be of type 'RepoDb.Enumerations.Operation'
            if (operationProperty.PropertyType != typeof(Operation))
            {
                throw new InvalidQueryExpressionException($"The 'Operation' property for field ''{fieldName}'' must be of type '{typeof(Operation).FullName}'.");
            }

            // The 'Value' property must always be present
            var valueProperty = properties?.FirstOrDefault(p => p.Name.ToLower() == StringConstant.Value.ToLower());

            // Check for the 'Value' property
            if (valueProperty == null)
            {
                throw new InvalidQueryExpressionException($"The 'Value' property for dynamic type query must be present at field ''{fieldName}''.");
            }

            // Get the 'Operation' and the 'Value' value
            var operation = (Operation)operationProperty.GetValue(value);
            value = valueProperty.GetValue(value);

            // Identify the 'Operation' and parse the correct value
            if (operation == Operation.Between || operation == Operation.NotBetween)
            {
                // Special case: (Field.Name = new { Operation = Operation.<Between|NotBetween>, Value = new [] { value1, value2 })
                ValidateBetweenOperations(fieldName, operation, value);
            }
            else if (operation == Operation.In || operation == Operation.NotIn)
            {
                // Special case: (Field.Name = new { Operation = Operation.<In|NotIn>, Value = new [] { value1, value2 })
                ValidateInOperations(fieldName, operation, value);
            }
            else
            {
                // Other Operations
                ValidateOtherOperations(fieldName, operation, value);
            }

            // Return
            return new QueryField(fieldName, operation, value);
        }

        private static void ValidateBetweenOperations(string fieldName, Operation operation, object value)
        {
            var valid = false;

            // Make sure it is an Array
            if (value?.GetType().IsArray == true)
            {
                var values = ((Array)value)
                    .AsEnumerable()
                    .ToList();

                // The items must only be 2. There should be no NULL and no generic types
                if (values.Count == 2)
                {
                    valid = !values.Any(v => v == null || v?.GetType().IsGenericType == true);
                }

                // All type must be the same
                if (valid)
                {
                    var type = values.First().GetType();
                    values.ForEach(v =>
                    {
                        if (valid == false) return;
                        valid = v?.GetType() == type;
                    });
                }
            }

            // Throw an error if not valid
            if (valid == false)
            {
                throw new InvalidQueryExpressionException($"Invalid value for field '{fieldName}' for operation '{operation.ToString()}'. The value must be an array of 2 values with identitcal data types.");
            }
        }

        private static void ValidateInOperations(string fieldName, Operation operation, object value)
        {
            var valid = false;

            // Make sure it is an array
            if (value?.GetType().IsArray == true)
            {
                var values = ((Array)value)
                    .AsEnumerable()
                    .ToList();

                // Make sure there is not NULL and no generic types
                valid = !values.Any(v => v == null || v?.GetType().IsGenericType == true);

                // All type must be the same
                if (valid)
                {
                    var type = values.First().GetType();
                    values.ForEach(v =>
                    {
                        if (valid == false) return;
                        valid = v?.GetType() == type;
                    });
                }
            }

            // Throw an error if not valid
            if (valid == false)
            {
                throw new InvalidQueryExpressionException($"Invalid value for field '{fieldName}' for operation '{operation.ToString()}'. The value must be an array values with identitcal data types.");
            }
        }

        private static void ValidateOtherOperations(string fieldName, Operation operation, object value)
        {
            var valid = false;

            // Special for Equal and NonEqual
            if ((operation == Operation.Equal || operation == Operation.NotEqual) && value == null)
            {
                // Most likely new QueryField("Field.Name", null) or new { FieldName = (object)null }.
                // The SQL must be (@FieldName IS <NOT> NULL)
                valid = true;
            }
            else
            {
                // Must not be a generic
                valid = (value?.GetType().IsGenericType == false);
            }

            // Throw an error if not valid
            if (valid == false)
            {
                throw new InvalidQueryExpressionException($"Invalid value for field '{fieldName}' for operation '{operation.ToString()}'.");
            }
        }
    }
}