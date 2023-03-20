/*
* @(#)LoggingProcessor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Processor
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Logging;
	using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// The LoggingProcessor that evaluates a logging request against a logging policy and
	/// return a conclusion to the client
	/// </summary>
	/// <version>1.0.0 05 Jan 2009 </version>
	internal sealed class LoggingProcessor
	{
		private RuleMatcher _ruleMatcher;

		/// <summary>
		/// Initiatiate an instance of LoggingProcessor class.
		/// </summary>
		public LoggingProcessor()
		{
			_ruleMatcher = new RuleMatcher();
		}

		/// <summary>
		/// Evaluate an logging request against a logging policy and return a
		/// Conclusion.
		/// </summary>
		/// <param name="loggingReq">The logging request.</param>
		/// <param name="policy">The logging policy.</param>
		/// <returns>A Conclusion object.</returns>
		public Conclusion Evaluate(LoggingRequest loggingReq, LoggingPolicy policy)
		{
			Conclusion conclusion = new Conclusion();

			// Get the decisions from the rules that matche the logging request.
			ArrayList decisions = _ruleMatcher.Match(loggingReq, policy);
			conclusion.DecisionList = decisions;
			
			if (decisions.Count > 0)
			{
				conclusion = GetConclusion(decisions, loggingReq.ActionType, policy);
			}
			else
			{
				// when there isn't any rules matched, take the default settings
				switch (loggingReq.ActionType)
				{
					case LoggingActionType.Read:
						conclusion.Status = policy.Setting.DefaultReadLogStatus;
						break;
					case LoggingActionType.Write:
						conclusion.Status = policy.Setting.DefaultWriteLogStatus;
						break;
					case LoggingActionType.Create:
						conclusion.Status = policy.Setting.DefaultCreateLogStatus;
						break;
					case LoggingActionType.Delete:
						conclusion.Status = policy.Setting.DefaultDeleteLogStatus;
						break;
					case LoggingActionType.Upload:
						conclusion.Status = policy.Setting.DefaultUploadLogStatus;
						break;
					case LoggingActionType.Download:
						conclusion.Status = policy.Setting.DefaultDownloadLogStatus;
						break;
                    case LoggingActionType.Import:
                        conclusion.Status = policy.Setting.DefaultImportLogStatus;
                        break;
                    case LoggingActionType.Export:
                        conclusion.Status = policy.Setting.DefaultExportLogStatus;
                        break;
				}
			}

			return conclusion;
		}

		/// <summary>
		/// Gets a conclusion of the evaluating an logging request.
		/// </summary>
		/// <param name="decisions">The list of decisions</param>
		/// <param name="actionType">The action type</param>
		/// <param name="policy">The logging policy</param>
		/// <returns>A Conclusion object</returns>
		private Conclusion GetConclusion(ArrayList decisions, LoggingActionType actionType,
			LoggingPolicy policy)
		{
			Conclusion conclusion = new Conclusion();
			conclusion.DecisionList = decisions;
			LoggingConflictResolutionType resolutionType = policy.Setting.ConflictResolutionType;

			if (HasConflict(decisions))
			{
				switch (resolutionType)
				{
					case LoggingConflictResolutionType.Offtp:
						conclusion.Status = LoggingStatus.Off;
						break;
					case LoggingConflictResolutionType.Ontp:
						conclusion.Status = LoggingStatus.On;
						break;
					case LoggingConflictResolutionType.Ntp:
						// take the default permission
						switch (actionType)
						{
							case LoggingActionType.Read:
								conclusion.Status = policy.Setting.DefaultReadLogStatus;
								break;
							case LoggingActionType.Write:
								conclusion.Status = policy.Setting.DefaultWriteLogStatus;
								break;
							case LoggingActionType.Create:
								conclusion.Status = policy.Setting.DefaultCreateLogStatus;
								break;
							case LoggingActionType.Delete:
								conclusion.Status = policy.Setting.DefaultDeleteLogStatus;
								break;
							case LoggingActionType.Upload:
								conclusion.Status = policy.Setting.DefaultUploadLogStatus;
								break;
							case LoggingActionType.Download:
								conclusion.Status = policy.Setting.DefaultDownloadLogStatus;
								break;
                            case LoggingActionType.Import:
                                conclusion.Status = policy.Setting.DefaultImportLogStatus;
                                break;
                            case LoggingActionType.Export:
                                conclusion.Status = policy.Setting.DefaultExportLogStatus;
                                break;
						}
						break;
				}
			}
			else
			{
				conclusion.Status = ((Decision) decisions[0]).Status;
			}

			return conclusion;
		}

		/// <summary>
		/// Gets the information indicating whether there is a conflict in the
		/// evaluating decisions
		/// </summary>
		/// <param name="decisions">The decisions</param>
		/// <returns>true if there is a conflict, false otherwise</returns>
		private bool HasConflict(ArrayList decisions)
		{
			bool hasConflict = false;

			if (decisions.Count > 1)
			{
				LoggingStatus status = ((Decision) decisions[0]).Status;

				for (int i = 1; i < decisions.Count; i++)
				{
					if (((Decision) decisions[i]).Status != status)
					{
						hasConflict = true;
						break;
					}
				}
			}

			return hasConflict;
		}
	}
}