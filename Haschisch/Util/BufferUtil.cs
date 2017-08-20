using System.Runtime.CompilerServices;

namespace Haschisch.Util
{
    internal static class BufferUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static uint BufferSize<T>(ref T buffer)
            where T : struct =>
            (uint)Unsafe.SizeOf<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static uint Append<TBuffer, TValue>(ref TBuffer buffer, ref uint bufferIdx, ref TValue value, uint offset)
            where TBuffer : struct
            where TValue : struct =>
            AppendRaw(ref buffer, ref bufferIdx, ref value, offset, (uint)Unsafe.SizeOf<TValue>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static uint AppendRaw<TBuffer, TValue>(ref TBuffer buffer, ref uint bufferIdx, ref TValue value, uint offset, uint valueLength)
            where TBuffer : struct
            where TValue : struct
        {
            var bufferSize = BufferSize(ref buffer);
            if (bufferIdx == bufferSize) { return 0; }

            var requested = valueLength - offset;
            var available = bufferSize - bufferIdx;
            var toWrite = requested < available ? requested : available;

            Unsafe.CopyBlock(
                Unsafe.AsPointer(ref Unsafe.Add<byte>(ref Unsafe.As<TBuffer, byte>(ref buffer), (int)bufferIdx)),
                Unsafe.AsPointer(ref Unsafe.Add<byte>(ref Unsafe.As<TValue, byte>(ref value), (int)offset)),
                toWrite);

            bufferIdx += toWrite;

            return toWrite;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ZeroUnusedBuffer<T>(ref T buffer, uint bufferIdx)
            where T : struct =>
            Unsafe.InitBlock(
                Unsafe.AsPointer(ref Unsafe.Add<byte>(ref Unsafe.As<T, byte>(ref buffer), (int)bufferIdx)),
                0,
                BufferSize(ref buffer) - bufferIdx);
    }
}
