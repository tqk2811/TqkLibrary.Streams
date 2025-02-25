namespace TqkLibrary.Streams.ThrottlingHelpers
{
    public class ThrottlingConfigure
    {
        const uint min_read = 1;

        /// <summary>
        /// delay in miliseconds
        /// </summary>
        public UInt16 DelayWriteStep { get; set; } = 0;

        /// <summary>
        /// Balanced for multi streams read/write. Maximum result for per step calc <see cref="CalcRead"/> and <see cref="CalcWrite"/>"/>
        /// </summary>
        public UInt16 Balanced { get; set; } = UInt16.MaxValue;

        /// <summary>
        /// zero mean no limit
        /// </summary>
        public UInt16 ReadBytesPerTime { get; set; } = 0;

        /// <summary>
        /// zero mean no limit
        /// </summary>
        public UInt16 WriteBytesPerTime { get; set; } = 0;

        /// <summary>
        /// less or equal zero mean no limit
        /// </summary>
        public TimeSpan Time { get; set; } = TimeSpan.FromSeconds(1);


        readonly object _lock_read = new();
        DateTime _lastTimeRead = DateTime.MinValue;
        uint _readBytes = 0;
        public int CalcRead(int count)
        {
            if (count < 0) throw new InvalidOperationException($"{nameof(count)} must be greater or equal 0");
            return (int)CalcRead((uint)count);
        }
        public uint CalcRead(uint count)
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

                count = Math.Max(min_read, Math.Min(ReadBytesPerTime - _readBytes, Balanced <= 0 ? UInt16.MaxValue : Balanced));
                _readBytes += count;

                return count;
            }
        }
        public void UpdateRealRead(int caculated, int realRead)
        {
            if (caculated < 0) throw new InvalidOperationException($"{nameof(caculated)} must be greater or equal 0");
            if (realRead < 0) throw new InvalidOperationException($"{nameof(realRead)} must be greater or equal 0");
            UpdateRealRead((uint)caculated, (uint)realRead);
        }
        public void UpdateRealRead(uint caculated, uint realRead)
        {
            if (caculated < realRead) throw new InvalidOperationException(realRead + " must be less or equal " + caculated);
            uint count = caculated - realRead;
            if (count > 0)
            {
                lock (_lock_read)
                {
                    if (count >= _readBytes) _readBytes = 0;
                    else _readBytes -= count;
                }
            }
        }




        readonly object _lock_write = new();
        DateTime _lastTimeWrite = DateTime.MinValue;
        uint _writeBytes = 0;
        public int CalcWrite(int count)
        {
            if (count < 0) throw new InvalidOperationException($"{nameof(count)} must be greater or equal 0");
            return (int)CalcWrite((uint)count);
        }
        public uint CalcWrite(uint count)
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

                count = Math.Max(0, Math.Min(WriteBytesPerTime - _writeBytes, Balanced <= 0 ? UInt16.MaxValue : Balanced));
                _writeBytes += count;

                return count;
            }
        }
    }
}
