using System.Globalization;
using System.Text;
using System.Text.Json;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads JSON tokens from a refillable UTF-8 buffer.
/// </summary>
internal sealed class JsonTokenReader
{
    #region Private Variables

    private readonly Stream _stream;
    private byte[] _buffer = new byte[4096];
    private int _end;
    private bool _finalBlock;
    private int _start;
    private JsonReaderState _state;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a reader at the stream's current position.
    /// </summary>
    /// <param name="stream">The UTF-8 input stream.</param>
    public JsonTokenReader(Stream stream)
    {
        _stream = stream;
        _end = stream.ReadAtLeast(_buffer,
            3,
            false);
        if (_end >= 3 && _buffer[0] == 0xEF && _buffer[1] == 0xBB && _buffer[2] == 0xBF)
        {
            _start = 3;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current token depth.
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    /// Gets the current token kind.
    /// </summary>
    public JsonTokenType Kind { get; private set; }

    /// <summary>
    /// Gets the current scalar or property name.
    /// </summary>
    public object? Value { get; private set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Moves to the next token.
    /// </summary>
    /// <returns>Whether another token exists.</returns>
    public bool MoveNext() => MoveNext(decodeValue: true);

    /// <summary>
    /// Moves past the children of the current value.
    /// </summary>
    public void SkipChildren()
    {
        if (Kind is not (JsonTokenType.StartArray or JsonTokenType.StartObject))
        {
            return;
        }
        var depth = Depth;
        do
        {
            if (!MoveNext(decodeValue: false))
            {
                throw new JsonException("Incomplete JSON value.");
            }
        }
        while (Depth != depth);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Advances through buffered tokens and refills as needed.
    /// </summary>
    /// <param name="decodeValue">Whether to decode a scalar token.</param>
    /// <returns>Whether another token exists.</returns>
    private bool MoveNext(bool decodeValue)
    {
        while (true)
        {
            var reader = new Utf8JsonReader(_buffer.AsSpan(_start,
                    _end - _start),
                _finalBlock,
                _state);
            var found = reader.Read();
            _start += (int)reader.BytesConsumed;
            _state = reader.CurrentState;
            if (found)
            {
                Depth = reader.CurrentDepth;
                Kind = reader.TokenType;
                Value = decodeValue ? ReadValue(ref reader) : null;
                return true;
            }
            if (_finalBlock)
            {
                return false;
            }
            Refill();
        }
    }

    /// <summary>
    /// Refills the buffer, preserving any incomplete token.
    /// </summary>
    private void Refill()
    {
        _end -= _start;
        _buffer
            .AsSpan(_start,
                _end)
            .CopyTo(_buffer);
        _start = 0;
        if (_end == _buffer.Length)
        {
            Array.Resize(ref _buffer,
                checked(_buffer.Length * 2));
        }
        var read = _stream.Read(_buffer,
            _end,
            _buffer.Length - _end);
        _end += read;
        _finalBlock = read == 0;
    }

    /// <summary>
    /// Decodes a scalar token.
    /// </summary>
    /// <param name="reader">The current token reader.</param>
    /// <returns>The scalar value.</returns>
    private static object? ReadValue(ref Utf8JsonReader reader) =>
        reader.TokenType switch
        {
            JsonTokenType.PropertyName or JsonTokenType.String => reader.GetString(),
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => ReadNumber(ref reader),
            _ => null
        };

    /// <summary>
    /// Decodes a number or preserves its text when out of range.
    /// </summary>
    /// <param name="reader">The number token reader.</param>
    /// <returns>The number or its text.</returns>
    private static object ReadNumber(ref Utf8JsonReader reader)
    {
        if (reader.TryGetInt64(out var integer))
        {
            return integer;
        }
        if (reader.TryGetDecimal(out var number) && IsExactDecimal(reader.ValueSpan,
            number))
        {
            return number;
        }
        return Encoding.UTF8.GetString(reader.ValueSpan);
    }

    /// <summary>
    /// Checks whether decimal parsing retained every significant digit.
    /// </summary>
    /// <param name="token">The numeric token.</param>
    /// <param name="number">The parsed decimal.</param>
    /// <returns>Whether the parsed number is exact.</returns>
    private static bool IsExactDecimal(ReadOnlySpan<byte> token,
        decimal number)
    {
        var exponentIndex = token.IndexOfAny((byte)'e',
            (byte)'E');
        var mantissa = exponentIndex < 0 ? token : token[..exponentIndex];
        var scale = SignificantScale(mantissa);
        if (scale is null)
        {
            return true;
        }
        var exponent = 0;
        return (exponentIndex < 0 || int.TryParse(token[(exponentIndex + 1)..],
                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out exponent)) &&
            (long)scale.Value - exponent <= number.Scale;
    }

    /// <summary>
    /// Finds the scale of a mantissa after removing trailing zeroes.
    /// </summary>
    /// <param name="mantissa">The signed mantissa.</param>
    /// <returns>The scale or null for zero.</returns>
    private static int? SignificantScale(ReadOnlySpan<byte> mantissa)
    {
        var point = mantissa.IndexOf((byte)'.');
        var fraction = point < 0 ? 0 : mantissa.Length - point - 1;
        var trailingZeroes = 0;
        for (var index = mantissa.Length - 1; index >= 0; index--)
        {
            switch (mantissa[index])
            {
                case (byte)'0':
                    trailingZeroes++;
                    break;
                case (byte)'.':
                case (byte)'-':
                    break;
                default:
                    return fraction - trailingZeroes;
            }
        }
        return null;
    }

    #endregion
}
