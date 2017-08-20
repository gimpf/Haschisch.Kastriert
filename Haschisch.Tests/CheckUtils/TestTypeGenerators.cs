namespace Haschisch.Tests
{
    using System;
    using System.Linq;
    using FsCheck;

    public static class Generate
    {
        public static Gen<Small> Small() =>
            Gen.Sized(sizedGen: size =>
                Gen.Choose(0, size + 1)
                    .Three()
                    .Select(sa => new Small(sa.Item1, sa.Item2, sa.Item3)));

        public static Gen<ValueTuple<Small, Small>> SmallPair() =>
            Gen.OneOf(
                Small().Select(x => ValueTuple.Create(x, x)),
                Small().Two().Select(v => v.ToValueTuple()));

        public static Gen<Medium> Medium() =>
            Gen.Sized(sizedGen: size =>
                Small().SelectMany(s =>
                    Gen.Elements(Enumerable.Range(0, size + 1).Select(length => new string('a', length)).ToArray())
                        .Select(t => new Medium(s, t))));

        public static Gen<ValueTuple<Medium, Medium>> MediumPair() =>
            Gen.OneOf(
                Medium().Select(x => ValueTuple.Create(x, x)),
                Medium().Two().Select(v => v.ToValueTuple()));

        public static Gen<Large> Large() =>
            Medium().SelectMany(m =>
                Gen.ArrayOf(Arb.Generate<int>()).Select(l =>
                    new Large(m, l)));

        public static Gen<ValueTuple<Large, Large>> LargePair() =>
            Gen.OneOf(
                Large().Select(x => ValueTuple.Create(x, x)),
                Large().Two().Select(v => v.ToValueTuple()));

        public static Gen<IHashable> IHashable()
        {
            return Gen.OneOf(
                Small().Select(x => x as IHashable),
                Medium().Select(x => x as IHashable),
                Large().Select(x => x as IHashable));
        }

        public static Gen<ValueTuple<IHashable, IHashable>> IHashablePair() =>
            Gen.OneOf(
                IHashable().Select(x => ValueTuple.Create(x, x)),
                IHashable().Two().Select(v => v.ToValueTuple()));
    }

    public class ArbitraryTestData
    {
        public static Arbitrary<Small> Small() => Arb.From(Generate.Small());
        public static Arbitrary<Medium> Medium() => Arb.From(Generate.Medium());
        public static Arbitrary<Large> Large() => Arb.From(Generate.Large());
        public static Arbitrary<IHashable> IHashable() => Arb.From(Generate.IHashable());
        public static Arbitrary<ValueTuple<IHashable, IHashable>> IHashablePair() => Arb.From(Generate.IHashablePair());
    }
}
