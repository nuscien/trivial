# Trivial libraries

[![MIT licensed](./assets/badge_lisence_MIT.svg)](https://github.com/nuscien/trivial/blob/master/LICENSE)
[![Build Status](https://dev.azure.com/nuscien/trivial/_apis/build/status/nuscien.trivial?branchName=master)](https://dev.azure.com/nuscien/trivial/_build/latest?definitionId=1&branchName=master)

Trivial libraries contain a lot of utilities, models, extensions and components as one of .NET supplementary, which are useful but trivial, to help you to focus the business logic of your projects.

### Trivial

![.NET Core 3.1](./assets/badge_NET_Core_3_1.svg)
![.NET Standard 2.0](./assets/badge_NET_Standard_2_0.svg)

This library includes utilities and services for tasks, security, JSON, etc.

```sh
PM > Install-Package Trivial
```

[![NuGet package](https://img.shields.io/nuget/dt/Trivial)](https://www.nuget.org/packages/Trivial)

- [Task and retry policy](./tasks/)
- [Text (also including JSON and CSV)](./text/)
- [Network](./net/)
- [Security and authentication](./security/)
- [Data](./data/)
- [File and stream](./io/)
- [Maths and numerals](./maths/)
- [Reflection](./reflection/)
- [Geography](./geo/)

### Trivial.Console

![.NET Standard 2.0](./assets/badge_NET_Standard_2_0.svg)

The useful utilities for console application including command dispatcher, arguments parser and some rich UX CLI controls.

```sh
PM > Install-Package Trivial.Console
```

[![NuGet package](https://img.shields.io/nuget/dt/Trivial.Console?label=nuget+downloads)](https://www.nuget.org/packages/Trivial.Console)

- [Console utilities and components](./console/)
