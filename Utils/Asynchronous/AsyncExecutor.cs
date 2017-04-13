using System;
using System.Collections.Generic;
using System.Threading;

namespace Utils.Asynchronous
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncExecutor<TItem>
    {
        /// <summary>
        /// Callback function to execute logging logic on item
        /// </summary>
        /// <param name="item">The item.</param>
        public delegate void Executor(TItem item);

        /// <summary>
        /// Callback method executed when exception appear.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public delegate void ExceptionHandler(Exception ex);

        private readonly Queue<TItem> _inputQueue;
        private readonly int _batchSize;
        private readonly ManualResetEvent _work;
        private readonly WaitHandle[] _waitHandles;
        private readonly Thread _thread;
        private readonly Executor _executor;
        private readonly ExceptionHandler _exceptionHandler;

        public AsyncExecutor(IAsyncExecuteReady<TItem> executor, ManualResetEvent stop, int batchSize)
        {
            _work = new ManualResetEvent(false);
            _waitHandles = new WaitHandle[] { stop, _work };
            _inputQueue = new Queue<TItem>();
            _batchSize = batchSize;
            _executor = executor.Execute;
            _exceptionHandler = executor.HandleException;
            _thread = new Thread(Execute);
        }

        /// <summary>
        /// Enqueues the specified input items.
        /// </summary>
        /// <param name="inputItems">The input items.</param>
        public void Enqueue(IEnumerable<TItem> inputItems)
        {
            lock (_inputQueue)
            {
                foreach (var inputItem in inputItems)
                {
                    _inputQueue.Enqueue(inputItem);
                }
                IsSomethingToDo();
            }
        }

        /// <summary>
        /// Enqueues the specified input item.
        /// </summary>
        /// <param name="inputItem">The input item.</param>
        public void Enqueue(TItem inputItem)
        {
            lock (_inputQueue)
            {
                _inputQueue.Enqueue(inputItem);
                _work.Set();
            }
        }

        /// <summary>
        /// Starts this thread instance.
        /// </summary>
        public void Start()
        {
            _thread.Start();
        }

        /// <summary>
        /// Stops this thread instance.
        /// </summary>
        public void Stop()
        {
            _thread.Join(0);
        }

        /// <summary>
        /// Determines whether worker thread has anything in input queue to process.
        /// </summary>
        private void IsSomethingToDo()
        {
            lock (_inputQueue)
            {
                if (_inputQueue.Count > 0)
                {
                    _work.Set();
                }
                else
                {
                    _work.Reset();
                }
            }
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        private void DoWork()
        {
            var first = new List<TItem>();
            lock (_inputQueue)
            {
                for (int i = 0; i < _batchSize && _inputQueue.Count > 0; i++)
                {
                    first.Add(_inputQueue.Dequeue());
                }
            }

            if (first.Count == 0)
            {
                return;
            }

            foreach (var item in first)
            {
                _executor(item);
            }
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        private void HandleException(Exception ex)
        {
            _exceptionHandler(ex);
        }

        /// <summary>
        /// Executes the main thread loop.
        /// </summary>
        private void Execute()
        {
            try
            {
                while (true)
                {
                    switch (WaitHandle.WaitAny(_waitHandles))
                    {
                        case 0:
                            return;
                        case 1:
                            DoWork();
                            IsSomethingToDo();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
