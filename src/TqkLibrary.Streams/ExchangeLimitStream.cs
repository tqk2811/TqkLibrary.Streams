using System;
using System.Collections.Generic;
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
        readonly object _lockRead = new object();
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (MaxBytesRead != 0 && count > 0)
            {
                lock (_lockRead)
                {
                    uint diff = MaxBytesRead - _bytesReadedCount;
                    count = (int)Math.Min(diff, (uint)count);
                    if (count == 0) return count;
                }
            }
            int byte_read = base.Read(buffer, offset, count);
            if(byte_read > 0)
            {
                lock (_lockRead)
                {
                    _bytesReadedCount += (uint)byte_read;
                }
            }
            return byte_read;
        }


        uint _bytesWritedCount = 0;
        readonly object _lockWrite = new object();
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (MaxBytesWrite != 0 && count > 0)
            {
                lock (_lockWrite)
                {
                    uint diff = MaxBytesWrite - _bytesWritedCount;
                    count = (int)Math.Min(diff, (uint)count);
                    if (count == 0) return;
                    _bytesWritedCount += (uint)count;
                }
            }
            base.Write(buffer, offset, count);
        }

    }
}
