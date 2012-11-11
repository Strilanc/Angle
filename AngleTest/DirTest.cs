using Angle;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DirTest {
    [TestMethod]
    public void Constants() {
        Assert.AreEqual(Dir.AlongPositiveX.UnitX, 1, 0.0001);
        Assert.AreEqual(Dir.AlongPositiveX.UnitY, 0, 0.0001);
        Assert.AreEqual(Dir.AlongPositiveY.UnitX, 0, 0.0001);
        Assert.AreEqual(Dir.AlongPositiveY.UnitY, 1, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeX.UnitX, -1, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeX.UnitY, 0, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeY.UnitX, 0, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeY.UnitY, -1, 0.0001);
        
        Assert.AreEqual(Dir.AlongPositiveX.NaturalAngle, 0, 0.0001);
        Assert.AreEqual(Dir.AlongPositiveY.NaturalAngle, Math.PI/2, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeX.NaturalAngle, Math.PI, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeY.NaturalAngle, 3*Math.PI/2, 0.0001);
    }
    [TestMethod]
    public void Equality() {
        var r = new[] {
            new[] {Dir.AlongNegativeX, Dir.AlongNegativeX, Dir.FromNaturalAngle(Basis.RadiansPerRotation/2)},
            new[] {Dir.AlongNegativeY, Dir.AlongNegativeY},
            new[] {Dir.AlongPositiveX, Dir.AlongPositiveX, Dir.FromNaturalAngle(Basis.RadiansPerRotation)},
            new[] {Dir.AlongPositiveY, Dir.AlongPositiveY},
            new[] {Dir.FromNaturalAngle(0.5)}
        };
        for (var i = 0; i < r.Length; i++) {
            var g1 = r[i];
            for (var j = 0; j < r.Length; j++) {
                var g2 = r[j];
                foreach (var e1 in g1) {
                    foreach (var e2 in g2) {
                        Assert.AreEqual(i != j, e1 != e2);
                        Assert.AreEqual(i == j, e1 == e2);
                        Assert.AreEqual(i == j, e1.Equals(e2));
                        Assert.AreEqual(i == j, e1.Equals(e2, Turn.FromNaturalAngle(0.001)));
                        Assert.AreEqual(i == j, e1.Equals((object)e2));
                        Assert.IsTrue(i != j || e1.GetHashCode() == e2.GetHashCode());
                    }
                }
            }
        }
        Assert.IsTrue(Dir.FromNaturalAngle(0).Equals(Dir.FromNaturalAngle(Basis.RadiansPerRotation + 0.0000001), Turn.FromNaturalAngle(0.001)));
        Assert.IsTrue(Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(3), Turn.Zero));
        Assert.IsTrue(Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(4), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(2), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(3), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(3.5), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(3.5), Turn.FromNaturalAngle(0.5)));
        Assert.IsTrue(!Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(3.5), Turn.FromNaturalAngle(0.1)));
        Assert.IsTrue(!Dir.FromNaturalAngle(3).Equals(Dir.FromNaturalAngle(4), Turn.FromNaturalAngle(0.5)));
    }
    [TestMethod]
    public void NaturalAngle() {
        Assert.AreEqual(Dir.AlongNegativeX.NaturalAngle, Basis.RadiansPerRotation/2, 0.0001);
        Assert.AreEqual(Dir.AlongNegativeY.NaturalAngle, Basis.RadiansPerRotation * 3 / 4, 0.0001);
        Assert.AreEqual(Dir.AlongPositiveX.NaturalAngle, 0, 0.0001);
        Assert.AreEqual(Dir.AlongPositiveY.NaturalAngle, Basis.RadiansPerRotation / 4, 0.0001);
        Assert.AreEqual(Dir.FromNaturalAngle(7).NaturalAngle, 7 - Basis.RadiansPerRotation, 0.0001);
        Assert.AreEqual(Dir.FromNaturalAngle(4).NaturalAngle, 4, 0.0001);
        Assert.AreEqual(Dir.FromNaturalAngle(-1).NaturalAngle, -1 + Basis.RadiansPerRotation, 0.0001);
    }
    [TestMethod]
    public void Arithmetic() {
        Assert.IsTrue(Dir.FromNaturalAngle(1) + Turn.FromNaturalAngle(2) == Dir.FromNaturalAngle(3));
        Assert.IsTrue(Dir.FromNaturalAngle(1) - Turn.FromNaturalAngle(2) == Dir.FromNaturalAngle(-1));
        Assert.IsTrue(Dir.FromNaturalAngle(1) - Dir.FromNaturalAngle(2) == Turn.FromNaturalAngle(-1));
    }
    private static void AssertThrows<E>(Action action) where E : Exception {
        try {
            action();
            Assert.Fail("Expected an exception of type " + typeof(E).FullName);
        } catch (E) {
        }
    }
    [TestMethod]
    public void Vector() {
        // cardinal directions
        Assert.AreEqual(Dir.FromVector(1, 0).NaturalAngle, 0, 0.0001);
        Assert.AreEqual(Dir.FromVector(0, 1).NaturalAngle, Math.PI / 2, 0.0001);
        Assert.AreEqual(Dir.FromVector(-1, 0).NaturalAngle, Math.PI, 0.0001);
        Assert.AreEqual(Dir.FromVector(0, -1).NaturalAngle, 3 * Math.PI / 2, 0.0001);
        Assert.AreEqual(Dir.FromVector(1, 1).NaturalAngle, Math.PI / 4, 0.0001);

        // single infinities are fine
        Assert.AreEqual(Dir.FromVector(double.PositiveInfinity, 0).NaturalAngle, 0, 0.0001);
        Assert.AreEqual(Dir.FromVector(double.PositiveInfinity, 0).NaturalAngle, 0, 0.0001);
        Assert.AreEqual(Dir.FromVector(double.NegativeInfinity, 1).NaturalAngle, Math.PI, 0.0001);
        Assert.AreEqual(Dir.FromVector(double.NegativeInfinity, 1).NaturalAngle, Math.PI, 0.0001);
        Assert.AreEqual(Dir.FromVector(0, double.PositiveInfinity).NaturalAngle, Math.PI / 2, 0.0001);
        Assert.AreEqual(Dir.FromVector(0, double.PositiveInfinity).NaturalAngle, Math.PI / 2, 0.0001);
        Assert.AreEqual(Dir.FromVector(1, double.NegativeInfinity).NaturalAngle, 3*Math.PI / 2, 0.0001);
        Assert.AreEqual(Dir.FromVector(1, double.NegativeInfinity).NaturalAngle, 3 * Math.PI / 2, 0.0001);

        // invalid cases
        AssertThrows<ArgumentException>(() => Dir.FromVector(0, 0));
        AssertThrows<ArgumentException>(() => Dir.FromVector(double.NaN, 0));
        AssertThrows<ArgumentException>(() => Dir.FromVector(0, double.NaN));
        AssertThrows<ArgumentException>(() => Dir.FromVector(double.NegativeInfinity, double.PositiveInfinity));
        AssertThrows<ArgumentException>(() => Dir.FromVector(double.PositiveInfinity, double.NegativeInfinity));
        AssertThrows<ArgumentException>(() => Dir.FromVector(double.NegativeInfinity, double.PositiveInfinity));
        AssertThrows<ArgumentException>(() => Dir.FromVector(double.PositiveInfinity, double.NegativeInfinity));
    }
    [TestMethod]
    public void SmallestEquivalent() {
        Assert.IsTrue(Turn.Zero.Equals(Turn.OneTurnClockwise.SmallestClockwiseEquivalent()));
        Assert.IsTrue(Turn.Zero.Equals(Turn.OneTurnClockwise.SmallestCounterClockwiseEquivalent()));
        Assert.IsTrue(Turn.Zero.Equals(Turn.OneTurnClockwise.SmallestSignedEquivalent()));

        Assert.IsTrue(Turn.Zero.Equals(Turn.OneTurnCounterClockwise.SmallestClockwiseEquivalent()));
        Assert.IsTrue(Turn.Zero.Equals(Turn.OneTurnCounterClockwise.SmallestCounterClockwiseEquivalent()));
        Assert.IsTrue(Turn.Zero.Equals(Turn.OneTurnCounterClockwise.SmallestSignedEquivalent()));

        var eps = Turn.FromNaturalAngle(0.0001);
        Assert.IsFalse(Turn.FromNaturalAngle(1).Equals(Turn.FromNaturalAngle(1 + Basis.RadiansPerRotation).SmallestClockwiseEquivalent(), eps));
        Assert.IsTrue(Turn.FromNaturalAngle(1).Equals(Turn.FromNaturalAngle(1 + Basis.RadiansPerRotation).SmallestCounterClockwiseEquivalent(), eps));
        Assert.IsTrue(Turn.FromNaturalAngle(1).Equals(Turn.FromNaturalAngle(1 + Basis.RadiansPerRotation).SmallestSignedEquivalent(), eps));

        Assert.IsTrue(Turn.FromNaturalAngle(-1).Equals(Turn.FromNaturalAngle(-1 + Basis.RadiansPerRotation).SmallestClockwiseEquivalent(), eps));
        Assert.IsFalse(Turn.FromNaturalAngle(-1).Equals(Turn.FromNaturalAngle(-1 + Basis.RadiansPerRotation).SmallestCounterClockwiseEquivalent(), eps));
        Assert.IsTrue(Turn.FromNaturalAngle(-1).Equals(Turn.FromNaturalAngle(-1 + Basis.RadiansPerRotation).SmallestSignedEquivalent(), eps));
    }
    [TestMethod]
    public void Abs() {
        Assert.IsTrue(Turn.Zero.Abs() == Turn.Zero);
        Assert.IsTrue(Turn.OneTurnCounterClockwise.Abs() == Turn.OneTurnCounterClockwise);
        Assert.IsTrue(Turn.OneTurnClockwise.Abs() == Turn.OneTurnCounterClockwise);
    }
    [TestMethod]
    public void Sign() {
        Assert.IsTrue(Turn.Zero.Sign() == 0);
        Assert.IsTrue(Turn.OneTurnCounterClockwise.Sign() == +1);
        Assert.IsTrue(Turn.OneTurnClockwise.Sign() == -1);
    }
    [TestMethod]
    public void ToStringWorks() {
        foreach (var e in new[] { Dir.AlongNegativeX, Dir.AlongNegativeY, Dir.AlongPositiveX, Dir.AlongPositiveY, Dir.FromNaturalAngle(0.04), Dir.FromNaturalAngle(500) }) {
            Assert.IsTrue(!string.IsNullOrEmpty(e.ToString()));
            Assert.IsTrue(!string.IsNullOrEmpty((e+Turn.OneDegreeClockwise).ToString()));
        }
    }
}
