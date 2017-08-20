using System;
using System.Linq;
using FsCheck;
using Haschisch.Tests;

namespace Haschisch.Benchmarks.Data
{
    public static class Generate
    {
        public static Large[] LargeItems(int dataSize, int itemCount, bool reproducible)
        {
            var stdGen = reproducible ? FsCheck.Random.mkStdGen(1234) : FsCheck.Random.newSeed();
            var generator = Tests.Generate.Large();
            return Seq.Unfold(stdGen, state =>
                {
                    var item = Gen.Eval(dataSize, state, generator);
                    var (_, nextState) = FsCheck.Random.stdNext(state);
                    return (nextState, item);
                })
                .Distinct()
                .Take(itemCount)
                .ToArray();
        }
    }
}
