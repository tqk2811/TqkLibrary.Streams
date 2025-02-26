using System;
using TqkLibrary.Streams.ThrottlingHelpers;

namespace TqkLibrary.Streams
{
    public class ThrottlingStream : BaseInheritStream
    {
        public ThrottlingConfigure Configure { get; } = new();

        public ThrottlingStream(Stream baseStream, bool disposeBaseStream = true) : base(baseStream, disposeBaseStream)
        {
        }
        public ThrottlingStream(ThrottlingConfigure configure, Stream baseStream, bool disposeBaseStream = true) : base(baseStream, disposeBaseStream)
        {
            this.Configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => ((Stream)this).BeginRead(buffer, offset, count, callback, state);
        public override int EndRead(IAsyncResult asyncResult)
            => ((Stream)this).EndRead(asyncResult);
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => ((Stream)this).BeginWrite(buffer, offset, count, callback, state);
        public override void EndWrite(IAsyncResult asyncResult)
            => ((Stream)this).EndWrite(asyncResult);



        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();//mustbe delay, can't add
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            if (count <= 0) return 0;

            int calcCount = 0;
            while (true)
            {
                calcCount = Configure.CalcRead(count);
                if (calcCount == 0)
                    await Task.Delay((int)Math.Min(Configure.DelayStep, 1000), cancellationToken);
                else
                    break;
            }
            int bytes_read = await _baseStream.ReadAsync(buffer, offset, calcCount, cancellationToken);
            Configure.UpdateRealRead(calcCount, bytes_read);
            return bytes_read;
        }



        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();//mustbe delay, can't add
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            //count = count - offset;
            while (count > 0)
            {
                int stepCount = Configure.CalcWrite(count);
                if (stepCount > 0)
                {
                    await _baseStream.WriteAsync(buffer, offset, stepCount, cancellationToken);

                    offset += stepCount;
                    count -= stepCount;
                }
                await Task.Delay((int)Math.Min(Configure.DelayStep, 1000), cancellationToken);
            }
        }
    }
}
