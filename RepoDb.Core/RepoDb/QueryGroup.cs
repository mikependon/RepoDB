using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Attributes;
using System.Reflection;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A widely-used class for defining the groupings when composing the query expression. This object is used by most operations
    /// to define the filters and expressions on the actual execution.
    /// </summary>
    public partial class QueryGroup : IEquatable<QueryGroup>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
        /// <param name="conjunction">The conjunction to be used for every group separation.</param>
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
            conjuctionTextAttribute ??= typeof(Conjunction)
                .GetMembers()
                .First(member => string.Equals(member.Name, Conjunction.ToString(), StringComparison.OrdinalIgnoreCase))
                .GetCustomAttribute<TextAttribute>();
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
            explore = queryGroup =>
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
            };

            // Explore
            explore(this);

            // Return the value
            return queryFields;
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
