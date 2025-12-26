using System.IO;
using System.Text;

#if !NET35 && !NET40

using System.Threading.Tasks;

#endif

namespace System;

/// <summary>
/// Stream helpers for reading and writing hexadecimal representations.
/// These helpers treat the stream content as UTF-8 encoded text when reading/writing hex.
/// </summary>
public static partial class HexConvert
{
    /// <summary>
    /// Writes the hexadecimal representation of the provided byte array to the target <see cref="Stream"/>.
    /// The output is written as UTF-8 encoded text.
    /// </summary>
    /// <param name="stream">Target stream to write to.</param>
    /// <param name="bytes">Bytes to format as hexadecimal.</param>
    /// <param name="uppercase">Use upper-case hex digits when <c>true</c>.</param>
    /// <param name="separator">Optional separator between bytes (e.g. " ").</param>
    /// <param name="prefixPerByte">Optional prefix before each byte (e.g. "0x").</param>
    public static void WriteHex(this Stream stream, byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));

        // Convert to hex string using existing API and write as UTF-8
        string hex = ToHexString(bytes, uppercase, separator, prefixPerByte);

        byte[] buffer = Encoding.UTF8.GetBytes(hex);
        stream.Write(buffer, 0, buffer.Length);
    }

#if !NET35 && !NET40

    /// <summary>
    /// Asynchronously writes the hexadecimal representation of the provided byte array to the target <see cref="Stream"/>.
    /// The output is written as UTF-8 encoded text.
    /// </summary>
    /// <param name="stream">Target stream to write to.</param>
    /// <param name="bytes">Bytes to format as hexadecimal.</param>
    /// <param name="uppercase">Use upper-case hex digits when <c>true</c>.</param>
    /// <param name="separator">Optional separator between bytes (e.g. " ").</param>
    /// <param name="prefixPerByte">Optional prefix before each byte (e.g. "0x").</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    public static async Task WriteHexAsync(this Stream stream, byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));

        string hex = ToHexString(bytes, uppercase, separator, prefixPerByte);
        byte[] buffer = Encoding.UTF8.GetBytes(hex);

#if NETCOREAPP2_1 || NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        await stream.WriteAsync(buffer.AsMemory()).ConfigureAwait(false);
#else
        await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
#endif
    }

#endif

    /// <summary>
    /// Reads the entire stream as UTF-8 encoded hex text and parses it into a byte array.
    /// The reader will read from the current position to the end of the stream.
    /// </summary>
    /// <param name="stream">Source stream containing hexadecimal text (UTF-8).</param>
    /// <returns>Parsed byte array.</returns>
    public static byte[] ReadHex(this Stream stream)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));

        using StreamReader reader = new(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024
#if !NET35 && !NET40
            , leaveOpen: true
#endif
            );
        string content = reader.ReadToEnd();
        return ToBytes(content);
    }

#if !NET35 && !NET40

    /// <summary>
    /// Asynchronously reads the entire stream as UTF-8 encoded hex text and parses it into a byte array.
    /// The reader will read from the current position to the end of the stream.
    /// </summary>
    /// <param name="stream">Source stream containing hexadecimal text (UTF-8).</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the parsed byte array.</returns>
    public static async Task<byte[]> ReadHexAsync(this Stream stream)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));

        using StreamReader reader = new(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string content = await reader.ReadToEndAsync().ConfigureAwait(false);
        return ToBytes(content);
    }

#endif
}
