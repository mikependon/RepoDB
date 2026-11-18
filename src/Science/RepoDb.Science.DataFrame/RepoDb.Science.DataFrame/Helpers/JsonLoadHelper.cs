using System.Text.Json;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Selects and projects records from UTF-8 JSON.
/// </summary>
internal static class JsonLoadHelper
{
    #region Public Methods

    /// <summary>
    /// Reports selected JSON records.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="collectionPath">The dot-separated collection path.</param>
    /// <param name="columns">The output names and scalar paths.</param>
    /// <param name="column">The column callback.</param>
    /// <param name="row">The record callback.</param>
    public static void Read(Stream stream,
        string? collectionPath,
        IReadOnlyDictionary<string, string>? columns,
        Action<string> column,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        var path = collectionPath is null ? [] : SplitPath(collectionPath);
        var mappings = PrepareMappings(columns,
            column);
        var reader = new JsonTokenReader(stream);
        RequireNext(reader);
        if (!SelectArrays(reader,
            path,
            0,
            mappings,
            row))
        {
            throw new JsonException($"JSON collection '{collectionPath}' was not found.");
        }
        if (reader.MoveNext())
        {
            throw new JsonException("Unexpected data after the JSON document.");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Registers and validates the output mappings.
    /// </summary>
    /// <param name="columns">The output names and scalar paths.</param>
    /// <param name="column">The column callback.</param>
    /// <returns>The prepared mappings.</returns>
    private static Dictionary<string, string[]>? PrepareMappings(IReadOnlyDictionary<string, string>? columns,
        Action<string> column)
    {
        if (columns is null)
        {
            return null;
        }
        var mappings = new Dictionary<string, string[]>(StringComparer.Ordinal);
        foreach (var (name, path) in columns)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Column names must be nonempty.",
                    nameof(columns));
            }
            mappings.Add(name,
                SplitPath(path));
            column(name);
        }
        return mappings;
    }

    /// <summary>
    /// Splits a path into nonempty property names.
    /// </summary>
    /// <param name="path">The property path.</param>
    /// <returns>The path segments.</returns>
    private static string[] SplitPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var parts = path.Split('.');
        if (Array.Exists(parts,
            string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("JSON paths require nonempty segments.",
                nameof(path));
        }
        return parts;
    }

    /// <summary>
    /// Traverses wrappers and intermediate arrays to find record arrays.
    /// </summary>
    /// <param name="reader">The current token reader.</param>
    /// <param name="path">The collection path.</param>
    /// <param name="index">The next path segment.</param>
    /// <param name="mappings">The output mappings.</param>
    /// <param name="row">The record callback.</param>
    /// <returns>Whether a record array was found.</returns>
    private static bool SelectArrays(JsonTokenReader reader,
        string[] path,
        int index,
        Dictionary<string, string[]>? mappings,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        if (index == path.Length)
        {
            ReadArray(reader,
                mappings,
                row);
            return true;
        }
        if (reader.Kind == JsonTokenType.StartObject)
        {
            return SelectProperty(reader,
                path,
                index,
                mappings,
                row);
        }
        if (reader.Kind != JsonTokenType.StartArray)
        {
            return false;
        }
        if (ReadNext(reader) == JsonTokenType.EndArray)
        {
            return true;
        }
        var found = false;
        do
        {
            found |= SelectArrays(reader,
                path,
                index,
                mappings,
                row);
        }
        while (ReadNext(reader) != JsonTokenType.EndArray);
        return found;
    }

    /// <summary>
    /// Selects the next property on a collection path.
    /// </summary>
    /// <param name="reader">The current object reader.</param>
    /// <param name="path">The collection path.</param>
    /// <param name="index">The next path segment.</param>
    /// <param name="mappings">The output mappings.</param>
    /// <param name="row">The record callback.</param>
    /// <returns>Whether a record array was found.</returns>
    private static bool SelectProperty(JsonTokenReader reader,
        string[] path,
        int index,
        Dictionary<string, string[]>? mappings,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        var found = false;
        var selected = false;
        while (ReadNext(reader) != JsonTokenType.EndObject)
        {
            var name = (string)reader.Value!;
            RequireNext(reader);
            if (!string.Equals(name,
                path[index],
                StringComparison.Ordinal))
            {
                reader.SkipChildren();
                continue;
            }
            if (selected)
            {
                throw new JsonException($"Duplicate JSON collection property '{name}'.");
            }
            selected = true;
            found = SelectArrays(reader,
                path,
                index + 1,
                mappings,
                row);
        }
        return found;
    }

    /// <summary>
    /// Reads and projects each record from a selected array.
    /// </summary>
    /// <param name="reader">The array reader.</param>
    /// <param name="mappings">The output mappings.</param>
    /// <param name="row">The record callback.</param>
    private static void ReadArray(JsonTokenReader reader,
        Dictionary<string, string[]>? mappings,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        if (reader.Kind != JsonTokenType.StartArray)
        {
            throw new JsonException("The selected JSON collection must be an array.");
        }
        while (ReadNext(reader) != JsonTokenType.EndArray)
        {
            if (reader.Kind != JsonTokenType.StartObject)
            {
                throw new JsonException("Each JSON record must be an object.");
            }
            var values = ReadObject(reader,
                mappings is not null);
            row(mappings is null ? values : Project(values,
                mappings));
        }
    }

    /// <summary>
    /// Reads scalar properties and optionally nested objects within one record.
    /// </summary>
    /// <param name="reader">The current object reader.</param>
    /// <param name="nested">Whether nested objects are needed.</param>
    /// <returns>The object properties.</returns>
    private static Dictionary<string, object?> ReadObject(JsonTokenReader reader,
        bool nested)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        var names = new HashSet<string>(StringComparer.Ordinal);
        while (ReadNext(reader) != JsonTokenType.EndObject)
        {
            var name = (string)reader.Value!;
            if (!names.Add(name))
            {
                throw new JsonException($"Duplicate JSON record property '{name}'.");
            }
            RequireNext(reader);
            ReadProperty(reader,
                values,
                name,
                nested);
        }
        return values;
    }

    /// <summary>
    /// Reads a property that can be projected.
    /// </summary>
    /// <param name="reader">The current value reader.</param>
    /// <param name="values">The record properties.</param>
    /// <param name="name">The property name.</param>
    /// <param name="nested">Whether to retain nested objects.</param>
    private static void ReadProperty(JsonTokenReader reader,
        Dictionary<string, object?> values,
        string name,
        bool nested)
    {
        if (nested && reader.Kind == JsonTokenType.StartObject)
        {
            values.Add(name,
                ReadObject(reader,
                    nested));
        }
        else if (reader.Kind is JsonTokenType.StartArray or JsonTokenType.StartObject)
        {
            if (nested)
            {
                values.Add(name,
                    reader.Kind);
            }
            reader.SkipChildren();
        }
        else
        {
            values.Add(name,
                reader.Value);
        }
    }

    /// <summary>
    /// Resolves the requested scalar columns.
    /// </summary>
    /// <param name="values">The record properties.</param>
    /// <param name="mappings">The output mappings.</param>
    /// <returns>The projected values.</returns>
    private static Dictionary<string, object?> Project(Dictionary<string, object?> values,
        Dictionary<string, string[]> mappings)
    {
        var result = new Dictionary<string, object?>(mappings.Count,
            StringComparer.Ordinal);
        foreach (var (name, path) in mappings)
        {
            result.Add(name,
                Resolve(values,
                    path));
        }
        return result;
    }

    /// <summary>
    /// Resolves a scalar path relative to a record.
    /// </summary>
    /// <param name="values">The record properties.</param>
    /// <param name="path">The scalar path.</param>
    /// <returns>The selected scalar or null.</returns>
    private static object? Resolve(Dictionary<string, object?> values,
        string[] path)
    {
        object? current = values;
        foreach (var segment in path)
        {
            if (current is not Dictionary<string, object?> properties ||
                !properties.TryGetValue(segment,
                    out current))
            {
                return null;
            }
            if (current is JsonTokenType.StartArray)
            {
                throw new JsonException($"JSON column path contains an array '{segment}'. Select the array with collectionPath.");
            }
        }
        return current is Dictionary<string, object?> ?
            throw new JsonException("A mapped JSON column must select a scalar.") : current;
    }

    /// <summary>
    /// Reads a required token.
    /// </summary>
    /// <param name="reader">The token reader.</param>
    /// <returns>The token kind.</returns>
    private static JsonTokenType ReadNext(JsonTokenReader reader)
    {
        RequireNext(reader);
        return reader.Kind;
    }

    /// <summary>
    /// Advances or reports incomplete JSON.
    /// </summary>
    /// <param name="reader">The token reader.</param>
    private static void RequireNext(JsonTokenReader reader)
    {
        if (!reader.MoveNext())
        {
            throw new JsonException("Incomplete JSON document.");
        }
    }

    #endregion
}
