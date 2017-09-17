using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class City64Hasher
    {
        public struct Block :
            IBlockHasher<ulong>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<ulong>,
            IUnsafeBlockHasher<int>
        {
            public ulong Hash(byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length == 0 ?
                    City64Steps.Hash(ref empty, 0) :
                    City64Steps.Hash(ref data[offset], (uint)length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length);

            int IUnsafeBlockHasher<int>.Hash(ref byte data, int length) => (int)City64Steps.Hash(ref data, (uint)length);

            public ulong Hash(ref byte data, int length) => City64Steps.Hash(ref data, (uint)length);
        }

        public struct Combiner : IHashCodeCombiner
        {
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;

                return (int)City64Steps.Hash_Len0to16(ref Unsafe.As<int, byte>(ref x1), sizeof(int));
            }

            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x = new PackedList<int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0);

                return (int)City64Steps.Hash_Len0to16(ref Unsafe.As<PackedList<int, int>, byte>(ref x), 2 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x = new PackedList<int, int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0,
                    value4?.GetHashCode() ?? 0);

                return (int)City64Steps.Hash_Len0to16(
                    ref Unsafe.As<PackedList<int, int, int, int>, byte>(ref x), 4 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x = new PackedList<int, int, int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0,
                    value4?.GetHashCode() ?? 0,
                    value5?.GetHashCode() ?? 0);

                return (int)City64Steps.Hash_Len17to32(
                    ref Unsafe.As<PackedList<int, int, int, int, int>, byte>(ref x), 5 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var x = new PackedList<int, int, int, int, int, int, int, int>(
                    value1?.GetHashCode() ?? 0,
                    value2?.GetHashCode() ?? 0,
                    value3?.GetHashCode() ?? 0,
                    value4?.GetHashCode() ?? 0,
                    value5?.GetHashCode() ?? 0,
                    value6?.GetHashCode() ?? 0,
                    value7?.GetHashCode() ?? 0,
                    value8?.GetHashCode() ?? 0);

                return (int)City64Steps.Hash_Len17to32(
                    ref Unsafe.As<PackedList<int, int, int, int, int, int, int, int>, byte>(ref x), 8 * sizeof(int));
            }
        }
    }
}
