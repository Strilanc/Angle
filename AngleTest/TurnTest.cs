using Angle;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TurnTest {
    [TestMethod]
    public void Constants() {
        Assert.IsTrue(Turn.OneTurnClockwise == -Turn.OneTurnCounterClockwise);
        Assert.IsTrue(Turn.OneRadianClockwise == -Turn.OneRadianCounterClockwise);
        Assert.IsTrue(Turn.OneDegreeClockwise == -Turn.OneDegreeCounterClockwise);
        Assert.IsTrue(Turn.Zero == -Turn.Zero);
    }
    [TestMethod]
    public void EqualityAndOrdering() {
        var r = new[] {
            new[] {Turn.OneTurnClockwise, Turn.FromNaturalAngle(-Basis.RadiansPerRotation)},
            new[] {Turn.Zero},
            new[] {Turn.FromNaturalAngle(0.2), Turn.FromNaturalAngle(0.2)},
            new[] {Turn.OneTurnCounterClockwise, Turn.FromNaturalAngle(Basis.RadiansPerRotation)}
        };
        for (var i = 0; i < r.Length; i++) {
            var g1 = r[i];
            for (var j = 0; j < r.Length; j++) {
                var g2 = r[j];
                foreach (var e1 in g1) {
                    foreach (var e2 in g2) {
                        Assert.AreEqual(i < j, e1 < e2);
                        Assert.AreEqual(i > j, e1 > e2);
                        Assert.AreEqual(i <= j, e1 <= e2);
                        Assert.AreEqual(i >= j, e1 >= e2);
                        Assert.AreEqual(i != j, e1 != e2);
                        Assert.AreEqual(i == j, e1 == e2);
                        Assert.AreEqual(i == j, e1.Equals(e2));
                        Assert.AreEqual(i == j, e1.Equals(e2, Turn.FromNaturalAngle(0.001)));
                        Assert.AreEqual(i == j, e1.Equals((object)e2));
                        Assert.AreEqual(i.CompareTo(j), e1.CompareTo(e2));
                        Assert.IsTrue(i != j || e1.GetHashCode() == e2.GetHashCode());
                    }
                }
            }
        }
        Assert.IsTrue(!Turn.FromNaturalAngle(0).Equals(Turn.FromNaturalAngle(Basis.RadiansPerRotation + 0.0000001), Turn.FromNaturalAngle(0.001)));
        Assert.IsTrue(Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(4), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(2), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(3), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(3.5), Turn.FromNaturalAngle(1)));
        Assert.IsTrue(Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(3.5), Turn.FromNaturalAngle(0.5)));
        Assert.IsTrue(!Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(3.5), Turn.FromNaturalAngle(0.1)));
        Assert.IsTrue(!Turn.FromNaturalAngle(3).Equals(Turn.FromNaturalAngle(4), Turn.FromNaturalAngle(0.5)));
    }
    [TestMethod]
    public void NaturalAngle() {
        Assert.IsTrue(Turn.OneTurnClockwise.NaturalAngle == -Basis.RadiansPerRotation);
        Assert.IsTrue(Turn.OneTurnCounterClockwise.NaturalAngle == Basis.RadiansPerRotation);
        Assert.IsTrue(Turn.Zero.NaturalAngle == 0);
        Assert.IsTrue(Turn.FromNaturalAngle(5).NaturalAngle == 5);
        Assert.IsTrue(Turn.FromNaturalAngle(4).NaturalAngle == 4);
        Assert.IsTrue(Turn.FromNaturalAngle(-0.5).NaturalAngle == -0.5);
    }
    [TestMethod]
    public void Arithmetic() {
        Assert.IsTrue(Turn.FromNaturalAngle(1) + Turn.FromNaturalAngle(2) == Turn.FromNaturalAngle(3));
        Assert.IsTrue(Turn.FromNaturalAngle(1) - Turn.FromNaturalAngle(2) == Turn.FromNaturalAngle(-1));
        Assert.IsTrue(Turn.FromNaturalAngle(1) * 2 == Turn.FromNaturalAngle(2));
        Assert.IsTrue(2 * Turn.FromNaturalAngle(1) == Turn.FromNaturalAngle(2));
        Assert.IsTrue(Turn.FromNaturalAngle(1) / 2 == Turn.FromNaturalAngle(0.5));
        Assert.IsTrue(Turn.FromNaturalAngle(1) / Turn.FromNaturalAngle(2) == 0.5);
        Assert.IsTrue(Turn.FromNaturalAngle(3) % Turn.FromNaturalAngle(2) == Turn.FromNaturalAngle(1));
        Assert.IsTrue(-Turn.FromNaturalAngle(3) == Turn.FromNaturalAngle(-3));
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
        foreach (var e in new[] { Turn.OneTurnClockwise, Turn.OneTurnClockwise / 360, Turn.Zero }) {
            Assert.IsTrue(!string.IsNullOrEmpty(e.ToString()));
            Assert.IsTrue(!string.IsNullOrEmpty((e * 2).ToString()));
            Assert.IsTrue(!string.IsNullOrEmpty((e / 2).ToString()));
            Assert.IsTrue(!string.IsNullOrEmpty((-e).ToString()));
            Assert.IsTrue(!string.IsNullOrEmpty((-e * 2).ToString()));
        }
    }
}
