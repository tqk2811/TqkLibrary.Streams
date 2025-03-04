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
        //ReadAsync -> BeginRead/EndRead -> Read
        [Obsolete]
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => throw new NotSupportedException();
        [Obsolete]
        public override int EndRead(IAsyncResult asyncResult)
            => throw new NotSupportedException();
        [Obsolete]
        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();//mustbe delay, can't add -> so BeginRead/EndRead must exclude






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
        [Obsolete]
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => throw new NotSupportedException();
        [Obsolete]
        public override void EndWrite(IAsyncResult asyncResult)
            => throw new NotSupportedException();
        [Obsolete]
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();//mustbe delay, can't add -> so BeginWrite/EndWrite must exclude
    }
}
