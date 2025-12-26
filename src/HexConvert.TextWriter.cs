using System.IO;
using System.Text;

namespace System;

/// <summary>
/// Extension helpers to write hexadecimal representations to writers and trace listeners.
/// Note: C# does not allow extension methods on static classes like <see cref="Console"/>.
/// Use these extensions on <see cref="TextWriter"/> (e.g. <c>Console.Out</c>) instances instead.
/// </summary>
public static partial class HexConvert
{
    /// <summary>
    /// Writes the hexadecimal representation of a byte array to the provided <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">Destination writer (for example <c>Console.Out</c>).</param>
    /// <param name="bytes">Byte array to format as hexadecimal.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits.</param>
    /// <param name="separator">Optional separator inserted between bytes (for example a space or dash).</param>
    /// <param name="prefixPerByte">Optional prefix to prepend before each byte (for example "0x").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="bytes"/> is <see langword="null"/>.</exception>
    public static void WriteHex(this TextWriter writer, byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        writer.Write(ToHexString(bytes, uppercase, separator, prefixPerByte));
    }

    /// <summary>
    /// Writes the hexadecimal representation of a byte array followed by a newline to the provided <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">Destination writer (for example <c>Console.Out</c>).</param>
    /// <param name="bytes">Byte array to format as hexadecimal.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits.</param>
    /// <param name="separator">Optional separator inserted between bytes (for example a space or dash).</param>
    /// <param name="prefixPerByte">Optional prefix to prepend before each byte (for example "0x").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="bytes"/> is <see langword="null"/>.</exception>
    public static void WriteHexLine(this TextWriter writer, byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        writer.WriteLine(ToHexString(bytes, uppercase, separator, prefixPerByte));
    }

#if !NET35 && !NET40 && !NETSTANDARD1_0
    /// <summary>
    /// Writes the hexadecimal representation of a read-only span of bytes to the provided <see cref="TextWriter"/>.
    /// This overload avoids allocating an intermediate array on platforms that support <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="writer">Destination writer (for example <c>Console.Out</c>).</param>
    /// <param name="bytes">Span of bytes to format as hexadecimal.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits.</param>
    /// <param name="separator">Optional separator inserted between bytes (for example a space or dash).</param>
    /// <param name="prefixPerByte">Optional prefix to prepend before each byte (for example "0x").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteHex(this TextWriter writer, ReadOnlySpan<byte> bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        StringBuilder sb = new(bytes.Length * (2 + (separator?.Length ?? 0) + (prefixPerByte?.Length ?? 0)));
        sb.AppendHex(bytes, uppercase, separator, prefixPerByte);
        writer.Write(sb.ToString());
    }

    /// <summary>
    /// Writes the hexadecimal representation of a read-only span of bytes followed by a newline to the provided <see cref="TextWriter"/>.
    /// This overload avoids allocating an intermediate array on platforms that support <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="writer">Destination writer (for example <c>Console.Out</c>).</param>
    /// <param name="bytes">Span of bytes to format as hexadecimal.</param>
    /// <param name="uppercase">If <see langword="true"/>, use upper-case hex digits.</param>
    /// <param name="separator">Optional separator inserted between bytes (for example a space or dash).</param>
    /// <param name="prefixPerByte">Optional prefix to prepend before each byte (for example "0x").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteHexLine(this TextWriter writer, ReadOnlySpan<byte> bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        StringBuilder sb = new(bytes.Length * (2 + (separator?.Length ?? 0) + (prefixPerByte?.Length ?? 0)));
        sb.AppendHex(bytes, uppercase, separator, prefixPerByte);
        writer.WriteLine(sb.ToString());
    }
#endif
}
