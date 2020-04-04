# Arithmetic

You may use `System.Math` class for most scenarios you will use mathematics. The class `Arithmetic` is the supplementary.

In `Trivial.Maths` [namespace](./) of `Trivial.dll` [library](../).

## Prime

You can test if a number is a prime and get further prime number by following way.

```csharp
// Test whether a specific integer is a prime number.
var isPrime1 = Arithmetic.IsPrime(256); // false
var isPrime2 = Arithmetic.IsPrime(257); // true
var isPrime3 = await Arithmetic.IsPrimeAsync(2305843009213693951); // true

// Get the prime number after the given number.
var nextPrime = await Arithmetic.PreviousPrimeAsync(524287); // 524341

// Get the prime number before the given number.
var prevPrime = await Arithmetic.NextPrimeAsync(524287); // 524269
```

## Factorial

And you can also calculate the factorial for a specific integer.

```csharp
var a = Arithmetic.Factorial(20); // 2432902008176640000
var b = Arithmetic.FactorialApproximate(100); // 9.33262154439442e+157
```

## GCD & LCM

The greatest common divisor and the least common multiple.

```csharp
var c = Arithmetic.Gcd(192, 128); // 64
var d = Arithmetic.Lcm(192, 128); // 384
```
