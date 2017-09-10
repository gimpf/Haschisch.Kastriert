using NUnit.Framework;

namespace Haschisch.Util.Tests
{
    [TestFixture]
    public class CheckUnsafeByteOps
    {
        [Test]
        public void UnsafeByteOps_PartialToUInt64_WithStartAtEnd_ReturnsZero()
        {
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = UnsafeByteOps.PartialToUInt64(ref buffer[0], (uint)buffer.Length, (uint)buffer.Length);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void UnsafeByteOps_PartialToUInt64_WithStartAfterEnd_ReturnsZero()
        {
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = UnsafeByteOps.PartialToUInt64(ref buffer[0], (uint)buffer.Length, (uint)buffer.Length + 1);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void UnsafeByteOps_PartialToUInt64_WithSingleByteRemaining_ReturnsValueOfLastByte()
        {
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = UnsafeByteOps.PartialToUInt64(ref buffer[0], (uint)buffer.Length, (uint)buffer.Length - 1);
            Assert.AreEqual(8, result);
        }

        [Test]
        public void UnsafeByteOps_PartialToUInt64_WithAllAvailable_ReturnsCorrectResult()
        {
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = UnsafeByteOps.PartialToUInt64(ref buffer[0], (uint)buffer.Length, 0);
            Assert.AreEqual(0x0807_0605_0403_0201UL, result);
        }
    }
}
