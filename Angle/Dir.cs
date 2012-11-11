using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Angle {
    /// <summary>
    /// A 2-dimensional direction on the XY plane.
    /// Isomorphic to a point on the unit circle.
    /// The 'point' in the affine space of angles.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Dir : IEquatable<Dir> {
        ///<summary>Points directly along the X axis, in the increasing direction.</summary>
        public static readonly Dir AlongPositiveX = new Dir(0);
        ///<summary>Points directly along the Y axis, in the increasing direction.</summary>
        public static readonly Dir AlongPositiveY = new Dir(Basis.RadiansPerRotation / 4);
        ///<summary>Points directly along the X axis, in the decreasing direction.</summary>
        public static readonly Dir AlongNegativeX = new Dir(Basis.RadiansPerRotation / 2);
        ///<summary>Points directly along the Y axis, in the decreasing direction.</summary>
        public static readonly Dir AlongNegativeY = new Dir(3 * Basis.RadiansPerRotation / 4);

        private readonly double _radians;
        private Dir(double radians) {
            if (double.IsNaN(radians)) throw new ArgumentOutOfRangeException("radians", "radians is NaN");
            if (double.IsInfinity(radians)) throw new ArgumentOutOfRangeException("radians", "radians is infinite");
            radians %= Basis.RadiansPerRotation;
            if (radians < 0) radians += Basis.RadiansPerRotation;
            this._radians = radians;
        }
        /// <summary>
        /// Returns the direction corresponding to the given angle in the natural basis.
        /// Example: The natural angle 0 corresponds to the direction from the origin to point (1, 0).
        /// Example: The natural angle pi/2 corresponds to the direction from the origin to point (0, 1).
        /// </summary>
        public static Dir FromNaturalAngle(double angle) {
            if (double.IsNaN(angle)) throw new ArgumentOutOfRangeException("angle", "angle is NaN");
            if (double.IsInfinity(angle)) throw new ArgumentOutOfRangeException("angle", "angle is infinite");
            return new Dir(angle);
        }
        ///<summary>Returns the direction along the given displacement.</summary>
        public static Dir FromVector(double dx, double dy) {
            if (dx == 0 && dy == 0) throw new ArgumentOutOfRangeException("dy", "dx == 0 and dy == 0");
            return FromNaturalAngle(Math.Atan2(dy, dx));
        }
        ///<summary>Returns the direction corresponding to the given angle in the given basis.</summary>
        public static Dir FromAngle(double angle, Basis basis) {
            if (double.IsNaN(angle)) throw new ArgumentOutOfRangeException("angle", "angle is NaN");
            if (double.IsInfinity(angle)) throw new ArgumentOutOfRangeException("angle", "angle is infinite");
            return FromNaturalAngle(basis.Origin + angle*basis.CounterClockwiseRadiansPerUnit);
        }

        ///<summary>The X component of the unit vector pointing along this direction.</summary>
        public double UnitX { get { return Math.Cos(_radians); } }
        ///<summary>The Y component of the unit vector pointing along this direction.</summary>
        public double UnitY { get { return Math.Sin(_radians); } }
        /// <summary>
        /// Returns the angle of this direction in the natural basis.
        /// Example: The direction from the origin to point (1, 0) has a natural angle of 0.
        /// Example: The direction from the origin to point (0, 1) has a natural angle of pi/2.
        /// </summary>
        public double NaturalAngle { get { return _radians; } }
        ///<summary>Returns the smallest non-negative angle corresponding to this direction in the given basis.</summary>
        [Pure]
        public double GetAngle(Basis basis) {
            var r = (NaturalAngle - basis.Origin)/basis.CounterClockwiseRadiansPerUnit;
            var x = Math.Abs(basis.UnitsPerCounterClockwiseTurn);
            r %= x;
            if (r < 0) r += x;
            return r;
        }
        ///<summary>Forces this direction to be inside the given range, rotating it by as little as possible.</summary>
        [Pure]
        public Dir ClampedInside(Range range) {
            return range.Clamp(this);
        }

        ///<summary>Rotates the given direction by the given turn.</summary>
        public static Dir operator +(Dir direction, Turn turn) {
            return new Dir(direction._radians + turn.NaturalAngle);
        }
        ///<summary>Rotates the given direction by the opposite of the given turn.</summary>
        public static Dir operator -(Dir direction, Turn turn) {
            return new Dir(direction._radians - turn.NaturalAngle);
        }
        ///<summary>Returns the turn necessary to rotate from the second given direction to the first given direction.</summary>
        public static Turn operator -(Dir direction1, Dir direction2) {
            return Turn.FromNaturalAngle(direction1.NaturalAngle - direction2.NaturalAngle);
        }

        ///<summary>Determines if two directions are equivalent.</summary>
        public static bool operator ==(Dir direction1, Dir direction2) {
            return direction1._radians == direction2._radians;
        }
        ///<summary>Determines if two directions are not equivalent.</summary>
        public static bool operator !=(Dir direction1, Dir direction2) {
            return direction1._radians != direction2._radians;
        }
        ///<summary>Determines if two directions are equivalent.</summary>
        public bool Equals(Dir other) {
            return this == other;
        }
        ///<summary>Determines if two directions are equivalent, within a tolerance.</summary>
        [Pure]
        public bool Equals(Dir other, Turn tolerance) {
            return (this - other).SmallestSignedEquivalent().Equals(Turn.Zero, tolerance);
        }
        public override bool Equals(object obj) {
            return obj is Dir && Equals((Dir)obj);
        }
        public override int GetHashCode() {
            return _radians.GetHashCode();
        }
        public override string ToString() {
            return String.Format("Towards: <{1:0.###}, {2:0.###}>, Natural Angle: {0:0.###}", _radians, UnitX, UnitY);
        }
    }
}
