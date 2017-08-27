namespace Haschisch.Benchmarks
{
    // as in famous StackOverflow answer
    public static class MultiplyAddCombiner
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
    }
}
