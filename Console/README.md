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
