using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public static class SimpleMixCombiner
    {
        // as in famous SO answer
        public static int CombineSimple<T1>(T1 value1)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int CombineSimple<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + value1?.GetHashCode() ?? 0;
                hash = hash * 23 + value2?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static int CombineSimple<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
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
    }
}
