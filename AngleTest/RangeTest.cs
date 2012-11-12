using Angle;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

[TestClass]
public class RangeTest {
    [TestMethod]
    public void Constants() {
        var eps = Turn.OneTurnClockwise*0.00001;
        var pp = Dir.FromVector(1, 1);
        var pn = Dir.FromVector(1, -1);
        var np = Dir.FromVector(-1, 1);
        var nn = Dir.FromVector(-1, -1);
        var px = Dir.AlongPositiveX;
        var py = Dir.AlongPositiveY;
        var nx = Dir.AlongNegativeX;
        var ny = Dir.AlongNegativeY;

        Action<Range, Dir?, Dir?, Dir[]> check = (range, cc, c, contents) => {
            foreach (var e in new[] {pp, pn, np, nn, px, py, nx, ny}) {
                Assert.IsTrue(range.Contains(e) == contents.Contains(e));
            }
            if (cc != null && c != null) {
                Assert.IsTrue(range.CounterClockwiseSide.Equals(cc.Value));
                Assert.IsTrue(range.Side(clockwiseSide: false).Equals(cc.Value));
                Assert.IsTrue(range.ClockwiseSide.Equals(c.Value));
                Assert.IsTrue(range.Side(clockwiseSide: true).Equals(c.Value));
                Assert.IsTrue(range.Span == (cc.Value - c.Value).MinimumCongruentCounterClockwiseTurn());
                Assert.IsTrue(range.Center.Equals(c.Value + (cc.Value - c.Value).MinimumCongruentCounterClockwiseTurn() / 2, eps));
            }
        };
        check(Range.AllDirections, null, null, new[] { pp, pn, np, nn, px, py, nx, ny });
        check(Range.PositiveX, py, ny, new[] { pp, pn, py, ny, px });
        check(Range.PositiveY, nx, px, new[] { pp, np, py, nx, px });
        check(Range.PositiveXPositiveY, py, px, new[] { pp, px, py });
        check(Range.PositiveXNegativeY, px, ny, new[] { pn, px, ny });
        check(Range.NegativeXPositiveY, nx, py, new[] { np, nx, py });
        check(Range.NegativeXNegativeY, ny, nx, new[] { nn, nx, ny });
    }
    [TestMethod]
    public void Equality() {
        var r = new[] {
            new[] {Range.NegativeX, Range.NegativeX},
            new[] {Range.NegativeY},
            new[] {Range.NegativeXPositiveY},
            new[] {Range.NegativeXNegativeY},
            new[] {Range.PositiveX},
            new[] {Range.FromStartToFinish(Dir.AlongPositiveX, Dir.AlongNegativeY, moveClockwise: false)},
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
        var dpx = Range.FromCenterAndMaxDeviation(Dir.AlongPositiveX, Turn.OneDegreeClockwise);
        var dpx2 = Range.FromCenterAndMaxDeviation(Dir.AlongPositiveX+Turn.OneDegreeClockwise*1.001, Turn.OneDegreeClockwise*1.001);
        Assert.IsTrue(dpx.Equals(dpx2, Turn.OneTurnClockwise * 0.01));
        Assert.IsTrue(dpx.Equals(dpx, Turn.Zero));
        Assert.IsTrue(!dpx.Equals(dpx2, Turn.Zero));
    }
    [TestMethod]
    public void FromCenterAndMaxDeviation() {
        Assert.IsTrue(Range.FromCenterAndMaxDeviation(Dir.AlongPositiveY, Turn.OneTurnClockwise * 0.25).Equals(Range.PositiveY));

        var r1 = Range.FromCenterAndMaxDeviation(Dir.AlongPositiveY, Turn.OneDegreeClockwise);
        var r2 = Range.FromCenterAndMaxDeviation(Dir.AlongPositiveY, -Turn.OneDegreeClockwise);
        Assert.IsTrue(r1.Equals(r2));
        Assert.IsTrue(!r1.Contains(Dir.AlongNegativeY));
        Assert.IsTrue(!r1.Contains(Dir.AlongPositiveY - Turn.OneDegreeClockwise * 1.0001));
        Assert.IsTrue(r1.Contains(Dir.AlongPositiveY - Turn.OneDegreeClockwise * 0.9999));
        Assert.IsTrue(r1.Contains(Dir.AlongPositiveY));
        Assert.IsTrue(r1.Contains(Dir.AlongPositiveY + Turn.OneDegreeClockwise * 0.9999));
        Assert.IsTrue(!r1.Contains(Dir.AlongPositiveY + Turn.OneDegreeClockwise * 1.0001));

        var r0 = Range.FromCenterAndMaxDeviation(Dir.AlongNegativeY, Turn.Zero);
        Assert.IsTrue(!r0.Contains(Dir.AlongPositiveY));
        Assert.IsTrue(r0.Contains(Dir.AlongNegativeY));
        Assert.IsTrue(!r0.Contains(Dir.AlongNegativeY + Turn.OneTurnClockwise * 1.0001));
        Assert.IsTrue(!r0.Contains(Dir.AlongNegativeY - Turn.OneTurnClockwise * 1.0001));
    }
    [TestMethod]
    public void FromStartToFinish() {
        Assert.IsTrue(Range.FromStartToFinish(Dir.AlongPositiveX, Dir.AlongNegativeX, moveClockwise: true).Equals(Range.NegativeY));
        Assert.IsTrue(Range.FromStartToFinish(Dir.AlongPositiveX, Dir.AlongNegativeX, moveClockwise: false).Equals(Range.PositiveY));

        var r0 = Range.FromStartToFinish(Dir.AlongNegativeY, Dir.AlongNegativeY, true);
        Assert.IsTrue(!r0.Contains(Dir.AlongPositiveY));
        Assert.IsTrue(r0.Contains(Dir.AlongNegativeY));
        Assert.IsTrue(!r0.Contains(Dir.AlongNegativeY + Turn.OneTurnClockwise * 1.0001));
        Assert.IsTrue(!r0.Contains(Dir.AlongNegativeY - Turn.OneTurnClockwise * 1.0001));

        var r1 = Range.FromStartToFinish(Dir.AlongPositiveX, Dir.AlongPositiveX + Turn.OneDegreeClockwise, moveClockwise: true);
        var r2 = Range.FromStartToFinish(Dir.AlongPositiveX + Turn.OneDegreeClockwise, Dir.AlongPositiveX, moveClockwise: false);
        Assert.IsTrue(r1.Equals(r2));
        Assert.IsTrue(!r1.Contains(Dir.AlongPositiveX - Turn.OneDegreeClockwise * 0.001));
        Assert.IsTrue(r1.Contains(Dir.AlongPositiveX + Turn.OneDegreeClockwise * 0.001));
        Assert.IsTrue(r1.Contains(Dir.AlongPositiveX + Turn.OneDegreeClockwise * 0.999));
        Assert.IsTrue(!r1.Contains(Dir.AlongPositiveX + Turn.OneDegreeClockwise * 1.001));

        var r3 = Range.FromStartToFinish(Dir.AlongPositiveX, Dir.AlongPositiveX + Turn.OneDegreeClockwise, moveClockwise: false);
        Assert.IsTrue(!r1.Equals(r3));
        Assert.IsTrue(r1.Inverse().Equals(r3, Turn.OneTurnClockwise * 0.00001));
    }
    [TestMethod]
    public void FromStartToFinishInInBasis() {
        var eps = Turn.OneTurnClockwise * 0.000001;
        var clock = Basis.FromDirectionAndUnits(Dir.AlongPositiveY, 12, true);
        Assert.IsTrue(Range.FromStartToFinishIncreasingInBasis(Dir.AlongPositiveX, Dir.AlongNegativeX, Basis.Natural).Equals(Range.PositiveY, eps));
        Assert.IsTrue(Range.FromStartToFinishIncreasingInBasis(Dir.AlongPositiveX, Dir.AlongNegativeX, clock).Equals(Range.NegativeY, eps));
        Assert.IsTrue(Range.FromStartToFinishDecreasingInBasis(Dir.AlongPositiveX, Dir.AlongNegativeX, Basis.Natural).Equals(Range.NegativeY, eps));
        Assert.IsTrue(Range.FromStartToFinishDecreasingInBasis(Dir.AlongPositiveX, Dir.AlongNegativeX, clock).Equals(Range.PositiveY, eps));

        Assert.IsTrue(Range.FromStartToFinishIncreasingInBasis(0, Math.PI, Basis.Natural).Equals(Range.PositiveY, eps));
        Assert.IsTrue(Range.FromStartToFinishIncreasingInBasis(3, 9, clock).Equals(Range.NegativeY, eps));
        Assert.IsTrue(Range.FromStartToFinishDecreasingInBasis(0, Math.PI, Basis.Natural).Equals(Range.NegativeY, eps));
        Assert.IsTrue(Range.FromStartToFinishDecreasingInBasis(3, 9, clock).Equals(Range.PositiveY, eps));
    }
    [TestMethod]
    public void Arithmetic() {
        Assert.IsTrue((Range.PositiveX + Turn.OneTurnClockwise * 0.5).Equals(Range.NegativeX));
        Assert.IsTrue((Range.PositiveX - Turn.OneTurnClockwise * 0.5).Equals(Range.NegativeX));
        Assert.IsTrue((Range.PositiveX + Turn.OneTurnClockwise * 0.25).Equals(Range.NegativeY));
        Assert.IsTrue((Range.PositiveX + Turn.OneTurnClockwise * 0.25).Equals(Range.NegativeY));
        Assert.IsTrue((Range.PositiveXPositiveY - Turn.OneTurnClockwise * 0.25).Equals(Range.NegativeXPositiveY));
    }
    [TestMethod]
    public void Inverse() {
        Assert.IsTrue(Range.PositiveX.Inverse().Equals(Range.NegativeX));
        Assert.IsTrue(Range.PositiveY.Inverse().Equals(Range.NegativeY));
    }
    [TestMethod]
    public void Clamp() {
        Action<Range, Dir, Dir> check = (range, dir, res) => {
            Assert.IsTrue(range.Clamp(dir).Equals(res));
            Assert.IsTrue(dir.ClampedInside(range).Equals(res));
        };
        check(Range.PositiveXPositiveY, Dir.AlongPositiveX, Dir.AlongPositiveX);
        check(Range.PositiveXPositiveY, Dir.AlongPositiveY, Dir.AlongPositiveY);
        check(Range.PositiveXPositiveY, Dir.AlongNegativeX, Dir.AlongPositiveY);
        check(Range.PositiveXPositiveY, Dir.AlongNegativeY, Dir.AlongPositiveX);
    }
    [TestMethod]
    public void ToStringWorks() {
        foreach (var e in new[] { Range.NegativeX, Range.PositiveXPositiveY, Range.AllDirections, Range.FromStartToFinish(Dir.AlongPositiveX, Dir.AlongPositiveX, true) }) {
            Assert.IsTrue(!string.IsNullOrEmpty(e.ToString()));
            Assert.IsTrue(!string.IsNullOrEmpty((e + Turn.OneDegreeClockwise).ToString()));
        }
    }
}
