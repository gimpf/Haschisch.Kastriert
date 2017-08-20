using System;
using System.Collections.Generic;

namespace Haschisch.Benchmarks.Data
{
    public static class Seq
    {
        public static IEnumerable<TItem> Unfold<TState, TItem>(TState? state, Func<TState, ValueTuple<TState?, TItem>> generator)
            where TState : struct
        {
            while (state != null)
            {
                var (nextState, item) = generator(state.Value);
                yield return item;
                state = nextState;
            }
        }

        public static IEnumerable<TItem> Unfold<TState, TItem>(TState state, Func<TState, ValueTuple<TState, TItem>> generator)
            where TState : class
        {
            while (state != null)
            {
                var (nextState, item) = generator(state);
                yield return item;
                state = nextState;
            }
        }
    }
}
