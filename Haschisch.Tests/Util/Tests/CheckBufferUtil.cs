using NUnit.Framework;

namespace Haschisch.Util.Tests
{
    [TestFixture]
    public class CheckBufferUtil
    {
        [Test]
        public void BufferUtil_ZeroUnusedBuffer_WithStartAtBeginning_ZeroesAll()
        {
            var buffer = 0xFFFF_FFFF_FFFF_FFFFUL;
            BufferUtil.ZeroUnusedBuffer(ref buffer, 0);
            Assert.AreEqual(0, buffer);
        }

        [Test]
        public void BufferUtil_ZeroUnusedBuffer_WithStartAtMidpoint_ZeroesUpperHalf()
        {
            var buffer = 0xFFFF_FFFF_FFFF_FFFFUL;
            BufferUtil.ZeroUnusedBuffer(ref buffer, 4);
            Assert.AreEqual(0xFFFF_FFFFU, (uint)buffer);
            Assert.AreEqual(0U, (uint)(buffer >> 32));
        }
    }
}
