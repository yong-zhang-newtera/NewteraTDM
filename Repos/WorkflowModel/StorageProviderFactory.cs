/*
* @(#) StorageProviderFactory.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// Creates an IStorageProvider instance based on storage type.
	/// </summary>
	/// <version> 	1.0.0 12 Dec 2006 </version>
	public class StorageProviderFactory
	{		
		// Static factory object, all invokers will use this factory object.
		private static StorageProviderFactory theFactory;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private StorageProviderFactory()
		{
		}

		/// <summary>
		/// Gets the StorageProviderFactory instance.
		/// </summary>
		/// <returns> The StorageProviderFactory instance.</returns>
		static public StorageProviderFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a IStorageProvider instance that use memory as storage.
		/// </summary>
		/// <returns>A IStorageProvider object.</returns>
		public IStorageProvider CreateMemoryProvider()
		{
            return new MemoryStorageProvider();
		}

        /// <summary>
        /// Creates a IStorageProvider instance that use files as storage.
        /// </summary>
        /// <param name="xomlFilePath">Xoml file path.</param>
        /// <returns>A IStorageProvider object.</returns>
        public IStorageProvider CreateFileProvider(string xomlFilePath)
        {
            return new FileStorageProvider(xomlFilePath);
        }

		static StorageProviderFactory()
		{
			// Initializing the factory.
			{
				theFactory = new StorageProviderFactory();
			}
		}
	}
}