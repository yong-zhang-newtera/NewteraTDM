/*
* @(#)ApiListViewItem.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Api;
    using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Represents a ListView item for a class api
	/// </summary>
	/// <version>  1.0.1 21 Oct 2015</version>
	public class ApiListViewItem : ListViewItem
	{
		private ClassElement _clsElement;
		private Api _api;

        /// <summary>
        /// Initializes a new instance of the ApiListViewItem class.
        /// </summary>
        /// <param name="clsElement">The ClassElement that the event is defined for.</param>
        /// <param name="api">The Api instance</param>
        public ApiListViewItem(ClassElement clsElement, Api api) : base(api.Name)
		{
            _clsElement = clsElement;
			_api = api;
		}

		/// <summary> 
		/// Gets the ClassElement instance
		/// </summary>
		/// <value> The ClassElement instance</value>
		public ClassElement ClassElement
		{
			get
			{
				return _clsElement;
			}
		}

		/// <summary> 
		/// Gets the Api instance
		/// </summary>
		/// <value> The Subscriber instance</value>
        public Api Api
		{
			get
			{
				return _api;
			}
		}
	}
}