using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb
{
    public class QueryField : IQueryField
    {
        public QueryField(string fieldName, object value)
            : this(fieldName, Operation.Equal, value)
        {
        }

        public QueryField(string fieldName, Operation operation, object value)
        {
            Field = new Field(fieldName);
            Operation = operation;
            Parameter = new Parameter(fieldName, value);
        }

        // Properties

        public IField Field { get; }

        public Operation Operation { get; }

        public IParameter Parameter { get; }

        // Methods

        public string GetOperationText()
        {
            var textAttribute = typeof(Operation)
                .GetMembers()
                .First(member => string.Equals(member.Name, Operation.ToString(), StringComparison.InvariantCultureIgnoreCase))
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }

        public override string ToString()
        {
            return $"{Field.ToString()} = {Parameter.ToString()}";
        }

        // Static Methods

        internal static IQueryField Parse(string fieldName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Value must not be null.");
            }
            var properties = value.GetType().GetProperties();
            var operationProperty = properties.FirstOrDefault(p => p.Name.ToLower() == Constant.Operation.ToLower());
            var valueProperty = properties.FirstOrDefault(p => p.Name.ToLower() == Constant.Value.ToLower());
            if (operationProperty == null)
            {
                throw new InvalidOperationException($"Operation property must be present.");
            }
            if (operationProperty.PropertyType != typeof(Operation))
            {
                throw new InvalidOperationException($"Operation property must be of type '{typeof(Operation).FullName}'.");
            }
            if (valueProperty == null)
            {
                throw new InvalidOperationException($"Value property must be present for field {fieldName.AsField()}.");
            }
            var operation = (Operation)operationProperty.GetValue(value);
            value = valueProperty.GetValue(value);
            if (operation == Operation.Between || operation == Operation.NotBetween)
            {
                ValidateBetweenOperations(fieldName, operation, value);
            }
            else if (operation == Operation.In || operation == Operation.NotIn)
            {
                ValidateInOperations(fieldName, operation, value);
            }
            else
            {
                ValidateOtherOperations(fieldName, operation, value);
            }
            return new QueryField(fieldName, operation, value);
        }

        private static void ValidateBetweenOperations(string fieldName, Operation operation, object value)
        {
            if (value.GetType().IsArray)
            {
                var values = ((Array)value).AsEnumerable().ToList();
                if (values.Count != 2)
                {
                    throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}). The count should be 2.");
                }
                if (values.Any(v => v == null || (bool)v?.GetType().IsGenericType))
                {
                    throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}).");
                }
            }
            else
            {
                throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}). Expecting an array values.");
            }
        }

        private static void ValidateInOperations(string fieldName, Operation operation, object value)
        {
            if (value.GetType().IsArray)
            {
                var values = ((Array)value).AsEnumerable().ToList();
                if (values.Any(v => v == null || (bool)v?.GetType().IsGenericType))
                {
                    throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}).");
                }
            }
            else
            {
                throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}). Expecting an array values.");
            }
        }

        private static void ValidateOtherOperations(string fieldName, Operation operation, object value)
        {
            if (value.GetType().IsGenericType)
            {
                throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}).");
            }
        }
    }
}