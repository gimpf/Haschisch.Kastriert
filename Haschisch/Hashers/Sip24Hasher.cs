using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class Sip24Hasher
    {
        private static readonly (ulong, ulong) DefaultSeed = GetNewSeed();

        public static (ulong, ulong) GetNewSeed()
        {
            Seeder.GetNewSeed(out (ulong, ulong) result);
            return result;
        }

        public struct Block :
            ISeedableBlockHasher<long, (ulong, ulong)>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<int>,
            ISeedableBlockHasher
        {
            public byte[] GetZeroSeed() => new byte[sizeof(ulong) * 2];

            public byte[] Hash(byte[] seed, byte[] data, int offset, int length) =>
                BitConverter.GetBytes(this.Hash(
                    (BitConverter.ToUInt64(seed, 0), BitConverter.ToUInt64(seed, sizeof(ulong))),
                    data,
                    offset,
                    length));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Hash(byte[] data, int offset, int length) =>
                this.Hash(DefaultSeed, data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Hash((ulong, ulong) seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length != 0 ?
                    Hash(seed, ref data[offset], length) :
                    Hash(seed, ref empty, length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe long Hash(ref byte data, int length) =>
                Hash(DefaultSeed, ref data, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IUnsafeBlockHasher<int>.Hash(ref byte data, int length) =>
                (int)Hash(DefaultSeed, ref data, length);

            private unsafe long Hash((ulong, ulong) seed, ref byte data, int length)
            {
                Sip24Steps.Initialize(seed, out var v0, out var v1, out var v2, out var v3);

                var lastIndex = length;
                var fullBlockEnd = length & ~(sizeof(ulong) - 1);

                for (var i = 0; i < fullBlockEnd; i += sizeof(ulong))
                {
                    Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, Unsafe.As<byte, ulong>(ref Unsafe.Add<byte>(ref data, i)));
                }

                return Sip24Steps.Finish(
                    ref v0, ref v1, ref v2, ref v3,
                    UnsafeByteOps.PartialToUInt64(ref data, (uint)lastIndex, (uint)fullBlockEnd),
                    (ulong)length);
            }
        }

        public struct Stream :
            ISeedableStreamingHasher<long, (ulong, ulong)>,
            ISeedableStreamingHasher<int, (ulong, ulong)>
        {
            private ulong v0;
            private ulong v1;
            private ulong v2;
            private ulong v3;
            private ulong buffer;
            private uint bufferIdx;
            private uint length;

            bool IStreamingHasherSink.AllowUnsafeWrite => false;

            unsafe int IStreamingHasherSink.UnsafeWrite(ref byte value, int maxLength) => throw new NotSupportedException();

            public void Initialize() => this.Initialize(DefaultSeed);

            public void Initialize((ulong, ulong) seed)
            {
                this.bufferIdx = 0;
                this.length = 0;
                Sip24Steps.Initialize(seed, out this.v0, out this.v1, out this.v2, out this.v3);
            }

            public void Write8(byte value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(byte)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            public void Write16(short value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(short)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            public void Write32(int value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(int)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            public void Write64(long value)
            {
                var written = BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, 0);
                if (written < sizeof(long)) { this.MixStep(); BufferUtil.Append(ref this.buffer, ref this.bufferIdx, ref value, written); }
            }

            public long Finish()
            {
                if (this.bufferIdx == BufferUtil.BufferSize(ref this.buffer))
                {
                    this.MixStep();
                }

                BufferUtil.ZeroUnusedBuffer(ref this.buffer, this.bufferIdx);
                this.length += (uint)this.bufferIdx;

                return Sip24Steps.Finish(ref this.v0, ref this.v1, ref this.v2, ref this.v3, this.buffer, this.length);
            }

            int IStreamingHasher<int>.Finish() => (int)this.Finish();

            private void MixStep()
            {
                Sip24Steps.SipCRound(ref this.v0, ref this.v1, ref this.v2, ref this.v3, this.buffer);
                this.length += 8;
                this.bufferIdx = 0;
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                Sip24Steps.Initialize(DefaultSeed, out var v0, out var v1, out var v2, out var v3);
                return (int)Sip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, (uint)x1, sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                Sip24Steps.Initialize(DefaultSeed, out var v0, out var v1, out var v2, out var v3);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1 | (ulong)x2 << 32);
                return (int)Sip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 2 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var x1 = (uint)(value1?.GetHashCode() ?? 0);
                var x2 = (uint)(value2?.GetHashCode() ?? 0);
                var x3 = (uint)(value3?.GetHashCode() ?? 0);
                Sip24Steps.Initialize(DefaultSeed, out var v0, out var v1, out var v2, out var v3);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, x1 | (ulong)x2 << 32);
                return (int)Sip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, x3, 3 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                Sip24Steps.Initialize(DefaultSeed, out var v0, out var v1, out var v2, out var v3);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1 | (ulong)x2 << 32);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x3 | (ulong)x4 << 32);
                return (int)Sip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 4 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                var x5 = value5?.GetHashCode() ?? 0;
                Sip24Steps.Initialize(DefaultSeed, out var v0, out var v1, out var v2, out var v3);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1 | (ulong)x2 << 32);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x3 | (ulong)x4 << 32);
                return (int)Sip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, (uint)x5, 5 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                Sip24Steps.Initialize(DefaultSeed, out var v0, out var v1, out var v2, out var v3);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1 | (ulong)x2 << 32);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x3 | (ulong)x4 << 32);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x5 | (ulong)x6 << 32);
                Sip24Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x7 | (ulong)x8 << 32);
                return (int)Sip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 8 * sizeof(int));
            }
        }

    }
}
