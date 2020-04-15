# Console line

A console component to let user write text.

In `Trivial.Console` [namespace](../) of `Trivial.Console.dll` [library](../../).

## Line control

Line control `Line` is used to control the I/O behavior and result of standard output stream.

- We can output a string with specific foreground (text) color and background color;
- we can output the string into a buffer and flush it when we need, and;
- we can remove the string in the buffer or already in the standard output stream before the line terminator.

Following is a sample.

```csharp
var Main(string[] args)
{
    var line = new Line();
    
    // Write something.
    line.Write("Loading {0}......", "something");

    // Remove some charactors and add other string with color.
    line.Backspace(3);
    line.Write(ConsoleColor.Green, "  Done!");

    // And you can append other string following above in the same line and in the default color.
    line.Write(" This is only for test.");

    // Add terminator.
    line.End();

    // You cannot add further terminator if there is no any string output.
    line.End();

    // So following will be in a new line.
    line.Write("Next line. ");

    // We can turn off the auto flush so that all strings write later will be in an output queue.
    line.AutoFlush = false;
    line.Write(ConsoleColor.Red, ConsoleColor.Yellow, "Red foreground and yellow background");
    line.End();
    line.Write("This will not be output immediately, neither.");

    // Now let's write them.
    line.Flush();

    // Following will not output until we call Flush member method or set AutoFlush property as true.
    line.Write(" Hello?");
    line.AutoFlush = true;
    line.End();
}
```

And we can also read password into a `SecureString` with the optional text mask.
It also support to remove the last input by `BACKSPACE` key and clear by `DELETE` key.

```csharp
var Main(string[] args)
{
    var line = new Line();

    // Read password.
    line.Write("Type password: ");
    var password = line.ReadPassword('*', ConsoleColor.Yellow);
    line.WriteLine();

    // Write a secure string to the standard output stream.
    line.Write("Your password is ");
    line.Write(ConsoleColor.Magenta, password);
    line.WriteLine('.');
}
```

Tips: You can use the [secure string extensions](./secure-string#secure-string-extensions) (`Trivial.Security.SecureStringExtensions` in `Trivial.dll` library) to convert the `SecureString` from/to `String`.
