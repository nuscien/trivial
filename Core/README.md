This library includes task utilities.

# Tasks

Just add following namespace to your code file to use.

```csharp
using Trivial.Tasks;
```

## Debounce

Maybe a handler will be asked to process several times in a short time but you just want to process once at the last time because the previous ones are obsolete, e.g. real-time search. You can use following method to do so.

```csharp
var task = HitTask.Debound(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

## Throttle

A handler to be frozen for a while after it has processed.

```csharp
var task = HitTask.Throttle(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(10000));

// Somewhere to raise.
task.ProcessAsync();
```

## Multiple

A handler to process for the specific times and it will be reset after a while.

```csharp
var task = HitTask.Multiple(() => {
    // Do something...
}, 10, null, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

## Times

A handler to process for the specific times only and it will be reset after a while, e.g. double click.

```csharp
var task = HitTask.Times(() => {
    // Do something...
}, 2, 2, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

# Mathematics

Just add following namespace to your code file to use.

```csharp
using Trivial.Maths;
```

## Arithmetic

There a lot of arithmetic functions.

```csharp
Arithmetic.IsPrime(2147483647); // True
Arithmetic.IsPrime(21474836479); // False
await Arithmetic.IsPrimeAsync(2305843009213693951); // False

Arithmetic.Factorial(10); // 3628800
```

## Numbers

You can get the number symbols as you want.

You can also get the number string in English words.

```csharp
EnglishNumber.Default.ToString(12345.67);
// twelve thousand three hundred and forty-five point six seven

EnglishNumber.Default.ToString(12345, true);
// one two three four five
```

And also for Chinese.

```csharp
ChineseNumber.Simplified.ToString(12345.67);
// 一万两千三百四十五点六七

ChineseNumber.Simplified.ToString(12345, true);
// 一二三四五

ChineseNumber.SimplifiedUppercase.ToString(12345);
// 壹萬贰仟叄佰肆拾伍

```

# Security

Just add following namespace to your code file to use.

```csharp
using Trivial.Security;
```
