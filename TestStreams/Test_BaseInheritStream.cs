using TqkLibrary.Streams;

namespace TestStreams
{
    [TestClass]
    public sealed class Test_BaseInheritStream
    {
        [TestMethod]
        public async Task Test()
        {
            using MemoryStream memoryStream = new();
            BaseInheritStream baseInheritStream = new(memoryStream);

            byte[] buffer = new byte[Random.Shared.Next(100, 200)];
            Random.Shared.NextBytes(buffer);
            byte[] buffer2 = new byte[buffer.Length];


            memoryStream.Write(buffer);
            memoryStream.Seek(0, SeekOrigin.Begin);
            int bytes_read = baseInheritStream.Read(buffer2);
            Assert.AreEqual(buffer.Length, bytes_read);
            Assert.IsTrue(buffer.SequenceEqual(buffer2));


            await memoryStream.WriteAsync(buffer);
            memoryStream.Seek(0, SeekOrigin.Begin);
            bytes_read = await baseInheritStream.ReadAsync(buffer2);
            Assert.AreEqual(buffer.Length, bytes_read);
            Assert.IsTrue(buffer.SequenceEqual(buffer2));
        }
    }
}
