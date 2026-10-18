# Console arguments

在 `Trivial.CommandLine` [命名空间](../)中。

<!-- Start -->

一个控制台组件，允许用户输入文本。

## Arguments

命令行参数是一个字符串数组。可以将其解析为具有结构化信息的程序可读对象。

```csharp
void Main(string[] args)
{
    // 从字符串数组解析参数。
    var arguments = new Arguments(args);

    // 可以通过键（不区分大小写）获取每个参数的值。
    Console.WriteLine("{0} {1}", arguments["say"], arguments["name"]);

    // 从字符串解析参数。
    var str = Console.ReadLine();
    arguments = new Arguments(str);

    // 可以通过索引获取值。
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

如果的应用程序包含一组功能，可能需要动词来帮助构建更灵活的控制台应用程序。每个动词可以有自己的业务逻辑。参数将填充到带有 `ArgumentAttribute` 属性的属性中。

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

并且还可以定义一个具有异步处理方法的动词处理程序。

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

然后可以使用调度程序来分派正确的动词处理程序进行处理。

```csharp
void Main(string[] args)
{
    var dispatcher = new Dispatcher();
    dispatcher.Register<FirstVerb>("one");
    dispatcher.Register<SecondVerb>("two");

    dispatcher.Process(args);
}
```

控制台:

```sh
> a.exe one --name Test
This is the verb handler 1.
Name is Test.
Name is Test.
> a.exe two
This is the verb handler 2. Step 1.
This is the verb handler 2. Step 2.
```
