# Color

Color adjustment, converter, parser and mixer.

In `Trivial.Drawing` [namespace](../) of `Trivial.dll` [library](../../).

## Adjustment

Adjust colors by following methods.

- White balance (lighten and darken)
- Toggle brightness (between light mode and dark mode)
- Opacity
- Saturation filter and grayscale
- Rotate hue
- With specific channel(s)
- Reverse

```csharp
var color = System.Drawing.Color.FromArgb(0xCC, 0x99, 0x33);

// Lighten.
ColorCalculator.Lighten(color, 0.1);

// Opacity.
ColorCalculator.Opacity(color, 0.9);

// Saturation filter
var color = ColorCalculator.Parse("hsl(318.413, 76.518%, 0.51568)");
ColorCalculator.Saturate(color, 0.2);

// Self-adaptation aturation filter
ColorCalculator.Saturate(color, RelativeSaturationLevels.High);
```

## Parser

Parse from a string by hex, RGBA, HSL or CMYK.

```csharp
var hex = ColorCalculator.Parse("#FFFF0000");
var rgba = ColorCalculator.Parse("cmyk(0, 0.83628, 0.25664, 0.11373, 0.8)");
var hsl = ColorCalculator.Parse("hsl(318.413, 76.518%, 0.51568)");
var cmyk = ColorCalculator.Parse("cmyk(0, 0.83628, 0.25664, 0.11373)");
```

## Converter

Convert to following color systems.

- HSL (hue-saturation-lightness)
- HSV (hue-saturation-value)
- HSI (hue-saturation-intensity)
- CMYK (cyan-magenta-yellow-black)
- CIE LAB (lightness and 2 chromaticities)
- CIE XYZ

```csharp
var color = Color.FromArgb(0xCC, 0x99, 0x33);
var (h, s, l) = ColorCalculator.ToHSL(color);
var (c, m, y, k) = ColorCalculator.ToCMYK(color);
var (l, a, b) = ColorCalculator.ToCIELAB(color);
```

Or convert from HSL or CMYK.

```csharp
var hsl = ColorCalculator.FromHSL(318.413, 76.518%, 0.51568);
var cmyk = ColorCalculator.FromCMYK(0, 0.83628, 0.25664, 0.11373);
```

## Mix

Overlay or mix 2 colors to result a new one.

```csharp
color = ColorCalculator.Overlay(
    Color.FromArgb(0.7, 240, 0, 0),
    Color.FromArgb(0, 240, 0));

color = ColorCalculator.Mix(
    ColorMixTypes.Normal,
    Color.FromArgb(240, 0, 0),
    Color.FromArgb(0, 240, 0));

color = ColorCalculator.Mix(
    ColorMixTypes.Lighten,
    Color.FromArgb(255, 192, 0),
    Color.FromArgb(0, 240, 64));

color = ColorCalculator.Mix(
    ColorMixTypes.Accent,
    Color.FromArgb(255, 192, 0),
    Color.FromArgb(0, 240, 64));
```

# Linear gradient

Create a specific number of color by a from color and an end color for linear gradient.

```csharp
var colors = ColorCalculator.LinearGradient(
    Color.FromArgb(0xCC, 0x99, 0x33),
    Color.FromArgb(0x33, 0x66, 0xCC),
    20
);
```
