using System;
using Angle;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class BasisTest {
    private static void AssertConsistent(Basis basis, double angle, Turn turn) {
        var eps = Turn.OneTurnClockwise * 0.00001;
        Assert.IsTrue(basis.AngleToTurn(angle).Equals(turn, eps));
        Assert.IsTrue(Turn.FromAngle(angle, basis).Equals(turn, eps));
        Assert.AreEqual(basis.TurnToAngle(turn), angle, 0.00001);
        Assert.AreEqual(turn.GetAngle(basis), angle, 0.00001);
    }
    private static void AssertConsistent(Basis basis, double angle, Dir dir) {
        var eps = Turn.OneTurnClockwise * 0.00001;
        Assert.IsTrue(basis.AngleToDir(angle).Equals(dir, eps));
        Assert.IsTrue(Dir.FromAngle(angle, basis).Equals(dir, eps));
        Assert.AreEqual(basis.DirToUnsignedAngle(dir), angle, 0.00001);
        Assert.AreEqual(dir.GetUnsignedAngle(basis), angle, 0.00001);
        
        var u = Math.Abs(basis.UnitsPerCounterClockwiseTurn);
        var s = angle * 2 >= u ? angle - u : angle;
        var r = basis.DirToSignedAngle(dir);
        Assert.AreEqual(r, dir.GetSignedAngle(basis));
        if ((Math.Abs(s - u / 2) > 0.01 || Math.Abs(r + u / 2) > 0.01) && (Math.Abs(s + u / 2) > 0.01 || Math.Abs(r - u / 2) > 0.01)) {
            Assert.AreEqual(r, s, 0.00001);
        }
    }

    [TestMethod]
    public void NaturalAngles() {
        Assert.IsFalse(Basis.Natural.IsClockwisePositive);
        
        AssertConsistent(Basis.Natural, 0, Dir.AlongPositiveX);
        AssertConsistent(Basis.Natural, Math.PI / 2, Dir.AlongPositiveY);
        AssertConsistent(Basis.Natural, Math.PI, Dir.AlongNegativeX);
        AssertConsistent(Basis.Natural, 3 * Math.PI / 2, Dir.AlongNegativeY);

        AssertConsistent(Basis.Natural, Basis.RadiansPerRotation, Turn.OneTurnCounterClockwise);

        // GetAngle corresponds to GetNaturalAngle
        foreach (var e in new[] { Dir.AlongNegativeX, Dir.AlongNegativeY, Dir.AlongPositiveX, Dir.AlongPositiveY }) {
            Assert.IsTrue(e.UnsignedNaturalAngle == e.GetUnsignedAngle(Basis.Natural));
            Assert.IsTrue(e.SignedNaturalAngle == e.GetSignedAngle(Basis.Natural));
            Assert.IsTrue(e.UnsignedNaturalAngle == Basis.Natural.DirToUnsignedAngle(e));
            Assert.IsTrue(e.SignedNaturalAngle == Basis.Natural.DirToSignedAngle(e));
        }
        foreach (var e in new[] { Turn.OneDegreeClockwise, Turn.OneGradianClockwise, Turn.OneRadianClockwise, Turn.OneTurnClockwise, Turn.Zero }) {
            Assert.IsTrue(e.NaturalAngle == e.GetAngle(Basis.Natural));
            Assert.IsTrue(e.NaturalAngle == Basis.Natural.TurnToAngle(e));
            Assert.IsTrue((-e).NaturalAngle == (-e).GetAngle(Basis.Natural));
        }
    }
    [TestMethod]
    public void ClockAngles() {
        var eps = Turn.OneTurnClockwise * 0.0001;
        var clock = Basis.FromDirectionAndUnits(Dir.AlongPositiveY, unitsPerTurn: 12, isClockwisePositive: true);
        Assert.AreEqual(clock.CounterClockwiseRadiansPerUnit * -12, Basis.RadiansPerRotation, 0.0001);
        Assert.AreEqual(clock.UnitsPerCounterClockwiseTurn, -12, 0.0001);
        Assert.IsTrue(clock.IsClockwisePositive); 

        AssertConsistent(clock, 0, Dir.AlongPositiveY);
        AssertConsistent(clock, 3, Dir.AlongPositiveX);
        AssertConsistent(clock, 6, Dir.AlongNegativeY);
        AssertConsistent(clock, 9, Dir.AlongNegativeX);
        Assert.IsTrue(clock.AngleToDir(-3).Equals(Dir.AlongNegativeX, eps));
        Assert.IsTrue(clock.AngleToDir(12).Equals(Dir.AlongPositiveY, eps));

        AssertConsistent(clock, -3, Turn.OneTurnClockwise * -0.25);
        AssertConsistent(clock, 0, Turn.OneTurnClockwise * 0);
        AssertConsistent(clock, 1, Turn.OneTurnClockwise / 12);
        AssertConsistent(clock, 3, Turn.OneTurnClockwise * 0.25);
        AssertConsistent(clock, 6, Turn.OneTurnClockwise * 0.5);
        AssertConsistent(clock, 9, Turn.OneTurnClockwise * 0.75);
        AssertConsistent(clock, 12, Turn.OneTurnClockwise * 1);
    }
    [TestMethod]
    public void CounterclockwiseDegrees() {
        var eps = Turn.OneTurnClockwise * 0.0001;
        var deg = Basis.FromDirectionAndUnits(Dir.AlongPositiveY, unitsPerTurn: Basis.DegreesPerRotation, isClockwisePositive: false);
        Assert.AreEqual(deg.CounterClockwiseRadiansPerUnit * 360, Basis.RadiansPerRotation, 0.0001);
        Assert.AreEqual(deg.UnitsPerCounterClockwiseTurn, 360, 0.0001);
        Assert.IsFalse(deg.IsClockwisePositive); 

        AssertConsistent(deg, 0, Dir.AlongPositiveY);
        AssertConsistent(deg, 90, Dir.AlongNegativeX);
        AssertConsistent(deg, 180, Dir.AlongNegativeY);
        AssertConsistent(deg, 270, Dir.AlongPositiveX);
        Assert.IsTrue(deg.AngleToDir(-90).Equals(Dir.AlongPositiveX, eps));
        Assert.IsTrue(deg.AngleToDir(360).Equals(Dir.AlongPositiveY, eps));

        AssertConsistent(deg, -90, Turn.OneTurnCounterClockwise * -0.25);
        AssertConsistent(deg, 0, Turn.OneTurnCounterClockwise * 0);
        AssertConsistent(deg, 1, Turn.OneTurnCounterClockwise / 360);
        AssertConsistent(deg, 90, Turn.OneTurnCounterClockwise * 0.25);
        AssertConsistent(deg, 180, Turn.OneTurnCounterClockwise * 0.5);
        AssertConsistent(deg, 270, Turn.OneTurnCounterClockwise * 0.75);
        AssertConsistent(deg, 360, Turn.OneTurnCounterClockwise * 1);
    }
    [TestMethod]
    public void ToStringWorks() {
        Assert.IsTrue(!string.IsNullOrEmpty(Basis.Natural.ToString()));
        Assert.IsTrue(!string.IsNullOrEmpty(Basis.FromDirectionAndUnits(Dir.AlongPositiveY, unitsPerTurn: Basis.DegreesPerRotation, isClockwisePositive: false).ToString()));
        Assert.IsTrue(!string.IsNullOrEmpty(Basis.FromDirectionAndUnits(Dir.AlongPositiveY, unitsPerTurn: 12, isClockwisePositive: true).ToString()));
    }
}
