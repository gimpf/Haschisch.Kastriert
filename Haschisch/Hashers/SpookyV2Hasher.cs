using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Haschisch.Util;

using Pack8 = Haschisch.Util.PackedList<ulong, ulong, ulong, ulong, ulong, ulong, ulong, ulong>;

namespace Haschisch.Hashers
{
    using Pack24 = PackedList<Pack8, Pack8, Pack8>;

    public static class SpookyV2Hasher
    {
        private static readonly (ulong, ulong) DefaultSeed = GetNewSeed();

        public static (ulong, ulong) GetNewSeed()
        {
            Seeder.GetNewSeed(out (ulong, ulong) result);
            return result;
        }

        public struct Block :
            ISeedableBlockHasher<(ulong, ulong), (ulong, ulong)>,
            ISeedableBlockHasher<long, (ulong, ulong)>,
            IBlockHasher<int>,
            ISeedableBlockHasher
        {
            public byte[] GetZeroSeed() => new byte[sizeof(ulong) * 2];

            public byte[] Hash(byte[] seed, byte[] data, int offset, int length) =>
                ByteOps.GetBytes(this.Hash(
                    (BitConverter.ToUInt64(seed, 0), BitConverter.ToUInt64(seed, sizeof(ulong))),
                    data,
                    offset,
                    length));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length).Item1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            long ISeedableBlockHasher<long, (ulong, ulong)>.Hash((ulong, ulong) seed, byte[] data, int offset, int length) =>
                (long)this.Hash(seed, data, offset, length).Item1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            long IBlockHasher<long>.Hash(byte[] data, int offset, int length) =>
                (long)this.Hash(DefaultSeed, data, offset, length).Item1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (ulong, ulong) Hash(byte[] data, int offset, int length) =>
                this.Hash(DefaultSeed, data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (ulong, ulong) Hash((ulong, ulong) seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                if (length == 0) { return HashShort(seed, ref empty, 0); }
                if (length < SpookyV2Steps.Long.MinLongSize) { return HashShort(seed, ref data[offset], length); }
                return HashLong(seed, ref data[offset], length);
            }

            internal unsafe static (ulong, ulong) HashShort((ulong, ulong) seed, ref byte data, int length)
            {
                Debug.Assert(length < SpookyV2Steps.Long.MinLongSize);

                SpookyV2Steps.Short.Initialize(seed, out var s0, out var s1, out var s2, out var s3);

                var blockEnd = length & ~((4 * sizeof(ulong)) - 1);

                var ts0 = s0;
                var ts1 = s1;
                var ts2 = s2;
                var ts3 = s3;
                for (var i = 0; i < blockEnd; i += 4 * sizeof(ulong))
                {
                    SpookyV2Steps.Short.Update(
                        ref ts0, ref ts1, ref ts2, ref ts3,
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 0 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 1 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 2 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 3 * sizeof(ulong))));
                }
                s0 = ts0;
                s1 = ts1;
                s2 = ts2;
                s3 = ts3;

                return SpookyV2Steps.Short.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(blockEnd + 0 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(blockEnd + 1 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(blockEnd + 2 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(blockEnd + 3 * sizeof(ulong))),
                    length);
            }

            private unsafe static (ulong, ulong) HashLong((ulong, ulong) seed, ref byte data, int length)
            {
                Debug.Assert(length >= SpookyV2Steps.Long.MinLongSize);

                SpookyV2Steps.Long.Initialize(
                    seed,
                    out var s0, out var s1, out var s2, out var s3,
                    out var s4, out var s5, out var s6, out var s7,
                    out var s8, out var s9, out var sA, out var sB);

                var end = length;
                var fullBlockEnd = length - (length % SpookyV2Steps.Long.BlockSize);

                var ts0 = s0;
                var ts1 = s1;
                var ts2 = s2;
                var ts3 = s3;
                var ts4 = s4;
                var ts5 = s5;
                var ts6 = s6;
                var ts7 = s7;
                var ts8 = s8;
                var ts9 = s9;
                var tsA = sA;
                var tsB = sB;

                for (var i = 0; i < fullBlockEnd; i += SpookyV2Steps.Long.BlockSize)
                {
                    SpookyV2Steps.Long.Update(
                        ref ts0, ref ts1, ref ts2, ref ts3,
                        ref ts4, ref ts5, ref ts6, ref ts7,
                        ref ts8, ref ts9, ref tsA, ref tsB,
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 0 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 1 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 2 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 3 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 4 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 5 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 6 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 7 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 8 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 9 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 10 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i + 11 * sizeof(ulong))));
                }

                s0 = ts0;
                s1 = ts1;
                s2 = ts2;
                s3 = ts3;
                s4 = ts4;
                s5 = ts5;
                s6 = ts6;
                s7 = ts7;
                s8 = ts8;
                s9 = ts9;
                sA = tsA;
                sB = tsB;

                return SpookyV2Steps.Long.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    ref s4, ref s5, ref s6, ref s7,
                    ref s8, ref s9, ref sA, ref sB,
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 0 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 1 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 2 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 3 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 4 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 5 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 6 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 7 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 8 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 9 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 10 * sizeof(ulong))),
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)(fullBlockEnd + 11 * sizeof(ulong))),
                    (ulong)length);
            }
        }

        public struct Stream :
            ISeedableStreamingHasher<(ulong, ulong), (ulong, ulong)>,
            ISeedableStreamingHasher<long, (ulong, ulong)>,
            ISeedableStreamingHasher<int, (ulong, ulong)>
        {
            private ulong s0;
            private ulong s1;
            private ulong s2;
            private ulong s3;
            private ulong s4;
            private ulong s5;
            private ulong s6;
            private ulong s7;
            private ulong s8;
            private ulong s9;
            private ulong sA;
            private ulong sB;
            private ulong length;
            private Pack24 buffer;
            private uint bufferIdx;

            bool IStreamingHasherSink.AllowUnsafeWrite => false;

            unsafe int IStreamingHasherSink.UnsafeWrite(ref byte value, int maxLength) => throw new NotSupportedException();

            public void Initialize() => this.Initialize(DefaultSeed);

            public void Initialize((ulong, ulong) seed)
            {
                this.length = 0;
                this.bufferIdx = 0;
                this.s0 = seed.Item1;
                this.s1 = seed.Item2;
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

            long IStreamingHasher<long>.Finish() => (long)this.Finish().Item1;

            int IStreamingHasher<int>.Finish() => (int)this.Finish().Item1;

            public (ulong, ulong) Finish()
            {
                if (this.bufferIdx == BufferUtil.BufferSize(ref this.buffer))
                {
                    this.MixStep();
                }

                BufferUtil.ZeroUnusedBuffer(ref this.buffer, this.bufferIdx);
                this.length += this.bufferIdx;

                if (this.length < SpookyV2Steps.Long.MinLongSize)
                {
                    return Block.HashShort((this.s0, this.s1), ref Unsafe.As<Pack24, byte>(ref this.buffer), (int)this.length);
                }

                ref var finalPartialBlock = ref Unsafe.As<Pack24, byte>(ref this.buffer);
                var offset = 0;

                if (this.bufferIdx >= SpookyV2Steps.Long.BlockSize)
                {
                    SpookyV2Steps.Long.Update(
                        ref s0, ref s1, ref s2, ref s3,
                        ref s4, ref s5, ref s6, ref s7,
                        ref s8, ref s9, ref sA, ref sB,
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 0 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 1 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 2 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 3 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 4 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 5 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 6 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 7 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 8 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 9 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 10 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 11 * sizeof(ulong))));

                    offset = 12 * sizeof(ulong);
                }

                return SpookyV2Steps.Long.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    ref s4, ref s5, ref s6, ref s7,
                    ref s8, ref s9, ref sA, ref sB,
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 0 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 1 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 2 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 3 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 4 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 5 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 6 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 7 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 8 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 9 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 10 * sizeof(ulong))),
                    Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref finalPartialBlock, offset + 11 * sizeof(ulong))),
                    this.length);
            }

            private void MixStep()
            {
                if (this.length == 0)
                {
                    SpookyV2Steps.Long.Initialize(
                        (this.s0, this.s1),
                        out this.s0, out this.s1, out this.s2, out this.s3,
                        out this.s4, out this.s5, out this.s6, out this.s7,
                        out this.s8, out this.s9, out this.sA, out this.sB);
                }

                SpookyV2Steps.Long.Update(
                        ref this.s0, ref this.s1, ref this.s2, ref this.s3,
                        ref this.s4, ref this.s5, ref this.s6, ref this.s7,
                        ref this.s8, ref this.s9, ref this.sA, ref this.sB,
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 0 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 1 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 2 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 3 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 4 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 5 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 6 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 7 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 8 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 9 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 10 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 11 * sizeof(ulong))));

                SpookyV2Steps.Long.Update(
                        ref this.s0, ref this.s1, ref this.s2, ref this.s3,
                        ref this.s4, ref this.s5, ref this.s6, ref this.s7,
                        ref this.s8, ref this.s9, ref this.sA, ref this.sB,
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 12 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 13 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 14 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 15 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 16 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 17 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 18 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 19 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 20 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 21 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 22 * sizeof(ulong))),
                        Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref Unsafe.As<Pack24, byte>(ref this.buffer), 23 * sizeof(ulong))));

                this.length += this.bufferIdx;
                this.bufferIdx = 0;
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            public int Combine<T1>(T1 x1)
            {
                var h0 = x1?.GetHashCode() ?? 0;

                SpookyV2Steps.Short.Initialize(DefaultSeed, out var s0, out var s1, out var s2, out var s3);
                return (int)SpookyV2Steps.Short.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    (uint)h0, 0, 0, 0,
                    sizeof(int)).Item1;
            }

            public int Combine<T1, T2>(T1 x1, T2 x2)
            {
                var h0 = x1?.GetHashCode() ?? 0;
                var h1 = x2?.GetHashCode() ?? 0;

                SpookyV2Steps.Short.Initialize(DefaultSeed, out var s0, out var s1, out var s2, out var s3);
                return (int)SpookyV2Steps.Short.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    (uint)h0 | (ulong)h1 << 32, 0, 0, 0,
                    2 * sizeof(int)).Item1;
            }

            public int Combine<T1, T2, T3, T4>(T1 x1, T2 x2, T3 x3, T4 x4)
            {
                var h0 = x1?.GetHashCode() ?? 0;
                var h1 = x2?.GetHashCode() ?? 0;
                var h2 = x3?.GetHashCode() ?? 0;
                var h3 = x4?.GetHashCode() ?? 0;

                SpookyV2Steps.Short.Initialize(DefaultSeed, out var s0, out var s1, out var s2, out var s3);
                return (int)SpookyV2Steps.Short.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    (uint)h0 | (ulong)h1 << 32, (uint)h2 | (ulong)h3 << 32, 0, 0,
                    4 * sizeof(int)).Item1;
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5)
            {
                var h0 = x1?.GetHashCode() ?? 0;
                var h1 = x2?.GetHashCode() ?? 0;
                var h2 = x3?.GetHashCode() ?? 0;
                var h3 = x4?.GetHashCode() ?? 0;
                var h4 = x5?.GetHashCode() ?? 0;

                SpookyV2Steps.Short.Initialize(DefaultSeed, out var s0, out var s1, out var s2, out var s3);
                return (int)SpookyV2Steps.Short.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    (uint)h0 | (ulong)h1 << 32, (uint)h2 | (ulong)h3 << 32, (uint)h4, 0,
                    5 * sizeof(int)).Item1;
            }

            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7, T8 x8)
            {
                var h0 = x1?.GetHashCode() ?? 0;
                var h1 = x2?.GetHashCode() ?? 0;
                var h2 = x3?.GetHashCode() ?? 0;
                var h3 = x4?.GetHashCode() ?? 0;
                var h4 = x5?.GetHashCode() ?? 0;
                var h5 = x6?.GetHashCode() ?? 0;
                var h6 = x7?.GetHashCode() ?? 0;
                var h7 = x8?.GetHashCode() ?? 0;

                SpookyV2Steps.Short.Initialize(DefaultSeed, out var s0, out var s1, out var s2, out var s3);
                SpookyV2Steps.Short.Update(
                    ref s0, ref s1, ref s2, ref s3,
                    (uint)h0 | (ulong)h1 << 32, (uint)h2 | (ulong)h3 << 32, (uint)h4 | (ulong)h5 << 32, (uint)h6 | (ulong)h7 << 32);
                return (int)SpookyV2Steps.Short.Finish(
                    ref s0, ref s1, ref s2, ref s3,
                    0, 0, 0, 0,
                    8 * sizeof(int)).Item1;
            }
        }
    }
}
