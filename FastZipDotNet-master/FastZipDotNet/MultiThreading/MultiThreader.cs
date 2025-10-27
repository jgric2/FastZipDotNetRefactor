namespace FastZipDotNet.MultiThreading
{
    public class MultiThreader<T>
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationToken _cancellationToken;

        public MultiThreader(int maxConcurrency, CancellationToken cancellationToken)
        {
            _semaphore = new SemaphoreSlim(maxConcurrency);
            _cancellationToken = cancellationToken;
        }

        public async Task ExecuteTasksAsync(List<T> items, Func<T, Task> taskFunction)
        {
            var tasks = new List<Task>();

            foreach (var item in items)
            {
                await _semaphore.WaitAsync(_cancellationToken);

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await taskFunction(item);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, _cancellationToken);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
