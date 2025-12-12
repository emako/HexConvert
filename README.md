# HexConvert

轻量级 C# 十六进制转换库（示例实现），提供字节数组与十六进制字符串的相互转换，支持忽略空白、`0x` 前缀、可选分隔符和大小写控制。

## 包含内容

- `src/HexConvert/HexConvert.cs` - 核心实现
- `tests/HexConvert.Tests` - xUnit 单元测试示例

## 用法示例

```csharp
using HexConvert;

var data = new byte[] { 0x01, 0xAF };
string hex = HexConverter.ToHexString(data, uppercase: true, separator: " ", prefixPerByte: "0x");
// "0x01 0xAF"

byte[] parsed = HexConverter.FromHexString("0x01 0xAF");

// 宽容解析：支持空白、分隔符和单个 0x 前缀
```

## 构建与测试

需要安装 .NET SDK（推荐 .NET 7）。

在仓库根目录运行：

```bash
dotnet restore
dotnet build
dotnet test
```

## 许可

MIT

## References

https://github.com/faustodavid/HexConverter
