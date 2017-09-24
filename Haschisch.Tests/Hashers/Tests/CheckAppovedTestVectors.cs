using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Haschisch.Hashers.Tests;
using NUnit.Framework;

namespace Haschisch.CheckUtils
{
    public class CheckApprovedTestVectors
    {
        private static readonly byte[] input = InitializeInputData();

        [Test]
        public void Hashers_HashToApprovedTestVectors()
        {
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "./Haschisch-TestVectors");
            Directory.CreateDirectory(basePath);

            GenerateAndReportVectors(CheckBaseHashProperties.Algorithms, basePath);

            var inconclusive = new List<string>();
            foreach (var fn in Directory.GetFiles(basePath, "*.test-vectors"))
            {
                var baseName = Path.GetFileName(fn);
                var stream = typeof(CheckApprovedTestVectors).Assembly.GetManifestResourceStream("Haschisch.Tests.TestVectors." + baseName);
                if (stream == null)
                {
                    inconclusive.Add(baseName);
                    continue;
                }

                using (var reader = new StreamReader(stream))
                {
                    var stored = reader.ReadToEnd();
                    var current = File.ReadAllText(fn);
                    Assert.AreEqual(stored, current, "Hash algorithm {0} failed.", baseName);
                }
            }

            if (inconclusive.Count > 0) { Assert.Inconclusive("Following hashers did not have test-vectors: {0}", string.Join(", ", inconclusive)); }
        }

        public static void GenerateAndReportVectors(IEnumerable<CheckBaseHashProperties.HashAlgorithm> algorithms, string basePath)
        {
            Task.WaitAll(algorithms
                .Select(GenerateAndReportVectors)
                .Select(WriteReport)
                .ToArray());

            async Task WriteReport((string name, (byte[] seed, object[] seededVectors)[] allVectors) x)
            {
                var resultFile = Path.Combine(basePath, "./" + x.name + ".test-vectors");

                using (var of = File.CreateText(resultFile))
                {
                    await of.WriteLineAsync(string.Format("name = \"{0}\"\n", x.name));

                    for (var i = 0; i < x.allVectors.Length; i++)
                    {
                        var (seed, seededVectors) = x.allVectors[i];
                        if (seed != null)
                        {
                            await of.WriteLineAsync(string.Format("\nseed = 0x{0}", BitConverter.ToString(seed).Replace("-", "")));
                            for (var j = 0; j < seededVectors.Length; j++)
                            {
                                await of.WriteLineAsync(string.Format("length = {0,3}, hash = 0x{1}", j, BitConverter.ToString((byte[])seededVectors[j]).Replace("-", "")));
                            }
                        }
                        else
                        {
                            for (var j = 0; j < seededVectors.Length; j++)
                            {
                                await of.WriteLineAsync(string.Format("length = {0,3}, hash = 0x{1:x8}", j, seededVectors[j]));
                            }
                        }
                    }
                }
            }
        }

        private static (string name, (byte[] seed, object[] vectors)[]) GenerateAndReportVectors(
            CheckBaseHashProperties.HashAlgorithm algo)
        {
            var hasher = algo.Block.First();
            return hasher is ISeedableBlockHasher sbh ?
                (algo.Name, GetVectorsForSeeds().ToArray()) :
                (algo.Name, new[] { GetVectorsForUnseedable() });

            IEnumerable<(byte[], object[])> GetVectorsForSeeds()
            {
                var seedLength = sbh.GetZeroSeed().Length;
                for (var i = 0; i <= seedLength; i++)
                {
                    var seed = sbh.GetZeroSeed();
                    for (var j = 0; j < i; j++)
                    {
                        seed[j] = (byte)(j + 1);
                    }

                    yield return GetVectorsForSeedable(seed);
                }
            }

            (byte[], object[]) GetVectorsForSeedable(byte[] seed) =>
                (seed, GetVectorsForSeed(seed).ToArray());

            IEnumerable<object> GetVectorsForSeed(byte[] seed)
            {
                for (var i = 0; i <= input.Length; i++)
                {
                    yield return sbh.Hash(seed, input, 0, i);
                }
            }

            (byte[], object[]) GetVectorsForUnseedable() =>
                (null, GetVectors().ToArray());

            IEnumerable<object> GetVectors()
            {
                for (var i = 0; i <= input.Length; i++)
                {
                    yield return hasher.Hash(input, 0, i);
                }
            }
        }

        private static byte[] InitializeInputData() =>
            Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
    }
}
