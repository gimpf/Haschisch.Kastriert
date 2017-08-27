using Haschisch.Hashers;

namespace Haschisch.Benchmarks
{
    public static class Murmur3AStepsCombiner
    {
        public static int Combine<T1>(T1 value1)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            Murmur3x8632Steps.Initialize(Murmur3x8632Hasher.DefaultSeed, out var state);
            Murmur3x8632Steps.MixStep(ref state, (uint)v1);
            return (int)Murmur3x8632Steps.FinishWithoutPartial(ref state, sizeof(int));
        }

        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            Murmur3x8632Steps.Initialize(Murmur3x8632Hasher.DefaultSeed, out var state);
            Murmur3x8632Steps.MixStep(ref state, (uint)v1);
            Murmur3x8632Steps.MixStep(ref state, (uint)v2);
            return (int)Murmur3x8632Steps.FinishWithoutPartial(ref state, 2 * sizeof(int));
        }

        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;
            var v4 = value4?.GetHashCode() ?? 0;
            Murmur3x8632Steps.Initialize(Murmur3x8632Hasher.DefaultSeed, out var state);
            Murmur3x8632Steps.MixStep(ref state, (uint)v1);
            Murmur3x8632Steps.MixStep(ref state, (uint)v2);
            Murmur3x8632Steps.MixStep(ref state, (uint)v3);
            Murmur3x8632Steps.MixStep(ref state, (uint)v4);
            return (int)Murmur3x8632Steps.FinishWithoutPartial(ref state, 4 * sizeof(int));
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;
            var v4 = value4?.GetHashCode() ?? 0;
            var v5 = value5?.GetHashCode() ?? 0;
            var v6 = value6?.GetHashCode() ?? 0;
            var v7 = value7?.GetHashCode() ?? 0;
            var v8 = value8?.GetHashCode() ?? 0;
            Murmur3x8632Steps.Initialize(Murmur3x8632Hasher.DefaultSeed, out var state);
            Murmur3x8632Steps.MixStep(ref state, (uint)v1);
            Murmur3x8632Steps.MixStep(ref state, (uint)v2);
            Murmur3x8632Steps.MixStep(ref state, (uint)v3);
            Murmur3x8632Steps.MixStep(ref state, (uint)v4);
            Murmur3x8632Steps.MixStep(ref state, (uint)v5);
            Murmur3x8632Steps.MixStep(ref state, (uint)v6);
            Murmur3x8632Steps.MixStep(ref state, (uint)v7);
            Murmur3x8632Steps.MixStep(ref state, (uint)v8);
            return (int)Murmur3x8632Steps.FinishWithoutPartial(ref state, 8 * sizeof(int));
        }
    }
}
