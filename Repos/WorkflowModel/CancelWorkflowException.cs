/*
* @(#)CancelWorkflowException.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Resources;
	using System.Reflection;
    using System.Runtime.Serialization;

	/// <summary>
	/// Throw when cancelling a workflow instance execution
	/// </summary>
    [Serializable]
	public class CancelWorkflowException : ApplicationException
	{
        public CancelWorkflowException()
        {
        }

		/// <summary>
		/// Initializing a CancelWorkflowException object.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public CancelWorkflowException(string reason):base(reason)
		{	
		}

        //����ʵ����ISerializable�ӿڵģ�����Ҳ���������л����캯�����������л�ʱ�����
        protected CancelWorkflowException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
	}
}