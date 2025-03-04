using TqkLibrary.Streams;

namespace TestStreams
{
    [TestClass]
    public sealed class Test_ExchangeLimitStream
    {
        [TestMethod]
        public void Test()
        {
            using MemoryStream memoryStream = new();
            ExchangeLimitStream exchangeLimitStream = new(memoryStream);
            exchangeLimitStream.MaxBytesRead = 100;
            exchangeLimitStream.MaxBytesWrite = 100;


            byte[] buffer = new byte[Random.Shared.Next(200, 400)];
            Random.Shared.NextBytes(buffer);
            byte[] buffer2 = new byte[buffer.Length];


            exchangeLimitStream.Write(buffer);
            Assert.AreEqual(memoryStream.Length, exchangeLimitStream.MaxBytesWrite);



            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.Write(buffer);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using MemoryStream memoryStream2 = new();
            exchangeLimitStream.CopyTo(memoryStream2);
            Assert.AreEqual(memoryStream2.Length, exchangeLimitStream.MaxBytesRead);
        }

        [TestMethod]
        public async Task TestAsync()
        {
            using MemoryStream memoryStream = new();
            ExchangeLimitStream exchangeLimitStream = new(memoryStream);
            exchangeLimitStream.MaxBytesRead = 100;
            exchangeLimitStream.MaxBytesWrite = 100;


            byte[] buffer = new byte[Random.Shared.Next(200, 400)];
            Random.Shared.NextBytes(buffer);
            byte[] buffer2 = new byte[buffer.Length];


            await exchangeLimitStream.WriteAsync(buffer);
            Assert.AreEqual(memoryStream.Length, exchangeLimitStream.MaxBytesWrite);



            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.WriteAsync(buffer);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using MemoryStream memoryStream2 = new();
            await exchangeLimitStream.CopyToAsync(memoryStream2);
            Assert.AreEqual(memoryStream2.Length, exchangeLimitStream.MaxBytesRead);
        }
    }
}
