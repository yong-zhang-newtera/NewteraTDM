/*
* @(#) UserManagerFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.UsrMgr
{
	using System;
	using System.Data;
	using System.Runtime.Remoting;

	using Newtera.Common.MetaData.Principal;
	using Newtera.Server.UsrMgr;

	/// <summary>
	/// Creates a IUserManager  based on settings in the application configuration file.
	/// </summary>
	/// <version> 	1.0.0	23 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class UserManagerFactory
	{		
		// Static factory object, all invokers will use this factory object.
		private static UserManagerFactory theFactory;

		private IUserManager _customUserManager = null;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private UserManagerFactory()
		{
		}

		/// <summary>
		/// Gets the UserManagerFactory instance.
		/// </summary>
		/// <returns> The UserManagerFactory instance.</returns>
		static public UserManagerFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a IUserManager based on the config settings.
		/// </summary>
		/// <returns>A IUserManager object</returns>
		public IUserManager Create()
		{
			IUserManager userManager = null;
			
			DirectoryConfig directoryConfig = DirectoryConfig.Instance;

			switch (directoryConfig.GetDirectoryType())
			{
				case DirectoryType.Newtera:
					userManager = new CMUserManager();
					break;

				case DirectoryType.ActiveDirectory:
					ActiveDirectoryManager activeDirectoryManager = new ActiveDirectoryManager();
					activeDirectoryManager.ConnectionString = directoryConfig.GetConnectionString();
					activeDirectoryManager.UserName = directoryConfig.GetUserName();
					activeDirectoryManager.Password = directoryConfig.GetUserPassword();
					userManager = activeDirectoryManager;
					break;

				case DirectoryType.Custom:

					if (_customUserManager == null)
					{
						string typeName = directoryConfig.GetCustomTypeName();
						userManager = CreateCustomUserManager(typeName);
						_customUserManager = userManager;
					}
					else
					{
						userManager = _customUserManager;
					}

					break;
			}

			return userManager;
		}

		/// <summary>
		/// Create a custom user manager based on the type name obtained from
		/// web.config file
		/// </summary>
		/// <param name="typeName">The type name of the custom user manager</param>
		/// <returns>An IUserMananger object</returns>
		private IUserManager CreateCustomUserManager(string typeName)
		{
			IUserManager userManager = null;

			if (typeName != null)
			{
				int index = typeName.IndexOf(",");
				string assemblyName = null;
				string className;

				if (index > 0)
				{
					className = typeName.Substring(0, index).Trim();
					assemblyName = typeName.Substring(index + 1).Trim();
				}
				else
				{
					className = typeName.Trim();
				}
				ObjectHandle obj;
				try
				{
					// try to create a converter from loaded assembly first
					obj = Activator.CreateInstance(assemblyName, className);
					userManager = (IUserManager) obj.Unwrap();
				}
				catch (Exception)
				{
					obj = null;
				}

				if (obj == null)
				{
					// try to create it from a dll
					string dllPath = AppDomain.CurrentDomain.BaseDirectory + assemblyName + ".dll";
					obj = Activator.CreateInstanceFrom(dllPath, className);
					userManager = (IUserManager) obj.Unwrap();
				}
			}
			else
			{
				throw new Exception("Web.config does not have CustomTypeName defined.");
			}
	
			return userManager;
		}

		static UserManagerFactory()
		{
			// Initializing the factory.
			{
				theFactory = new UserManagerFactory();
			}
		}
	}
}