using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class HalfSip13Hasher
    {
        internal static readonly (uint, uint) DefaultKey = GetNewSeed();

        public static (uint, uint) GetNewSeed()
        {
            Seeder.GetNewSeed(out (uint, uint) result);
            return result;
        }

        public struct Block :
            ISeedableBlockHasher<int, (uint, uint)>,
            IBlockHasher<int>,
            IUnsafeBlockHasher<int>,
            ISeedableBlockHasher
        {
            public byte[] GetZeroSeed() => new byte[sizeof(uint) * 2];

            public byte[] Hash(byte[] seed, byte[] data, int offset, int length) =>
                BitConverter.GetBytes(this.Hash((BitConverter.ToUInt32(seed, 0), BitConverter.ToUInt32(seed, sizeof(uint))), data, offset, length));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Hash(byte[] data, int offset, int length) =>
                this.Hash(DefaultKey, data, offset, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Hash((uint, uint) seed, byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length != 0 ?
                    Hash(seed, ref data[offset], length) :
                    Hash(seed, ref empty, length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe int Hash(ref byte data, int length) =>
                Hash(DefaultKey, ref data, length);

            private unsafe static int Hash((uint, uint) seed, ref byte data, int length)
            {
                HalfSip13Steps.Initialize(seed, out var v0, out var v1, out var v2, out var v3);

                var lastIndex = length;
                var bigBlockEnd = length - (length % (8 * sizeof(uint)));
                var fullBlockEnd = length - (length % sizeof(uint));

                var pv0 = v0;
                var pv1 = v1;
                var pv2 = v2;
                var pv3 = v3;

                for (var i = 0; i < bigBlockEnd; i += 8 * sizeof(uint))
                {
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i)));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + sizeof(uint))));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (2 * sizeof(uint)))));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (3 * sizeof(uint)))));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (4 * sizeof(uint)))));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (5 * sizeof(uint)))));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (6 * sizeof(uint)))));
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (7 * sizeof(uint)))));
                }

                for (var i = bigBlockEnd; i < fullBlockEnd; i += sizeof(uint))
                {
                    HalfSip13Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i)));
                }

                v0 = pv0;
                v1 = pv1;
                v2 = pv2;
                v3 = pv3;

                return HalfSip13Steps.Finish(
                    ref v0, ref v1, ref v2, ref v3,
                    UnsafeByteOps.PartialToUInt32(ref data, (uint)lastIndex, (uint)fullBlockEnd),
                    (uint)length);
            }
        }

        public struct Stream : ISeedableStreamingHasher<int, (uint, uint)>
        {
            private uint v0;
            private uint v1;
            private uint v2;
            private uint v3;
            private uint length;
            private uint buffer;
            private uint bufferIdx;

            bool IStreamingHasherSink.AllowUnsafeWrite => false;

            unsafe int IStreamingHasherSink.UnsafeWrite(ref byte value, int maxLength) => throw new NotSupportedException();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize() => this.Initialize(DefaultKey);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize((uint, uint) seed)
            {
                this.length = 0;
                this.bufferIdx = 0;
                HalfSip13Steps.Initialize(seed, out this.v0, out this.v1, out this.v2, out this.v3);
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
                    return HalfSip13Steps.Finish(ref this.v0, ref this.v1, ref this.v2, ref this.v3, this.buffer, this.length);
                }
            }

            private void MixStep()
            {
                HalfSip13Steps.SipCRound(ref this.v0, ref this.v1, ref this.v2, ref this.v3, this.buffer);
                this.length += 4;
                this.bufferIdx = 0;
            }
        }

        public struct Combiner : IHashCodeCombiner
        {
            public int Combine<T1>(T1 value1)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                HalfSip13Steps.Initialize(DefaultKey, out var v0, out var v1, out var v2, out var v3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1);
                return HalfSip13Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, sizeof(int));
            }

            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                HalfSip13Steps.Initialize(DefaultKey, out var v0, out var v1, out var v2, out var v3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x2);
                return HalfSip13Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 2 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                HalfSip13Steps.Initialize(DefaultKey, out var v0, out var v1, out var v2, out var v3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x2);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x4);
                return HalfSip13Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 4 * sizeof(int));
            }

            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var x1 = value1?.GetHashCode() ?? 0;
                var x2 = value2?.GetHashCode() ?? 0;
                var x3 = value3?.GetHashCode() ?? 0;
                var x4 = value4?.GetHashCode() ?? 0;
                var x5 = value5?.GetHashCode() ?? 0;
                HalfSip13Steps.Initialize(DefaultKey, out var v0, out var v1, out var v2, out var v3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x2);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x4);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x5);
                return HalfSip13Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 5 * sizeof(int));
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
                HalfSip13Steps.Initialize(DefaultKey, out var v0, out var v1, out var v2, out var v3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x1);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x2);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x3);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x4);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x5);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x6);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x7);
                HalfSip13Steps.SipCRound(ref v0, ref v1, ref v2, ref v3, (uint)x8);
                return HalfSip13Steps.Finish(ref v0, ref v1, ref v2, ref v3, 0, 8 * sizeof(int));
            }
        }
    }
}
