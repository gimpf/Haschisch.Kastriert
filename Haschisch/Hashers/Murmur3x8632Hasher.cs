using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class Murmur3x8632Hasher
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
                const int MultiBlockSize = 4 * sizeof(uint);
                var endIndex = length;
                var remaining = length % sizeof(uint);
                var multiBlockEndIndex = length - (length % MultiBlockSize);
                var fullBlockEndIndex = endIndex - remaining;

                Murmur3x8632Steps.Initialize(seed, out var state);

                var s = state;

                for (var i = 0; i < multiBlockEndIndex; i += MultiBlockSize)
                {
                    var x0 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i));
                    var x1 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (1 * sizeof(uint))));
                    var x2 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (2 * sizeof(uint))));
                    var x3 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (3 * sizeof(uint))));
                    Murmur3x8632Steps.MixStep(ref s, x0);
                    Murmur3x8632Steps.MixStep(ref s, x1);
                    Murmur3x8632Steps.MixStep(ref s, x2);
                    Murmur3x8632Steps.MixStep(ref s, x3);
                }

                state = s;

                for (var i = multiBlockEndIndex; i < fullBlockEndIndex; i += sizeof(uint))
                {
                    var x0 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i));
                    Murmur3x8632Steps.MixStep(ref state, x0);
                }

                var s0 = UnsafeByteOps.PartialToUInt32(ref data, fullBlockEndIndex, (uint)(endIndex - fullBlockEndIndex));
                return (int)Murmur3x8632Steps.Finish(ref state, s0, (uint)length);
            }
        }

        public struct Stream : ISeedableStreamingHasher<int, uint>
        {
            private uint state;
            private uint length;
            private uint buffer;
            private uint bufferIdx;

            bool IStreamingHasherSink.AllowUnsafeWrite => false;

            unsafe int IStreamingHasherSink.UnsafeWrite(ref byte value, int maxLength) => throw new NotSupportedException();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize() => this.Initialize(DefaultSeed);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize(uint seed)
            {
                Murmur3x8632Steps.Initialize(seed, out this.state);
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
                unchecked
                {
                    this.Write32((int)value);
                    this.Write32((int)(value >> 32));
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Finish()
            {
                unchecked
                {
                    if (this.bufferIdx == sizeof(uint))
                    {
                        this.MixStep();
                    }

                    BufferUtil.ZeroUnusedBuffer(ref this.buffer, this.bufferIdx);
                    this.length += (uint)this.bufferIdx;

                    return (int)Murmur3x8632Steps.Finish(ref this.state, this.buffer, this.length);
                }
            }

            private void MixStep()
            {
                Murmur3x8632Steps.MixStep(ref this.state, this.buffer);
                this.length += 4;
                this.bufferIdx = 0;
            }
        }
    }
}
