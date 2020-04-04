# Trivial.Console

The useful utilities for console application. You can use the library free for your console application to parse the arguments, dispatch verbs, etc. It also contains some rich user interface console controls, such as selection, for you use.

### Import

This library targets .NET Standard 2.0.

You can install the package from [NuGet](https://www.nuget.org/packages/Trivial.Console) to your project by following way.

```sh
PM > Install-Package Trivial.Console
```

Or you can also clone the repository to build and add the reference to your project.

### Namespace

Then add following namespace to your code file to use.

```csharp
using Trivial.Console;
```

### Features

- [Arguments](./arguments) An instance provided a way to access values from the arguments and dispatch the command verbs.
- [Line](./line) You can write a temporary line to the standard output stream and update it later when need before the line terminator.
- [Selection](./selection) You can write a selection to the standard output stream so that use can use arrow key and other keys to select the item.
- [Progress](./progress) You can write a progress bar to the standard output stream to show a status value update.
