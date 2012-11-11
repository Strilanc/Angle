using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Angle {
    /// <summary>
    /// A rotation.
    /// The 'delta' in the affine space of angles.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Turn : IEquatable<Turn>, IComparable<Turn> {
        ///<summary>The identity rotation.</summary>
        public static readonly Turn Zero = default(Turn);
        ///<summary>1 complete counter clockwise rotation.</summary>
        public static readonly Turn OneTurnCounterClockwise = new Turn(Basis.RadiansPerRotation);
        ///<summary>1/(2*pi)'th of a counter clockwise rotation.</summary>
        public static readonly Turn OneRadianCounterClockwise = new Turn(1);
        ///<summary>1/360'th of a counter clockwise rotation.</summary>
        public static readonly Turn OneDegreeCounterClockwise = new Turn(Basis.RadiansPerRotation / Basis.DegreesPerRotation);
        ///<summary>1/400'th of a counter clockwise rotation.</summary>
        public static readonly Turn OneGradianCounterClockwise = new Turn(Basis.RadiansPerRotation / Basis.GradiansPerRotation);

        ///<summary>1 complete clockwise rotation.</summary>
        public static readonly Turn OneTurnClockwise = -OneTurnCounterClockwise;
        ///<summary>1/(2*pi)'th of a clockwise rotation.</summary>
        public static readonly Turn OneRadianClockwise = -OneRadianCounterClockwise;
        ///<summary>1/360'th of a clockwise rotation.</summary>
        public static readonly Turn OneDegreeClockwise = -OneDegreeCounterClockwise;
        ///<summary>1/400'th of a clockwise rotation.</summary>
        public static readonly Turn OneGradianClockwise = -OneGradianCounterClockwise;

        private readonly double _radians;
        private Turn(double radians) {
            if (double.IsNaN(radians)) throw new ArgumentOutOfRangeException("radians", "radians is NaN");
            if (double.IsInfinity(radians)) throw new ArgumentOutOfRangeException("radians", "radians is infinite");
            this._radians = radians;
        }
        ///<summary>Returns the turn corresponding to a counter-clockwise rotation by the given angle in radians.</summary>
        public static Turn FromNaturalAngle(double angle) {
            return new Turn(angle);
        }
        ///<summary>Returns the turn corresponding to a rotation by the given angle in the given basis.</summary>
        public static Turn FromAngle(double angle, Basis basis) {
            return FromNaturalAngle(angle * basis.CounterClockwiseRadiansPerUnit);
        }

        ///<summary>Returns the angle corresponding to a rotation by the given turn, in counter-clockwise radians.</summary>
        public double NaturalAngle { get { return _radians; } }
        ///<summary>Returns the angle corresponding to a rotation by the given turn, in the units of the given basis.</summary>
        public double GetAngle(Basis basis) {
            return _radians/basis.CounterClockwiseRadiansPerUnit;
        }
        
        ///<summary>The smallest turn (by magnitude) with an equivalent effect on directions.</summary>
        [Pure]
        public Turn SmallestSignedEquivalent() {
            var r = this % OneTurnCounterClockwise;
            if (r*2 <= -OneTurnCounterClockwise) r += OneTurnCounterClockwise;
            if (r*2 > OneTurnCounterClockwise) r -= OneTurnCounterClockwise;
            return r;
        }
        ///<summary>The smallest counter-clockwise turn with an equivalent effect on directions.</summary>
        [Pure]
        public Turn SmallestCounterClockwiseEquivalent() {
            var r = this % OneTurnCounterClockwise;
            if (r < Zero) r += OneTurnCounterClockwise;
            return r;
        }
        ///<summary>The smallest clockwise turn with an equivalent effect on directions.</summary>
        [Pure]
        public Turn SmallestClockwiseEquivalent() {
            var r = -this % OneTurnCounterClockwise;
            if (r < Zero) r += OneTurnCounterClockwise;
            return -r;
        }
        ///<summary>Returns a counter-clockwise rotation with the same magnitude.</summary>
        [Pure]
        public Turn Abs() {
            return this >= Zero ? this : -this;
        }
        ///<summary>Returns -1,0, or +1 based on  the rotation being clockwise, zero, or counter-clockwise respectively.</summary>
        [Pure]
        public int Sign() {
            return Math.Sign(this.CompareTo(Zero));
        }

        ///<summary>Returns a turn equivalent to applying both given turns.</summary>
        public static Turn operator +(Turn turn1, Turn turn2) {
            return new Turn(turn1._radians + turn2._radians);
        }
        ///<summary>Returns a turn equivalent to adding the first given turn and the inverse of the second given turn.</summary>
        public static Turn operator -(Turn turn1, Turn turn2) {
            return new Turn(turn1._radians - turn2._radians);
        }
        ///<summary>Inverts the given turn. Rotating by the result will undo rotating by the input.</summary>
        public static Turn operator -(Turn turn) {
            return new Turn(-turn._radians);
        }
        ///<summary>Scales the given turn by the given factor.</summary>
        public static Turn operator *(Turn turn, double factor) {
            return new Turn(turn._radians * factor);
        }
        ///<summary>Scales the given turn by the given factor.</summary>
        public static Turn operator *(double factor, Turn turn) {
            return new Turn(factor * turn._radians);
        }
        ///<summary>Divides the given turn by the given factor.</summary>
        public static Turn operator /(Turn turn, double divisor) {
            return new Turn(turn._radians/divisor);
        }
        ///<summary>Returns the ratio between two turns.</summary>
        public static double operator /(Turn turn1, Turn turn2) {
            return turn1._radians/turn2._radians;
        }
        /// <summary>
        /// Returns the remainder left from dividing the first given turn by the second.
        /// Uses the standard C# rules for signs, with counter clockwise turns being considered positive.
        /// </summary>
        public static Turn operator %(Turn turn, Turn divisor) {
            return new Turn(turn._radians%divisor._radians);
        }

        ///<summary>Determines if one turn is a greater counter-clockwise rotation than another.</summary>
        public static bool operator >(Turn turn1, Turn turn2) {
            return turn1._radians > turn2._radians;
        }
        ///<summary>Determines if one turn is a lesser counter-clockwise rotation than another.</summary>
        public static bool operator <(Turn turn1, Turn turn2) {
            return turn1._radians < turn2._radians;
        }
        ///<summary>Determines if one turn is a greater or equal counter-clockwise rotation than another.</summary>
        public static bool operator >=(Turn turn1, Turn turn2) {
            return turn1._radians >= turn2._radians;
        }
        ///<summary>Determines if one turn is a lesser or equal counter-clockwise rotation than another.</summary>
        public static bool operator <=(Turn turn1, Turn turn2) {
            return turn1._radians <= turn2._radians;
        }
        /// <summary>
        /// Determines if a turn is equivalent to another.
        /// Note that the count of full rotations matters.
        /// For example, a full clockwise rotation is not equivalent to no rotation.
        /// Use one of the 'SmallestX' methods if you want to remove ambiguities based on rotation count.
        /// </summary>
        public static bool operator ==(Turn turn1, Turn turn2) {
            return turn1._radians == turn2._radians;
        }
        /// <summary>
        /// Determines if a turn is not equivalent to another.
        /// Note that the count of full rotations matters.
        /// For example, a full clockwise rotation is not equivalent to no rotation.
        /// Use one of the 'SmallestX' methods if you want to remove ambiguities based on rotation count.
        /// </summary>
        public static bool operator !=(Turn turn1, Turn turn2) {
            return turn1._radians != turn2._radians;
        }
        /// <summary>
        /// Determines if a turn is equivalent to another.
        /// Note that the count of full rotations matters.
        /// For example, a full clockwise rotation is not equivalent to no rotation.
        /// Use one of the 'SmallestX' methods if you want to remove ambiguities based on rotation count.
        /// </summary>
        public bool Equals(Turn other) {
            return this == other;
        }
        /// <summary>
        /// Determines if a turn is equivalent to another, within a given tolerance.
        /// Note that the count of full rotations matters.
        /// For example, a full clockwise rotation is not equivalent to no rotation.
        /// Use one of the 'SmallestX' methods if you want to remove ambiguities based on rotation count.
        /// </summary>
        [Pure]
        public bool Equals(Turn other, Turn tolerance) {
            return (this - other).Abs() <= tolerance.Abs();
        }
        ///<summary>Compares the two given rotations, with "more counter-clockwise" corresponding to "larger".</summary>
        public int CompareTo(Turn other) {
            return this._radians.CompareTo(other._radians);
        }
        public override bool Equals(object obj) {
            return obj is Turn && Equals((Turn)obj);
        }
        public override int GetHashCode() {
            return _radians.GetHashCode();
        }
        public override string ToString() {
            if (this == Zero) return "0 turns";
            return String.Format(
                "{0:0.###} turns {1}",
                Math.Abs(this/OneTurnCounterClockwise),
                this.Sign() == OneTurnClockwise.Sign() ? "clockwise" : "counterclockwise");
        }
    }
}
