using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;

namespace Trivial.Maths;

/// <summary>
/// The numeral bitmap collection of Beacon in the Galaxy.
/// </summary>
public static class BitgNumeralBitmap
{
    /// <summary>
    /// The bitmap width.
    /// </summary>
    public const int WIDTH = 5;

    /// <summary>
    /// The bitmap height.
    /// </summary>
    public const int HEIGHT = 7;

    /// <summary>
    /// The radix.
    /// </summary>
    public const int RADIX = 10;

    /// <summary>
    /// Gets the boolean array of zero.
    /// </summary>
    public readonly static bool[] Zero = new[] { false, false, true, false, false, false, false, true, false, false, true, true, true, true, true, true, false, false,false, false, true, true, true, true, true, true, false, false, true, false, false, false, false, true, false, false };

    /// <summary>
    /// Gets the boolean array of one.
    /// </summary>
    public readonly static bool[] One = new[] { true, true, true, true, true, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false };

    /// <summary>
    /// Gets the boolean array of two.
    /// </summary>
    public readonly static bool[] Two = new[] { true, true, true, false, false, false, false, true, false, false, true, false, true, false, false, true, false, false ,false, false, true, false, true, true, true, true, false, false, false, false, true, true, true, false, false };

    /// <summary>
    /// Gets the boolean array of three.
    /// </summary>
    public readonly static bool[] Three = new[] { true, true, true, true, true, true, false, false, false, false, true, true, true, true, true, true, false, false, false, false, true, false, true, true, true, true, false, false, false, false, true, false, false, false, false};

    /// <summary>
    /// Gets the boolean array of four.
    /// </summary>
    public readonly static bool[] Four = new[] { true, false, true, true, true, true, false, false, false, false, true, true, true, false, false, true, false, false, false, false, true, true, true, false, false, true, false, false, false, false, true, true, true, false, false };

    /// <summary>
    /// Gets the boolean array of five.
    /// </summary>
    public readonly static bool[] Five = new[] { true, true, true, false, true, true, true, true, false, true, true, false, true, false, true, true, false, false, false, false, true, true, true, false, false, false, false, true, false, false, true, true, true, true, true };

    /// <summary>
    /// Gets the boolean array of six.
    /// </summary>
    public readonly static bool[] Six = new[] { false, false, false, false, true, false, true, false, true, true, true, true, true, false, true, false, false, true, false, false, true, true, true, false, false, true, false, true, false, false, true, true, true, false, false };

    /// <summary>
    /// Gets the boolean array of seven.
    /// </summary>
    public readonly static bool[] Seven = new[] { true, true, true, false, true, false, false, true, true, true, false, false, true, false, true, false, true, false, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false };

    /// <summary>
    /// Gets the boolean array of eight.
    /// </summary>
    public readonly static bool[] Eight = new[] { true, false, true, true, true, true, true, false, false, false, true, true, true, false, false, true, false, true, false, false, true, true, true, true, true, true, false, true, false, false, true, true, true, true, true };

    /// <summary>
    /// Gets the boolean array of nine.
    /// </summary>
    public readonly static bool[] Nine = new[] { false, false, false, false, true, false, true, false, false, true, true, false, true, false, true, true, false, true, false, true, true, true, true, true, true, false, false, true, false, false, true, true, true, false, false };

    /// <summary>
    /// Gets the boolean array of equal sign.
    /// </summary>
    public readonly static bool[] EqualSign = new[] { false, false, true, true, true, false, false, true, false, false, true, true, true, true, true, false, false, true, false, false, true, true, true, true, true, false, false, true, false, false, true, true, true, false, false };

    /// <summary>
    /// Gets the boolean array of negative sign.
    /// </summary>
    public readonly static bool[] NegativeSign = new[] { false, false, true, true, true, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true, true, true };

    /// <summary>
    /// Gets the boolean array of plus sign.
    /// </summary>
    public readonly static bool[] PlusSign = new[] { true, false, true, true, true, true, false, false, false, false, true, false, true, false, false, true, true, true, true, false, true, false, true, false, false, true, false, false, false, false, true, false, true, false, false };

    /// <summary>
    /// Gets the boolean array of equal sign.
    /// </summary>
    public readonly static bool[] MinusSign = new[] { true, true, true, true, true, false, true, false, true, false, true, false, false, false, false, true, false, false, false, false, true, true, true, true, true, true, false, false, false, false, true, false, false, false, false };

    /// <summary>
    /// Gets the boolean array of dot.
    /// </summary>
    public readonly static bool[] Dot = new[] { false, false, true, true, true, false, false, false, false, true, false, false, false, false, true, false, true, false, false, false, true, false, false, false, false, true, false, false, false, false, true, true, true, false, false };
}
