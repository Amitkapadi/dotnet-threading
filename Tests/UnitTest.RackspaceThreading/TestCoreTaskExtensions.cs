﻿// Copyright (c) Rackspace, US Inc. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace UnitTest.RackspaceThreading
{
#if !NET40PLUS
    extern alias tpl;
#endif

    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rackspace.Threading;

#if !NET40PLUS
    using tpl::System.Threading;
    using tpl::System.Threading.Tasks;
    using AggregateException = tpl::System.AggregateException;
#else
    using System.Threading;
    using System.Threading.Tasks;
#endif

    [TestClass]
    public class TestCoreTaskExtensions
    {
        #region Finally 1

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally1_CompletedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> finallyAction = task => executed = true;

            Task combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally1_CanceledAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Action<Task> finallyAction = task => executed = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally1_FaultedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Action<Task> finallyAction = task => executed = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally1_CompletedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception cleanupException = new InvalidOperationException();
            Action<Task> finallyAction = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally1_CanceledAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception cleanupException = new InvalidOperationException();
            Action<Task> finallyAction = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally1_FaultedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Action<Task> finallyAction = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Finally 2

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CompletedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> finallyFunc = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CanceledAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Func<Task, Task> finallyFunc = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_FaultedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Func<Task, Task> finallyFunc = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CompletedAntecedent_CanceledCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CanceledAntecedent_CanceledCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_FaultedAntecedent_CanceledCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CompletedAntecedent_FaultedPreCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CanceledAntecedent_FaultedPreCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_FaultedAntecedent_FaultedPreCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CompletedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw cleanupException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CanceledAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw cleanupException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_FaultedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw cleanupException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CompletedAntecedent_NullCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc =
                task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_CanceledAntecedent_NullCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc =
                task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally2_FaultedAntecedent_NullCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Func<Task, Task> finallyFunc =
                task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Finally 3

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally3_CompletedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);
            Action<Task<object>> finallyAction = task => executed = true;

            Task<object> combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally3_CanceledAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();
            Action<Task<object>> finallyAction = task => executed = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally3_FaultedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;
            Action<Task<object>> finallyAction = task => executed = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally3_CompletedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);

            Exception cleanupException = new InvalidOperationException();
            Action<Task<object>> finallyAction = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally3_CanceledAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();

            Exception cleanupException = new InvalidOperationException();
            Action<Task<object>> finallyAction = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally3_FaultedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Action<Task<object>> finallyAction = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Finally 4

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CompletedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);
            Func<Task<object>, Task> finallyFunc = task => Task.Factory.StartNew(() => executed = true);

            Task<object> combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CanceledAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();
            Func<Task<object>, Task> finallyFunc = task => Task.Factory.StartNew(() => executed = true);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_FaultedAntecedent_CompletedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;
            Func<Task<object>, Task> finallyFunc = task => Task.Factory.StartNew(() => executed = true);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CompletedAntecedent_CanceledCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CanceledAntecedent_CanceledCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_FaultedAntecedent_CanceledCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CompletedAntecedent_FaultedPreCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CanceledAntecedent_FaultedPreCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_FaultedAntecedent_FaultedPreCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc = task =>
                {
                    executed = true;
                    throw cleanupException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CompletedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw cleanupException;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CanceledAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw cleanupException;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_FaultedAntecedent_FaultedCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw cleanupException;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(cleanupException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CompletedAntecedent_NullCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc =
                task =>
                {
                    executed = true;
                    return null;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_CanceledAntecedent_NullCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = CompletedTask.Canceled<object>();

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc =
                task =>
                {
                    executed = true;
                    return null;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestFinally4_FaultedAntecedent_NullCleanup()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<object> antecedent = faultedCompletionSource.Task;

            Exception cleanupException = new InvalidOperationException();
            Func<Task<object>, Task> finallyFunc =
                task =>
                {
                    executed = true;
                    return null;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Finally(antecedent, finallyFunc);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Select 1

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect1_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> continuationAction = task => executed = true;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect1_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Action<Task> continuationAction = task => executed = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect1_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Action<Task> continuationAction = task => executed = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect1_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect1_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect1_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        #endregion

        #region Select 2

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> continuationAction = task => executed = true;
            bool supportsErrors = false;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Action<Task> continuationAction = task => executed = true;
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Action<Task> continuationAction = task => executed = true;
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> continuationAction = task => executed = true;
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Action<Task> continuationAction = task => executed = true;
            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Action<Task> continuationAction = task => executed = true;
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect2_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Action<Task> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Select 3

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect3_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            Task<object> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect3_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect3_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect3_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect3_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect3_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        #endregion

        #region Select 4

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            object result = new object();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect4_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, object> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Select 5

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect5_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            Task<string> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect5_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect5_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect5_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect5_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect5_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        #endregion

        #region Select 6

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    return result;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect6_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, string> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        #endregion

        #region Select 7

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect7_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Action<Task<string>> continuationAction = task => executed = true;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect7_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();
            Action<Task<string>> continuationAction = task => executed = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect7_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;
            Action<Task<string>> continuationAction = task => executed = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect7_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect7_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect7_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        #endregion

        #region Select 8

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Action<Task<string>> continuationAction = task => executed = true;
            bool supportsErrors = false;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();
            Action<Task<string>> continuationAction = task => executed = true;
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;
            Action<Task<string>> continuationAction = task => executed = true;
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an ArgumentException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(expectedException, ex.InnerExceptions[0]);
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Action<Task<string>> continuationAction = task => executed = true;
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();
            Action<Task<string>> continuationAction = task => executed = true;
            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;
            Action<Task<string>> continuationAction = task => executed = true;
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected a TaskCanceledException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Canceled, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.IsInstanceOfType(ex.InnerExceptions[0], typeof(TaskCanceledException));
                Assert.IsFalse(executed);
            }
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestSelect8_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Action<Task<string>> continuationAction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
                combinedTask.Wait();
                Assert.Fail("Expected an InvalidOperationException wrapped in an AggregateException");
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(combinedTask, "Failed to create the combined task.");
                Assert.AreEqual(TaskStatus.Faulted, combinedTask.Status);
                Assert.AreEqual(1, ex.InnerExceptions.Count);
                Assert.AreSame(continuationException, ex.InnerExceptions[0]);
                Assert.IsTrue(executed);
            }
        }

        #endregion
    }
}
