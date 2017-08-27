namespace Haschisch.Benchmarks
{
    // similar as the regular one, except that all hash-codes are extracted
    // before doing the mixing
    public static class MultiplyAddReorderedCombiner
    {
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
    }
}
