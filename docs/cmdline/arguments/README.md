# Console arguments

A console component to let user write text.

In `Trivial.CommandLine` [namespace](../) of `Trivial.dll` [library](../../).

## Arguments

The command arguments is an array of string. You can parse it to a program readable object with structured information.

```csharp
void Main(string[] args)
{
    // Parse arguments from a string array.
    var arguments = new Arguments(args);

    // You can get the value of each parameter by key (case-insensitive).
    Console.WriteLine("{0} {1}", arguments["say"], arguments["name"]);

    // Parse arguments from a string.
    var str = Console.ReadLine();
    arguments = new Arguments(str);

    // You can get the value by index.
    Console.WriteLine("{0} {1}", arguments[1], arguments[0]);
}
```

Console:

```sh
> a.exe --name Kingcean Tuan --say Hello
Hello Kingcean Tuan
> hijklmn abcdefg
abcdefg hijklmn
```

## Verb

If your application contains a set of functionality, you may need verbs to help you to build a more flexible console application. Each verb can has its own business logic. The arguments will be filled into the property with attribute `ArgumentAttribute`.

```csharp
class FirstVerb : Verb
{
    [Argument("name")]
    public string Name { get; set; }

    public bool HasName => HasParameter("name");

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
class SecondVerb : AsyncVerb
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
}
```

Console:

```sh
> a.exe one --name Test
This is the verb handler 1.
Name is Test.
Name is Test.
> a.exe two
This is the verb handler 2. Step 1.
This is the verb handler 2. Step 2.
```
