using System.Runtime.CompilerServices;
using Haschisch.Hashers;
using Haschisch.Util;

namespace Haschisch.Benchmarks
{
    // Like Murmur3A_TG, plus
    // - extend with seed
    public static class Murmur3A_TG_WithSeed_Combiner
    {
        internal static readonly int Seed = (int)Murmur3x8632Hasher.DefaultSeed;

        public static int Combine<T1>(T1 value1)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            return FinalizeValue(combinedValue, sizeof(int));
        }

        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 2);
        }

        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 3);
        }

        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 4);
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 5);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value6?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 6);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value6?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value7?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 7);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value6?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value7?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value8?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 8);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
        {
            var combinedValue = CombineValue(value1?.GetHashCode() ?? 0, Seed);
            combinedValue = CombineValue(value2?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value6?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value7?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value8?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value9?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 9);
        }

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
