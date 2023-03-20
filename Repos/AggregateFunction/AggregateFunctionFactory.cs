/*
* @(#)AggregateFunctionFactory.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Function
{
	using System;
    using System.Threading;
	using System.Xml;
	using System.Runtime.Remoting;
	using System.Collections;

	/// <summary>
	/// A singleton class that creates instances of IAggregateFunction
	/// </summary>
	/// <version>1.0.0 12 Dec 2010 </version>
	public class AggregateFunctionFactory
	{
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static AggregateFunctionFactory theFactory;
		
		static AggregateFunctionFactory()
		{
			theFactory = new AggregateFunctionFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private AggregateFunctionFactory()
		{
		}

		/// <summary>
		/// Gets the AggregateFunctionFactory instance.
		/// </summary>
		/// <returns> The AggregateFunctionFactory instance.</returns>
		static public AggregateFunctionFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IAggregateFunction based on a type name of the
		/// exporter.
		/// </summary>
		/// <param name="typeName">The type name</param>
		/// <returns>A IAggregateFunction instance</returns>
		/// <remarks>The function type name format is ClassName,AssemblyName, for example:
		///  "Newtera.Algorithm.MaxFunction, Newtera.Algorithm"</remarks>
		public IAggregateFunction Create(string typeName)
		{
            lock (this)
            {
                IAggregateFunction function = null;

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
                        // try to create a function from loaded assembly first
                        obj = Activator.CreateInstance(assemblyName, className);
                        function = (IAggregateFunction)obj.Unwrap();
                    }
                    catch (Exception)
                    {
                        obj = null;
                    }

                    if (obj == null)
                    {
                        // try to create it from a dll
                        string dllPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + assemblyName + ".dll";
                        obj = Activator.CreateInstanceFrom(dllPath, className);
                        function = (IAggregateFunction)obj.Unwrap();
                    }
                }

                return function;
            }
		}
	}
}