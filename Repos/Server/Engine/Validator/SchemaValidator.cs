/*
* @(#)SchemaValidator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Validator
{
	using System;
	using System.Xml.Schema;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Perform data validation when insert and update data instances. The validation is
	/// based on the constraints defined schema model. The validator can extended its
	/// validation using the data in the database, such as validating reference constraints,
	/// or uniquness.
	/// </summary>
	/// <version>  	1.0.0 11 Jul 2003 </version>
	/// <author>  Yong Zhang  </author>
	internal class SchemaValidator
	{
		private SchemaModel _schemaModel;

		/// <summary>
		/// Initiating an instance of SchemaValidator
		/// </summary>
		/// <param name="model">The schema model contains constraints</param> 
		public SchemaValidator(SchemaModel model, bool isDbEnabled)
		{
			_schemaModel = model;
		}

		/// <summary>
		/// Perform the validation
		/// </summary>
		/// <param name="instanceDoc">The document representing an instance</param>
		/// <exception cref="ValidationException">thrown if the validation fails</exception>
		public void Validate(VDocument instanceDoc)
		{
		}

		/// <summary>
		/// Schema validation event handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static public void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
		}
	}
}