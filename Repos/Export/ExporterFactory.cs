/*
* @(#)ExporterFactory.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Export
{
	using System;
	using System.Xml;
	using System.Runtime.Remoting;
	using System.Collections;

	/// <summary>
	/// A singleton class that creates instances of IExporter
	/// </summary>
	/// <version>1.0.0 08 Sep 2004 </version>
	/// <author> Yong Zhang </author>
	public class ExporterFactory
	{
		private Hashtable _table;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ExporterFactory theFactory;
		
		static ExporterFactory()
		{
			theFactory = new ExporterFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ExporterFactory()
		{
			_table = new Hashtable();
		}

		/// <summary>
		/// Gets the ExporterFactory instance.
		/// </summary>
		/// <returns> The ExporterFactory instance.</returns>
		static public ExporterFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IExporter based on a type name of the
		/// exporter.
		/// </summary>
		/// <param name="typeName">The type name</param>
		/// <returns>A IExporter instance</returns>
		/// <remarks>The exporter type name format is ClassName,AssemblyName, for example:
		///  "Newtera.Common.MetaData.Mappings.Converter.DelimitedTextFileConverter, Newtera.Common"</remarks>
		public IExporter Create(string typeName)
		{
            lock (this)
            {
                if (typeName == null)
                {
                    return null;
                }

                // if the converter has been created, return it
                IExporter converter = (IExporter)_table[typeName];

                if (converter == null)
                {
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
                            converter = (IExporter)obj.Unwrap();
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
                            converter = (IExporter)obj.Unwrap();
                        }

                        // keep for reuse
                        _table[typeName] = converter;
                    }
                }

                return converter;
            }
		}
	}
}