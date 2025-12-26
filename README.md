# HexConvert

Lightweight C# hexadecimal conversion library. Provides conversion between byte arrays and hexadecimal strings and offers lenient parsing options such as ignoring whitespace, accepting `0x` prefixes, configurable separators, and uppercase/lowercase output.

## Contents

- `src/` - library project (`System.HexConvert`)
- `src/System.HexConvert.csproj` - multi-targeted project
- `tests/` - unit test project (`System.HexConvert.Tests`)

## Features

- Convert `byte[]` to hex string and back
- Accepts and ignores whitespace and separators when parsing
- Tolerates single or per-byte `0x` prefixes
- Configurable uppercase/lowercase output and optional separators/prefixes

## Supported target frameworks

This repository contains a multi-targeted library project covering a wide range of .NET frameworks and versions. Examples present in this workspace include (but are not limited to):

- .NET Standard: 1.0 → 2.1
- .NET Framework: 3.5, 4.0, 4.5 → 4.8.1
- .NET Core: 1.0 → 3.1
- .NET (formerly .NET Core): 5 → 10

Choose the appropriate target for your project when referencing the library.

## Usage example

```csharp
using System.HexConvert; // namespace may vary depending on the build

var data = new byte[] { 0x01, 0xAF };
string hex = HexConverter.ToHexString(data, uppercase: true, separator: " ", prefixPerByte: "0x");
// "0x01 0xAF"

byte[] parsed = HexConverter.FromHexString("0x01 0xAF");

// Parsing is lenient: accepts whitespace, separators and single or per-byte 0x prefixes
```

## Build and test

Requires the .NET SDK. Recommended: .NET 7 or later.

From the repository root run:

```bash
dotnet restore
dotnet build
dotnet test
```

## License

MIT

## References

- Original sources and inspiration: https://github.com/faustodavid/HexConverter
- This repository: https://github.com/emako/HexConvert
