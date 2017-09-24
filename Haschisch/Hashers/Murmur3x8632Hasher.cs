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
                var remaining = length & (sizeof(uint) - 1);
                var multiBlockEndIndex = length & ~(MultiBlockSize - 1);
                var fullBlockEndIndex = length - remaining;

                Murmur3x8632Steps.Initialize(seed, out var state);

                var s = state;

                for (var i = 0; i < multiBlockEndIndex; i += MultiBlockSize)
                {
                    var x0 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i));
                    var x1 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (1 * sizeof(uint))));
                    var x2 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (2 * sizeof(uint))));
                    var x3 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (3 * sizeof(uint))));
                    s = Murmur3x8632Steps.MixStep(x0, s);
                    s = Murmur3x8632Steps.MixStep(x1, s);
                    s = Murmur3x8632Steps.MixStep(x2, s);
                    s = Murmur3x8632Steps.MixStep(x3, s);
                }

                state = s;

                for (var i = multiBlockEndIndex; i < fullBlockEndIndex; i += sizeof(uint))
                {
                    var x0 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i));
                    state = Murmur3x8632Steps.MixStep(x0, state);
                }

                var s0 = UnsafeByteOps.PartialToUInt32(ref data, (uint)length, (uint)fullBlockEndIndex);
                return (int)Murmur3x8632Steps.Finish(state, s0, (uint)length);
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

                    return (int)Murmur3x8632Steps.Finish(this.state, this.buffer, this.length);
                }
            }

            private void MixStep()
            {
                this.state = Murmur3x8632Steps.MixStep(this.buffer, this.state);
                this.length += 4;
                this.bufferIdx = 0;
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1)
            {
                var v1 = value1?.GetHashCode() ?? 0;
                Murmur3x8632Steps.Initialize(DefaultSeed, out var state);
                state = Murmur3x8632Steps.MixStep((uint)v1, state);
                return (int)Murmur3x8632Steps.FinishWithoutPartial(state, sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var v1 = value1?.GetHashCode() ?? 0;
                var v2 = value2?.GetHashCode() ?? 0;
                Murmur3x8632Steps.Initialize(DefaultSeed, out var state);
                state = Murmur3x8632Steps.MixStep((uint)v1, state);
                state = Murmur3x8632Steps.MixStep((uint)v2, state);
                return (int)Murmur3x8632Steps.FinishWithoutPartial(state, 2 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                Murmur3x8632Steps.Initialize(DefaultSeed, out var state);
                state = Murmur3x8632Steps.MixStep(v1, state);
                state = Murmur3x8632Steps.MixStep(v2, state);
                state = Murmur3x8632Steps.MixStep(v3, state);
                return (int)Murmur3x8632Steps.FinishWithoutPartial(state, 3 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var v1 = value1?.GetHashCode() ?? 0;
                var v2 = value2?.GetHashCode() ?? 0;
                var v3 = value3?.GetHashCode() ?? 0;
                var v4 = value4?.GetHashCode() ?? 0;
                Murmur3x8632Steps.Initialize(DefaultSeed, out var state);
                state = Murmur3x8632Steps.MixStep((uint)v1, state);
                state = Murmur3x8632Steps.MixStep((uint)v2, state);
                state = Murmur3x8632Steps.MixStep((uint)v3, state);
                state = Murmur3x8632Steps.MixStep((uint)v4, state);
                return (int)Murmur3x8632Steps.FinishWithoutPartial(state, 4 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var v1 = value1?.GetHashCode() ?? 0;
                var v2 = value2?.GetHashCode() ?? 0;
                var v3 = value3?.GetHashCode() ?? 0;
                var v4 = value4?.GetHashCode() ?? 0;
                var v5 = value5?.GetHashCode() ?? 0;
                Murmur3x8632Steps.Initialize(DefaultSeed, out var state);
                state = Murmur3x8632Steps.MixStep((uint)v1, state);
                state = Murmur3x8632Steps.MixStep((uint)v2, state);
                state = Murmur3x8632Steps.MixStep((uint)v3, state);
                state = Murmur3x8632Steps.MixStep((uint)v4, state);
                state = Murmur3x8632Steps.MixStep((uint)v5, state);
                return (int)Murmur3x8632Steps.FinishWithoutPartial(state, 5 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
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
                Murmur3x8632Steps.Initialize(DefaultSeed, out var state);
                state = Murmur3x8632Steps.MixStep((uint)v1, state);
                state = Murmur3x8632Steps.MixStep((uint)v2, state);
                state = Murmur3x8632Steps.MixStep((uint)v3, state);
                state = Murmur3x8632Steps.MixStep((uint)v4, state);
                state = Murmur3x8632Steps.MixStep((uint)v5, state);
                state = Murmur3x8632Steps.MixStep((uint)v6, state);
                state = Murmur3x8632Steps.MixStep((uint)v7, state);
                state = Murmur3x8632Steps.MixStep((uint)v8, state);
                return (int)Murmur3x8632Steps.FinishWithoutPartial(state, 8 * sizeof(int));
            }
        }
    }
}
