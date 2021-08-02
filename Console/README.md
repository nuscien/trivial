# [Trivial.Console](../docs/console)

This library includes these useful tools and controls about console app.

- Cmd arguments parser and processor.
- Rich components of CLI.

## Import

Just add following namespace to your code file to use.

```csharp
using Trivial.CommandLine;
using Trivial.Console;
```

## Line

We provide a line component for the standard output stream that you can write a string which you can update before the line terminator.
Following is a sample.

```csharp
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
```

And you can also read the password into a `SecureString` with the optional text mask.

```csharp
var line = new Line();

// Read password.
line.Write("Type password: ");
var password = line.ReadPassword('*', ConsoleColor.Yellow);
line.WriteLine();

// Write a secure string to the standard output stream.
line.Write("Your password is ");
line.Write(ConsoleColor.Magenta, password);
line.WriteLine('.');
```

## Select

You can output a beautiful list or table with customized style to the standard output stream 
so that user just need press the arrow buttons and `ENTER` in keyboard to select.

```csharp
// Create an instance for adding items and setting options.
var col = new SelectionData<string>();

// Add some items. For each item, you can set a string value,
// an additional data, an additional displayed title and an additional hot key.
col.Add('a', "char a", "a", "char [a]");
col.Add('b', "char b", "b", "char [b]");
for (var i = 0; i < 120; i++)
{
    col.Add("num " + i, i.ToString());
}

// Create an options for display.
var options = new SelectionOptions
{
    // You can define a question string after the list.
    Question = "Please select one: ",

    // We can define the colors of the item selected.
    SelectedForegroundColor = ConsoleColor.White,
    SelectedBackgroundColor = ConsoleColor.Blue,

    // The selected item will also be displayed after the question string.
    // So you can define its color.
    DefaultValueForegroundColor = ConsoleColor.Cyan,

    // At the end of the list, the tips will be displayed before user press any key.
    // There is a default value and you can customize it.
    // And you can disable it by set it as null.
    Tips = "Tips: You can use arrow key to select and press ENTER key to continue.",

    // Then you can define its color.
    TipsForegroundColor = ConsoleColor.Yellow,

    // You can define the prefix for the item and the one selected.
    SelectedPrefix = "√ ",
    Prefix = " ",

    // You can define the column count for the list.
    Column = 5,

    // You can define the maximum rows to displayed.
    // A paging will be displayed if the count of the list is greater than it.
    MaxRow = 10,

    // Press ESC can cancel this selection.
    // But you can enable the manual way by set a manual question
    // so that user can type the words directly.
    ManualQuestion = "Type: "
};

// Write it to the standard output stream and wait for user selection.
var result = LineUtilities.Select(col, options);

// You can get the result.
Console.WriteLine("The result is {0}.", result.Value);
```

## Progress

You can output a progress bar to update during a task running.

```csharp
// Define an options that you can custom the style.
var progressOptions = new ProgressLineOptions();

// Ouput the component in console and get the progress instance to update.
var progress = LineUtilities.WriteLine("Processing", progressOptions);

// A time-consuming work here.
for (var i = 0; i <= 50; i++)
{
    await Task.Delay(10);

    // And report the progress updated.
    progress.Report(0.02 * i);
}
```
