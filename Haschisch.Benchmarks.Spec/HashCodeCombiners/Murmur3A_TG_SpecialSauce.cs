using System.Runtime.CompilerServices;
using Haschisch.Hashers;
using Haschisch.Util;

namespace Haschisch.Benchmarks
{
    // Like Murmur3A_TG, plus
    // - extend with seed
    // - reorder getting hash-codes
    // - aggressive-inline much more
    public static class Murmur3A_TG_SpecialSauce_Combiner
    {
        internal static readonly int Seed = (int)Murmur3x8632Hasher.DefaultSeed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1>(T1 value1)
        {
            var v1 = value1?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v1, Seed);
            return FinalizeValue(combinedValue, sizeof(int) * 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v1, Seed);
            combinedValue = CombineValue(v2, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v1, Seed);
            combinedValue = CombineValue(v2, combinedValue);
            combinedValue = CombineValue(v3, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;
            var v4 = value4?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v1, Seed);
            combinedValue = CombineValue(v2, combinedValue);
            combinedValue = CombineValue(v3, combinedValue);
            combinedValue = CombineValue(v4, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;
            var v4 = value4?.GetHashCode() ?? 0;
            var v5 = value5?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v1, Seed);
            combinedValue = CombineValue(v2, combinedValue);
            combinedValue = CombineValue(v3, combinedValue);
            combinedValue = CombineValue(v4, combinedValue);
            combinedValue = CombineValue(v5, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            var combinedValue = CombineValue(v1, Seed);
            combinedValue = CombineValue(v2, combinedValue);
            combinedValue = CombineValue(v3, combinedValue);
            combinedValue = CombineValue(v4, combinedValue);
            combinedValue = CombineValue(v5, combinedValue);
            combinedValue = CombineValue(v6, combinedValue);
            combinedValue = CombineValue(v7, combinedValue);
            combinedValue = CombineValue(v8, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;
            var v4 = value4?.GetHashCode() ?? 0;
            var v5 = value5?.GetHashCode() ?? 0;
            var v6 = value6?.GetHashCode() ?? 0;
            var v7 = value7?.GetHashCode() ?? 0;
            var v8 = value8?.GetHashCode() ?? 0;
            var v9 = value9?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v1, Seed);
            combinedValue = CombineValue(v2, combinedValue);
            combinedValue = CombineValue(v3, combinedValue);
            combinedValue = CombineValue(v4, combinedValue);
            combinedValue = CombineValue(v5, combinedValue);
            combinedValue = CombineValue(v6, combinedValue);
            combinedValue = CombineValue(v7, combinedValue);
            combinedValue = CombineValue(v8, combinedValue);
            combinedValue = CombineValue(v9, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 9);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CombineValue(int value, int seed)
        {
            var combinedHashCode = value;

            unchecked
            {
                combinedHashCode *= (-862048943);
                combinedHashCode = (int)(RotateLeft((uint)(combinedHashCode), 15));
                combinedHashCode *= 461845907;
                combinedHashCode ^= seed;
                combinedHashCode = (int)(RotateLeft((uint)(combinedHashCode), 13));
                combinedHashCode *= 5;
                combinedHashCode -= 430675100;
            }

            return combinedHashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FinalizeValue(int combinedValue, int bytesCombined)
        {
            var finalizedHashCode = combinedValue;

            unchecked
            {
                finalizedHashCode ^= bytesCombined;
                finalizedHashCode ^= (int)((uint)(finalizedHashCode) >> 16);
                finalizedHashCode *= (-2048144789);

                finalizedHashCode ^= (int)((uint)(finalizedHashCode) >> 13);
                finalizedHashCode *= (-1028477387);
                finalizedHashCode ^= (int)((uint)(finalizedHashCode) >> 16);
            }

            return finalizedHashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint RotateLeft(uint value, byte bits)
        {
            // This pattern is recognized by the JIT and should be optimized
            // to a ROL instruction rather than two shifts.
            return unchecked((value << bits) | (value >> (32 - bits)));
        }
    }
}
