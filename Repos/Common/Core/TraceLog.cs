/*
* @(#) TraceLog.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;
	using System.IO;

	/// <summary>
	/// Provides an utility class for keeping server side trace log
	/// </summary>
	/// <version> 	1.0.0 05 May 2007 </version>
	public class TraceLog
	{
        private const int FILE_SIZE_LIMIT = 1000000;
        private const string LOG_FILE_SUB_PATH = @"temp\TraceLog.txt";

        private string _logFilePath = null;
        private bool _isEnabled = false;

		// Static factory object, all invokers will use this factory object.
		private static TraceLog theLog;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private TraceLog()
		{
            if (_logFilePath == null)
            {
                _logFilePath = NewteraNameSpace.GetAppHomeDir();
                if (!_logFilePath.EndsWith(@"\"))
                {
                    _logFilePath += @"\";
                }

                _logFilePath += LOG_FILE_SUB_PATH;
            }
		}

		/// <summary>
		/// Gets the TraceLog instance.
		/// </summary>
		/// <returns> The TraceLog instance.</returns>
		static public TraceLog Instance
		{
			get
			{
				return theLog;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the trace log is enabled or not.
        /// default is false.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        /// <summary>
        /// Gets the TextWriter of the trace log
        /// </summary>
        public TextWriter GetTextWriter()
        {
            return CreateStreamWriter();
        }
		
		/// <summary>
		/// Write a trace messages to the trace log
		/// </summary>
		/// <param name="message">A meesage to be written to the log</param>
		public void WriteLines(string[] messages)
		{
            if (Enabled)
            {
                StreamWriter sw = null;

                try
                {
                    sw = CreateStreamWriter();

                    sw.WriteLine("************ Message Begin ***********");
                    sw.WriteLine("Timestamp: " + DateTime.Now.ToString());
                    foreach (string msg in messages)
                    {
                        sw.WriteLine(msg);
                    }
                    sw.WriteLine("************ Message End *************");
                    sw.WriteLine("");
                }
                catch (Exception)
                {
                    // ignore the error
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
            }
		}

        /// <summary>
        /// Gets the log text
        /// </summary>
        /// <returns></returns>
        public string GetLogText()
        {
            string text = "";

            if (File.Exists(this._logFilePath))
            {
                using (StreamReader sr = new StreamReader(this._logFilePath))
                {
                    text = sr.ReadToEnd();
                }
            }

            return text;
        }

        /// <summary>
        /// Clear server log
        /// </summary>
        public void ClearLog()
        {
            if (File.Exists(this._logFilePath))
            {
                // delete the log file
                File.Delete(this._logFilePath);
            }
        }

        /// <summary>
        /// Create a StreamWriter of the trace log. Caller is responsible for
        /// closing the StreamWriter returned.
        /// </summary>
        /// <returns></returns>
        private StreamWriter CreateStreamWriter()
        {
            StreamWriter sw = null;

            if (File.Exists(this._logFilePath))
            {
                FileInfo fileInfo = new FileInfo(this._logFilePath);
                if (fileInfo.Length > FILE_SIZE_LIMIT)
                {
                    // log has grown too big, delete it and start a new one
                    File.Delete(this._logFilePath);

                    sw = new StreamWriter(this._logFilePath);
                }
                else
                {
                    // open the log and append the message to the log
                    FileStream fs = new FileStream(this._logFilePath, FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs);
                }
            }
            else
            {
                sw = new StreamWriter(this._logFilePath);
            }

            return sw;
        }

		static TraceLog()
		{
			// Initializing the factory.
			{
				theLog = new TraceLog();
			}
		}
	}
}