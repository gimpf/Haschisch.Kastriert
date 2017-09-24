using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class City32Hasher
    {
        public struct Block :
            IBlockHasher<int>,
            IUnsafeBlockHasher<int>
        {
            public int Hash(byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return (int)(length == 0 ?
                    City32Steps.Hash(ref empty, 0) :
                    City32Steps.Hash(ref data[offset], (uint)length));
            }

            public int Hash(ref byte data, int length) => (int)City32Steps.Hash(ref data, (uint)length);
        }

        public struct Combiner : IHashCodeCombiner
        {
            internal static uint Seed = GetNewSeed();

            internal static uint GetNewSeed()
            {
                Seeder.GetNewSeed(out uint seed);
                return seed;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len5to12(Seed, v1, v1, 2 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);

                return (int)City32Steps.Hash_Len5to12(Seed, v2, v1, 3 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);

                var a = v1;
                var b = v1;
                var c = v2;
                var d = v2;
                var e = Seed;
                var f = v3;

                return (int)City32Steps.Hash_Len13to24(a, b, c, d, e, f, 4 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);

                var a = ((ushort)(v1 >> 16) | ((uint)(ushort)v2 << 16));
                var b = v1;
                var c = v3;
                var d = ((ushort)(v2 >> 16) | ((uint)(ushort)v3 << 16));
                var e = Seed;
                var f = v4;

                return (int)City32Steps.Hash_Len13to24(a, b, c, d, e, f, 5 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);

                var a = v2;
                var b = v1;
                var c = v4;
                var d = v3;
                var e = Seed;
                var f = v5;

                return (int)City32Steps.Hash_Len13to24(a, b, c, d, e, f, 6 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);
                var v6 = (uint)(value6?.GetHashCode() ?? 0);
                var v7 = (uint)(value7?.GetHashCode() ?? 0);
                var v8 = (uint)(value8?.GetHashCode() ?? 0);

                City32Steps.Initialize_Gt24(
                    v8, v7, v5, v6, v4,
                    9 * sizeof(int),
                    out var h, out var g, out var f);
                City32Steps.Update_Gt24(
                    ref h, ref g, ref f,
                    Seed, v1, v2, v3, v4);
                return (int)City32Steps.Finish(ref h, ref g, ref f);
            }
        }
    }
}
