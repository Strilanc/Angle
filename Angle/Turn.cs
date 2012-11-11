﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Angle {
    /// <summary>
    /// An amount to rotate by.
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

        ///<summary>A comparer that considers turns that are more counter-clockwise to be larger.</summary>
        public static readonly IComparer<Turn> CounterClockwiseComparer = new InternalUtil.AnonymousComparer<Turn>(
            (e1, e2) => e1._radians.CompareTo(e2._radians));
        ///<summary>A comparer that considers turns that are more clockwise to be larger.</summary>
        public static readonly IComparer<Turn> ClockwiseComparer = new InternalUtil.AnonymousComparer<Turn>(
            (e1, e2) => (-e1._radians).CompareTo(-e2._radians));
        ///<summary>A comparer that considers turns that involve more rotation, whether clockwise or counter-clockwise, to be larger.</summary>
        public static readonly IComparer<Turn> AbsoluteRotationComparer = new InternalUtil.AnonymousComparer<Turn>(
            (e1, e2) => Math.Abs(e1._radians).CompareTo(Math.Abs(e2._radians)));
        
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
            return new Turn(_radians.DifMod(Basis.RadiansPerRotation));
        }
        ///<summary>The smallest counter-clockwise turn with an equivalent effect on directions.</summary>
        [Pure]
        public Turn SmallestCounterClockwiseEquivalent() {
            return new Turn(_radians.ProperMod(Basis.RadiansPerRotation));
        }
        ///<summary>The smallest clockwise turn with an equivalent effect on directions.</summary>
        [Pure]
        public Turn SmallestClockwiseEquivalent() {
            return new Turn(-(-_radians).ProperMod(Basis.RadiansPerRotation));
        }
        ///<summary>Returns a counter-clockwise rotation with the same magnitude.</summary>
        [Pure]
        public Turn AbsCounterClockwise() {
            return _radians >= 0 ? this : -this;
        }
        ///<summary>Returns a clockwise rotation with the same magnitude.</summary>
        [Pure]
        public Turn AbsClockwise() {
            return _radians <= 0 ? this : -this;
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
        ///<summary>Determines if this turn represents a positive counter-clockwise rotation.</summary>
        public bool IsCounterClockwise { get { return _radians > 0; } }
        ///<summary>Determines if this turn represents a positive clockwise rotation.</summary>
        public bool IsClockwise { get { return _radians < 0; } }

        ///<summary>Determines if this turn represents more clockwise (fewer counter-clockwise) rotations than the given turn.</summary>
        [Pure]
        public bool IsMoreClockwiseThan(Turn other) {
            return _radians < other._radians;
        }
        ///<summary>Determines if this turn represents more counter-clockwise (fewer clockwise) rotations than the given turn.</summary>
        [Pure]
        public bool IsMoreCounterClockwiseThan(Turn other) {
            return _radians > other._radians;
        }
        ///<summary>Determines if this turn represents more absolute rotation, either clockwise or counter-clockwise, than the given turn.</summary>
        [Pure]
        public bool IsMoreRotationThan(Turn other) {
            return Math.Abs(_radians) > Math.Abs(other._radians);
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
            return !(this - other).IsMoreRotationThan(tolerance);
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
                "{0:0.###} {1} turns",
                Math.Abs(this/OneTurnCounterClockwise),
                _radians >= 0 ? "clockwise" : "counterclockwise");
        }
    }
}