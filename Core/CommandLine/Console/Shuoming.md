# 命令行基本交互

在 `Trivial.CommandLine` [命名空间](../)中。

<!-- Start -->

一个适用于终端和命令提示符的自适应命令行界面。

## Console

我们为标准输出流提供了一个接口，可以在其中写入字符串，并在行终止符之前更新它。它支持终端和命令提示符。

以下是一个示例。

```csharp
var cli = StyleConsole.Default;
    
// 输出文本。
cli.Write("Loading {0}......", "something");

// 删除一些字符并添加其他带颜色的字符串。
cli.Backspace(3);
cli.Write(ConsoleColor.Green, "  Done!");

// 并且可以在同一行中以默认颜色追加其他字符串。
cli.Write(" This is only for test.");

// 添加行终止符。
cli.Write(Environment.NewLine);

// 所以下面的内容将出现在新的命令行界面中。
cli.Write("Next cli. ");

// 我们可以关闭自动刷新，以便稍后写入的所有字符串将进入输出队列。
cli.AutoFlush = false;
cli.Write(ConsoleColor.Red, ConsoleColor.Yellow, "Red foreground and yellow background");
cli.Write(Environment.NewLine);
cli.Write("This will not be output immediately, neither.");

// 现在让我们写入它们。
cli.Flush();

// 下面的内容将不会输出，直到我们调用 Flush 成员方法或将 AutoFlush 属性设置为 true。
cli.Write(" Hello?");
cli.AutoFlush = true;
cli.Write(Environment.NewLine);
```

并且还可以将密码读取到一个带有可选文本掩码的 `SecureString` 中。

```csharp
// 读取密码。
cli.Write("输入密码: ");
var password = cli.ReadPassword(ConsoleColor.Yellow, '*');
cli.WriteLine();

// 将安全字符串写入标准输出流。
cli.Write("密码是 ");
cli.Write(ConsoleColor.Magenta, password.ToUnsecureString());
cli.WriteLine('.');
```
