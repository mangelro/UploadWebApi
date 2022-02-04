using System;
using System.Threading;
using System.Threading.Tasks;

namespace UploadWebApi.Infraestructura
{
    /// <summary>
    /// Clase de utilidad para ejecutar métodos asíncronos dentro de procesos sincronos
    /// </summary>
    public static class AsyncUtil
    {
        private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        /// <summary>
        /// Executa un metodo Task asíncrono el cual retorna void síncronamente
        /// USAGE: AsyncUtil.RunSync(() => AsyncMethod());
        /// </summary>
        /// <param name="task">Task method to execute</param>
        public static void RunSync(Func<Task> task)
                => _taskFactory
                    .StartNew(task)
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult();

        /// <summary>
        /// Executa un metodo Task<T> asíncrono el cual retorna T síncronamente
        /// USAGE: T result = AsyncUtil.RunSync(() => AsyncMethod<T>());
        /// </summary>
        /// <typeparam name="TResult">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task)
                => _taskFactory
                    .StartNew(task)
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult();
    }
}
