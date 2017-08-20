using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class HalfSip24Hasher
    {
        internal static readonly (uint, uint) DefaultKey = GetNewSeed();

        public static (uint, uint) GetNewSeed() => HalfSip13Hasher.GetNewSeed();

        public struct Block :
            ISeedableBlockHasher<int, (uint, uint)>,
            IBlockHasher<int>,
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

            private static int Hash((uint, uint) seed, ref byte data, int length)
            {
                HalfSip24Steps.Initialize(seed, out var v0, out var v1, out var v2, out var v3);

                var lastIndex = length;
                var bigBlockEnd = length - (length % (8 * sizeof(uint)));
                var fullBlockEnd = length - (length % sizeof(uint));

                var pv0 = v0;
                var pv1 = v1;
                var pv2 = v2;
                var pv3 = v3;
                for (var i = 0; i < bigBlockEnd; i += 8 * sizeof(uint))
                {
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i)));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + sizeof(uint))));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (2 * sizeof(uint)))));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (3 * sizeof(uint)))));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (4 * sizeof(uint)))));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (5 * sizeof(uint)))));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (6 * sizeof(uint)))));
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i + (7 * sizeof(uint)))));
                }

                for (var i = bigBlockEnd; i < fullBlockEnd; i += sizeof(uint))
                {
                    HalfSip24Steps.SipCRound(ref pv0, ref pv1, ref pv2, ref pv3, Unsafe.As<byte, uint>(ref Unsafe.Add(ref data, i)));
                }

                v0 = pv0;
                v1 = pv1;
                v2 = pv2;
                v3 = pv3;

                return HalfSip24Steps.Finish(ref v0, ref v1, ref v2, ref v3, UnsafeByteOps.PartialToUInt32(ref data, fullBlockEnd, (uint)(lastIndex - fullBlockEnd)), (uint)length);
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
                HalfSip24Steps.Initialize(seed, out this.v0, out this.v1, out this.v2, out this.v3);
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
                    return HalfSip24Steps.Finish(ref this.v0, ref this.v1, ref this.v2, ref this.v3, this.buffer, this.length);
                }
            }

            private void MixStep()
            {
                HalfSip24Steps.SipCRound(ref this.v0, ref this.v1, ref this.v2, ref this.v3, this.buffer);
                this.length += 4;
                this.bufferIdx = 0;
            }
        }
    }
}
