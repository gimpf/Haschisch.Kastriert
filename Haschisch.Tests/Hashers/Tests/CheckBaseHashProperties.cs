using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Haschisch.Tests;
using NUnit.Framework;

namespace Haschisch.Hashers.Tests
{
    [TestFixture]
    public class CheckBaseHashProperties
    {
        static CheckBaseHashProperties()
        {
            Arb.Register<ArbitraryTestData>();
        }

        public struct HashAlgorithm
        {
            public string Name;
            public IStreamingHasher<int>[] Streaming;
            public IBlockHasher<int>[] Block;
            public IHashCodeCombiner[] Combiner;

            public static HashAlgorithm Create(
                string name, IStreamingHasher<int>[] streaming, IBlockHasher<int>[] block, IHashCodeCombiner[] combiner)
            {
                return new HashAlgorithm
                {
                    Name = name,
                    Streaming = streaming,
                    Block = block,
                    Combiner = combiner
                };
            }

            public override string ToString() => this.Name;
        }

        public static HashAlgorithm[] Algorithms =>
            new HashAlgorithm[]
            {
                // Marvin Implementations
                HashAlgorithm.Create(
                    "marvin32",
                    Stream(default(Marvin32Hasher.Stream)),
                    Block(default(Marvin32Hasher.Block)),
                    Combiner(default(Marvin32Hasher.Combiner))),
                HashAlgorithm.Create(
                    "murmur3-x86-32",
                    Stream(default(Murmur3x8632Hasher.Stream)),
                    Block(default(Murmur3x8632Hasher.Block)),
                    Combiner(default(Murmur3x8632Hasher.Combiner))),
                HashAlgorithm.Create(
                    "xxhash32",
                    Stream(default(XXHash32Hasher.Stream)),
                    Block(default(XXHash32Hasher.Block)),
                    Combiner(default(XXHash32Hasher.Combiner))),
                HashAlgorithm.Create(
                    "xxhash64",
                    Stream(default(XXHash64Hasher.Stream)),
                    Block(default(XXHash64Hasher.Block)),
                    Combiner(default(XXHash64Hasher.Combiner))),
                HashAlgorithm.Create(
                    "SeaHash",
                    Stream(default(SeaHasher.Stream)),
                    Block(default(SeaHasher.Block)),
                    Combiner()), // TODO: only activate after fixing seeding, default(SeaHasher.Combiner)),
                HashAlgorithm.Create(
                    "hsip13",
                    Stream(default(HalfSip13Hasher.Stream)),
                    Block(default(HalfSip13Hasher.Block)),
                    Combiner(default(HalfSip13Hasher.Combiner))),
                HashAlgorithm.Create(
                    "hsip24",
                    Stream(default(HalfSip24Hasher.Stream)),
                    Block(default(HalfSip24Hasher.Block)),
                    Combiner(default(HalfSip24Hasher.Combiner))),
                HashAlgorithm.Create(
                    "sip13",
                    Stream(default(Sip13Hasher.Stream)),
                    Block(default(Sip13Hasher.Block)),
                    Combiner(default(Sip13Hasher.Combiner))),
                HashAlgorithm.Create(
                    "sip24",
                    Stream(default(Sip24Hasher.Stream)),
                    Block(default(Sip24Hasher.Block)),
                    Combiner(default(Sip24Hasher.Combiner))),
                HashAlgorithm.Create(
                    "spookyv2",
                    Stream(default(SpookyV2Hasher.Stream)),
                    Block(default(SpookyV2Hasher.Block)),
                    Combiner(default(SpookyV2Hasher.Combiner))),
                HashAlgorithm.Create(
                    "city32",
                    Stream(),
                    Block(default(City32Hasher.Block)),
                    Combiner(default(City32OldHasher.CombinerUnseeded), default(City32OldHasher.CombinerNonUnrolled)))
            };

        public static IEnumerable<IEqualityComparer<IHashable>> EqualityComparers =>
            new IEqualityComparer<IHashable>[] { EqualityComparer<IHashable>.Default }
            .Concat(Algorithms.Where(x => x.Streaming.Any()).Select(alg =>
                (IEqualityComparer<IHashable>)Activator.CreateInstance(
                    typeof(HashableEqualityComparer<,>).MakeGenericType(
                        typeof(IHashable),
                        alg.Streaming.First().GetType()))));

        public static IEnumerable<IBlockHasher<int>> Hashers =>
            Algorithms.Select(alg => alg.Block.First());

        [Test]
        [TestCaseSource(nameof(EqualityComparers))]
        public void HashCode_IsEqual_WithEqualValues(IEqualityComparer<IHashable> ec)
        {
            DoCheck.That(
                ArbitraryTestData.IHashablePair(),
                (ValueTuple<IHashable, IHashable> v) =>
                    (ec.GetHashCode(v.Item1) == ec.GetHashCode(v.Item2))
                        .When(ec.Equals(v.Item1, v.Item2)));
        }

        [Test]
        [TestCaseSource(nameof(EqualityComparers))]
        public void HashCode_IsDifferent_WithDifferentValues(IEqualityComparer<IHashable> ec)
        {
            // marvin fails this by construction
            if (ec.GetType() != typeof(HashableEqualityComparer<IHashable, Marvin32Hasher.Stream>))
            {
                // there was a bad bug once w.r.t. empty strings...
                Assert.AreNotEqual(
                    ec.GetHashCode(new Medium(default(Small), string.Empty)),
                    ec.GetHashCode(default(Small)));
            }

            // not strictly necessary, but for our simple test data this really
            // should hold for all supplied prime-time hashers
            DoCheck.That(
                ArbitraryTestData.IHashablePair(),
                (ValueTuple<IHashable, IHashable> v) =>
                    (ec.GetHashCode(v.Item1) != ec.GetHashCode(v.Item2))
                        .When(!ec.Equals(v.Item1, v.Item2)));
        }

        [Test]
        [TestCaseSource(nameof(Hashers))]
        public void HashCode_WithAddedByte_IsDifferent(IBlockHasher<int> hasher)
        {
            DoCheck.That((byte[] data, byte inserted) =>
            {
                var h = HashIt(hasher, data, data.Length);
                return
                    Enumerable.Range(0, data.Length).All(byteIdx =>
                    {
                        var nextData = new byte[data.Length + 1];
                        Array.Copy(data, 0, nextData, 0, byteIdx);
                        nextData[byteIdx] = inserted;
                        Array.Copy(data, byteIdx, nextData, byteIdx + 1, data.Length - byteIdx);
                        return h != HashIt(hasher, nextData, nextData.Length);
                    });
            });
        }

        [Test]
        [TestCaseSource(nameof(Hashers))]
        public void HashCode_WithChangedBit_IsDifferent(IBlockHasher<int> hasher)
        {
            DoCheck.That((byte[] data) =>
            {
                var h = HashIt(hasher, data, data.Length);
                return
                    Enumerable.Range(0, data.Length).SelectMany(byteIdx =>
                    Enumerable.Range(0, 7).Select(bitIdx => ValueTuple.Create(byteIdx, bitIdx)))
                    .All(t =>
                    {
                        var (byteIdx, bitIdx) = t;
                        var nextData = FlipBit(data, byteIdx, bitIdx);
                        return h != HashIt(hasher, nextData, data.Length);
                    });
            });
        }

        [Test]
        [TestCaseSource(nameof(Hashers))]
        public void HashCode_WithNullPadding_IsDifferent(IBlockHasher<int> hasher)
        {
            DoCheck.That((byte[] data) =>
            {
                if ((data?.Length ?? 0) == 0) { return true; }

                var frontPadded = new byte[data.Length + 4];
                Array.Copy(data, 0, frontPadded, 4, data.Length);
                var endPadded = new byte[data.Length + 4];
                Array.Copy(data, 0, endPadded, 0, data.Length);

                var h1 = HashIt(hasher, data, data.Length);
                var h2 = HashIt(hasher, frontPadded, frontPadded.Length);
                var h3 = HashIt(hasher, endPadded, endPadded.Length);
                return h1 != h2 &&
                    h1 != h3 &&
                    (!data.Any(x => x != 0) || h2 != h3); // if all data is 0, adding null at front or back cannot make a difference
            });
        }

        [Test]
        [TestCaseSource(nameof(Algorithms))]
        public void HashCode_OfEquivalentHashers_IsEqual(HashAlgorithm algorithm)
        {
            var data = Enumerable.Range(0, 512).Select(x => (byte)x).ToArray();
            for (var i = 0; i <= data.Length; i++)
            {
                var len = i;
                var hashCodes =
                    algorithm.Streaming.Select(hasher => (hasher.ToString(), HashItBy1(hasher, data, len))).ToArray()
                    .Concat(algorithm.Block.Select(hasher => (hasher.ToString(), HashItBlockwise(hasher, data, 0, len))))
                    .Concat(IsCombinable(len) ? algorithm.Combiner.Select(hasher => (hasher.ToString(), HashItByCombining(hasher, data, i))) : Enumerable.Empty<(string, int)>())
                    .ToArray();

                foreach (var hc in hashCodes.Skip(1))
                {
                    Assert.AreEqual(hashCodes[0].Item2, hc.Item2, "hasher {0} failed for length {1}", hc.Item1, len);
                }
            }

            bool IsCombinable(int length) =>
                algorithm.Combiner != null && (length == 4 || length == 8 || length == 16 || length == 32);
        }

        [Test]
        [TestCaseSource(nameof(Algorithms))]
        public void HashCode_UsingDifferentStreamingMethods_IsEqual(HashAlgorithm algorithm)
        {
            if (!algorithm.Streaming.Any()) { return; }

            var hasher = algorithm.Streaming.First();

            var data = Enumerable.Range(0, 512).Select(x => (byte)x).ToArray();
            for (var i = 0; i <= data.Length; i++)
            {
                var expected = HashItBy1(hasher, data, i);
                Assert.AreEqual(expected, HashItBy2(hasher, data, i), "by2 failed for length {0}", i);
                Assert.AreEqual(expected, HashItBy4(hasher, data, i), "by4 failed for length {0}", i);
                Assert.AreEqual(expected, HashItBy8(hasher, data, i), "by8 failed for length {0}", i);
                Assert.AreEqual(expected, HashItJittery(hasher, data, i), "byJittery failed for length {0}", i);
            }
        }

        [Test]
        [TestCaseSource(nameof(Algorithms))]
        public void HashCode_UsingUnalignedOffsetByBlock_IsEqual(HashAlgorithm algorithm)
        {
            var data = Enumerable.Range(0, 512).Select(x => (byte)x).ToArray();
            var dataUnaligned = new byte[data.Length + 1];
            data[0] = 0xBE; // don't care
            Array.Copy(data, 0, dataUnaligned, 1, data.Length);

            foreach (var hasher in algorithm.Block)
            {
                // many hashes have "small" and "large" modes, with a variety
                // of switch-over values; so test all kinds of lengths to be
                // sure to hit it
                for (var i = 0; i <= data.Length; i++)
                {
                    var expected = HashItBlockwise(hasher, data, 0, i);
                    Assert.AreEqual(expected, HashItBlockwise(hasher, dataUnaligned, 1, i));
                }
            }
        }

        private static int HashItByCombining(IHashCodeCombiner combiner, byte[] data, int maxCount)
        {
            switch (maxCount)
            {
                case 4:
                    return combiner.Combine(
                        BitConverter.ToInt32(data, 0));
                case 8:
                    return combiner.Combine(
                        BitConverter.ToInt32(data, 0),
                        BitConverter.ToInt32(data, 4));
                case 16:
                    return combiner.Combine(
                        BitConverter.ToInt32(data, 0),
                        BitConverter.ToInt32(data, 4),
                        BitConverter.ToInt32(data, 8),
                        BitConverter.ToInt32(data, 12));
                case 32:
                    return combiner.Combine(
                        BitConverter.ToInt32(data, 0),
                        BitConverter.ToInt32(data, 4),
                        BitConverter.ToInt32(data, 8),
                        BitConverter.ToInt32(data, 12),
                        BitConverter.ToInt32(data, 16),
                        BitConverter.ToInt32(data, 20),
                        BitConverter.ToInt32(data, 24),
                        BitConverter.ToInt32(data, 28));
                default:
                    throw new ArgumentOutOfRangeException(nameof(maxCount));
            }
        }

        private static int HashIt(IBlockHasher<int> hasher, byte[] data, int maxCount)
        {
            return hasher.Hash(data, 0, maxCount);
        }

        private static int HashItBy1(IStreamingHasher<int> hasher, byte[] data, int maxCount)
        {
            hasher.Initialize();
            for (var i = 0; i < maxCount; i++) { hasher.Write8(data[i]); }
            return hasher.Finish();
        }

        private static int HashItBy2(IStreamingHasher<int> hasher, byte[] data, int maxCount)
        {
            hasher.Initialize();
            var blockEnd = maxCount - (maxCount % 2);
            for (var i = 0; i < blockEnd; i += 2) { hasher.Write16((short)(data[i] | (data[i + 1] << 8))); }
            for (var i = blockEnd; i < maxCount; i++) { hasher.Write8(data[i]); }
            return hasher.Finish();
        }

        private static int HashItBy4(IStreamingHasher<int> hasher, byte[] data, int maxCount)
        {
            hasher.Initialize();
            var bigBlockEnd = maxCount - (maxCount % 4);
            var blockEnd = maxCount - (maxCount % 2);
            for (var i = 0; i < bigBlockEnd; i += 4) { hasher.Write32(BitConverter.ToInt32(data, i)); }
            for (var i = bigBlockEnd; i < blockEnd; i += 2) { hasher.Write16((short)(data[i] | (data[i + 1] << 8))); }
            for (var i = blockEnd; i < maxCount; i++) { hasher.Write8(data[i]); }
            return hasher.Finish();
        }

        private static int HashItBy8(IStreamingHasher<int> hasher, byte[] data, int maxCount)
        {
            hasher.Initialize();
            var biggestBlockEnd = maxCount - (maxCount % 8);
            var bigBlockEnd = maxCount - (maxCount % 4);
            var blockEnd = maxCount - (maxCount % 2);
            for (var i = 0; i < biggestBlockEnd; i += 8) { hasher.Write64(BitConverter.ToInt64(data, i)); }
            for (var i = biggestBlockEnd; i < bigBlockEnd; i += 4) { hasher.Write32(BitConverter.ToInt32(data, i)); }
            for (var i = bigBlockEnd; i < blockEnd; i += 2) { hasher.Write16((short)(data[i] | (data[i + 1] << 8))); }
            for (var i = blockEnd; i < maxCount; i++) { hasher.Write8(data[i]); }
            return hasher.Finish();
        }

        private static int HashItJittery(IStreamingHasher<int> hasher, byte[] data, int maxCount)
        {
            hasher.Initialize();

            var biggestBlockEnd = Math.Max(0, maxCount - (maxCount % 8) - 8);
            var actualEnd = 0;
            var j = 0;
            for (var i = 0; i < biggestBlockEnd;)
            {
                switch (j++ % 4)
                {
                    case 0:
                        hasher.Write8(data[i]);
                        i += 1;
                        break;
                    case 1:
                        hasher.Write16((short)(data[i] | data[i + 1] << 8));
                        i += 2;
                        break;
                    case 2:
                        hasher.Write32(BitConverter.ToInt32(data, i));
                        i += 4;
                        break;
                    case 3:
                        hasher.Write64(BitConverter.ToInt64(data, i));
                        i += 8;
                        break;
                }

                actualEnd = i;
            }

            for (var i = actualEnd; i < maxCount; i++) { hasher.Write8(data[i]); }

            return hasher.Finish();
        }

        private static int HashItBlockwise(IBlockHasher<int> hasher, IList<byte> data, int offset, int maxCount) =>
            hasher.Hash(data.ToArray(), offset, maxCount);

        private static byte FlipBit(byte data, int bit) =>
            (byte)(data ^ (0x01 << bit));

        private static byte[] FlipBit(byte[] data, int byteIdx, int bitIdx)
        {
            var nextData = new byte[data.Length];
            Array.Copy(data, nextData, data.Length);
            nextData[byteIdx] = FlipBit(data[byteIdx], bitIdx);
            return nextData;
        }

        private static IStreamingHasher<int>[] Stream(params IStreamingHasher<int>[] hashers) => hashers;

        private static IBlockHasher<int>[] Block(params IBlockHasher<int>[] hashers) => hashers;

        private static IHashCodeCombiner[] Combiner(params IHashCodeCombiner[] hashers) => hashers;
    }
}