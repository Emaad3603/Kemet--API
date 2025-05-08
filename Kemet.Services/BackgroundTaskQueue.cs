using Kemet.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue =
            Channel.CreateUnbounded<Func<CancellationToken, Task>>();

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem) =>
            _queue.Writer.TryWrite(workItem);

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken ct) =>
            await _queue.Reader.ReadAsync(ct);
    }

}
