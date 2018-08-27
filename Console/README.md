You can use the library for your console application to parse the arguments, write the selection, dispatch verbs, etc.

# Import

Just add following namespace to your code file to use.

```csharp
using Trivial.Console;
```

# Parse

You can parse the arguments which is in string array or just in string to a readable object by programming.

```csharp
// Suppose the arguments is:
// a.exe --name Kingcean Tuan --say Hello

void Main(string[] args)
{
    var arguments = new Arguments(args);
    Console.WriteLine("{0} {1}", arguments["say"], arguments["name"]);

    // Console -> Hello Kingcean Tuan
}
```

It still work well when the arguments is from a string.

```csharp
void Main(string[] args)
{
    var str = Console.ReadLine();

    // Suppose the str is:
    // hijklmn abcdefg

    var arguments = new Arguments(str);
    Console.WriteLine("{0} {1}", arguments[1], arguments[0]);

    // Console -> abcdefg hijklmn
}
```

# Verb

If your application contains a set of functionalities, you may need verbs to help you to build a more flexible console application.
Each verb can has its own business logic. The arguments will be filled into the property with attribute `ArgumentAttribute`.

```csharp
class FirstVerb: Verb
{
    [Argument("name")]
    public string Name { get; set; }

    public override string Description => "Test 1";

    public override void Process()
    {
        Console.WriteLine("This is the verb handler 1.");
        Console.WriteLine("Name is {0}.", Name);
        Console.WriteLine("Name is {0}.", Arguments["name"]);
    }
}
```

And you can also define a verb handler with async process method.

```csharp
class SecondVerb: AsyncVerb
{
    public override string Description => "Test 2";

    public override async Task Process()
    {
        Console.WriteLine("This is the verb handler 2. Step 1.");
        await Task.Run(() => {
            Console.WriteLine("This is the verb handler 2. Step 2.");
        });
    }
}
```

Then you can use dispatcher to dispatch the correct verb handler to process.

```csharp
void Main(string[] args)
{
    var dispatcher = new Dispatcher();
    dispatcher.Register<FirstVerb>("one");
    dispatcher.Register<SecondVerb>("two");

    dispatcher.Process(args);

    // a.exe one --name Test
    // Console -> This is the verb handler 1.
    // Console -> Name is Test.
    // Console -> Name is Test.

    // a.ext two
    // Console -> This is the verb handler 2. Step 1.
    // Console -> This is the verb handler 2. Step 2.
}
```

# Line

You can write a specified string value to the standard output stream and then change it. Following is a sample.

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

# Select

You can have a way to list a lot of items so that user can select one of them just by arrow key.

```csharp
var Main(string[] args)
{
    // Create an instance for adding items and setting options.
    var col = new Selection<string>();

    // Add some items. For each item, you can set a string value,
    // an additional data, an additional displayed title and an additional hot key.
    col.Add("char a", "a", "char [a]", 'a');
    col.Add("char b", "b", "char [b]", 'b');
    for (var i = 0; i < 120; i++)
    {
        col.Add("num " + i, i.ToString());
    }

    // You can define a question string after the list.
    col.Question = "Please select one: ";

    // We can define the colors of the item selected.
    col.SelectedForegroundColor = ConsoleColor.White;
    col.SelectedBackgroundColor = ConsoleColor.Blue;

    // The selected item will also be displayed after the question string.
    // So you can define its color.
    col.DefaultValueForegroundColor = ConsoleColor.Cyan;

    // At the end of the list, the tips will be displayed before user press any key.
    // There is a default value and you can customize it.
    // And you can disable it by set it as null.
    col.Tips = "Tips: You can use arrow key to select and press ENTER key to continue.";

    // Then you can define its color.
    col.TipsForegroundColor = ConsoleColor.Yellow;

    // You can define the prefix for the item and the one selected.
    col.SelectedPrefix = "√ ";
    col.Prefix = " ";

    // You can define the column count for the list.
    col.Column = 5;

    // You can define the maximum rows to displayed.
    // A paging will be displayed if the count of the list is greater than it.
    col.MaxRow = 10;

    // Press ESC can cancel this selection.
    // But you can enable the manual way by set a manual question
	// so that user can type the words directly.
    col.ManualQuestion = "Type: ";

    // Write it to the standard output stream and wait for user selection.
    var result = LineUtilities.Select(col);

	// You can get the result.
    Console.WriteLine("The result is {0}.", result.Value);
}
```
