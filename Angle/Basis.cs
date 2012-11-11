using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Angle {
    /// <summary>
    /// A system of angles, determining the size and clockwise-ness of the angular units and the direction of the zero angle.
    /// Used for translating raw angle values to and from invariant directions (Dir) and rotations (Turn).
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Basis {
        ///<summary>The number of radians in a full rotation (2*pi).</summary>
        public const double RadiansPerRotation = 2 * Math.PI;
        ///<summary>The number of degrees in a full rotation (360).</summary>
        public const double DegreesPerRotation = 360;
        ///<summary>The number of gradians in a full rotation (400).</summary>
        public const double GradiansPerRotation = 400;
        /// <summary>
        /// Natural angular units.
        /// The origin is along the positive X axis.
        /// The unit is counter-clockwise radians.
        /// For example, angle 0 is along the vector (x:1,y:0) and angle pi/2 is along the vector (x:0, y:1).
        /// </summary>
        public static readonly Basis Natural = default(Basis);

        private readonly double _origin;
        private readonly double _counterClockwiseRadiansPerUnitMinusOne;
        private Basis(double origin, double counterClockwiseRadiansPerUnit) {
            this._origin = origin;
            this._counterClockwiseRadiansPerUnitMinusOne = counterClockwiseRadiansPerUnit - 1;
        }
        ///<summary>Returns a basis with a zero angle along the given direction, and units that satisfy the given constraints.</summary>
        public static Basis FromDirectionAndUnits(Dir origin, double unitsPerTurn, bool isClockwisePositive) {
            if (unitsPerTurn <= 0) throw new ArgumentOutOfRangeException("unitsPerTurn", "unitsPerTurn <= 0");
            if (double.IsNaN(unitsPerTurn)) throw new ArgumentOutOfRangeException("unitsPerTurn", "unitsPerTurn is not a number");
            if (double.IsInfinity(unitsPerTurn)) throw new ArgumentOutOfRangeException("unitsPerTurn", "unitsPerTurn is infinite");
            var sign = isClockwisePositive ? -1 : +1;
            var factor = RadiansPerRotation / unitsPerTurn;
            return new Basis(origin.UnsignedNaturalAngle, sign * factor);
        }

        ///<summary>The angle, in the natural basis, where the origin (zero) of this basis is located.</summary>
        public double Origin { get { return _origin; } }
        ///<summary>The conversion factor that, when multiplied against clockwise-negative radians, results in an angle measured in the angular unit of this basis.</summary>
        public double CounterClockwiseRadiansPerUnit { get { return _counterClockwiseRadiansPerUnitMinusOne + 1; } }
        ///<summary>The number of units in this basis that make up a counter-clockwise turn.</summary>
        public double UnitsPerCounterClockwiseTurn { get { return RadiansPerRotation/CounterClockwiseRadiansPerUnit; } }
        ///<summary>Determines if increasing an angle corresponds to rotating clockwise or counterclockwise.</summary>
        public bool IsClockwisePositive { get { return CounterClockwiseRadiansPerUnit < 0; } }

        ///<summary>Returns the direction corresponding to the given angle in this basis.</summary>
        [Pure]
        public Dir AngleToDir(double angle) {
            if (double.IsNaN(angle)) throw new ArgumentOutOfRangeException("angle", "angle is NaN");
            if (double.IsInfinity(angle)) throw new ArgumentOutOfRangeException("angle", "angle is infinite");
            return Dir.FromNaturalAngle(Origin + angle * CounterClockwiseRadiansPerUnit);
        }
        ///<summary>Returns the turn corresponding to a rotation by the given angle in this basis.</summary>
        [Pure]
        public Turn AngleToTurn(double angle) {
            return Turn.FromAngle(angle, this);
        }
        ///<summary>Returns the smallest non-negative angle corresponding to the given direction in this basis.</summary>
        [Pure]
        public double DirToUnsignedAngle(Dir direction) {
            var r = (direction.UnsignedNaturalAngle - Origin) / CounterClockwiseRadiansPerUnit;
            return r.ProperMod(Math.Abs(UnitsPerCounterClockwiseTurn));
        }
        ///<summary>Returns the smallest positive or negative angle corresponding to the given direction in this basis.</summary>
        [Pure]
        public double DirToSignedAngle(Dir direction) {
            var r = (direction.UnsignedNaturalAngle - Origin) / CounterClockwiseRadiansPerUnit;
            return r.DifMod(Math.Abs(UnitsPerCounterClockwiseTurn));
        }
        ///<summary>Returns the angle corresponding to a rotation by the given turn in this basis.</summary>
        [Pure]
        public double TurnToAngle(Turn turn) {
            return turn.GetAngle(this);
        }

        public override string ToString() {
            var positiveDir = IsClockwisePositive ? "clockwise" : "counterclockwise";
            var unitsPerTurn = Math.Abs(UnitsPerCounterClockwiseTurn);
            var unit = Math.Abs(unitsPerTurn - RadiansPerRotation) <= 0.000001 ? "Radians"
                     : Math.Abs(unitsPerTurn - DegreesPerRotation) <= 0.000001 ? "Degrees"
                     : Math.Abs(unitsPerTurn - GradiansPerRotation) <= 0.000001 ? "Gradians"
                     : Math.Abs(unitsPerTurn - 1) <= 0.000001 ? "Turns"
                     : String.Format("{0:0.###}/Turn", unitsPerTurn);
            return String.Format(
                "Unit: {0} ({1}), Zero: {2}",
                unit,
                positiveDir,
                Dir.FromNaturalAngle(_origin));
        }
    }
}
