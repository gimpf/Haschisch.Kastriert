using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class Marvin32Hasher
    {
        internal static readonly ulong DefaultSeed = GetNewSeed();

        public static ulong GetNewSeed()
        {
            Seeder.GetNewSeed(out ulong result);
            return result;
        }

        public struct Block :
            ISeedableBlockHasher<long, ulong>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<int>,
            ISeedableBlockHasher
        {
            public byte[] GetZeroSeed() => new byte[sizeof(ulong)];

            public byte[] Hash(byte[] seed, byte[] data, int offset, int length) =>
                BitConverter.GetBytes(this.Hash(BitConverter.ToUInt64(seed, 0), data, offset, length));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length)
            {
                var h = (ulong)this.Hash(data, offset, length);
                return (int)((uint)(h >> 32) ^ (uint)h);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Hash(byte[] data, int offset, int length) =>
                this.Hash(DefaultSeed, data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Hash(ulong seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length != 0 ?
                    Hash(ref data[offset], length, seed) :
                    Hash(ref empty, 0, seed);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe int Hash(ref byte data, int length)
            {
                var h = (ulong)Hash(ref data, length, DefaultSeed);
                return (int)((uint)(h >> 32) ^ (uint)h);
            }

            internal static long Hash(ref byte data, int count, ulong seed)
            {
                Marvin32Steps.Initialize(seed, out var p0, out var p1);

                var end = count - (count % sizeof(ulong));

                // making temporary copies of the state vars around loops seems to be
                // one of the best tricks to help the JIT compiler producing better code;
                // here it provides around a ~30% performance gain.
                var ip0 = p0;
                var ip1 = p1;

                for (var i = 0; i < end; i += sizeof(ulong))
                {
                    Marvin32Steps.Update(ref ip0, ref ip1, Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i)));
                }

                p0 = ip0;
                p1 = ip1;

                var remaining = count % sizeof(ulong);
                var value = UnsafeByteOps.PartialToUInt64(ref data, end, (uint)remaining);
                return Marvin32Steps.Finish(ref p0, ref p1, value, remaining);
            }
        }

        public struct Stream :
            ISeedableStreamingHasher<long, ulong>, ISeedableStreamingHasher<int, ulong>
        {
            private ulong buffer;
            private uint bufferIdx;
            private uint length;
            private uint p0;
            private uint p1;

            public bool AllowUnsafeWrite => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize() => this.Initialize(DefaultSeed);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize(ulong seed)
            {
                this.bufferIdx = 0;
                this.length = 0;
                Marvin32Steps.Initialize(seed, out this.p0, out this.p1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Write8(byte value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(byte)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Write16(short value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(short)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Write32(int value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(int)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Write64(long value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(long)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Finish()
            {
                if (this.bufferIdx == BufferUtil.BufferSize(ref this.buffer))
                {
                    this.MixStep();
                }

                this.length += (uint)this.bufferIdx;
                return Marvin32Steps.Finish(ref this.p0, ref this.p1, this.buffer, (int)(this.length % sizeof(ulong)));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IStreamingHasher<int>.Finish()
            {
                var result = (ulong)this.Finish();
                return (int)((uint)result ^ (uint)(result >> 32));
            }

            public unsafe int UnsafeWrite(ref byte value, int maxLength)
            {
                if (this.bufferIdx == BufferUtil.BufferSize(ref this.buffer))
                {
                    this.MixStep();
                }

                if (this.bufferIdx != 0) { throw new NotImplementedException("unaligned unsafe write not supported"); }

                var directUpdateLen = maxLength - (maxLength % sizeof(ulong));
                var ip0 = this.p0;
                var ip1 = this.p1;

                for (var i = 0; i < directUpdateLen; i += sizeof(ulong))
                {
                    Marvin32Steps.Update(ref ip0, ref ip1, Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref value, i)));
                }

                this.p0 = ip0;
                this.p1 = ip1;
                this.length += (uint)directUpdateLen;

                for (var i = directUpdateLen; i < maxLength; i++)
                {
                    this.Write8(Unsafe.Add(ref value, i));
                }

                return maxLength;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void MixStep()
            {
                Marvin32Steps.Update(ref this.p0, ref this.p1, this.buffer);
                this.length += 8;
                this.bufferIdx = 0;
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                Marvin32Steps.Initialize(DefaultSeed, out var p0, out var p1);
                return (int)Marvin32Steps.Finish(ref p0, ref p1, (uint) x1, sizeof(int));
            }

            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                Marvin32Steps.Initialize(DefaultSeed, out var p0, out var p1);
                Marvin32Steps.Update(ref p0, ref p1, (uint)x1 | ((ulong)x2 << 32));
                return (int)Marvin32Steps.Finish(ref p0, ref p1, 0, 0);
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                Marvin32Steps.Initialize(DefaultSeed, out var p0, out var p1);
                Marvin32Steps.Update(ref p0, ref p1, (uint)x1 | ((ulong)x2 << 32));
                Marvin32Steps.Update(ref p0, ref p1, (uint)x3 | ((ulong)x4 << 32));
                return (int)Marvin32Steps.Finish(ref p0, ref p1, 0, 0);
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                var x5 = value5?.GetHashCode() ?? 0;
                Marvin32Steps.Initialize(DefaultSeed, out var p0, out var p1);
                Marvin32Steps.Update(ref p0, ref p1, (uint)x1 | ((ulong)x2 << 32));
                Marvin32Steps.Update(ref p0, ref p1, (uint)x3 | ((ulong)x4 << 32));
                return (int)Marvin32Steps.Finish(ref p0, ref p1, (uint)x5, sizeof(uint));
            }

            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                var x5 = value5?.GetHashCode() ?? 0;
                var x6 = value6?.GetHashCode() ?? 0;
                var x7 = value7?.GetHashCode() ?? 0;
                var x8 = value8?.GetHashCode() ?? 0;
                Marvin32Steps.Initialize(DefaultSeed, out var p0, out var p1);
                Marvin32Steps.Update(ref p0, ref p1, (uint)x1 | ((ulong)x2 << 32));
                Marvin32Steps.Update(ref p0, ref p1, (uint)x3 | ((ulong)x4 << 32));
                Marvin32Steps.Update(ref p0, ref p1, (uint)x5 | ((ulong)x6 << 32));
                Marvin32Steps.Update(ref p0, ref p1, (uint)x7 | ((ulong)x8 << 32));
                return (int)Marvin32Steps.Finish(ref p0, ref p1, 0, 0);
            }
        }
    }
}
