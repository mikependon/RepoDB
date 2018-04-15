using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using System.Reflection;
using RepoDb.Attributes;

namespace RepoDb
{
    public class QueryGroup : IQueryGroup
    {
        private bool _isFixed = false;

        // Constructors
        public QueryGroup(IEnumerable<IQueryField> queryFields, IEnumerable<IQueryGroup> queryGroups = null, Conjunction conjunction = Conjunction.And)
        {
            Conjunction = conjunction;
            QueryFields = queryFields;
            QueryGroups = queryGroups;
        }

        // Properties
        public Conjunction Conjunction { get; }

        public IEnumerable<IQueryField> QueryFields { get; }

        public IEnumerable<IQueryGroup> QueryGroups { get; }

        // Methods
        public string GetConjunctionText()
        {
            var textAttribute = typeof(Conjunction)
                .GetMembers()
                .First(member => string.Equals(member.Name, Conjunction.ToString(), StringComparison.InvariantCultureIgnoreCase))
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }

        public string GetString()
        {
            var groupList = new List<string>();
            var conjunction = GetConjunctionText();
            if (QueryFields != null && QueryFields.Any())
            {
                var fieldList = new List<string>();
                QueryFields.ToList().ForEach(queryField =>
                {
                    fieldList.Add(queryField.AsFieldAndParameter());
                });
                groupList.Add(fieldList.Join($" {conjunction} "));
            }
            if (QueryGroups != null && QueryGroups.Any())
            {
                var fieldList = new List<string>();
                QueryGroups.ToList().ForEach(queryGroup =>
                {
                    fieldList.Add(queryGroup.GetString());
                });
                groupList.Add(fieldList.Join($" {conjunction} "));
            }
            return $"({groupList.Join($" {conjunction} ")})";
        }

        public IEnumerable<IQueryField> GetAllQueryFields()
        {
            var traverse = (Action<IQueryGroup>)null;
            var queryFields = new List<IQueryField>();
            traverse = new Action<IQueryGroup>((queryGroup) =>
            {
                if (queryGroup.QueryFields != null && queryGroup.QueryFields.Any())
                {
                    queryFields.AddRange(queryGroup.QueryFields);
                }
                if (queryGroup.QueryGroups != null && queryGroup.QueryGroups.Any())
                {
                    queryGroup.QueryGroups.ToList().ForEach(qg =>
                    {
                        traverse(qg);
                    });
                }
            });
            traverse(this);
            return queryFields;
        }

        public IQueryGroup Fix()
        {
            if (_isFixed)
            {
                return this;
            }
            var firstList = GetAllQueryFields()?
                .OrderBy(queryField => queryField.Parameter.Name)
                .ToList();
            if (firstList != null && firstList.Any())
            {
                var secondList = new List<IQueryField>(firstList);
                for (var i = 0; i < firstList.Count; i++)
                {
                    var iQueryField = firstList[i];
                    for (var c = 0; c < secondList.Count; c++)
                    {
                        var cQueryField = secondList[c];
                        if (iQueryField == cQueryField)
                        {
                            continue;
                        }
                        if (string.Equals(iQueryField.Field.Name, cQueryField.Field.Name,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            var fieldValue = ((Parameter)cQueryField.Parameter);
                            fieldValue.Name = $"{cQueryField.Parameter.Name}_{c}";
                        }
                    }
                    secondList.RemoveAll(queryField => string.Equals(iQueryField.Field.Name, queryField.Field.Name,
                        StringComparison.InvariantCultureIgnoreCase));
                }
            }
            _isFixed = true;
            return this;
        }

        // Static Methods

        public static IQueryGroup Parse(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"Parameter '{Constant.Obj.ToLower()}' cannot be null.");
            }
            var queryFields = new List<IQueryField>();
            var queryGroups = new List<IQueryGroup>();
            var conjunction = Conjunction.And;
            obj.GetType().GetProperties().ToList().ForEach(property =>
            {
                var fieldName = property.Name;
                if (string.Equals(fieldName, Constant.Conjunction, StringComparison.InvariantCultureIgnoreCase))
                {
                    conjunction = (Conjunction)property.GetValue(obj);
                }
                else if (string.Equals(fieldName, Constant.QueryGroups, StringComparison.InvariantCultureIgnoreCase))
                {
                    var value = property.GetValue(obj);
                    if (value is Array)
                    {
                        Array.ForEach((object[])value, (item) =>
                        {
                            queryGroups.Add(Parse(item));
                        });
                    }
                    else
                    {
                        queryGroups.Add(Parse(value));
                    }
                }
                else
                {
                    var value = property.GetValue(obj);
                    var type = value?.GetType();
                    if (type?.IsGenericType == false && type?.IsClass == false)
                    {
                        queryFields.Add(new QueryField(fieldName, value));
                    }
                    else
                    {
                        var operationProperty = type
                            .GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower() == Constant.Operation.ToLower());
                        var valueProperty = type
                            .GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower() == Constant.Value.ToLower());
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
                            throw new InvalidOperationException($"Value property must be present.");
                        }
                        var operation = (Operation)operationProperty.GetValue(value);
                        value = valueProperty.GetValue(value);
                        if (value == null)
                        {
                            throw new InvalidOperationException($"Value property must not be null.");
                        }
                        if (operation == Operation.All || operation == Operation.Any)
                        {
                            if (value.GetType().IsArray)
                            {
                                var qfs = new List<IQueryField>();
                                ((Array)value).AsEnumerable().ToList().ForEach(underlyingValue =>
                                {
                                    qfs.Add(QueryField.Parse(fieldName, underlyingValue));
                                });
                                var queryGroup = new QueryGroup(qfs, null, operation == Operation.All ? Conjunction.And : Conjunction.Or);
                                queryGroups.Add(queryGroup);
                            }
                            else
                            {
                                queryFields.Add(QueryField.Parse(fieldName, value));
                            }
                        }
                        else
                        {
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
                            queryFields.Add(new QueryField(fieldName, operation, value));
                        }
                    }
                }
            });
            return new QueryGroup(queryFields, queryGroups, conjunction);
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
                else
                {
                    if (values.Any(v => v == null || (bool)v?.GetType().IsGenericType))
                    {
                        throw new InvalidOperationException($"Invalid value for field {fieldName.AsField()} (Operation: {operation.ToString()}).");
                    }
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