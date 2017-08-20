using Haschisch.Hashers;
using NUnit.Framework;

namespace Haschisch.Hashers.Tests
{
    [TestFixture]
    public class CheckSeaHashTestVectors
    {
        [Test]
        public void Hash_SeaHash_ReturnsExpectedHash()
        {
            var hasher = default(SeaHasher.Stream);
            hasher.Initialize();
            foreach (byte b in "to be or not to be")
            {
                hasher.Write8(b);
            }

            var h = hasher.Finish();
            Assert.AreEqual((ulong)h, 1988685042348123509UL);
        }
    }
}
