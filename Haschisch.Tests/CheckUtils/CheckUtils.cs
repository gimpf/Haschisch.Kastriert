using System;
using FsCheck;

namespace Haschisch.Tests
{
    public static class DoCheck
    {
        private static Configuration cfg;

        static DoCheck()
        {
            cfg = Configuration.QuickThrowOnFailure;
            cfg.MaxNbOfTest = 100;
            cfg.MaxNbOfFailedTests = 1000000;
            cfg.StartSize = 0;
            cfg.EndSize = 10;
        }

        public static void That<T>(Func<T, bool> p) => That(Prop.ForAll(Arb.From<T>(), p));

        public static void That<T>(Func<T, Property> p) => That(Prop.ForAll(Arb.From<T>(), p));

        public static void That<T>(Arbitrary<T> a, Func<T, Property> p) => That(Prop.ForAll(a, p));

        public static void That<T1, T2>(Func<T1, T2, bool> p) => That(Prop.ForAll(Arb.From<T1>(), Arb.From<T2>(), p));

        public static void That(Property p) => p.Check(cfg);
    }
}