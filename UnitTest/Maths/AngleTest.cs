using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths;

[TestClass]
public class AngleTest
{
    [TestMethod]
    public void TestAngleConstructionAndConversions()
    {
        var angle = new Angle(30, 15, 20);
        Assert.AreEqual(30, angle.Degree);
        Assert.AreEqual(15, angle.Arcminute);
        Assert.AreEqual(20f, angle.Arcsecond);
        Assert.AreEqual(30 + 15 / 60d + 20 / 3600d, angle.Degrees, 1e-6);
        Assert.AreEqual(angle.Degrees / 180 * Math.PI, angle.Radians, 1e-12);
        Assert.AreEqual(angle.Degrees / 180, angle.RadiansWithoutPi, 1e-12);
        Assert.AreEqual(angle.Degrees / 360, angle.Turns, 1e-12);
        Assert.AreEqual(angle.Degrees / 0.9, angle.Grads, 1e-12);

        var negative = new Angle(-30, 15, 20);
        Assert.IsTrue(negative.IsNegative);
        Assert.IsFalse(negative.Positive);
        Assert.AreEqual(-30 - 15 / 60d - 20 / 3600d, negative.Degrees, 1e-6);
        Assert.AreEqual(Math.Abs(negative.Degrees), negative.AbsDegrees, 1e-12);

        Assert.AreEqual(90, Angle.FromRadian(Math.PI / 2).Degrees, 1e-12);
        Assert.IsTrue(Angle.ZeroDegree.IsZero);
        Assert.IsFalse(angle.IsZero);
    }

    [TestMethod]
    public void TestAngleParsingArithmeticAndComparison()
    {
        var parsed = Angle.Parse("30°15'20\"");
        Assert.AreEqual(30 + 15 / 60d + 20 / 3600d, parsed.Degrees, 1e-6);
        Assert.AreEqual(12.5, Angle.Parse("12.5").Degrees, 1e-6);
        Assert.AreEqual(30 + 15 / 60d + 20 / 3600d, Angle.Parse("30;15;20").Degrees, 1e-6);

        var first = new Angle(30);
        var second = new Angle(15);
        Assert.AreEqual(45, (first + second).Degrees);
        Assert.AreEqual(15, (first - second).Degrees);
        Assert.AreEqual(-30, (-first).Degrees);
        Assert.AreEqual(60, (first * 2).Degrees);
        Assert.AreEqual(15, (first / 2).Degrees);
        Assert.IsTrue(first > second);
        Assert.IsTrue(first >= second);
        Assert.IsTrue(second < first);
        Assert.IsTrue(second <= first);
        Assert.IsTrue(first.Equals(30));
        Assert.IsTrue(new Angle.Model(30) > new Angle.Model(15));
        Assert.IsTrue(new Angle.Model(30) >= new Angle.Model(30));
        Assert.IsTrue(new Angle.Model(15) < new Angle.Model(30));
        Assert.IsTrue(new Angle.Model(30) <= new Angle.Model(30));
        Assert.AreEqual(31, first.AddDegrees(1).Degrees);
        Assert.AreEqual(31, first.AddArcminutes(60).Degrees);
        Assert.AreEqual(30 + 1 / 3600d, first.AddArcseconds(1).Degrees, 1e-6);
        Assert.AreEqual("30°", first.ToString());
    }

    [TestMethod]
    public void TestBoundaryOptions()
    {
        var cycleBoundary = new Angle.BoundaryOptions(360, false, Angle.RectifyModes.Cycle);
        Assert.AreEqual(10, new Angle(370, cycleBoundary).Degrees);
        Assert.AreEqual(350, new Angle(-10, cycleBoundary).Degrees);

        var strictBoundary = new Angle.BoundaryOptions(90, false, Angle.RectifyModes.None);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Angle(91, strictBoundary));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Angle(-1, strictBoundary));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Angle(0, new Angle.BoundaryOptions(0, false, Angle.RectifyModes.Cycle)));
    }

    [TestMethod]
    public void TestAngleModelMutationAndClone()
    {
        var model = new Angle.Model(30.5);
        Assert.AreEqual(30.5, model.Degrees, 1e-6);
        model.Arcminute = 45;
        Assert.AreEqual(30.75, model.Degrees, 1e-6);
        model.Degrees = 45;
        Assert.AreEqual(45, model.Degrees);
        model.Positive = false;
        Assert.AreEqual(-45, model.Degrees);
        Assert.IsTrue(model.IsNegative);

        var clone = model.Clone();
        Assert.AreNotSame(model, clone);
        Assert.AreEqual(model.Degrees, clone.Degrees);
        clone.Degrees = 90;
        Assert.AreEqual(-45, model.Degrees);
        Assert.AreEqual(90, clone.Degrees);

        var bounded = new Angle.Model(370, new Angle.BoundaryOptions(360, false, Angle.RectifyModes.Cycle));
        Assert.AreEqual(10, bounded.Degrees);
        Assert.AreEqual(90, Angle.RightModel.Degrees);
    }

    [TestMethod]
    public void TestAngleExtensionsAndJson()
    {
        IAngle acute = new Angle(45);
        IAngle right = Angle.Right;
        IAngle obtuse = new Angle(120);
        IAngle reflex = new Angle(270);
        Assert.IsTrue(acute.IsAcute());
        Assert.IsTrue(right.IsRight());
        Assert.IsTrue(acute.IsOblique());
        Assert.IsTrue(obtuse.IsObtuse());
        Assert.IsTrue(reflex.IsReflex());
        Assert.IsTrue(acute.LessThanRound());
        Assert.AreEqual("30°15'20\"", new Angle(30, 15, 20).ToAbsAngleString());
        Assert.AreEqual(Math.Sin(acute.Degrees), acute.Sin(), 1e-12);
        Assert.AreEqual(Math.Cos(acute.Degrees), acute.Cos(), 1e-12);

        var json = JsonSerializer.Serialize(new Angle(30.5));
        Assert.AreEqual("30.5", json);
        Assert.AreEqual(30.5, JsonSerializer.Deserialize<Angle>(json).Degrees, 1e-6);
        Assert.AreEqual(45, JsonSerializer.Deserialize<Angle>("\"45\"").Degrees, 1e-6);

        var modelJson = JsonSerializer.Serialize(new Angle.Model(12.25));
        Assert.AreEqual("12.25", modelJson);
        Assert.AreEqual(12.25, JsonSerializer.Deserialize<Angle.Model>(modelJson).Degrees, 1e-6);
        Assert.IsNull(JsonSerializer.Deserialize<Angle.Model>("null"));
        Assert.AreEqual("null", JsonSerializer.Serialize<Angle.Model>(null));
    }
}
