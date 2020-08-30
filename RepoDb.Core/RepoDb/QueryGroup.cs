using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System.Linq.Expressions;
using System.Reflection;
using System.Dynamic;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A widely-used object for defining the groupings for the query expression. This object is used by most of the repository operations
    /// to define the filtering and query expressions for the actual execution.
    /// </summary>
    public class QueryGroup : IEquatable<QueryGroup>
    {
        private bool isFixed = false;
        private int? hashCode = null;
        private TextAttribute conjuctionTextAttribute = null;

        #region Constructors

        /** QueryField **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        public QueryGroup(QueryField queryField) :
            this(queryField?.AsEnumerable(),
                (IEnumerable<QueryGroup>)null,
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        public QueryGroup(QueryField queryField,
            QueryGroup queryGroup) :
            this(queryField?.AsEnumerable(),
                queryGroup?.AsEnumerable(),
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(QueryField queryField,
            Conjunction conjunction) :
            this(queryField?.AsEnumerable(),
                (IEnumerable<QueryGroup>)null,
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryField queryField,
            bool isNot) :
            this(queryField?.AsEnumerable(),
                (IEnumerable<QueryGroup>)null,
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryField queryField,
            Conjunction conjunction,
            bool isNot) :
            this(queryField?.AsEnumerable(),
                (IEnumerable<QueryGroup>)null,
                conjunction,
                isNot)
        { }

        /** QueryFields **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields) :
            this(queryFields,
                (IEnumerable<QueryGroup>)null,
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            QueryGroup queryGroup) :
            this(queryFields,
                queryGroup?.AsEnumerable(),
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            Conjunction conjunction) :
            this(queryFields,
                (IEnumerable<QueryGroup>)null,
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            bool isNot) :
            this(queryFields,
                (IEnumerable<QueryGroup>)null,
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            Conjunction conjunction,
            bool isNot) :
            this(queryFields,
                (IEnumerable<QueryGroup>)null,
                conjunction,
                isNot)
        { }

        /** QueryGroup **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        public QueryGroup(QueryGroup queryGroup) :
            this((IEnumerable<QueryField>)null,
                queryGroup?.AsEnumerable(),
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(QueryGroup queryGroup,
            Conjunction conjunction) :
            this((IEnumerable<QueryField>)null,
                queryGroup?.AsEnumerable(),
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryGroup queryGroup,
            bool isNot) :
            this((IEnumerable<QueryField>)null,
                queryGroup?.AsEnumerable(),
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryGroup queryGroup,
            Conjunction conjunction,
            bool isNot) :
            this((IEnumerable<QueryField>)null,
                queryGroup?.AsEnumerable(),
                conjunction,
                isNot)
        { }

        /** QueryGroups **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        public QueryGroup(IEnumerable<QueryGroup> queryGroups) :
            this((IEnumerable<QueryField>)null,
                queryGroups,
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(IEnumerable<QueryGroup> queryGroups,
            Conjunction conjunction) :
            this((IEnumerable<QueryField>)null,
                queryGroups,
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryGroup> queryGroups,
            bool isNot) :
            this((IEnumerable<QueryField>)null,
                queryGroups,
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryGroup> queryGroups,
            Conjunction conjunction,
            bool isNot) :
            this((IEnumerable<QueryField>)null,
                queryGroups,
                conjunction,
                isNot)
        { }

        /** QueryField / QueryGroup **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(QueryField queryField,
            QueryGroup queryGroup,
            Conjunction conjunction) :
            this(queryField?.AsEnumerable(),
                queryGroup?.AsEnumerable(),
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryField queryField,
            QueryGroup queryGroup,
            bool isNot) :
            this(queryField?.AsEnumerable(),
                queryGroup?.AsEnumerable(),
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryField queryField,
            QueryGroup queryGroup,
            Conjunction conjunction,
            bool isNot) :
            this(queryField?.AsEnumerable(),
                queryGroup?.AsEnumerable(),
                conjunction,
                isNot)
        { }

        /** QueryField / QueryGroups **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        public QueryGroup(QueryField queryField,
            IEnumerable<QueryGroup> queryGroups) :
            this(queryField?.AsEnumerable(),
                queryGroups,
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(QueryField queryField,
            IEnumerable<QueryGroup> queryGroups,
            Conjunction conjunction) :
            this(queryField?.AsEnumerable(),
                queryGroups,
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryField queryField,
            IEnumerable<QueryGroup> queryGroups,
            bool isNot) :
            this(queryField?.AsEnumerable(),
                queryGroups,
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryField">The field to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(QueryField queryField,
            IEnumerable<QueryGroup> queryGroups,
            Conjunction conjunction,
            bool isNot) :
            this(queryField?.AsEnumerable(),
                queryGroups,
                conjunction,
                isNot)
        { }

        ///** QueryFields / QueryGroup **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            QueryGroup queryGroup,
            Conjunction conjunction) :
            this(queryFields,
                queryGroup?.AsEnumerable(),
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            QueryGroup queryGroup,
            bool isNot) :
            this(queryFields,
                queryGroup?.AsEnumerable(),
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroup">The child query group to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            QueryGroup queryGroup,
            Conjunction conjunction,
            bool isNot) :
            this(queryFields,
                queryGroup?.AsEnumerable(),
                conjunction,
                isNot)
        { }

        /** QueryFields / QueryGroups **/

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            IEnumerable<QueryGroup> queryGroups) :
            this(queryFields,
                queryGroups,
                Conjunction.And,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            IEnumerable<QueryGroup> queryGroups,
            Conjunction conjunction) :
            this(queryFields,
                queryGroups,
                conjunction,
                false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            IEnumerable<QueryGroup> queryGroups,
            bool isNot) :
            this(queryFields,
                queryGroups,
                Conjunction.And,
                isNot)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="queryFields">The list of fields to be grouped for the query expression.</param>
        /// <param name="queryGroups">The child query groups to be grouped for the query expression.</param>
        /// <param name="conjunction">The conjunction to be used for every group seperation.</param>
        /// <param name="isNot">The prefix to be added whether the field value is in opposite state.</param>
        public QueryGroup(IEnumerable<QueryField> queryFields,
            IEnumerable<QueryGroup> queryGroups,
            Conjunction conjunction,
            bool isNot)
        {
            Conjunction = conjunction;
            QueryFields = queryFields?.AsList();
            QueryGroups = queryGroups.AsList();
            IsNot = isNot;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the conjunction used by this object.
        /// </summary>
        public Conjunction Conjunction { get; }

        /// <summary>
        /// Gets the list of child <see cref="QueryField"/> objects.
        /// </summary>
        public IEnumerable<QueryField> QueryFields { get; }

        /// <summary>
        /// Gets the list of child <see cref="QueryGroup"/> objects.
        /// </summary>
        public IEnumerable<QueryGroup> QueryGroups { get; }

        /// <summary>
        /// Gets the value whether the grouping is in opposite field-value state.
        /// </summary>
        public bool IsNot { get; private set; }

        #endregion

        #region Methods (Internal)

        /// <summary>
        /// Prepend an underscore on every parameter object.
        /// </summary>
        internal void PrependAnUnderscoreAtTheParameters()
        {
            var queryFields = GetFields(true);
            if (queryFields?.Any() != true)
            {
                return;
            }
            foreach (var queryField in queryFields)
            {
                queryField.PrependAnUnderscoreAtParameter();
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="IsNot"/> property.
        /// </summary>
        /// <param name="value">The <see cref="bool"/> value the defines the <see cref="IsNot"/> property.</param>
        internal void SetIsNot(bool value) =>
            IsNot = value;

        /// <summary>
        /// Fix the names of the parameters in every <see cref="QueryField"/> object of the target list of <see cref="QueryGroup"/>s.
        /// </summary>
        /// <param name="queryGroups">The list of query groups.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object containing all the fields.</returns>
        internal static void FixForQueryMultiple(QueryGroup[] queryGroups)
        {
            for (var i = 0; i < queryGroups.Length; i++)
            {
                foreach (var field in queryGroups[i].GetFields(true))
                {
                    field.Parameter.SetName(string.Format("T{0}_{1}", i, field.Parameter.Name));
                }
            }
        }

        /// <summary>
        /// Converts every <see cref="QueryGroup"/> object of the list of <see cref="QueryGroupTypeMap"/> into an <see cref="object"/> 
        /// with all the child <see cref="QueryField"/>s as the property/value to that object. The value of every property of the created
        /// object will be an instance of the <see cref="CommandParameter"/> with the proper type, name and value.
        /// </summary>
        /// <param name="queryGroupTypeMaps">The list of <see cref="QueryGroupTypeMap"/> objects to be converted.</param>
        /// <param name="fixParameters">A boolean value whether to fix the parameter name before converting.</param>
        /// <returns>An instance of an object that contains all the definition of the converted underlying <see cref="QueryFields"/>s.</returns>
        internal static object AsMappedObject(QueryGroupTypeMap[] queryGroupTypeMaps,
            bool fixParameters = true)
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;

            foreach (var queryGroupTypeMap in queryGroupTypeMaps)
            {
                AsMappedObject(dictionary, queryGroupTypeMap, fixParameters);
            }

            return (ExpandoObject)dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="fixParameters"></param>
        private static void AsMappedObject(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            bool fixParameters = true)
        {
            var queryFields = queryGroupTypeMap
                .QueryGroup
                .GetFields(true);

            // Identify if there are fields to count
            if (queryFields.Any() != true)
            {
                return;
            }

            // Fix the variables for the parameters
            if (fixParameters == true)
            {
                queryGroupTypeMap.QueryGroup.Fix();
            }

            // Iterate all the query fields
            AsMappedObjectForQueryFields(dictionary, queryGroupTypeMap, queryFields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryFields"></param>
        private static void AsMappedObjectForQueryFields(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            IEnumerable<QueryField> queryFields)
        {
            foreach (var queryField in queryFields)
            {
                if (queryField.Operation == Operation.Between ||
                    queryField.Operation == Operation.NotBetween)
                {
                    AsMappedObjectForBetweenQueryField(dictionary, queryGroupTypeMap, queryField);
                }
                else if (queryField.Operation == Operation.In ||
                    queryField.Operation == Operation.NotIn)
                {
                    AsMappedObjectForInQueryField(dictionary, queryGroupTypeMap, queryField);
                }
                else
                {
                    AsMappedObjectForNormalQueryField(dictionary, queryGroupTypeMap, queryField);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryField"></param>
        private static void AsMappedObjectForBetweenQueryField(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            QueryField queryField)
        {
            var values = GetValueList(queryField.Parameter.Value);

            // Left
            var left = string.Concat(queryField.Parameter.Name, "_Left");
            if (!dictionary.ContainsKey(left))
            {
                var leftValue = values.Count > 0 ? values[0] : null;
                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(left,
                        new CommandParameter(left, leftValue, queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(left, leftValue);
                }
            }

            // Right
            var right = string.Concat(queryField.Parameter.Name, "_Right");
            if (!dictionary.ContainsKey(right))
            {
                var rightValue = values.Count > 1 ? values[1] : null;
                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(right,
                        new CommandParameter(right, rightValue, queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(right, rightValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryField"></param>
        private static void AsMappedObjectForInQueryField(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            QueryField queryField)
        {
            var values = GetValueList(queryField.Parameter.Value);

            for (var i = 0; i < values.Count; i++)
            {
                var parameterName = string.Concat(queryField.Parameter.Name, "_In_", i);
                if (dictionary.ContainsKey(parameterName))
                {
                    continue;
                }

                if (queryGroupTypeMap.MappedType != null)
                {
                    dictionary.Add(parameterName,
                        new CommandParameter(parameterName, values[i], queryGroupTypeMap.MappedType));
                }
                else
                {
                    dictionary.Add(parameterName, values[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="queryGroupTypeMap"></param>
        /// <param name="queryField"></param>
        private static void AsMappedObjectForNormalQueryField(IDictionary<string, object> dictionary,
            QueryGroupTypeMap queryGroupTypeMap,
            QueryField queryField)
        {
            if (dictionary.ContainsKey(queryField.Parameter.Name))
            {
                return;
            }

            if (queryGroupTypeMap.MappedType != null)
            {
                dictionary.Add(queryField.Parameter.Name,
                    new CommandParameter(queryField.Parameter.Name, queryField.Parameter.Value, queryGroupTypeMap.MappedType));
            }
            else
            {
                dictionary.Add(queryField.Parameter.Name, queryField.Parameter.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private static IList<T> GetValueList<T>(T value)
        {
            var list = new List<T>();

            if (value is System.Collections.IEnumerable)
            {
                var items = ((System.Collections.IEnumerable)value)
                    .OfType<T>()
                    .AsList();
                list.AddRange(items);
            }
            else
            {
                list.AddIfNotNull(value);
            }

            return list;
        }

        /// <summary>
        /// Forces to set the <see cref="isFixed"/> variable to True.
        /// </summary>
        private void ForceIsFixedVariables()
        {
            if (QueryGroups?.Any() == true)
            {
                foreach (var queryGroup in QueryGroups)
                {
                    queryGroup.ForceIsFixedVariables();
                }
            }
            isFixed = true;
        }

        /// <summary>
        /// Reset all the query fields.
        /// </summary>
        private void ResetQueryFields()
        {
            if (QueryFields?.Any() != true)
            {
                return;
            }
            foreach (var field in QueryFields)
            {
                field.Reset();
            }
        }

        /// <summary>
        /// Reset all the query groups.
        /// </summary>
        private void ResetQueryGroups()
        {
            if (QueryGroups?.Any() != true)
            {
                return;
            }
            foreach (var group in QueryGroups)
            {
                group.Reset();
            }
        }

        /// <summary>
        /// Fix the query fields names.
        /// </summary>
        /// <param name="fields"></param>
        private void FixQueryFields(IEnumerable<QueryField> fields)
        {
            var firstList = fields
                .OrderBy(queryField => queryField.Parameter.Name)
                .AsList();
            var secondList = new List<QueryField>(firstList);

            foreach (var firstQueryField in firstList)
            {
                for (var fieldIndex = 0; fieldIndex < secondList.Count; fieldIndex++)
                {
                    var secondQueryField = secondList[fieldIndex];
                    if (ReferenceEquals(firstQueryField, secondQueryField))
                    {
                        continue;
                    }
                    if (firstQueryField.Field.Equals(secondQueryField.Field))
                    {
                        var fieldValue = secondQueryField.Parameter;
                        fieldValue.SetName(string.Concat(secondQueryField.Parameter.Name, "_", fieldIndex));
                    }
                }
                secondList.RemoveAll(qf => qf.Field.Equals(firstQueryField.Field));
            }
        }

        #endregion

        #region Methods (Public)

        /// <summary>
        /// Reset the <see cref="QueryGroup"/> back to its default state (as is newly instantiated).
        /// </summary>
        public void Reset()
        {
            ResetQueryFields();
            ResetQueryGroups();
            conjuctionTextAttribute = null;
            isFixed = false;
            hashCode = null;
        }

        /// <summary>
        /// Fix the names of the <see cref="Parameter"/> on every <see cref="QueryField"/> (and on every child <see cref="QueryGroup"/>) of the current <see cref="QueryGroup"/>.
        /// </summary>
        /// <returns>The current instance.</returns>
        public QueryGroup Fix()
        {
            if (isFixed)
            {
                return this;
            }

            // Check the presence
            var fields = GetFields(true);

            // Check any item
            if (fields?.Any() != true)
            {
                return this;
            }

            // Fix the fields
            FixQueryFields(fields);

            // Force the variables
            ForceIsFixedVariables();

            // Return the current instance
            return this;
        }

        /// <summary>
        /// Make the current instance of <see cref="QueryGroup"/> object to become an expression for 'Update' operations.
        /// </summary>
        public void IsForUpdate() =>
            PrependAnUnderscoreAtTheParameters();

        /// <summary>
        /// Gets the text value of <see cref="TextAttribute"/> implemented at the <see cref="Conjunction"/> property value of this instance.
        /// </summary>
        /// <returns>A string instance containing the value of the <see cref="TextAttribute"/> text property.</returns>
        public string GetConjunctionText()
        {
            if (conjuctionTextAttribute == null)
            {
                conjuctionTextAttribute = typeof(Conjunction)
                    .GetMembers()
                    .First(member => string.Equals(member.Name, Conjunction.ToString(), StringComparison.OrdinalIgnoreCase))
                    .GetCustomAttribute<TextAttribute>();
            }
            return conjuctionTextAttribute.Text;
        }


        /// <summary>
        /// Gets the stringified query expression format of the current instance. A formatted string for field-operation-parameter will be
        /// conjuncted by the value of the <see cref="Conjunction"/> property.
        /// </summary>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A stringified formatted-text of the current instance.</returns>
        public string GetString(IDbSetting dbSetting) =>
            GetString(0, dbSetting);

        /// <summary>
        /// Gets the stringified query expression format of the current instance. A formatted string for field-operation-parameter will be
        /// conjuncted by the value of the <see cref="RepoDb.Enumerations.Conjunction"/> property.
        /// </summary>
        /// <param name="index">The parameter index for batch operation.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A stringified formatted-text of the current instance.</returns>
        public string GetString(int index,
            IDbSetting dbSetting)
        {
            // Fix first the parameters
            Fix();

            // Variables
            var groupList = new List<string>();
            var conjunction = GetConjunctionText();
            var separator = string.Concat(" ", GetConjunctionText(), " ");

            // Check the instance fields
            var queryFields = QueryFields.AsList();
            if (queryFields?.Count > 0)
            {
                var fields = QueryFields
                    .Select(qf =>
                        qf.AsFieldAndParameter(index, dbSetting)).Join(separator);
                groupList.Add(fields);
            }

            // Check the instance groups
            var queryGroups = QueryGroups.AsList();
            if (queryGroups?.Count > 0)
            {
                var groups = QueryGroups
                    .Select(qg => qg.GetString(index, dbSetting)).Join(separator);
                groupList.Add(groups);
            }

            // Return the value
            return IsNot ? string.Concat("NOT (", groupList.Join(conjunction), ")") : string.Concat("(", groupList.Join(separator), ")");
        }

        /// <summary>
        /// Gets all the child <see cref="QueryField"/> objects associated on the current instance.
        /// </summary>
        /// <param name="traverse">Identify whether to explore all the children of the child <see cref="QueryGroup"/> objects.</param>
        /// <returns>An enumerable list of <see cref="QueryField"/> objects.</returns>
        public IEnumerable<QueryField> GetFields(bool traverse)
        {
            // Variables
            var explore = (Action<QueryGroup>)null;
            var queryFields = new List<QueryField>();

            // Logic for traverse
            explore = new Action<QueryGroup>(queryGroup =>
            {
                // Check child fields
                if (queryGroup.QueryFields?.Any() == true)
                {
                    queryFields.AddRange(queryGroup.QueryFields);
                }

                // Check child groups
                if (traverse == true && queryGroup.QueryGroups?.Any() == true)
                {
                    foreach (var qg in queryGroup.QueryGroups)
                    {
                        explore(qg);
                    }
                }
            });

            // Explore
            explore(this);

            // Return the value
            return queryFields;
        }

        #endregion

        #region Parse (Expression)

        /// <summary>
        /// Parses a customized query expression.
        /// </summary>
        /// <typeparam name="TEntity">The target entity type</typeparam>
        /// <param name="expression">The expression to be converted to a <see cref="QueryGroup"/> object.</param>
        /// <returns>An instance of the <see cref="QueryGroup"/> object that contains the parsed query expression.</returns>
        public static QueryGroup Parse<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            // Guard the presense of the expression
            if (expression == null)
            {
                throw new NullReferenceException("Expression cannot be null.");
            }

            // Parse the expression base on type
            var parsed = Parse<TEntity>(expression.Body);

            // Throw an unsupported exception if not parsed
            if (parsed == null)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Return the parsed values
            return parsed.Fix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(Expression expression)
            where TEntity : class
        {
            if (expression.IsLambda())
            {
                return Parse<TEntity>(expression.ToLambda().Body);
            }
            else if (expression.IsBinary())
            {
                return Parse<TEntity>(expression.ToBinary());
            }
            else if (expression.IsUnary())
            {
                return Parse<TEntity>(expression.ToUnary(), null, expression.NodeType, true);
            }
            else if (expression.IsMethodCall())
            {
                return Parse<TEntity>(expression.ToMethodCall(), false, true);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(BinaryExpression expression)
            where TEntity : class
        {
            var leftQueryGroup = (QueryGroup)null;
            var rightQueryGroup = (QueryGroup)null;
            var rightValue = (object)null;
            var skipRight = false;
            var isEqualsTo = true;

            // TODO: Refactor this

            /*
             * LEFT
             */

            // Get the value in the right
            if (expression.IsExtractable())
            {
                rightValue = expression.Right.GetValue();
                skipRight = true;
                if (rightValue is bool)
                {
                    isEqualsTo = Equals(rightValue, false) == false;
                }
            }

            // Binary
            if (expression.Left.IsBinary() == true)
            {
                leftQueryGroup = Parse<TEntity>(expression.Left.ToBinary());
                leftQueryGroup.SetIsNot(isEqualsTo == false);
            }
            // Unary
            else if (expression.Left.IsUnary() == true)
            {
                leftQueryGroup = Parse<TEntity>(expression.Left.ToUnary(), rightValue, expression.NodeType, isEqualsTo);
            }
            // MethodCall
            else if (expression.Left.IsMethodCall())
            {
                leftQueryGroup = Parse<TEntity>(expression.Left.ToMethodCall(), false, isEqualsTo);
            }
            else
            {
                // Extractable
                if (expression.IsExtractable())
                {
                    var queryField = QueryField.Parse<TEntity>(expression);
                    leftQueryGroup = new QueryGroup(queryField);
                    skipRight = true;
                }
            }

            // Identify the node type
            if (expression.NodeType == ExpressionType.NotEqual)
            {
                leftQueryGroup.SetIsNot(leftQueryGroup.IsNot == isEqualsTo);
            }

            /*
             * RIGHT
             */

            if (skipRight == false)
            {
                // Binary
                if (expression.Right.IsBinary() == true)
                {
                    rightQueryGroup = Parse<TEntity>(expression.Right.ToBinary());
                }
                // Unary
                else if (expression.Right.IsUnary() == true)
                {
                    rightQueryGroup = Parse<TEntity>(expression.Right.ToUnary(), null, expression.NodeType, true);
                }
                // MethodCall
                else if (expression.Right.IsMethodCall())
                {
                    rightQueryGroup = Parse<TEntity>(expression.Right.ToMethodCall(), false, true);
                }

                // Return both of them
                if (leftQueryGroup != null && rightQueryGroup != null)
                {
                    var conjunction = (expression.NodeType == ExpressionType.OrElse) ? Conjunction.Or : Conjunction.And;
                    return new QueryGroup(new[] { leftQueryGroup, rightQueryGroup }, conjunction);
                }
            }

            // Return either one of them
            return leftQueryGroup ?? rightQueryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="rightValue"></param>
        /// <param name="expressionType"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(UnaryExpression expression,
            object rightValue,
            ExpressionType expressionType,
            bool isEqualsTo)
            where TEntity : class
        {
            if (expression.Operand?.IsMember() == true)
            {
                return Parse<TEntity>(expression.Operand.ToMember(), rightValue, expressionType, false, true);
            }
            else if (expression.Operand?.IsMethodCall() == true)
            {
                return Parse<TEntity>(expression.Operand.ToMethodCall(), (expression.NodeType == ExpressionType.Not), isEqualsTo);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="rightValue"></param>
        /// <param name="expressionType"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MemberExpression expression,
            object rightValue,
            ExpressionType expressionType,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            var queryGroup = (QueryGroup)null;
            var value = rightValue;
            var isForBoolean = expression.Type == typeof(bool) &&
                (expressionType == ExpressionType.Not || expressionType == ExpressionType.AndAlso || expressionType == ExpressionType.OrElse);
            var ignoreIsNot = false;

            // Handle for boolean
            if (value == null)
            {
                if (isForBoolean)
                {
                    value = false;
                    ignoreIsNot = true;
                }
                else
                {
                    value = expression.GetValue();
                }
            }

            // Check if there are values
            if (value != null)
            {
                // Specialized for enum
                if (expression.Type.IsEnum)
                {
                    value = Enum.ToObject(expression.Type, value);
                }

                // Create a new field
                var field = (QueryField)null;

                if (isForBoolean)
                {
                    field = new QueryField(expression.Member.GetMappedName(),
                        value);
                    ignoreIsNot = true;
                }
                else
                {
                    field = new QueryField(expression.Member.GetMappedName(),
                        QueryField.GetOperation(expressionType),
                        value);
                }

                // Set the query group
                queryGroup = new QueryGroup(field);

                // Set the query group IsNot property
                if (ignoreIsNot == false)
                {
                    queryGroup.SetIsNot(isEqualsTo == false);
                }
            }

            // Return the result
            return queryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup Parse<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // Check methods for the 'Like', both 'Array.<All|Any>()'
            if (expression.Method.Name == "All" || expression.Method.Name == "Any")
            {
                return ParseAllOrAnyForArrayOrAnyForList<TEntity>(expression, isNot, isEqualsTo);
            }

            // Check methods for the 'Like', both 'Array.Contains()' and 'StringProperty.Contains()'
            else if (expression.Method.Name == "Contains")
            {
                if (expression.Object?.IsMember() == true)
                {
                    // Cast to proper object
                    var member = expression.Object.ToMember();
                    if (member.Type == typeof(string))
                    {
                        // Check for the (p => p.Property.Contains("A")) for LIKE
                        return ParseContainsOrStartsWithOrEndsWithForStringProperty<TEntity>(expression, isNot, isEqualsTo);
                    }
                    else if (member.Type.IsConstructedGenericType == true)
                    {
                        // Check for the (p => list.Contains(p.Property)) or (p => (new List<int> { 1, 2 }).Contains(p.Property))
                        return ParseContainsForArrayOrList<TEntity>(expression, isNot, isEqualsTo);
                    }
                }
                else
                {
                    // Check for the (array.Contains(p.Property)) or (new [] { value1, value2 }).Contains(p.Property))
                    return ParseContainsForArrayOrList<TEntity>(expression, isNot, isEqualsTo);
                }
            }

            // Check methods for the 'Like', both 'StringProperty.StartsWith()' and 'StringProperty.EndsWith()'
            else if (expression.Method.Name == "StartsWith" || expression.Method.Name == "EndsWith")
            {
                if (expression.Object?.IsMember() == true)
                {
                    if (expression.Object.ToMember().Type == typeof(string))
                    {
                        return ParseContainsOrStartsWithOrEndsWithForStringProperty<TEntity>(expression, isNot, isEqualsTo);
                    }
                }
            }

            // Return null if not supported
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup ParseAllOrAnyForArrayOrAnyForList<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // TODO: Refactor this

            // Return null if there is no any arguments
            if (expression.Arguments?.Any() != true)
            {
                return null;
            }

            // Get the last property
            var last = expression
                .Arguments
                .LastOrDefault();

            // Make sure the last is a member
            if (last == null || last?.IsLambda() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure the last is a binary
            var lambda = last.ToLambda();
            if (lambda.Body.IsBinary() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure it is a member
            var binary = lambda.Body.ToBinary();
            if (binary.Left.IsMember() == false && binary.Right.IsMember() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported. Expression must contain a single condition to any property of type '{typeof(TEntity).FullName}'.");
            }

            // Make sure it is a property
            var member = binary.Left.IsMember() ? binary.Left.ToMember().Member : binary.Right.ToMember().Member;
            if (member.IsPropertyInfo() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure the property is in the entity
            var property = member.ToPropertyInfo();
            if (PropertyCache.Get<TEntity>().FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression.ToString()}'. The property {property.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Variables needed for fields
            var queryFields = new List<QueryField>();
            var conjunction = Conjunction.And;

            // Support only various methods
            if (expression.Method.Name == "Any")
            {
                conjunction = Conjunction.Or;
            }
            else if (expression.Method.Name == "All")
            {
                conjunction = Conjunction.And;
            }

            // Call the method
            var first = expression.Arguments.First();
            var values = (object)null;

            // Identify the type of the argument
            if (first.IsNewArray())
            {
                values = first.ToNewArray().GetValue();
            }
            else if (first.IsMember())
            {
                values = first.ToMember().GetValue();
            }

            // Values must be an array
            if (values is System.Collections.IEnumerable)
            {
                var operation = QueryField.GetOperation(binary.NodeType);
                foreach (var value in (System.Collections.IEnumerable)values)
                {
                    var queryField = new QueryField(PropertyMappedNameCache.Get(property), operation, value);
                    queryFields.Add(queryField);
                }
            }

            // Return the result
            return new QueryGroup(queryFields, conjunction, (isNot == isEqualsTo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup ParseContainsForArrayOrList<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // TODO: Refactor this

            // Return null if there is no any arguments
            if (expression.Arguments?.Any() != true)
            {
                return null;
            }

            // Get the last arg
            var last = expression
                .Arguments
                .LastOrDefault();

            // Make sure the last arg is a member
            if (last == null || last?.IsMember() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure it is a property info
            var member = last.ToMember().Member;
            if (member.IsPropertyInfo() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Get the property
            var property = member.ToPropertyInfo();

            // Make sure the property is in the entity
            if (PropertyCache.Get<TEntity>().FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression.ToString()}'. The property {property.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Get the values
            var values = (object)null;

            // Array/List Separation
            if (expression.Object == null)
            {
                // Expecting an array
                values = expression.Arguments.First().GetValue();
            }
            else
            {
                // Expecting a list here
                values = expression.Object.GetValue();
            }

            // Add to query fields
            var operation = (isNot == false && isEqualsTo == true) ? Operation.In : Operation.NotIn;
            var queryField = new QueryField(PropertyMappedNameCache.Get(property), operation, values);

            // Return the result
            var queryGroup = new QueryGroup(queryField);

            // Set the IsNot value
            queryGroup.SetIsNot(isNot == true && isEqualsTo == false);

            // Return the instance
            return queryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="isNot"></param>
        /// <param name="isEqualsTo"></param>
        /// <returns></returns>
        private static QueryGroup ParseContainsOrStartsWithOrEndsWithForStringProperty<TEntity>(MethodCallExpression expression,
            bool isNot,
            bool isEqualsTo)
            where TEntity : class
        {
            // TODO: Refactor this

            // Return null if there is no any arguments
            if (expression.Arguments?.Any() != true)
            {
                return null;
            }

            // Get the value arg
            var value = Convert.ToString(expression.Arguments.FirstOrDefault()?.GetValue());

            // Make sure it has a value
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Make sure it is a property info
            var member = expression.Object.ToMember().Member;
            if (member.IsPropertyInfo() == false)
            {
                throw new NotSupportedException($"Expression '{expression.ToString()}' is currently not supported.");
            }

            // Get the property
            var property = member.ToPropertyInfo();

            // Make sure the property is in the entity
            if (PropertyCache.Get<TEntity>().FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new InvalidExpressionException($"Invalid expression '{expression.ToString()}'. The property {property.Name} is not defined on a target type '{typeof(TEntity).FullName}'.");
            }

            // Add to query fields
            var operation = (isNot == isEqualsTo) ? Operation.NotLike : Operation.Like;
            var queryField = new QueryField(PropertyMappedNameCache.Get(property),
                operation,
                ConvertToLikeableValue(expression.Method.Name, value));

            // Return the result
            return new QueryGroup(queryField.AsEnumerable());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertToLikeableValue(string methodName,
            string value)
        {
            if (methodName == "Contains")
            {
                value = value.StartsWith("%") ? value : string.Concat("%", value);
                value = value.EndsWith("%") ? value : string.Concat(value, "%");
            }
            else if (methodName == "StartsWith")
            {
                value = value.EndsWith("%") ? value : string.Concat(value, "%");
            }
            else if (methodName == "EndsWith")
            {
                value = value.StartsWith("%") ? value : string.Concat("%", value);
            }
            return value;
        }

        #endregion

        #region Parse (Dynamics)

        /// <summary>
        /// Parses an object and convert back the result to an instance of <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="obj">The instance of the object to be parsed.</param>
        /// <returns>An instance of the <see cref="QueryGroup"/> with parsed properties and values.</returns>
        public static QueryGroup Parse<T>(T obj)
        {
            // Check for value
            if (obj == null)
            {
                throw new ArgumentNullException("Parameter 'obj' cannot be null.");
            }

            // Type of the object
            var type = obj.GetType();

            // Filter the type
            if (type.IsGenericType == false && type != StaticType.Object && type.IsClassType() == false)
            {
                return null;
            }

            // Declare variables
            var queryFields = new List<QueryField>();

            // Iterate every property
            foreach (var property in type.GetProperties())
            {
                queryFields.Add(new QueryField(property.AsField(), property.GetValue(obj)));
            }

            // Return
            return queryFields?.Any() == true ? new QueryGroup(queryFields).Fix() : null;
        }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="QueryGroup"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to check if this is already taken
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            var hashCode = 0;

            // Iterates the child query field
            if (QueryFields != null)
            {
                foreach (var queryField in QueryFields)
                {
                    hashCode += queryField.GetHashCode();
                }
            }

            // Iterates the child query groups
            if (QueryGroups?.Any() == true)
            {
                foreach (var queryGroup in QueryGroups)
                {
                    hashCode += queryGroup.GetHashCode();
                }
            }

            // Set with conjunction
            hashCode += (int)Conjunction;

            // Set the IsNot
            hashCode += IsNot.GetHashCode();

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="QueryGroup"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="QueryGroup"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryGroup other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="QueryGroup"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryGroup"/> object.</param>
        /// <param name="objB">The second <see cref="QueryGroup"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(QueryGroup objA,
            QueryGroup objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryGroup"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryGroup"/> object.</param>
        /// <param name="objB">The second <see cref="QueryGroup"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryGroup objA,
            QueryGroup objB) =>
            (objA == objB) == false;

        #endregion
    }
}
