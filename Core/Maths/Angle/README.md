# Angle

In `Trivial.Maths` [namespace](./maths).

<!-- Start -->

This module provides a unified representation of angles, angle arithmetic, boundary handling, and commonly used extension methods. Degrees are the base unit; angles can also be represented as degrees, arcminutes, and arcseconds.

## Types

- `IAngle`: An angle interface exposing the `Degree`, `Arcminute`, and `Arcsecond` components, as well as the total number of `Degrees`. Use it in APIs that accept different angle types.
- `Angle`: An immutable value type suitable for representing and passing angles. Its properties are read-only; modifications and arithmetic produce a new `Angle`.
- `Angle.Model`: A mutable reference type. Update an angle through its `Degree`, `Arcminute`, `Arcsecond`, or `Degrees` property, and optionally provide boundary options when constructing it.

`Degree` is the integer degree component; `Arcminute` and `Arcsecond` are the arcminute and arcsecond components. 60 arcseconds make 1 arcminute, and 60 arcminutes make 1 degree. `Degrees` is the signed total in degrees. Both concrete types also provide values such as `AbsDegrees`, `Radians`, `RadiansWithoutPi`, `Turns`, and `Grads`. Radians are calculated as `Degrees / 180 * π`, while `RadiansWithoutPi` is `Degrees / 180`.

## Creation, Parsing, and Arithmetic

```csharp
using Trivial.Maths;

Angle angle = new Angle(30, 15, 20);      // 30°15′20″
Angle fromDegrees = new Angle(30.5);      // 30.5°
Angle fromRadians = Angle.FromRadian(Math.PI / 2); // 90°
Angle parsed = Angle.Parse("30°15'20\"");

var editable = new Angle.Model(30.5);
editable.Arcminute = 45;
```

You can also convert `int` or `double` values directly to `Angle` or `Angle.Model`. `Parse` accepts text representing a total number of degrees, as well as components separated by semicolons, commas, or degree/arcminute/arcsecond symbols. Both types support addition, subtraction, negation, multiplication and division by numeric values, and comparisons. These operations produce a new angle value. Conversions between `Angle` and `Angle.Model` are also available.

`Angle.ZeroDegree`, `Angle.OneDegree`, `Angle.Right`, `Angle.Straight`, and `Angle.Full` represent 0°, 1°, 90°, 180°, and 360°, respectively. Their mutable model counterparts are `Angle.ZeroDegreeModel`, `Angle.OneDegreeModel`, `Angle.RightModel`, `Angle.StraightModel`, and `Angle.FullModel`.

## Boundary Options

`Angle.BoundaryOptions(max, canBeNegative, rectify)` limits or rectifies an angle during construction. `max` must be greater than 0. `canBeNegative` determines whether negative angles are allowed. The rectification mode is specified by `Angle.RectifyModes`:

- `None`: Throws `ArgumentOutOfRangeException` when the value is out of range.
- `Cycle`: Wraps out-of-range values around the range.
- `Bounce`: Reflects values back when they cross a boundary.

For example, the following normalizes 370° using cyclic boundaries:

```csharp
var boundary = new Angle.BoundaryOptions(360, false, Angle.RectifyModes.Cycle);
var normalized = new Angle(370, boundary);
```

`Angle.Model` accepts the same options through its constructor. If no boundary options are provided, no range correction is performed.

## Trigonometric

`AngleExtensions` provides methods for `IAngle`, including angle classification (`IsAcute`, `IsRight`, `IsOblique`, `IsObtuse`, `IsReflex`, and `LessThanRound`), trigonometric functions (`Sin`, `Cos`, `Tan`, `Cot`, `Sec`, and `Csc`), and the `ToAbsAngleString` formatting method.

Note: The current trigonometric extensions pass `Degrees` directly to the trigonometric functions in `System.Math`, whose input unit is radians. To calculate a trigonometric function for an angle in degrees, convert it to radians first; for example, use `Math.Sin(angle.Radians)`.

## JSON

`Angle` and `Angle.Model` are serialized as a numeric total in degrees, not as degree/minute/second objects. Deserialization also accepts numeric strings. `Angle.Model` additionally supports JSON `null`, which is also written as `null` when serialized.

