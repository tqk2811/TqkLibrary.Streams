namespace TqkLibrary.Streams
{
    public class AsynchronousOnlyStream : BaseInheritStream
    {
        public AsynchronousOnlyStream(Stream baseStream, bool disposeBaseStream = true) : base(baseStream, disposeBaseStream)
        {

        }

        //ReadAsync -> BeginRead/EndRead -> Read
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => _baseStream.BeginRead(buffer, offset, count, callback, state);
        public override int EndRead(IAsyncResult asyncResult)
            => _baseStream.EndRead(asyncResult);
        [Obsolete]
        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException($"Use asynchronous method only");


        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => _baseStream.BeginWrite(buffer, offset, count, callback, state);
        public override void EndWrite(IAsyncResult asyncResult)
            => _baseStream.EndWrite(asyncResult);
        [Obsolete]
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException($"Use asynchronous method only");
    }
}
