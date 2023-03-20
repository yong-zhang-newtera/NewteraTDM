/*
* @(#)ConverterFactory.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.Converter
{
	using System;
	using System.Xml;
	using System.Runtime.Remoting;
	using System.Collections;

	/// <summary>
	/// A singleton class that creates instances of IDataSourceConverter
	/// </summary>
	/// <version>1.0.0 08 Sep 2004 </version>
	/// <author> Yong Zhang </author>
	public class ConverterFactory
	{
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ConverterFactory theFactory;
		
		static ConverterFactory()
		{
			theFactory = new ConverterFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ConverterFactory()
		{
		}

		/// <summary>
		/// Gets the ConverterFactory instance.
		/// </summary>
		/// <returns> The ConverterFactory instance.</returns>
		static public ConverterFactory Instance
		{
			get
			{
				return theFactory;
			}
		}

        /// <summary>
		/// Creates an instance of IMappingNode type based on a type name of the
		/// converter.
		/// </summary>
		/// <param name="typeName">The type name</param>
		/// <returns>A IDataSourceConverter instance</returns>
		/// <remarks>The converter type name format is ClassName,AssemblyName, for example:
		///  "Newtera.Common.MetaData.Mappings.Converter.DelimitedTextFileConverter, Newtera.Common"</remarks>
        public IDataSourceConverter Create(string typeName)
        {
            return Create(typeName, null);
        }
		
		/// <summary>
		/// Creates an instance of IMappingNode type based on a type name of the
		/// converter.
		/// </summary>
		/// <param name="typeName">The type name</param>
        /// <param name="assemblyDir">The directory where the converter assembly are installed , null to indicate the assembly is at the default path.</param>
		/// <returns>A IDataSourceConverter instance</returns>
		/// <remarks>The converter type name format is ClassName,AssemblyName, for example:
		///  "Newtera.Common.MetaData.Mappings.Converter.DelimitedTextFileConverter, Newtera.Common"</remarks>
        public IDataSourceConverter Create(string typeName, string assemblyDir)
		{
			if (typeName == null)
			{
				return null;
			}

			IDataSourceConverter converter = null;

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
					converter = (IDataSourceConverter) obj.Unwrap();
				}
				catch (Exception)
				{
					obj = null;
				}

				if (obj == null)
				{
					// try to create it from a dll
                    string basePath;
                    if (assemblyDir != null)
                    {
                        basePath = assemblyDir;
                    }
                    else
                    {
                        basePath = AppDomain.CurrentDomain.BaseDirectory; // default location
                    }

                    string dllPath = basePath + assemblyName + ".dll";
					obj = Activator.CreateInstanceFrom(dllPath, className);
					converter = (IDataSourceConverter) obj.Unwrap();
				}
			}

            if (converter == null)
            {
                throw new Exception("Unable to create a converter for " + typeName);
            }

			return converter;
		}
	}
}