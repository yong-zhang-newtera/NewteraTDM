
namespace Newtera.MLStudio.Utilities
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Threading;

    using Newtera.MLStudio.ViewModel;

	/// <summary>
	/// A singleton class that creates a connection string for accessing web services.
	/// </summary>
	public class ConnectionStringBuilder
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ConnectionStringBuilder theBuilder;

		static ConnectionStringBuilder()
		{
			theBuilder = new ConnectionStringBuilder();
		}

		/// <summary>
		/// Gets the ConnectionStringBuilder instance.
		/// </summary>
		/// <returns> The ConnectionStringBuilder instance.</returns>
		static public ConnectionStringBuilder Instance
		{
			get
			{
				return theBuilder;
			}
		}
		
		/// <summary>
		/// Creates a connection string to access a schema via web services
		/// </summary>
		/// <param name="schemaInfo">The schema info to be accessed.</param>
		/// <returns>A connection string</returns>
		public string Create(SchemaInfo schemaInfo)
		{
			StringBuilder builder = new StringBuilder();
		    
			builder.Append("SCHEMA_NAME=").Append(schemaInfo.Name).Append(";SCHEMA_VERSION=").Append(schemaInfo.Version);

			return builder.ToString();
		}
	}
}