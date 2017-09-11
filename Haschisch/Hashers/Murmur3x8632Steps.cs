using System.Runtime.CompilerServices;
using Haschisch.Util;

namespace Haschisch.Hashers
{
    internal static class Murmur3x8632Steps
    {
        internal const uint C1 = 0xcc9e2d51;
        internal const uint C2 = 0x1b873593;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(uint seed, out uint state)
        {
            state = seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MixStep(uint value, uint state)
        {
            value *= C1;
            value = BitOps.RotateLeft(value, 15);
            value *= C2;

            state ^= value;
            state = BitOps.RotateLeft(state, 13);
            state = (state * 5) + 0xe6546b64;

            return state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MixFinalPartial(ref uint state, uint partial, uint length)
        {
            uint stateUpdate = 0;
            switch (length % sizeof(uint))
            {
                case 3: stateUpdate ^= partial & 0x00FF0000; goto case 2;
                case 2: stateUpdate ^= partial & 0x0000FF00; goto case 1;
                case 1: stateUpdate ^= partial & 0x000000FF;
                    stateUpdate *= C1;
                    stateUpdate = BitOps.RotateLeft(stateUpdate, 15);
                    stateUpdate *= C2;
                    state ^= stateUpdate;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FinishWithoutPartial(uint state, uint length)
        {
            state ^= length;
            return FMix32(state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Finish(uint state, uint partial, uint length)
        {
            MixFinalPartial(ref state, partial, length);
            return FinishWithoutPartial(state, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint FMix32(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;

            return h;
        }
    }
}
