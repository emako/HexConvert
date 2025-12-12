using System.Globalization;
using System.Text;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System;

#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Utility helpers for converting between byte arrays and hexadecimal strings.
/// Supports optional separators and tolerant parsing (ignores whitespace and 0x prefixes).
/// </summary>
public static class HexConvert
{
    public static string ToHexString(this byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)
    {
        if (bytes is null)
            throw new ArgumentNullException(nameof(bytes));

        StringBuilder sb = new(bytes.Length * (2 + (separator?.Length ?? 0) + (prefixPerByte?.Length ?? 0)));
        string format = uppercase ? "X2" : "x2";

        for (int i = 0; i < bytes.Length; i++)
        {
            if (i > 0 && !string.IsNullOrEmpty(separator))
                sb.Append(separator);

            if (!string.IsNullOrEmpty(prefixPerByte))
                sb.Append(prefixPerByte);

            sb.Append(bytes[i].ToString(format, CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }

    public static byte[] ToBytes(this string hex)
    {
        if (!TryParse(hex, out byte[]? bytes))
            throw new FormatException("Input string is not a valid hexadecimal representation.");

        return bytes!;
    }

    public static bool TryParse(string? hex, out byte[]? bytes)
    {
        bytes = null;

#if NET35
        if (StringUtils.IsNullOrWhiteSpace(hex))
#else
        if (string.IsNullOrWhiteSpace(hex))
#endif
        {
            bytes = [];
            return true;
        }

        string s = hex!.Trim();
        StringBuilder cleaned = new(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (char.IsWhiteSpace(c) || c == '-' || c == ':' || c == ',')
                continue;

            // skip leading 0x/0X sequences
            if (c == '0' && i + 1 < s.Length && (s[i + 1] == 'x' || s[i + 1] == 'X'))
            {
                i++; // skip the 'x'
                continue;
            }

            if (c == 'x' || c == 'X')
                continue;

            if (FromHexChar(c) < 0)
            {
                bytes = null;
                return false;
            }

            cleaned.Append(c);
        }

        if (cleaned.Length == 0)
        {
            bytes = [];
            return true;
        }

        if ((cleaned.Length & 1) != 0)
            cleaned.Insert(0, '0');

        int len = cleaned.Length / 2;
        byte[] result = new byte[len];

        for (int i = 0; i < len; i++)
        {
            int hi = FromHexChar(cleaned[2 * i]);
            int lo = FromHexChar(cleaned[2 * i + 1]);

            if (hi < 0 || lo < 0)
            {
                bytes = null;
                return false;
            }
            result[i] = (byte)((hi << 4) | lo);
        }

        bytes = result;
        return true;
    }

    private static int FromHexChar(char c)
    {
        if (c >= '0' && c <= '9') return c - '0';
        if (c >= 'a' && c <= 'f') return c - 'a' + 10;
        if (c >= 'A' && c <= 'F') return c - 'A' + 10;
        return -1;
    }
}

#if NET35
file static class StringUtils
{
    public static bool IsNullOrWhiteSpace(string? value)
    {
        if (value == null)
            return true;

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i]))
                return false;
        }

        return true;
    }
}
#endif
