using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class City64WithSeedsHasher
    {
        internal static readonly ulong Seed0 = Seeder.GetNewSeed<ulong>();
        internal static readonly ulong Seed1 = Seeder.GetNewSeed<ulong>();

        public struct Block :
            IBlockHasher<ulong>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<ulong>,
            IUnsafeBlockHasher<int>,
            ISeedableBlockHasher
        {
            public byte[] GetZeroSeed() => new byte[2 * sizeof(ulong)];

            public byte[] Hash(byte[] seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);
                var seed0 = BitConverter.ToUInt64(seed, 0);
                var seed1 = BitConverter.ToUInt64(seed, sizeof(ulong));
                var h = Hash(seed0, seed1, ref data[offset], length);
                return BitConverter.GetBytes(h);
            }

            public ulong Hash(byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                var h = length == 0 ?
                    City64Steps.Hash(ref empty, 0) :
                    City64Steps.Hash(ref data[offset], (uint)length);

                return City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length);

            unsafe int IUnsafeBlockHasher<int>.Hash(ref byte data, int length)
            {
                var h = City64Steps.Hash(ref data, (uint)length);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            unsafe public ulong Hash(ref byte data, int length)
            {
                var h = City64Steps.Hash(ref data, (uint)length);
                return City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static unsafe ulong Hash(ulong seed0, ulong seed1, ref byte data, int length)
            {
                var h = City64Steps.Hash(ref data, (uint)length);
                return City64Steps.MixSeeds(h, seed0, seed1);
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2, value3);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2, value3, value4);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2, value3, value4, value5);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2, value3, value4, value5, value6);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2, value3, value4, value5, value6, value7);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var h = City64Hasher.Combiner.CombineRaw(value1, value2, value3, value4, value5, value6, value7, value8);
                return (int)City64Steps.MixSeeds(h, Seed0, Seed1);
            }
        }
    }
}
