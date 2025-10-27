using System.Runtime.CompilerServices;

namespace FastZipDotNet.MultiThreading
{
    public class AdjustableSemaphore
    {
        private readonly object _lockObject = new object();
        private int _availableCount;
        private int _maximumCount;

        public AdjustableSemaphore(int maximumCount)
        {
            if (maximumCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCount), "Must be greater than or equal to 0.");
            }

            _maximumCount = maximumCount;
            _availableCount = maximumCount;
        }

        public int MaximumCount
        {
            get
            {
                lock (_lockObject)
                {
                    return _maximumCount;
                }
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Must be greater than or equal to 0.");
                }

                lock (_lockObject)
                {
                    _availableCount += value - _maximumCount;
                    _maximumCount = value;
                    Monitor.PulseAll(_lockObject);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitOne()
        {
            lock (_lockObject)
            {
                while (_availableCount <= 0)
                {
                    Monitor.Wait(_lockObject);
                }
                _availableCount--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitOne(CancellationToken cancellationToken)
        {
            lock (_lockObject)
            {
                while (_availableCount <= 0)
                {
                    // Check if cancellation has been requested
                    cancellationToken.ThrowIfCancellationRequested();
                    Monitor.Wait(_lockObject);
                }
                _availableCount--;
            }
        }

        // Convenience wrapper for async code
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WaitAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => WaitOne(cancellationToken), cancellationToken);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release()
        {
            lock (_lockObject)
            {
                if (_availableCount < _maximumCount)
                {
                    _availableCount++;
                    Monitor.Pulse(_lockObject);
                }
                else
                {
                    throw new SemaphoreFullException("Adding the given count to the semaphore would cause it to exceed its maximum count.");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetSemaphoreInfo(out int maxCount, out int usedCount, out int availableCount)
        {
            lock (_lockObject)
            {
                maxCount = _maximumCount;
                usedCount = _maximumCount - _availableCount;
                availableCount = _availableCount;
            }
        }
    }
}
