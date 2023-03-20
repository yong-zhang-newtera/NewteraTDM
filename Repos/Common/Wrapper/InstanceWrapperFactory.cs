/*
* @(#) InstanceWrapperFactory.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Wrapper
{
	using System;

	/// <summary>
	/// Creates an IInstanceWrapper instance based on storage type.
	/// </summary>
	/// <version> 	1.0.0 12 Dec 2006 </version>
	public class InstanceWrapperFactory
	{		
		// Static factory object, all invokers will use this factory object.
		private static InstanceWrapperFactory theFactory;
		
        // the service to get the binding instance from the database, null at design time
        private IBindingInstanceService _service;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private InstanceWrapperFactory()
		{
            _service = null;
		}

		/// <summary>
		/// Gets the InstanceWrapperFactory instance.
		/// </summary>
		/// <returns> The InstanceWrapperFactory instance.</returns>
		static public InstanceWrapperFactory Instance
		{
			get
			{
				return theFactory;
			}
		}

        /// <summary>
        /// Gets or sets the service for getting a binding instance from the database.
        /// </summary>
        public IBindingInstanceService BindingInstanceService
        {
            get
            {
                return _service;
            }
            set
            {
                _service = value;
            }
        }

        /// <summary>
        /// Creates the IInstanceWrapper object for a workflow instance of given id.
        /// </summary>
        /// <param name="workflowInstanceId">The id of workflow instance.</param>
        /// <returns>A IInstanceWrapper object.</returns>
        public IInstanceWrapper GetInstanceWrapper(Guid workflowInstanceId)
        {
            if (_service != null)
            {
                return _service.GetBindingInstance(workflowInstanceId);
            }
            else
            {
                // it is at the design time, return a dummy instance wrapper
                return new DummyInstance();
            }
        }

		static InstanceWrapperFactory()
		{
			// Initializing the factory.
			{
				theFactory = new InstanceWrapperFactory();
			}
		}
	}
}