using System;
using System.Diagnostics.CodeAnalysis;
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
}
