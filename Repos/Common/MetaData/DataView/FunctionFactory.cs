/*
* @(#)FunctionFactory.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates a function instance of IDataViewElement.
	/// </summary>
	/// <version>  	1.0.0 15 Oct 2007 </version>
	public class FunctionFactory
	{
        public static readonly string[] CONDITION_FUNCTIONS = new string[] {
            @"wfstate", @"before"};

        public static readonly string[] ACTION_FUNCTIONS = new string[] {
            @"error"};

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static FunctionFactory theFactory;
		
		static FunctionFactory()
		{
			theFactory = new FunctionFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private FunctionFactory()
		{
		}

		/// <summary>
		/// Gets the FunctionFactory instance.
		/// </summary>
		/// <returns> The FunctionFactory instance.</returns>
		static public FunctionFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a function instance of IDataViewElement type
		/// </summary>
		/// <param name="name">function name</param>
        /// <returns>A IFunctionElement instance</returns>
        public IFunctionElement Create(string name)
		{
			IFunctionElement function = null;

			switch (name)
			{
				case "wfstate":
                    function = new WFStateFunction();
					break;
                case "before":
                    function = new BeforeValueFunction();
                    break;
                case "error":
                    function = new ErrorFunction();
                    break;
			}

			return function;
		}
	}
}