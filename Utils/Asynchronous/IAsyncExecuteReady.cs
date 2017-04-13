using System;

namespace Utils.Asynchronous
{
    /// <summary>
    /// Interface indicating that concrete class
    /// is ready for parallel execution using custom
    /// async execute library
    /// </summary>
    public interface IAsyncExecuteReady<in T>
    {
        /// <summary>
        /// Executes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Execute(T item);

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        void HandleException(Exception ex);
    }
}
