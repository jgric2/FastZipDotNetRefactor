using System.Runtime.CompilerServices;

public class AdjustableSemaphore
{
    private readonly object _lockObject = new object();
    private int _availableCount;
    private int _maximumCount;

    public AdjustableSemaphore(int maximumCount)
    {
        if (maximumCount < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumCount), "Must be >= 0");

        _maximumCount = maximumCount;
        _availableCount = maximumCount;
    }

    public int MaximumCount
    {
        get
        {
            lock (_lockObject) { return _maximumCount; }
        }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Must be >= 0");

            lock (_lockObject)
            {
                int delta = value - _maximumCount;
                _maximumCount = value;

                long newAvailable = (long)_availableCount + delta;
                if (newAvailable > _maximumCount) newAvailable = _maximumCount;
                if (newAvailable < 0) newAvailable = 0;
                _availableCount = (int)newAvailable;

                // Wake up all waiters to re-evaluate against the new max
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
                // Indefinite wait; spurious wakeups handled by the while loop
                Monitor.Wait(_lockObject);
            }
            _availableCount--;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WaitOne(CancellationToken cancellationToken)
    {
        // Fast path if not cancellable: identical to WaitOne()
        if (!cancellationToken.CanBeCanceled)
        {
            WaitOne();
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Register a callback that wakes up any waiters when cancellation is requested.
        // We pulse all waiters to guarantee the cancelled waiter wakes (even if a different
        // thread happens to be pulsed first).
        using var registration = cancellationToken.Register(static state =>
        {
            var lockObj = (object)state!;
            lock (lockObj)
            {
                Monitor.PulseAll(lockObj);
            }
        }, _lockObject);

        lock (_lockObject)
        {
            while (_availableCount <= 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Monitor.Wait(_lockObject);
            }
            _availableCount--;
        }
    }

    // Convenience for async code: run the blocking waiter on a thread pool thread
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
                Monitor.Pulse(_lockObject); // Wake a single waiter
            }
            else
            {
                throw new SemaphoreFullException("Releasing would exceed maximum count.");
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


//using System.Runtime.CompilerServices;

//public class AdjustableSemaphore
//{
//    private readonly object _lockObject = new object();
//    private int _availableCount;
//    private int _maximumCount;

//    public AdjustableSemaphore(int maximumCount)
//    {
//        if (maximumCount < 0)
//            throw new ArgumentOutOfRangeException(nameof(maximumCount), "Must be >= 0");

//        _maximumCount = maximumCount;
//        _availableCount = maximumCount;
//    }

//    public int MaximumCount
//    {
//        get
//        {
//            lock (_lockObject) { return _maximumCount; }
//        }
//        set
//        {
//            if (value < 0)
//                throw new ArgumentOutOfRangeException(nameof(value), "Must be >= 0");

//            lock (_lockObject)
//            {
//                // Adjust available count by the delta but clamp to [0, newMax]
//                int delta = value - _maximumCount;
//                _maximumCount = value;

//                long newAvailable = (long)_availableCount + delta;
//                if (newAvailable > _maximumCount) newAvailable = _maximumCount;
//                if (newAvailable < 0) newAvailable = 0;
//                _availableCount = (int)newAvailable;

//                // Wake up all waiters to re-evaluate
//                Monitor.PulseAll(_lockObject);
//            }
//        }
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void WaitOne()
//    {
//        lock (_lockObject)
//        {
//            while (_availableCount <= 0)
//            {
//                // Use a timeout to avoid indefinite waits (helps with spurious wakeups)
//                Monitor.Wait(_lockObject, 50);
//            }
//            _availableCount--;
//        }
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void WaitOne(CancellationToken cancellationToken)
//    {
//        lock (_lockObject)
//        {
//            while (_availableCount <= 0)
//            {
//                cancellationToken.ThrowIfCancellationRequested();
//                // Wait with timeout so we can recheck cancellation
//                Monitor.Wait(_lockObject, 50);
//            }
//            _availableCount--;
//        }
//    }

//    // Convenience for async code: run the blocking waiter on a thread pool thread
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public Task WaitAsync(CancellationToken cancellationToken = default)
//    {
//        return Task.Run(() => WaitOne(cancellationToken), cancellationToken);
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void Release()
//    {
//        lock (_lockObject)
//        {
//            if (_availableCount < _maximumCount)
//            {
//                _availableCount++;
//                Monitor.Pulse(_lockObject);
//            }
//            else
//            {
//                throw new SemaphoreFullException("Releasing would exceed maximum count.");
//            }
//        }
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public void GetSemaphoreInfo(out int maxCount, out int usedCount, out int availableCount)
//    {
//        lock (_lockObject)
//        {
//            maxCount = _maximumCount;
//            usedCount = _maximumCount - _availableCount;
//            availableCount = _availableCount;
//        }
//    }
//}