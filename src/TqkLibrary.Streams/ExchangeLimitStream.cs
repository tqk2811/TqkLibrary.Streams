using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TqkLibrary.Streams
{
    public class ExchangeLimitStream : BaseInheritStream
    {
        /// <summary>
        /// 0 mean unlimit
        /// </summary>
        public uint MaxBytesRead { get; set; } = uint.MaxValue;
        /// <summary>
        /// 0 mean unlimit
        /// </summary>
        public uint MaxBytesWrite { get; set; } = uint.MaxValue;

        public ExchangeLimitStream(Stream baseStream, bool disposeBaseStream = true) : base(baseStream, disposeBaseStream)
        {
        }

        uint _bytesReadedCount = 0;
        public override int Read(byte[] buffer, int offset, int count)
        {
            int byte_read = base.Read(buffer, offset, _CalcRead(count));
            _UpdateRead(byte_read);
            return byte_read;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int byte_read = await base.ReadAsync(buffer, offset, _CalcRead(count), cancellationToken);
            _UpdateRead(byte_read);
            return byte_read;
        }
        int _CalcRead(int count)//stream should synchronize call
        {
            if (MaxBytesRead != 0 && count > 0)
            {
                if (MaxBytesRead >= _bytesReadedCount)
                {
                    uint diff = MaxBytesRead - _bytesReadedCount;
                    count = (int)Math.Min(diff, (uint)count);
                }
                else
                {
                    count = 0;
                }
            }
            return count;
        }
        void _UpdateRead(int byte_read)//stream should synchronize call
        {
            if (byte_read > 0)
            {
                _bytesReadedCount += (uint)byte_read;
            }
        }





        uint _bytesWritedCount = 0;
        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, _CalcAndUpdateWrite(count));
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _baseStream.WriteAsync(buffer, offset, _CalcAndUpdateWrite(count), cancellationToken);
        }

        int _CalcAndUpdateWrite(int count)//stream should synchronize call
        {
            if (MaxBytesWrite != 0 && count > 0)
            {
                uint diff = MaxBytesWrite - _bytesWritedCount;
                count = (int)Math.Min(diff, (uint)count);
                if (count == 0) return count;
                _bytesWritedCount += (uint)count;
            }
            return count;
        }
    }
}
