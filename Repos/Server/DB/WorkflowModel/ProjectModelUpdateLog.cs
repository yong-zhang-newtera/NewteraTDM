/*
* @(#)ProjectModelUpdateLog.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Keep the log of project model updates and provide a summary of execution status
	/// </summary>
	/// <version>  	1.0.0 15 Dec 2006 </version>
	public class ProjectModelUpdateLog
	{
		private StringBuilder _log;
		private int _dmlCount;
		private int _errorCount;
		private bool _showConsole;

		/// <summary>
		/// Initializes a new instance of the ProjectModelUpdateLog class
		/// </summary>
		public ProjectModelUpdateLog()
		{
			_log = new StringBuilder();
			_dmlCount = 0;
			_errorCount = 0;
			_showConsole = false;
		}

		/// <summary>
		/// Gets information indicating whether there are errors occured during
		/// the execution of project model updates
		/// </summary>
		/// <value>True if there are errors, false otherwise.</value>
		public bool HasError
		{
			get
			{
				if (_errorCount > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whenther to display
		/// the log on Console. This is for running unit test purpose only.
		/// </summary>
		/// <value>true if the log is displayed on console, false otherwise. Default is false.</value>
		public bool ShowConsole
		{
			get
			{
				return _showConsole;
			}
			set
			{
				_showConsole = value;
			}
		}

		/// <summary>
		/// Gets the summary of execution log
		/// </summary>
		public string Summary
		{
			get
			{
				StringBuilder summary = new StringBuilder();

				summary.Append("********* Project ModelUpdate Log *********\n\n");

				summary.Append(_log.ToString()).Append("\n\n");

				summary.Append("********* Summary *********\n\n");
				
				summary.Append("Number of DDL executed : ").Append(_dmlCount).Append("\n");

				summary.Append("Number of Failed DDL : ").Append(_errorCount).Append("\n");

				return summary.ToString();
			}
		}

		public void Append(string log, LogType type)
		{
			_log.Append(log).Append("\n\n");

			if (_showConsole)
			{
				System.Console.WriteLine(log);
				System.Console.WriteLine("");
			}

			switch (type)
			{
				case LogType.DML:

					_dmlCount++;
					break;

				case LogType.Error:

					_errorCount++;
					break;
			}
		}
	}

	/// <summary>
	/// Types of log entries.
	/// </summary>
	public enum LogType
	{
		None,
		DML,
		Error
	}
}