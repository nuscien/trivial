You can use the library for your console application to parse the arguments and manage the verbs.

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

# Utilities

You can write a specified string value to the standard output stream and then change it.
And you can have a way to list a lot of items so that user can select one of them just by arrow key.

To create an instance for write a string with ability to manage it later, you can create a class by following sample.

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

If you have a list and want user select, you can code like following sample.

```csharp
var Main(string[] args)
{
    var col = new Selection<string>();
    col.Add("char a", "a", "char [a]", 'a');
    col.Add("char b", "b", "char [b]", 'b');
    for (var i = 0; i < 120; i++)
    {
        col.Add("num " + i, i.ToString());
    }

    col.SelectedForegroundColor = ConsoleColor.White;
    col.SelectedBackgroundColor = ConsoleColor.Blue;
    col.DefaultValueForegroundColor = ConsoleColor.Cyan;
    col.TipsForegroundColor = ConsoleColor.Yellow;
    col.SelectedPrefix = "√ ";
    col.Prefix = " ";
    col.MaxRow = 10;
    col.Column = 5;
    col.ManualQuestion = "Type: ";
    var result = LineUtilities.Select(col);

    Console.WriteLine("The result is {0}.", result.Value);
}
```
