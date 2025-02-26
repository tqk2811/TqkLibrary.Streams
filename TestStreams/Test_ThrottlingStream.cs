using System.Diagnostics;
using TqkLibrary.Streams;
using TqkLibrary.Streams.ThrottlingHelpers;

namespace TestStreams
{
    [TestClass]
    public sealed class Test_ThrottlingStream
    {
        [DataTestMethod]
        [DataRow(20, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(100, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(1000, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(163, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(1239, 100u, 100u, 1000.0, 10u, 0u)]
        public async Task Test(int bufferLength, uint ReadBytesPerTime, uint WriteBytesPerTime, double timeMili, uint Balanced, uint DelayStep)
        {
            ThrottlingConfigure throttling = new();
            throttling.ReadBytesPerTime = ReadBytesPerTime;
            throttling.WriteBytesPerTime = WriteBytesPerTime;
            throttling.Time = TimeSpan.FromMilliseconds(timeMili);
            throttling.Balanced = Balanced;
            throttling.DelayStep = DelayStep;

            byte[] buffer = new byte[bufferLength];
            Random.Shared.NextBytes(buffer);
            byte[] buffer2 = new byte[buffer.Length];


            using MemoryStream memoryStream = new();
            using ThrottlingStream throttlingStream = new(throttling, memoryStream);

            Stopwatch stopwatch = Stopwatch.StartNew();
            await throttlingStream.WriteAsync(buffer);
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.Elapsed >= ((buffer.Length * 1.0 / throttling.ReadBytesPerTime - 1) * throttling.Time));


            throttlingStream.Seek(0, SeekOrigin.Begin);

            int byteRead = 0;
            int offset = 0;

            stopwatch.Restart();
            do
            {
                byteRead = await throttlingStream.ReadAsync(buffer2, offset, buffer2.Length - offset);
                offset += byteRead;
            }
            while (byteRead > 0);
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.Elapsed >= ((buffer.Length * 1.0 / throttling.WriteBytesPerTime - 1) * throttling.Time));
            Assert.IsTrue(buffer.SequenceEqual(buffer2));
        }


        [DataTestMethod]
        [DataRow(20, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(100, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(1000, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(163, 100u, 100u, 1000.0, 10u, 0u)]
        [DataRow(1239, 100u, 100u, 1000.0, 10u, 0u)]
        public async Task TestWithAsynchronousOnlyStream(int bufferLength, uint ReadBytesPerTime, uint WriteBytesPerTime, double timeMili, uint Balanced, uint DelayStep)
        {
            ThrottlingConfigure throttling = new();
            throttling.ReadBytesPerTime = ReadBytesPerTime;
            throttling.WriteBytesPerTime = WriteBytesPerTime;
            throttling.Time = TimeSpan.FromMilliseconds(timeMili);
            throttling.Balanced = Balanced;
            throttling.DelayStep = DelayStep;

            byte[] buffer = new byte[bufferLength];
            Random.Shared.NextBytes(buffer);
            byte[] buffer2 = new byte[buffer.Length];


            using MemoryStream memoryStream = new();
            using AsynchronousOnlyStream asynchronousOnlyStream = new(memoryStream);
            using ThrottlingStream throttlingStream = new(throttling, asynchronousOnlyStream);

            Stopwatch stopwatch = Stopwatch.StartNew();
            await throttlingStream.WriteAsync(buffer);
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.Elapsed >= ((buffer.Length * 1.0 / throttling.ReadBytesPerTime - 1) * throttling.Time));


            throttlingStream.Seek(0, SeekOrigin.Begin);

            int byteRead = 0;
            int offset = 0;

            stopwatch.Restart();
            do
            {
                byteRead = await throttlingStream.ReadAsync(buffer2, offset, buffer2.Length - offset);
                offset += byteRead;
            }
            while (byteRead > 0);
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.Elapsed >= ((buffer.Length * 1.0 / throttling.WriteBytesPerTime - 1) * throttling.Time));
            Assert.IsTrue(buffer.SequenceEqual(buffer2));
        }
    }
}
