# Angle

在 `Trivial.Maths` [命名空间](../) 中。

<!-- Start -->

本模块提供角度的统一表示、角度运算、边界处理和常用扩展方法。角度以度为基础单位，并支持度、角分、角秒之间的分解与组合。

## 类型

- `IAngle`：角度接口，提供 `Degree`、`Arcminute`、`Arcsecond` 三个分量，以及总度数 `Degrees`。适用于需要同时接受不同角度类型的 API。
- `Angle`：不可变值类型。适合表示和传递角度；属性为只读，修改或运算会产生新的 `Angle`。
- `Angle.Model`：可变引用类型。可通过 `Degree`、`Arcminute`、`Arcsecond` 或 `Degrees` 属性更新角度，并可在构造时附加边界选项。

`Degree` 是整数度分量，`Arcminute` 是角分，`Arcsecond` 是角秒；60 角秒为 1 角分，60 角分为 1 度。`Degrees` 是带符号的总度数。两种具体类型还提供 `AbsDegrees`、`Radians`、`RadiansWithoutPi`、`Turns` 和 `Grads` 等读数；其中弧度为 `Degrees / 180 * π`，`RadiansWithoutPi` 为 `Degrees / 180`。

## 创建、解析与运算

```csharp
using Trivial.Maths;

Angle angle = new Angle(30, 15, 20);      // 30°15′20″
Angle fromDegrees = new Angle(30.5);      // 30.5°
Angle fromRadians = Angle.FromRadian(Math.PI / 2); // 90°
Angle parsed = Angle.Parse("30°15'20\"");

var editable = new Angle.Model(30.5);
editable.Arcminute = 45;
```

也可以直接将 `int` 或 `double` 转换为 `Angle` 或 `Angle.Model`。`Parse` 接受总度数文本，以及以分号、逗号或度/角分/角秒符号分隔的分量文本。两种类型均支持加减、取负、乘除数值和比较；运算结果是新的角度值。`Angle` 与 `Angle.Model` 之间也提供转换。

`Angle.ZeroDegree`、`Angle.OneDegree`、`Angle.Right`、`Angle.Straight`、`Angle.Full` 分别表示 0°、1°、90°、180° 和 360°；对应的可变模型版本为 `Angle.ZeroDegreeModel`、`Angle.OneDegreeModel`、`Angle.RightModel`、`Angle.StraightModel` 和 `Angle.FullModel`。

## 边界选项

`Angle.BoundaryOptions(max, canBeNegative, rectify)` 用于构造时限制或修正角度。`max` 必须大于 0；`canBeNegative` 指定是否允许负角；修正模式是 `Angle.RectifyModes`：

- `None`：越界时抛出 `ArgumentOutOfRangeException`。
- `Cycle`：越界后按范围循环。
- `Bounce`：越过边界后反向折返。

例如，下面的 370° 会按周期边界修正：

```csharp
var boundary = new Angle.BoundaryOptions(360, false, Angle.RectifyModes.Cycle);
var normalized = new Angle(370, boundary);
```

`Angle.Model` 可通过构造函数接收相同的选项。未提供边界选项时，不执行范围修正。

## 三角函数

`AngleExtensions` 面向 `IAngle` 提供角度类型判断（`IsAcute`、`IsRight`、`IsOblique`、`IsObtuse`、`IsReflex`、`LessThanRound`）、三角函数（`Sin`、`Cos`、`Tan`、`Cot`、`Sec`、`Csc`）以及 `ToAbsAngleString` 格式化方法。

注意：当前三角函数扩展将 `Degrees` 直接传给 `System.Math` 的三角函数；而这些 .NET API 的输入单位是弧度。需要按角度求三角函数时，请先转换为弧度，例如 `Math.Sin(angle.Radians)`。

## JSON

`Angle` 和 `Angle.Model` 的 JSON 表示均为总度数数值，而不是度分秒对象。反序列化也接受数值字符串；`Angle.Model` 另外支持 JSON `null`，序列化时也会写为 `null`。

