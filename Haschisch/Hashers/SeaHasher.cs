﻿using System;
using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    public static class SeaHasher
    {
        public struct Block :
            IBlockHasher<int>,
            IBlockHasher<long>,
            IUnsafeBlockHasher<int>,
            IUnsafeBlockHasher<long>
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int IUnsafeBlockHasher<int>.Hash(ref byte data, int length) =>
                (int)this.Hash(ref data, length);

            public long Hash(ref byte data, int length)
            {
                // TODO this should use the newly generated seed always; use proper seeding to validate test-vectors
                SeaHashSteps.InitializeForTestVectors(out var a, out var b, out var c, out var d);

                var indexFullBlockEnd = length & ~(SeaHashSteps.FullBlockSize - 1);

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

                var indexFullWordEnd = length & ~(sizeof(ulong) - 1);
                for (var i = indexFullBlockEnd; i < indexFullWordEnd; i += sizeof(ulong))
                {
                    var x0 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i));
                    SeaHashSteps.MixStep(ref a, ref b, ref c, ref d, x0);
                }

                if (indexFullWordEnd != length)
                {
                    SeaHashSteps.MixStep(
                        ref a,
                        ref b,
                        ref c,
                        ref d,
                        UnsafeByteOps.PartialToUInt64(ref data, (uint)length, (uint)indexFullWordEnd));
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

            public bool AllowUnsafeWrite => true;

            public unsafe int UnsafeWrite(ref byte data, int maxLength)
            {
                if (this.bufferIdx != 0) { throw new NotSupportedException("unaligned block hashing not supported"); }

                var indexFullBlockEnd = maxLength & ~(SeaHashSteps.FullBlockSize - 1);

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

                var indexFullWordEnd = maxLength & ~(sizeof(ulong) - 1);
                for (var i = indexFullBlockEnd; i < indexFullWordEnd; i += sizeof(ulong))
                {
                    var x0 = Unsafe.As<byte, ulong>(ref Unsafe.Add(ref data, i));
                    SeaHashSteps.MixStep(ref a, ref b, ref c, ref d, x0);
                }

                if (indexFullWordEnd != maxLength)
                {
                    BufferUtil.AppendRaw(
                        ref this.buffer, ref this.bufferIdx, ref data, (uint)indexFullWordEnd, (uint)maxLength);
                }

                this.length += (uint)maxLength;
                return maxLength;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Initialize()
            {
                this.bufferIdx = 0;
                this.length = 0;
                SeaHashSteps.InitializeForTestVectors(out this.a, out this.b, out this.c, out this.d);
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

        public struct Combiner : IHashCodeCombiner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1>(T1 value1)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixFinalPartialStep(ref a, v1);
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2>(T1 value1, T2 value2)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixFinalPartialStep(ref a, ((ulong)v2 << 32) | v1);
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 2 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixFinalPartialStep(
                    ref a,
                    ref b,
                    v1 | ((ulong)v2 << 32),
                    v3);
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 3 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixFinalPartialStep(
                    ref a,
                    ref b,
                    v1 | ((ulong)v2 << 32),
                    v3 | ((ulong)v4 << 32));
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 4 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixFinalPartialStep(
                    ref a,
                    ref b,
                    ref c,
                    v1 | ((ulong)v2 << 32),
                    v3 | ((ulong)v4 << 32),
                    v5);
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 5 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);
                var v6 = (uint)(value6?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixFinalPartialStep(
                    ref a,
                    ref b,
                    ref c,
                    v1 | ((ulong)v2 << 32),
                    v3 | ((ulong)v4 << 32),
                    v5 | ((ulong)v6 << 32));
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 6 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);
                var v6 = (uint)(value6?.GetHashCode() ?? 0);
                var v7 = (uint)(value7?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixStep(
                    ref a,
                    ref b,
                    ref c,
                    ref d,
                    v1 | ((ulong)v2 << 32),
                    v3 | ((ulong)v4 << 32),
                    v5 | ((ulong)v6 << 32),
                    v7);
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 7 * sizeof(int));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
                T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
            {
                var v1 = (uint)(value1?.GetHashCode() ?? 0);
                var v2 = (uint)(value2?.GetHashCode() ?? 0);
                var v3 = (uint)(value3?.GetHashCode() ?? 0);
                var v4 = (uint)(value4?.GetHashCode() ?? 0);
                var v5 = (uint)(value5?.GetHashCode() ?? 0);
                var v6 = (uint)(value6?.GetHashCode() ?? 0);
                var v7 = (uint)(value7?.GetHashCode() ?? 0);
                var v8 = (uint)(value8?.GetHashCode() ?? 0);
                SeaHashSteps.Initialize(out var a, out var b, out var c, out var d);
                SeaHashSteps.MixStep(
                    ref a,
                    ref b,
                    ref c,
                    ref d,
                    v1 | ((ulong)v2 << 32),
                    v3 | ((ulong)v4 << 32),
                    v5 | ((ulong)v6 << 32),
                    v7 | ((ulong)v8 << 32));
                return (int)SeaHashSteps.Finish(ref a, ref b, ref c, ref d, 8 * sizeof(int));
            }
        }
    }
}
