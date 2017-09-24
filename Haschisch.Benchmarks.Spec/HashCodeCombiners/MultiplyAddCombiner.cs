using System.Runtime.CompilerServices;

namespace Haschisch.Benchmarks
{
    // as in famous StackOverflow answer
    public struct MultiplyAddCombiner : IHashCodeCombiner
    {
        public static int Combine<T1>(T1 value1)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                hash = hash * 23 + value2?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                hash = hash * 23 + value2?.GetHashCode() ?? 0;
                hash = hash * 23 + value3?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                hash = hash * 23 + value2?.GetHashCode() ?? 0;
                hash = hash * 23 + value3?.GetHashCode() ?? 0;
                hash = hash * 23 + value4?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                hash = hash * 23 + value2?.GetHashCode() ?? 0;
                hash = hash * 23 + value3?.GetHashCode() ?? 0;
                hash = hash * 23 + value4?.GetHashCode() ?? 0;
                hash = hash * 23 + value5?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                hash = hash * 23 + value2?.GetHashCode() ?? 0;
                hash = hash * 23 + value3?.GetHashCode() ?? 0;
                hash = hash * 23 + value4?.GetHashCode() ?? 0;
                hash = hash * 23 + value5?.GetHashCode() ?? 0;
                hash = hash * 23 + value6?.GetHashCode() ?? 0;
                hash = hash * 23 + value7?.GetHashCode() ?? 0;
                hash = hash * 23 + value8?.GetHashCode() ?? 0;
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
        int IHashCodeCombiner.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7, T8 x8) => Combine(x1, x2, x3, x4, x5, x6, x7, x8);
    }
}
