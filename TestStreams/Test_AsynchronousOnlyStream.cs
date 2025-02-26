using System.Threading.Tasks;
using TqkLibrary.Streams;

namespace TestStreams
{
    [TestClass]
    public sealed class Test_AsynchronousOnlyStream
    {
        [TestMethod]
        public async Task Test_WriteAsyncReadAsync()
        {
            using MemoryStream memoryStream = new();
            using AsynchronousOnlyStream asynchronousOnlyStream = new(memoryStream);

            byte[] buffer = new byte[Random.Shared.Next(100, 200)];
            Random.Shared.NextBytes(buffer);
            byte[] buffer2 = new byte[buffer.Length];

            await asynchronousOnlyStream.WriteAsync(buffer);
            asynchronousOnlyStream.Seek(0, SeekOrigin.Begin);
            int bytes_read = await asynchronousOnlyStream.ReadAsync(buffer2);
            Assert.AreEqual(buffer.Length, bytes_read);
            Assert.IsTrue(buffer.SequenceEqual(buffer2));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Test_Read_Exception()
        {
            using MemoryStream memoryStream = new();
            using AsynchronousOnlyStream asynchronousOnlyStream = new(memoryStream);

            byte[] buffer = new byte[Random.Shared.Next(100, 200)];
            asynchronousOnlyStream.Read(buffer);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Test_Write_Exception()
        {
            using MemoryStream memoryStream = new();
            using AsynchronousOnlyStream asynchronousOnlyStream = new(memoryStream);

            byte[] buffer = new byte[Random.Shared.Next(100, 200)];
            asynchronousOnlyStream.Write(buffer);
        }
    }
}
