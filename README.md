![logo](branding/titlebar.png)

[![GitHub license](https://img.shields.io/github/license/emako/HexConvert)](https://github.com/emako/HexConvert/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/v/HexConvert.svg)](https://nuget.org/packages/HexConvert) [![Actions](https://github.com/emako/HexConvert/actions/workflows/library.nuget.yml/badge.svg)](https://github.com/emako/HexConvert/actions/workflows/library.nuget.yml)

# HexConvert

Lightweight C# hexadecimal conversion library. This repository contains a small, multi-targeted implementation of helpers to convert between byte arrays and hexadecimal strings with several API flavors: simple string APIs, `StringBuilder` extension helpers, high-performance `Span`-based APIs and Trace/Debug helpers. The library is implemented in the `System` namespace and exposes a static `HexConvert` helper.

## Project layout

- `src/HexConvert.cs` - core string-based APIs: `ToHexString`, `ToBytes`, `TryParse` (lenient parser)
- `src/HexConvert.StringBuilder.cs` - `StringBuilder` extension methods: `AppendHex` (byte, byte[], `ReadOnlySpan<byte>` on supported platforms)
- `src/HexConvert.Span.cs` - high-performance `Span`-based APIs: `GetString`, `GetChars`, `GetBytes`, pooled helpers and buffer-aware overloads
- `src/HexConvert.TraceListener.cs` - helpers for writing hex output to `TraceListener` (`WriteHex`, `WriteHexLine`)
- `src/HexConvert.Stream.cs` - stream helpers for reading/writing hex text (`WriteHex`, `WriteHexAsync`, `ReadHex`, `ReadHexAsync`)
- `tests/` - unit tests (`System.HexConvert.Tests`)

## Namespace and public types

- Namespace: `System`
- Static partial class: `HexConvert`
- Key methods (overview):
  - `string ToHexString(this byte[] bytes, bool uppercase = false, string? separator = null, string? prefixPerByte = null)`
  - `byte[] ToBytes(this string hex)` and `bool TryParse(string? hex, out byte[]? bytes)` — lenient parsing that ignores whitespace, common separators, and `0x` prefixes; odd-length input is padded with a leading `0`
  - `StringBuilder.AppendHex(...)` overloads for single `byte`, `byte[]`, and `ReadOnlySpan<byte>` (when supported)
  - `GetString(ReadOnlySpan<byte>)`, `GetChars(ReadOnlySpan<byte>, Span<char>)`, `GetBytes(ReadOnlySpan<char>, Span<byte>)`, and pooled variants (returning a `RentedArraySegmentWrapper<T>`)
  - `TraceListener.WriteHex(...)` and `TraceListener.WriteHexLine(...)` overloads for `byte[]` and `ReadOnlySpan<byte>` (when supported)
  - `Stream` helpers: `WriteHex(Stream, byte[])`, `WriteHexAsync(Stream, byte[])`, `ReadHex(Stream)`, `ReadHexAsync(Stream)` — operate on UTF-8 text and use existing conversion APIs
  - `RentedArraySegmentWrapper<T>`: a small readonly struct that wraps a rented array segment and returns the array to the pool on `Dispose()`

## Behavior and features

- Parsing is lenient and tolerant:
  - Ignores whitespace characters and common separators such as `-`, `:`, `,`
  - Skips `0x` / `0X` prefixes (both single leading prefix and per-byte `0x` sequences)
  - Accepts odd-length hex input by padding with a leading `0`
  - Returns `false` from `TryParse` (or throws `FormatException` from `ToBytes`) when invalid hex characters are present
- Formatting features:
  - Choose uppercase or lowercase hex digits
  - Optional separator string between bytes (e.g. space or dash)
  - Optional per-byte prefix (e.g. `"0x"`)
- High-performance span APIs avoid allocations and offer buffer-aware overloads; pooled helpers use `ArrayPool<T>` to minimize allocations
- Stream helpers read/write UTF-8 text and delegate parsing/formatting to the existing APIs. Async methods are provided where supported. Stream readers/writers use `leaveOpen: true` on platforms that support the parameter to avoid closing the underlying stream.

## Examples

Basic string-based conversion

```csharp
using System;

byte[] data = { 0x01, 0xAF };
string hex = HexConvert.ToHexString(data, uppercase: true, separator: " ", prefixPerByte: "0x");
// hex == "0x01 0xAF"

byte[] parsed = HexConvert.ToBytes("0x01 0xAF");
// or
if (HexConvert.TryParse("0x1 AF", out var bytes)) { /* use bytes */ }
```

Using StringBuilder extensions

```csharp
var sb = new System.Text.StringBuilder();
sb.AppendHex(new byte[] { 0x0A, 0xFF }, uppercase: false, separator: ":", prefixPerByte: null);
Console.WriteLine(sb.ToString()); // "0a:ff"
```

Span-based (high-performance) APIs

```csharp
ReadOnlySpan<byte> span = new byte[] { 0x01, 0xAB };
string s = HexConvert.GetString(span);

// Get bytes from a hex char span into a pre-allocated buffer
char[] chars = s.ToCharArray();
Span<byte> outBuffer = stackalloc byte[chars.Length / 2 + 1];
int written = HexConvert.GetBytes(chars, outBuffer); // on supported platforms use ReadOnlySpan<char>
```

Using pooled helpers (remember to dispose)

```csharp
ReadOnlySpan<char> hexChars = "abcd".AsSpan();
using var rented = HexConvert.GetBytesPooled(hexChars);
byte[] bytes = rented.ArraySegment.Array!; // note: ArraySegment and length available on the wrapper
// When disposed the underlying array is returned to the pool
```

Trace listener helpers

```csharp
var listener = new System.Diagnostics.ConsoleTraceListener();
listener.WriteHex(new byte[] { 1, 2, 3 }, uppercase: true, separator: " ", prefixPerByte: "0x");
```

Stream helpers

```csharp
// Write to a MemoryStream (synchronous)
using var ms = new MemoryStream();
byte[] data = { 0x01, 0x02 };
ms.WriteHex(data, uppercase: true, separator: " ", prefixPerByte: "0x");
ms.Position = 0;
using var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
string text = sr.ReadToEnd(); // "0x01 0x02"

// Read from a stream
using var input = new MemoryStream(Encoding.UTF8.GetBytes("0x0A0B"));
byte[] parsed = input.ReadHex(); // { 0x0A, 0x0B }

// Async variants are available on supported platforms: WriteHexAsync / ReadHexAsync
```

## Supported target frameworks

The project in `src/System.HexConvert.csproj` is multi-targeted and aims to be widely compatible. The repository includes support for many targets (examples):

- .NET Standard 1.0 → 2.1
- .NET Framework 3.5 → 4.8.1
- .NET Core 1.0 → 3.1
- .NET 5 → 10

Pick the appropriate target when referencing the library.

## Build and test

Requires the .NET SDK. Recommended: .NET 7 or later.

From the repository root:

```bash
dotnet restore
dotnet build
dotnet test
```

## License

[MIT](LICENSE)

## References

- Original inspiration: https://github.com/faustodavid/HexConverter
- This repository: https://github.com/emako/HexConvert
