/*
* @(#)CountExecutor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace Newtera.Server.Logging
{
	/// <summary>
	/// Wrapper class for the work to be done.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ProcessingQueueEventArgs<T> : EventArgs
	{
		protected T work;
		
		public T Work
		{
			get { return work; }
		}

		public ProcessingQueueEventArgs(T work)
		{
			this.work = work;
		}
	}

	/// <summary>
	/// Wrapper class for recording an exception that occurred while processing the work packet.
	/// </summary>
	public class ProcessingQueueExceptionEventArgs : EventArgs
	{
		protected Exception exception;

		public Exception Exception
		{
			get { return exception; }
		}

		public ProcessingQueueExceptionEventArgs(Exception e)
		{
			exception = e;
		}
	}

	/// <summary>
	/// A re-usable class for processing items in a worker thread as they are queued.
	/// </summary>
	public class ProcessingQueue<T>
	{
		public delegate void DoWorkDelegate(object sender, ProcessingQueueEventArgs<T> args);
		public delegate void WorkExceptionDelegate(object sender, ProcessingQueueExceptionEventArgs args);

        public event DoWorkDelegate DoWork;
        public event WorkExceptionDelegate WorkException;

		protected Thread processThread;
		protected Queue<T> workQueue;
		protected EventWaitHandle waitProcess;
		protected bool stop;
		
		/// <summary>
		/// Constructor.  Initializes the work queue, wait process, and processing thread.
		/// </summary>
		public ProcessingQueue()
		{
			workQueue = new Queue<T>();
			waitProcess = new EventWaitHandle(false, EventResetMode.AutoReset);
			processThread = new Thread(new ThreadStart(ProcessQueueWork));
			processThread.IsBackground = true;
			processThread.Start();
		}

		/// <summary>
		/// Enqueue a work item.
		/// </summary>
		/// <param name="work"></param>
		public void QueueForWork(T work)
		{
			lock (workQueue)
			{
				workQueue.Enqueue(work);
			}

			waitProcess.Set();

		}

		/// <summary>
		/// Stop the work processing thread.
		/// </summary>
		public void Stop()
		{
			stop = true;
			waitProcess.Set();
		}

		/// <summary>
		/// Process queued work.
		/// </summary>
		protected void ProcessQueueWork()
		{
			while (!stop)
			{
				// Wait for some work.
				waitProcess.WaitOne();
				bool haveWork;

				// Finish remaining work before stopping.
				do
				{
					// Initialize to the default work value.
					T work = default(T);
					// Assume no work.
					haveWork = false;

					// Prevent enqueing from a different thread.
					lock (workQueue)
					{
						// Do we have work?  This might be 0 if stopping or if all work is processed.
						if (workQueue.Count > 0)
						{
							// Get the work.
							work = workQueue.Dequeue();
							// Yes, we have work.
							haveWork = true;
						}
					}

					// If we have work...
					if (haveWork)
					{
						try
						{
							// Try processing it.
							OnDoWork(new ProcessingQueueEventArgs<T>(work));
						}
						catch (Exception e)
						{
							// Oops, inform application of a work error.
							OnWorkException(new ProcessingQueueExceptionEventArgs(e));
						}
					}

				} while (haveWork);	// continue processing if there was work.
			}
		}

		/// <summary>
		/// Override this method if you want to handle work in a derived class.  This method
		/// calls any events wired in to the DoWork event.
		/// </summary>
		/// <param name="workEventArgs"></param>
		protected virtual void OnDoWork(ProcessingQueueEventArgs<T> workEventArgs)
		{
			if (DoWork != null)
			{
				DoWork(this, workEventArgs);
			}
		}

		/// <summary>
		/// Override this method if you want to handle work exceptions in a derived class.
		/// This method calls any events wired in to the WorkException event.
		/// </summary>
		/// <param name="workExceptionArgs"></param>
		protected virtual void OnWorkException(ProcessingQueueExceptionEventArgs workExceptionArgs)
		{
			if (WorkException != null)
			{
				WorkException(this, workExceptionArgs);
			}
		}
	}
}
