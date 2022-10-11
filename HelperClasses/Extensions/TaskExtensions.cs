using System.Threading;
using System.Threading.Tasks;

namespace CoreUtilities.HelperClasses.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Task"/> and <see cref="Task{TResult}"/> classes.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Converts a <see cref="Task{TResult}"/> to a cancellable task, which can be provided with a 
        /// <see cref="CancellationToken"/> to cancel a long running async task manually.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task to run.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to cancel the task.</param>
        /// <returns>A <see cref="Task{TResult}"/>.</returns>
        public static Task<T> AsCancellable<T>(this Task<T> task, CancellationToken token)
        {
            if (!token.CanBeCanceled)
            {
                return task;
            }

            var tcs = new TaskCompletionSource<T>();
            // This cancels the returned task:
            // 1. If the token has been canceled, it cancels the TCS straightaway
            // 2. Otherwise, it attempts to cancel the TCS whenever
            //    the token indicates cancelled
            token.Register(() => tcs.TrySetCanceled(token),
                useSynchronizationContext: false);

            task.ContinueWith(t =>
                {
                    // Complete the TCS per task status
                    // If the TCS has been cancelled, this continuation does nothing
                    if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else if (task.IsFaulted)
                    {
                        tcs.TrySetException(t.Exception);
                    }
                    else
                    {
                        var result = tcs.TrySetResult(t.Result);
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return tcs.Task;
        }

        /// <summary>
        /// Converts a <see cref="Task"/> to a cancellable task, which can be provided with a 
        /// <see cref="CancellationToken"/> to cancel a long running async task manually.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task to run.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to cancel the task.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task AsCancellable(this Task task, CancellationToken token)
        {
            if (!token.CanBeCanceled)
            {
                return task;
            }

            var tcs = new TaskCompletionSource();
            // This cancels the returned task:
            // 1. If the token has been canceled, it cancels the TCS straightaway
            // 2. Otherwise, it attempts to cancel the TCS whenever
            //    the token indicates cancelled
            token.Register(() => tcs.TrySetCanceled(token),
                useSynchronizationContext: false);

            task.ContinueWith(t =>
                {
                    // Complete the TCS per task status
                    // If the TCS has been cancelled, this continuation does nothing
                    if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else if (task.IsFaulted)
                    {
                        tcs.TrySetException(t.Exception);
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return tcs.Task;
        }
    }
}
