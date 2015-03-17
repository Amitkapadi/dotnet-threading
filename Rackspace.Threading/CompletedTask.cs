// Copyright (c) Rackspace, US Inc. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Rackspace.Threading
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides static methods to create completed <see cref="Task"/> and <see cref="Task{TResult}"/> instances.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public static class CompletedTask
    {
        /// <summary>
        /// Gets a completed <see cref="Task"/>.
        /// </summary>
        /// <value>A completed <see cref="Task"/>.</value>
        public static Task Default
        {
            get
            {
                return Task.FromResult(default(VoidResult));
            }
        }

        /// <summary>
        /// Gets a canceled <see cref="Task"/>.
        /// </summary>
        /// <returns>A canceled <see cref="Task"/>.</returns>
        public static Task Canceled()
        {
            return Canceled<VoidResult>();
        }

        /// <summary>
        /// Gets a canceled <see cref="Task{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">The task result type.</typeparam>
        /// <returns>A canceled <see cref="Task{TResult}"/>.</returns>
        public static Task<TResult> Canceled<TResult>()
        {
            TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
            completionSource.SetCanceled();
            return completionSource.Task;
        }
    }
}
