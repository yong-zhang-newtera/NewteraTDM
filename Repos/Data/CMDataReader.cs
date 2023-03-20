/*
* @(#)CMDataReader.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Xml;
	using System.Data;

	using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// Provides a means of reading one or more forward-only pages of result in xml format
	/// obtained by executing a command at database.
	/// </summary>
	/// <version>  	1.0.0 08 July 2006 </version>
	/// <author>Yong Zhang </author>
	public class CMDataReader : IDisposable
	{
		// The DataReader should always be open when returned to the user.
		private bool _isOpen = true;
		private XmlDocument _doc;
		private QueryReader _queryReader;
		private bool _closeConnection;
		
		/* 
		* Keep track of the connection in order to implement the
		* CommandBehavior.CloseConnection flag. A null reference means
		* normal behavior (do not automatically close).
		*/
		private CMConnection _connection = null;
		
		/*
		* Because the user should not be able to directly create a 
		* DataReader object, the constructors are internal to the package.
		* 
		* This constructor takes result and connection
		*/
		internal CMDataReader(QueryReader queryReader, CMConnection connection) : this(queryReader, connection, false)
		{
		}

		/*
		* Because the user should not be able to directly create a 
		* DataReader object, the constructors are internal to the package.
		* 
		* This constructor takes result and connection
		*/
		internal CMDataReader(QueryReader queryReader, CMConnection connection, bool closeConnection)
		{
			_doc = null;
			_queryReader = queryReader;
			_connection = connection;
			_closeConnection = closeConnection;
		}

		/// <summary>
		/// distructor in case the CMDataReader is disposed by GC
		/// </summary>
		~CMDataReader()
		{
			this.Close();
		}

		/// <summary>
		///  Gets a value indicating whether the data reader is closed.
		/// </summary>
		/// <returns> true if the data reader is closed; otherwise, false. </returns>
		/// <remarks>IsClosed and RecordsAffected are the only properties that you can call after the IDataReader is closed.
		/// </remarks>
		public bool IsClosed
		{
			get
			{
				return !_isOpen;
			}
		}
		
		/// <summary>
		/// Return the Xml Document result of next page
		/// </summary>
		/// <returns> The Xml Document result, or an empty document if it reaches the end of result</returns>
		public XmlDocument GetXmlDocument()
		{
			return _doc;
		}
		
		/// <summary>
		/// Closes the CMDataReader 0bject, as well as the associated connection
		/// </summary>
		/// <remarks>You must explicitly call the Close method when you are through using the IDataReader to use the associated IDbConnection for any other purpose.
		/// </remarks>
		public void Close()
		{
			/*
			  * Close the reader and connection
			  */
			_isOpen = false;

            try
            {
                if (_closeConnection && _connection != null)
			{
				_connection.Close();
			}

			if (_queryReader != null)
			{

                    _queryReader.Close();
			}
            }
            catch (Exception)
            {

            }
        }
		
		/// <summary> Advances the CMDataReader to the next page.
		/// </summary>
		/// <returns> true if there are more pages; otherwise, false. </returns>
		public bool Read()
		{
			bool status = true;

			// get next result page from query reader
			_doc = _queryReader.GetNextPage();

			if (_doc.DocumentElement.ChildNodes.Count == 0)
			{
				status = false;
			}

			return status;
		}

		#region IDisposable ≥…‘±

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
			this.Close();
		}

		#endregion
	}
}