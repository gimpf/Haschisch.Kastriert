using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Haschisch.Benchmarks
{
    // from https://gist.githubusercontent.com/tannergooding/89bd72f05ab772bfe5ad3a03d6493650/raw/a4be3d3e6d391b137d564b3403413fe360a940b9/HashCode.cs
    // extended with null checks
    public struct HashCode
    {
        private int _bytesCombined;
        private int _combinedValue;

        public void Add<T>(T value)
        {
            _combinedValue = CombineValue(value?.GetHashCode() ?? 0, _combinedValue);
            _bytesCombined += sizeof(int);
        }

        public void Add<T>(T value, IEqualityComparer<T> comparer)
        {
            _combinedValue = CombineValue(comparer.GetHashCode(value), _combinedValue);
            _bytesCombined += sizeof(int);
        }

        [Obsolete("Use ToHashCode to retrieve the computed hash code.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override int GetHashCode()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            throw new NotImplementedException();
        }

        public int ToHashCode()
        {
            return FinalizeValue(_combinedValue, _bytesCombined);
        }

        #region Static Methods
        // No need to actually instantiate an instance of HashCode for these methods. The number of bytes combined
        // is constant per method (sizeof(int) * number of inputs) and the combined value can be a local.

        public static int Combine<T1>(T1 value1)
        {
            return FinalizeValue(value1?.GetHashCode() ?? 0, sizeof(int));
        }

        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var combinedValue = CombineValue(value2?.GetHashCode() ?? 0, value1?.GetHashCode() ?? 0);
            return FinalizeValue(combinedValue, sizeof(int) * 2);
        }

        public static int Combine_Reordered<T1, T2>(T1 value1, T2 value2)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v2, v1);
            return FinalizeValue(combinedValue, sizeof(int) * 2);
        }

        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var combinedValue = CombineValue(value2?.GetHashCode() ?? 0, value1?.GetHashCode() ?? 0);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 4);
        }

        public static int Combine_Reordered<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var v1 = value1?.GetHashCode() ?? 0;
            var v2 = value2?.GetHashCode() ?? 0;
            var v3 = value3?.GetHashCode() ?? 0;
            var v4 = value4?.GetHashCode() ?? 0;

            var combinedValue = CombineValue(v2, v1);
            combinedValue = CombineValue(v3, combinedValue);
            combinedValue = CombineValue(v4, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 4);
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            var combinedValue = CombineValue(value2?.GetHashCode() ?? 0, value1?.GetHashCode() ?? 0);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 4);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            var combinedValue = CombineValue(value2?.GetHashCode() ?? 0, value1?.GetHashCode() ?? 0);
            combinedValue = CombineValue(value3?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value4?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value5?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value6?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value7?.GetHashCode() ?? 0, combinedValue);
            combinedValue = CombineValue(value8?.GetHashCode() ?? 0, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 8);
        }

        public static int Combine_Reordered<T1, T2, T3, T4, T5, T6, T7, T8>(
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

            var combinedValue = CombineValue(v2, v1);
            combinedValue = CombineValue(v3, combinedValue);
            combinedValue = CombineValue(v4, combinedValue);
            combinedValue = CombineValue(v5, combinedValue);
            combinedValue = CombineValue(v6, combinedValue);
            combinedValue = CombineValue(v7, combinedValue);
            combinedValue = CombineValue(v8, combinedValue);
            return FinalizeValue(combinedValue, sizeof(int) * 8);
        }
        #endregion Static Methods

        #region Helper Methods
        private static int CombineValue(int value, int seed)
        {
            var combinedHashCode = value;

            unchecked
            {
                combinedHashCode *= (-862048943);
                combinedHashCode = unchecked((int)(RotateLeft(unchecked((uint)(combinedHashCode)), 15)));
                combinedHashCode *= 461845907;
                combinedHashCode ^= seed;
                combinedHashCode = unchecked((int)(RotateLeft(unchecked((uint)(combinedHashCode)), 13)));
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
        #endregion
    }
}
