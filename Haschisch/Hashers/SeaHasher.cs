using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class SeaHasher
    {
        public struct Block :
            IBlockHasher<int>,
            IBlockHasher<long>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Hash(byte[] data, int offset, int length)
            {
                Require.ValidRange(data, offset, length);

                byte empty = 0;
                return length != 0 ?
                    this.Hash(ref data[offset], length) :
                    this.Hash(ref empty, length);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IBlockHasher<int>.Hash(byte[] data, int offset, int length) =>
                (int)this.Hash(data, offset, length);

            private long Hash(ref byte data, int length)
            {
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);

                var indexEnd = length;
                var indexFullBlockEnd = indexEnd - (length % SeaHashSteps.FullBlockSize);

                var ia = a;
                var ib = b;
                var ic = c;
                var id = d;

                for (var i = 0; i < indexFullBlockEnd; i += SeaHashSteps.FullBlockSize)
                {
                    var x0 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i));
                    var x1 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i + sizeof(ulong)));
                    var x2 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i + (2 * sizeof(ulong))));
                    var x3 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i + (3 * sizeof(ulong))));

                    SeaHashSteps.MixStep(ref ia, ref ib, ref ic, ref id, x0, x1, x2, x3);
                }

                a = ia;
                b = ib;
                c = ic;
                d = id;

                var indexFullWordEnd = indexEnd - (length % sizeof(ulong));
                for (var i = indexFullBlockEnd; i < indexFullWordEnd; i += sizeof(ulong))
                {
                    var x0 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i));
                    SeaHashSteps.MixStep(ref a, ref b, ref c, ref d, x0);
                }

                if (indexFullWordEnd != indexEnd)
                {
                    SeaHashSteps.MixStep(
                        ref a,
                        ref b,
                        ref c,
                        ref d,
                        UnsafeByteOps.PartialToUInt64(ref data, indexFullWordEnd, (uint)(indexEnd - indexFullWordEnd)));
                }

                return (long)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, (uint)length);
            }
        }

        public struct Stream : IStreamingHasher<long>, IStreamingHasher<int>
        {
            private ulong buffer;
            private uint bufferIdx;
            private uint length;
            private ulong a;
            private ulong b;
            private ulong c;
            private ulong d;

            bool IStreamingHasherSink.AllowUnsafeWrite => false;

            unsafe int IStreamingHasherSink.UnsafeWrite(ref byte value, int maxLength) => throw new NotSupportedException();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize()
            {
                this.bufferIdx = 0;
                this.length = 0;
                SeaHashSteps.Initialize(out this.a, out this.b, out this.c, out this.d);
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
                unchecked
                {
                    if (this.bufferIdx > 0)
                    {
                        BufferUtil.ZeroUnusedBuffer(ref this.buffer, this.bufferIdx);
                        this.MixStep();
                    }

                    return (long)SeaHashSteps.Finish(ref this.a, ref this.b, ref this.c, ref this.d, this.length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IStreamingHasher<int>.Finish() => (int)this.Finish();

            private void MixStep()
            {
                SeaHashSteps.MixStep(ref this.a, ref this.b, ref this.c, ref this.d, this.buffer);
                this.length += this.bufferIdx;
                this.bufferIdx = 0;
            }
        }
    }
}
