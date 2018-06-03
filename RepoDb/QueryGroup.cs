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
    /// <summary>
    /// A widely-used object for defining the groupings for the query expressions. This object is used by most of the repository operations
    /// to define the filtering and query expressions for the actual execution.
    /// </summary>
    public class QueryGroup : IQueryGroup
    {
        private bool _isFixed = false;

        /// <summary>
        /// Creates a new instance of <i>RepoDb.QueryGroup</i> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expressions.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expressions.</param>
        /// <param name="conjunction">
        /// The conjunction to be used for every group seperation. The value could be <i>AND</i> or <i>OR</i>.
        /// Uses the <i>RepoDb.Enumerations.Conjunction</i> enumeration values.
        /// </param>
        public QueryGroup(IEnumerable<IQueryField> queryFields, IEnumerable<IQueryGroup> queryGroups = null, Conjunction conjunction = Conjunction.And)
        {
            Conjunction = conjunction;
            QueryFields = queryFields;
            QueryGroups = queryGroups;
        }

        /// <summary>
        /// Gets the conjunction used by this object..
        /// </summary>
        public Conjunction Conjunction { get; }

        /// <summary>
        /// Gets the list of fields being grouped by this object.
        /// </summary>
        public IEnumerable<IQueryField> QueryFields { get; }

        /// <summary>
        /// Gets the list of child query groups being grouped by this object.
        /// </summary>
        public IEnumerable<IQueryGroup> QueryGroups { get; }

        /// <summary>
        /// Gets the text value of <i>RepoDb.Attributes.TextAttribute</i> implemented at the <i>Conjunction</i> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <i>RepoDb.Attributes.TextAttribute</i> text property.</returns>
        public string GetConjunctionText()
        {
            var textAttribute = typeof(Conjunction)
                .GetMembers()
                .First(member => member.Name.ToLower() == Conjunction.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }

        /// <summary>
        /// Gets the stringified format of the current instance. A formatted string for field-operation-parameter will be returned conjuncted by
        /// the value of the <i>Conjunction</i> property. Example, if the (Field=FirstName and the Operation=Like and the Conjunction=AND) then the
        /// following stringified string will be returned: (FirstName NOT LIKE @FirstName AND ....).
        /// </summary>
        /// <returns>A stringified formatted-text of the current instance.</returns>
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

        /// <summary>
        /// Gets all the child query groups associated on the current instance.
        /// </summary>
        /// <returns>An enumerable list of child query groups.</returns>
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

        /// <summary>
        /// Fixes the parameter naming convention. This method must be called atleast once prior the actual operation execution.
        /// Please note that every repository operation itself is calling this method before the actual execution.
        /// </summary>
        /// <returns>The current instance.</returns>
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

        /// <summary>
        /// This method is used to parse the customized query tree expression. This method expects a dynamic object and converts it to the actual
        /// <i>RepoDb.Interfaces.IQueryGroup</i> that defines the query tree expression.
        /// </summary>
        /// <param name="obj">A dynamic query tree expression. Ex:
        /// new {
        ///     Conjunction = Conjunction.And,
        ///     Company = "Microsoft",
        ///     FirstName = new { Operation = Operation.Like, Value = "An%" },
        ///     UpdatedDate = new { Operation = Operation.LessThan, Value = DateTime.UtcNow.Date }
        /// }
        /// </param>
        /// <returns></returns>
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
                    if (type?.IsGenericType == false)
                    {
                        queryFields.Add(new QueryField(fieldName, value));
                    }
                    else
                    {
                        var operationProperty = type?
                            .GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower() == Constant.Operation.ToLower());
                        var valueProperty = type?
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