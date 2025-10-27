using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastZipDotNet.MultiThreading
{
    public class AdjustableSemaphoreTaskManager<T>
    {
        private readonly AdjustableSemaphore _semaphore;
        private readonly CancellationToken _cancellationToken;

        public AdjustableSemaphoreTaskManager(int maxConcurrency, CancellationToken cancellationToken)
        {
            _semaphore = new AdjustableSemaphore(maxConcurrency);
            _cancellationToken = cancellationToken;
        }

        public async Task ExecuteTasksAsync(IEnumerable<T> items, Func<T, Task<bool>> taskFunction)
        {
            var tasks = new List<Task<bool>>();

            foreach (var item in items)
            {
                _semaphore.WaitOne(_cancellationToken);

                var task = Task.Run(async () =>
                {
                    try
                    {
                        return await taskFunction(item);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, _cancellationToken);

                tasks.Add(task);
            }

            var results = await Task.WhenAll(tasks);

            // Handle the overall success/fail status
            if (Array.Exists(results, result => result == false))
            {
                throw new Exception("One or more tasks failed.");
            }
        }
    }
}
