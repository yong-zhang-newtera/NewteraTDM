/*
* @(#)AlgorithmFactory.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Algorithm
{
	using System;
	using System.Xml;
	using System.Runtime.Remoting;
	using System.Collections;

	/// <summary>
	/// A singleton class that creates instances of IAlgorithm
	/// </summary>
	/// <version>1.0.0 20 Aug 2007 </version>
	public class AlgorithmFactory
	{
		private Hashtable _table;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static AlgorithmFactory theFactory;
		
		static AlgorithmFactory()
		{
			theFactory = new AlgorithmFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private AlgorithmFactory()
		{
			_table = new Hashtable();
		}

		/// <summary>
		/// Gets the AlgorithmFactory instance.
		/// </summary>
		/// <returns> The AlgorithmFactory instance.</returns>
		static public AlgorithmFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IAlgorithm based on a type name of the
		/// exporter.
		/// </summary>
		/// <param name="typeName">The type name</param>
		/// <returns>A IAlgorithm instance</returns>
		/// <remarks>The algorithm type name format is ClassName,AssemblyName, for example:
		///  "Newtera.Algorithm.MaxValue, Newtera.Algorithm"</remarks>
		public IAlgorithm Create(string typeName)
		{
            lock (this)
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    return null;
                }

                // if the algorithm has been created, return it
                IAlgorithm algorithm = (IAlgorithm)_table[typeName];

                if (algorithm == null)
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
                            // try to create a algorithm from loaded assembly first
                            obj = Activator.CreateInstance(assemblyName, className);
                            algorithm = (IAlgorithm)obj.Unwrap();
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
                            algorithm = (IAlgorithm)obj.Unwrap();
                        }

                        // keep for reuse
                        _table[typeName] = algorithm;
                    }
                }

                return algorithm;
            }
		}
	}
}