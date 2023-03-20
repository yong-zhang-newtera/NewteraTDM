/*
* @(#)VirtualAttributeValueGeneratorContext.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Xml;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System.Threading;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Generator;

	/// <summary> 
	/// A container for keeping the context of executing a virtual attribute value generator
	/// </summary>
	/// <version> 1.0.0 08 Sep 2016</version>
    public class VirtualAttributeValueGeneratorContext
	{
        private IFormula _formular;
        private InstanceElementWrapper _wrapper;

        public VirtualAttributeValueGeneratorContext(IFormula formular, InstanceElementWrapper wrapper)
        {
            _formular = formular;
            _wrapper = wrapper;
        }

        /// <summary>
        /// gets or sets the function that generates the value of a virtual attribute
        /// </summary>
        public IFormula Formular
        {
            get
            {
                return _formular;
            }
            set
            {
                _formular = value;
            }
        }

        /// <summary>
        /// gets or sets the instance wrapper that provides a xml instance as context for generating the value
        /// </summary>
        public InstanceElementWrapper Wrapper
        {
            get
            {
                return _wrapper;
            }
            set
            {
                _wrapper = value;
            }
        }
    }
}