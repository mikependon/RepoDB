#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.IO;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Validates a definition that is already in memory. The structural checks run first, and the strict checks only run on a definition with a sound structure.
    /// It does not read, trace or throw: the reading, the tracing and the throwing belong to the callers.
    /// </summary>
    internal static class SchemaDefinitionValidator
    {
        #region Internal Methods

        /// <summary>
        /// Gets the description of the definition that a trace receives as the statement of the validation.
        /// </summary>
        /// <param name="definition">The definition to be validated.</param>
        /// <returns>The description of the definition.</returns>
        internal static string Describe(SchemaDefinitionInfo definition) =>
            definition.RootTable == null ? "Validate definition" : $"Validate definition of {DefinitionFindings.Describe(definition.RootTable)}";

        /// <summary>
        /// Validates the definition: the structure first, and then, if the structure is sound, the strict checks.
        /// </summary>
        /// <param name="definition">The definition to be validated.</param>
        /// <returns>The outcome of the validation. The definition is only part of it if there are no errors.</returns>
        internal static ValidateDefinitionResult Validate(SchemaDefinitionInfo definition)
        {
            if (definition.Tables == null)
            {
                return new ValidateDefinitionResult { Errors = { "The definition has no tables. Add table definitions to the Tables collection. Location: Tables." } };
            }
            var findings = SchemaDefinitionStructureValidator.Validate(definition);
            if (findings.Errors.Count == 0)
            {
                findings = SchemaDefinitionStrictValidator.Validate(definition);
            }
            return CreateResult(definition,
                findings);
        }

        /// <summary>
        /// Validates the definition, and throws the first error if it is asked to.
        /// </summary>
        /// <param name="definition">The definition to be validated.</param>
        /// <param name="throwOnError">Whether the first error is thrown instead of being reported in the outcome.</param>
        /// <returns>The outcome of the validation. The definition is only part of it if there are no errors.</returns>
        /// <exception cref="InvalidDataException"><paramref name="throwOnError"/> is <c>true</c> and the definition has errors.</exception>
        internal static ValidateDefinitionResult Validate(SchemaDefinitionInfo definition,
            bool throwOnError)
        {
            var result = Validate(definition);
            if (throwOnError && !result.IsValid)
            {
                throw new InvalidDataException(result.Errors[0]);
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the outcome of the validation of a definition.
        /// </summary>
        /// <param name="definition">The definition that was validated.</param>
        /// <param name="findings">The errors and the warnings that were found.</param>
        /// <returns>The outcome. The definition is only part of it if there are no errors.</returns>
        private static ValidateDefinitionResult CreateResult(SchemaDefinitionInfo definition,
            DefinitionFindings findings) =>
            new ValidateDefinitionResult
            {
                Definition = findings.Errors.Count == 0 ? definition : null,
                Errors = findings.Errors,
                Warnings = findings.Warnings
            };

        #endregion
    }
}
