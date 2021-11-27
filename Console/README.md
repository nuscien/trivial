# [Trivial.Console](https://trivial.kingcean.net/cmdline)

This library includes a lot of useful rich command line controls.

## Import

Add following namespace to your code file to use.

```csharp
using Trivial.CommandLine;
```

## Select

A beautiful list or table with customized style to the standard output stream 
so that user just need press the arrow buttons and `ENTER` in keyboard to select.

```csharp
// Create an instance for adding items and setting options.
var col = new Trivial.Collection.SelectionData<string>();

// Add some items. For each item, you can set a hotkey, a display name and the data.
col.Add('a', "char a", "a");
col.Add('b', "char b", "b");
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
    SelectedForegroundConsoleColor = ConsoleColor.White,
    SelectedBackgroundConsoleColor = ConsoleColor.Blue,

    // The selected item will also be displayed after the question string.
    // So you can define its color.
    ItemForegroundConsoleColor = ConsoleColor.Cyan,

    // At the end of the list, the tips will be displayed before user press any key.
    // There is a default value and you can customize it.
    // And you can disable it by set it as null.
    Tips = "Tips: You can use arrow key to select and press ENTER key to continue.",

    // Then you can define its color.
    TipsForegroundConsoleColor = ConsoleColor.Yellow,

    // You can define the prefix for the item and the one selected.
    SelectedPrefix = "> ",
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
var result = DefaultConsole.Select(col, options);

// You can get the result.
DefaultConsole.WriteLine("The result is {0}.", result.Value);
```

## Progress

A progress bar in terminal that can present the latest state during the specific task running.

```csharp
// Define an options that you can custom the style.
var progressStyle = new ConsoleProgressStyle
{
    ValueConsoleColor = ConsoleColor.White
};

// Ouput the component in console and get the progress instance to update.
var progress = DefaultConsole.WriteLine(progressStyle, "Processing");

// A time-consuming work here.
for (var i = 0; i <= 50; i++)
{
    await Task.Delay(10);

    // And report the progress updated.
    progress.Report(0.02 * i);
}
```

## JSON

Following is a sample to format JSON into terminal.

```csharp
var json = new Trivial.Text.JsonObjectNode();
// and then add some properties to json.
DefaultConsole.WriteLine(json);
```

## Linear gradient

Output a string with linear gradient.

```csharp
DefaultConsole.WriteLine(new LinearGradientConsoleStyle(
    ConsoleColor.Gray,  // Fallback color.
    Color.FromArgb(15, 250, 250),   // From color.
    Color.FromArgb(85, 168, 255))   // To color
    {
        Bold = true     // Additional font style
    },"Trivial Sample");
```
