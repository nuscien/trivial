# Statistics

In `Trivial.Maths` [namespace](./maths).

<!-- Start -->

This directory contains statistical and prediction utilities exposed through the static partial class `StatisticalMethod`.

## Descriptive statistics

`StatisticalMethod` provides:

- `Mean` for integer, long, float, double, and decimal sequences.
- `Median` for comparable values, with overloads that accept a key selector and return a count.
- `Mode` to return all values tied for the highest frequency.
- `Variance`, `SampleVariance`, `StandardDeviation`, and `SampleDeviation` for integer, long, float, and double sequences. Population variance divides by the number of items; sample variance divides by one less than the number of items. Standard deviation is the square root of the corresponding variance.
- `Min` and `Max` to find an extremum, optionally returning its first index or a default value.

For null or empty inputs, many of these methods return a NaN, an empty result, or a supplied default, depending on the method. `Min` and `Max` overloads without a default throw for null or empty input. Check the specific overload's behavior when handling empty data.

## Score transforms

- `Softmax` and `Softmin` transform scores into normalized exponential weights.
- `Hardmax` and `Hardmin` produce values marking the maximum or minimum positions. Overloads with `equalDivisionMax` can divide the selected weight equally across tied extrema.

These transforms are available for integer, float, and double inputs, with collection, array, and (where provided) `ReadOnlySpan<T>` overloads. Float-specific variants include names ending in `F` where applicable.

## Differences

`Difference` returns the change between each item and its predecessor. It supports numeric sequences (`short`, `int`, `long`, `float`, `double`, and `decimal`), `DateTime` sequences (returning `TimeSpan`), and `TimeSpan` sequences. Enumerable overloads return an empty sequence for a null input and produce one fewer result than the input contains. Span overloads return arrays.

```csharp
using Trivial.Maths;

var changes = StatisticalMethod.Difference(new[] { 10, 13, 12 });
// 3, -1
```

## Prediction and sampling

- `NaiveBayes` predicts labels from training feature counts and class labels.
- `LinearRegression` fits a linear model to training data and predicts values for test rows. Both `float` and `double` overloads are available.
- `MetropolisHastings` generates a sample sequence using the Metropolis-Hastings algorithm. It accepts a `Random` instance for reproducibility and a burn-in count.
- `GaussianMixture` evaluates the implemented Gaussian probability-density function for a value, mean, and variance.

```csharp
var predictions = StatisticalMethod.LinearRegression(
	new double[,] { { 1 }, { 2 }, { 3 } },
	new[] { 2d, 4d, 6d },
	new double[,] { { 4 }, { 5 } });
```
