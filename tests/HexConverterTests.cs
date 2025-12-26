using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Xunit;

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public sealed class HexConverterTests
{
    [Fact]
    public void RoundTrip_NoSeparator_Lowercase()
    {
        byte[] data = [0x00, 0xAB, 0x5C, 0xFF];
        string hex = data.ToHexString();
        Assert.Equal("00ab5cff", hex);

        byte[] parsed = hex.ToBytes();
        Assert.Equal(data, parsed);
    }

    [Fact]
    public void RoundTrip_WithSpaces_Prefix()
    {
        byte[] data = [0x1, 0x2, 0x3, 0xF0];
        string hex = data.ToHexString(uppercase: true, separator: " ", prefixPerByte: "0x");

        Assert.Equal("0x01 0x02 0x03 0xF0", hex);
        Assert.True(HexConvert.TryParse(hex, out byte[] parsed));
        Assert.Equal(data, parsed);
    }

    [Fact]
    public void AppendHex_ByteArray_Produces_Same_String_As_ToHexString()
    {
        byte[] data = [0x10, 0x20, 0x30, 0xAB];
        string expected = data.ToHexString(uppercase: false, separator: "-", prefixPerByte: null);

        StringBuilder sb = new();
        sb.AppendHex(data, uppercase: false, separator: "-", prefixPerByte: null);

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendHex_Span_Produces_Same_String_As_ToHexString_With_Prefix()
    {
        byte[] data = [0x01, 0x02, 0x03];
        string expected = data.ToHexString(uppercase: true, separator: " ", prefixPerByte: "0x");

        StringBuilder sb = new();
        sb.AppendHex(data.AsSpan(), uppercase: true, separator: " ", prefixPerByte: "0x");

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendHex_Single_Byte_Works_With_Prefix_And_Case()
    {
        StringBuilder sb = new();
        sb.AppendHex(0x5C, uppercase: true, prefix: "0x");
        Assert.Equal("0x5C", sb.ToString());
    }

    [Fact]
    public void TextWriter_WriteHex_Writes_Correct()
    {
        byte[] data = [0xDE, 0xAD, 0xBE, 0xEF];
        string expected = data.ToHexString();

        using StringWriter sw = new();
        sw.WriteHex(data, uppercase: false, separator: null, prefixPerByte: null);

        Assert.Equal(expected, sw.ToString());
    }

    [Fact]
    public void TextWriter_WriteHexLine_Writes_Correct()
    {
        byte[] data = [0x0A, 0x0B];
        string expected = data.ToHexString();

        using StringWriter sw = new();
        sw.WriteHexLine(data, uppercase: false, separator: null, prefixPerByte: null);

        Assert.Equal(expected + Environment.NewLine, sw.ToString());
    }

    [Fact]
    public void TraceListener_WriteHex_Writes_Correct()
    {
        byte[] data = [0x01, 0x02, 0x03];
        string expected = data.ToHexString(uppercase: true, separator: " ", prefixPerByte: "0x");

        using StringWriter sw = new();
        var listener = new TextWriterTraceListener(sw);
        listener.WriteHex(data, uppercase: true, separator: " ", prefixPerByte: "0x");
        listener.Flush();

        Assert.Equal(expected, sw.ToString());
    }

    [Fact]
    public void TraceListener_WriteHexLine_Writes_Correct()
    {
        byte[] data = [0xFF];
        string expected = data.ToHexString();

        using StringWriter sw = new();
        var listener = new TextWriterTraceListener(sw);
        listener.WriteHexLine(data, uppercase: false, separator: null, prefixPerByte: null);
        listener.Flush();

        Assert.Equal(expected + Environment.NewLine, sw.ToString());
    }
}
