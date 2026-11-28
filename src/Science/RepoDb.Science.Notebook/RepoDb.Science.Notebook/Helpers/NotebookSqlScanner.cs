namespace RepoDb.Science.Helpers;

/// <summary>
/// Splits a SQL script into statements of tokens, rejecting text that databases may read differently.
/// </summary>
internal sealed class NotebookSqlScanner
{
    #region Private Variables

    private readonly string _sql;
    private readonly List<List<string>> _statements = [[]];
    private int _index;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a scanner.
    /// </summary>
    /// <param name="sql">The script.</param>
    private NotebookSqlScanner(string sql) => _sql = sql;

    #endregion

    #region Public Methods

    /// <summary>
    /// Scans a script.
    /// </summary>
    /// <param name="sql">The script.</param>
    /// <returns>The statements, or null when the script is ambiguous.</returns>
    public static IReadOnlyList<IReadOnlyList<string>>? Scan(string sql)
    {
        var scanner = new NotebookSqlScanner(sql);
        while (scanner._index < sql.Length)
        {
            if (!scanner.TryRead())
            {
                return null;
            }
        }
        return scanner._statements
            .Where(statement => statement.Count > 0)
            .ToList();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a token to the current statement.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>Always true.</returns>
    private bool Add(string token)
    {
        _statements[^1].Add(token);
        return true;
    }

    /// <summary>
    /// Checks whether a text is at the current position.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>Whether it is there.</returns>
    private bool At(string text) => _sql
        .AsSpan(_index)
        .StartsWith(text,
            StringComparison.Ordinal);

    /// <summary>
    /// Checks whether a character is part of a word.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsWordCharacter(char character) => char.IsLetterOrDigit(character) || character is '_' or '@';

    /// <summary>
    /// Skips a block comment that every database reads the same way.
    /// </summary>
    /// <returns>Whether the comment is safe.</returns>
    private bool TryBlockComment()
    {
        var start = _index + 2;
        var end = _sql.IndexOf("*/",
            start,
            StringComparison.Ordinal);
        if (end < 0)
        {
            return false;
        }
        var body = _sql[start..end];
        _index = end + 2;
        return !body.StartsWith('!')
            && !body.StartsWith("M!",
                StringComparison.Ordinal)
            && !body.Contains("/*",
                StringComparison.Ordinal);
    }

    /// <summary>
    /// Skips a bracketed identifier that cannot hide other code.
    /// </summary>
    /// <returns>Whether the identifier is safe.</returns>
    private bool TryBracket()
    {
        var end = _sql.IndexOf(']',
            _index + 1);
        if (end < 0)
        {
            return false;
        }
        var body = _sql[(_index + 1)..end];
        _index = end + 1;
        return body.IndexOfAny(['\'', '"', '`', ';', '\\']) < 0
            && !body.Contains("--",
                StringComparison.Ordinal)
            && !body.Contains("/*",
                StringComparison.Ordinal)
            && !At("]")
            && Add(string.Empty);
    }

    /// <summary>
    /// Skips a line comment that starts with two dashes and a space.
    /// </summary>
    /// <returns>Whether the comment is safe.</returns>
    private bool TryLineComment()
    {
        _index += 2;
        if (_index < _sql.Length && !char.IsWhiteSpace(_sql[_index]))
        {
            return false;
        }
        while (_index < _sql.Length && _sql[_index] is not ('\r' or '\n'))
        {
            _index++;
        }
        return true;
    }

    /// <summary>
    /// Skips a quoted text without backslashes.
    /// </summary>
    /// <returns>Whether the text is safe.</returns>
    private bool TryQuoted()
    {
        var quote = _sql[_index++];
        while (_index < _sql.Length)
        {
            var character = _sql[_index++];
            if (character == '\\')
            {
                return false;
            }
            if (character != quote)
            {
                continue;
            }
            if (_index >= _sql.Length || _sql[_index] != quote)
            {
                return Add(string.Empty);
            }
            _index++;
        }
        return false;
    }

    /// <summary>
    /// Reads the next part of the script.
    /// </summary>
    /// <returns>Whether the part is safe.</returns>
    private bool TryRead()
    {
        if (At("--"))
        {
            return TryLineComment();
        }
        if (At("/*"))
        {
            return TryBlockComment();
        }
        return _sql[_index] switch
        {
            '\'' or '"' or '`' => TryQuoted(),
            '[' => TryBracket(),
            var character when IsWordCharacter(character) => TryWord(),
            _ => TrySymbol()
        };
    }

    /// <summary>
    /// Reads a separator or a symbol.
    /// </summary>
    /// <returns>Whether it is safe.</returns>
    private bool TrySymbol()
    {
        var character = _sql[_index++];
        if (character is '#' or '$' or '\\' || (character == '\r' && !At("\n")))
        {
            return false;
        }
        if (character == ';')
        {
            _statements.Add([]);
            return true;
        }
        return char.IsWhiteSpace(character) || Add(character.ToString());
    }

    /// <summary>
    /// Reads a word that does not open an alternative quoting.
    /// </summary>
    /// <returns>Whether the word is safe.</returns>
    private bool TryWord()
    {
        var start = _index;
        while (_index < _sql.Length && IsWordCharacter(_sql[_index]))
        {
            _index++;
        }
        var word = _sql[start.._index].ToUpperInvariant();
        return !(word is "Q" or "NQ" && At("'")) && Add(word);
    }

    #endregion
}
