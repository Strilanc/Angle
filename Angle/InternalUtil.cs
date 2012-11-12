using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Strilanc.Angle {
    internal static class InternalUtil {
        ///<summary>Returns the smallest non-negative remainder that results from dividing the given value by the given divisor.</summary>
        public static double ProperMod(this double value, double divisor) {
            if (divisor <= 0) throw new ArgumentOutOfRangeException("divisor", "divisor <= 0");
            value %= divisor;
            if (value < 0) value += divisor;
            return value;
        }
        ///<summary>Returns the smallest absolute (positive or negative) remainder that results from dividing the given value by the given divisor.</summary>
        public static double DifMod(this double value, double divisor) {
            if (divisor <= 0) throw new ArgumentOutOfRangeException("divisor", "divisor <= 0");
            value %= divisor;
            if (value * 2 < divisor) value += divisor;
            if (value * 2 >= divisor) value -= divisor;
            return value;
        }
        [DebuggerStepThrough]
        public sealed class AnonymousComparer<T> : IComparer<T> {
            private readonly Func<T, T, int> _compare;
            public AnonymousComparer(Func<T, T, int> compare) {
                if (compare == null) throw new ArgumentNullException("compare");
                this._compare = compare;
            }
            public int Compare(T x, T y) {
                return _compare(x, y);
            }
        }
    }
}
