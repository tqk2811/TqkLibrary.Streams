using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.Streams.ThrottlingHelpers
{
    public class ThrottlingConfigure
    {
        const int min_read = 1;

        /// <summary>
        /// delay in miliseconds
        /// </summary>
        public int DelayWriteStep { get; set; } = 10;

        /// <summary>
        /// Balanced for multi streams read/write. Maximum result for per step calc <see cref="CalcRead"/> and <see cref="CalcWrite"/>"/>
        /// </summary>
        public int Balanced { get; set; } = int.MaxValue;

        /// <summary>
        /// less or equal zero mean no limit
        /// </summary>
        public int ReadBytesPerTime { get; set; } = 0;

        /// <summary>
        /// less or equal zero mean no limit
        /// </summary>
        public int WriteBytesPerTime { get; set; } = 0;

        /// <summary>
        /// less or equal zero mean no limit
        /// </summary>
        public TimeSpan Time { get; set; } = TimeSpan.FromSeconds(1);


        readonly object _lock_read = new();
        DateTime _lastTimeRead = DateTime.MinValue;
        int _readBytes = 0;
        public int CalcRead(int count)
        {
            if (ReadBytesPerTime <= 0 || Time <= TimeSpan.Zero || count == 0) return count;
            lock (_lock_read)
            {
                DateTime now = DateTime.UtcNow;
                if (now > _lastTimeRead.Add(Time))
                {
                    _lastTimeRead = now;
                    _readBytes = 0;
                }

                count = Math.Max(min_read, Math.Min(ReadBytesPerTime - _readBytes, Balanced <= 0 ? int.MaxValue : Balanced));
                _readBytes += count;

                return count;
            }
        }
        public void UpdateRealRead(int count)
        {
            if(count > 0)
            {
                lock (_lock_read)
                {
                    _readBytes -= count;
                }
            }
        }




        readonly object _lock_write = new();
        DateTime _lastTimeWrite = DateTime.MinValue;
        int _writeBytes = 0;
        public int CalcWrite(int count)
        {
            if (WriteBytesPerTime <= 0 || Time <= TimeSpan.Zero || count == 0) return count;
            lock (_lock_write)
            {
                DateTime now = DateTime.UtcNow;
                if (now > _lastTimeWrite.Add(Time))
                {
                    _lastTimeWrite = now;
                    _writeBytes = 0;
                }

                count = Math.Max(0, Math.Min(WriteBytesPerTime - _writeBytes, Balanced <= 0 ? int.MaxValue : Balanced));
                _writeBytes += count;

                return count;
            }
        }
    }
}
