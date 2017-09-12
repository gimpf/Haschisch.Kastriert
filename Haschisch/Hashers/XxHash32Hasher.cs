using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class XXHash32Hasher
    {
        internal static readonly uint DefaultSeed = GetNewSeed();

        public static uint GetNewSeed()
        {
            Seeder.GetNewSeed(out uint result);
            return result;
        }

        public struct Block :
            ISeedableBlockHasher<int, uint>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<int>,
            ISeedableBlockHasher
        {
            public byte[] GetZeroSeed() => new byte[sizeof(uint)];

            public byte[] Hash(byte[] seed, byte[] data, int offset, int length) =>
                BitConverter.GetBytes(this.Hash(BitConverter.ToUInt32(seed, 0), data, offset, length));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Hash(byte[] data, int offset, int length) =>
                this.Hash(DefaultSeed, data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Hash(uint seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length != 0 ?
                    this.Hash(seed, ref data[offset], length) :
                    this.Hash(seed, ref empty, length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe int Hash(ref byte data, int length) =>
                this.Hash(DefaultSeed, ref data, length);

            public int Hash(uint seed, ref byte data, int length)
            {
                var endIndex = length;
                var remaining = length & (XXHash32Steps.FullBlockSize - 1);
                var fullBlockEndIndex = endIndex - remaining;

                uint h;
                if (length >= XXHash32Steps.FullBlockSize)
                {
                    XXHash32Steps.Long.Initialize(seed, out var v1, out var v2, out var v3, out var v4);

                    for (var i = 0; i < fullBlockEndIndex; i += XXHash32Steps.FullBlockSize)
                    {
                        var x0 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i));
                        var x1 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + sizeof(uint)));
                        var x2 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (2 * sizeof(uint))));
                        var x3 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (3 * sizeof(uint))));

                        XXHash32Steps.Long.MixStep(ref v1, ref v2, ref v3, ref v4, x0, x1, x2, x3);
                    }

                    h = XXHash32Steps.Long.GetSmallStateStart(ref v1, ref v2, ref v3, ref v4);
                }
                else
                {
                    h = seed + XXHash32Steps.Prime32n5;
                }

                XXHash32Steps.Short.Initialize(h, (uint)length, out var state);

                var s0 = UnsafeByteOps.PartialToUInt32(ref data, (uint)endIndex, (uint)(fullBlockEndIndex + 0 * sizeof(uint)));
                var s1 = UnsafeByteOps.PartialToUInt32(ref data, (uint)endIndex, (uint)(fullBlockEndIndex + 1 * sizeof(uint)));
                var s2 = UnsafeByteOps.PartialToUInt32(ref data, (uint)endIndex, (uint)(fullBlockEndIndex + 2 * sizeof(uint)));
                var s3 = UnsafeByteOps.PartialToUInt32(ref data, (uint)endIndex, (uint)(fullBlockEndIndex + 3 * sizeof(uint)));
                XXHash32Steps.Short.MixFinalPartialBlock(ref state, remaining, s0, s1, s2, s3);

                return (int)XXHash32Steps.Short.Finish(ref state);
            }
        }

        public struct Stream : ISeedableStreamingHasher<int, uint>
        {
            private uint v1;
            private uint v2;
            private uint v3;
            private uint v4;
            private uint length;
            private uint bufferIdx;
            private PackedList<uint, uint, uint, uint> buffer;
            private uint seed;

            public bool AllowUnsafeWrite => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize() => this.Initialize(DefaultSeed);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize(uint seed)
            {
                this.seed = seed;
                // XXHash32Steps.Long.Initialize(seed, out this.v1, out this.v2, out this.v3, out this.v4);
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

                var blockEnd = maxLength & ~(XXHash32Steps.FullBlockSize - 1);

                if ((this.length + (ulong)maxLength) > XXHash32Steps.FullBlockSize)
                {
                    if (this.length < XXHash32Steps.FullBlockSize)
                    {
                        XXHash32Steps.Long.Initialize(this.seed, out this.v1, out this.v2, out this.v3, out this.v4);
                    }

                    var iv1 = this.v1;
                    var iv2 = this.v2;
                    var iv3 = this.v3;
                    var iv4 = this.v4;
                    for (var i = 0u; i < blockEnd; i += XXHash32Steps.FullBlockSize)
                    {
                        XXHash32Steps.Long.MixStep(
                            ref iv1,
                            ref iv2,
                            ref iv3,
                            ref iv4,
                            Unsafe.As<byte, uint>(ref Unsafe.Add(ref value, (int)(i + 0 * sizeof(uint)))),
                            Unsafe.As<byte, uint>(ref Unsafe.Add(ref value, (int)(i + 1 * sizeof(uint)))),
                            Unsafe.As<byte, uint>(ref Unsafe.Add(ref value, (int)(i + 2 * sizeof(uint)))),
                            Unsafe.As<byte, uint>(ref Unsafe.Add(ref value, (int)(i + 3 * sizeof(uint)))));
                    }
                    this.v1 = iv1;
                    this.v2 = iv2;
                    this.v3 = iv3;
                    this.v4 = iv4;
                }

                this.length += (uint)blockEnd;

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

                    var h = this.length >= XXHash32Steps.FullBlockSize ?
                        XXHash32Steps.Long.GetSmallStateStart(ref this.v1, ref this.v2, ref this.v3, ref this.v4) :
                        this.seed + XXHash32Steps.Prime32n5;

                    XXHash32Steps.Short.Initialize(h, this.length, out h);
                    XXHash32Steps.Short.MixFinalPartialBlock(ref h, (int)this.length % ((int)BufferUtil.BufferSize(ref this.buffer)), this.buffer.V1, this.buffer.V2, this.buffer.V3, this.buffer.V4);
                    return (long)XXHash32Steps.Short.Finish(ref h);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IStreamingHasher<int>.Finish() => (int)this.Finish();

            private void MixStep()
            {
                if (this.length == 0) { XXHash32Steps.Long.Initialize(seed, out this.v1, out this.v2, out this.v3, out this.v4); }
                this.length += (uint)this.bufferIdx;
                XXHash32Steps.Long.MixStep(ref this.v1, ref this.v2, ref this.v3, ref this.v4, this.buffer.V1, this.buffer.V2, this.buffer.V3, this.buffer.V4);
                this.bufferIdx = 0;
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                XXHash32Steps.Short.Initialize(DefaultSeed + XXHash32Steps.Prime32n5, sizeof(int), out var state);
                XXHash32Steps.Short.MixFinalInt(ref state, (uint)x1);
                return (int)XXHash32Steps.Short.Finish(ref state);
            }

            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                XXHash32Steps.Short.Initialize(DefaultSeed + XXHash32Steps.Prime32n5, 2 * sizeof(int), out var state);
                XXHash32Steps.Short.MixFinalInt(ref state, (uint)x1);
                XXHash32Steps.Short.MixFinalInt(ref state, (uint)x2);
                return (int)XXHash32Steps.Short.Finish(ref state);
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                XXHash32Steps.Long.Initialize(DefaultSeed, out var v1, out var v2, out var v3, out var v4);
                XXHash32Steps.Long.MixStep(ref v1, ref v2, ref v3, ref v4, (uint)x1, (uint)x2, (uint)x3, (uint)x4);
                var s = XXHash32Steps.Long.GetSmallStateStart(ref v1, ref v2, ref v3, ref v4);
                XXHash32Steps.Short.Initialize(s, 4 * sizeof(int), out var state);
                return (int)XXHash32Steps.Short.Finish(ref state);
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                var x5 = value5?.GetHashCode() ?? 0;
                XXHash32Steps.Long.Initialize(DefaultSeed, out var v1, out var v2, out var v3, out var v4);
                XXHash32Steps.Long.MixStep(ref v1, ref v2, ref v3, ref v4, (uint)x1, (uint)x2, (uint)x3, (uint)x4);
                var s = XXHash32Steps.Long.GetSmallStateStart(ref v1, ref v2, ref v3, ref v4);
                XXHash32Steps.Short.Initialize(s, 5 * sizeof(int), out var state);
                XXHash32Steps.Short.MixFinalInt(ref state, (uint)x5);
                return (int)XXHash32Steps.Short.Finish(ref state);
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
                XXHash32Steps.Long.Initialize(DefaultSeed, out var v1, out var v2, out var v3, out var v4);
                XXHash32Steps.Long.MixStep(ref v1, ref v2, ref v3, ref v4, (uint)x1, (uint)x2, (uint)x3, (uint)x4);
                XXHash32Steps.Long.MixStep(ref v1, ref v2, ref v3, ref v4, (uint)x5, (uint)x6, (uint)x7, (uint)x8);
                var s = XXHash32Steps.Long.GetSmallStateStart(ref v1, ref v2, ref v3, ref v4);
                XXHash32Steps.Short.Initialize(s, 8 * sizeof(int), out var state);
                return (int)XXHash32Steps.Short.Finish(ref state);
            }
        }
    }
}
