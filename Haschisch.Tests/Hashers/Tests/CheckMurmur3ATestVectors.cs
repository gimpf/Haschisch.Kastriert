using NUnit.Framework;

namespace Haschisch.Hashers.Tests
{
    [TestFixture]
    public class CheckMurmur3ATestVectors
    {
        // see https://stackoverflow.com/a/31929528/15529
        private static readonly (byte[], uint, uint)[] Vectors = new[]
        {
            // Input, Seed, Expected
            (new byte[0], 0u, 0u         ), // with zero data and zero seed, everything becomes zero
            (new byte[0], 1u, 0x514E28B7u ), // ignores nearly all the math
            (new byte[0], 0xffffffffu, 0x81F16F39u ), // make sure your seed uses unsigned 32-bit math
            (new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0u, 0x76293B50u ), // make sure 4-byte chunks use unsigned math
            (new byte[] { 0x21, 0x43, 0x65, 0x87 }, 0u, 0xF55B516Bu ), // Endian order.UInt32 should end up as 0x87654321
            (new byte[] { 0x21, 0x43, 0x65, 0x87 }, 0x5082EDEEu, 0x2362F9DEu ), // Special seed value eliminates initial key with xor
            (new byte[] { 0x21, 0x43, 0x65    },    0u, 0x7E4A8634u ), // Only three bytes.Should end up as 0x654321
            (new byte[] { 0x21, 0x43       },       0u, 0xA0F7B07Au ), // Only two bytes.Should end up as 0x4321
            (new byte[] { 0x21          },          0u, 0x72661CF4u ), // Only one byte. Should end up as 0x21
            (new byte[] { 0x00, 00, 00, 00 },       0u, 0x2362F9DEu ), // Make sure compiler doesn't see zero and convert to null
            (new byte[] { 0x00, 00, 00    },        0u, 0x85F0B427u ),
            (new byte[] { 0x00, 00,       },        0u, 0x30F4C306u ),
            (new byte[] { 0x00          },          0u, 0x514E28B7u )
        };

        [Test]
        public void Hash_Murmur3A_HashBytes_ReturnsExpectedHashCode()
        {
            for (var i = 0; i < Vectors.Length; i++)
            {
                var item = Vectors[i];
                var h = default(Murmur3x8632Hasher.Block);
                var actual = (uint)h.Hash(item.Item2, item.Item1, 0, item.Item1.Length);
                Assert.AreEqual(item.Item3, actual, "Failed test vector #{0}", i);
            }
        }
    }
}
