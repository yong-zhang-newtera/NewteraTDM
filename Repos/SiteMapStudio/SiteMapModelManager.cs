/*
* @(#)SiteMapModelManager.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.SiteMapStudio
{
	using System;
	using System.Xml;

    using Newtera.Common.MetaData.SiteMap;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A singleton class that keeps sitemap and side menu models for fast access
	/// </summary>
	/// <version>1.0.0 14 Jun 2009 </version>
	internal class SiteMapModelManager
	{
        private SiteMapModelSet _modelSet;
        private SiteMapModel _model;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static SiteMapModelManager theManager;
		
		static SiteMapModelManager()
		{
			theManager = new SiteMapModelManager();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private SiteMapModelManager()
		{
            _model = null;
		}

		/// <summary>
		/// Gets the SiteMapModelManager instance.
		/// </summary>
		/// <returns> The SiteMapModelManager instance.</returns>
		static public SiteMapModelManager Instance
		{
			get
			{
				return theManager;
			}
		}

        public SiteMapModelSet ModelSet
        {
            get
            {
                return _modelSet;
            }
            set
            {
                _modelSet = value;
            }
        }

        public SiteMapModel SelectedSiteMapModel
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }
	}
}