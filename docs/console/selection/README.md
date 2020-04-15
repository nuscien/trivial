# Console selection

A console component to let user select an item in a specific list by keyboard.

In `Trivial.Console` [namespace](../) of `Trivial.Console.dll` [library](../../).

## Selection control

Sometimes we need provide some options to users to select to continue the rest workflow or action.
The simplist and old-style way is to show a hint which tells user the options and their shortcut keys (or keyword to type).
It is hard to handle when either the count of options are greater than 3 or any of the option tips is too long.

But with this control, you can output a beautiful list or table with customized style to the standard output stream
so that the user just need press the arrow keys and `ENTER` in keyboard to select.

To do so, you need define a list `SelectionData` to add all options.
Then call `LineUtilities.Select(SelectionData, SelectionOptions)` static method to output the control to But with this component, you can output a beautiful list or table with customized style to the standard output stream.
The second parameter of the method is optional that you can initialize an instance to cusotmize the style and other settings.
This method returns a result containing the information about what user select.
Following is a sample.

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
