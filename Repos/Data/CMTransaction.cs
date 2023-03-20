/*
* @(#)CMTransaction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;

	/// <summary>
	/// Represents a transaction to be performed at a catalog.
	/// </summary>
	/// <version>  	1.0.0 26 Aug 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class CMTransaction : IDbTransaction
	{
		
		private CMConnection _connection;
		
		/// <summary>
		/// The package scope constructor that takes a connection.
		/// </summary>
		internal CMTransaction(CMConnection connection)
		{
			_connection = connection;
		}
		
		/// <summary>
		/// Specifies the Connection object to associate with the transaction.
		/// </summary>
		/// <value> The CMConnection object to associate with the transaction.
		/// </value>
		public CMConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Specifies the Connection object to associate with the transaction.
		/// </summary>
		/// <value> The IDbConnection object to associate with the transaction.
		/// </value>
		IDbConnection IDbTransaction.Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Specifies the IsolationLevel for this transaction.
		/// </summary>
		/// <value>The IsolationLevel for this transaction. The default is ReadCommitted.</value>
		public IsolationLevel IsolationLevel
		{
			get
			{
				return IsolationLevel.ReadCommitted;
			}
		}
		
		/// <summary>
		/// Commits the CM transaction.
		/// </summary>
		/// <remarks>Not supported</remarks>
		public void Commit()
		{
			// no-op
		}
		
		/// <summary>
		/// Rolls back a transaction from a pending state.
		/// </summary>
		/// <remarks>Not supported</remarks>
		public void Rollback()
		{
			// no-op
		}

		/// <summary>
		/// Implementing IDisposal interface
		/// </summary>
		void IDisposable.Dispose() 
		{
			this.Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) 
		{
			/*
			 * Dispose of the object and perform any cleanup.
			 */
		}
	}
}