using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The extensions for action retry.
    /// </summary>
    public static class RetryExtensions
    {
        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The action to process.</param>
        /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Action action, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
        {
            var result = new RetryResult();
            if (action == null) return result;
            if (needThrow == null) needThrow = ex => ex;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    action();
                    result.Success();
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = needThrow(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value, cancellationToken);
            }
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The action to process.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Action action, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry));
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process.</param>
        /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
        {
            var result = new RetryResult();
            if (action == null) return result;
            if (needThrow == null) needThrow = ex => ex;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                try
                {
                    await action(cancellationToken);
                    result.Success();
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = needThrow(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value, cancellationToken);
            }
        }

        ///// <summary>
        ///// Processes an action with this retry policy.
        ///// </summary>
        ///// <param name="policy">The retry policy.</param>
        ///// <param name="action">The async action to process.</param>
        ///// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        ///// <param name="cancellationToken">The optional cancellation token.</param>
        ///// <returns>The processing retry result.</returns>
        //public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, Func<Exception, bool> needThrow, CancellationToken cancellationToken = default)
        //{
        //    return ProcessAsync(policy, action, ex =>
        //    {
        //        return needThrow(ex) ? ex : null;
        //    }, cancellationToken);
        //}

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry));
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, CancellationToken cancellationToken, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry), cancellationToken);
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action which will return a result to process.</param>
        /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
        {
            var result = new RetryResult<T>();
            if (action == null) return result;
            if (needThrow == null) needThrow = ex => ex;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var r = await action(cancellationToken);
                    result.Success(r);
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = needThrow(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value, cancellationToken);
            }
        }

        ///// <summary>
        ///// Processes an action with this retry policy.
        ///// </summary>
        ///// <param name="policy">The retry policy.</param>
        ///// <param name="action">The async action which will return a result to process.</param>
        ///// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        ///// <param name="cancellationToken">The optional cancellation token.</param>
        ///// <returns>The processing retry result.</returns>
        //public static Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, Func<Exception, bool> needThrow, CancellationToken cancellationToken = default)
        //{
        //    return ProcessAsync(policy, action, ex =>
        //    {
        //        return needThrow(ex) ? ex : null;
        //    }, cancellationToken);
        //}

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action which will return a result to process.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry));
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action which will return a result to process.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry), cancellationToken);
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process to return a value indicating whether success and a result if success.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<(bool, T)>> action, CancellationToken cancellationToken)
        {
            var result = new RetryResult<T>();
            if (action == null) return result;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                try
                {
                    var (succ, r) = await action(cancellationToken);
                    if (succ)
                    {
                        result.Success(r);
                        return result;
                    }

                    result.Fail();
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    throw;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value, cancellationToken);
            }
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process to return an exception if failed and a result if success.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<(Exception, T)>> action, CancellationToken cancellationToken)
        {
            var result = new RetryResult<T>();
            if (action == null) return result;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var (ex, r) = await action(cancellationToken);
                    if (ex == null)
                    {
                        result.Success(r);
                        return result;
                    }

                    result.Fail(ex);
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    throw;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value, cancellationToken);
            }
        }

        /// <summary>
        /// Creates a retry record instance.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <returns>A retry record instance.</returns>
        public static RetryInstance CreateInstance(IRetryPolicy policy)
        {
            return policy?.CreateInstance() ?? new InternalRetryInstance();
        }

        private static Func<Exception, Exception> HitException(params Type[] exceptionTypesForRetry)
        {
            return ex =>
            {
                var exType = ex.GetType();
                foreach (var item in exceptionTypesForRetry)
                {
                    if (exType == item || exType.IsSubclassOf(item)) return null;
                }

                return ex;
            };
        }
    }
}
