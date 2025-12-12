using System;
using Xunit;
using System;
using Xunit;

namespace HexConvert.Tests;

public class HexConverterTests
{
    [Fact]
    public void RoundTrip_NoSeparator_Lowercase()
    {
        var data = new byte[] { 0x00, 0xAB, 0x5C, 0xFF };
        var hex = HexConvert.HexConverter.ToHexString(data);
        Assert.Equal("00ab5cff", hex);
        var parsed = HexConvert.HexConverter.FromHexString(hex);
        Assert.Equal(data, parsed);
    }

    [Fact]
    public void RoundTrip_WithSpaces_Prefix()
    {
        var data = new byte[] { 0x1, 0x2, 0x3, 0xF0 };
        var hex = HexConvert.HexConverter.ToHexString(data, uppercase: true, separator: " ", prefixPerByte: "0x");
        Assert.Equal("0x01 0x02 0x03 0xF0", hex);
        Assert.True(HexConvert.HexConverter.TryParseHexString(hex, out var parsed));
        Assert.Equal(data, parsed);
    }

    [Fact]
    public void Parse_OddLength_AddsLeadingZero()
    {
        string hex = "abc"; // should be interpreted as 0abc => 0A BC -> bytes 0x0A,0xBC
        Assert.True(HexConvert.HexConverter.TryParseHexString(hex, out var parsed));
        Assert.Equal(new byte[] { 0x0A, 0xBC }, parsed);
    }

    [Fact]
    public void Parse_Invalid_ReturnsFalse()
    {
        string hex = "zz";
        Assert.False(HexConvert.HexConverter.TryParseHexString(hex, out var _));
    }
}
n    [Fact]    }        Assert.Equal(data, parsed);        Assert.True(HexConvert.HexConverter.TryParseHexString(hex, out var parsed));        Assert.Equal("0x01 0x02 0x03 0xF0", hex);        var hex = HexConvert.HexConverter.ToHexString(data, uppercase: true, separator: " ", prefixPerByte: "0x");        var data = new byte[] { 0x1, 0x2, 0x3, 0xF0 };    {    public void RoundTrip_WithSpaces_Prefix()n    [Fact]