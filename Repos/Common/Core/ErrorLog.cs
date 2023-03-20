/*
* @(#) ErrorLog.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;
	using System.IO;

	/// <summary>
	/// Provides an utility class for keeping server side error log
	/// </summary>
	/// <version> 	1.0.0	21 Jan 2007 </version>
	public class ErrorLog
	{
        private const int FILE_SIZE_LIMIT = 1000000;
        private const string LOG_FILE_SUB_PATH = @"temp\ServerLog.txt";

        private string _logFilePath = null;

		// Static factory object, all invokers will use this factory object.
		private static ErrorLog theLog;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private ErrorLog()
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
		/// Gets the ErrorLog instance.
		/// </summary>
		/// <returns> The ErrorLog instance.</returns>
		static public ErrorLog Instance
		{
			get
			{
				return theLog;
			}
		}
		
		/// <summary>
		/// Write a message to the server log
		/// </summary>
		/// <param name="message">A meesage to be written to the log</param>
		public void WriteLine(string message)
		{
            StreamWriter sw = null;

            try
            {
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

                sw.WriteLine("************ Message Begin ***********");
                sw.WriteLine("Timestamp: " + DateTime.Now.ToString());
                sw.WriteLine(message);
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

		static ErrorLog()
		{
			// Initializing the factory.
			{
				theLog = new ErrorLog();
			}
		}
	}
}