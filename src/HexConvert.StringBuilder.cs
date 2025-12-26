using System.Text;

namespace System;

/// <summary>
/// Provides extension methods for appending hexadecimal representations to a <see cref="StringBuilder"/>.
/// </summary>
public static partial class HexConvert
{
    /// <summary>
    /// Lower-case hex digit lookup table ('0'..'f').
    /// </summary>
    private static readonly char[] s_hexLower = "0123456789abcdef".ToCharArray();

    /// <summary>
    /// Upper-case hex digit lookup table ('0'..'F').
    /// </summary>
    private static readonly char[] s_hexUpper = "0123456789ABCDEF".ToCharArray();

    /// <summary>
    /// Appends the hexadecimal representation of a single byte to the provided <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append to.</param>
    /// <param name="b">The byte to format as hex.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits; otherwise use lower-case.</param>
    /// <param name="prefix">Optional string to prepend before the two hex characters for this byte (e.g. "0x").</param>
    /// <returns>The original <see cref="StringBuilder"/> instance (<paramref name="sb"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sb"/> is <see langword="null"/>.</exception>
    public static StringBuilder AppendHex(this StringBuilder sb, byte b, bool uppercase = false, string? prefix = null)
    {
        if (sb is null) throw new ArgumentNullException(nameof(sb));

        // Select the appropriate lookup table for hex digits.
        char[] table = uppercase ? s_hexUpper : s_hexLower;

        // Optional prefix (commonly "0x").
        if (!string.IsNullOrEmpty(prefix))
            sb.Append(prefix);

        // Append the high and low nibble characters.
        sb.Append(table[b >> 4]);
        sb.Append(table[b & 0xF]);

        return sb;
    }

    /// <summary>
    /// Appends the hexadecimal representation of a byte array to the provided <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append to.</param>
    /// <param name="bytes">Array of bytes to format as hex.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits; otherwise use lower-case.</param>
    /// <param name="separator">Optional separator string inserted between bytes (e.g. " ").</param>
    /// <param name="prefixPerByte">Optional string to prepend before each byte's hex representation (e.g. "0x").</param>
    /// <returns>The original <see cref="StringBuilder"/> instance (<paramref name="sb"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sb"/> or <paramref name="bytes"/> is <see langword="null"/>.</exception>
    public static StringBuilder AppendHex(this StringBuilder sb, byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (sb is null) throw new ArgumentNullException(nameof(sb));
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));

#if NET35 || NET40 || NETSTANDARD1_0
        // Platforms without Span<T> support: operate directly on the byte array.
        char[] table = uppercase ? s_hexUpper : s_hexLower;
        for (int i = 0; i < bytes.Length; i++)
        {
            // Insert separator between bytes when provided.
            if (i > 0 && !string.IsNullOrEmpty(separator))
                sb.Append(separator);

            // Optional per-byte prefix.
            if (!string.IsNullOrEmpty(prefixPerByte))
                sb.Append(prefixPerByte);

            byte b = bytes[i];
            sb.Append(table[b >> 4]);
            sb.Append(table[b & 0xF]);
        }

        return sb;
#else
        // Forward to the Span-based implementation on platforms that support it.
        return sb.AppendHex(bytes.AsSpan(), uppercase, separator, prefixPerByte);
#endif
    }

#if !NET35 && !NET40 && !NETSTANDARD1_0
    /// <summary>
    /// Appends the hexadecimal representation of a read-only span of bytes to the provided <see cref="StringBuilder"/>.
    /// This overload avoids allocating an intermediate array on platforms that support <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append to.</param>
    /// <param name="bytes">Span of bytes to format as hex.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits; otherwise use lower-case.</param>
    /// <param name="separator">Optional separator string inserted between bytes (e.g. " ").</param>
    /// <param name="prefixPerByte">Optional string to prepend before each byte's hex representation (e.g. "0x").</param>
    /// <returns>The original <see cref="StringBuilder"/> instance (<paramref name="sb"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sb"/> is <see langword="null"/>.</exception>
    public static StringBuilder AppendHex(this StringBuilder sb, ReadOnlySpan<byte> bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (sb is null) throw new ArgumentNullException(nameof(sb));

        // Select the appropriate lookup table for hex digits.
        char[] table = uppercase ? s_hexUpper : s_hexLower;

        for (int i = 0; i < bytes.Length; i++)
        {
            // Insert separator between bytes when provided.
            if (i > 0 && !string.IsNullOrEmpty(separator))
                sb.Append(separator);

            // Optional per-byte prefix.
            if (!string.IsNullOrEmpty(prefixPerByte))
                sb.Append(prefixPerByte);

            byte b = bytes[i];
            sb.Append(table[b >> 4]);
            sb.Append(table[b & 0xF]);
        }

        return sb;
    }
#endif
}
