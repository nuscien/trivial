# CLI

An adaptive command line interface for both terminal and command prompt.

In `Trivial.CommandLine` [namespace](../) of `Trivial.dll` [library](../../).

## Console

We provide an interface for the standard output stream that you can write a string which you can update before the line terminator.
It supports both terminal and command prompt.
Following is a sample.

```csharp
var cli = StyleConsole.Default;
    
// Write something.
cli.Write("Loading {0}......", "something");

// Remove some charactors and add other string with color.
cli.Backspace(3);
cli.Write(ConsoleColor.Green, "  Done!");

// And you can append other string following above in the same line and in the default color.
cli.Write(" This is only for test.");

// Add terminator.
cli.Write(Environment.NewLine);

// So following will be in a new cli.
cli.Write("Next cli. ");

// We can turn off the auto flush so that all strings write later will be in an output queue.
cli.AutoFlush = false;
cli.Write(ConsoleColor.Red, ConsoleColor.Yellow, "Red foreground and yellow background");
cli.Write(Environment.NewLine);
cli.Write("This will not be output immediately, neither.");

// Now let's write them.
cli.Flush();

// Following will not output until we call Flush member method or set AutoFlush property as true.
cli.Write(" Hello?");
cli.AutoFlush = true;
cli.Write(Environment.NewLine);
```

And you can also read the password into a `SecureString` with the optional text mask.

```csharp
// Read password.
cli.Write("Type password: ");
var password = cli.ReadPassword(ConsoleColor.Yellow, '*');
cli.WriteLine();

// Write a secure string to the standard output stream.
cli.Write("Your password is ");
cli.Write(ConsoleColor.Magenta, password.ToUnsecureString());
cli.WriteLine('.');
```
