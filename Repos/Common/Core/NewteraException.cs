/*
* @(#)NewteraException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Resources;
	using System.Reflection;

	/// <summary>
	/// Provides the base functionality for exceptions in Newtera Namespace
	/// </summary>
	/// <version>      1.0.0 2 JUL 2003
	/// </version>
	/// <author> Yong Zhang </author>
	/// <example>
	///
	///	class ExceptionTestClass 
	///	{
	///		public static void Main() 
	///		{
	///			try 
	///			{
	///				// do something;
	///			}
	///			catch (NewteraException e) 
	///			{
	///				// display localized message
	///				Console.WriteLine(e.LocalizedMessage);
	///			}
	///			catch (Exception e) 
	///			{
	///				// display unlocalized error message
	///				Console.WriteLine(e.Message);
	///			}
	///		}    
	///	}
	/// </example>
	abstract public class NewteraException : ApplicationException
	{
		private string _localizedMsg = null;

		/// <summary>
		/// Initializing a NewteraException object.
		/// </summary>
		/// <param name="reason">a description of the exception.</param>
		public NewteraException(string reason):base(reason)
		{	
		}
		
		/// <summary>
		/// Initializing a NewteraException object.
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		public NewteraException(string reason, Exception ex) : base(reason, ex)
		{
		}

		/// <summary>
		/// Gets or sets a localized error message for the exception.
		/// </summary>
		/// <value> The localized message. The default is what returned from Message property</value>
		public string LocalizedMessage
		{
			get
			{
				string localizedMsg = null;

				if (_localizedMsg != null)
				{
					return _localizedMsg;
				}
				else if ((localizedMsg = LocalizedMessageStore.TheInstance.GetLocalizedError(this)) != null)
				{
					return localizedMsg;
				}
				else
				{
					localizedMsg = this.Message; // Use the default
				}
				
				return localizedMsg;
			}
			set
			{
				_localizedMsg = value;
			}
		}

		/// <summary>
		/// Sets the localized error message with the arguments in the message
		/// replaced by provided argument values.
		/// </summary>
		/// <param name="arguments">Array of arguments. arguments[n] 
		/// corresponds to {n} in the resource bundle message template.
		/// </param>
		/// <returns>The formated message</returns>
		/// <example>
		/// <code>
		/// try
		/// {
		///		Login(dbName, usrName, passwd);
		///	}
		///	catch (InvalidUserNameException ex)
		///	{
		///		string localizedMsg = ex.GetFormatedLocalizedMessage(dbName, usrName);
		///		Console.WriteLine(localizedMsg);
		/// }
		/// </code>
		/// Message before formatting:
		/// 
		/// User {0} does not have an account for DB {1}
		/// 
		/// Message after formatting:
		/// 
		/// User David does not have an account for DB ABC
		/// </example>
		public string GetFormatedLocalizedMessage(params object[] arguments)
		{
			return LocalizedMessageStore.TheInstance.GetLocalizedError(this, arguments);
		}
	}

	/// <summary>
	/// Provides methods to retrieve a localized message base on an exception type
	/// </summary>
	/// <version>      1.0.0 02 JUL 2003
	/// </version>
	/// <author> Yong Zhang </author>
	/// <remarks>This class is implemented as singleton</remarks>
	internal class LocalizedMessageStore
	{
		private string _errorResource = "ErrorResource";
		private ResourceManager _resourceManager = null;
		
		/**
		 * Singleton's private instance.
		 */
		private static LocalizedMessageStore theLocalizedMessageStore;
    
		// Initializing the localed message store.
		static LocalizedMessageStore()
		{
			theLocalizedMessageStore = new LocalizedMessageStore();
		}
    
		/// <summary>
		/// The private constructor.
		/// </summary>
		private LocalizedMessageStore() 
		{       
		}
    
		/// <summary>
		/// Gets the LocalizedMessageStore instance
		/// </summary>
		/// <value>The single LocalizedMessageStore instance</value>
		public static LocalizedMessageStore TheInstance 
		{   
			get
			{
				return theLocalizedMessageStore;
			}
		}

		/// <summary>
		/// Using the default culture to get localized message for the given exception.
		/// </summary>
		/// <param name="ex">The exception</param>
		/// <returns> The localized error message.</returns>
		public string GetLocalizedError(Exception ex)
		{
			// Get the culture of the currently executing thread.
			// The value of ci will determine the culture of
			// the resources that the resource manager retrieves.
			CultureInfo ci = Thread.CurrentThread.CurrentCulture;

			return GetLocalizedError(ex, ci, null);
		}
		
		/// <summary>
		/// Using a specific culture to get localized message for the given exception.
		/// </summary>
		/// <param name="ex">The exception</param>
		/// <param name="culture">The culture</param>
		/// <returns> The localized error message.</returns>
		public string GetLocalizedError(System.Exception ex, CultureInfo culture)
		{
			return GetLocalizedError(ex, culture, null);
		}
		
		/// <summary>
		/// Using default culture to get localized message for the exception.
		/// Replaces the variables with the auguments.
		/// </summary>
		/// <param name="ex">The exception</param>
		/// <param name="args">The arguments</param>
		/// <returns> The localized error message.</returns>
		public string GetLocalizedError(System.Exception ex, params object[] args)
		{
			// Get the culture of the currently executing thread.
			// The value of ci will determine the culture of
			// the resources that the resource manager retrieves.
			CultureInfo ci = Thread.CurrentThread.CurrentCulture;
			
			return GetLocalizedError(ex, ci, args);
		}
		
		/// <summary>
		/// Using a specific culture to get localized message for the exception.
		/// Replaces the variables with the auguments.
		/// </summary>
		/// <param name="ex">The exception</param>
		/// <param name="culture">The culture info</param>
		/// <param name="args">The arguments containing user messages.</param>
		/// <returns> The localized error message.</returns>
		public string GetLocalizedError(Exception ex, CultureInfo culture, params object[] args)
		{
			string message = null;

			// Create a resource manager to retrieve resources if it is null
			if (_resourceManager == null)
			{
				_resourceManager = new ResourceManager(_errorResource, Assembly.GetExecutingAssembly());
			}
        
			/*
			* Look up a localized message from the resource manager, using the
			* classname of the exception (or its superclasses) as the resource
			* name. If no resource was found, return null.
			*/
			System.Type type = ex.GetType();
			while ((message == null) && (type != null))
			{
				// type.FullName gets the fully qualified name of the Type,
				// for example, Newtera.Common.MetaData.Schema.ReadSchemaException
				string messageName = type.FullName;
				message = _resourceManager.GetString(messageName);
				if (message == null)
				{
					// No message defined for the exception, get the message for
					// its inheried class.
					type = type.BaseType;
				}
			}

			// If there are arguments, format the message
			if (message != null && args != null)
			{
				message = string.Format(message, args);
			}
			
			return message;
		}
	}
}