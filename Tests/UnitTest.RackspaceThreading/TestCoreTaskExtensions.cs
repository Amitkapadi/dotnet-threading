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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally1_NullAntecedent_CompletedCleanup()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Action<Task> finallyAction = task =>
                {
                };

            CoreTaskExtensions.Finally(antecedent, finallyAction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally1_CompletedAntecedent_NullCleanup()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> finallyAction = null;

            CoreTaskExtensions.Finally(antecedent, finallyAction);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally2_NullAntecedent_CompletedCleanup()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Func<Task, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                });

            CoreTaskExtensions.Finally(antecedent, finallyFunc);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally2_CompletedAntecedent_NullCleanupFunction()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> finallyFunc = null;

            CoreTaskExtensions.Finally(antecedent, finallyFunc);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally3_NullAntecedent_CompletedCleanup()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = null;
            Action<Task<object>> finallyAction = task =>
                {
                };

            CoreTaskExtensions.Finally(antecedent, finallyAction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Action{Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally3_CompletedAntecedent_NullCleanup()
        {
            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);
            Action<Task<object>> finallyAction = null;

            CoreTaskExtensions.Finally(antecedent, finallyAction);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally4_NullAntecedent_CompletedCleanup()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<object> antecedent = null;
            Func<Task<object>, Task> finallyFunc = task =>
                Task.Factory.StartNew(() =>
                {
                });

            CoreTaskExtensions.Finally(antecedent, finallyFunc);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Finally{TResult}(Task{TResult}, Func{Task{TResult}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFinally4_CompletedAntecedent_NullCleanupFunction()
        {
            // declaring these makes it clear we are testing the correct overload
            object result = new object();
            Task<object> antecedent = CompletedTask.FromResult(result);
            Func<Task<object>, Task> finallyFunc = null;

            CoreTaskExtensions.Finally(antecedent, finallyFunc);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect1_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Action<Task> continuationAction = task =>
                {
                };

            CoreTaskExtensions.Select(antecedent, continuationAction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect1_CompletedAntecedent_NullContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> continuationAction = null;

            CoreTaskExtensions.Select(antecedent, continuationAction);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect2_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Action<Task> continuationAction = task =>
                {
                };
            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect2_CompletedAntecedent_NullContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> continuationAction = null;
            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect2_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Action<Task> continuationAction = task =>
                {
                };
            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select(Task, Action{Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect2_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Action<Task> continuationAction = null;
            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect3_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            object result = new object();
            Func<Task, object> continuationFunction = task => result;

            CoreTaskExtensions.Select(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect3_CompletedAntecedent_NullContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, object> continuationFunction = null;

            CoreTaskExtensions.Select(antecedent, continuationFunction);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect4_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            object result = new object();
            Func<Task, object> continuationFunction = task => result;

            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect4_CompletedAntecedent_NullContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, object> continuationFunction = null;

            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect4_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            object result = new object();
            Func<Task, object> continuationFunction = task => result;

            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TResult}(Task, Func{Task, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect4_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, object> continuationFunction = null;

            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect5_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = null;
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task => result;

            CoreTaskExtensions.Select(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect5_CompletedAntecedent_NullContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            Func<Task<List<object>>, string> continuationFunction = null;

            CoreTaskExtensions.Select(antecedent, continuationFunction);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect6_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = null;
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task => result;

            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect6_CompletedAntecedent_NullContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            Func<Task<List<object>>, string> continuationFunction = null;

            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect6_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = null;
            string result = "Test string";
            Func<Task<List<object>>, string> continuationFunction = task => result;

            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect6_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            Func<Task<List<object>>, string> continuationFunction = null;

            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationFunction, supportsErrors);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect7_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = null;
            Action<Task<string>> continuationAction = task =>
                {
                };

            CoreTaskExtensions.Select(antecedent, continuationAction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect7_CompletedAntecedent_NullContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Action<Task<string>> continuationAction = null;

            CoreTaskExtensions.Select(antecedent, continuationAction);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect8_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = null;
            Action<Task<string>> continuationAction = task =>
                {
                };
            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect8_CompletedAntecedent_NullContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Action<Task<string>> continuationAction = null;
            bool supportsErrors = false;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
        }

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect8_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = null;
            Action<Task<string>> continuationAction = task =>
                {
                };
            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Select{TSource}(Task{TSource}, Action{Task{TSource}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSelect8_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Action<Task<string>> continuationAction = null;
            bool supportsErrors = true;

            CoreTaskExtensions.Select(antecedent, continuationAction, supportsErrors);
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

        #region Then 1

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen1_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                });

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen1_CompletedAntecedent_NullContinuationFunction()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> continuationFunction = null;

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CompletedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CanceledAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_FaultedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CompletedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CanceledAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_FaultedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CompletedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_CanceledAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen1_FaultedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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

        #region Then 2

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen2_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                });
            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen2_CompletedAntecedent_NullContinuationFunction_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> continuationFunction = null;
            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = false;

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen2_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                });
            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen2_CompletedAntecedent_NullContinuationFunction_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> continuationFunction = null;
            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            Func<Task, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_CanceledAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then(Task, Func{Task, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen2_FaultedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Func<Task, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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

        #region Then 3

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen3_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() => result);

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen3_CompletedAntecedent_NullContinuationFunction()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task<object>> continuationFunction = null;

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            Task<object> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CompletedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CanceledAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_FaultedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CompletedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CanceledAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_FaultedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CompletedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_CanceledAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen3_FaultedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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

        #region Then 4

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen4_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() => result);

            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen4_CompletedAntecedent_NullContinuationFunction_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task<object>> continuationFunction = null;
            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = false;

            Task<object> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen4_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = null;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() => result);

            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen4_CompletedAntecedent_NullContinuationFunction_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            Func<Task, Task<object>> continuationFunction = null;
            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = true;

            Task<object> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;
            object result = new object();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = true;

            Task<object> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                Task.Factory.StartNew<object>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Default;

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_CanceledAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task antecedent = CompletedTask.Canceled();

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen4_FaultedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<object> faultedCompletionSource = new TaskCompletionSource<object>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task antecedent = faultedCompletionSource.Task;

            Func<Task, Task<object>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task<object> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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

        #endregion

        #region Then 5

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen5_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = null;
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() => result);

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen5_CompletedAntecedent_NullContinuationFunction()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            Func<Task<List<object>>, Task<string>> continuationFunction = null;

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            Task<string> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CompletedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CanceledAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_FaultedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CompletedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CanceledAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_FaultedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CompletedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_CanceledAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen5_FaultedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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

        #region Then 6

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen6_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = null;
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() => result);

            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen6_CompletedAntecedent_NullContinuationFunction_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            Func<Task<List<object>>, Task<string>> continuationFunction = null;
            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = false;

            Task<string> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen6_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = null;
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() => result);

            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen6_CompletedAntecedent_NullContinuationFunction_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            Func<Task<List<object>>, Task<string>> continuationFunction = null;
            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = true;

            Task<string> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;
            string result = "Test string";
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    return result;
                });

            bool supportsErrors = true;

            Task<string> combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.AreSame(result, combinedTask.Result);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Unreachable");
                }, cts.Token);

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                Task.Factory.StartNew<string>(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.FromResult(new List<object>());

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_CanceledAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<List<object>> antecedent = CompletedTask.Canceled<List<object>>();

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen6_FaultedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<List<object>> faultedCompletionSource = new TaskCompletionSource<List<object>>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<List<object>> antecedent = faultedCompletionSource.Task;

            Func<Task<List<object>>, Task<string>> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task<string> combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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

        #endregion

        #region Then 7

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen7_NullAntecedent_CompletedContinuation()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = null;
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                });

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen7_CompletedAntecedent_NullContinuationFunction()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Func<Task<string>, Task> continuationFunction = null;

            CoreTaskExtensions.Then(antecedent, continuationFunction);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CompletedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CanceledAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_FaultedAntecedent_CompletedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CompletedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CanceledAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_FaultedAntecedent_CanceledContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CompletedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CanceledAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_FaultedAntecedent_FaultedContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CompletedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CanceledAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_FaultedAntecedent_FaultedPreContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CompletedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_CanceledAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen7_FaultedAntecedent_NullContinuation()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction);
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

        #region Then 8

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen8_NullAntecedent_CompletedContinuation_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = null;
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                });
            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen8_CompletedAntecedent_NullContinuationFunction_NoErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Func<Task<string>, Task> continuationFunction = null;
            bool supportsErrors = false;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = false;

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_CompletedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_CanceledContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_FaultedContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_FaultedPreContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_NullContinuation_NoErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = false;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen8_NullAntecedent_CompletedContinuation_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = null;
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                });
            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThen8_CompletedAntecedent_NullContinuationFunction_SupportsErrors()
        {
            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Func<Task<string>, Task> continuationFunction = null;
            bool supportsErrors = true;

            CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_CompletedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;
            Func<Task<string>, Task> continuationFunction = task => Task.Factory.StartNew(() => executed = true);
            bool supportsErrors = true;

            Task combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
            combinedTask.Wait();
            Assert.AreEqual(TaskStatus.RanToCompletion, combinedTask.Status);
            Assert.IsTrue(executed);
        }

        /// <summary>
        /// This method test the behavior of the
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_CanceledContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            CancellationTokenSource cts = new CancellationTokenSource();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    cts.Cancel();
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_FaultedContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                Task.Factory.StartNew(() =>
                {
                    executed = true;
                    throw continuationException;
                });

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_FaultedPreContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Exception continuationException = new InvalidOperationException();
            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    throw continuationException;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CompletedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.FromResult("Test string");

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_CanceledAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            Task<string> antecedent = CompletedTask.Canceled<string>();

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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
        /// <see cref="CoreTaskExtensions.Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
        /// method.
        /// </summary>
        [TestMethod]
        public void TestThen8_FaultedAntecedent_NullContinuation_SupportsErrors()
        {
            bool executed = false;

            // declaring these makes it clear we are testing the correct overload
            TaskCompletionSource<string> faultedCompletionSource = new TaskCompletionSource<string>();
            Exception expectedException = new ArgumentException();
            faultedCompletionSource.SetException(expectedException);
            Task<string> antecedent = faultedCompletionSource.Task;

            Func<Task<string>, Task> continuationFunction = task =>
                {
                    executed = true;
                    return null;
                };

            bool supportsErrors = true;

            Task combinedTask = null;

            try
            {
                combinedTask = CoreTaskExtensions.Then(antecedent, continuationFunction, supportsErrors);
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

        #endregion
    }
}
