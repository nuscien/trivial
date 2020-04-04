# Console selection

A console component to let user select an item in a specific list by keyboard.

In `Trivial.Console` [namespace](./README) of `Trivial.Console.dll` [library](../README).

## Console selection component

You can output a beautiful list or table with customized style to the standard output stream so that user just need press the arrow keys and `ENTER` in keyboard to select.

```csharp
var Main(string[] args)
{
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
}
```
