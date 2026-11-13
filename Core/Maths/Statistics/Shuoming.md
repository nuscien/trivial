# 统计

在 `Trivial.Maths` [命名空间](../) 中。

<!-- Start -->

本目录通过静态分部类 `StatisticalMethod` 提供统计与预测工具。

## 描述性统计

`StatisticalMethod` 提供以下方法：

- `Mean`：计算整数、长整数、单精度浮点数、双精度浮点数和十进制数序列的平均值。
- `Median`：计算可比较值的中位数；部分重载支持键选择器，并返回中位数项的数量。
- `Mode`：返回所有出现频率最高的值；如果多个值并列最高频，都会返回。
- `Variance`、`SampleVariance`、`StandardDeviation` 和 `SampleDeviation`：支持整数、长整数、单精度浮点数和双精度浮点数序列。总体方差除以数据项数量；样本方差除以数据项数量减一。标准差是对应方差的平方根。
- `Min` 和 `Max`：查找最小值或最大值；可选返回其首次出现的索引或在无结果时使用的默认值。

输入为 `null` 或空序列时，具体行为取决于方法：许多方法会返回 NaN、空结果或指定的默认值；未提供默认值的 `Min` 和 `Max` 重载则会对 `null` 或空输入抛出异常。处理空数据时，请确认所用重载的具体行为。

## 分数变换

- `Softmax` 和 `Softmin`：将分数转换为归一化的指数权重。
- `Hardmax` 和 `Hardmin`：标记最大值或最小值所在位置。带 `equalDivisionMax` 参数的重载可以在并列极值之间平均分配选中权重。

这些变换支持整数、单精度浮点数和双精度浮点数输入，并提供集合、数组以及（视具体方法而定）`ReadOnlySpan<T>` 重载。部分单精度浮点数版本的方法名以 `F` 结尾。

## 差分

`Difference` 返回序列中每项与其前一项的差值。支持数值序列（`short`、`int`、`long`、`float`、`double` 和 `decimal`）、`DateTime` 序列（结果为 `TimeSpan`），以及 `TimeSpan` 序列。对于可枚举序列，输入为 `null` 时返回空序列，输出项数比输入少一；`ReadOnlySpan<T>` 重载返回数组。

```csharp
using Trivial.Maths;

var changes = StatisticalMethod.Difference(new[] { 10, 13, 12 });
// 3, -1
```

## 预测与采样

- `NaiveBayes`：根据训练特征计数和类别标签预测类别。
- `LinearRegression`：根据训练数据拟合线性模型，并预测测试数据各行的值；提供 `float` 和 `double` 两种重载。
- `MetropolisHastings`：使用 Metropolis–Hastings 算法生成样本序列。可传入 `Random` 实例以便控制随机过程，并指定预热样本数量（burn-in）。
- `GaussianMixture`：根据给定的数值、均值和方差计算实现中的高斯概率密度值。

```csharp
var predictions = StatisticalMethod.LinearRegression(
	new double[,] { { 1 }, { 2 }, { 3 } },
	new[] { 2d, 4d, 6d },
	new double[,] { { 4 }, { 5 } });
```
