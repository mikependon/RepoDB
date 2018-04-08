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

        public IQueryGroup FixParameters()
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
            var reserves = new[] { Constant.Conjunction, Constant.QueryGroups };
            var properties = obj.GetType().GetProperties().ToList();
            properties.ForEach(property =>
            {
                var fieldName = property.Name;
                if (!reserves.Contains(fieldName, StringComparer.InvariantCultureIgnoreCase))
                {
                    var value = property.GetValue(obj);
                    var type = value?.GetType();
                    queryFields.Add(type.IsGenericType ? QueryField.Parse(fieldName, value) : new QueryField(fieldName, value));
                }
                else
                {
                    if (string.Equals(fieldName, Constant.Conjunction, StringComparison.InvariantCultureIgnoreCase))
                    {
                        conjunction = (Conjunction)property.GetValue(obj);
                    }
                    else if (string.Equals(fieldName, Constant.QueryGroups, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var value = property.GetValue(obj);
                        if (value is Array)
                        {
                            Array.ForEach<object>((object[])value, (item) =>
                            {
                                queryGroups.Add(Parse(item));
                            });
                        }
                        else
                        {
                            queryGroups.Add(Parse(value));
                        }
                    }
                }
            });
            return new QueryGroup(queryFields, queryGroups, conjunction);
        }
    }
}