using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Benchmarks
{
    public static class GenericCombiner<THasher>
        where THasher : IUnsafeBlockHasher<int>
    {
        public static int Combine<T1>(T1 value1)
        {
            var h = value1?.GetHashCode() ?? 0;

            return default(THasher).Hash(
                ref Unsafe.As<int, byte>(ref h),
                sizeof(int));
        }

        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var h = new PackedList<int, int>(
                value1?.GetHashCode() ?? 0,
                value2?.GetHashCode() ?? 0);

            return default(THasher).Hash(
                ref Unsafe.As<PackedList<int, int>, byte>(ref h),
                2 * sizeof(int));
        }

        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            var h = new PackedList<int, int, int>(
                value1?.GetHashCode() ?? 0,
                value2?.GetHashCode() ?? 0,
                value3?.GetHashCode() ?? 0);

            return default(THasher).Hash(
                ref Unsafe.As<PackedList<int, int, int>, byte>(ref h),
                3 * sizeof(int));
        }

        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var h = new PackedList<int, int, int, int>(
                value1?.GetHashCode() ?? 0,
                value2?.GetHashCode() ?? 0,
                value3?.GetHashCode() ?? 0,
                value4?.GetHashCode() ?? 0);

            return default(THasher).Hash(
                ref Unsafe.As<PackedList<int, int, int, int>, byte>(ref h),
                4 * sizeof(int));
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            var h = new PackedList<int, int, int, int, int, int, int, int>(
                value1?.GetHashCode() ?? 0,
                value2?.GetHashCode() ?? 0,
                value3?.GetHashCode() ?? 0,
                value4?.GetHashCode() ?? 0,
                value5?.GetHashCode() ?? 0,
                value6?.GetHashCode() ?? 0,
                value7?.GetHashCode() ?? 0,
                value8?.GetHashCode() ?? 0);

            return default(THasher).Hash(
                ref Unsafe.As<PackedList<int, int, int, int, int, int, int, int>, byte>(ref h),
                8 * sizeof(int));
        }
    }
}
