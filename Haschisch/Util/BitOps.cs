using System.Runtime.CompilerServices;

namespace Haschisch.Util
{
    internal static class BitOps
    {
        // All RotateLefts are expected to be compiled down to a single 'rol' instruction
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotateLeft(int value, int steps) =>
            (int)RotateLeft((uint)value, steps);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint value, int steps) =>
            (value << steps) | (value >> ((sizeof(uint) * 8) - steps));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, int steps) =>
            (value >> steps) | (value << ((sizeof(uint) * 8) - steps));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RotateLeft(long value, int steps) =>
            (long)RotateLeft((ulong)value, steps);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong value, int steps) =>
            (value << steps) | (value >> ((sizeof(ulong) * 8) - steps));
    }
}
