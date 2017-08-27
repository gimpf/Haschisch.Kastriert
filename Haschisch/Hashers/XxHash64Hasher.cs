using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class XXHash64Hasher
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
            public long Hash(byte[] data, int offset, int length) =>
                this.Hash(DefaultSeed, data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Hash(ulong seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length != 0 ?
                    this.Hash(seed, ref data[offset], length) :
                    this.Hash(seed, ref empty, length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe int Hash(ref byte data, int length)
            {
                var h = this.Hash(DefaultSeed, ref data, length);
                return (int)h;
            }

            private long Hash(ulong seed, ref byte data, int length)
            {
                var endIndex = length;
                var remaining = length % XXHash64Steps.FullBlockSize;
                var fullBlockEndIndex = endIndex - remaining;

                ulong h;
                if (length >= XXHash64Steps.FullBlockSize)
                {
                    XXHash64Steps.Long.Initialize(seed, out var v1, out var v2, out var v3, out var v4);

                    for (var i = 0; i < fullBlockEndIndex; i += XXHash64Steps.FullBlockSize)
                    {
                        var x0 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i));
                        var x1 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i + sizeof(ulong)));
                        var x2 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i + (2 * sizeof(ulong))));
                        var x3 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i + (3 * sizeof(ulong))));
                        XXHash64Steps.Long.MixStep(ref v1, ref v2, ref v3, ref v4, x0, x1, x2, x3);
                    }

                    h = XXHash64Steps.Long.GetSmallState(ref v1, ref v2, ref v3, ref v4);
                }
                else
                {
                    h = seed + XXHash64Steps.Prime64n5;
                }

                XXHash64Steps.Short.Initialize(h, (uint)length, out h);
                var xp1 = UnsafeByteOps.PartialToUInt64(ref data, fullBlockEndIndex + (0 * sizeof(ulong)), (uint)(endIndex - (fullBlockEndIndex + (0 * sizeof(ulong)))));
                var xp2 = UnsafeByteOps.PartialToUInt64(ref data, fullBlockEndIndex + (1 * sizeof(ulong)), (uint)(endIndex - (fullBlockEndIndex + (1 * sizeof(ulong)))));
                var xp3 = UnsafeByteOps.PartialToUInt64(ref data, fullBlockEndIndex + (2 * sizeof(ulong)), (uint)(endIndex - (fullBlockEndIndex + (2 * sizeof(ulong)))));
                var xp4 = UnsafeByteOps.PartialToUInt64(ref data, fullBlockEndIndex + (3 * sizeof(ulong)), (uint)(endIndex - (fullBlockEndIndex + (3 * sizeof(ulong)))));
                XXHash64Steps.Short.MixFinalPartialBlock(ref h, length % (4 * sizeof(ulong)), xp1, xp2, xp3, xp4);
                return (long)XXHash64Steps.Short.Finish(ref h);
            }
        }

        public struct Stream : ISeedableStreamingHasher<long, ulong>, IStreamingHasher<int>
        {
            private ulong seed;
            private ulong v1;
            private ulong v2;
            private ulong v3;
            private ulong v4;
            private ulong length;
            private PackedList<ulong, ulong, ulong, ulong> buffer;
            private uint bufferIdx;

            public bool AllowUnsafeWrite => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize() => this.Initialize(XXHash64Hasher.DefaultSeed);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize(ulong seed)
            {
                this.seed = seed;
                //XXHash64Steps.Long.Initialize(seed, out this.v1, out this.v2, out this.v3, out this.v4);
                this.length = 0;
                this.bufferIdx = 0;
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

            public unsafe int UnsafeWrite(ref byte value, int maxLength)
            {
                if (this.bufferIdx != 0) { throw new NotSupportedException("unaligned block hashing not supported"); }

                var blockSize = BufferUtil.BufferSize(ref this.buffer);
                var blockEnd = maxLength - (maxLength % blockSize);

                if ((this.length + (ulong)maxLength) > XXHash64Steps.FullBlockSize)
                {
                    if (this.length < XXHash64Steps.FullBlockSize)
                    {
                        XXHash64Steps.Long.Initialize(this.seed, out this.v1, out this.v2, out this.v3, out this.v4);
                    }

                    var iv1 = this.v1;
                    var iv2 = this.v2;
                    var iv3 = this.v3;
                    var iv4 = this.v4;
                    for (var i = 0u; i < blockEnd; i += blockSize)
                    {
                        XXHash64Steps.Long.MixStep(
                            ref iv1,
                            ref iv2,
                            ref iv3,
                            ref iv4,
                            Unsafe.As<byte, ulong>(ref Unsafe.Add(ref value, (int)(i + 0 * sizeof(ulong)))),
                            Unsafe.As<byte, ulong>(ref Unsafe.Add(ref value, (int)(i + 1 * sizeof(ulong)))),
                            Unsafe.As<byte, ulong>(ref Unsafe.Add(ref value, (int)(i + 2 * sizeof(ulong)))),
                            Unsafe.As<byte, ulong>(ref Unsafe.Add(ref value, (int)(i + 3 * sizeof(ulong)))));
                    }
                    this.v1 = iv1;
                    this.v2 = iv2;
                    this.v3 = iv3;
                    this.v4 = iv4;
                }

                this.length += (ulong)blockEnd;

                var written = BufferUtil.AppendRaw(
                    ref this.buffer, ref this.bufferIdx, ref value, (uint)blockEnd, (uint)maxLength);

                return (int)maxLength;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Finish()
            {
                unchecked
                {
                    var bufferSize = BufferUtil.BufferSize(ref this.buffer);
                    if (this.bufferIdx == bufferSize)
                    {
                        this.MixStep();
                    }

                    this.length += this.bufferIdx;
                    BufferUtil.ZeroUnusedBuffer(ref this.buffer, this.bufferIdx);

                    var h = this.length >= XXHash64Steps.FullBlockSize ?
                        XXHash64Steps.Long.GetSmallState(ref this.v1, ref this.v2, ref this.v3, ref this.v4) :
                        this.seed + XXHash64Steps.Prime64n5;

                    XXHash64Steps.Short.Initialize(h, this.length, out h);
                    XXHash64Steps.Short.MixFinalPartialBlock(ref h, (int)this.length % ((int)BufferUtil.BufferSize(ref this.buffer)), this.buffer.V1, this.buffer.V2, this.buffer.V3, this.buffer.V4);
                    return (long)XXHash64Steps.Short.Finish(ref h);
                }
            }

            private void MixStep()
            {
                if (this.length == 0) { XXHash64Steps.Long.Initialize(seed, out this.v1, out this.v2, out this.v3, out this.v4); }
                this.length += (uint)this.bufferIdx;
                XXHash64Steps.Long.MixStep(ref this.v1, ref this.v2, ref this.v3, ref this.v4, this.buffer.V1, this.buffer.V2, this.buffer.V3, this.buffer.V4);
                this.bufferIdx = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IStreamingHasher<int>.Finish() => (int)this.Finish();
        }

        public struct Combiner : IHashCodeCombiner
        {
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                XXHash64Steps.Short.Initialize(DefaultSeed + XXHash64Steps.Prime64n5, sizeof(int), out var state);
                state = XXHash64Steps.Short.MixFinalInt(state, (uint)x1);
                return (int)XXHash64Steps.Short.Finish(ref state);
            }

            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                XXHash64Steps.Short.Initialize(DefaultSeed + XXHash64Steps.Prime64n5, 2 * sizeof(int), out var state);
                state = XXHash64Steps.Short.MixFinalLong(state, (uint)x1 | (ulong)x2 << 32);
                return (int)XXHash64Steps.Short.Finish(ref state);
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                XXHash64Steps.Short.Initialize(DefaultSeed + XXHash64Steps.Prime64n5, 4 * sizeof(int), out var state);
                state = XXHash64Steps.Short.MixFinalLong(state, (uint)x1 | (ulong)x2 << 32);
                state = XXHash64Steps.Short.MixFinalLong(state, (uint)x3 | (ulong)x4 << 32);
                return (int)XXHash64Steps.Short.Finish(ref state);
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                var x5 = value5?.GetHashCode() ?? 0;
                XXHash64Steps.Short.Initialize(DefaultSeed + XXHash64Steps.Prime64n5, 5 * sizeof(int), out var state);
                state = XXHash64Steps.Short.MixFinalLong(state, (uint)x1 | (ulong)x2 << 32);
                state = XXHash64Steps.Short.MixFinalLong(state, (uint)x3 | (ulong)x4 << 32);
                state = XXHash64Steps.Short.MixFinalInt(state, (uint)x5);
                return (int)XXHash64Steps.Short.Finish(ref state);
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
                XXHash64Steps.Long.Initialize(DefaultSeed, out var v1, out var v2, out var v3, out var v4);
                XXHash64Steps.Long.MixStep(
                    ref v1,
                    ref v2,
                    ref v3,
                    ref v4,
                    (uint)x1 | (ulong)x2 << 32,
                    (uint)x3 | (ulong)x4 << 32,
                    (uint)x5 | (ulong)x6 << 32,
                    (uint)x7 | (ulong)x8 << 32);
                var s = XXHash64Steps.Long.GetSmallState(ref v1, ref v2, ref v3, ref v4);
                XXHash64Steps.Short.Initialize(s, 8 * sizeof(int), out var state);
                return (int)XXHash64Steps.Short.Finish(ref state);
            }
        }
    }
}
