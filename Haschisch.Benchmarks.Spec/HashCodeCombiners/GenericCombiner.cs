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
    }
}
