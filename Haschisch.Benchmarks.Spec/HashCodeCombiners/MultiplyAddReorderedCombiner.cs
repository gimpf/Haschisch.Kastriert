﻿using System.Runtime.CompilerServices;

namespace Haschisch.Benchmarks
{
    // similar as the regular one, except that all hash-codes are extracted
    // before doing the mixing
    public struct MultiplyAddReorderedCombiner : IHashCodeCombiner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1>(T1 value1)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;
                var h3 = value3?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                hash = hash * 23 + h3;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;
                var h3 = value3?.GetHashCode() ?? 0;
                var h4 = value4?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                hash = hash * 23 + h3;
                hash = hash * 23 + h4;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;
                var h3 = value3?.GetHashCode() ?? 0;
                var h4 = value4?.GetHashCode() ?? 0;
                var h5 = value5?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                hash = hash * 23 + h3;
                hash = hash * 23 + h4;
                hash = hash * 23 + h5;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;
                var h3 = value3?.GetHashCode() ?? 0;
                var h4 = value4?.GetHashCode() ?? 0;
                var h5 = value5?.GetHashCode() ?? 0;
                var h6 = value6?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                hash = hash * 23 + h3;
                hash = hash * 23 + h4;
                hash = hash * 23 + h5;
                hash = hash * 23 + h6;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;
                var h3 = value3?.GetHashCode() ?? 0;
                var h4 = value4?.GetHashCode() ?? 0;
                var h5 = value5?.GetHashCode() ?? 0;
                var h6 = value6?.GetHashCode() ?? 0;
                var h7 = value7?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                hash = hash * 23 + h3;
                hash = hash * 23 + h4;
                hash = hash * 23 + h5;
                hash = hash * 23 + h6;
                hash = hash * 23 + h7;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            unchecked
            {
                var h1 = value1?.GetHashCode() ?? 0;
                var h2 = value2?.GetHashCode() ?? 0;
                var h3 = value3?.GetHashCode() ?? 0;
                var h4 = value4?.GetHashCode() ?? 0;
                var h5 = value5?.GetHashCode() ?? 0;
                var h6 = value6?.GetHashCode() ?? 0;
                var h7 = value7?.GetHashCode() ?? 0;
                var h8 = value8?.GetHashCode() ?? 0;

                int hash = 17;
                hash = hash * 23 + h1;
                hash = hash * 23 + h2;
                hash = hash * 23 + h3;
                hash = hash * 23 + h4;
                hash = hash * 23 + h5;
                hash = hash * 23 + h6;
                hash = hash * 23 + h7;
                hash = hash * 23 + h8;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1>(T1 x1) => Combine(x1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2>(T1 x1, T2 x2) => Combine(x1, x2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2, T3>(T1 x1, T2 x2, T3 x3) => Combine(x1, x2, x3);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2, T3, T4>(T1 x1, T2 x2, T3 x3, T4 x4) => Combine(x1, x2, x3, x4);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2, T3, T4, T5>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5) => Combine(x1, x2, x3, x4, x5);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2, T3, T4, T5, T6>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6) => Combine(x1, x2, x3, x4, x5, x6);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2, T3, T4, T5, T6, T7>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7) => Combine(x1, x2, x3, x4, x5, x6, x7);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IHashCodeCombiner.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7, T8 x8) => Combine(x1, x2, x3, x4, x5, x6, x7, x8);
    }
}
