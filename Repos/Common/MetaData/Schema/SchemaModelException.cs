/*
* @(#)SchemaModelException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// The SchemaModelException class is the default exception type for metadata 
	/// package. It is highly recommended that a new exception class is defined 
	/// and subclassed from this exception class for each specific error that 
	/// might occur in program of this namespace.
	/// 
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
    public class SchemaModelException : NewteraException
	{
		/// <summary>
		/// Initializing a SchemaModelException object
		/// </summary>
		/// <param name="reason">a description of the exception</param>
		public SchemaModelException(string reason):base(reason)
		{
		}
		
		/// <summary>
		/// Initializing a SchemaModelException object
		/// </summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// <param name="ex">the root cause exception</param>
		///  
		public SchemaModelException(string reason, Exception ex):base(reason, ex)
		{
		}
	}

	/// <summary> The exception that is thrown when there is a formatting error in
	/// a schema model. 
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class SchemaFormattingException : SchemaModelException
	{
		/// <summary> Initializing a SchemaFormattingException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		public SchemaFormattingException(string reason) : base(reason)
		{
		}
	}

	/// <summary> The exception that is thrown when attempts to read a schema
	/// failed. 
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class ReadSchemaException : SchemaModelException
	{	
		/// <summary> Initializing a ReadSchemaException object</summary>
		/// 
		/// <param name="ex">the root exception</param>
		/// <param name="reason">a description of the exception</param>
		/// 
		public ReadSchemaException(string reason, Exception ex):base(reason, ex)
		{
		}
	}

	/// <summary> The exception that is thrown when attempts to read a schema
	/// failed. 
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class WriteSchemaException : SchemaModelException
	{	
		/// <summary> Initializing a WriteSchemaException object</summary>
		/// 
		/// <param name="ex">the root exception</param>
		/// <param name="reason">a description of the exception</param>
		/// 
		public WriteSchemaException(string reason, Exception ex) : base(reason, ex)
		{
		}
	}

	/// <summary>The exception that is thrown when attempting to add a class that is
	/// already exists.
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class DuplicateClassNameException : SchemaModelException
	{	
		/// <summary> Initializing a DuplicateClassNameException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public DuplicateClassNameException(string reason) : base(reason)
		{
		}
	}

	/// <summary>The exception that is thrown when attempting to add a constraint that is
	/// already exists.
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class DuplicateConstraintNameException : SchemaModelException
	{	
		/// <summary> Initializing a DuplicateConstraintNameException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public DuplicateConstraintNameException(string reason) : base(reason)
		{
		}
	}

	/// <summary>The exception that is thrown when attempting to add an attribute that is
	/// already exists.
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class DuplicateAttributeNameException : SchemaModelException
	{	
		/// <summary> Initializing a DuplicateAttributeNameException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public DuplicateAttributeNameException(string reason) : base(reason)
		{
		}
	}

	/// <summary>The exception that is thrown when attempting to add a value that is
	/// already exists.
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class DuplicateValueException : SchemaModelException
	{	
		/// <summary> Initializing a DuplicateValueException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public DuplicateValueException(string reason) : base(reason)
		{
		}
	}

	/// <summary>The exception that is thrown when attempting to remove a constraint
	/// that has been referenced.
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class ConstraintUsedException : SchemaModelException
	{	
		/// <summary> Initializing a ConstraintUsedException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public ConstraintUsedException(string reason) : base(reason)
		{
		}
	}

	/// <summary>The exception that is thrown when encounter an invalid class name
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003
	/// </version>
	/// <author>  		Yong Zhang
	/// 
	/// </author>
	public class InvalidClassNameException : SchemaModelException
	{	
		/// <summary> Initializing a InvalidClassNameException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public InvalidClassNameException(string reason) : base(reason)
		{
		}
	}

	/// <summary>The exception that is thrown when the reversed relationship
	/// attribute name is missing.
	/// </summary>
	/// <version>  	1.0.0 25 Jun 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class MissingReversedRelationshipNameException : SchemaModelException
	{	
		/// <summary> Initializing a MissingReversedRelationshipNameException object</summary>
		/// 
		/// <param name="reason">a description of the exception</param>
		/// 
		public MissingReversedRelationshipNameException(string reason) : base(reason)
		{
		}
	}
}