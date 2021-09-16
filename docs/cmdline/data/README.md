# Data Formatter

Format and output the struct to terminal.

In `Trivial.CommandLine` [namespace](../) of `Trivial.Console.dll` [library](../../).

## JSON

![Screenshot](./json.jpg)

Following is a sample to format JSON into command line app.

```csharp
var json = new Trivial.Text.JsonObjectNode();
// and then add some properties to json.
DefaultConsole.WriteLine(json);
```

This is not available for Command Prompt.

## Exception

Following is a sample to format an exception into command line app.

```csharp
var ex = new InvalidOperationException();
DefaultConsole.WriteLine(ex);
```
