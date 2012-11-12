using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Strilanc.Angle {
    [DebuggerDisplay("{ToString()}")]
    public struct Range : IEquatable<Range> {
        public static readonly Range AllDirections = new Range(Dir.AlongPositiveX, Turn.OneTurnCounterClockwise);
        public static readonly Range PositiveY = new Range(Dir.AlongPositiveX, Turn.OneTurnCounterClockwise * 0.5);
        public static readonly Range NegativeY = new Range(Dir.AlongNegativeX, Turn.OneTurnCounterClockwise * 0.5);
        public static readonly Range PositiveX = new Range(Dir.AlongNegativeY, Turn.OneTurnCounterClockwise * 0.5);
        public static readonly Range NegativeX = new Range(Dir.AlongPositiveY, Turn.OneTurnCounterClockwise * 0.5);
        public static readonly Range PositiveXPositiveY = new Range(Dir.AlongPositiveX, Turn.OneTurnCounterClockwise * 0.25);
        public static readonly Range NegativeXPositiveY = new Range(Dir.AlongPositiveY, Turn.OneTurnCounterClockwise * 0.25);
        public static readonly Range NegativeXNegativeY = new Range(Dir.AlongNegativeX, Turn.OneTurnCounterClockwise * 0.25);
        public static readonly Range PositiveXNegativeY = new Range(Dir.AlongNegativeY, Turn.OneTurnCounterClockwise * 0.25);

        private readonly Dir _start;
        private readonly Turn _span;
        private Range(Dir start, Turn span) {
            this._start = start;
            this._span = span.IsMoreCounterClockwiseThan(Turn.OneTurnCounterClockwise) ? Turn.OneTurnCounterClockwise : span;
        }

        /// <summary>
        /// Returns a range corresponding to the directions that are covered by starting from a given start direction then rotating until a given finish direction is reached.
        /// The direction of rotation (clockwise vs counter-clockwise) is determined by the moveClockwise parameter.
        /// </summary>
        public static Range FromStartToFinish(Dir start, Dir finish, bool moveClockwise) {
            if (moveClockwise) return FromStartToFinish(finish, start, false);
            return new Range(start, (finish - start).MinimumCongruentCounterClockwiseTurn());
        }
        ///<summary>Returns a range corresponding to the directions that can be reached by starting from the given center direction and turning up to the given maximum (in either direction).</summary>
        public static Range FromCenterAndMaxDeviation(Dir center, Turn absMaxDeviation) {
            var d = absMaxDeviation.AbsCounterClockwise();
            return new Range(center - d, d * 2);
        }
        /// <summary>
        /// Returns a range corresponding to the directions that are covered by starting from a given start direction then rotating until a given finish direction is reached.
        /// The direction of rotation (clockwise vs counter-clockwise) is the positive direction in the given basis.
        /// </summary>
        public static Range FromStartToFinishIncreasingInBasis(Dir start, Dir finish, Basis basis) {
            return FromStartToFinish(start, finish, basis.IsClockwisePositive);
        }
        /// <summary>
        /// Returns a range corresponding to the directions that are covered by starting from a given start direction then rotating until a given finish direction is reached.
        /// The direction of rotation (clockwise vs counter-clockwise) is the negative direction in the given basis.
        /// </summary>
        public static Range FromStartToFinishDecreasingInBasis(Dir start, Dir finish, Basis basis) {
            return FromStartToFinish(start, finish, !basis.IsClockwisePositive);
        }
        /// <summary>
        /// Returns a range corresponding to the directions that are covered by starting from a given start angle then rotating until a given finish angle is reached.
        /// The direction of rotation (clockwise vs counter-clockwise) is the positive direction in the given basis.
        /// </summary>
        public static Range FromStartToFinishIncreasingInBasis(double startAngle, double finishAngle, Basis basis) {
            return FromStartToFinishIncreasingInBasis(basis.AngleToDir(startAngle), basis.AngleToDir(finishAngle), basis);
        }
        /// <summary>
        /// Returns a range corresponding to the directions that are covered by starting from a given start angle then rotating until a given finish angle is reached.
        /// The direction of rotation (clockwise vs counter-clockwise) is the negative direction in the given basis.
        /// </summary>
        public static Range FromStartToFinishDecreasingInBasis(double startAngle, double finishAngle, Basis basis) {
            return FromStartToFinishDecreasingInBasis(basis.AngleToDir(startAngle), basis.AngleToDir(finishAngle), basis);
        }

        ///<summary>The amount of rotation necessary to turn from the clockwise side to the counter-clockwise side.</summary>
        public Turn Span { get { return _span; } }
        ///<summary>The counter-clockwise side of the range (inclusive). Values that are further counter-clockwise are not in the range (unless the range covers all directions).</summary>
        public Dir CounterClockwiseSide { get { return _start + _span; } }
        ///<summary>The clockwise side of the range (inclusive). Values that are further clockwise are not in the range (unless the range covers all directions).</summary>
        public Dir ClockwiseSide { get { return _start; } }
        ///<summary>The center of the range, where equal amounts of absolute rotation are needed to reach either side.</summary>
        public Dir Center { get { return _start + _span/2; } }
        
        ///<summary>Returns the inverse range, which includes directions not in this range and excludes directions in this range (except the end points are shared).</summary>
        [Pure]
        public Range Inverse() {
            return new Range(_start + _span, Turn.OneTurnCounterClockwise - _span);
        }
        ///<summary>Determines if a given direction is in this range.</summary>
        [Pure]
        public bool Contains(Dir direction) {
            return !(direction - _start).MinimumCongruentCounterClockwiseTurn().IsMoreCounterClockwiseThan(_span);
        }
        ///<summary>Returns the clockwise or counter-clockwise side of this range, depending on the given parameter.</summary>
        [Pure]
        public Dir Side(bool clockwiseSide) {
            return clockwiseSide ? ClockwiseSide : CounterClockwiseSide;
        }
        ///<summary>Forces the given direction to be inside this range, rotating it by as little as possible.</summary>
        [Pure]
        public Dir Clamp(Dir direction) {
            var d = (direction - Center).MinimumCongruentTurn();
            if (!d.IsMoreRotationThan(_span / 2)) return direction;
            return Side(d.IsClockwise);
        }

        ///<summary>Rotates the given range by the given turn.</summary>
        public static Range operator +(Range range, Turn turn) {
            return new Range(range._start + turn, range._span);
        }
        ///<summary>Rotates the given range by the opposite of the given turn.</summary>
        public static Range operator -(Range range, Turn turn) {
            return new Range(range._start - turn, range._span);
        }
        ///<summary>Determines if two ranges are equivalent. Note that two 'all directions' ranges are only equal if they have the same center.</summary>
        public static bool operator ==(Range range1, Range range2) {
            return range1.Equals(range2);
        }
        ///<summary>Determines if two ranges are not equivalent. Note that two 'all directions' ranges are only equal if they have the same center.</summary>
        public static bool operator !=(Range range1, Range range2) {
            return !range1.Equals(range2);
        }

        ///<summary>Determines if two ranges are equivalent. Note that two 'all directions' ranges are only equal if they have the same center.</summary>
        public bool Equals(Range other) {
            return _start.Equals(other._start) 
                && _span.Equals(other._span);
        }
        ///<summary>Determines if two ranges are equivalent, within some tolerance. Note that two 'all directions' ranges are only equal if they have the same center.</summary>
        [Pure]
        public bool Equals(Range other, Turn tolerance) {
            return _start.Equals(other._start, tolerance) 
                && _span.Equals(other._span, tolerance);
        }
        public override bool Equals(object obj) {
            return obj is Range && Equals((Range)obj);
        }
        public override int GetHashCode() {
            unchecked {
                return _start.GetHashCode() ^ (_span.GetHashCode()*3);
            }
        }
        public override string ToString() {
            return String.Format("Span: {1:0.###} turns, Center: {0}", Center, Math.Abs(Span / Turn.OneTurnCounterClockwise));
        }
    }
}
