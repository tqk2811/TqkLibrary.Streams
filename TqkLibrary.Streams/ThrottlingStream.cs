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
            => throw new NotSupportedException();
        public override int EndRead(IAsyncResult asyncResult)
            => throw new NotSupportedException();
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => throw new NotSupportedException();
        public override void EndWrite(IAsyncResult asyncResult)
            => throw new NotSupportedException();



        public override int Read(byte[] buffer, int offset, int count)
        {
            int calcCount = Configure.CalcRead(count);
            int bytes_read = _baseStream.Read(buffer, offset, calcCount);
            Configure.UpdateRealRead(calcCount - bytes_read);
            return bytes_read;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            int calcCount = Configure.CalcRead(count);
            int bytes_read = await base.ReadAsync(buffer, offset, calcCount, cancellationToken);
            Configure.UpdateRealRead(calcCount - bytes_read);
            return bytes_read;
        }



        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();//mustbe delay, can't add
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            while (count > 0)
            {
                int stepCount = Configure.CalcWrite(count);
                if (stepCount > 0)
                {
                    await _baseStream.WriteAsync(buffer, offset, stepCount, cancellationToken);

                    offset += stepCount;
                    count -= stepCount;
                }
                await Task.Delay(Math.Max(Configure.DelayWriteStep, 0), cancellationToken);
            }
        }
    }
}
