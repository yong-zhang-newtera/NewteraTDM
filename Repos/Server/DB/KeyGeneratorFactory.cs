/*
* @(#) KeyGeneratorFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Collections;
	using System.Data;
	using Newtera.Common.Core;

	/// <summary>
	/// A factory for all KeyGenerators.
	/// </summary>
	/// <version> 	1.0.0	23 Oct 2003 </version>
	/// <author> Yong Zhang </author>
	public class KeyGeneratorFactory
	{		
		// Static factory object, all invokers will use this factory object.
		private static KeyGeneratorFactory theFactory;

        private KeyGenerator _objIdGenerator;
		private KeyGenerator _schemaIdGenerator;
		private KeyGenerator _classIdGenerator;
		private KeyGenerator _attributeIdGenerator;
		private KeyGenerator _transformerIdGenerator;
        private KeyGenerator _projectIdGenerator;
        private KeyGenerator _workflowIdGenerator;
        private KeyGenerator _valueIdGenerator;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private KeyGeneratorFactory()
		{
            _objIdGenerator = null;
			_schemaIdGenerator = null;
			_classIdGenerator = null;
			_attributeIdGenerator = null;
			_transformerIdGenerator = null;
            _projectIdGenerator = null;
            _workflowIdGenerator = null;
            _valueIdGenerator = null;
		}

		/// <summary>
		/// Gets the KeyGeneratorFactory instance.
		/// </summary>
		/// <returns> The KeyGeneratorFactory instance.</returns>
		static public KeyGeneratorFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a specific KeyGenerator.
		/// </summary>
		/// <param name="type">Type of KeyGenerator</param>
		/// <param name="schemaInfo">schema information.</param>
		/// <returns>A KeyGenerator instance.</returns>
		public KeyGenerator Create(KeyGeneratorType type, SchemaInfo schemaInfo)
		{
			KeyGenerator generator = null;
			IDataProvider dataProvider;

            lock (this)
            {
                switch (type)
                {
                    case KeyGeneratorType.ObjId:

                        if (_objIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new ObjIdGenerator(dataProvider);

                            _objIdGenerator = generator;
                        }
                        else
                        {
                            generator = _objIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.SchemaId:

                        if (_schemaIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new SchemaIdGenerator(dataProvider);

                            _schemaIdGenerator = generator;
                        }
                        else
                        {
                            generator = _schemaIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.ClassId:

                        if (_classIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new ClassIdGenerator(dataProvider);

                            _classIdGenerator = generator;
                        }
                        else
                        {
                            generator = _classIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.AttributeId:

                        if (_attributeIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new AttributeIdGenerator(dataProvider);

                            _attributeIdGenerator = generator;
                        }
                        else
                        {
                            generator = _attributeIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.TransformerId:

                        if (_transformerIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new TransformerIdGenerator(dataProvider);

                            _transformerIdGenerator = generator;
                        }
                        else
                        {
                            generator = _transformerIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.ProjectId:

                        if (_projectIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new ProjectIdGenerator(dataProvider);

                            _projectIdGenerator = generator;
                        }
                        else
                        {
                            generator = _projectIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.WorkflowId:

                        if (_workflowIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new WorkflowIdGenerator(dataProvider);

                            _workflowIdGenerator = generator;
                        }
                        else
                        {
                            generator = _workflowIdGenerator;
                        }

                        break;

                    case KeyGeneratorType.ValueId:

                        if (_valueIdGenerator == null)
                        {
                            dataProvider = DataProviderFactory.Instance.Create();

                            generator = new ValueIdGenerator(dataProvider);

                            _valueIdGenerator = generator;
                        }
                        else
                        {
                            generator = _valueIdGenerator;
                        }

                        break;
                }

                return generator;
            }
		}

		static KeyGeneratorFactory()
		{
			// Initializing the factory.
			{
				theFactory = new KeyGeneratorFactory();
			}
		}
	}

	/// <summary>
	/// Specify the types of id generator
	/// </summary>
	public enum KeyGeneratorType
	{
		ObjId,
		SchemaId,
		ClassId,
		AttributeId,
		TransformerId,
        ProjectId,
        WorkflowId,
        ValueId
	}
}